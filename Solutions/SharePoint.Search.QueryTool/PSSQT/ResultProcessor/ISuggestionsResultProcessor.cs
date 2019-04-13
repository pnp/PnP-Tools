using SearchQueryTool.Model;
using System;

namespace PSSQT.ResultProcessor
{
    //
    // SearchSuggestions
    //

    public interface ISuggestionsResultProcessor
    {
        void Configure();

        void Process(SearchSuggestionsResult searchSuggestionsResult);

        bool HandleException(Exception ex, SearchSuggestionsRequest searchSuggestionsRequest);
    }
}
