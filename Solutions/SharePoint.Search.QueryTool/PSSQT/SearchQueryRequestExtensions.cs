using System;
using System.Text;
using SearchQueryTool.Model;

namespace PSSQT
{
    public static class SearchQueryRequestExtension
    {
        public static string PrintDebug(this SearchQueryRequest searchQueryRequest)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(String.Format("\n*** Search Query Request ***\nQuery: {0}\n", searchQueryRequest.QueryText));
            sb.Append(String.Format("EnableStemming: {0}\n", searchQueryRequest.EnableStemming));
            sb.Append(String.Format("EnablePhonetic: {0}\n", searchQueryRequest.EnablePhonetic));
            sb.Append(String.Format("EnableNicknames: {0}\n", searchQueryRequest.EnableNicknames));
            sb.Append(String.Format("TrimDuplicates: {0}\n", searchQueryRequest.TrimDuplicates));
            sb.Append(String.Format("EnableFql: {0}\n", searchQueryRequest.EnableFql));
            sb.Append(String.Format("EnableQueryRules: {0}\n", searchQueryRequest.EnableQueryRules));
            sb.Append(String.Format("ProcessBestBets: {0}\n", searchQueryRequest.ProcessBestBets));
            sb.Append(String.Format("ByPassResultTypes: {0}\n", searchQueryRequest.ByPassResultTypes));
            sb.Append(String.Format("ProcessPersonalFavorites: {0}\n", searchQueryRequest.ProcessPersonalFavorites));
            //sb.Append(String.Format("GenerateBlockRankLog: {0}\n", searchQueryRequest.GenerateBlockRankLog));
            sb.Append(String.Format("IncludeRankDetail: {0}\n", searchQueryRequest.IncludeRankDetail));
            sb.Append(String.Format("StartRow: {0}\n", searchQueryRequest.StartRow));
            sb.Append(String.Format("RowLimit: {0}\n", searchQueryRequest.RowLimit));
            sb.Append(String.Format("RowsPerPage: {0}\n", searchQueryRequest.RowsPerPage));
            sb.Append(String.Format("SelectProperties: {0}\n", searchQueryRequest.SelectProperties));
            //sb.Append(String.Format("GraphQuery: {0}\n", searchQueryRequest.GraphQuery));
            //sb.Append(String.Format("GraphRankingModel: {0}\n", searchQueryRequest.GraphRankingModel));
            sb.Append(String.Format("Refiners: {0}\n", searchQueryRequest.Refiners));
            //sb.Append(String.Format("RefinementFilters: {0}\n", searchQueryRequest.RefinementFilters));
            sb.Append(String.Format("HitHighlightedProperties: {0}\n", searchQueryRequest.HitHighlightedProperties));
            sb.Append(String.Format("RankingModelId: {0}\n", searchQueryRequest.RankingModelId));
            sb.Append(String.Format("SortList: {0}\n", searchQueryRequest.SortList));
            //sb.Append(String.Format("Culture: {0}\n", searchQueryRequest.Culture));
            sb.Append(String.Format("SourceId: {0}\n", searchQueryRequest.SourceId));
            sb.Append(String.Format("HiddenConstraints: {0}\n", searchQueryRequest.HiddenConstraints));
            //sb.Append(String.Format("ResultsUrl: {0}\n", searchQueryRequest.ResultsUrl));
            //sb.Append(String.Format("QueryTag: {0}\n", searchQueryRequest.QueryTag));
            //sb.Append(String.Format("CollapseSpecification: {0}\n", searchQueryRequest.CollapseSpecification));
            sb.Append(String.Format("QueryTemplate: {0}\n", searchQueryRequest.QueryTemplate));
            //sb.Append(String.Format("TrimDuplicatesIncludeId: {0}\n", searchQueryRequest.TrimDuplicatesIncludeId));
            sb.Append(String.Format("ClientType: {0}\n", searchQueryRequest.ClientType));
            //sb.Append(String.Format("PersonalizationData: {0}\n", searchQueryRequest.PersonalizationData));
            sb.Append(String.Format("Accept Type: {0}\n", searchQueryRequest.AcceptType));
            sb.Append(String.Format("Http Method Type: {0}\n", searchQueryRequest.HttpMethodType));
            sb.Append(String.Format("HTTP URI: {0}\n", searchQueryRequest.GenerateUri()));

            return sb.ToString();
        }
        public static string PrintVerbose(this SearchQueryRequest searchQueryRequest)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(String.Format("Query Text: {0}\n", searchQueryRequest.QueryText));
            sb.Append(String.Format("HTTP URI: {0}\n", searchQueryRequest.GenerateUri()));

            return sb.ToString();
        }

        public static string GenerateUri(this SearchQueryRequest searchQueryRequest)
        {
            if (searchQueryRequest.HttpMethodType == HttpMethodType.Post)
            {
                var sb = new StringBuilder();

                sb.Append(searchQueryRequest.GenerateHttpPostUri().ToString());
                sb.Append("\n\nPAYLOAD\n");
                sb.Append(searchQueryRequest.GenerateHttpPostBodyPayload());

                return sb.ToString();
            }

            return searchQueryRequest.GenerateHttpGetUri().ToString();
        }

    }
}
