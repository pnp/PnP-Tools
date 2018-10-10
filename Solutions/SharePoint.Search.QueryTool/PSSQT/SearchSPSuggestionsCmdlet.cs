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
            HelpMessage = "Number of results."
        )]
        public int? RowLimit { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Save parameters as a preset. Load using LoadPreset"
        )]
        public string SavePreset { get; set; }


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

                if (!(String.IsNullOrWhiteSpace(SaveSite) && String.IsNullOrWhiteSpace(SavePreset)))
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

  
 
            var requestResponsePair = task.Result;
            var request = requestResponsePair.Item1;

            using (var response = requestResponsePair.Item2)
            {
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

                    var searchResults = new SearchSuggestionsResult
                    {
                        RequestUri = request.RequestUri,
                        RequestMethod = request.Method,
                        ContentType = response.ContentType,

                        ResponseContent = content,
                        RequestHeaders = requestHeaders,
                        ResponseHeaders = responseHeaders,
                        StatusCode = response.StatusCode,
                        StatusDescription = response.StatusDescription,
                        HttpProtocolVersion = response.ProtocolVersion.ToString()
                    };
                    searchResults.Process();

                    SetSuggestionsResultItems(searchResults);
                }
            }
 
        }

        private void foo()
        {
            int totalRows = 0;
            bool keepTrying = true;


            IQueryResultProcessor queryResultProcessor = QueryResultProcessorFactory.SelectQueryResultProcessor(ResultProcessor.Value, this, searchQueryRequest);

            queryResultProcessor.Configure();    // May add required properties to retrieve, modify the searchQueryRequest (e.g. rankdetail etc.)

            while (keepTrying)
            {
                keepTrying = false;

                try
                {
                    var requestResponsePair = HttpRequestRunner.RunWebRequest(searchSuggestionsRequest);

                    var queryResults = requestResponsePair.GetResultItem();

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

        }

        private int GetResults(SearchSuggestionsRequest searchSuggestionsRequest)
        {
 
            return 0;
        }



        #endregion
    }
}
