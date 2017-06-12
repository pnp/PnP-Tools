using PSSQT.RankLogParser;
using SearchQueryTool.Helpers;
using SearchQueryTool.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Xml.Linq;

namespace PSSQT
{

    public class ExplainRankResultProcessor : RankXMLResultProcessor
    {
        public string XsltFilename { get; set; }

        public ExplainRankResultProcessor(Cmdlet cmdlet) :
            base(cmdlet)
        {
        }


        protected override void ProcessPrimaryRelevantResults(List<ResultItem> relevantResults)
        {
            XElement results = new XElement("results");
            //var aggregateValues = new Dictionary<string, RankingFeatureAggregateValues>();
            //var metaResults = new List<Dictionary<string, double>>();
            XElement rankcontrib = new XElement("rankcontributions");
            rankcontrib.Add(new XAttribute("note", "Please note that these are estimated values."));
            
            int n = 1;

            if (relevantResults != null)
            {

                foreach (var resultItem in relevantResults)
                {
                    XElement result = new XElement("result", new XAttribute("pos", n));

                    if (SelectedProperties != null)
                    {
                        XmlAddSelectedProperties(resultItem, result, rankcontrib, n);
                    }
                    else
                    {
                        foreach (var key in resultItem.Keys)
                        {
                            result.Add(new XElement(key, resultItem[key]));
                        }
                    }

                    results.Add(result);
                    n++;
                }
            }

            results.Add(rankcontrib);

            string html;

            if (!String.IsNullOrWhiteSpace(XsltFilename))
            {
                var xslt = File.ReadAllText(XsltFilename);

                Cmdlet.WriteDebug(String.Format("XSLT:\n{0}", xslt));

                html = TransformResults.Transform(results.ToString(), xslt);
            }
            else
            {
                Cmdlet.WriteVerbose(String.Format("XSLT:\n{0}", TransformResults.DefaultXSLT));

                html = TransformResults.Transform(results.ToString());
            }


            PSObject item = new PSObject(html);

            PrimaryResultWriteItem(item);
        }

        public override void Configure()
        {
            base.Configure();

            if (Cmdlet is SearchSPIndex)
            {
                var cl = (SearchSPIndex)Cmdlet;

                if (!String.IsNullOrWhiteSpace(cl.ExplainRankXsltFile))
                {
                    XsltFilename = cl.GetRootedPath(cl.ExplainRankXsltFile.Trim());
                }
            }
        }

        protected override void EnsurePropertiesPresent()
        {
            base.EnsurePropertiesPresent();

            if (Cmdlet is SearchSPIndex)
            {
                var cl = (SearchSPIndex)Cmdlet;

                cl.AddProperty("Title");
                cl.AddProperty("Path");
            }
        }

        protected void XmlAddSelectedProperties(ResultItem resultItem, XElement element, XElement rankcontrib, int n)
        {
            // force the order of SelectProperties. If user specifies -Properties "Author,Title", they should appear in that order
            foreach (var selProp in SelectedProperties)
            {
                var key = resultItem.Keys.FirstOrDefault(k => k.Equals(selProp, StringComparison.InvariantCultureIgnoreCase));

                if (!String.IsNullOrWhiteSpace(key))
                {
                    XmlAddItemProperty(element, key, resultItem[key], rankcontrib, n);
                }
            }
        }

        protected virtual void XmlAddItemProperty(XElement element, string key, string value, XElement rankcontrib, int n)
        {
            if (key.Equals("RankDetail", StringComparison.InvariantCultureIgnoreCase))
            {
                calculateEstimatedRankContributions(element, value, rankcontrib, n);
                element.Add(XElement.Parse(value));
                // add weighted 
            }
            else
            {
                element.Add(new XElement(key, value));
            }
        }

        private void calculateEstimatedRankContributions(XElement element, string value, XElement rankcontrib, int n)
        {
     
            RankLog ranklog = RankLog.CreateRankLogFromXml(value);
            RankLogStage finalStage = ranklog.FinalRankStage;

            XElement result = new XElement("result");

            result.Add(new XAttribute("pos", n));

            foreach (var feature in finalStage.Features)
            {
                result.Add(new XAttribute(feature.Name, feature.EstimatedRankContribution()));
            }

            // add external boost. "Rank" returned with results - "rank_after" from the last stage in RankXML
            XElement rank = element.Element("Rank");

            double finalRank = double.Parse(rank.Value);
            double externalBoost = rank != null ? (Math.Round(finalRank - finalStage.RankAfter, 4)) : 0.0;
            double threshold = 1.0E-4;

            if (Math.Abs(externalBoost) < threshold)
            {
                externalBoost = 0;
            }

            result.Add(new XAttribute("ExternalBoost", externalBoost));
            result.Add(new XElement("Rank", Math.Round(finalRank,3)));

            rankcontrib.Add(result);
                
        }
    }

}
