using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SearchQueryTool.Helpers
{
    public class RankLogParser
    {
        // Fields
        public Guid ModelID = Guid.Empty;
        public ScoreDetail[] ScoreDetails = new ScoreDetail[] {null, null};

        // Methods
        public RankLogParser(string ranklogXML)
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            var settings = new XmlReaderSettings
                               {
                                   CloseInput = true,
                                   XmlResolver = null,
                                   DtdProcessing = DtdProcessing.Prohibit
                               };
            var document = new XmlDocument();
            using (XmlReader reader = XmlReader.Create(new StringReader(ranklogXML), settings))
            {
                document.Load(reader);
            }
            ModelID = Guid.Parse(document.DocumentElement.Attributes["id"].Value);
            XmlNodeList childNodes = document.DocumentElement.SelectNodes("/rank_log")[0].ChildNodes;
            int index = 0;
            foreach (XmlNode node in childNodes)
            {
                if (node.Name != "query")
                {
                    ScoreDetails[index] = new ScoreDetail();
                    ScoreDetails[index].ModelId = Guid.Parse(node.Attributes["id"].Value);
                    ScoreDetails[index].Score = float.Parse(node.Attributes["rank_after"].Value);
                    ScoreDetails[index].OriginalScore = float.Parse(node.Attributes["rank_after"].Value);
                    ScoreDetails[index].ModelType = (node.Attributes["type"].Value == "linear")
                                                        ? "Linear"
                                                        : "2-layer neural net";
                    var list2 = new List<RankingFeature>();
                    XmlNode node2 = node["bm25"];
                    XmlNode xmlRankingModel = node["stage_model"];
                    XmlNode node4 = xmlRankingModel["bm25_feature"];
                    if ((node2 != null) && (node4 != null))
                    {
                        var item = new BM25(node2, xmlRankingModel, node4);
                        list2.Add(item);
                    }
                    foreach (XmlNode node5 in node.ChildNodes)
                    {
                        if (node5.Name == "stage_model")
                        {
                            continue;
                        }
                        string str = (node5.Attributes["name"] != null) ? node5.Attributes["name"].Value : null;
                        if (string.IsNullOrEmpty(str))
                        {
                            continue;
                        }
                        XmlNode xmlRankingModelFeature = null;
                        foreach (XmlNode node7 in xmlRankingModel.ChildNodes)
                        {
                            if ((node7.Attributes["name"] != null) && (node7.Attributes["name"].Value == str))
                            {
                                xmlRankingModelFeature = node7;
                                break;
                            }
                        }
                        if (xmlRankingModelFeature == null)
                        {
                            throw new Exception(
                                string.Format("Ranking stage {0} feature {1} is not found in ranking model",
                                              node.Attributes["id"].Value, str));
                        }
                        RankingFeature feature = null;
                        try
                        {
                            string name = node5.Name;
                            if (name != null)
                            {
                                if (!(name == "bucketed_static_feature"))
                                {
                                    if (name == "static_feature")
                                    {
                                        goto Label_036E;
                                    }
                                    if (name == "proximity_feature")
                                    {
                                        goto Label_037D;
                                    }
                                    if (name == "dynamic")
                                    {
                                        goto Label_038C;
                                    }
                                }
                                else
                                {
                                    feature = new BucketedStatic(node5, xmlRankingModel, xmlRankingModelFeature);
                                }
                            }
                            goto Label_03A1;
                            Label_036E:
                            feature = new StaticRank(node5, xmlRankingModel, xmlRankingModelFeature);
                            goto Label_03A1;
                            Label_037D:
                            feature = new MinSpan(node5, xmlRankingModel, xmlRankingModelFeature);
                            goto Label_03A1;
                            Label_038C:
                            feature = new Dynamic(node5, xmlRankingModel, xmlRankingModelFeature);
                        }
                        catch
                        {
                            feature = null;
                        }
                        Label_03A1:
                        if (feature != null)
                        {
                            list2.Add(feature);
                        }
                    }
                    ScoreDetails[index].RankingFeatures = list2;
                    index++;
                }
            }
        }
    }
}