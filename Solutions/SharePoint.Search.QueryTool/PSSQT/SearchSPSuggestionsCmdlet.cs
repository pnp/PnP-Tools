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
using PSSQT.ResultProcessor;

/**
 * <ParameterSetName	P1	P2
 * Site                 X   X
 * Query                X   X 
 * LoadPreset	        	X
 **/

namespace PSSQT
{
    [Cmdlet(VerbsCommon.Search, "SPSuggestions", DefaultParameterSetName = "P1")]
    public class SearchSPSuggestionsCmdlet
        : AbstractSearchSPCmdlet<SearchSuggestionsRequest>
    {
        #region PrivateMembers

        #endregion

        #region ScriptParameters

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Number of query suggestions."
        )]
        public int? NumberOfQuerySuggestions { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Number of result suggestions."
        )]

        public int? NumberOfResultSuggestions { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Enable hit highlithing of search term in query."
        )]
        public SwitchParameter EnableHitHighlighting { get; set; } = true;

        [Parameter(
             Mandatory = false,
             ValueFromPipelineByPropertyName = false,
             ValueFromPipeline = false,
             HelpMessage = "Disable hit highlithing of search term in query."
        )]
        public SwitchParameter DisableHitHighlighting { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Enable Capitalize First Letters."
        )]
        public SwitchParameter EnableCapitalizeFirstLetters { get; set; } = true;

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Disable Capitalize First Letters."
        )]
        public SwitchParameter DisableCapitalizeFirstLetters { get; set; }

        [Parameter(
             Mandatory = false,
             ValueFromPipelineByPropertyName = false,
             ValueFromPipeline = false,
             HelpMessage = "Enable Show People Name Suggestions."
         )]
        public SwitchParameter EnableShowPeopleNameSuggestions { get; set; } = true;

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Disable Show People Name Suggestions."
        )]
        public SwitchParameter DisableShowPeopleNameSuggestions { get; set; }

        [Parameter(
              Mandatory = false,
              ValueFromPipelineByPropertyName = false,
              ValueFromPipeline = false,
              HelpMessage = "Enable Pre Query Suggestions."
          )]
        public SwitchParameter EnablePreQuerySuggestions { get; set; } = true;

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Disable Pre Query Suggestions."
        )]
        public SwitchParameter DisablePreQuerySuggestions { get; set; }
        #endregion

        #region Methods


        protected override void SetRequestParameters(SearchSuggestionsRequest searchSuggestionsRequest)
        {
            base.SetRequestParameters(searchSuggestionsRequest);

            // TODO: do searchSuggestions specifics

            if (NumberOfQuerySuggestions.HasValue)
            {
                searchSuggestionsRequest.NumberOfQuerySuggestions = NumberOfQuerySuggestions.Value;
            }

            if (NumberOfResultSuggestions.HasValue)
            {
                searchSuggestionsRequest.NumberOfResultSuggestions = NumberOfResultSuggestions;
            }

            searchSuggestionsRequest.PreQuerySuggestions = GetThreeWaySwitchValue(EnablePreQuerySuggestions, DisablePreQuerySuggestions);
            searchSuggestionsRequest.HitHighlighting = GetThreeWaySwitchValue(EnableHitHighlighting, DisableHitHighlighting);
            searchSuggestionsRequest.CapitalizeFirstLetters = GetThreeWaySwitchValue(EnableCapitalizeFirstLetters, DisableCapitalizeFirstLetters);
            searchSuggestionsRequest.ShowPeopleNameSuggestions = GetThreeWaySwitchValue(EnableShowPeopleNameSuggestions, DisableShowPeopleNameSuggestions);
        }

        private void GetResults(SearchSuggestionsRequest searchSuggestionsRequest)
        {
            ISuggestionsResultProcessor suggestionResultProcessor = new SuggestionsResultProcessor(this);

            suggestionResultProcessor.Configure();

            WriteVerbose($"Request: {searchSuggestionsRequest.ToString()}");

            var requestResponsePair = HttpRequestRunner.RunWebRequest(searchSuggestionsRequest);

            var suggestionResults = requestResponsePair.GetResultItem<SearchSuggestionsResult>();


            suggestionResultProcessor.Process(suggestionResults);
        }

        protected override void SaveRequestPreset(SearchConnection searchConnection, SearchSuggestionsRequest searchRequest)
        {
            throw new NotImplementedException();   // never called since IsSavePreset is always false
        }

        protected override bool IsSavePreset()
        {
            return false;    // You must use Search-SPIndex to save a preset
        }

        protected override void PresetLoaded(ref SearchSuggestionsRequest searchRequest, SearchPreset preset)
        {
            searchRequest.CopyFrom(preset.Request);
        }


                

        protected override void ExecuteRequest(SearchSuggestionsRequest searchRequest)
        {
            GetResults(searchRequest);
        }

        #endregion
    }
}
