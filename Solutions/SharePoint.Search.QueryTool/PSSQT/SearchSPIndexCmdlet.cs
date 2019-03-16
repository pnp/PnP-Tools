using PSSQT.Helpers;
using SearchQueryTool.Helpers;
using SearchQueryTool.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
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
        : AbstractSearchSPCmdlet<SearchQueryRequest>
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
            HelpMessage = "Select query log client type."
        )]
        public QueryLogClientType? ClientType { get; set; }


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
            HelpMessage = "Enable multi geo search."
        )]

        public SwitchParameter EnableMultiGeoSearch { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Disable multi geo search."
        )]

        public SwitchParameter DisableMultiGeoSearch { get; set; }

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
            HelpMessage = "Specify refiners."
        )]
        public List<string> Refiners { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Select the result processor. One of Basic, BasicAll, Primary, All, Raw, RankDetail, RankXML, ExplainRank, Refiners, FormatResults,..."
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
             HelpMessage = "Specify number of milliseconds to sleep between each query batch (500 results) when using RowLimit > 500."
         )]
        public int SleepBetweenQueryBatches { get; set; } = 0;

        [Parameter(
             Mandatory = false,
             ValueFromPipelineByPropertyName = false,
             ValueFromPipeline = false,
             HelpMessage = "Include a result block with personal OneDrive results when searching SPO."
         )]
        public SwitchParameter IncludePersonalOneDriveResults { get; set; }

        #endregion

        #region Methods

        protected override void SetRequestParameters(SearchQueryRequest searchQueryRequest)
        {
            base.SetRequestParameters(searchQueryRequest);

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

            if (EnableMultiGeoSearch || MultiGeoSearchConfiguration != null || DisableMultiGeoSearch)
            {
                if (EnableMultiGeoSearch || MultiGeoSearchConfiguration != null)
                {
                    searchQueryRequest.EnableMultiGeoSearch = true;
                }

                if (MultiGeoSearchConfiguration != null)
                {
                    searchQueryRequest.MultiGeoSearchConfiguration = PSSQT.MultiGeoSearchConfiguration.Format(MultiGeoSearchConfiguration, MethodType == HttpMethodType.Post);
                }

                if (DisableMultiGeoSearch)
                {
                    searchQueryRequest.EnableMultiGeoSearch = false;
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

            searchQueryRequest.IncludePersonalOneDriveResults = IncludePersonalOneDriveResults.IsPresent ? true : (searchQueryRequest.IncludePersonalOneDriveResults.HasValue ? searchQueryRequest.IncludePersonalOneDriveResults : false);

            searchQueryRequest.Refiners = new StringListArgumentParser(Refiners).Parse() ?? searchQueryRequest.Refiners;
        }
 
        protected override void PresetLoaded(ref SearchQueryRequest searchRequest, SearchPreset preset)
        {
            searchRequest = preset.Request;
        }

        protected override void ExecuteRequest(SearchQueryRequest searchRequest)
        {
            if (LimitAll || RowLimit > SearchResultBatchSize)
            {
                // Try to loop through all results in increments of 500
                searchRequest.RowLimit = SearchResultBatchSize;

                int totalRows = (StartRow.HasValue ? StartRow.Value : 0) + 1;
                int remaining = 0;

                while (searchRequest.StartRow < totalRows)
                {
                    ShowProgress(searchRequest.StartRow.Value, totalRows, remaining);

                    totalRows = GetResults(searchRequest);

                    if (!LimitAll)
                    {
                        totalRows = (RowLimit.HasValue ? RowLimit.Value : rowLimitDefault);
                    }
                    searchRequest.StartRow += SearchResultBatchSize;
                    remaining = totalRows - searchRequest.StartRow.Value;
                    //Console.WriteLine(remaining);
                    searchRequest.RowLimit = remaining < SearchResultBatchSize ? remaining : SearchResultBatchSize;

                    if (SleepBetweenQueryBatches > 0)
                    {
                        Thread.Sleep(SleepBetweenQueryBatches);
                    }

                }
            }
            else
            {
                GetResults(searchRequest);
            }
        }

        protected override void SaveRequestPreset(SearchConnection searchConnection, SearchQueryRequest searchRequest)
        {
            if (IsSavePreset())
            {
                SearchPreset newPreset = new SearchPreset();
                var path = GetPresetFilename(SavePreset);

                newPreset.Name = Path.GetFileNameWithoutExtension(path);
                newPreset.Path = path;

                if (!Path.IsPathRooted(SavePreset))
                {
                    newPreset.Path = Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, newPreset.Path);
                }

                searchConnection.CopyFrom(searchRequest);

                newPreset.Connection = searchConnection;
                newPreset.Request = SetSelectProperties(searchRequest);

                newPreset.Save();

                WriteInformation(new HostInformationMessage
                {
                    Message = $"Preset saved to {newPreset.Path}",
                    ForegroundColor = Host.UI.RawUI.BackgroundColor,        // invert
                    BackgroundColor = Host.UI.RawUI.ForegroundColor,
                    NoNewLine = false
                }, new[] { "PSHOST" });
            }
        }

        protected override bool IsSavePreset()
        {
            return !String.IsNullOrWhiteSpace(SavePreset);
        }

        protected internal void AddSelectProperty(string property)
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

        private SearchQueryRequest SetSelectProperties(SearchQueryRequest searchQueryRequest)
        {
            searchQueryRequest.SelectProperties = new SelectPropertiesListArgumentParser(SelectProperties, searchQueryRequest).Parse();

            // Cmdlet.SelectProperties is used by ResultProcessors
            SelectProperties = searchQueryRequest.SelectProperties != null ? searchQueryRequest.SelectProperties.Split(',').ToList() : null;

            WriteVerbose(searchQueryRequest.PrintVerbose());
            WriteDebug(searchQueryRequest.PrintDebug());

            return searchQueryRequest;
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

        #endregion
    }
}
