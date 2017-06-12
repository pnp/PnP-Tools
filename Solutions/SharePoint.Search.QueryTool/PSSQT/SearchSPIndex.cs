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


/**
 * <ParameterSetName	P1	P2
 * Site                 X   X
 * Query                X   X 
 * LoadPreset	        	X
 **/

namespace PSSQT
{
    [Cmdlet(VerbsCommon.Search, "SPIndex",DefaultParameterSetName = "P1")]
    public class SearchSPIndex
        : PSCmdlet
    {
        #region PrivateMembers
        private static readonly int SearchResultBatchSize = 500;
        private static readonly char[] trimChars = { ' ', '\t', '\n', '\r' };
        private static readonly string sortDescending = ":descending";
        private static readonly string sortAscending = ":ascending";

        // default values for script parameters
        private static readonly string selectPropertiesDefault = "Title,Path,WorkId";
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
            HelpMessage = "Select Properties."
        )]
        [Alias("Properties")]
        public List<string> SelectProperties { get; set; }



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
        public string Refiners { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Select the result processor. One of Basic, Primary, All, Raw, RankDetail, RankXML, ExplainRank, Refiners."
        )]
        public ResultProcessor ResultProcessor { get; set; } = ResultProcessor.Primary;


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
        public string Sort { get; set; }


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
            HelpMessage = "Use this if you are connecting to SPO."
        )]

        public SwitchParameter SPO { get; set; }


        #endregion

        #region Methods

        public string SelectPropertiesAsString
        {
            get
            {
                return SelectProperties == null ?  null : string.Join(",", SelectProperties);
            }

            private set
            {
                AddProperty(value);
            }
        }

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
                    string path = GetPresetFilename(LoadPreset);

                    WriteDebug("Loading preset " + path);

                    SearchPreset preset = new SearchPreset(path, true);

                    searchConnection = preset.Connection;
                    searchQueryRequest = preset.Request;

                    WriteDebug("Finished loading preset " + path);
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
                    WriteVerbose(searchQueryRequest.PrintVerbose());
                    WriteDebug(searchQueryRequest.PrintDebug());

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
                WriteDebug(ex.StackTrace);

                WriteError(new ErrorRecord(ex,
                           "SearchSPIndexError",
                           ErrorCategory.NotSpecified,
                           null)
                          );
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
            WriteDebug(searchQueryRequest.ToString());

            if (String.IsNullOrWhiteSpace(searchQueryRequest.QueryText))
            {
                throw new Exception("Query text cannot be null.");
            }
        }

        private SearchQueryRequest Up2Date(SearchQueryRequest searchQueryRequest)
        {
            var copy = SelectProperties;
            SelectProperties = new List<string>();

            foreach (var p in copy)
            {
                SelectProperties.Add(p.ToLower());
            }

            searchQueryRequest.SelectProperties = SelectPropertiesAsString;

            WriteVerbose("Properties: " + searchQueryRequest.SelectProperties);

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
                newPreset.Request = Up2Date(searchQueryRequest);

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
            // We always update the list internally, and set the searchQueryRequest string in the Up2Date method which is called last thing before searchQueryRequest is used.
            if (SelectProperties == null || SelectProperties.Count == 0)
            {
                SelectPropertiesAsString = searchQueryRequest.SelectProperties ?? selectPropertiesDefault;
            }
            else
            {
                NormalizeProperties();
            }

            searchQueryRequest.SortList = GetSortList() ?? searchQueryRequest.SortList;

            // set based on switches
            bool? switchValue;

            switchValue = GetThreeWaySwitchValue(EnableStemming, DisableStemming);
            if (switchValue.HasValue) searchQueryRequest.EnableStemming = switchValue;

            switchValue = GetThreeWaySwitchValue(EnablePhonetic, DisablePhonetic);
            if (switchValue.HasValue) searchQueryRequest.EnablePhonetic = switchValue;

            switchValue = GetThreeWaySwitchValue(EnableNickNames, DisableNickNames);
            if (switchValue.HasValue) searchQueryRequest.EnableNicknames = switchValue;

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
            searchQueryRequest.Refiners = Refiners ?? searchQueryRequest.Refiners;

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
            else if (SPO || searchQueryRequest.AuthenticationType == AuthenticationType.SPO)
            {
                Guid runspaceId = Guid.Empty;
                using (var ps = PowerShell.Create(RunspaceMode.CurrentRunspace))
                {
                    runspaceId = ps.Runspace.InstanceId;

                    CookieCollection cc;
                    
                    bool found = Tokens.TryGetValue(runspaceId, out cc);

                    if (! found)
                    {
                        //cc = WebAuthentication.GetAuthenticatedCookies(searchQueryRequest.SharePointSiteUrl, searchQueryRequest.AuthenticationType);
                        cc = SPOClientWebAuth.GetAuthenticatedCookies(searchQueryRequest.SharePointSiteUrl);

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
            else
            {
                searchQueryRequest.AuthenticationType = AuthenticationType.CurrentUser;
                WindowsIdentity currentWindowsIdentity = WindowsIdentity.GetCurrent();
                searchQueryRequest.UserName = currentWindowsIdentity.Name;
            }
        }

        private void NormalizeProperties()
        {
            if (SelectProperties != null && SelectProperties.Count > 0)
            {
                List<string> copy = SelectProperties;

                SelectProperties = null;

                foreach (var property in copy)
                {
                    AddProperty(property);
                }
            }
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

        private string GetSortList()
        {
            if (!String.IsNullOrWhiteSpace(Sort))
            {
                var elements = Sort.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                List<string> sl = new List<string>(elements.Length);

                foreach (var e in elements)
                {
                    if (e.EndsWith(sortDescending) || e.EndsWith(sortAscending))
                    {
                        sl.Add(e);
                    }
                    else
                    {
                        var parts = e.Split(':');

                        if (parts.Length > 1)
                        {
                            if (sortAscending.StartsWith(String.Format(":{0}", parts[1])))
                            {
                                sl.Add(String.Format("{0}{1}", parts[0], sortAscending));
                            }
                            else if (sortDescending.StartsWith(String.Format(":{0}", parts[1])))
                            {
                                sl.Add(String.Format("{0}{1}", parts[0], sortDescending));
                            }
                            else
                            {
                                throw new Exception(String.Format("Unrecognized sort direction specifier: {0}, Use {1} or {2}", parts[1], sortAscending, sortDescending));
                            }
                        }
                        else
                        {
                            sl.Add(String.Format("{0}{1}", e, sortAscending));
                        }
                    }
                }

                Sort = String.Join(",", sl);
            }

            WriteDebug("Sort: " + Sort);

            return Sort;
        }

        private int GetResults(SearchQueryRequest searchQueryRequest)
        {
            int totalRows = 0;
            bool keepTrying = true;

            IQueryResultProcessor queryResultProcessor = QueryResultProcessorFactory.SelectQueryResultProcessor(ResultProcessor, this);

            queryResultProcessor.Configure();    // May add required properties to retrieve (e.g. rankdetail etc.)

            while (keepTrying)
            {
                keepTrying = false;

                try
                {
                    var requestResponsePair = HttpRequestRunner.RunWebRequest(Up2Date(searchQueryRequest));

                    var queryResults = GetResultItem(requestResponsePair);

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
                    if (! queryResultProcessor.HandleException(ex))
                    {
                        throw;
                    }
                }

            }

            return totalRows;
        }

        public void AddProperty(string property)
        {
            string input = property.Trim();

            if (!string.IsNullOrWhiteSpace(input))
            {
                if (SelectProperties == null)
                {
                    SelectProperties = new List<string>();
                }

                // For backward compatibility, property can be a comma separated string of properties. We'll split and add each of them

                var props = input.Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var prop in props)
                {
                    if (!SelectProperties.Contains(prop, StringComparer.InvariantCultureIgnoreCase))
                    {
                        SelectProperties.Add(prop);
                    }
                }

                SelectProperties.ForEach(e => e.ToLower());
            }
        }

        private SearchQueryResult GetResultItem(HttpRequestResponsePair requestResponsePair)
        {
            SearchQueryResult searchResults;
            var request = requestResponsePair.Item1;

            using (var response = requestResponsePair.Item2)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(String.Format("HTTP {0}: {1}", (int)response.StatusCode, response.StatusDescription));
                }

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

        private static bool? GetThreeWaySwitchValue(SwitchParameter enable, SwitchParameter disable)
        {
            bool? result = null;

            if (enable) result = true;
            if (disable) result = false;    // disable overrides enable
                                            // else  result = null which means use default value
            return result;
        }

        private string GetPresetFilename(string presetName)
        {
            string path = presetName;

            if (!path.EndsWith(".xml"))
            {
                path += ".xml";
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
