using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchQueryTool.Model
{
    public class SearchSuggestionsRequest: SearchRequest
    {
        public bool? PreQuerySuggestions { get; set; }
        public bool? ShowPeopleNameSuggestions { get; set; }
        public int? NumberOfQuerySuggestions { get; set; }
        public int? NumberOfResultSuggestions { get; set; }
        public bool? HitHighlighting { get; set; }
        public bool? CapitalizeFirstLetters { get; set; }
        public int? Culture { get; set; }

        public override string GenerateHttpGetUri()
        {
            string restUri = this.SharePointSiteUrl;

            StringBuilder uriBuilder = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(restUri))
            {
                uriBuilder.Append(restUri);

                if (!restUri.EndsWith("/"))
                    uriBuilder.Append("/");
            }

            uriBuilder.AppendFormat("_api/search/suggest?querytext='{0}'", this.QueryText);

            if (this.PreQuerySuggestions == true)
                uriBuilder.Append("&fprequerysuggestions=true");
            else if (this.PreQuerySuggestions == false)
                uriBuilder.Append("&fprequerysuggestions=false");

            if (this.ShowPeopleNameSuggestions == true)
                uriBuilder.Append("&showpeoplenamesuggestions=true");
            else if (this.ShowPeopleNameSuggestions == false)
                uriBuilder.Append("&showpeoplenamesuggestions=false");

            if (this.HitHighlighting == true)
                uriBuilder.Append("&fhithighlighting=true");
            else if (this.HitHighlighting == false)
                uriBuilder.Append("&fhithighlighting=false");

            if (this.CapitalizeFirstLetters == true)
                uriBuilder.Append("&fcapitalizefirstletters=true");
            else if (this.CapitalizeFirstLetters == false)
                uriBuilder.Append("&fcapitalizefirstletters=false");

            if (this.NumberOfQuerySuggestions.HasValue)
                uriBuilder.AppendFormat("&inumberofquerysuggestions={0}", this.NumberOfQuerySuggestions.Value);

            if (this.NumberOfResultSuggestions.HasValue)
                uriBuilder.AppendFormat("&inumberofresultsuggestions={0}", this.NumberOfResultSuggestions.Value);

            if (this.Culture.HasValue)
                uriBuilder.AppendFormat("&culture={0}", this.Culture.Value);

            return uriBuilder.ToString();
        }

        public override string GenerateHttpPostUri()
        {
            throw new NotImplementedException();
        }

        public override string GenerateHttpPostBodyPayload()
        {
            throw new NotImplementedException();
        }
    }
}
