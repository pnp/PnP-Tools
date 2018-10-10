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
    [Cmdlet(VerbsCommon.Search, "SPSuggestions", DefaultParameterSetName = "P1")]
    public class SearchSPSuggestionsCmdlet
        : AbstractSearchSPCmdlet
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
        public SwitchParameter EnableHitHighlighting { get; set; }

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
        public SwitchParameter EnableCapitalizeFirstLetters { get; set; }

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
        public SwitchParameter EnableShowPeopleNameSuggestions { get; set; }

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
        public SwitchParameter EnablePreQuerySuggestions { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Disable Pre Query Suggestions."
        )]
        public SwitchParameter DisablePreQuerySuggestions { get; set; }
        #endregion

        #region Methods


        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                SearchConnection searchConnection = new SearchConnection();
                SearchSuggestionsRequest searchSuggestionsRequest = new SearchSuggestionsRequest();

                // Load Preset
                if (ParameterSetName == "P2")
                {
                    SearchPreset preset = LoadPresetFromFile();
                    searchConnection = preset.Connection;
                }

                // Set Script Parameters from Command Line

                SetRequestParameters(searchConnection, searchSuggestionsRequest);

                // Save Site/Preset

                if (!(String.IsNullOrWhiteSpace(SaveSite) /*&& String.IsNullOrWhiteSpace(SavePreset)*/))
                {
                    throw new NotImplementedException("TODO: implement save site for suggestions");
                    //if (!String.IsNullOrWhiteSpace(SaveSite))
                    //{
                    //    SaveSiteToFile(searchConnection);
                    //}

                    //if (!String.IsNullOrWhiteSpace(SavePreset))
                    //{
                    //    SavePresetToFile(searchConnection, searchSuggestionsRequest);
                    //}
                }
                else // Perform the Search
                {
                    EnsureValidQuery(searchSuggestionsRequest);

                    GetResults(searchSuggestionsRequest);

                }

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex,
                           "SearchSPSuggestionsError",
                           ErrorCategory.NotSpecified,
                           null)
                          );

                WriteDebug(ex.StackTrace);
            }
        }


        private void EnsureValidQuery(SearchSuggestionsRequest searchSuggestionsRequest)
        {
            if (String.IsNullOrWhiteSpace(searchSuggestionsRequest.QueryText))
            {
                throw new Exception("Query text cannot be null.");
            }
        }





        private void SetRequestParameters(SearchConnection searchConnection, SearchSuggestionsRequest searchSuggestionsRequest)
        {
            base.SetRequestParameters(searchConnection, searchSuggestionsRequest);

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



        #endregion
    }
}
