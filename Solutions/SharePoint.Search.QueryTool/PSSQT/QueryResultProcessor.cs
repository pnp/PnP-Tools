using PSSQT.Helpers;
using PSSQT.RankLogParser;
using SearchQueryTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace PSSQT
{
    public enum ResultProcessor
    {
        Basic,
        BasicAll,
        Primary,
        All,
        Refiners,
        Raw,
        RankDetail,
        RankXML,
        ExplainRank,
        RankContribution,
        AllProperties,
        AllPropertiesInline,
        ManagedProperties,
        CrawledProperties,
        FormatResults
    }

    public interface IQueryResultProcessor
    {
        void Configure();

        void Process(SearchQueryResult searchQueryResult);

        bool HandleException(Exception ex, SearchQueryRequest searchQueryRequest);

    }

    public static class QueryResultProcessorFactory
    {
        public static IQueryResultProcessor SelectQueryResultProcessor(ResultProcessor type, SearchSPIndexCmdlet cmdlet, SearchQueryRequest searchQueryRequest)
        {
            IQueryResultProcessor qrp = null;

            switch (type)
            {
                case ResultProcessor.Raw:
                    qrp = new RawResultProcessor(cmdlet);
                    break;

                case ResultProcessor.Primary:
                    qrp = new PrimaryResultsResultProcessor(cmdlet);
                    break;

                case ResultProcessor.All:
                    qrp = new AllResultsResultProcessor(cmdlet);
                    break;

                case ResultProcessor.Refiners:
                    qrp = new RefinerResultProcessor(cmdlet);
                    break;

                case ResultProcessor.Basic:
                    qrp = new BasicResultProcessor(cmdlet);
                    break;

                case ResultProcessor.BasicAll:
                    qrp = new BasicAllResultProcessor(cmdlet);
                    break;

                case ResultProcessor.RankXML:
                    qrp = new RankXMLResultProcessor(cmdlet);
                    break;

                case ResultProcessor.RankDetail:
                    qrp = new RankDetailResultProcessor(cmdlet);
                    break;

                case ResultProcessor.ExplainRank:
                    qrp = new ExplainRankResultProcessor(cmdlet);
                    break;

                case ResultProcessor.RankContribution:
                    qrp = new RankContributionresultProcessor(cmdlet);
                    break;

                case ResultProcessor.AllProperties:
                    qrp = new AllPropertiesResultProcessor(cmdlet, searchQueryRequest);
                    break;

                case ResultProcessor.AllPropertiesInline:
                    qrp = new AllPropertiesInlineResultProcessor(cmdlet, searchQueryRequest);
                    break;

                case ResultProcessor.ManagedProperties:
                    qrp = new ManagedPropertiesResultProcessor(cmdlet, searchQueryRequest);
                    break;

                case ResultProcessor.CrawledProperties:
                    qrp = new CrawledPropertiesResultProcessor(cmdlet, searchQueryRequest);
                    break;

                case ResultProcessor.FormatResults:
                    qrp = new FormatResultsResultProcessor(cmdlet);
                    break;

                default:
                    throw new NotImplementedException("No result processor match " + type);
            }

            return qrp;
        }

    }

    public abstract class AbstractQueryResultProcessor : IQueryResultProcessor
    {

        protected RetryHandler retryHandler;

        public AbstractQueryResultProcessor(SearchSPIndexCmdlet cmdlet)
        {
            this.Cmdlet = cmdlet;
            retryHandler = new RetryHandler(Cmdlet);
        }

        public SearchSPIndexCmdlet Cmdlet { get; private set; }

        public abstract void Process(SearchQueryResult searchQueryResult);

        public virtual void Configure()
        {
            UserNotification();

            EnsurePropertiesPresent();
        }

        protected virtual void EnsurePropertiesPresent()
        {
            // do nothing by default
        }

        protected virtual void UserNotification()
        {
            // do nothing by default
        }

        public virtual bool HandleException(Exception ex, SearchQueryRequest searchQueryRequest)
        {
            ExceptionHandler handler = ExceptionHandlerFactory.Create(Cmdlet, ex);

            retryHandler.DelegateHandler = handler;

            return retryHandler.HandleException(ex, searchQueryRequest);
        }

 
        public bool IsEndOfPipeline()
        {
            return Cmdlet.MyInvocation.PipelinePosition == Cmdlet.MyInvocation.PipelineLength;
        }

    }

    public class BaseQueryResultProcessor : AbstractQueryResultProcessor
    {
        public BaseQueryResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {
        }

        public List<string> SelectedProperties
        {
            get
            {
                // this will throw an exception if we implement something other that SearchSPIndex in the future. Serves as a reminder to look into this.
                return ((SearchSPIndexCmdlet)Cmdlet).SelectProperties;
            }
        }

        public override void Process(SearchQueryResult queryResults)
        {
            ProcessSearchQueryResultInfo(queryResults);


            if (queryResults.PrimaryQueryResult.TotalRows == 0)
            {
                Cmdlet.WriteWarning("The query returned zero results.");
            }
            else
            {
                ProcessPrimaryQueryResults(queryResults.PrimaryQueryResult);
            }

            if (queryResults.SecondaryQueryResults != null)
            {
                ProcessSecondaryQueryResults(queryResults.SecondaryQueryResults);
            }

            if (queryResults.TriggeredRules != null)
            { 
                ProcessTriggeredRules(queryResults.TriggeredRules);
            }

        }

        protected virtual void ProcessPrimaryQueryResults(QueryResult primaryQueryResult)
        {
            ProcessPrimaryQueryResultsInfo(primaryQueryResult);
            ProcessPrimaryRelevantResults(primaryQueryResult.RelevantResults);
            ProcessPrimaryRefinerResults(primaryQueryResult.RefinerResults);
        }

        protected virtual void ProcessSecondaryQueryResults(List<QueryResult> secondaryQueryResults)
        {
            ProcessSecondaryQueryResultsListInfo(secondaryQueryResults);

            if (secondaryQueryResults != null)
            {
                int resultGroup = 2;   // Primary results are result group 1
                foreach (var queryResult in secondaryQueryResults)
                {
                    ProcessSecondaryQueryResultsInfo(resultGroup, queryResult);
                    ProcessSecondaryRelevantResults(resultGroup, queryResult.RelevantResults);
                    ProcessSecondryRefinerResults(resultGroup, queryResult.RefinerResults);

                    resultGroup++;
                }
            }
        }

        protected virtual void ProcessTriggeredRules(List<string> triggeredRules)
        {
            WriteVerbose(String.Format("Number of triggered rules: {0}", triggeredRules != null ? triggeredRules.Count : 0));
        }



        protected virtual void ProcessSearchQueryResultInfo(SearchQueryResult queryResults)
        {
            WriteVerbose(String.Format("HTTP status code: {0}", queryResults.StatusCode));
            WriteVerbose(String.Format("Status description: {0}", queryResults.StatusDescription));
            WriteVerbose(String.Format("HTTP protocol version: {0}", queryResults.HttpProtocolVersion));
            WriteVerbose(String.Format("Response content type: {0}", queryResults.ContentType));

            Cmdlet.WriteDebug(String.Format("Response headers: {0}", String.Join(",", queryResults.ResponseHeaders.AllKeys)));
        }


        protected virtual void ProcessPrimaryQueryResultsInfo(QueryResult primaryQueryResult)
        {
            WriteVerbose(String.Format("Total rows: {0}", primaryQueryResult.TotalRows));
            WriteVerbose(String.Format("Total rows incl. duplicates: {0}", primaryQueryResult.TotalRowsIncludingDuplicates));
            WriteVerbose(String.Format("Query id: {0}", primaryQueryResult.QueryId));
            WriteVerbose(String.Format("Query modification: {0}", primaryQueryResult.QueryModification));
            WriteVerbose(String.Format("Query rule id: {0}", primaryQueryResult.QueryRuleId));
        }

        protected virtual void ProcessPrimaryRefinerResults(List<RefinerResult> refinerResults)
        {
            //WriteInfo(String.Format("Number of refiners: {0}", refinerResults != null ? refinerResults.Count : 0));
        }

        protected virtual void ProcessPrimaryRelevantResults(List<ResultItem> relevantResults)
        {
            WriteVerbose(String.Format("Number of relevant primary results: {0}", relevantResults != null ? relevantResults.Count : 0));
        }

        protected virtual void ProcessSecondaryQueryResultsListInfo(List<QueryResult> secondaryQueryResults)
        {
            WriteVerbose(String.Format("Number of secondary result groups: {0}", secondaryQueryResults != null ? secondaryQueryResults.Count : 0));
        }

        protected virtual void ProcessSecondaryQueryResultsInfo(int resultGroup, QueryResult queryResult)
        {
            WriteVerbose(String.Format("Total rows in group {1}: {0}", queryResult.TotalRows, resultGroup));
            WriteVerbose(String.Format("Total rows incl. duplicates in group {1}: {0}", queryResult.TotalRowsIncludingDuplicates, resultGroup));
            WriteVerbose(String.Format("Query id group {1}: {0}", queryResult.QueryId, resultGroup));
            WriteVerbose(String.Format("Query modification in group {1}: {0}", queryResult.QueryModification, resultGroup));
            WriteVerbose(String.Format("Query rule id in group {1}: {0}", queryResult.QueryRuleId, resultGroup));

            if (! String.IsNullOrWhiteSpace(queryResult.ResultTitle))
            {
                WriteVerbose(String.Format("Result title in group {1}: {0}", queryResult.ResultTitle, resultGroup));
            }

            if (! String.IsNullOrWhiteSpace(queryResult.ResultTitleUrl))
            {
                WriteVerbose(String.Format("Result title url in group {1}: {0}", queryResult.ResultTitleUrl, resultGroup));
            }
        }

        protected virtual void ProcessSecondaryRelevantResults(int resultGroup, List<ResultItem> relevantResults)
        {
            WriteVerbose(String.Format("Number of relevant secondary results in group {1}: {0}", relevantResults != null ? relevantResults.Count : 0, resultGroup));
        }

        protected virtual void ProcessSecondryRefinerResults(int resultGroup, List<RefinerResult> refinerResults)
        {
            //throw new NotImplementedException();
        }

        protected virtual void WriteVerbose(string text)
        {
            Cmdlet.WriteVerbose(text);
        }
    }

 
    /*
     * I don't want to break backwards compatibility, so I'l leaving Basic as is and adding BasicAll instead
     */

    public class BasicAllResultProcessor : BaseQueryResultProcessor
    {
        public BasicAllResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {

        }

        public override void Process(SearchQueryResult queryResults)
        {
            var item = new PSObject();

            PopulateItemFromSearchQueryResult(queryResults, item);

            if (queryResults.PrimaryQueryResult != null)
            {
                PopulateItemFromQueryResult(queryResults.PrimaryQueryResult, item, 1);
            }

            PopulateItemFromSecondaryResults(queryResults, item);

            Cmdlet.WriteObject(item);
        }

        protected virtual void PopulateItemFromSecondaryResults(SearchQueryResult queryResults, PSObject item)
        {
            if (queryResults.SecondaryQueryResults != null)
            {
                int resultGroup = 2;   // Primary results are result group 1

                foreach (var queryResult in queryResults.SecondaryQueryResults)
                {
                    PopulateItemFromQueryResult(queryResult, item, resultGroup);

                    resultGroup++;
                }

            }
        }

        protected virtual void PopulateItemFromSearchQueryResult(SearchQueryResult queryResults, PSObject item)
        {
            item.Properties.Add(new PSVariableProperty(new PSVariable("StatusCode", queryResults.StatusCode)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("StatusDescription", queryResults.StatusDescription)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("HTTPProtocalVersion", queryResults.HttpProtocolVersion)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("ContentType", queryResults.ContentType)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("ElapsedTime", queryResults.ElapsedTime)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("ElapsedMilliseconds", queryResults.ElapsedMilliseconds)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("QueryElapsedTime", queryResults.QueryElapsedTime)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("NumSecondaryResults", queryResults.SecondaryQueryResults != null ? queryResults.SecondaryQueryResults.Count : 0)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("NumTriggeredRules", queryResults.TriggeredRules != null ? queryResults.TriggeredRules.Count : 0)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("TriggeredRules", queryResults.TriggeredRules)));
        }

        protected virtual void PopulateItemFromQueryResult(QueryResult queryResult, PSObject item, int resultGroup)
        {
            //item.Properties.Add(new PSVariableProperty(new PSVariable("ResponseHeaders", String.Join(",",queryResults.ResponseHeaders.AllKeys))));
            item.Properties.Add(new PSVariableProperty(new PSVariable($"[{resultGroup}] TotalRows", queryResult.TotalRows)));
            item.Properties.Add(new PSVariableProperty(new PSVariable($"[{resultGroup}] TotalRowsInclDups", queryResult.TotalRowsIncludingDuplicates)));
            item.Properties.Add(new PSVariableProperty(new PSVariable($"[{resultGroup}] QueryId", queryResult.QueryId)));
            item.Properties.Add(new PSVariableProperty(new PSVariable($"[{resultGroup}] QueryModification", queryResult.QueryModification)));
            item.Properties.Add(new PSVariableProperty(new PSVariable($"[{resultGroup}] QueryRuleId", queryResult.QueryRuleId)));
        }
    }

    public class BasicResultProcessor : BasicAllResultProcessor
    {
        public BasicResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {

        }

        protected override void PopulateItemFromSecondaryResults(SearchQueryResult queryResults, PSObject item)
        {
            // no secondary result information
        }

        protected override void PopulateItemFromQueryResult(QueryResult queryResult, PSObject item, int resultGroup)
        {
            item.Properties.Add(new PSVariableProperty(new PSVariable("TotalRows", queryResult.TotalRows)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("TotalRowsInclDups", queryResult.TotalRowsIncludingDuplicates)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("QueryId", queryResult.QueryId)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("QueryModification", queryResult.QueryModification)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("QueryRuleId", queryResult.QueryRuleId)));
        }

    }

    public class RefinerResultProcessor : BaseQueryResultProcessor
    {
        public RefinerResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {

        }

        protected override void ProcessPrimaryRefinerResults(List<RefinerResult> refinerResults)
        {
            if (refinerResults != null && refinerResults.Count > 0)
            {
                foreach (var refinerResult in refinerResults)
                {

                    foreach (var refinerItem in refinerResult)
                    {
                        var item = new PSObject();

                        item.Properties.Add(
                                    new PSVariableProperty(
                                        new PSVariable("RefinerName", refinerResult.Name)));

                        item.Properties.Add(
                                    new PSVariableProperty(
                                        new PSVariable("RefinerCount", refinerResult.Count)));

                        item.Properties.Add(
                                    new PSVariableProperty(
                                        new PSVariable("Name", refinerItem.Name)));
                        item.Properties.Add(
                                    new PSVariableProperty(
                                        new PSVariable("Count", refinerItem.Count)));
                        // This seems to always be the same as Name?
                        //item.Properties.Add(
                        //            new PSVariableProperty(
                        //                new PSVariable("Value", refinerItem.Value)));

                        Cmdlet.WriteObject(item);
                    }

                }
            }
            else
            {
                ZeroResultsWriteWarning();

            }
        }

        protected virtual void ZeroResultsWriteWarning()
        {
            Cmdlet.WriteWarning("The query returned zero refiner results. Make sure you specify which refiners to retrieve by using -Refiners.");
        }
    }


    public class ManagedPropertiesResultProcessor : RefinerResultProcessor
    {
        private SearchQueryRequest request;

        public ManagedPropertiesResultProcessor(SearchSPIndexCmdlet cmdlet, SearchQueryRequest searchQueryRequest) :
            base(cmdlet)
        {
            this.request = searchQueryRequest;
        }

        public override void Configure()
        {
            base.Configure();

            request.Refiners = "ManagedProperties(filter=600/0/*)";
        }

        protected override void ZeroResultsWriteWarning()
        {
            Cmdlet.WriteWarning("The query returned zero results for the managedproperties(filter=600/0/*) refiner.");
        }

    }

    public class CrawledPropertiesResultProcessor : RefinerResultProcessor
    {
        private SearchQueryRequest request;

        public CrawledPropertiesResultProcessor(SearchSPIndexCmdlet cmdlet, SearchQueryRequest searchQueryRequest) :
            base(cmdlet)
        {
            this.request = searchQueryRequest;
        }

        public override void Configure()
        {
            base.Configure();

            request.Refiners = "CrawledProperties(filter=900/0/*)";
        }

        protected override void ZeroResultsWriteWarning()
        {
            Cmdlet.WriteWarning("The query returned zero results for the crawledproperties(filter=900/0/*) refiner.");
        }

    }

    public class PrimaryResultsResultProcessor : BaseQueryResultProcessor
    {
        public PrimaryResultsResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {
        }


        protected override void ProcessPrimaryRelevantResults(List<ResultItem> relevantResults)
        {
            Cmdlet.WriteDebug(String.Format("Processing {0} relevant results.", relevantResults == null ? 0 : relevantResults.Count));

            if (relevantResults != null)
            {

                foreach (var resultItem in relevantResults)
                {
                    var item = new PSObject();

                    PrimaryResultPrePopulateItem(item);

                    PrimaryResultPopulateItem(resultItem, item);

                    PrimaryResultPostPopulateItem(item);

                    PrimaryResultWriteItem(item);

                }
            }
        }

        protected virtual void PrimaryResultPopulateItem(ResultItem resultItem, PSObject item)
        {
            if (SelectedProperties != null)
            {
                AddSelectedProperties(resultItem, item);
            }
            else
            {
                foreach (var key in resultItem.Keys)
                {
                    item.Properties.Add(
                        new PSVariableProperty(
                            new PSVariable(key, resultItem[key])));
                }
            }
        }

        protected virtual void AddSelectedProperties(ResultItem resultItem, PSObject item)
        {
            // force the order of SelectProperties. If user specifies -Properties "Author,Title", they should appear in that order
            foreach (var selProp in SelectedProperties)
            {
                var key = resultItem.Keys.FirstOrDefault(k => k.Equals(selProp, StringComparison.InvariantCultureIgnoreCase));

                if (!String.IsNullOrWhiteSpace(key))
                {
                    AddItemProperty(item, key, resultItem[key]);
                }
            }
        }

        protected virtual void AddItemProperty(PSObject item, string key, object value)
        {
            item.Properties.Add(
                new PSVariableProperty(
                    new PSVariable(key, value)));
        }

        protected virtual void PrimaryResultWriteItem(PSObject item)
        {
            Cmdlet.WriteObject(item);
        }

        protected virtual void PrimaryResultPrePopulateItem(PSObject item)
        {
            // override in deriving classes if necessary;
        }

        protected virtual void PrimaryResultPostPopulateItem(PSObject item)
        {
            // override in deriving classes if necessary;
        }

        protected override void ProcessSecondaryQueryResults(List<QueryResult> secondaryQueryResults)
        {
            base.ProcessSecondaryQueryResults(secondaryQueryResults);

            if (secondaryQueryResults != null && secondaryQueryResults.Count > 0)
            {
                WarnAboutSecondaryResults();
            }
        }

        protected virtual void WarnAboutSecondaryResults()
        {
            Cmdlet.WriteWarning("There are secondary results. Use -ResultProcessor All to see them.");
        }

    }

    public class FormatResultsResultProcessor : PrimaryResultsResultProcessor
    {
        private Helpers.ConsoleFormatter formatter;

        public FormatResultsResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {
            if (IsEndOfPipeline())
            {
                formatter = new Helpers.ConsoleFormatter(cmdlet);
                formatter.FormatDividerLine();
            }
        }

        protected override void EnsurePropertiesPresent()
        {
            base.EnsurePropertiesPresent();

            Cmdlet.AddSelectProperty("title");
            Cmdlet.AddSelectProperty("HitHighlightedSummary");    // case matters!!!
            Cmdlet.AddSelectProperty("path");
        }

        protected override void PrimaryResultWriteItem(PSObject item)
        {
            if (IsEndOfPipeline())
            {
                formatter.FormatResult(item);    
            }
            else
            {
                base.PrimaryResultWriteItem(item);
            }
        }
    }

    public class AllResultsResultProcessor : PrimaryResultsResultProcessor
    {
        private static readonly string resultGroupPropertyName = "ResultGroup";
        public AllResultsResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {

        }

        protected override void PrimaryResultPrePopulateItem(PSObject item)
        {
            item.Properties.Add(
                new PSVariableProperty(
                    new PSVariable(resultGroupPropertyName, 1)));
        }

        protected override void ProcessSecondaryRelevantResults(int resultGroup, List<ResultItem> relevantResults)
        {
            Cmdlet.WriteDebug(String.Format("Processing {0} relevant results in secondary result group {1}.", relevantResults != null ? relevantResults.Count : 0, resultGroup));

            if (relevantResults != null)
            {

                foreach (var resultItem in relevantResults)
                {
                    var item = new PSObject();

                    SecondaryResultPrePopulateItem(resultGroup, item);

                    AddSelectedProperties(resultItem, item);

                    SecondaryResultPostPopulateItem(resultGroup, item);

                    SecondaryResultWriteItem(resultGroup, item);

                }
            }
        }

        protected virtual void SecondaryResultPrePopulateItem(int resultGroup, PSObject item)
        {
            item.Properties.Add(
                new PSVariableProperty(
                    new PSVariable(resultGroupPropertyName, resultGroup)));
        }

        protected virtual void SecondaryResultPostPopulateItem(int resultGroup, PSObject item)
        {
        }

        protected virtual void SecondaryResultWriteItem(int resultGroup, PSObject item)
        {
            Cmdlet.WriteObject(item);
        }

        protected override void WarnAboutSecondaryResults()
        {
            // do nothing
        }

    }

    public class RawResultProcessor : AbstractQueryResultProcessor
    {
        public RawResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {

        }

        public override void Process(SearchQueryResult queryResults)
        {
            var item = new PSObject(queryResults);

            Cmdlet.WriteObject(item);
        }

    }

    public class BaseRankDetailProcessor : PrimaryResultsResultProcessor
    {
        protected BaseRankDetailProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {
        }

        protected override void UserNotification()
        {
            base.UserNotification();

            Cmdlet.WriteWarning("Please note that RankDetail is experimental and only an approximation.");
        }

        protected override void EnsurePropertiesPresent()
        {
            base.EnsurePropertiesPresent();
  
            Cmdlet.AddSelectProperty("WorkId");
            Cmdlet.AddSelectProperty("Rank");
            Cmdlet.AddSelectProperty("RankDetail");
        }

        public override bool HandleException(Exception ex, SearchQueryRequest searchQueryRequest)
        {
            //Cmdlet.WriteWarning(">>>" + ex.GetType().ToString());

            if (ex.Message.Contains("HTTP 500"))
            {
                Cmdlet.WriteWarning("Please note that you need Search Service Application administrative rights to use rank result processors.");
            }

            return base.HandleException(ex, searchQueryRequest);
        }

    }

    public class RankXMLResultProcessor : BaseRankDetailProcessor
    {
        public static readonly int maxResults = 100;

        public RankXMLResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {

        }

        protected override void ProcessPrimaryQueryResults(QueryResult primaryQueryResult)
        {
                EnsureMaxResults(primaryQueryResult);

                base.ProcessPrimaryQueryResults(primaryQueryResult);
        }

        protected void EnsureMaxResults(QueryResult primaryQueryResult)
        {
            if (primaryQueryResult.TotalRows > maxResults)
            {
                // plan is to catch and resubmit with filter limiting result set to RowLimit or 100, whichever is smaller
                var ex = new RankDetailTooManyResults(String.Format("Too many results for rank detail: {0}. Max is {1}.", primaryQueryResult.TotalRows, maxResults));

                ex.QueryFilter = GetQueryFilter(primaryQueryResult.RelevantResults);

                throw ex;
            }
        }

        private string GetQueryFilter(List<ResultItem> relevantResults)
        {
            List<string> workids = new List<string>();
            int n = 1;

            foreach (var resultItem in relevantResults)
            {
                var key = resultItem.Keys.FirstOrDefault(k => k.Equals("workid", StringComparison.InvariantCultureIgnoreCase));

                if (!String.IsNullOrWhiteSpace(key))
                {
                    workids.Add(String.Format("WorkId={0}", resultItem[key]));
                }

                if (++n > maxResults) break;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(" (");
            sb.Append(String.Join(" OR ", workids));
            sb.Append(")");

            var filter = sb.ToString();

            Cmdlet.WriteDebug(String.Format("IncludeRankFilter: {0}", filter));

            return filter;
        }

    }


    public class RankDetailResultProcessor : RankXMLResultProcessor
    {
        public RankDetailResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {
        }

        protected override void AddItemProperty(PSObject item, string key, object value)
        {
            if (key.Equals("RankDetail", StringComparison.InvariantCultureIgnoreCase))
            {
                var rlogparser = new SearchQueryTool.Helpers.RankLogParser((string) value);

                List<string> rankFeatures = new List<string>();

                int n = rlogparser.ScoreDetails.Length;

                // only look at last stage

                if (n > 0)
                {
                    var scoreDetail = rlogparser.ScoreDetails[n - 1];

                    foreach (var rankFeature in scoreDetail.RankingFeatures)
                    {
                        IRankingFeatureFormatter formatter = RankingFeatureFormatterFactory.SelectFormatter(rankFeature);

                        rankFeatures.Add(formatter.Format());
                    }
                }

                base.AddItemProperty(item, key, String.Join(",", rankFeatures));
            }
            else
            {
                base.AddItemProperty(item, key, value);
            }
        }
    }

    public class RankContributionresultProcessor : RankXMLResultProcessor
    {
        public RankContributionresultProcessor(SearchSPIndexCmdlet cmdlet) : 
            base(cmdlet)
        {
        }

        protected override void AddItemProperty(PSObject item, string key, object value)
        {
            if (key.Equals("RankDetail", StringComparison.InvariantCultureIgnoreCase))
            {
                var rlog = RankLog.CreateRankLogFromXml((string) value);

                RankLogStage stage = rlog.FinalRankStage;

                foreach (var feature in stage.Features)
                {
                    item.Properties.Add(
                        new PSVariableProperty(
                            new PSVariable(feature.Name, feature.EstimatedRankContribution())));
                }
            }
            else
            {
                base.AddItemProperty(item, key, value);
            }

        }
    }


    //
    // SearchSuggestions
    //

    public interface ISuggestionsResultProcessor
    {
        void Configure();

        void Process(SearchSuggestionsResult searchSuggestionsResult);

        bool HandleException(Exception ex, SearchSuggestionsRequest searchSuggestionsRequest);
    }

    public class SuggestionsResultProcessor : ISuggestionsResultProcessor
    {
        public SuggestionsResultProcessor(Cmdlet cmdlet)
        {
            this.Cmdlet = cmdlet;
        }

        public Cmdlet Cmdlet { get; }

        public void Configure()
        {
            
        }

        public bool HandleException(Exception ex, SearchSuggestionsRequest searchSuggestionsRequest)
        {
            throw new NotImplementedException();
        }

        public virtual void Process(SearchSuggestionsResult searchSuggestionsResult)
        {
            foreach (var resultItem in searchSuggestionsResult.SuggestionResults)
            {
                var item = new PSObject();

                item.Properties.Add(new PSVariableProperty(new PSVariable("Query", resultItem.Query)));
                item.Properties.Add(new PSVariableProperty(new PSVariable("IsPersonal", resultItem.IsPersonal)));

                Cmdlet.WriteObject(item);
            }
        }
    }

    public class RawSuggestionsResultProcessor : SuggestionsResultProcessor
    {
        public RawSuggestionsResultProcessor(Cmdlet cmdlet) : base(cmdlet)
        {
        }


        public override void Process(SearchSuggestionsResult searchSuggestionsResult)
        {
            var item = new PSObject(searchSuggestionsResult);

            Cmdlet.WriteObject(item);
        }
    }
}
