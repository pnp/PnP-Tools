using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using MaxMelcher.QueryLogger.Utils;
using Microsoft.AspNet.SignalR.Client;
using mshtml;
using SearchQueryTool.Helpers;
using SearchQueryTool.Model;
using Path = System.IO.Path;
using ResultItem = SearchQueryTool.Model.ResultItem;
using ADAL = Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace SearchQueryTool
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private enum SearchMethodType
        {
            Query,
            Suggest
        }

        private enum PropertyType
        {
            Managed,
            Crawled
        }

        private const string DefaultSharePointSiteUrl = "http://localhost";
        private const string ConnectionPropsXmlFileName = "connection-props.xml";
        private const string AuthorityUri = "https://login.windows.net/common/oauth2/authorize";
        private ADAL.AuthenticationContext AuthContext = null;

        private SearchQueryRequest _searchQueryRequest;
        private readonly SearchSuggestionsRequest _searchSuggestionsRequest;
        private SearchConnection _searchConnection;
        public SearchResultPresentationSettings SearchPresentationSettings;
        private string _presetAnnotation;
        private SearchResult _searchResults;
        private bool _enableExperimentalFeatures;
        private bool _firstInit = true;

        public SearchPresetList SearchPresets { get; set; }
        private string PresetFolderPath { get; set; }

        public SearchHistory SearchHistory { get; set; }
        internal string HistoryFolderPath { get; set; }
        public History History { get; }

        public SafeObservable<SearchQueryDebug> ObservableQueryCollection { get; set; }

        private readonly object _locker = new object();

        private IHubProxy _hub;
        private HubConnection _hubConnection;

        public MainWindow()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
            _searchQueryRequest = new SearchQueryRequest { SharePointSiteUrl = DefaultSharePointSiteUrl };
            _searchSuggestionsRequest = new SearchSuggestionsRequest { SharePointSiteUrl = DefaultSharePointSiteUrl };
            _searchSuggestionsRequest = new SearchSuggestionsRequest { SharePointSiteUrl = DefaultSharePointSiteUrl };
            _searchConnection = new SearchConnection();

            ObservableQueryCollection = new SafeObservable<SearchQueryDebug>(Dispatcher);

            InitializeComponent();
            InitializeControls();

            HistoryFolderPath = InitDirectoryFromSetting("HistoryFolderPath", @".\History");
            var historyMaxFiles = ReadSettingInt("HistoryMaxFiles", 1000);

            History = new History(this);
            History.PruneHistoryDir(HistoryFolderPath, historyMaxFiles);
            SearchHistory = new SearchHistory(HistoryFolderPath);
            History.RefreshHistoryButtonState();
            History.LoadLatestHistory(HistoryFolderPath);

            // Get setting or use sensible default if not found
            var tmpPath = ReadSetting("PresetsFolderPath");
            if (!Regex.IsMatch(tmpPath, @"^\w", RegexOptions.IgnoreCase) && !tmpPath.StartsWith(@"\"))
            {
                tmpPath = Path.GetFullPath(tmpPath);
            }
            PresetFolderPath = (!string.IsNullOrEmpty(tmpPath)) ? tmpPath : Path.GetFullPath(@".\Presets");
            if (!Directory.Exists(tmpPath))
            {
                try
                {
                    Directory.CreateDirectory(tmpPath);
                }
                catch (Exception)
                {
                }
            }
            LoadSearchPresetsFromFolder(PresetFolderPath);
        }

        private string InitDirectoryFromSetting(string settingName, string defaultPath)
        {
            var tmpPath = String.Empty;
            try
            {
                tmpPath = ReadSetting(settingName);
                tmpPath = (!String.IsNullOrEmpty(tmpPath)) ? tmpPath : Path.GetFullPath(defaultPath);
                if (!Directory.Exists(tmpPath))
                {
                    Directory.CreateDirectory(tmpPath);
                }
            }
            catch (Exception ex)
            {
                StateBarTextBlock.Text = String.Format("Failed to create folder for setting {0}, defaultPath {1}: {2}", settingName, defaultPath, ex.Message);
            }
            return tmpPath;
        }

        private static int ReadSettingInt(string key, int defaultValue)
        {
            int ret;
            try
            {
                ret = Int32.Parse(ConfigurationManager.AppSettings[key]); ;
            }
            catch (Exception)
            {
                ret = defaultValue;
            }
            return ret;
        }

        private void InitializeControls()
        {
            // Default to setting identify as current Windows user identify
            SetCurrentWindowsUserIdentity();

            // Only load connection properties on initial load to avoid overriding settings in preset
            if (_firstInit)
            {
                _searchConnection = LoadSearchConnection();

                // Expand connection box on first startup
                ConnectionExpanderBox.IsExpanded = true;

                _firstInit = false;
            }

            //LoadSearchPresetsFromFolder();
            AnnotatePresetTextBox.Text = _presetAnnotation;
            UpdateRequestUriStringTextBlock();
            UpdateSearchQueryRequestControls(_searchQueryRequest);
            UpdateSearchConnectionControls(_searchConnection);

            //Enable the cross acces to this collection elsewhere - see: http://stackoverflow.com/questions/14336750/upgrading-to-net-4-5-an-itemscontrol-is-inconsistent-with-its-items-source
            DebugGrid.ItemsSource = ObservableQueryCollection;
        }

        static string ReadSetting(string key)
        {
            string ret = null;
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? String.Empty;
                ret = result;
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
            return ret;
        }
        private void UpdateSearchQueryRequestControls(SearchQueryRequest searchQueryRequest)
        {
            QueryTextBox.Text = searchQueryRequest.QueryText;

            EnableStemmingCheckBox.IsChecked = searchQueryRequest.EnableStemming;
            EnablePhoneticCheckBox.IsChecked = searchQueryRequest.EnablePhonetic;
            EnableNicknamesCheckBox.IsChecked = searchQueryRequest.EnableNicknames;
            TrimDuplicatesCheckBox.IsChecked = searchQueryRequest.TrimDuplicates;
            EnableFqlCheckBox.IsChecked = searchQueryRequest.EnableFql;
            EnableQueryRulesCheckBox.IsChecked = searchQueryRequest.EnableQueryRules;
            ProcessBestBetsCheckBox.IsChecked = searchQueryRequest.ProcessBestBets;
            ByPassResultTypesCheckBox.IsChecked = searchQueryRequest.ByPassResultTypes;
            ProcessPersonalFavoritesCheckBox.IsChecked = searchQueryRequest.ProcessPersonalFavorites;
            GenerateBlockRankLogCheckBox.IsChecked = searchQueryRequest.GenerateBlockRankLog;
            IncludeRankDetailCheckBox.IsChecked = searchQueryRequest.IncludeRankDetail;

            StartRowTextBox.Text = (searchQueryRequest.StartRow != null) ? searchQueryRequest.StartRow.ToString() : "0";
            RowsTextBox.Text = (searchQueryRequest.RowLimit != null) ? searchQueryRequest.RowLimit.ToString() : "10";
            RowsPerPageTextBox.Text = (searchQueryRequest.RowsPerPage != null)
                ? searchQueryRequest.RowsPerPage.ToString()
                : "0";

            SelectPropertiesTextBox.Text = searchQueryRequest.SelectProperties;
            RefinersTextBox.Text = searchQueryRequest.Refiners;
            SortListTextBox.Text = searchQueryRequest.SortList;
            HiddenConstraintsTextBox.Text = searchQueryRequest.HiddenConstraints;
            TrimDuplicatesIncludeIdTextBox.Text = searchQueryRequest.TrimDuplicatesIncludeId.ToString();
            HitHighlightedPropertiesTextBox.Text = searchQueryRequest.HitHighlightedProperties;
            PersonalizationDataTextBox.Text = searchQueryRequest.PersonalizationData;
            CultureTextBox.Text = searchQueryRequest.Culture;
            QueryTemplateTextBox.Text = searchQueryRequest.QueryTemplate;
            RefinementFiltersTextBox.Text = searchQueryRequest.RefinementFilters;
            // NB: Avoid setting this. Buggy?
            //RankingModelIdTextBox.Text = _searchQueryRequest.RankingModelId;
            SourceIdTextBox.Text = searchQueryRequest.SourceId;
            CollapseSpecTextBox.Text = searchQueryRequest.CollapseSpecification;
            AppendedQueryPropertiesTextBox.Text = searchQueryRequest.AppendedQueryProperties;
        }

        private SearchMethodType CurrentSearchMethodType
        {
            get
            {
                var selectedTabItem = SearchMethodTypeTabControl.SelectedItem as TabItem;
                if (selectedTabItem == QueryMethodTypeTabItem)
                {
                    // query
                    return SearchMethodType.Query;
                }
                // suggest
                return SearchMethodType.Suggest;
            }
        }

        private HttpMethodType CurrentHttpMethodType
        {
            get
            {
                if (HttpGetMethodRadioButton == null) return HttpMethodType.Get; // default to HTTP Get

                return (HttpGetMethodRadioButton.IsChecked.Value ? HttpMethodType.Get : HttpMethodType.Post);
            }
        }



        #region Event Handlers

        #region Event Handlers for controls common to both query and suggestions

        /// <summary>
        ///     Handles the Click event of the RunButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            string dc = (AuthenticationMethodComboBox.SelectedItem as ComboBoxItem).DataContext as string;
            if (AuthenticationTypeComboBox.SelectedIndex == 1 && dc == "SPOAuth2")
            {
                await AdalLogin(false);
            }

            SearchMethodType currentSelectedSearchMethodType = CurrentSearchMethodType;

            // fire off the query operation
            if (currentSelectedSearchMethodType == SearchMethodType.Query)
            {
                StartSearchQueryRequest();
            }
            else if (currentSelectedSearchMethodType == SearchMethodType.Suggest)
            {
                StartSearchSuggestionRequest();
            }
        }

        /// <summary>
        ///     Handles the LostFocus event of the SharePointSiteUrlTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void SharePointSiteUrlTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string url = SharePointSiteUrlTextBox.Text.Trim();

            try
            {
                Uri testUri = new Uri(url);
            }
            catch (Exception)
            {
                SharePointSiteUrlAlertImage.Visibility = Visibility.Visible;
                return;
            }

            SharePointSiteUrlAlertImage.Visibility = Visibility.Hidden;

            _searchQueryRequest.SharePointSiteUrl = SharePointSiteUrlTextBox.Text.Trim();
            _searchSuggestionsRequest.SharePointSiteUrl = SharePointSiteUrlTextBox.Text.Trim();

            // TODO: Should we reset auth cookies if site url changes? Then we need to detect the history
            // _searchQueryRequest.Cookies = null;
            // _searchSuggestionsRequest.Cookies = null;

            UpdateRequestUriStringTextBlock();
        }

        /// <summary>
        ///     Handles the SelectionChanged event of the SearchMethodTypeTabControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void SearchMethodTypeTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateRequestUriStringTextBlock();
        }

        /// <summary>
        ///     Handles the SelectionChanged event of the AuthenticationTypeComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void AuthenticationTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = AuthenticationTypeComboBox.SelectedIndex;
            if (selectedIndex == 0) // use current user
            {
                if (AuthenticationUsernameTextBox != null)
                    AuthenticationUsernameTextBox.IsEnabled = false;

                if (AuthenticationPasswordTextBox != null)
                    AuthenticationPasswordTextBox.IsEnabled = false;

                if (AuthenticationMethodComboBox != null)
                {
                    AuthenticationMethodComboBox.SelectedIndex = 0;
                    AuthenticationMethodComboBox.IsEnabled = false;
                }

                SetCurrentWindowsUserIdentity();

                _searchQueryRequest.AuthenticationType = AuthenticationType.CurrentUser;
                _searchSuggestionsRequest.AuthenticationType = AuthenticationType.CurrentUser;

                if (UsernameAndPasswordTextBoxContainer != null)
                {
                    UsernameAndPasswordTextBoxContainer.Visibility = Visibility.Visible;
                }
                if (LoginButtonContainer != null)
                {
                    LoginButtonContainer.Visibility = Visibility.Hidden;
                }
                if (AuthenticationMethodComboBox != null)
                    AuthenticationMethodComboBox.IsEnabled = true;
            }
            else if (selectedIndex == 1)
            {
                AuthenticationMethodComboBox.IsEnabled = true;
                AuthenticationUsernameTextBox.IsEnabled = true;
                AuthenticationPasswordTextBox.IsEnabled = true;

                AuthenticationMethodComboBox_SelectionChanged(null, null);
                if (AuthenticationMethodComboBox != null)
                    AuthenticationMethodComboBox.IsEnabled = true;
            }
            else
            {
                //anonymous
                _searchQueryRequest.AuthenticationType = AuthenticationType.Anonymous;
                _searchSuggestionsRequest.AuthenticationType = AuthenticationType.Anonymous;
                if (AuthenticationMethodComboBox != null)
                    AuthenticationMethodComboBox.IsEnabled = false;
            }

            AuthenticationMethodComboBox_SelectionChanged(sender, e);
        }

        private void AuthenticationMethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 0 - Windows
            // 1 - SharePoint Online
            // 2 - Forms-based
            // 3 - Forefront gateway (UAG/TMG)

            //if (this.AuthenticationTypeComboBox.SelectedIndex == 2) return;
            //if (this.AuthenticationMethodComboBox.SelectedIndex == 2) return; //anonymous

            if (AuthenticationTypeComboBox.SelectedIndex == 0)
            {
                return;
            }

            LoginInfo.Visibility = Visibility.Hidden;
            string dc = (AuthenticationMethodComboBox.SelectedItem as ComboBoxItem).DataContext as string;
            if (AuthenticationTypeComboBox.SelectedIndex == 2) dc = "Anonymous";
            if (dc == "WinAuth")
            {
                _searchQueryRequest.AuthenticationType = AuthenticationType.Windows;
                _searchSuggestionsRequest.AuthenticationType = _searchQueryRequest.AuthenticationType;

                UsernameAndPasswordTextBoxContainer.Visibility = Visibility.Visible;
                LoginButtonContainer.Visibility = Visibility.Hidden;
            }
            else if (dc == "SPOAuth")
            {
                _searchQueryRequest.AuthenticationType = AuthenticationType.SPO;
                _searchSuggestionsRequest.AuthenticationType = _searchQueryRequest.AuthenticationType;

                UsernameAndPasswordTextBoxContainer.Visibility = Visibility.Hidden;
                LoginButtonContainer.Visibility = Visibility.Visible;
                LoggedinLabel.Visibility = Visibility.Hidden;
                LoginInfo.Visibility = Visibility.Visible;
            }
            else if (dc == "SPOAuth2")
            {
                _searchQueryRequest.AuthenticationType = AuthenticationType.SPOManagement;
                _searchSuggestionsRequest.AuthenticationType = _searchQueryRequest.AuthenticationType;

                UsernameAndPasswordTextBoxContainer.Visibility = Visibility.Hidden;
                LoginButtonContainer.Visibility = Visibility.Visible;
                LoggedinLabel.Visibility = Visibility.Hidden;
                LoginInfo.Visibility = Visibility.Visible;
            }
            else if (dc == "FormsAuth")
            {
                _searchQueryRequest.AuthenticationType = AuthenticationType.Forms;
                _searchSuggestionsRequest.AuthenticationType = _searchQueryRequest.AuthenticationType;

                UsernameAndPasswordTextBoxContainer.Visibility = Visibility.Visible;
                LoginButtonContainer.Visibility = Visibility.Hidden;
            }
            else if (dc == "ForefrontAuth")
            {
                _searchQueryRequest.AuthenticationType = AuthenticationType.Forefront;
                _searchSuggestionsRequest.AuthenticationType = _searchQueryRequest.AuthenticationType;

                UsernameAndPasswordTextBoxContainer.Visibility = Visibility.Hidden;
                LoginButtonContainer.Visibility = Visibility.Visible;
                LoggedinLabel.Visibility = Visibility.Hidden;
            }
            else if (dc == "Anonymous")
            {
                _searchQueryRequest.AuthenticationType = AuthenticationType.Anonymous;
                _searchSuggestionsRequest.AuthenticationType = _searchQueryRequest.AuthenticationType;

                UsernameAndPasswordTextBoxContainer.Visibility = Visibility.Hidden;
                LoginButtonContainer.Visibility = Visibility.Hidden;
            }
        }

        private void AuthenticationUsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _searchQueryRequest.UserName = AuthenticationUsernameTextBox.Text;
            _searchSuggestionsRequest.UserName = AuthenticationUsernameTextBox.Text;
        }

        private void AuthenticationPasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            AuthenticationUsernameTextBox_LostFocus(sender, e); // trigger setting of username

            _searchQueryRequest.SecurePassword = AuthenticationPasswordTextBox.SecurePassword;
            _searchQueryRequest.Password = AuthenticationPasswordTextBox.Password;

            _searchSuggestionsRequest.SecurePassword = AuthenticationPasswordTextBox.SecurePassword;
            _searchSuggestionsRequest.Password = AuthenticationPasswordTextBox.Password;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoggedinLabel.Visibility = Visibility.Hidden;
            _searchQueryRequest.Cookies = null;
            _searchSuggestionsRequest.Cookies = null;

            try
            {
                SharePointSiteUrlTextBox_LostFocus(sender, e);
                string dc = ((ComboBoxItem)AuthenticationMethodComboBox.SelectedItem).DataContext as string;
                if (dc == "SPOAuth2")
                {
                    try
                    {
                        await AdalLogin(true);
                        if (string.IsNullOrWhiteSpace(_searchQueryRequest.Token)) throw new ApplicationException("No token");
                        LoggedinLabel.Visibility = Visibility.Visible;
                    }
                    catch (Exception exception)
                    {
                        ShowMsgBox(
                            $"Authentication failed. Please try again.\n\n{_searchQueryRequest.SharePointSiteUrl}\n\n{exception.Message}");
                    }
                }
                else
                {
                    AuthContext = null;
                    CookieCollection cc = WebAuthentication.GetAuthenticatedCookies(_searchQueryRequest.SharePointSiteUrl, _searchQueryRequest.AuthenticationType);

                    if (cc == null)
                    {
                        ShowMsgBox(
                            $"Authentication failed. Please try again.\n\n{_searchQueryRequest.SharePointSiteUrl}");
                    }
                    else
                    {
                        LoggedinLabel.Visibility = Visibility.Visible;
                    }
                    _searchQueryRequest.Cookies = cc;
                    _searchSuggestionsRequest.Cookies = cc;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
                ShowMsgBox(
                    $"Authentication failed. Please try again.\n\n{_searchQueryRequest.SharePointSiteUrl}\n\n{ex.Message}");
            }
        }

        async Task AdalLogin(bool forcePrompt)
        {
            var spUri = new Uri(_searchQueryRequest.SharePointSiteUrl);

            string resourceUri = spUri.Scheme + "://" + spUri.Authority;
            const string clientId = "9bc3ab49-b65d-410a-85ad-de819febfddc";
            const string redirectUri = "https://oauth.spops.microsoft.com/";

            Window ownerWindow = Application.Current.MainWindow.Owner;

            ADAL.AuthenticationResult authenticationResult;

            if (AuthContext == null || forcePrompt)
            {
                ADAL.TokenCache cache = new ADAL.TokenCache();
                AuthContext = new ADAL.AuthenticationContext(AuthorityUri, cache);
            }
            try
            {
                if (forcePrompt) throw new ADAL.AdalSilentTokenAcquisitionException();
                authenticationResult = await AuthContext.AcquireTokenSilentAsync(resourceUri, clientId);
            }
            catch (ADAL.AdalSilentTokenAcquisitionException)
            {
                var authParam = new ADAL.PlatformParameters(ADAL.PromptBehavior.Always, ownerWindow);
                authenticationResult = await AuthContext.AcquireTokenAsync(resourceUri, clientId, new Uri(redirectUri), authParam);
            }

            _searchQueryRequest.Token = authenticationResult.CreateAuthorizationHeader();
            _searchSuggestionsRequest.Token = authenticationResult.CreateAuthorizationHeader();
        }

        #endregion

        #region Event Handlers for Controls on the Search Query Tab

        /// <summary>
        ///     Handles the Handler event of the SearchQueryTextBox_LostFocus control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void SearchQueryTextBox_LostFocus_Handler(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                string dataContext = (tb.DataContext as string) ?? "";
                switch (dataContext.ToLower())
                {
                    case "query":
                        _searchQueryRequest.QueryText = tb.Text.Trim();
                        break;
                    case "querytemplate":
                        _searchQueryRequest.QueryTemplate = tb.Text.Trim();
                        break;
                    case "selectproperties":
                        _searchQueryRequest.SelectProperties = tb.Text.Trim();
                        break;
                    case "refiners":
                        _searchQueryRequest.Refiners = tb.Text.Trim();
                        break;
                    case "refinementfilters":
                        _searchQueryRequest.RefinementFilters = tb.Text.Trim();
                        break;
                    case "trimduplicatesincludeid":
                        _searchQueryRequest.TrimDuplicatesIncludeId = DataConverter.TryConvertToLong(tb.Text.Trim());
                        break;
                    case "sortlist":
                        _searchQueryRequest.SortList = tb.Text.Trim();
                        break;
                    case "hithighlightedproperties":
                        _searchQueryRequest.HitHighlightedProperties = tb.Text.Trim();
                        break;
                    case "rankingmodelid":
                        _searchQueryRequest.RankingModelId = tb.Text.Trim();
                        break;
                    case "hiddenconstraints":
                        _searchQueryRequest.HiddenConstraints = tb.Text.Trim();
                        break;
                    case "personalizationdata":
                        _searchQueryRequest.PersonalizationData = tb.Text.Trim();
                        break;
                    case "resultsurl":
                        _searchQueryRequest.ResultsUrl = tb.Text.Trim();
                        break;
                    case "querytag":
                        _searchQueryRequest.QueryTag = tb.Text.Trim();
                        break;
                    case "collapsespecification":
                        _searchQueryRequest.CollapseSpecification = tb.Text.Trim();
                        break;
                    case "startrow":
                        _searchQueryRequest.StartRow = DataConverter.TryConvertToInt(tb.Text.Trim());
                        break;
                    case "rows":
                        _searchQueryRequest.RowLimit = DataConverter.TryConvertToInt(tb.Text.Trim());
                        break;
                    case "rowsperpage":
                        _searchQueryRequest.RowsPerPage = DataConverter.TryConvertToInt(tb.Text.Trim());
                        break;
                    case "appendedqueryproperties":
                        _searchQueryRequest.AppendedQueryProperties = tb.Text.Trim();
                        break;
                }

                UpdateRequestUriStringTextBlock();
            }

            if (sender is ComboBox cb)
            {
                string dataContext = (cb.DataContext as string) ?? "";
                switch (dataContext.ToLower())
                {
                    case "sourceid":
                        {
                            string sourceId = cb.Text;
                            ComboBoxItem comboBoxItem = cb.SelectedItem as ComboBoxItem;
                            if (comboBoxItem?.Tag != null)
                            {
                                sourceId = comboBoxItem.Tag as string;
                            }

                            if (sourceId != null)
                            {
                                _searchQueryRequest.SourceId = System.Web.HttpUtility.UrlDecode(sourceId.Trim());
                            }

                            cb.Text = _searchQueryRequest.SourceId;
                            break;
                        }
                    case "culture":
                        {
                            string culture = cb.Text;
                            ComboBoxItem comboBoxItem = cb.SelectedItem as ComboBoxItem;
                            if (comboBoxItem?.Tag != null)
                            {
                                culture = comboBoxItem.Tag as string;
                            }

                            if (culture != null)
                            {
                                _searchQueryRequest.Culture = culture;
                            }

                            cb.Text = _searchQueryRequest.Culture;
                            break;
                        }
                }

                UpdateRequestUriStringTextBlock();
            }
        }

        /// <summary>
        ///     Handles the CheckChanged event of the SearchQueryCheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void SearchQueryCheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb)
            {
                string datacontext = (cb.DataContext as string) ?? "";
                switch (datacontext.ToLower())
                {
                    case "enablestemming":
                        _searchQueryRequest.EnableStemming = cb.IsChecked;
                        break;
                    case "enablequeryrules":
                        _searchQueryRequest.EnableQueryRules = cb.IsChecked;
                        break;
                    case "enablenicknames":
                        _searchQueryRequest.EnableNicknames = cb.IsChecked;
                        break;
                    case "processbestbets":
                        _searchQueryRequest.ProcessBestBets = cb.IsChecked;
                        break;
                    case "trimduplicates":
                        _searchQueryRequest.TrimDuplicates = cb.IsChecked;
                        break;
                    case "enablefql":
                        _searchQueryRequest.EnableFql = cb.IsChecked;
                        break;
                    case "enablephonetic":
                        _searchQueryRequest.EnablePhonetic = cb.IsChecked;
                        break;
                    case "bypassresulttypes":
                        _searchQueryRequest.ByPassResultTypes = cb.IsChecked;
                        break;
                    case "processpersonalfavorites":
                        _searchQueryRequest.ProcessPersonalFavorites = cb.IsChecked;
                        break;
                    case "generateblockranklog":
                        _searchQueryRequest.GenerateBlockRankLog = cb.IsChecked;
                        break;
                    case "includerankdetail":
                        _searchQueryRequest.IncludeRankDetail = cb.IsChecked;
                        break;
                    case "experimentalfeatures":
                        _enableExperimentalFeatures = cb.IsChecked.Value;
                        break;
                }

                UpdateRequestUriStringTextBlock();
            }
        }

        /// <summary>
        ///     Handles the SelectionChanged event of the QueryLogClientTypeComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void QueryLogClientTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedValue = ((ComboBoxItem)QueryLogClientTypeComboBox.SelectedItem).DataContext as string;
            _searchQueryRequest.ClientType = selectedValue;

            UpdateRequestUriStringTextBlock();
        }

        private void ResetCheckboxesButton_Click(object sender, RoutedEventArgs e)
        {
            _searchQueryRequest.EnableStemming = null;
            EnableStemmingCheckBox.IsChecked = null;

            _searchQueryRequest.EnableQueryRules = null;
            EnableQueryRulesCheckBox.IsChecked = null;

            _searchQueryRequest.EnableNicknames = null;
            EnableNicknamesCheckBox.IsChecked = null;

            _searchQueryRequest.ProcessBestBets = null;
            ProcessBestBetsCheckBox.IsChecked = null;

            _searchQueryRequest.TrimDuplicates = null;
            TrimDuplicatesCheckBox.IsChecked = null;

            _searchQueryRequest.EnableFql = null;
            EnableFqlCheckBox.IsChecked = null;

            _searchQueryRequest.EnablePhonetic = null;
            EnablePhoneticCheckBox.IsChecked = null;

            _searchQueryRequest.ByPassResultTypes = null;
            ByPassResultTypesCheckBox.IsChecked = null;

            _searchQueryRequest.ProcessPersonalFavorites = null;
            ProcessPersonalFavoritesCheckBox.IsChecked = null;

            _searchQueryRequest.GenerateBlockRankLog = null;
            GenerateBlockRankLogCheckBox.IsChecked = null;

            UpdateRequestUriStringTextBlock();
        }

        private void HttpMethodModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateRequestUriStringTextBlock();
        }

        #endregion

        #region Event Handlers for Controls on the Search Suggestion Tab

        private void SearchSuggestionsTextBox_LostFocus_Handler(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                string dataContext = (tb.DataContext as string) ?? "";
                switch (dataContext.ToLower())
                {
                    case "query":
                        _searchSuggestionsRequest.QueryText = tb.Text.Trim();
                        break;
                    case "numberofquerysuggestions":
                        _searchSuggestionsRequest.NumberOfQuerySuggestions = DataConverter.TryConvertToInt(tb.Text.Trim());
                        break;
                    case "numberofresultsuggestions":
                        _searchSuggestionsRequest.NumberOfResultSuggestions = DataConverter.TryConvertToInt(tb.Text.Trim());
                        break;
                }

                UpdateRequestUriStringTextBlock();
            }

            if (sender is ComboBox cb)
            {
                string dataContext = (cb.DataContext as string) ?? "";
                switch (dataContext.ToLower())
                {
                    case "suggestionsculture":
                        {
                            string suggestionsCulture = cb.Text;
                            if (cb.SelectedItem is ComboBoxItem comboBoxItem && comboBoxItem.Tag != null)
                            {
                                suggestionsCulture = comboBoxItem.Tag as string;
                            }
                            if (!string.IsNullOrWhiteSpace(suggestionsCulture))
                            {
                                _searchSuggestionsRequest.Culture = DataConverter.TryConvertToInt(suggestionsCulture);
                                cb.Text = _searchSuggestionsRequest.Culture.ToString();
                            }
                            else
                            {
                                _searchSuggestionsRequest.Culture = null;
                                cb.Text = "";
                            }
                            break;
                        }
                }

                UpdateRequestUriStringTextBlock();
            }
        }

        private void SearchSuggestionsCheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb)
            {
                string datacontext = (cb.DataContext as string) ?? "";
                switch (datacontext.ToLower())
                {
                    case "prequerysuggestions":
                        _searchSuggestionsRequest.PreQuerySuggestions = cb.IsChecked;
                        break;
                    case "showpeoplenamesuggestions":
                        _searchSuggestionsRequest.ShowPeopleNameSuggestions = cb.IsChecked;
                        break;
                    case "hithighlighting":
                        _searchSuggestionsRequest.HitHighlighting = cb.IsChecked;
                        break;
                    case "capitalizefirstletters":
                        _searchSuggestionsRequest.CapitalizeFirstLetters = cb.IsChecked;
                        break;
                }

                UpdateRequestUriStringTextBlock();
            }
        }

        #endregion

        #region Event Handlers for Menu controls

        /// <summary>
        ///     Handles the Click event of the PasteExampleButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void PasteExampleButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source != null)
            {
                object d = (e.Source as Button).DataContext;
                if (d != null && d is String)
                {
                    string senderDatacontext = d as string;
                    if (!string.IsNullOrWhiteSpace(senderDatacontext))
                    {
                        string exampleString = SampleStrings.GetExampleStringFor(senderDatacontext);
                        if (!string.IsNullOrWhiteSpace(exampleString))
                        {
                            switch (senderDatacontext.ToLower())
                            {
                                case "rankingmodelid":
                                    RankingModelIdTextBox.Text = exampleString;
                                    RankingModelIdTextBox.Focus();
                                    break;
                                case "hiddenconstraints":
                                    HiddenConstraintsTextBox.Text = exampleString;
                                    HiddenConstraintsTextBox.Focus();
                                    break;
                                case "selectproperties":
                                    SelectPropertiesTextBox.Text = exampleString;
                                    SelectPropertiesTextBox.Focus();
                                    break;

                                case "refiners":
                                    RefinersTextBox.Text = exampleString;
                                    RefinersTextBox.Focus();
                                    break;
                                case "refinementfilters":
                                    RefinementFiltersTextBox.Text = exampleString;
                                    RefinementFiltersTextBox.Focus();
                                    break;
                                case "sortlist":
                                    SortListTextBox.Text = exampleString;
                                    SortListTextBox.Focus();
                                    break;
                                case "sourceid":
                                    SourceIdTextBox.Text = exampleString;
                                    SourceIdTextBox.Focus();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }


        private void CleanSelectProperties_Click(object sender, RoutedEventArgs e)
        {
            var dirtyProperties = SelectPropertiesTextBox.Text;
            if (!string.IsNullOrWhiteSpace(dirtyProperties))
            {
                var dirtyPropertyList = dirtyProperties.Split(',');
                if (dirtyPropertyList != null && dirtyPropertyList.Length > 0)
                {
                    var cleanList = dirtyPropertyList.Distinct().ToList();
                    cleanList.Sort();
                    var cleanProperties = String.Join(",", cleanList);
                    SelectPropertiesTextBox.Text = cleanProperties;
                    SelectPropertiesTextBox.Focus();
                }
            }
        }

        private void MenuSaveConnectionProperties_Click(object sender, RoutedEventArgs e)
        {
            // Create a connection object with all data from the user interface
            var connection = GetSearchConnectionFromUi();

            // Store to disk as XML
            try
            {
                var outputPath = Path.Combine(Environment.CurrentDirectory, ConnectionPropsXmlFileName);
                connection.SaveXml(outputPath);
                StateBarTextBlock.Text = $"Successfully saved connection properties to {Path.GetFullPath(outputPath)}";
            }
            catch (Exception ex)
            {
                ShowMsgBox("Failed to save connection properties. Error:" + ex.Message);
            }
        }

        /// <summary>
        ///     Handles the Click event of the MenuFileExit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MenuFileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Handles the Click event of the MenuAbout control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            new AboutBox().Show();
        }

        #endregion

        #endregion

        #region Request Building methods and Helpers

        /// <summary>
        ///     Starts the search query request.
        /// </summary>
        private void StartSearchQueryRequest()
        {
            string status = "Init";
            string queryText = QueryTextBox.Text.Trim();
            if (string.IsNullOrEmpty(queryText))
            {
                QueryTextBox.Focus();
                return;
            }
            try
            {
                _searchQueryRequest.QueryText = queryText;
                UpdateRequestUriStringTextBlock();

                status = "Running";
                MarkRequestOperation(true, status);
                RequestStarted();

                _searchQueryRequest.HttpMethodType = CurrentHttpMethodType;
                _searchQueryRequest.AcceptType = AcceptJsonRadioButton.IsChecked.Value
                    ? AcceptType.Json
                    : AcceptType.Xml;

                //todo this should be split to several methods so we can reuse them
                Task.Factory.StartNew(() => HttpRequestRunner.RunWebRequest(_searchQueryRequest),
                    TaskCreationOptions.LongRunning)
                    .ContinueWith(task =>
                    {
                        if (task.Exception != null)
                        {
                            RequestFailed();
                            ShowError(task.Exception);
                        }
                        else
                        {
                            var requestResponsePair = task.Result;
                            if (requestResponsePair != null)
                            {
                                var response = requestResponsePair.Item2;
                                if (null != response)
                                {
                                    if (response.StatusCode.Equals(HttpStatusCode.OK))
                                    {
                                        RequestSuccessful();
                                        status = "Done";
                                    }
                                    else
                                    {
                                        RequestFailed();
                                        status = String.Format("HTTP {0}, {1}", response.StatusCode, response.StatusDescription);
                                    }
                                }
                            }
                            var searchResults = GetResultItem(requestResponsePair);
                            _searchResults = searchResults;

                            // set status
                            SetHitStatus(searchResults);

                            // set the result
                            SetStatsResult(searchResults);
                            SetRawResult(searchResults);
                            SetPrimaryQueryResultItems(searchResults);
                            SetRefinementResultItems(searchResults);
                            SetSecondaryQueryResultItems(searchResults);
                        }

                    }, TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith(task =>
                    {
                        if (task.Exception != null)
                        {
                            ShowError(task.Exception);
                        }
                        MarkRequestOperation(false, status);
                    });
            }
            catch (Exception ex)
            {
                MarkRequestOperation(false, status);
                RequestFailed();
                ShowError(ex);
            }
        }

        private void SetHitStatus(SearchQueryResult searchResults)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (null != searchResults)
                {
                    var totalRows = 0;
                    var queryElapsedTime = "0";
                    if (null != searchResults.QueryElapsedTime)
                    {
                        queryElapsedTime = searchResults.QueryElapsedTime;
                    }

                    if (null != searchResults.PrimaryQueryResult)
                    {
                        totalRows = searchResults.PrimaryQueryResult.TotalRows;
                    }
                    HitStatusTextBlock.Text = String.Format("{0} hits in {1} ms", totalRows, queryElapsedTime);
                }
            });
        }

        private void RequestStarted()
        {
            this.Dispatcher.Invoke(() =>
            {
                ConnectionExpanderBox.Foreground = Brushes.Purple;
                HitStatusTextBlock.Text = "...";
            });
        }

        private void RequestSuccessful()
        {
            this.Dispatcher.Invoke(() =>
            {
                ConnectionExpanderBox.IsExpanded = false;
                ConnectionExpanderBox.Foreground = Brushes.Green;
                RequestUriLengthTextBox.Foreground = Brushes.Gray;
                HitStatusTextBlock.Foreground = Brushes.Black;

                // Save this successful item to our history
                History.SaveHistoryItem();

                // Reload entire history list and reset the current point to the latest entry
                SearchHistory = new SearchHistory(HistoryFolderPath);
                History.RefreshHistoryButtonState();

                RefreshBreadCrumbs();
            });
        }

        private void RequestFailed()
        {
            this.Dispatcher.Invoke(() =>
            {
                ConnectionExpanderBox.Foreground = Brushes.Red;
                HitStatusTextBlock.Foreground = Brushes.Red;
                RequestUriLengthTextBox.Foreground = Brushes.Red;
            });
        }

        private Button GetHiddenConstraintsButton(string text)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
            };

            var uriSource = new Uri(@"/SearchQueryTool;component/Images/remove.png", UriKind.Relative);
            //var uriSource = new Uri(@"/Resources;Images/remove.png", UriKind.Relative);
            //var uriSource = new Uri(@"/Resources;remove.png", UriKind.Relative);

            //var img = new BitmapImage(uriSource);
            //var img = new BitmapImage(new Uri("Images/alert_icon.png", UriKind.Relative));
            var img = new BitmapImage(new Uri("Images/remove.png", UriKind.Relative));

            //var icon = new Image() { Source = new BitmapImage(uriSource), Margin = new Thickness(5, 0, 5, 0) };
            var icon = new Image() { Source = img, Margin = new Thickness(5, 0, 5, 0) };

            panel.Children.Add(icon);
            panel.Children.Add(new TextBlock() { Text = text });
            var button = new Button
            {
                Margin = new Thickness(5),
                Content = panel,
            };

            button.Click += HiddenConstraintsRemoveButton_OnClick;

            return button;
        }

        private static List<string> ParseHiddenConstraints(string input)
        {
            var ret = new List<string>();

            if (string.IsNullOrWhiteSpace(input)) return ret;
            var regex = new Regex(@"(?<key>\(*\w+)[:|=<>](?<value>""[\S\s]+""\)*|\w+\)*)");

            try
            {
                var buf = input;
                var match = regex.Match(buf);
                while (match.Success)
                {
                    var m = match.Captures[0].ToString();
                    buf = buf.Replace(m, "").Trim();
                    ret.Add(m);
                    match = regex.Match(buf);
                }
            }
            catch
            {
                // ignore
            }
            return ret;
        }

        private void RefreshBreadCrumbs()
        {
            this.Dispatcher.Invoke(() =>
            {
                // Clear old hidden constraints
                HiddenConstraintWrapPanel.Children.Clear();

                // ParseHiddenConstraints hidden constraints and populate panel with new breadcrumb buttons
                var hiddenConstraints = HiddenConstraintsTextBox.Text;

                if (string.IsNullOrWhiteSpace(hiddenConstraints))
                {
                    // Initialize state when we have no hidden constraints
                    ExpanderHiddenConstraints.Header = String.Format("Hidden Constraints");
                    BreadCrumbDockPanel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // Extract patterns like with quotes and space, e.g.: ContentSource:"Local SharePoint"
                    var list = ParseHiddenConstraints(hiddenConstraints);
                    if (list.Any())
                    {
                        ExpanderHiddenConstraints.Header = String.Format("Hidden Constraints ({0})", list.Count());
                        foreach (var button in list.Select(GetHiddenConstraintsButton))
                        {
                            HiddenConstraintWrapPanel.Children.Add(button);
                        }
                        BreadCrumbDockPanel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        // Hide breadcrumbs when there are no matches
                        BreadCrumbDockPanel.Visibility = Visibility.Collapsed;
                    }
                }
            });
        }

        private void HiddenConstraintsRemoveButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Read text from button
            var btn = sender as Button;
            if (btn != null)
            {
                var btnPanel = btn.Content as StackPanel;
                if (btnPanel != null)
                {
                    var textBlock = btnPanel.Children.OfType<TextBlock>().FirstOrDefault();
                    if (textBlock != null)
                    {
                        var text = textBlock.Text;

                        // Remove text from HiddenConstraints
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            HiddenConstraintsTextBox.Text = HiddenConstraintsTextBox.Text.Replace(text.Trim(), "").Trim();

                            // Fire focus lost event to refresh the breadcrumbs (i.e. should remove the button we clicked)
                            RefreshBreadCrumbs();
                        }
                    }
                }
            }
        }

        private SearchQueryResult GetResultItem(HttpRequestResponsePair requestResponsePair)
        {
            SearchQueryResult searchResults;
            var request = requestResponsePair.Item1;

            using (var response = requestResponsePair.Item2)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    NameValueCollection requestHeaders = new NameValueCollection();
                    foreach (var header in request.Headers.AllKeys)
                    {
                        requestHeaders.Add(header, request.Headers[header]);
                    }

                    NameValueCollection responseHeaders = new NameValueCollection();
                    foreach (var header in response.Headers.AllKeys)
                    {
                        responseHeaders.Add(header, response.Headers[header]);
                    }

                    string requestContent = "";
                    if (request.Method == "POST")
                    {
                        requestContent = requestResponsePair.Item3;
                    }

                    searchResults = new SearchQueryResult
                    {
                        RequestUri = request.RequestUri,
                        RequestMethod = request.Method,
                        RequestContent = requestContent,
                        ContentType = response.ContentType,
                        ResponseContent = content,
                        RequestHeaders = requestHeaders,
                        ResponseHeaders = responseHeaders,
                        StatusCode = response.StatusCode,
                        StatusDescription = response.StatusDescription,
                        HttpProtocolVersion = response.ProtocolVersion.ToString()
                    };
                    searchResults.Process();
                }
            }
            return searchResults;
        }


        /// <summary>
        ///     Starts the search suggestion request.
        /// </summary>
        private void StartSearchSuggestionRequest()
        {
            string queryText = SuggestionsQueryTextBox.Text.Trim();
            if (string.IsNullOrEmpty(queryText))
            {
                SuggestionsQueryTextBox.Focus();
                return;
            }

            _searchSuggestionsRequest.QueryText = queryText;
            UpdateRequestUriStringTextBlock();

            try
            {
                MarkRequestOperation(true, "Running");

                _searchSuggestionsRequest.HttpMethodType = HttpMethodType.Get;
                // Only get is supported for suggestions
                _searchSuggestionsRequest.AcceptType = AcceptJsonRadioButton.IsChecked.Value
                    ? AcceptType.Json
                    : AcceptType.Xml;

                _searchSuggestionsRequest.Cookies = _searchQueryRequest.Cookies;
                _searchSuggestionsRequest.UserName = _searchQueryRequest.UserName;
                _searchSuggestionsRequest.Password = _searchQueryRequest.Password;
                _searchSuggestionsRequest.SecurePassword = _searchQueryRequest.SecurePassword;


                Task.Factory.StartNew(() => { return HttpRequestRunner.RunWebRequest(_searchSuggestionsRequest); },
                    TaskCreationOptions.LongRunning)
                    .ContinueWith(task =>
                    {
                        MarkRequestOperation(false, "Done");
                        if (task.Exception != null)
                        {
                            ShowError(task.Exception);
                        }
                        else
                        {
                            var requestResponsePair = task.Result;
                            var request = requestResponsePair.Item1;

                            using (var response = requestResponsePair.Item2)
                            {
                                using (var reader = new StreamReader(response.GetResponseStream()))
                                {
                                    var content = reader.ReadToEnd();

                                    NameValueCollection requestHeaders = new NameValueCollection();
                                    foreach (var header in request.Headers.AllKeys)
                                    {
                                        requestHeaders.Add(header, request.Headers[header]);
                                    }

                                    NameValueCollection responseHeaders = new NameValueCollection();
                                    foreach (var header in response.Headers.AllKeys)
                                    {
                                        responseHeaders.Add(header, response.Headers[header]);
                                    }

                                    var searchResults = new SearchSuggestionsResult
                                    {
                                        RequestUri = request.RequestUri,
                                        RequestMethod = request.Method,
                                        ContentType = response.ContentType,

                                        ResponseContent = content,
                                        RequestHeaders = requestHeaders,
                                        ResponseHeaders = responseHeaders,
                                        StatusCode = response.StatusCode,
                                        StatusDescription = response.StatusDescription,
                                        HttpProtocolVersion = response.ProtocolVersion.ToString()
                                    };
                                    searchResults.Process();

                                    // set the result
                                    SetStatsResult(searchResults);
                                    SetRawResult(searchResults);
                                    SetSuggestionsResultItems(searchResults);
                                }
                            }
                        }

                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                // log
                ShowError(ex);
            }
            finally
            {
                MarkRequestOperation(false, "Done");
            }
        }

        /// <summary>
        ///     Creates and populates the the Statistics tab from data from the passed in <paramref name="searchResult" />.
        ///     This method is used for both query and suggestions.
        /// </summary>
        /// <param name="searchResult">The search result.</param>
        private void SetStatsResult(SearchResult searchResult)
        {
            ScrollViewer sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            TextBox tb = new TextBox
            {
                BorderBrush = null,
                IsReadOnly = true,
                IsReadOnlyCaretVisible = false,
                TextWrapping = TextWrapping.WrapWithOverflow
            };

            tb.AppendText(
                $"HTTP/{searchResult.HttpProtocolVersion} {(int)searchResult.StatusCode} {searchResult.StatusDescription}\n");
            if (searchResult.StatusCode != HttpStatusCode.OK)
            {
                tb.AppendText(searchResult.ResponseContent);
            }

            if (searchResult is SearchQueryResult)
            {
                var searchQueryResult = searchResult as SearchQueryResult;

                if (!string.IsNullOrEmpty(searchQueryResult.SerializedQuery))
                    tb.AppendText($"\tSerialized Query:\n{searchQueryResult.SerializedQuery}\n\n");

                if (!string.IsNullOrEmpty(searchQueryResult.QueryElapsedTime))
                    tb.AppendText($"\tElapsed Time (ms): {searchQueryResult.QueryElapsedTime}\n\n");

                if (searchQueryResult.TriggeredRules != null && searchQueryResult.TriggeredRules.Count > 0)
                {
                    tb.AppendText("\tTriggered Rules:\n");

                    foreach (var rule in searchQueryResult.TriggeredRules)
                    {
                        tb.AppendText($"\t\tQuery Rule Id: {rule}\n");
                    }
                    tb.AppendText("\n");
                }

                if (searchQueryResult.PrimaryQueryResult != null)
                {
                    tb.AppendText("\tPrimary Query Results:\n");
                    tb.AppendText($"\t\tTotal Rows: {searchQueryResult.PrimaryQueryResult.TotalRows}\n");
                    tb.AppendText(
                        $"\t\tTotal Rows Including Duplicates: {searchQueryResult.PrimaryQueryResult.TotalRowsIncludingDuplicates}\n");
                    tb.AppendText($"\t\tQuery Id: {searchQueryResult.PrimaryQueryResult.QueryId}\n");
                    tb.AppendText($"\t\tQuery Rule Id: {searchQueryResult.PrimaryQueryResult.QueryRuleId}\n");
                    tb.AppendText($"\t\tQuery Modification: {searchQueryResult.PrimaryQueryResult.QueryModification}\n");

                }

                if (searchQueryResult.SecondaryQueryResults != null)
                {
                    tb.AppendText("\n");
                    tb.AppendText("\tSecondary Query Results:\n");

                    foreach (var sqr in searchQueryResult.SecondaryQueryResults)
                    {
                        tb.AppendText(String.Format("\t\tSecondary Query Result {0}\n", sqr.QueryRuleId));
                        tb.AppendText(String.Format("\t\tTotal Rows: {0}\n", sqr.TotalRows));
                        tb.AppendText(String.Format("\t\tTotal Rows Including Duplicates: {0}\n",
                            sqr.TotalRowsIncludingDuplicates));
                        tb.AppendText(String.Format("\t\tQuery Id: {0}\n", sqr.QueryId));
                        tb.AppendText(String.Format("\t\tQuery Rule Id: {0}\n", sqr.QueryRuleId));
                        tb.AppendText(String.Format("\t\tQuery Modification: {0}\n", sqr.QueryModification));
                    }
                }
            }

            sv.Content = tb;
            StatsResultTabItem.Content = sv;
        }

        /// <summary>
        ///     Creates and populates the the Headers tab from data from the passed in <paramref name="requestHeaders" /> and
        ///     <paramref name="responseHeaders" />.
        ///     This method is used for both query and suggestions.
        /// </summary>
        /// <param name="searchResult">The search result.</param>
        private void SetRawResult(SearchResult searchResult)
        {
            ScrollViewer sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            TextBox tb = new TextBox
            {
                BorderBrush = null,
                IsReadOnly = true,
                IsReadOnlyCaretVisible = false,
                FontSize = 12
            };

            tb.AppendText(String.Format("{0}\t{1}\tHTTP {2}\n\n", searchResult.RequestMethod, searchResult.RequestUri,
                searchResult.HttpProtocolVersion));

            tb.AppendText("Request:" + Environment.NewLine);
            foreach (var header in searchResult.RequestHeaders.AllKeys)
            {
                tb.AppendText(String.Format("\t{0}: {1}{2}", header, searchResult.RequestHeaders[header],
                    Environment.NewLine));
            }
            tb.AppendText("\n\t" + searchResult.RequestContent + "\n");
            tb.AppendText("\n\n");

            tb.AppendText("Response:\n");
            tb.AppendText(String.Format("\tHTTP/{0} {1} {2}\n", searchResult.HttpProtocolVersion,
                (int)searchResult.StatusCode, searchResult.StatusDescription));
            foreach (var header in searchResult.ResponseHeaders.AllKeys)
            {
                tb.AppendText(String.Format("\t{0}: {1}{2}", header, searchResult.ResponseHeaders[header],
                    Environment.NewLine));
            }

            if (AcceptJsonRadioButton.IsChecked.HasValue && AcceptJsonRadioButton.IsChecked.Value)
            {
                searchResult.ResponseContent = JsonHelper.FormatJson(searchResult.ResponseContent);
            }
            else
            {
                searchResult.ResponseContent = XmlHelper.PrintXml(searchResult.ResponseContent);
            }

            tb.AppendText("\n\t" + searchResult.ResponseContent + "\n");

            sv.Content = tb;
            RawResultTabItem.Content = sv;
        }

        /// <summary>
        ///     Creates and populates the the Primary results tab from data from the passed in <paramref name="searchResult" />.
        ///     This method is used for only query results.
        /// </summary>
        /// <param name="searchResult">The search result.</param>
        private void SetPrimaryQueryResultItems(SearchQueryResult searchResult)
        {
            ScrollViewer sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            if (searchResult.PrimaryQueryResult != null && searchResult.PrimaryQueryResult.RelevantResults != null /*&&
                searchResult.PrimaryQueryResult.TotalRows > 0*/)
            {
                StackPanel spTop = new StackPanel { Orientation = Orientation.Vertical };

                int counter = 1;

                foreach (ResultItem resultItem in searchResult.PrimaryQueryResult.RelevantResults)
                {
                    StackPanel spEntry = new StackPanel { Margin = new Thickness(25) };

                    string resultTitle;
                    if (!string.IsNullOrWhiteSpace(resultItem.Title))
                        resultTitle = resultItem.Title + "";
                    else if (resultItem.ContainsKey("PreferredName"))
                        resultTitle = resultItem["PreferredName"] + "";
                    else if (resultItem.ContainsKey("DocId"))
                        resultTitle = String.Format("DocId: {0}", resultItem["DocId"] + "");
                    else
                        resultTitle = "<No title to display>";

                    resultTitle = counter + ". " + resultTitle;

                    if (SearchPresentationSettings != null && SearchPresentationSettings.PrimaryResultsTitleFormat != null)
                    {
                        var userFormat = SearchPresentationSettings.PrimaryResultsTitleFormat;
                        if (!string.IsNullOrWhiteSpace(userFormat))
                        {
                            resultTitle = CustomizeTitle(userFormat, resultItem, counter);
                        }
                    }

                    string path = resultItem.Path;

                    Hyperlink titleLink = new Hyperlink();
                    if (!string.IsNullOrWhiteSpace(resultItem.Path))
                    {
                        string linkpath = resultItem.Path.Replace('\\', '/'); // for fileshares which have url replacement
                        titleLink.NavigateUri = new Uri(linkpath);
                        titleLink.RequestNavigate += HyperlinkOnRequestNavigate;
                    }
                    titleLink.Foreground = Brushes.DarkBlue;
                    titleLink.FontSize = 14;
                    titleLink.Inlines.Add(new Run(resultTitle));

                    TextBlock linkBlock = new TextBlock();
                    linkBlock.Inlines.Add(titleLink);

                    spEntry.Children.Add(linkBlock);

                    //Dont expand entries
                    Expander propsExpander = new Expander { IsExpanded = false, Header = "View" };
                    StackPanel spProps = new StackPanel();

                    var keys = resultItem.Keys.ToList();
                    keys.Sort();
                    if (_searchQueryRequest.IncludeRankDetail.HasValue &&
                        _searchQueryRequest.IncludeRankDetail.Value && !keys.Exists(k => k.Equals("rankdetail", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        keys.Add("RankDetail");
                        resultItem["RankDetail"] = string.Empty;
                    }

                    foreach (string key in keys)
                    {
                        var val = resultItem[key];
                        DockPanel propdp = new DockPanel();
                        if (string.Equals("Path", key, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrWhiteSpace(val))
                        {
                            val = val.Replace('\\', '/');
                            TextBox textBox = new TextBox
                            {
                                IsReadOnly = true,
                                IsReadOnlyCaretVisible = false,
                                Text = String.Format("{0}: ", key),
                                BorderBrush = null,
                                BorderThickness = new Thickness(0),
                                Foreground = Brushes.DarkGreen,
                                FontWeight = FontWeights.Bold,
                                FontSize = 14
                            };
                            propdp.Children.Add(textBox);

                            Hyperlink hyperlink = new Hyperlink
                            {
                                NavigateUri = new Uri(val),
                            };
                            hyperlink.RequestNavigate += HyperlinkOnRequestNavigate;
                            hyperlink.Inlines.Add(new Run(val));


                            TextBlock tb = new TextBlock();
                            tb.Inlines.Add(hyperlink);

                            propdp.Children.Add(tb);
                        }
                        else if (key.Equals("RankDetail", StringComparison.InvariantCultureIgnoreCase)
                            /* &&!string.IsNullOrWhiteSpace(val)*/)
                        {
                            string langKey =
                                resultItem.Keys.FirstOrDefault(
                                    k => k.Equals("language", StringComparison.InvariantCultureIgnoreCase));
                            string titleKey =
                                resultItem.Keys.FirstOrDefault(
                                    k => k.Equals("title", StringComparison.InvariantCultureIgnoreCase));

                            Helpers.RankDetail.ResultItem item = new Helpers.RankDetail.ResultItem
                            {
                                Xml = val,
                                Title = resultItem[langKey],
                                Path = path,
                                Language = resultItem[titleKey],
                            };
                            string workIdKey =
                                resultItem.Keys.FirstOrDefault(
                                    k => k.Equals("WorkId", StringComparison.InvariantCultureIgnoreCase) || k.Equals("DocId", StringComparison.InvariantCultureIgnoreCase));
                            if (!string.IsNullOrWhiteSpace(workIdKey))
                            {
                                item.WorkId = resultItem[workIdKey];
                            }
                            if (string.IsNullOrWhiteSpace(workIdKey) && string.IsNullOrWhiteSpace(val))
                            {
                                MessageBox.Show(
                                    "You must include the managed properties 'WorkId' or 'DocId' in the select properties to get rank details when more than 100 results are matched",
                                    "Property WorkId missing", MessageBoxButton.OK);
                                return;
                            }


                            var tb = new TextBox
                            {
                                IsReadOnly = true,
                                IsReadOnlyCaretVisible = true,
                                Text = "Show rank details...",
                                BorderBrush = null,
                                BorderThickness = new Thickness(0),
                                Foreground = Brushes.DodgerBlue,
                                FontSize = 14,
                                TextDecorations = TextDecorations.Underline,
                                Cursor = Cursors.Hand,
                                Background = Brushes.Transparent,
                                DataContext = item
                            };
                            tb.PreviewMouseLeftButtonUp += rankDetail_MouseLeftButtonUp;
                            propdp.Children.Add(tb);
                        }
                        else
                        {
                            TextBox textBox = new TextBox
                            {
                                IsReadOnly = true,
                                IsReadOnlyCaretVisible = false,
                                Text = String.Format("{0}: ", key),
                                BorderBrush = null,
                                BorderThickness = new Thickness(0),
                                Foreground = Brushes.DarkGreen,
                                FontWeight = FontWeights.Bold,
                                FontSize = 14
                            };

                            propdp.Children.Add(textBox);

                            string formatedVal = val;
                            if (key.Equals("Edges", StringComparison.InvariantCultureIgnoreCase))
                                formatedVal = JsonHelper.FormatJson(val);

                            propdp.Children.Add
                                (
                                    new TextBox
                                    {
                                        IsReadOnly = true,
                                        IsReadOnlyCaretVisible = true,
                                        Text = formatedVal,
                                        BorderBrush = null,
                                        BorderThickness = new Thickness(0),
                                        Foreground = Brushes.Green,
                                        FontSize = 14
                                    }
                                );
                        }

                        spProps.Children.Add(propdp);
                    }
                    //add everything to the item
                    propsExpander.Content = spProps;
                    spEntry.Children.Add(propsExpander);
                    spTop.Children.Add(spEntry);

                    //if there is an OWA Server and the Property ServerRedirectedEmbedURL is set, then add an inline Browser Control to show the preview
                    if (_enableExperimentalFeatures && resultItem.ContainsKey("ServerRedirectedEmbedURL"))
                    {
                        string embedUrl = resultItem["ServerRedirectedEmbedURL"];

                        if (!string.IsNullOrEmpty(embedUrl))
                        {
                            //add a preview control
                            Expander previewExpander = new Expander { IsExpanded = false, Header = "Preview" };
                            StackPanel previewPanel = new StackPanel();
                            WebBrowser browser = new WebBrowser();
                            browser.Height = 400;
                            previewExpander.Content = previewPanel;
                            previewExpander.Expanded += (sender, args) =>
                            {
                                if (browser.Source == null) //only load the first time 
                                {
                                    browser.Navigate(new Uri(embedUrl));
                                }
                            };

                            previewPanel.Children.Add(browser);
                            spEntry.Children.Add(previewExpander);
                        }
                    }

                    if (_enableExperimentalFeatures && resultItem.ContainsKey("FileType"))
                    //add a preview for html content
                    {
                        string fileType = resultItem["FileType"];
                        if (fileType.Equals("html"))
                        {
                            //add a preview control
                            Expander previewExpander = new Expander { IsExpanded = false, Header = "Preview" };
                            StackPanel previewPanel = new StackPanel();
                            WebBrowser browser = new WebBrowser();
                            browser.Height = 400;

                            previewExpander.Content = previewPanel;
                            previewExpander.Expanded += (sender, args) =>
                            {
                                if (browser.Source == null) //only load the first time 
                                {
                                    browser.Navigate(new Uri(resultItem["Path"]));

                                    browser.Navigated += delegate
                                    {
                                        HTMLDocument htmlDocument = ((HTMLDocument)browser.Document);
                                        IHTMLStyleSheet styleSheet = htmlDocument.createStyleSheet("", 0);
                                        styleSheet.cssText = "body {zoom: 80% }";
                                    };
                                }
                            };

                            previewPanel.Children.Add(browser);
                            spEntry.Children.Add(previewExpander);
                        }
                    }

                    //add an link to view all properties according to this article: http://blogs.technet.com/b/searchguys/archive/2013/12/11/how-to-all-managed-properties-of-a-document.aspx
                    AddViewAllPropertiesLink(resultItem, spProps);

                    counter++;
                }

                sv.Content = spTop;
            }
            else
            {
                TextBox tb = new TextBox
                {
                    BorderBrush = null,
                    BorderThickness = new Thickness(0),
                    IsReadOnly = true,
                    IsReadOnlyCaretVisible = false,
                    Text = "The query returned zero items!",
                    Margin = new Thickness(30)
                };
                sv.Content = tb;
            }

            PrimaryResultsTabItem.Content = sv;
        }

        private string CustomizeTitle(string userFormat, ResultItem resultItem, int counter)
        {
            var customizedTitle = userFormat;
            foreach (KeyValuePair<string, string> item in resultItem)
            {
                var oldValue = "{" + $"{item.Key}" + "}";
                var newValue = "";
                if (resultItem.ContainsKey(item.Key))
                {
                    newValue = resultItem[item.Key] + "";
                }

                customizedTitle = customizedTitle.Replace(oldValue, newValue);

            }

            customizedTitle = customizedTitle.Replace("{counter}", $"{counter}");
            return customizedTitle;
        }

        /// <summary>
        ///     Open the path to the item in a browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkOnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void AddViewAllPropertiesLink(ResultItem resultItem, StackPanel spEntry)
        {
            DockPanel propdp = new DockPanel();
            var tb = new TextBox
            {
                IsReadOnly = true,
                IsReadOnlyCaretVisible = false,
                Text = "Managed properties ",
                BorderBrush = null,
                BorderThickness = new Thickness(0),
                Foreground = Brushes.DodgerBlue,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextDecorations = TextDecorations.Underline,
                Cursor = Cursors.Hand,
            };

            tb.PreviewMouseLeftButtonUp += (sender, e) => OpenPreviewAllProperties(sender, e, resultItem, PropertyType.Managed);
            propdp.Children.Add(tb);

            var tb2 = new TextBox
            {
                IsReadOnly = true,
                IsReadOnlyCaretVisible = false,
                Text = "Crawled property names",
                BorderBrush = null,
                BorderThickness = new Thickness(0),
                Foreground = Brushes.DodgerBlue,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextDecorations = TextDecorations.Underline,
                Cursor = Cursors.Hand,
            };

            tb2.PreviewMouseLeftButtonUp += (sender, e) => OpenPreviewAllProperties(sender, e, resultItem, PropertyType.Crawled);
            propdp.Children.Add(tb2);

            spEntry.Children.Add(propdp);
        }

        private void OpenPreviewAllProperties(object sender, MouseButtonEventArgs e, ResultItem resultItem, PropertyType propertyType)
        {
            //Todo this method neeeds some refactor love
            MarkRequestOperation(false, "Running");

            //Query a new search with refiner: "ManagedProperties(filter=5000/0/*) and the path of the selected item
            SearchQueryRequest sqr = new SearchQueryRequest();
            sqr.AcceptType = _searchQueryRequest.AcceptType;
            sqr.AuthenticationType = _searchQueryRequest.AuthenticationType;
            sqr.ClientType = _searchQueryRequest.ClientType;
            sqr.Culture = _searchQueryRequest.Culture;
            sqr.EnableFql = _searchQueryRequest.EnableFql;
            sqr.EnableNicknames = _searchQueryRequest.EnableFql;
            sqr.EnablePhonetic = _searchQueryRequest.EnablePhonetic;
            sqr.EnableQueryRules = _searchQueryRequest.EnableQueryRules;
            sqr.EnableStemming = _searchQueryRequest.EnableStemming;
            if (_searchQueryRequest.SourceId != null)
            {
                sqr.SourceId = _searchQueryRequest.SourceId;
            }

            //try get the search result with the workId of the current hit, see: http://techmikael.blogspot.no/2014/10/look-up-item-based-on-items-id.html
            string workIdKey =
                resultItem.Keys.FirstOrDefault(k => k.Equals("WorkId", StringComparison.InvariantCultureIgnoreCase) || k.Equals("DocId", StringComparison.InvariantCultureIgnoreCase));

            if (workIdKey == null)
            {
                MessageBox.Show(
                    "You must include the managed properties 'WorkId/DocId' in the select properties to get all properties",
                    "Property WorkId missing", MessageBoxButton.OK);
                return;
            }

            sqr.QueryText = string.Format("WorkId:\"{0}\"", resultItem[workIdKey]);

            sqr.ResultsUrl = _searchQueryRequest.ResultsUrl;

            sqr.SharePointSiteUrl = _searchQueryRequest.SharePointSiteUrl;
            sqr.Cookies = _searchQueryRequest.Cookies;
            sqr.Token = _searchQueryRequest.Token;
            sqr.UserName = _searchQueryRequest.UserName;
            sqr.Password = _searchQueryRequest.Password;
            sqr.SecurePassword = _searchQueryRequest.SecurePassword;

            if (propertyType == PropertyType.Managed)
            {
                //this is the magic ingredient to get all the properties back
                sqr.Refiners = "ManagedProperties(filter=5000/0/*)";
            }
            else
            {
                sqr.Refiners = "CrawledProperties(filter=5000/0/*)";
            }

            try
            {
                var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                var tokenSource = new CancellationTokenSource();
                CancellationToken ct = tokenSource.Token;
                Task.Factory.StartNew(() => HttpRequestRunner.RunWebRequest(sqr), ct,
                    TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent, scheduler).ContinueWith(
                        task =>
                        {
                            HttpRequestResponsePair result = task.Result;
                            SearchQueryResult resultItem2 = GetResultItem(result);

                            //Extract all Properties from refiner result
                            if (resultItem2.PrimaryQueryResult == null ||
                                resultItem2.PrimaryQueryResult.RefinerResults == null)
                            {
                                MessageBox.Show(
                                    "The ManagedProperties property does not contain values. See https://github.com/wobba/SPO-Trigger-Reindex for more information on how to enable this feature.",
                                    "ManagedProperties is empty", MessageBoxButton.OK);
                                return;
                            }
                            if (propertyType == PropertyType.Crawled)
                            {
                                RefinerResult crawledPropertyNames = resultItem2.PrimaryQueryResult.RefinerResults[0];
                                ResultItem cpResult = new ResultItem();
                                var cpGroupMapper = new Dictionary<string, string>
                                    {
                                        { "00020329-0000-0000-C000-000000000046", "SharePoint" },
                                        { "00130329-0000-0130-c000-000000131346", "SharePoint" },
                                        { "00140329-0000-0140-c000-000000141446", "SharePoint" },
                                        { "0b63e343-9ccc-11d0-bcdb-00805fccce04", "Basic" },
                                        { "158d7563-aeff-4dbf-bf16-4a1445f0366c","SharePoint" },
                                        { "49691c90-7e17-101a-a91c-08002b2ecda9", "Basic" },
                                        { "B725F130-47EF-101A-A5F1-02608C9EEBAC","Basic" },
                                        { "012357bd-1113-171d-1f25-292bb0b0b0b0", "Internal" },
                                        { "ED280121-B677-4E2A-8FBC-0D9E2325B0A2", "SharePoint" },
                                        { "F29F85E0-4FF9-1068-AB91-08002B27B3D9","Office" },
                                        { "64ae120f-487d-445a-8d5a-5258f99cb970","Document Parser" },
                                        { "e835446c-937b-4492-95f3-d89988b01039","MetadataExtractor" }
                                    };

                                foreach (var item in crawledPropertyNames)
                                {
                                    foreach (var map in cpGroupMapper)
                                    {
                                        if (Regex.IsMatch(item.Name, map.Key + ":", RegexOptions.IgnoreCase))
                                        {
                                            item.Name = Regex.Replace(item.Name, map.Key + ":", "", RegexOptions.IgnoreCase);
                                            while (cpResult.ContainsKey(item.Name))
                                            {
                                                item.Name += " ";
                                            }
                                            cpResult.Add(item.Name, map.Value);
                                        }
                                    }
                                }

                                PropertiesDetail pd = new PropertiesDetail(cpResult, sqr.QueryText);
                                pd.Show();
                            }
                            else
                            {
                                var refiners = resultItem2.PrimaryQueryResult.RefinerResults[0];

                                //Query again with the select properties set
                                sqr.Refiners = "";
                                sqr.SelectProperties = String.Join(",", refiners.Select(x => x.Name).ToArray());
                                sqr.SelectProperties = string.Join(",",
                                    sqr.SelectProperties.Split(',').Except(new[]
                                        {"ClassificationLastScan", "ClassificationConfidence", "ClassificationCount", "ClassificationContext"}));

                                sqr.HttpMethodType = HttpMethodType.Post;

                                Task.Factory.StartNew(() => HttpRequestRunner.RunWebRequest(sqr), ct,
                                    TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent, scheduler)
                                    .ContinueWith(
                                        innerTask =>
                                        {
                                            HttpRequestResponsePair innerResult = innerTask.Result;
                                            SearchQueryResult innerResult2 = GetResultItem(innerResult);

                                            if (innerResult2.PrimaryQueryResult == null)
                                            {
                                                MessageBox.Show("Could not load properties for the item");
                                            }
                                            else
                                            {
                                                //Open the new window and show the properties there
                                                ResultItem relevantResult = innerResult2.PrimaryQueryResult.RelevantResults[0];
                                                PropertiesDetail pd = new PropertiesDetail(relevantResult, sqr.QueryText);
                                                pd.Show();
                                            }

                                        }).ContinueWith(innerTask =>
                                        {
                                            if (innerTask.Exception != null)
                                            {
                                                ShowError(task.Exception);
                                            }
                                        }, scheduler);
                            }
                        }, scheduler).ContinueWith(task =>
                        {
                            if (task.Exception != null)
                            {
                                ShowError(task.Exception);
                            }
                            MarkRequestOperation(false, "Done");
                        }, scheduler);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                MarkRequestOperation(false, "Done");
            }
        }

        private void rankDetail_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            Helpers.RankDetail.ResultItem item = (Helpers.RankDetail.ResultItem)tb.DataContext;
            if (string.IsNullOrWhiteSpace(item.Xml))
            {
                // We have more than 100 results and have to request more
                //Todo this method neeeds some refactor love
                MarkRequestOperation(false, "Running");
                SearchQueryRequest sqr = new SearchQueryRequest
                {
                    AcceptType = _searchQueryRequest.AcceptType,
                    AuthenticationType = _searchQueryRequest.AuthenticationType,
                    ClientType = _searchQueryRequest.ClientType,
                    Culture = _searchQueryRequest.Culture,
                    EnableFql = _searchQueryRequest.EnableFql,
                    EnableNicknames = _searchQueryRequest.EnableFql,
                    EnablePhonetic = _searchQueryRequest.EnablePhonetic,
                    EnableQueryRules = _searchQueryRequest.EnableQueryRules,
                    EnableStemming = _searchQueryRequest.EnableStemming,
                    RankingModelId = _searchQueryRequest.RankingModelId,
                    RowLimit = 1,
                    IncludeRankDetail = true,
                    QueryText = _searchQueryRequest.QueryText,
                    QueryTemplate = _searchQueryRequest.QueryTemplate,
                    ResultsUrl = _searchQueryRequest.ResultsUrl,
                    SharePointSiteUrl = _searchQueryRequest.SharePointSiteUrl,
                    Cookies = _searchQueryRequest.Cookies,
                    UserName = _searchQueryRequest.UserName,
                    Password = _searchQueryRequest.Password,
                    SecurePassword = _searchQueryRequest.SecurePassword,
                    Token = _searchQueryRequest.Token
                };
                if (!string.IsNullOrWhiteSpace(_searchQueryRequest.RefinementFilters))
                {
                    sqr.RefinementFilters = _searchQueryRequest.RefinementFilters + ",";
                }
                sqr.RefinementFilters += "WorkId:" + item.WorkId;

                try
                {
                    var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                    CancellationToken ct = new CancellationToken();
                    Task.Factory.StartNew(() => HttpRequestRunner.RunWebRequest(sqr), ct,
                        TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent, scheduler).ContinueWith(
                            task =>
                            {
                                var result = task.Result;
                                SearchQueryResult searchResult = GetResultItem(result);
                                if (searchResult.PrimaryQueryResult != null &&
                                    searchResult.PrimaryQueryResult.RelevantResults != null &&
                                    searchResult.PrimaryQueryResult.TotalRows > 0)
                                {
                                    ResultItem resultItem = searchResult.PrimaryQueryResult.RelevantResults.First();
                                    var rdKey = resultItem.Keys.SingleOrDefault(k => k.ToLower() == "rankdetail");
                                    if (rdKey != null)
                                    {
                                        item.Xml = resultItem[rdKey];
                                    }
                                    RankDetail rd = new RankDetail();
                                    rd.DataContext = item;
                                    rd.Show();
                                }
                            }, scheduler).ContinueWith(task =>
                            {
                                if (task.Exception != null)
                                {
                                    ShowError(task.Exception);
                                }
                                MarkRequestOperation(false, "Done");
                            }, scheduler);
                }
                catch (Exception ex)
                {
                    ShowError(ex);
                }
                finally
                {
                    MarkRequestOperation(false, "Done");
                }
            }
            else
            {
                RankDetail rd = new RankDetail();
                rd.DataContext = tb.DataContext;
                rd.Show();
            }
        }


        /// <summary>
        ///     Creates and populates the the Refinement results tab from data from the passed in <paramref name="searchResult" />.
        ///     This method is used for only query results.
        /// </summary>
        /// <param name="searchResult">The search result.</param>
        private void SetRefinementResultItems(SearchQueryResult searchResult)
        {
            ScrollViewer sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            if (searchResult.PrimaryQueryResult != null && searchResult.PrimaryQueryResult.RefinerResults != null
                && searchResult.PrimaryQueryResult.RefinerResults.Count > 0)
            {
                StackPanel spTop = new StackPanel { Orientation = Orientation.Vertical };

                int counter = 1;

                foreach (var refinerItem in searchResult.PrimaryQueryResult.RefinerResults)
                {
                    StackPanel spEntry = new StackPanel { Margin = new Thickness(25) };
                    TextBox titleTB = new TextBox
                    {
                        IsReadOnly = true,
                        IsReadOnlyCaretVisible = false,
                        Text = String.Format("{0}. {1}", counter, refinerItem.Name),
                        BorderBrush = null,
                        BorderThickness = new Thickness(0),
                        Foreground = Brushes.DarkBlue,
                        FontSize = 18
                    };
                    spEntry.Children.Add(titleTB);

                    Expander propsExpander = new Expander { IsExpanded = false, Header = "View Entries" };
                    StackPanel spProps = new StackPanel();

                    foreach (var re in refinerItem)
                    {
                        DockPanel propdp = new DockPanel();
                        propdp.Children.Add
                            (
                                new TextBox
                                {
                                    IsReadOnly = true,
                                    IsReadOnlyCaretVisible = false,
                                    Text = "Refinement Name:",
                                    BorderBrush = null,
                                    BorderThickness = new Thickness(0),
                                    Foreground = Brushes.DarkGreen,
                                    FontWeight = FontWeights.Normal,
                                    FontSize = 12
                                }
                            );
                        propdp.Children.Add
                            (
                                new TextBox
                                {
                                    IsReadOnly = true,
                                    IsReadOnlyCaretVisible = false,
                                    Text = String.Format("{0}", re.Name.Replace("\n", " ")),
                                    BorderBrush = null,
                                    BorderThickness = new Thickness(0),
                                    Foreground = Brushes.DarkMagenta,
                                    FontWeight = FontWeights.Normal,
                                    FontSize = 12
                                }
                            );
                        spProps.Children.Add(propdp);

                        propdp = new DockPanel();
                        propdp.Children.Add
                            (
                                new TextBox
                                {
                                    IsReadOnly = true,
                                    IsReadOnlyCaretVisible = false,
                                    Text = "Refinement Count:",
                                    BorderBrush = null,
                                    BorderThickness = new Thickness(0),
                                    Foreground = Brushes.DarkGreen,
                                    FontWeight = FontWeights.Normal,
                                    FontSize = 12
                                }
                            );
                        propdp.Children.Add
                            (
                                new TextBox
                                {
                                    IsReadOnly = true,
                                    IsReadOnlyCaretVisible = false,
                                    Text = String.Format("{0}", re.Count),
                                    BorderBrush = null,
                                    BorderThickness = new Thickness(0),
                                    Foreground = Brushes.DarkMagenta,
                                    FontWeight = FontWeights.Normal,
                                    FontSize = 12
                                }
                            );
                        spProps.Children.Add(propdp);

                        propdp = new DockPanel();
                        propdp.Children.Add
                            (
                                new TextBox
                                {
                                    IsReadOnly = true,
                                    IsReadOnlyCaretVisible = false,
                                    Text = "Refinement Token:",
                                    BorderBrush = null,
                                    BorderThickness = new Thickness(0),
                                    Foreground = Brushes.DarkGreen,
                                    FontWeight = FontWeights.Normal,
                                    FontSize = 12
                                }
                            );
                        propdp.Children.Add
                            (
                                new TextBox
                                {
                                    IsReadOnly = true,
                                    IsReadOnlyCaretVisible = false,
                                    Text = String.Format("{0}", re.Token.Replace("\n", " ")),
                                    BorderBrush = null,
                                    BorderThickness = new Thickness(0),
                                    Foreground = Brushes.DarkMagenta,
                                    FontWeight = FontWeights.Normal,
                                    FontSize = 12
                                }
                            );
                        spProps.Children.Add(propdp);

                        propdp = new DockPanel();
                        propdp.Children.Add
                            (
                                new TextBox
                                {
                                    IsReadOnly = true,
                                    IsReadOnlyCaretVisible = false,
                                    Text = "Refinement Value:",
                                    BorderBrush = null,
                                    BorderThickness = new Thickness(0),
                                    Foreground = Brushes.DarkGreen,
                                    FontWeight = FontWeights.Normal,
                                    FontSize = 12
                                }
                            );
                        propdp.Children.Add
                            (
                                new TextBox
                                {
                                    IsReadOnly = true,
                                    IsReadOnlyCaretVisible = false,
                                    Text = String.Format("{0}", re.Value.Replace("\n", " ")),
                                    BorderBrush = null,
                                    BorderThickness = new Thickness(0),
                                    Foreground = Brushes.DarkMagenta,
                                    FontWeight = FontWeights.Normal,
                                    FontSize = 12
                                }
                            );
                        spProps.Children.Add(propdp);

                        spProps.Children.Add(new Line { Height = 18 });
                    }

                    propsExpander.Content = spProps;
                    spEntry.Children.Add(propsExpander);
                    spTop.Children.Add(spEntry);

                    counter++;
                }

                sv.Content = spTop;
            }

            RefinementResultsTabItem.Content = sv;
        }

        /// <summary>
        ///     Creates and populates the the Secondary results tab from data from the passed in <paramref name="searchResult" />.
        ///     This method is used for only query results.
        /// </summary>
        /// <param name="searchResult">The search result.</param>
        private void SetSecondaryQueryResultItems(SearchQueryResult searchResult)
        {
            ScrollViewer sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            if (searchResult.SecondaryQueryResults != null && searchResult.SecondaryQueryResults.Count > 0)
            {
                StackPanel spTop = new StackPanel { Orientation = Orientation.Vertical };

                int counter = 1;

                foreach (var sqr in searchResult.SecondaryQueryResults)
                {
                    if (sqr.RelevantResults == null || sqr.RelevantResults.Count == 0)
                        continue;

                    foreach (var resultItem in sqr.RelevantResults)
                    {
                        StackPanel spEntry = new StackPanel { Margin = new Thickness(25) };

                        string resultTitle;
                        if (resultItem.ContainsKey("Title"))
                            resultTitle = resultItem["Title"];
                        else if (resultItem.ContainsKey("title"))
                            resultTitle = resultItem["title"];
                        else if (resultItem.ContainsKey("DocId"))
                            resultTitle = String.Format("DocId: {0}", resultItem["DocId"]);
                        else
                            resultTitle = "";

                        TextBox titleTB = new TextBox
                        {
                            IsReadOnly = true,
                            IsReadOnlyCaretVisible = false,
                            Text = String.Format("{0}. {1}", counter, resultTitle),
                            BorderBrush = null,
                            BorderThickness = new Thickness(0),
                            Foreground = Brushes.DarkBlue,
                            FontSize = 14
                        };
                        spEntry.Children.Add(titleTB);

                        Expander propsExpander = new Expander
                        {
                            IsExpanded = searchResult.SecondaryQueryResults.Count == 1,
                            Header = "View"
                        };
                        StackPanel spProps = new StackPanel();
                        foreach (var kv in resultItem)
                        {
                            DockPanel propdp = new DockPanel();
                            propdp.Children.Add
                                (
                                    new TextBox
                                    {
                                        IsReadOnly = true,
                                        IsReadOnlyCaretVisible = false,
                                        Text = String.Format("{0}: ", kv.Key),
                                        BorderBrush = null,
                                        BorderThickness = new Thickness(0),
                                        Foreground = Brushes.DarkGreen,
                                        FontWeight = FontWeights.Bold,
                                        FontSize = 14
                                    }
                                );

                            propdp.Children.Add
                                (
                                    new TextBox
                                    {
                                        IsReadOnly = true,
                                        IsReadOnlyCaretVisible = true,
                                        Text = String.Format("{0}", kv.Value),
                                        BorderBrush = null,
                                        BorderThickness = new Thickness(0),
                                        Foreground = Brushes.Green,
                                        FontSize = 14
                                    }
                                );

                            spProps.Children.Add(propdp);
                        }

                        propsExpander.Content = spProps;
                        spEntry.Children.Add(propsExpander);
                        spTop.Children.Add(spEntry);

                        counter++;
                    }
                }

                sv.Content = spTop;
            }

            SecondaryResultsTabItem.Content = sv;
        }

        /// <summary>
        ///     Creates and populates the the Suggestion results tab from data from the passed in <paramref name="searchResult" />.
        ///     This method is used for only suggestion results.
        /// </summary>
        /// <param name="searchResult">The search result.</param>
        private void SetSuggestionsResultItems(SearchSuggestionsResult searchResult)
        {
            ScrollViewer sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            if (searchResult.SuggestionResults != null && searchResult.SuggestionResults.Count > 0)
            {
                StackPanel spTop = new StackPanel { Orientation = Orientation.Vertical };

                int counter = 1;

                foreach (var resultITem in searchResult.SuggestionResults)
                {
                    StackPanel spEntry = new StackPanel { Margin = new Thickness(25) };
                    TextBox queryTB = new TextBox
                    {
                        IsReadOnly = true,
                        IsReadOnlyCaretVisible = false,
                        Text = String.Format("{0}. {1}", counter, resultITem.Query),
                        BorderBrush = null,
                        BorderThickness = new Thickness(0),
                        Foreground = Brushes.DarkBlue,
                        FontSize = 14
                    };

                    spEntry.Children.Add(queryTB);

                    TextBox isPersonalTB = new TextBox
                    {
                        IsReadOnly = true,
                        IsReadOnlyCaretVisible = false,
                        Text = String.Format("IsPersonal: {0}", resultITem.IsPersonal),
                        BorderBrush = null,
                        BorderThickness = new Thickness(0),
                        Foreground = Brushes.Chocolate,
                        FontSize = 14
                    };

                    spEntry.Children.Add(isPersonalTB);


                    spTop.Children.Add(spEntry);

                    counter++;
                }

                sv.Content = spTop;
            }
            else
            {
                TextBox tb = new TextBox
                {
                    BorderBrush = null,
                    BorderThickness = new Thickness(0),
                    IsReadOnly = true,
                    IsReadOnlyCaretVisible = false,
                    Text = "The query returned no items!",
                    Margin = new Thickness(30)
                };
                sv.Content = tb;
            }

            SuggestionResultsTabItem.Content = sv;
        }

        /// <summary>
        ///     Updates the request URI string text block.
        /// </summary>
        private void UpdateRequestUriStringTextBlock()
        {
            try
            {
                if (SharePointSiteUrlTextBox != null)
                {
                    _searchQueryRequest.SharePointSiteUrl = SharePointSiteUrlTextBox.Text.Trim();
                    _searchSuggestionsRequest.SharePointSiteUrl = SharePointSiteUrlTextBox.Text.Trim();
                }

                var searchMethodType = CurrentSearchMethodType;
                var httpMethodType = CurrentHttpMethodType;
                if (searchMethodType == SearchMethodType.Query)
                {
                    if (httpMethodType == HttpMethodType.Get)
                    {
                        var uri = _searchQueryRequest.GenerateHttpGetUri().ToString();
                        if (RequestUriLengthTextBox != null)
                        {
                            RequestUriLengthTextBox.Text = $"HTTP GET {uri.Length.ToString()}";
                        }

                        RequestUriStringTextBox.Text = uri;
                    }

                    else if (httpMethodType == HttpMethodType.Post)
                    {
                        var uri = _searchQueryRequest.GenerateHttpGetUri().ToString();
                        if (RequestUriLengthTextBox != null)
                        {
                            RequestUriLengthTextBox.Text = $"HTTP POST {uri.Length.ToString()}";
                        }
                        RequestUriStringTextBox.Text = _searchQueryRequest.GenerateHttpPostUri().ToString();
                    }
                }
                else if (searchMethodType == SearchMethodType.Suggest)
                {
                    RequestUriStringTextBox.Text = _searchSuggestionsRequest.GenerateHttpGetUri().ToString();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }

            RequestUriStringTextBox.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Marks the request operation by disabling or enabling controls.
        /// </summary>
        /// <param name="starting">if set to <c>true</c> [starting] all ResultTabs are cleared.</param>
        /// <param name="status">The status.</param>
        private void MarkRequestOperation(bool starting, string status)
        {
            this.Dispatcher.Invoke(() =>
            {
                RunButton.IsEnabled = !starting;

                if (starting)
                {
                    ClearResultTabs();
                    QueryGroupBox.IsEnabled = false;
                    ConnectionExpanderBox.IsEnabled = false;
                }
                else
                {
                    QueryGroupBox.IsEnabled = true;
                    ConnectionExpanderBox.IsEnabled = true;
                }

                ProgressBar.Visibility = starting ? Visibility.Visible : Visibility.Hidden;
                Duration duration = new Duration(TimeSpan.FromSeconds(30));
                DoubleAnimation doubleanimation = new DoubleAnimation(100.0, duration);

                if (starting)
                    ProgressBar.BeginAnimation(RangeBase.ValueProperty, doubleanimation);
                else
                    ProgressBar.BeginAnimation(RangeBase.ValueProperty, null);

                StateBarTextBlock.Text = status;
            });
        }

        /// <summary>
        ///     Clears the result tabs.
        /// </summary>
        private void ClearResultTabs()
        {
            this.Dispatcher.Invoke(() =>
            {
                StatsResultTabItem.Content = null;
                RawResultTabItem.Content = null;
                PrimaryResultsTabItem.Content = null;
                RefinementResultsTabItem.Content = null;
                SecondaryResultsTabItem.Content = null;
                SuggestionResultsTabItem.Content = null;
            });
        }

        /// <summary>
        ///     Shows the error.
        /// </summary>
        /// <param name="error">The error.</param>
        private void ShowError(Exception error)
        {
            ClearResultTabs();

            ScrollViewer sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            TextBox tb = new TextBox();
            tb.BorderBrush = null;
            tb.IsReadOnly = true;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.IsReadOnlyCaretVisible = false;

            if (error != null)
            {
                tb.AppendText(error.Message + Environment.NewLine + Environment.NewLine);
            }

            if (error != null)
            {
                Exception inner = error.InnerException;
                while (inner != null)
                {
                    tb.AppendText(inner.Message + Environment.NewLine + Environment.NewLine);
                    inner = inner.InnerException;
                }
            }

            sv.Content = tb;
            StatsResultTabItem.Content = sv;
        }

        private void ShowMsgBox(string message)
        {
            MessageBox.Show(message);
        }

        /// <summary>
        ///     Sets the impersonation level selections.
        /// </summary>
        private void SetCurrentWindowsUserIdentity()
        {
            WindowsIdentity currentWindowsIdentity = WindowsIdentity.GetCurrent();
            if (currentWindowsIdentity != null)
            {
                if (AuthenticationUsernameTextBox != null
                    && string.IsNullOrWhiteSpace(AuthenticationUsernameTextBox.Text))
                    AuthenticationUsernameTextBox.Text = currentWindowsIdentity.Name;
            }
        }

        /// <summary>
        ///     Adds the copy command.
        /// </summary>
        /// <param name="control">The control.</param>
        private void AddCopyCommand(Control control)
        {
            ContextMenu cm = new ContextMenu();
            control.ContextMenu = cm;

            MenuItem mi = new MenuItem();
            mi.Command = ApplicationCommands.Copy;
            mi.CommandTarget = control;
            mi.Header = ApplicationCommands.Copy.Text;
            cm.Items.Add(mi);

            CommandBinding copyCmdBinding = new CommandBinding();
            copyCmdBinding.Command = ApplicationCommands.Copy;
            copyCmdBinding.Executed += CopyCmdBinding_Executed;
            copyCmdBinding.CanExecute += CopyCmdBinding_CanExecute;
            control.CommandBindings.Add(copyCmdBinding);
        }

        /// <summary>
        ///     Handles the Executed event of the CopyCmdBinding control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.</param>
        private void CopyCmdBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Set text to clip board 
            if (sender is TreeViewItem)
            {
                Clipboard.SetText((string)(sender as TreeViewItem).Header);
            }
        }

        /// <summary>
        ///     Handles the CanExecute event of the CopyCmdBinding control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CanExecuteRoutedEventArgs" /> instance containing the event data.</param>
        private void CopyCmdBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //Check for text 
            if (sender is TreeViewItem)
            {
                TreeViewItem tvi = sender as TreeViewItem;
                if (tvi.Header != null)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
        }
        #endregion

        private void ConnectToSignalR_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_hubConnection != null)
                {
                    _hubConnection.Dispose();
                }
                _hubConnection = new HubConnection(SignalRHubUrlTextBox.Text);


                _hub = _hubConnection.CreateHubProxy("UlsHub");

                _hubConnection.StateChanged += change =>
                {
                    if (change.NewState == ConnectionState.Disconnected)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            SignalRUrlImage.ToolTip = "Disconnected";
                            SignalRUrlImage.Source = new BitmapImage(new Uri("Images/alert_icon.png", UriKind.Relative));
                            SignalRUrlImage.Visibility = Visibility.Visible;
                            DebugTabItem.IsEnabled = false;
                        }));
                    }
                    else if (change.NewState == ConnectionState.Connected)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            SignalRUrlImage.ToolTip = "Connected";
                            SignalRUrlImage.Source =
                                new BitmapImage(new Uri("Images/connected_icon.png", UriKind.Relative));
                            SignalRUrlImage.Visibility = Visibility.Visible;
                            DebugTabItem.IsEnabled = true;
                        }));
                    }
                    else if (change.NewState == ConnectionState.Reconnecting)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            SignalRUrlImage.ToolTip = "Reconnecting";
                            SignalRUrlImage.Source =
                                new BitmapImage(new Uri("Images/reconnect_icon.png", UriKind.Relative));
                            SignalRUrlImage.Visibility = Visibility.Visible;
                            DebugTabItem.IsEnabled = false;
                        }));
                    }
                };

                _hub.On<LogEntry>("addSearchQuery", ProcessQueryLogEntry);

                _hubConnection.Start().Wait();
            }
            catch (Exception ex)
            {
                ShowMsgBox("Could not connect to signalr hub: " + ex.Message);
            }
        }

        public void LogMessageToFile(string msg)
        {
            StreamWriter sw = File.AppendText(
                "c:\\log.txt");
            try
            {
                string logLine = String.Format(
                    "{0}.", msg);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }


        private void ProcessQueryLogEntry(LogEntry logEntry)
        {
            try
            {
                Regex firstQuery =
                    new Regex(
                        "^Microsoft.Office.Server.Search.Query.Ims.ImsQueryInternal : New request: Query text '(.*)', Query template '(.*)'; HiddenConstraints:(.*); SiteSubscriptionId: (.*)");
                Regex personalResults =
                    new Regex(
                        "^Microsoft.Office.Server.Search.Query.Pipeline.Processing.QueryRouterEvaluator : QueryRouterEvaluator: Received (.*) PersonalFavoriteResults.*");
                Regex relevantResults =
                    new Regex(
                        "^Microsoft.Office.Server.Search.Query.Pipeline.Processing.QueryRouterEvaluator : QueryRouterEvaluator: Received (.*) RelevantResults results.*");
                Regex refinementResults =
                    new Regex(
                        "^Microsoft.Office.Server.Search.Query.Pipeline.Processing.QueryRouterEvaluator : QueryRouterEvaluator: Received (.*) RefinementResults.*");
                Regex boundVariables = new Regex("^QueryClassifierEvaluator : (.*)$");

                //TODO simplify this logic
                //if (logEntry.Message.Contains("SharePoint"))
                //    LogMessageToFile(logEntry.Message);

                if (firstQuery.IsMatch(logEntry.Message))
                {
                    var query = firstQuery.Match(logEntry.Message).Groups[1].Value;
                    var queryTemplate = firstQuery.Match(logEntry.Message).Groups[2].Value;
                    var hiddenConstraints = firstQuery.Match(logEntry.Message).Groups[3].Value;
                    var siteSubscriptionId = firstQuery.Match(logEntry.Message).Groups[4].Value;
                    if (!string.IsNullOrEmpty(query))
                    {
                        SearchQueryDebug debug = new SearchQueryDebug(logEntry.Correlation, Dispatcher);
                        debug.Query = string.Format("{0}", query);

                        if (!string.IsNullOrEmpty(queryTemplate))
                        {
                            debug.Template = string.Format("{0}", queryTemplate);
                        }

                        if (!string.IsNullOrWhiteSpace(hiddenConstraints))
                        {
                            debug.HiddenConstraint = string.Format("{0}", hiddenConstraints);
                        }

                        if (!string.IsNullOrEmpty(siteSubscriptionId))
                        {
                            debug.SiteSubscriptionId = string.Format("{0}", siteSubscriptionId);
                        }

                        ObservableQueryCollection.Add(debug);
                    }
                }
                else
                {
                    //locking? 
                    SearchQueryDebug debug =
                        ObservableQueryCollection.FirstOrDefault(d => d.Correlation == logEntry.Correlation);
                    if (debug != null)
                    {
                        if (logEntry.Message.StartsWith("QueryTemplateHelper: "))
                        {
                            try
                            {
                                Monitor.Enter(_locker);
                                string queryTemplateHelper = logEntry.Message.Replace("QueryTemplateHelper: ", "");
                                debug.QueryTemplateHelper.Add(queryTemplateHelper);
                            }
                            finally
                            {
                                Monitor.Exit(_locker);
                            }
                        }
                        else if (logEntry.Category == "Linguistic Processing")
                        {
                            try
                            {
                                Monitor.Enter(_locker);
                                if (
                                    logEntry.Message.StartsWith(
                                        "Microsoft.Ceres.ContentEngine.NlpEvaluators.QuerySuggestionEvaluator"))
                                {
                                    debug.QuerySuggestion =
                                        logEntry.Message.Replace(
                                            "Microsoft.Ceres.ContentEngine.NlpEvaluators.QuerySuggestionEvaluator: ", "");
                                }
                                else if (string.IsNullOrEmpty(debug.QueryExpanded1))
                                {
                                    debug.QueryExpanded1 =
                                        logEntry.Message.Replace(
                                            "Microsoft.Ceres.ContentEngine.NlpEvaluators.Tokenizer.QueryWordBreakerProducer: ",
                                            "");
                                }
                                else if (logEntry.Message.StartsWith("..."))
                                {
                                    debug.QueryExpanded3 += logEntry.Message.TrimStart(new[] { '.' });
                                }
                                else if (string.IsNullOrEmpty(debug.QueryExpanded2))
                                {
                                    debug.QueryExpanded2 =
                                        logEntry.Message.Replace(
                                            "Microsoft.Ceres.ContentEngine.NlpEvaluators.Tokenizer.QueryWordBreakerProducer: ",
                                            "").TrimEnd(new[] { '.' });
                                }
                            }
                            finally
                            {
                                Monitor.Exit(_locker);
                            }
                        }
                        else if (boundVariables.IsMatch(logEntry.Message))
                        {
                            string value = boundVariables.Match(logEntry.Message).Groups[1].Value;
                            debug.BoundVariables.Add(value);
                        }
                        else if (relevantResults.IsMatch(logEntry.Message))
                        {
                            debug.RelevantResults = relevantResults.Match(logEntry.Message).Groups[1].Value;
                        }
                        else if (refinementResults.IsMatch(logEntry.Message))
                        {
                            debug.RefinerResults = refinementResults.Match(logEntry.Message).Groups[1].Value;
                        }
                    }
                    else if (ObservableQueryCollection.Any(q => logEntry.Message.Contains(q.Correlation)))
                    //child correlation, this can be a very expensive query... 
                    {
                        if (
                            logEntry.Message.StartsWith(
                                "Microsoft.Office.Server.Search.Query.Pipeline.Processing.QueryRouterEvaluator : QueryRouterEvaluator: "))
                        {
                            debug =
                                ObservableQueryCollection.FirstOrDefault(
                                    q => logEntry.Message.Contains(q.Correlation));
                            if (debug != null)
                                debug.PersonalResults = personalResults.Match(logEntry.Message).Groups[1].Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void QueryDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //QueryTextBox.Text = "";
        }

        private void FreshnessBoost_Click(object sender, RoutedEventArgs e)
        {
            FreshnessBoost fb = new FreshnessBoost();
            fb.Show();
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            var options = new SearchPresentationSettings();
            options.Show();
        }

        /// <summary>
        /// Get a SearchConnection object based on the current alues of the relevant input boxes in the user interface.
        /// </summary>
        /// <returns>A SearchConnection object</returns>
        internal SearchConnection GetSearchConnectionFromUi()
        {
            var connection = new SearchConnection
            {
                SpSiteUrl = SharePointSiteUrlTextBox.Text.Trim(),
                Timeout = WebRequestTimeoutTextBox.Text.Trim(),
                Accept = (AcceptJsonRadioButton.IsChecked == true ? "json" : "xml"),
                HttpMethod = (HttpGetMethodRadioButton.IsChecked == true ? "GET" : "POST"),
                Username = (AuthenticationUsernameTextBox.IsEnabled) ? AuthenticationUsernameTextBox.Text.Trim() : "",
                AuthTypeIndex = AuthenticationTypeComboBox.SelectedIndex,
                AuthMethodIndex = AuthenticationMethodComboBox.SelectedIndex,
                EnableExperimentalFeatures = _enableExperimentalFeatures
            };
            return connection;
        }

        /// <summary>
        /// Get a SearchQueryRequest object based on the current alues of the relevant input boxes in the user interface.
        /// </summary>
        /// <returns>A SearchConnection object</returns>
        internal SearchQueryRequest GetSearchQueryRequestFromUi()
        {
            return _searchQueryRequest;
        }

        /// <summary>
        /// Save current search query and connection settings to a new XML file. A file dialog asks end user for output path.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Parameters in the event</param>
        private void SaveAsNewPresetButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var request = GetSearchQueryRequestFromUi();
                var connection = GetSearchConnectionFromUi();
                var annotation = AnnotatePresetTextBox.Text;

                var userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                var timeStamp = DateTime.Now;
                var versionHistory = $"{timeStamp}: Created by {userName}";
                var newAnnotation = $"{annotation}\r\n- {versionHistory}";

                var preset = new SearchPreset()
                {
                    Request = request,
                    Connection = connection,
                    Annotation = newAnnotation
                };

                // Bring up Save As dialog to save current settings as new preset to set the Path and Name (derived from path) for the preset
                var dlg = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = "Example Preset Query Settings",
                    DefaultExt = ".xml",
                    InitialDirectory = Path.GetFullPath(PresetFolderPath),
                    Filter = "Xml files (.xml)|*.xml"

                };

                if (dlg.ShowDialog() == true)
                {
                    preset.Path = dlg.FileName;
                    preset.Name = Path.GetFileNameWithoutExtension(preset.Path);

                    var r = preset.Save();
                    AnnotatePresetTextBox.Text = newAnnotation;
                    StateBarTextBlock.Text = String.Format("{0} new preset {1}", r ? "Saved" : "Failed to save", preset.Path);

                    // Reload saved files to populate combobox again
                    LoadSearchPresetsFromFolder(PresetFolderPath);
                }
                else
                {
                    StateBarTextBlock.Text = String.Format("Failed to write XML preset");
                }
            }
            catch (Exception ex)
            {
                StateBarTextBlock.Text = String.Format("Failed to write XML preset: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Save an updated version of the currently selected settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavePresetButton_OnClick(object sender, RoutedEventArgs e)
        {
            var selected = PresetComboBox.SelectedItem as SearchPreset;
            if (selected != null)
            {
                var annotation = AnnotatePresetTextBox.Text;

                var userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                var timeStamp = DateTime.Now;
                var versionHistory = $"{timeStamp}: Version saved by {userName}";
                var newAnnotation = $"{annotation}\r\n- {versionHistory}";

                var preset = new SearchPreset()
                {
                    Request = GetSearchQueryRequestFromUi(),
                    Connection = GetSearchConnectionFromUi(),
                    PresentationSettings = SearchPresentationSettings,
                    Annotation = newAnnotation,
                    Path = selected.Path,
                    Name = Path.GetFileNameWithoutExtension(selected.Path)
                };

                var r = preset.Save();
                AnnotatePresetTextBox.Text = newAnnotation;
                StateBarTextBlock.Text = String.Format("{0} preset {1}", r ? "Saved" : "Failed to save", preset.Path);
            }
        }

        /// <summary>
        /// Load a preset from XML
        /// </summary>
        /// <param name="path">Path to XML file containing the deserialized preset.</param>
        protected internal void LoadPreset(string path)
        {
            var serializer = new XmlSerializer(typeof(SearchPreset));
            try
            {
                using (var reader = new StreamReader(path))
                {
                    var searchPreset = serializer.Deserialize(reader) as SearchPreset;
                    if (searchPreset != null)
                    {
                        _searchQueryRequest = searchPreset.Request;
                        _searchConnection = searchPreset.Connection;
                        SearchPresentationSettings = searchPreset.PresentationSettings ?? new SearchResultPresentationSettings();
                        _presetAnnotation = searchPreset.Annotation;

                        InitializeControls();
                        StateBarTextBlock.Text = String.Format("Successfully read XML preset from {0}", path);
                    }
                    else
                    {
                        throw new Exception(String.Format("Failed to deserialize file: '{0}'", path));
                    }
                }
            }
            catch (Exception ex)
            {
                StateBarTextBlock.Text = String.Format("Failed to read XML from {0}: {1}", path, ex.Message);
            }

        }

        /// <summary>
        /// Load search connection setting from file and return as an SearchConnection object.
        /// </summary>
        /// <returns></returns>
        private SearchConnection LoadSearchConnection()
        {
            var connection = new SearchConnection();

            try
            {
                var path = Path.Combine(Environment.CurrentDirectory, ConnectionPropsXmlFileName);
                connection.Load(path);
            }
            catch (Exception ex)
            {
                ShowMsgBox("Failed to read connection properties. Error:" + ex.Message);
            }

            return connection;
        }

        private void UpdateSearchConnectionControls(SearchConnection connection)
        {
            if (connection == null)
            {
                return;
            }

            try
            {
                if (!string.IsNullOrEmpty(connection.SpSiteUrl)) { SharePointSiteUrlTextBox.Text = connection.SpSiteUrl; }
                if (!string.IsNullOrEmpty(connection.Timeout)) { WebRequestTimeoutTextBox.Text = connection.Timeout; }

                if (!string.IsNullOrEmpty(connection.Accept))
                {
                    switch (connection.Accept.ToLower())
                    {
                        case "json":
                            AcceptJsonRadioButton.IsChecked = true;
                            break;
                        case "xml":
                            AcceptXmlRadioButton.IsChecked = true;
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(connection.HttpMethod))
                {
                    switch (connection.HttpMethod.ToLower())
                    {
                        case "get":
                            HttpGetMethodRadioButton.IsChecked = true;
                            break;
                        case "post":
                            HttpPostMethodRadioButton.IsChecked = true;
                            break;
                    }
                }

                AuthenticationTypeComboBox.SelectedIndex = connection.AuthTypeIndex;
                AuthenticationMethodComboBox.SelectedIndex = connection.AuthMethodIndex;

                if (!string.IsNullOrEmpty(connection.HttpMethod) && AuthenticationUsernameTextBox.IsEnabled)
                {
                    AuthenticationUsernameTextBox.Text = connection.Username;
                    _searchQueryRequest.UserName = connection.Username;
                    _searchSuggestionsRequest.UserName = connection.Username;
                }

                ExperimentalFeaturesCheckBox.IsChecked = connection.EnableExperimentalFeatures;
            }
            catch (Exception ex)
            {
                ShowMsgBox("Failed to update connection properties. Error:" + ex.Message);
            }
        }

        private void LoadSearchPresetsFromFolder(string presetFolderPath)
        {
            try
            {
                var presetFilter = PresetFilterTextBox.Text;
                SearchPresets = new SearchPresetList(presetFolderPath);
                PresetComboBox.ItemsSource = SearchPresets.Presets;
            }
            catch (Exception ex)
            {
                ShowMsgBox("Failed to read search presets. Error:" + ex.Message);
            }
        }

        /// <summary>
        /// Handle event when an entry in the preset combobox is selected. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PresetComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedPreset = (PresetComboBox.SelectedItem as SearchPreset);

            // Load this preset and refresh user interface
            if (selectedPreset != null)
            {
                LoadPreset(selectedPreset.Path);
            }
        }


        private void CopyClipboardButton_OnClickButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_searchResults == null)
            {
                StateBarTextBlock.Text = "No search result to copy";
                return;
            }

            string content;
            if (AcceptJsonRadioButton.IsChecked.HasValue && AcceptJsonRadioButton.IsChecked.Value)
            {
                content = JsonHelper.FormatJson(_searchResults.ResponseContent);
                content = Regex.Replace(content, "\r\n\\s+\r\n", "\r\n");
            }
            else
            {
                content = XmlHelper.PrintXml(_searchResults.ResponseContent);
            }
            Clipboard.SetText(content);
        }

        private void PresetComboBox_OnDropDownOpened(object sender, EventArgs e)
        {
            RefreshPresetList();
        }

        public void RefreshPresetList()
        {
            try
            {
                var filter = PresetFilterTextBox.Text;
                var presets = SearchPresets.Presets.Where(p => p.Include(filter));
                PresetComboBox.ItemsSource = presets;
            }
            catch (Exception ex)
            {
                ShowMsgBox("Failed to read search presets. Error:" + ex.Message);
            }
        }


        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            History.BackButton_OnClick(sender, e);
        }

        private void ForwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            History.ForwardButton_OnClick(sender, e);
        }

        private void AnnotatePreset_OnClick(object sender, RoutedEventArgs e)
        {
            AnnotatePresetTextBox.Visibility = AnnotatePresetTextBox.IsVisible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void PresetFilterTextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            RefreshPresetList();
            PresetComboBox.IsDropDownOpen = true;
        }

        private void PresetFilterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                RefreshPresetList();
                PresetComboBox.IsDropDownOpen = true;
            }
        }

        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            History.Clear_OnClick(sender, e);
            History.PruneHistoryDir(HistoryFolderPath, 0);
            ResetCheckboxesButton_Click(sender, e);
            _searchQueryRequest = new SearchQueryRequest();
            InitializeControls();
        }
    }
}