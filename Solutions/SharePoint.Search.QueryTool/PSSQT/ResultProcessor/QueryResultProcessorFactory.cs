using SearchQueryTool.Model;
using System;

namespace PSSQT.ResultProcessor
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
                    qrp = new RankContributionResultProcessor(cmdlet);
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
}
