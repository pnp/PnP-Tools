using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SearchQueryTool.Model
{
    /// <summary>
    /// Represents search suggestions results.
    /// </summary>
    public class SearchSuggestionsResult : SearchResult
    {
        public SearchSuggestionsResult()
            :base()
        {
        }

        public List<SuggestionResulItem> SuggestionResults { get; private set; }

        /// <summary>
        /// Fireoff processing of the search result content.
        /// </summary>
        public override void Process()
        {
            if (this.ContentType.StartsWith("application/json"))
            {
                ProcessJson();
            }
            else if (this.ContentType.StartsWith("application/xml"))
            {
                ProcessXml();
            }
        }

        private void ProcessXml()
        {
            XNamespace d = "http://schemas.microsoft.com/ado/2007/08/dataservices";

            var root = XElement.Parse(this.ResponseContent);

            if (root != null && root.HasElements)
            {
                XElement queriesElm = root.Element(d + "Queries");
                if (queriesElm != null && queriesElm.HasElements)
                {
                    List<SuggestionResulItem> resultItems = new List<SuggestionResulItem>();

                        foreach (var item in queriesElm.Elements(d + "element"))
                        {
                            if (item != null && item.Element(d + "Query") != null)
                            {
                                var query = item.Element(d + "Query").Value;
                                var isPersonal = (bool)item.Element(d + "IsPersonal");

                                resultItems.Add(new SuggestionResulItem { Query = query, IsPersonal = isPersonal });
                            }
                        }

                        this.SuggestionResults = resultItems;
                }
            }
        }

        private void ProcessJson()
        {
            XmlReader reader =
                       JsonReaderWriterFactory
                               .CreateJsonReader(Encoding.UTF8.GetBytes(this.ResponseContent), new XmlDictionaryReaderQuotas());

            XElement root = XElement.Load(reader);
            XElement suggestElm = root.XPathSelectElement("//suggest");

            if (suggestElm != null)
            {
                XElement queriesElm = suggestElm.Element("Queries");
                if (queriesElm != null)
                {
                    if (queriesElm.Element("results") != null)
                    {
                        List<SuggestionResulItem> resultItems = new List<SuggestionResulItem>();

                        foreach (var item in queriesElm.Element("results").Elements("item"))
                        {
                            if (item != null && item.Element("Query") != null)
                            {
                                var query = item.Element("Query").Value;
                                var isPersonal = (bool)item.Element("IsPersonal");

                                resultItems.Add(new SuggestionResulItem { Query = query, IsPersonal = isPersonal });
                            }
                        }

                        this.SuggestionResults = resultItems;
                    
                    }
                }
            }
        }
    }

    public class SuggestionResulItem
    {
        public string Query { get; set; }
        public bool IsPersonal { get; set; }
    }        
}
