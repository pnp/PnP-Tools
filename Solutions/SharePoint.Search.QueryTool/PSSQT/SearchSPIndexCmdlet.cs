using System;
using System.Management.Automation;
using SearchQueryTool.Model;
using SearchQueryTool.Helpers;
using System.Security.Principal;
using System.Net;
using System.IO;
using System.Linq;
using System.Collections.Specialized;
using System.Collections.Generic;
using PSSQT.Helpers;
using PSSQT.Helpers.Authentication;
using System.Threading;



/**
 * <ParameterSetName	P1	P2
 * Site                 X   X
 * Query                X   X 
 * LoadPreset	        	X
 **/

namespace PSSQT
{
    [Cmdlet(VerbsCommon.Search, "SPIndex", DefaultParameterSetName = "P1")]
    public class SearchSPIndexCmdlet
        : PSCmdlet
    {
        #region PrivateMembers
        private static readonly int SearchResultBatchSize = 500;
        private static readonly char[] trimChars = { ' ', '\t', '\n', '\r' };


        // default values for script parameters
        private static readonly int startRowDefault = 0;
        private static readonly int rowLimitDefault = 10;
        private static readonly int timeoutDefault = 60;

        private static readonly Dictionary<Guid, CookieCollection> Tokens = new Dictionary<Guid, CookieCollection>();   // SPO Auth tokens


        public enum QueryLogClientType
        {
            CSOM,
            RSS,
            Alerts,
            ObjectModelBackwardsCompatible,
            AllResultsQuery,
            PeopleResultsQuery,
            VideoResultsQuery,
            SiteResultsQuery_All,
            SiteResultsQuery_Docs,
            SiteResultsQuery_Sites,
            ContentSearchHigh,
            ContentSearchRegular,
            CatalogItemReuseQuery,
            ContentSearchLow,
            SearchWebPartConfiguration,
            DiscoverySearch,
            DiscoveryDownloadManager,
            DocsSharedWithMe,
            MyTaskSync,
            SEOSiteMapQuery,
            MySiteSecurityTrimmer,
            Monitoring,
            ReportsAndDataResultsQuery,
            InplaceListSearch,
            TrendingTagsQuery,
            Unknown
        }

        #endregion

        #region ScriptParameters
 

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            Position = 0,
            HelpMessage = "Query terms.",
            ParameterSetName = "P1"
        )]
        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            Position = 0,
            HelpMessage = "Query terms.",
            ParameterSetName = "P2"
        )]
        public string[] Query { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Credentials."
        )]
        public PSCredential Credential { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Select Properties. You can use the value :default: to retrieve the default properties from SharePoint."
        )]
        [Alias("Properties")]
        public List<string> SelectProperties { get; set; }

        [Parameter(
             Mandatory = false,
             ValueFromPipelineByPropertyName = false,
             ValueFromPipeline = false,
             HelpMessage = "Hithighlighted Properties. "
         )]
        public List<string> HithighlightedProperties { get; set; }



        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Result Offset."
        )]
        public int? StartRow { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Number of results."
        )]
        public int? RowLimit { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Request timeout in seconds. Defaults to 60s."
        )]
        public int? Timeout { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Accept Type. Accept XML or JSON."
        )]
        public AcceptType? AcceptType { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Method Type. Use GET or POST."
        )]
        public HttpMethodType? MethodType { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Select query log client type."
        )]
        public QueryLogClientType? ClientType { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "SharePoint site to connect to. If it starts with http(s)//, use directly, otherwise load from connection file. See -SaveSite",
            ParameterSetName = "P1"
        )]
        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "SharePoint site to connect to. If it starts with http(s)//, use directly, otherwise load from connection file. See -SaveSite",
            ParameterSetName = "P2"
        )]
        public string Site { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Save connection properties to file. E.g. Search-SPindex -Site https://host1/path -SaveSite host1"
        )]
        public string SaveSite { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Try to retrieve ALL results. USE WITH CAUTION!!"
        )]
        [Alias("RowLimitAll")]
        public SwitchParameter LimitAll { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Enable stemming. Default is <X>."
        )]

        public SwitchParameter EnableStemming { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Disable stemming. Please note that Disable overrides Enable. Default is <X>."
        )]

        public SwitchParameter DisableStemming { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Enable phonetic. Default is <X>."
        )]

        public SwitchParameter EnablePhonetic { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Disable phonetic. Please note that Disable overrides Enable. Default is <X>."
        )]

        public SwitchParameter DisablePhonetic { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Enable nicknames. Default is <X>."
        )]

        public SwitchParameter EnableNickNames { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Enable multi geo search. Default is false."
        )]

        public SwitchParameter EnableMultiGeoSearch { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Array of MultiGeoSearchConfigurations. The use of this implies EnableMultiGeoSearch:true."
        )]

        public MultiGeoSearchConfiguration[] MultiGeoSearchConfiguration { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Disable nicknames. Default is <X>."
        )]

        public SwitchParameter DisableNickNames { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Trim duplicates. Default is <X>."
        )]
        [Alias("TrimDuplicates")]
        public SwitchParameter EnableTrimDuplicates { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Disable Trim duplicates. Default is <X>."
        )]
        public SwitchParameter DisableTrimDuplicates { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Enable FQL. Default is <X>."
        )]

        public SwitchParameter EnableFql { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Disable FQL. Default is <X>."
        )]

        public SwitchParameter DisableFql { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Enable Query Rules. Default is <X>."
        )]

        public SwitchParameter EnableQueryRules { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Disable Query Rules. Default is <X>."
        )]

        public SwitchParameter DisableQueryRules { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Process best bets. Default is <X>."
        )]
        [Alias("ProcessBestBets")]
        public SwitchParameter EnableProcessBestBets { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Disable process best bets. Default is <X>."
        )]
        public SwitchParameter DisableProcessBestBets { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Bypass result types. Default is <X>."
        )]
        [Alias("ByPassResultTypes")]
        public SwitchParameter EnableByPassResultTypes { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Disable bypass result types. Default is <X>."
        )]
        public SwitchParameter DisableByPassResultTypes { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Process personal favorites. Default is <X>."
        )]
        [Alias("ProcessPersonalFavorites")]
        public SwitchParameter EnableProcessPersonalFavorites { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Disable process personal favorites. Default is <X>."
        )]

        public SwitchParameter DisableProcessPersonalFavorites { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Specify refiners."
        )]
        public List<string> Refiners { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Select the result processor. One of Basic, Primary, All, Raw, RankDetail, RankXML, ExplainRank, Refiners."
        )]
        public ResultProcessor? ResultProcessor { get; set; } //= ResultProcessor.Primary;


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Ranking model id."
        )]
        public string RankingModelId { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Ranking model name. Select from a list of predefined ranking models. (overrides RankingModelId)"
        )]
        public RankModelName? RankingModelName { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Sort List."
        )]
        public List<string> Sort { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Result source id."
        )]
        [Alias("SourceId")]
        public string ResultSourceId { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Result source name. Pick from a list of predefined result sources. (Overrides ResultSourceId)"
        )]
        public ResultSourceName? ResultSourceName { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Hidden constraints."
        )]
        public string HiddenConstraints { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Query template."
        )]
        public string QueryTemplate { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Specify ExplainRank result processor XSLT. (override the builtin XSLT)."
        )]
        public string ExplainRankXsltFile { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Save parameters as a preset. Load using LoadPreset"
        )]
        public string SavePreset { get; set; }


        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Load parameters from file. Use SavePreset to save a preset. Script parameters on the command line overrides.",
            ParameterSetName = "P2"
        )]
        [Alias("Preset")]
        public string LoadPreset { get; set; }

        [Parameter(
             Mandatory = false,
             ValueFromPipelineByPropertyName = false,
             ValueFromPipeline = false,
             HelpMessage = "Collapse Specification."
         )]

        public string CollapseSpecification { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Specify authentication mode."
        )]


        public PSAuthenticationMethod AuthenticationMethod { get; set; } = PSAuthenticationMethod.Windows;


        [Parameter(
             Mandatory = false,
             ValueFromPipelineByPropertyName = false,
             ValueFromPipeline = false,
             HelpMessage = "Specify number of milliseconds to sleep between each query batch (500 results) when using RowLimit > 500."
         )]


        public int SleepBetweenQueryBatches { get; set; } = 0;

        [Parameter(
             Mandatory = false,
             ValueFromPipelineByPropertyName = false,
             ValueFromPipeline = false,
             HelpMessage = "Force a login prompt when you are using -AuthenticationMode SPOManagement."
        )]


        public SwitchParameter ForceLoginPrompt { get; set; }

        #endregion

        #region Methods

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                WriteDebug("Enter ProcessRecord");

                SearchConnection searchConnection = new SearchConnection();
                SearchQueryRequest searchQueryRequest = new SearchQueryRequest();

                // Load Preset
                if (ParameterSetName == "P2")
                {
                    string path = GetPresetFilename(LoadPreset, true);

                    WriteVerbose("Loading preset " + path);

                    SearchPreset preset = new SearchPreset(path, true);

                    searchConnection = preset.Connection;
                    searchQueryRequest = preset.Request;
                }

                // Set Script Parameters from Command Line

                SetRequestParameters(searchConnection, searchQueryRequest);

                // Save Site/Preset

                if (!(String.IsNullOrWhiteSpace(SaveSite) && String.IsNullOrWhiteSpace(SavePreset)))
                {
                    if (!String.IsNullOrWhiteSpace(SaveSite))
                    {
                        SaveSiteToFile(searchConnection);
                    }

                    if (!String.IsNullOrWhiteSpace(SavePreset))
                    {
                        SavePresetToFile(searchConnection, searchQueryRequest);
                    }
                }
                else // Perform the Search
                {
                    EnsureValidQuery(searchQueryRequest);

                    // split select properties
                    //splitSelectProperties(searchQueryRequest);

                    if (LimitAll || RowLimit > SearchResultBatchSize)
                    {
                        // Try to loop through all results in increments of 500
                        searchQueryRequest.RowLimit = SearchResultBatchSize;

                        int totalRows = (StartRow.HasValue ? StartRow.Value : 0) + 1;
                        int remaining = 0;

                        while (searchQueryRequest.StartRow < totalRows)
                        {
                            ShowProgress(searchQueryRequest.StartRow.Value, totalRows, remaining);

                            totalRows = GetResults(searchQueryRequest);

                            if (!LimitAll)
                            {
                                totalRows = (RowLimit.HasValue ? RowLimit.Value : rowLimitDefault);
                            }
                            searchQueryRequest.StartRow += SearchResultBatchSize;
                            remaining = totalRows - searchQueryRequest.StartRow.Value;
                            //Console.WriteLine(remaining);
                            searchQueryRequest.RowLimit = remaining < SearchResultBatchSize ? remaining : SearchResultBatchSize;

                            if (SleepBetweenQueryBatches > 0)
                            {
                                Thread.Sleep(SleepBetweenQueryBatches);
                            }
                            
                        }
                    }
                    else
                    {
                        GetResults(searchQueryRequest);
                    }

                }

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex,
                           "SearchSPIndexError",
                           ErrorCategory.NotSpecified,
                           null)
                          );

                WriteDebug(ex.StackTrace);
            }
        }

        private void ShowProgress(int startRow, int totalRows, int remaining)
        {
            if (remaining == 0)
            {
                WriteProgress(new ProgressRecord(1, "Processing results...", "..."));
            }
            else
            {
                WriteProgress(new ProgressRecord(1, "Processing results...", String.Format(" {0} out of {1}", startRow, totalRows)));
            }
        }

        private void EnsureValidQuery(SearchQueryRequest searchQueryRequest)
        {
            if (String.IsNullOrWhiteSpace(searchQueryRequest.QueryText))
            {
                throw new Exception("Query text cannot be null.");
            }
        }

        private SearchQueryRequest SetSelectProperties(SearchQueryRequest searchQueryRequest)
        {
            searchQueryRequest.SelectProperties = new SelectPropertiesListArgumentParser(SelectProperties, searchQueryRequest).Parse();

            // Cmdlet.SelectProperties is used by ResultProcessors
            SelectProperties = searchQueryRequest.SelectProperties != null ? searchQueryRequest.SelectProperties.Split(',').ToList() : null;

            WriteVerbose(searchQueryRequest.PrintVerbose());
            WriteDebug(searchQueryRequest.PrintDebug());

            return searchQueryRequest;
        }

        private void SavePresetToFile(SearchConnection searchConnection, SearchQueryRequest searchQueryRequest)
        {
            if (!String.IsNullOrWhiteSpace(SavePreset))
            {
                SearchPreset newPreset = new SearchPreset();
                var path = GetPresetFilename(SavePreset);

                newPreset.Name = Path.GetFileNameWithoutExtension(path);
                newPreset.Path = path;

                if (!Path.IsPathRooted(SavePreset))
                {
                    newPreset.Path = Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, newPreset.Path);
                }

                newPreset.Connection = searchConnection;
                newPreset.Request = SetSelectProperties(searchQueryRequest);

                newPreset.Save();

                WriteVerbose("Configuration saved to " + newPreset.Path);
            }
        }

        private void SaveSiteToFile(SearchConnection searchConnection)
        {
            if (!(Site.StartsWith("http://") || Site.StartsWith("https://")))
            {
                throw new Exception("-Site should be a valid http(s):// URL when you use -SaveSite.");
            }

            var fileName = GetPresetFilename(SaveSite);

            WriteDebug(String.Format("Save Site: {0}", searchConnection.SpSiteUrl));

            searchConnection.SaveXml(fileName);

            WriteVerbose("Connection properties saved to " + fileName);
        }

        private void SetRequestParameters(SearchConnection searchConnection, SearchQueryRequest searchQueryRequest)
        {
            searchQueryRequest.SharePointSiteUrl = GetSPSite() ?? searchQueryRequest.SharePointSiteUrl;
            searchConnection.SpSiteUrl = searchQueryRequest.SharePointSiteUrl;

            searchQueryRequest.QueryText = GetQuery() ?? searchQueryRequest.QueryText;

            searchQueryRequest.HttpMethodType = MethodType.HasValue ? MethodType.Value : searchQueryRequest.HttpMethodType;
            searchConnection.HttpMethod = searchQueryRequest.HttpMethodType.ToString();

            searchQueryRequest.AcceptType = AcceptType.HasValue ? AcceptType.Value : searchQueryRequest.AcceptType;

            searchQueryRequest.ClientType = GetClientTypeName(searchQueryRequest);

            // This cmdlet keeps select properties as a list of strings, and SearchQueryRequest uses a string.

            // searchQueryRequest.SelectProperties is set very last thing before we execute the query in SetSelectProperties() 


            searchQueryRequest.HitHighlightedProperties = new StringListArgumentParser(HithighlightedProperties).Parse() ?? searchQueryRequest.HitHighlightedProperties;


            searchQueryRequest.SortList = new SortListArgumentParser(Sort).Parse() ?? searchQueryRequest.SortList;

            searchQueryRequest.CollapseSpecifiation = CollapseSpecification ?? searchQueryRequest.CollapseSpecifiation;


            // set based on switches
            bool? switchValue;

            switchValue = GetThreeWaySwitchValue(EnableStemming, DisableStemming);
            if (switchValue.HasValue) searchQueryRequest.EnableStemming = switchValue;

            switchValue = GetThreeWaySwitchValue(EnablePhonetic, DisablePhonetic);
            if (switchValue.HasValue) searchQueryRequest.EnablePhonetic = switchValue;

            switchValue = GetThreeWaySwitchValue(EnableNickNames, DisableNickNames);
            if (switchValue.HasValue) searchQueryRequest.EnableNicknames = switchValue;

            if (EnableMultiGeoSearch || MultiGeoSearchConfiguration != null)
            {
                searchQueryRequest.EnableMultiGeoSearch = true;

                if (MultiGeoSearchConfiguration != null)
                {
                    searchQueryRequest.MultiGeoSearchConfiguration = PSSQT.MultiGeoSearchConfiguration.Format(MultiGeoSearchConfiguration);
                }
            }

            switchValue = GetThreeWaySwitchValue(EnableTrimDuplicates, DisableTrimDuplicates);
            if (switchValue.HasValue) searchQueryRequest.TrimDuplicates = switchValue;

            switchValue = GetThreeWaySwitchValue(EnableFql, DisableFql);
            if (switchValue.HasValue) searchQueryRequest.EnableFql = switchValue;

            switchValue = GetThreeWaySwitchValue(EnableQueryRules, DisableQueryRules);
            if (switchValue.HasValue) searchQueryRequest.EnableQueryRules = switchValue;

            switchValue = GetThreeWaySwitchValue(EnableProcessBestBets, DisableProcessBestBets);
            if (switchValue.HasValue) searchQueryRequest.ProcessBestBets = switchValue;

            switchValue = GetThreeWaySwitchValue(EnableByPassResultTypes, DisableByPassResultTypes);
            if (switchValue.HasValue) searchQueryRequest.ByPassResultTypes = switchValue;

            switchValue = GetThreeWaySwitchValue(EnableProcessPersonalFavorites, DisableProcessPersonalFavorites);
            if (switchValue.HasValue) searchQueryRequest.ProcessPersonalFavorites = switchValue;

            searchQueryRequest.RankingModelId = RankingModelId ?? searchQueryRequest.RankingModelId;

            if (RankingModelName.HasValue)
            {
                if (!String.IsNullOrWhiteSpace(RankingModelId))
                {
                    WriteWarning("RankingModelName overrides RankingModelId. Please use one or the other.");
                }

                searchQueryRequest.RankingModelId = RankModel.Select(RankingModelName.Value);
            }

            searchQueryRequest.SourceId = ResultSourceId ?? searchQueryRequest.SourceId;

            if (ResultSourceName.HasValue)
            {
                if (!String.IsNullOrWhiteSpace(ResultSourceId))
                {
                    WriteWarning("ResultSourceName overrides ResutSourceId. Please use one or the other.");
                }

                searchQueryRequest.SourceId = ResultSource.Select(ResultSourceName.Value);
            }

            searchQueryRequest.HiddenConstraints = HiddenConstraints ?? searchQueryRequest.HiddenConstraints;
            searchQueryRequest.QueryTemplate = QueryTemplate ?? searchQueryRequest.QueryTemplate;

            searchQueryRequest.RowLimit = RowLimit.HasValue ? RowLimit : (searchQueryRequest.RowLimit.HasValue ? searchQueryRequest.RowLimit : rowLimitDefault);
            searchQueryRequest.StartRow = StartRow.HasValue ? StartRow : (searchQueryRequest.StartRow.HasValue ? searchQueryRequest.StartRow : startRowDefault);
            searchQueryRequest.Timeout = Timeout.HasValue ? Timeout : (searchQueryRequest.Timeout.HasValue ? searchQueryRequest.Timeout : timeoutDefault);

            searchQueryRequest.Refiners = new StringListArgumentParser(Refiners).Parse() ?? searchQueryRequest.Refiners;

            SetRequestAutheticationType(searchQueryRequest);
        }

        private void SetRequestAutheticationType(SearchQueryRequest searchQueryRequest)
        {
            if (Credential != null || searchQueryRequest.AuthenticationType == AuthenticationType.Windows)
            {
                if (Credential == null)
                {
                    var userName = searchQueryRequest.UserName;

                    Credential = this.Host.UI.PromptForCredential("Enter username/password", "", userName, "");
                }

                searchQueryRequest.AuthenticationType = AuthenticationType.Windows;
                searchQueryRequest.UserName = Credential.UserName;
                searchQueryRequest.SecurePassword = Credential.Password;
            }
            else if (searchQueryRequest.AuthenticationType == AuthenticationType.SPO)
            {
                SPOLegacyLogin(searchQueryRequest);
            }
            else if (AuthenticationMethod == PSAuthenticationMethod.SPOManagement || searchQueryRequest.AuthenticationType == AuthenticationType.SPOManagement)
            {
                AdalLogin(searchQueryRequest, ForceLoginPrompt.IsPresent);
                //searchSuggestionsRequest.Token = token;
            }
            else
            {
                searchQueryRequest.AuthenticationType = AuthenticationType.CurrentUser;
                WindowsIdentity currentWindowsIdentity = WindowsIdentity.GetCurrent();
                searchQueryRequest.UserName = currentWindowsIdentity.Name;
            }
        }

        internal void SPOLegacyLogin(SearchQueryRequest searchQueryRequest)
        {
            Guid runspaceId = Guid.Empty;
            using (var ps = PowerShell.Create(RunspaceMode.CurrentRunspace))
            {
                runspaceId = ps.Runspace.InstanceId;

                CookieCollection cc;

                bool found = Tokens.TryGetValue(runspaceId, out cc);

                if (!found)
                {
                    cc = PSWebAuthentication.GetAuthenticatedCookies(this, searchQueryRequest.SharePointSiteUrl, AuthenticationType.SPO);

                    if (cc == null)
                    {
                        throw new RuntimeException("Authentication cookie returned is null! Authentication failed. Please try again.");  // TODO find another exception
                    }
                    else
                    {
                        Tokens.Add(runspaceId, cc);
                    }
                }

                searchQueryRequest.AuthenticationType = AuthenticationType.SPO;
                searchQueryRequest.Cookies = cc;
                //searchSuggestionsRequest.Cookies = cc;
            }
        }

        internal static void AdalLogin(SearchQueryRequest searchQueryRequest, bool forceLogin)
        {
            AdalAuthentication adalAuth = new AdalAuthentication();

            var task = adalAuth.Login(searchQueryRequest.SharePointSiteUrl, forceLogin);

            if (!task.Wait(300000))
            {
                throw new TimeoutException("Prompt for user credentials timed out after 5 minutes.");
            }

            var token = task.Result;

            searchQueryRequest.AuthenticationType = AuthenticationType.SPOManagement;
            searchQueryRequest.Token = token;
        }

        private string DefaultClientTypeName()
        {
            // ContentSearchRegular prevents cluttering the page impression table and seems appropriate as default when scripting
            // http://www.techmikael.com/2015/05/always-set-client-type-on-sharepoint.html

            return Enum.GetName(typeof(QueryLogClientType), QueryLogClientType.ContentSearchRegular);
        }

        private string GetClientTypeName(SearchQueryRequest searchQueryRequest)
        {
            return ClientType.HasValue ? Enum.GetName(typeof(QueryLogClientType), ClientType) :
                (string.IsNullOrWhiteSpace(searchQueryRequest.ClientType) ? DefaultClientTypeName() : searchQueryRequest.ClientType);
        }

        private string GetQuery()
        {
            return Query == null ? null : String.Join(" ", Query);
        }


        private int GetResults(SearchQueryRequest searchQueryRequest)
        {
            int totalRows = 0;
            bool keepTrying = true;
 

            // Pick default result processor
            if (!ResultProcessor.HasValue)     // user has not specified one
            {
                if (Refiners != null)
                {
                    ResultProcessor = PSSQT.ResultProcessor.Refiners;
                }
                else
                {
                    ResultProcessor = PSSQT.ResultProcessor.Primary;
                }

                WriteVerbose(String.Format("Using ResultProcessor {0}", Enum.GetName(typeof(ResultProcessor), ResultProcessor)));
            }



            IQueryResultProcessor queryResultProcessor = QueryResultProcessorFactory.SelectQueryResultProcessor(ResultProcessor.Value, this, searchQueryRequest);

            queryResultProcessor.Configure();    // May add required properties to retrieve, modify the searchQueryRequest (e.g. rankdetail etc.)

            while (keepTrying)
            {
                keepTrying = false;

                try
                {
                    var requestResponsePair = HttpRequestRunner.RunWebRequest(SetSelectProperties(searchQueryRequest));

                    var queryResults = requestResponsePair.GetResultItem<SearchQueryResult>();

                    totalRows = queryResults.PrimaryQueryResult.TotalRows;

                    queryResultProcessor.Process(queryResults);
                }
                catch (RankDetailTooManyResults ex)
                {
                    WriteWarning("More than 100 results in result set. Resubmitting query with filter to get RankDetail.");

                    searchQueryRequest.QueryText += ex.QueryFilter;
                    keepTrying = true;
                }
                catch (Exception ex)
                {
                    if (!queryResultProcessor.HandleException(ex, searchQueryRequest))
                    {
                        throw;
                    }

                    // if exception was handled, we will try again
                    keepTrying = true;
                }

            }

            return totalRows;
        }

        public void AddSelectProperty(string property)
        {
            string trimmedProperty = property.Trim();

            if (SelectProperties == null)
            {
                SelectProperties = new List<string>();
            }

            if (!SelectProperties.Contains(trimmedProperty, StringComparer.InvariantCultureIgnoreCase))
            {
                SelectProperties.Add(trimmedProperty);
            }

        }

        private static bool? GetThreeWaySwitchValue(SwitchParameter enable, SwitchParameter disable)
        {
            bool? result = null;

            if (enable) result = true;
            if (disable) result = false;    // disable overrides enable
                                            // else  result = null which means use default value
            return result;
        }


        private string GetPresetFilename(string presetName, bool searchPath = false)
        {
            string path = presetName;

            if (!path.EndsWith(".xml"))
            {
                path += ".xml";
            }

            if (searchPath && !Path.IsPathRooted(path))
            {
                // always check current directory first
                var rootedPath = GetRootedPath(path);

                if (!File.Exists(rootedPath))
                {
                    var environmentVariable = Environment.GetEnvironmentVariable("PSSQT_PresetsPath");

                    if (environmentVariable != null)
                    {
                        var result = environmentVariable
                            .Split(';')
                            .Where(s => File.Exists(Path.Combine(s, path)))
                            .FirstOrDefault();

                        if (result == null)
                        {
                            throw new ArgumentException(String.Format("File \"{0}\" not found in current directory or PSSQT_PresetsPath", path));
                        }

                        return Path.Combine(result, path);
                    }
                }

                return rootedPath;
            }


            return GetRootedPath(path);
        }

        internal string GetRootedPath(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, path);
            }

            return path;
        }


        private string GetSPSite()
        {
            if (String.IsNullOrWhiteSpace(Site) || Site.StartsWith("http://") || Site.StartsWith("https://"))
            {
                return Site;
            }


            var fileName = GetPresetFilename(Site);

            SearchConnection sc = new SearchConnection();

            sc.Load(fileName);

            return sc.SpSiteUrl;
        }
        #endregion
    }
}
