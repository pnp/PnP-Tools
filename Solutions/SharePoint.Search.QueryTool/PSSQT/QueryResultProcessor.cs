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
        Primary,
        All,
        Refiners,
        Raw,
        RankDetail,
        RankXML,
        ExplainRank,
        RankContribution,
        AllProperties,
        AllPropertiesInline
    }

    public interface IQueryResultProcessor
    {
        void Configure();

        void Process(SearchQueryResult searchQueryResult);

               
        bool HandleException(Exception ex);
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

                default:
                    throw new NotImplementedException("No result processor match " + type);
            }

            return qrp;
        }

    }

    public abstract class AbstractQueryResultProcessor : IQueryResultProcessor
    {
        public AbstractQueryResultProcessor(SearchSPIndexCmdlet cmdlet)
        {
            this.Cmdlet = cmdlet;
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

        public virtual bool HandleException(Exception ex)
        {
            return false;   // just rethrow
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
            WriteInfo(String.Format("Number of triggered rules: {0}", triggeredRules != null ? triggeredRules.Count : 0));
        }



        protected virtual void ProcessSearchQueryResultInfo(SearchQueryResult queryResults)
        {
            WriteInfo(String.Format("HTTP status code: {0}", queryResults.StatusCode));
            WriteInfo(String.Format("Status description: {0}", queryResults.StatusDescription));
            WriteInfo(String.Format("HTTP protocol version: {0}", queryResults.HttpProtocolVersion));
            WriteInfo(String.Format("Response content type: {0}", queryResults.ContentType));

            Cmdlet.WriteDebug(String.Format("Response headers: {0}", String.Join(",", queryResults.ResponseHeaders.AllKeys)));
        }


        protected virtual void ProcessPrimaryQueryResultsInfo(QueryResult primaryQueryResult)
        {
            WriteInfo(String.Format("Total rows: {0}", primaryQueryResult.TotalRows));
            WriteInfo(String.Format("Total rows incl. duplicates: {0}", primaryQueryResult.TotalRowsIncludingDuplicates));
            WriteInfo(String.Format("Query id: {0}", primaryQueryResult.QueryId));
            WriteInfo(String.Format("Query modification: {0}", primaryQueryResult.QueryModification));
            WriteInfo(String.Format("Query rule id: {0}", primaryQueryResult.QueryRuleId));
        }

        protected virtual void ProcessPrimaryRefinerResults(List<RefinerResult> refinerResults)
        {
            //WriteInfo(String.Format("Number of refiners: {0}", refinerResults != null ? refinerResults.Count : 0));
        }

        protected virtual void ProcessPrimaryRelevantResults(List<ResultItem> relevantResults)
        {
            WriteInfo(String.Format("Number of relevant primary results: {0}", relevantResults != null ? relevantResults.Count : 0));
        }

        protected virtual void ProcessSecondaryQueryResultsListInfo(List<QueryResult> secondaryQueryResults)
        {
            WriteInfo(String.Format("Number of secondary result groups: {0}", secondaryQueryResults != null ? secondaryQueryResults.Count : 0));
        }

        protected virtual void ProcessSecondaryQueryResultsInfo(int resultGroup, QueryResult queryResult)
        {
            WriteInfo(String.Format("Total rows in group {1}: {0}", queryResult.TotalRows, resultGroup));
            WriteInfo(String.Format("Total rows incl. duplicates in group {1}: {0}", queryResult.TotalRowsIncludingDuplicates, resultGroup));
            WriteInfo(String.Format("Query id group {1}: {0}", queryResult.QueryId, resultGroup));
            WriteInfo(String.Format("Query modification in group {1}: {0}", queryResult.QueryModification, resultGroup));
            WriteInfo(String.Format("Query rule id in group {1}: {0}", queryResult.QueryRuleId, resultGroup));
        }

        protected virtual void ProcessSecondaryRelevantResults(int resultGroup, List<ResultItem> relevantResults)
        {
            WriteInfo(String.Format("Number of relevant secondary results in group {1}: {0}", relevantResults != null ? relevantResults.Count : 0, resultGroup));
        }

        protected virtual void ProcessSecondryRefinerResults(int resultGroup, List<RefinerResult> refinerResults)
        {
            //throw new NotImplementedException();
        }

        protected virtual void WriteInfo(string text)
        {
            Cmdlet.WriteVerbose(text);
        }
    }

    public class BasicResultProcessor : BaseQueryResultProcessor
    {
        public BasicResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {

        }

        public override void Process(SearchQueryResult queryResults)
        {
            var item = new PSObject();

            item.Properties.Add(new PSVariableProperty(new PSVariable("StatusCode", queryResults.StatusCode)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("StatusDescription", queryResults.StatusDescription)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("HTTPProtocalVersion", queryResults.HttpProtocolVersion)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("ContentType", queryResults.ContentType)));
            //item.Properties.Add(new PSVariableProperty(new PSVariable("ResponseHeaders", String.Join(",",queryResults.ResponseHeaders.AllKeys))));
            item.Properties.Add(new PSVariableProperty(new PSVariable("TotalRows", queryResults.PrimaryQueryResult.TotalRows)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("TotalRowsInclDups", queryResults.PrimaryQueryResult.TotalRowsIncludingDuplicates)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("QueryId", queryResults.PrimaryQueryResult.QueryId)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("QueryModification", queryResults.PrimaryQueryResult.QueryModification)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("QueryRuleId", queryResults.PrimaryQueryResult.QueryRuleId)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("NumSecondaryResults", queryResults.SecondaryQueryResults != null ? queryResults.SecondaryQueryResults.Count : 0)));
            //item.Properties.Add(new PSVariableProperty(new PSVariable("NumRefiners", queryResults.PrimaryQueryResult.RefinerResults != null ? queryResults.PrimaryQueryResult.RefinerResults.Count : 0)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("NumTriggeredRules", queryResults.TriggeredRules != null ? queryResults.TriggeredRules.Count : 0)));

            Cmdlet.WriteObject(item);
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
                Cmdlet.WriteWarning("The query returned zero refiner results.");
            }
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

        public override bool HandleException(Exception ex)
        {
            //Cmdlet.WriteWarning(">>>" + ex.GetType().ToString());

            if (ex.Message.Contains("HTTP 500"))
            {
                Cmdlet.WriteWarning("Please note that you need Search Service Application administrative rights to use rank result processors.");
            }

            return base.HandleException(ex);
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


}
