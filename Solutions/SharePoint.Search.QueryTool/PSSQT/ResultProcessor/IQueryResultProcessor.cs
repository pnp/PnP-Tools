using SearchQueryTool.Model;
using System;

namespace PSSQT.ResultProcessor
{
    public interface IQueryResultProcessor
    {
        void Configure();

        void Process(SearchQueryResult searchQueryResult);

        bool HandleException(Exception ex, SearchQueryRequest searchQueryRequest);

    }
}
