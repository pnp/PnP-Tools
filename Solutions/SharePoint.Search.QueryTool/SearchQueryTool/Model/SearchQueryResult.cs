using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SearchQueryTool.Model
{
    public class ResultItem : SortedDictionary<string, string>
    {

        /// <summary>
        /// Return the title of the result.
        /// </summary>
        public string Title
        {
            get
            {
                KeyValuePair<string, string> title = this.FirstOrDefault(x => x.Key.Equals("Title", StringComparison.InvariantCultureIgnoreCase));
                return title.Value;
            }
        }

        /// <summary>
        /// Returns the path of the result.
        /// </summary>
        public string Path
        {
            get
            {
                return this.FirstOrDefault(x => x.Key.Equals("Path", StringComparison.InvariantCultureIgnoreCase)).Value;
            }
        }

    }

    public class RefinerResult : List<RefinementItem>
    {
        public string Name { get; set; }
    }

    public class RefinementItem
    {
        public long Count { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public string Value { get; set; }
    }

    public class QueryResult
    {
        public string QueryId { get; set; }
        public string QueryRuleId { get; set; }
        public string QueryModification { get; set; }
        public int TotalRows { get; set; }
        public int TotalRowsIncludingDuplicates { get; set; }
        public List<ResultItem> RelevantResults { get; set; }
        public List<RefinerResult> RefinerResults { get; set; }
        public string ResultTitle { get; internal set; }
        public string ResultTitleUrl { get; internal set; }
    }

    /// <summary>
    /// Represents the search query results.
    /// </summary>
    public class SearchQueryResult : SearchResult
    {
        public SearchQueryResult()
            : base()
        {
        }

        public string SerializedQuery { get; private set; }
        public string QueryElapsedTime { get; private set; }
        public List<string> TriggeredRules { get; private set; }
        public QueryResult PrimaryQueryResult { get; private set; }
        public List<QueryResult> SecondaryQueryResults { get; private set; }

        public bool IsPartial { get; internal set; }
        public string MultiGeoSearchStatus { get; internal set; }

        /// <summary>
        /// Fireoff processing of the search result content.
        /// </summary>
        public override void Process()
        {
            if (this.ContentType.StartsWith("application/json"))
            {
                if (this.RequestMethod == "GET")
                {
                    ProcessJsonFromGetRequest();
                }
                else
                {
                    ProcessJsonFromPostRequest();
                }
            }
            else if (this.ContentType.StartsWith("application/xml"))
            {
                ProcessXml();
            }
        }

        private static XElement GetResultXML(string itsastring)
        {
            byte[] bytes = new UTF8Encoding().GetBytes(itsastring);
            var reader = new StreamReader(new MemoryStream(bytes), true);
            var xr = XmlReader.Create(reader, new XmlReaderSettings { CheckCharacters = false });
            return XElement.Load(xr);
        }
        private void ProcessXml()
        {
            XNamespace d = "http://schemas.microsoft.com/ado/2007/08/dataservices";

            var root = GetResultXML(this.ResponseContent);

            if (root != null && root.HasElements)
            {
                this.QueryElapsedTime = (string)root.Element(d + "ElapsedTime");

                XElement propertiesElm = root.Element(d + "Properties");
                if (propertiesElm != null && propertiesElm.HasElements)
                {
                    foreach (var elm in propertiesElm.Elements(d + "element"))
                    {
                        if (elm != null && elm.Element(d + "Key") != null)
                        {
                            if (elm.Element(d + "Key").Value == "SerializedQuery")
                            {
                                this.SerializedQuery = elm.Element(d + "Value").Value;
                                // break; // need to parse through all elements
                            }
                            else if (elm.Element(d + "Key").Value == "IsPartial")
                            {
                                if (bool.TryParse(elm.Element(d + "Value").Value, out bool isPartial))
                                {
                                    IsPartial = isPartial;
                                }
                            }
                            else if (elm.Element(d + "Key").Value == "MultiGeoSearchStatus")
                            {
                                this.MultiGeoSearchStatus = elm.Element(d + "Value").Value;
                            }
                        }
                    }
                }

                #region Primary Query Result

                var primaryQueryResult = root.Element(d + "PrimaryQueryResult");
                if (primaryQueryResult != null)
                {
                    this.PrimaryQueryResult = new QueryResult();

                    if (primaryQueryResult.Element(d + "QueryId") != null)
                    {
                        this.PrimaryQueryResult.QueryId = (string)primaryQueryResult.Element(d + "QueryId");
                    }

                    if (primaryQueryResult.Element(d + "QueryRuleId") != null)
                    {
                        this.PrimaryQueryResult.QueryRuleId = (string)primaryQueryResult.Element(d + "QueryRuleId");
                    }

                    #region Relevant Results

                    var relevantResults = primaryQueryResult.Element(d + "RelevantResults");
                    if (relevantResults != null)
                    {
                        if (relevantResults.Element(d + "TotalRows") != null)
                        {
                            this.PrimaryQueryResult.TotalRows = (int)relevantResults.Element(d + "TotalRows");
                        }

                        if (relevantResults.Element(d + "TotalRowsIncludingDuplicates") != null)
                        {
                            this.PrimaryQueryResult.TotalRowsIncludingDuplicates = (int)relevantResults.Element(d + "TotalRowsIncludingDuplicates");
                        }

                        XElement queryMod = relevantResults.Descendants(d + "Key").FirstOrDefault(e => e.Value == "QueryModification");
                        if (queryMod != null)
                        {
                            this.PrimaryQueryResult.QueryModification = queryMod.XPathSelectElement("..").Element(d + "Value").Value;
                        }

                        var table = relevantResults.Element(d + "Table");
                        if (table != null && table.HasElements)
                        {
                            var rows = table.Element(d + "Rows");
                            if (rows != null && rows.HasElements)
                            {
                                List<ResultItem> resultItems = new List<ResultItem>();

                                var items = rows.Elements(d + "element");
                                foreach (var item in items)
                                {
                                    ResultItem resultItem = new ResultItem();

                                    if (item.Element(d + "Cells") != null
                                        && item.Element(d + "Cells").HasElements)
                                    {
                                        var itemValues = item.Element(d + "Cells").Elements(d + "element");
                                        foreach (var itemValue in itemValues)
                                        {
                                            resultItem.Add(itemValue.Element(d + "Key").Value, itemValue.Element(d + "Value").Value);
                                        }
                                    }

                                    resultItems.Add(resultItem);
                                }

                                this.PrimaryQueryResult.RelevantResults = resultItems;
                            }
                        }
                    }

                    #endregion

                    #region Refinement Results

                    var refinementResults = primaryQueryResult.Element(d + "RefinementResults");
                    if (refinementResults != null)
                    {
                        var refiners = refinementResults.Element(d + "Refiners");
                        if (refiners != null && refiners.HasElements)
                        {
                            List<RefinerResult> refinerResults = new List<RefinerResult>();

                            var items = refiners.Elements(d + "element");
                            foreach (var item in items)
                            {
                                RefinerResult refinerResult = new RefinerResult();
                                refinerResult.Name = (string)item.Element(d + "Name");

                                if (item.Element(d + "Entries") != null && item.Element(d + "Entries").HasElements)
                                {
                                    var entries = item.Element(d + "Entries").Elements(d + "element");
                                    foreach (var entry in entries)
                                    {
                                        refinerResult.Add(new RefinementItem
                                        {
                                            Count = (long)entry.Element(d + "RefinementCount"),
                                            Name = (string)entry.Element(d + "RefinementName"),
                                            Token = (string)entry.Element(d + "RefinementToken"),
                                            Value = (string)entry.Element(d + "RefinementValue"),
                                        });
                                    }
                                }

                                refinerResults.Add(refinerResult);
                            }

                            this.PrimaryQueryResult.RefinerResults = refinerResults;
                        }
                    }

                    #endregion
                }

                #endregion

                #region Secondary Query Results

                XElement secondaryQueryResults = root.Element(d + "SecondaryQueryResults");
                if (secondaryQueryResults != null && secondaryQueryResults.HasElements)
                {
                    this.SecondaryQueryResults = new List<QueryResult>();

                    var resultItems = secondaryQueryResults.Elements(d + "element");
                    foreach (var resultItem in resultItems)
                    {
                        QueryResult secondaryQueryResult = new QueryResult();

                        if (resultItem.Element(d + "QueryId") != null)
                        {
                            secondaryQueryResult.QueryId = (string)resultItem.Element(d + "QueryId");
                        }

                        if (resultItem.Element(d + "QueryRuleId") != null)
                        {
                            secondaryQueryResult.QueryRuleId = (string)resultItem.Element(d + "QueryRuleId");
                        }

                        var relevantResults = resultItem.Element(d + "RelevantResults");
                        if (relevantResults != null)
                        {
                            if (relevantResults.Element(d + "TotalRows") != null)
                            {
                                secondaryQueryResult.TotalRows = (int)relevantResults.Element(d + "TotalRows");
                            }

                            if (relevantResults.Element(d + "TotalRowsIncludingDuplicates") != null)
                            {
                                secondaryQueryResult.TotalRowsIncludingDuplicates = (int)relevantResults.Element(d + "TotalRowsIncludingDuplicates");
                            }

                            XElement queryMod = relevantResults.Descendants(d + "Key").FirstOrDefault(e => e.Value == "QueryModification");
                            if (queryMod != null)
                            {
                                secondaryQueryResult.QueryModification = queryMod.XPathSelectElement("..").Element(d + "Value").Value;
                            }

                            var table = relevantResults.Element(d + "Table");
                            if (table != null && table.HasElements)
                            {
                                var rows = table.Element(d + "Rows");
                                if (rows != null && rows.HasElements)
                                {
                                    List<ResultItem> resultRows = new List<ResultItem>();

                                    var items = rows.Elements(d + "element");
                                    foreach (var item in items)
                                    {
                                        ResultItem resultRow = new ResultItem();

                                        if (item.Element(d + "Cells") != null && item.Element(d + "Cells").HasElements)
                                        {
                                            var itemValues = item.Element(d + "Cells").Elements(d + "element");
                                            foreach (var itemValue in itemValues)
                                            {
                                                resultRow.Add(itemValue.Element(d + "Key").Value, itemValue.Element(d + "Value").Value);
                                            }
                                        }

                                        resultRows.Add(resultRow);
                                    }

                                    secondaryQueryResult.RelevantResults = resultRows;
                                }
                            }
                        }

                        this.SecondaryQueryResults.Add(secondaryQueryResult);
                    }

                }

                #endregion
            }
        }

        private void ProcessJsonFromGetRequest()
        {
            XmlReader reader =
                       JsonReaderWriterFactory
                               .CreateJsonReader(Encoding.UTF8.GetBytes(this.ResponseContent), new XmlDictionaryReaderQuotas());

            XElement root = XElement.Load(reader);
            XElement queryElm = root.XPathSelectElement("//query");

            if (queryElm != null)
            {
                XElement elapsedTimeElm = queryElm.Element("ElapsedTime");
                if (elapsedTimeElm != null)
                {
                    this.QueryElapsedTime = elapsedTimeElm.Value;
                }

                XElement propertiesElm = queryElm.Element("Properties");
                if (propertiesElm != null && propertiesElm.HasElements)
                {
                    if (propertiesElm.Element("results") != null)
                    {
                        foreach (var item in propertiesElm.Element("results").Elements("item"))
                        {
                            if (item != null && item.Element("Key") != null)
                            {
                                if (item.Element("Key").Value == "SerializedQuery")
                                {
                                    this.SerializedQuery = item.Element("Value").Value;
                                }
                                else if (item.Element("Key").Value == "IsPartial")
                                {
                                    if (bool.TryParse(item.Element("Value").Value, out bool isPartial))
                                    {
                                        this.IsPartial = isPartial;
                                    }
                                }
                                else if (item.Element("Key").Value == "MultiGeoSearchStatus")
                                {
                                    this.MultiGeoSearchStatus = item.Element("Value").Value;
                                }
                            }
                        }
                    }
                }

                #region Triggered Rules

                XElement triggeredRulesElm = queryElm.Element("TriggeredRules");
                if (triggeredRulesElm != null && triggeredRulesElm.HasElements)
                {
                    if (triggeredRulesElm.Element("results") != null)
                    {
                        this.TriggeredRules = new List<string>();

                        foreach (var item in triggeredRulesElm.Element("results").Elements("item"))
                        {
                            this.TriggeredRules.Add(item.Value);
                        }
                    }
                }

                #endregion

                #region Primary Query Results

                XElement primaryQueryResult = queryElm.Element("PrimaryQueryResult");
                if (primaryQueryResult != null)
                {
                    this.PrimaryQueryResult = new QueryResult();

                    if (primaryQueryResult.Element("QueryId") != null)
                    {
                        this.PrimaryQueryResult.QueryId = (string)primaryQueryResult.Element("QueryId");
                    }

                    if (primaryQueryResult.Element("QueryRuleId") != null)
                    {
                        this.PrimaryQueryResult.QueryRuleId = (string)primaryQueryResult.Element("QueryRuleId");
                    }

                    #region Relevant Results

                    var relevantResults = primaryQueryResult.Element("RelevantResults");
                    if (relevantResults != null)
                    {
                        if (relevantResults.Element("TotalRows") != null)
                        {
                            this.PrimaryQueryResult.TotalRows = (int)relevantResults.Element("TotalRows");
                        }

                        if (relevantResults.Element("TotalRowsIncludingDuplicates") != null)
                        {
                            this.PrimaryQueryResult.TotalRowsIncludingDuplicates = (int)relevantResults.Element("TotalRowsIncludingDuplicates");
                        }

                        XElement queryMod = relevantResults.Descendants("Key").FirstOrDefault(e => e.Value == "QueryModification");
                        if (queryMod != null)
                        {
                            this.PrimaryQueryResult.QueryModification = queryMod.XPathSelectElement("..").Element("Value").Value;
                        }

                        var table = relevantResults.Element("Table");
                        if (table != null && table.HasElements)
                        {
                            var rows = table.Element("Rows");
                            if (rows != null && rows.Element("results") != null && rows.Element("results").HasElements)
                            {
                                List<ResultItem> resultItems = new List<ResultItem>();

                                var items = rows.Element("results").Elements("item");
                                foreach (var item in items)
                                {
                                    ResultItem resultItem = new ResultItem();

                                    if (item.Element("Cells") != null && item.Element("Cells").Element("results") != null
                                        && item.Element("Cells").Element("results").HasElements)
                                    {
                                        var itemValues = from i in item.Element("Cells").Element("results").Elements("item")
                                                         orderby i.Element("Key").Value ascending
                                                         select i;
                                        foreach (var itemValue in itemValues)
                                        {
                                            resultItem.Add(itemValue.Element("Key").Value, itemValue.Element("Value").Value);
                                        }
                                    }

                                    resultItems.Add(resultItem);
                                }

                                this.PrimaryQueryResult.RelevantResults = resultItems;
                            }
                        }
                    }

                    #endregion

                    #region Refinement Results

                    var refinementResults = primaryQueryResult.Element("RefinementResults");
                    if (refinementResults != null)
                    {
                        var refiners = refinementResults.Element("Refiners");
                        if (refiners != null && refiners.HasElements)
                        {
                            var results = refiners.Element("results");
                            if (results != null && results.HasElements)
                            {
                                List<RefinerResult> refinerResults = new List<RefinerResult>();

                                var items = results.Elements("item");
                                foreach (var item in items)
                                {
                                    RefinerResult refinerResult = new RefinerResult();
                                    refinerResult.Name = (string)item.Element("Name");

                                    if (item.Element("Entries") != null && item.Element("Entries").Element("results") != null
                                        && item.Element("Entries").Element("results").HasElements)
                                    {
                                        var entries = item.Element("Entries").Element("results").Elements("item");
                                        foreach (var entry in entries)
                                        {
                                            refinerResult.Add(new RefinementItem
                                                              {
                                                                  Count = (long)entry.Element("RefinementCount"),
                                                                  Name = (string)entry.Element("RefinementName"),
                                                                  Token = (string)entry.Element("RefinementToken"),
                                                                  Value = (string)entry.Element("RefinementValue"),
                                                              });
                                        }
                                    }

                                    refinerResults.Add(refinerResult);
                                }

                                this.PrimaryQueryResult.RefinerResults = refinerResults;
                            }
                        }
                    }

                    #endregion
                }

                #endregion

                #region Secondary Query Results

                XElement secondaryQueryResults = queryElm.Element("SecondaryQueryResults");
                if (secondaryQueryResults != null && secondaryQueryResults.Element("results") != null)
                {
                    var resultsElm = secondaryQueryResults.Element("results");

                    if (resultsElm.HasElements)
                    {
                        this.SecondaryQueryResults = new List<QueryResult>();

                        var resultItems = resultsElm.Elements("item");
                        foreach (var resultItem in resultItems)
                        {
                            QueryResult secondaryQueryResult = new QueryResult();

                            if (resultItem.Element("QueryId") != null)
                            {
                                secondaryQueryResult.QueryId = (string)resultItem.Element("QueryId");
                            }

                            if (resultItem.Element("QueryRuleId") != null)
                            {
                                secondaryQueryResult.QueryRuleId = (string)resultItem.Element("QueryRuleId");
                            }

                            var relevantResults = resultItem.Element("RelevantResults");
                            if (relevantResults != null)
                            {
                                if (relevantResults.Element("ResultTitle") != null)
                                {
                                    secondaryQueryResult.ResultTitle = (string)relevantResults.Element("ResultTitle");
                                }

                                if (relevantResults.Element("ResultTitleUrl") != null)
                                {
                                    secondaryQueryResult.ResultTitleUrl = (string)relevantResults.Element("ResultTitleUrl");
                                }

                                if (relevantResults.Element("TotalRows") != null)
                                {
                                    secondaryQueryResult.TotalRows = (int)relevantResults.Element("TotalRows");
                                }

                                if (relevantResults.Element("TotalRowsIncludingDuplicates") != null)
                                {
                                    secondaryQueryResult.TotalRowsIncludingDuplicates = (int)relevantResults.Element("TotalRowsIncludingDuplicates");
                                }

                                XElement queryMod = relevantResults.Descendants("Key").FirstOrDefault(e => e.Value == "QueryModification");
                                if (queryMod != null)
                                {
                                    secondaryQueryResult.QueryModification = queryMod.XPathSelectElement("..").Element("Value").Value;
                                }

                                var table = relevantResults.Element("Table");
                                if (table != null && table.HasElements)
                                {
                                    var rows = table.Element("Rows");
                                    if (rows != null && rows.Element("results") != null && rows.Element("results").HasElements)
                                    {
                                        List<ResultItem> resultRows = new List<ResultItem>();

                                        var items = rows.Element("results").Elements("item");
                                        foreach (var item in items)
                                        {
                                            ResultItem resultRow = new ResultItem();

                                            if (item.Element("Cells") != null && item.Element("Cells").Element("results") != null
                                                && item.Element("Cells").Element("results").HasElements)
                                            {
                                                var itemValues = item.Element("Cells").Element("results").Elements("item");
                                                foreach (var itemValue in itemValues)
                                                {
                                                    resultRow.Add(itemValue.Element("Key").Value, itemValue.Element("Value").Value);
                                                }
                                            }

                                            resultRows.Add(resultRow);
                                        }

                                        secondaryQueryResult.RelevantResults = resultRows;
                                    }
                                }
                            }

                            this.SecondaryQueryResults.Add(secondaryQueryResult);
                        }
                    }
                }

                #endregion
            }
        }

        private void ProcessJsonFromPostRequest()
        {
            XmlReader reader =
                       JsonReaderWriterFactory
                               .CreateJsonReader(Encoding.UTF8.GetBytes(this.ResponseContent), new XmlDictionaryReaderQuotas());

            XElement root = XElement.Load(reader);
            XElement postqueryElm = root.XPathSelectElement("//postquery");

            if (postqueryElm != null)
            {
                XElement elapsedTimeElm = postqueryElm.Element("ElapsedTime");
                if (elapsedTimeElm != null)
                {
                    this.QueryElapsedTime = elapsedTimeElm.Value;
                }

                XElement propertiesElm = postqueryElm.Element("Properties");
                if (propertiesElm != null && propertiesElm.HasElements)
                {
                    if (propertiesElm.Element("results") != null)
                    {
                        foreach (var item in propertiesElm.Element("results").Elements("item"))
                        {
                            if (item != null && item.Element("Key") != null)
                            {
                                if (item.Element("Key").Value == "SerializedQuery")
                                {
                                    this.SerializedQuery = item.Element("Value").Value;
                                }
                                else if (item.Element("Key").Value == "IsPartial")
                                {
                                    if (bool.TryParse(item.Element("Value").Value, out bool isPartial))
                                    {
                                        this.IsPartial = isPartial;
                                    }
                                }
                                else if (item.Element("Key").Value == "MultiGeoSearchStatus")
                                {
                                    this.MultiGeoSearchStatus = item.Element("Value").Value;
                                }
                            }
                        }
                    }
                }

                #region Triggered Rules

                XElement triggeredRulesElm = postqueryElm.Element("TriggeredRules");
                if (triggeredRulesElm != null && triggeredRulesElm.HasElements)
                {
                    if (triggeredRulesElm.Element("results") != null)
                    {
                        this.TriggeredRules = new List<string>();

                        foreach (var item in triggeredRulesElm.Element("results").Elements("item"))
                        {
                            this.TriggeredRules.Add(item.Value);
                        }
                    }
                }

                #endregion

                #region Primary Query Results

                XElement primaryQueryResult = postqueryElm.Element("PrimaryQueryResult");
                if (primaryQueryResult != null)
                {
                    this.PrimaryQueryResult = new QueryResult();

                    if (primaryQueryResult.Element("QueryId") != null)
                    {
                        this.PrimaryQueryResult.QueryId = (string)primaryQueryResult.Element("QueryId");
                    }

                    if (primaryQueryResult.Element("QueryRuleId") != null)
                    {
                        this.PrimaryQueryResult.QueryRuleId = (string)primaryQueryResult.Element("QueryRuleId");
                    }

                    #region Relevant Results

                    var relevantResults = primaryQueryResult.Element("RelevantResults");
                    if (relevantResults != null)
                    {
                        if (relevantResults.Element("TotalRows") != null)
                        {
                            this.PrimaryQueryResult.TotalRows = (int)relevantResults.Element("TotalRows");
                        }

                        if (relevantResults.Element("TotalRowsIncludingDuplicates") != null)
                        {
                            this.PrimaryQueryResult.TotalRowsIncludingDuplicates = (int)relevantResults.Element("TotalRowsIncludingDuplicates");
                        }

                        XElement queryMod = relevantResults.Descendants("Key").FirstOrDefault(e => e.Value == "QueryModification");
                        if (queryMod != null)
                        {
                            this.PrimaryQueryResult.QueryModification = queryMod.XPathSelectElement("..").Element("Value").Value;
                        }

                        var table = relevantResults.Element("Table");
                        if (table != null && table.HasElements)
                        {
                            var rows = table.Element("Rows");
                            if (rows != null && rows.Element("results") != null && rows.Element("results").HasElements)
                            {
                                List<ResultItem> resultItems = new List<ResultItem>();

                                var items = rows.Element("results").Elements("item");
                                foreach (var item in items)
                                {
                                    ResultItem resultItem = new ResultItem();

                                    if (item.Element("Cells") != null && item.Element("Cells").Element("results") != null
                                        && item.Element("Cells").Element("results").HasElements)
                                    {
                                        var itemValues = item.Element("Cells").Element("results").Elements("item");
                                        foreach (var itemValue in itemValues)
                                        {
                                            resultItem.Add(itemValue.Element("Key").Value, itemValue.Element("Value").Value);
                                        }
                                    }

                                    resultItems.Add(resultItem);
                                }

                                this.PrimaryQueryResult.RelevantResults = resultItems;
                            }
                        }
                    }

                    #endregion

                    #region Refinement Results

                    var refinementResults = primaryQueryResult.Element("RefinementResults");
                    if (refinementResults != null)
                    {
                        var refiners = refinementResults.Element("Refiners");
                        if (refiners != null && refiners.HasElements)
                        {
                            var results = refiners.Element("results");
                            if (results != null && results.HasElements)
                            {
                                List<RefinerResult> refinerResults = new List<RefinerResult>();

                                var items = results.Elements("item");
                                foreach (var item in items)
                                {
                                    RefinerResult refinerResult = new RefinerResult();
                                    refinerResult.Name = (string)item.Element("Name");

                                    if (item.Element("Entries") != null && item.Element("Entries").Element("results") != null
                                        && item.Element("Entries").Element("results").HasElements)
                                    {
                                        var entries = item.Element("Entries").Element("results").Elements("item");
                                        foreach (var entry in entries)
                                        {
                                            refinerResult.Add(new RefinementItem
                                            {
                                                Count = (long)entry.Element("RefinementCount"),
                                                Name = (string)entry.Element("RefinementName"),
                                                Token = (string)entry.Element("RefinementToken"),
                                                Value = (string)entry.Element("RefinementValue"),
                                            });
                                        }
                                    }

                                    refinerResults.Add(refinerResult);
                                }

                                this.PrimaryQueryResult.RefinerResults = refinerResults;
                            }
                        }
                    }

                    #endregion
                }

                #endregion

                #region Secondary Query Results

                XElement secondaryQueryResults = postqueryElm.Element("SecondaryQueryResults");
                if (secondaryQueryResults != null && secondaryQueryResults.Element("results") != null)
                {
                    var resultsElm = secondaryQueryResults.Element("results");

                    if (resultsElm.HasElements)
                    {
                        this.SecondaryQueryResults = new List<QueryResult>();

                        var resultItems = resultsElm.Elements("item");
                        foreach (var resultItem in resultItems)
                        {
                            QueryResult secondaryQueryResult = new QueryResult();

                            if (resultItem.Element("QueryId") != null)
                            {
                                secondaryQueryResult.QueryId = (string)resultItem.Element("QueryId");
                            }

                            if (resultItem.Element("QueryRuleId") != null)
                            {
                                secondaryQueryResult.QueryRuleId = (string)resultItem.Element("QueryRuleId");
                            }

                            var relevantResults = resultItem.Element("RelevantResults");
                            if (relevantResults != null)
                            {
                                if (relevantResults.Element("TotalRows") != null)
                                {
                                    secondaryQueryResult.TotalRows = (int)relevantResults.Element("TotalRows");
                                }

                                if (relevantResults.Element("TotalRowsIncludingDuplicates") != null)
                                {
                                    secondaryQueryResult.TotalRowsIncludingDuplicates = (int)relevantResults.Element("TotalRowsIncludingDuplicates");
                                }

                                XElement queryMod = relevantResults.Descendants("Key").FirstOrDefault(e => e.Value == "QueryModification");
                                if (queryMod != null)
                                {
                                    secondaryQueryResult.QueryModification = queryMod.XPathSelectElement("..").Element("Value").Value;
                                }

                                var table = relevantResults.Element("Table");
                                if (table != null && table.HasElements)
                                {
                                    var rows = table.Element("Rows");
                                    if (rows != null && rows.Element("results") != null && rows.Element("results").HasElements)
                                    {
                                        List<ResultItem> resultRows = new List<ResultItem>();

                                        var items = rows.Element("results").Elements("item");
                                        foreach (var item in items)
                                        {
                                            ResultItem resultRow = new ResultItem();

                                            if (item.Element("Cells") != null && item.Element("Cells").Element("results") != null
                                                && item.Element("Cells").Element("results").HasElements)
                                            {
                                                var itemValues = item.Element("Cells").Element("results").Elements("item");
                                                foreach (var itemValue in itemValues)
                                                {
                                                    resultRow.Add(itemValue.Element("Key").Value, itemValue.Element("Value").Value);
                                                }
                                            }

                                            resultRows.Add(resultRow);
                                        }

                                        secondaryQueryResult.RelevantResults = resultRows;
                                    }
                                }
                            }

                            this.SecondaryQueryResults.Add(secondaryQueryResult);
                        }
                    }
                }

                #endregion
            }
        }
    }
}
