using System;
using System.Collections.Generic;
using System.Xml;
using static PSSQT.RankLogHelper;

namespace PSSQT.RankLogParser
{
    public class RankLogStageModel : RankLogElement
    {
        public string Type { get; set; }
        public int NumHiddenNodes { get; set; }

        public bool IsLayer2 { get; set; }

        public List<double> Layer2Threshold { get; set; }
        public List<double> Layer2Weight { get; set; }

        public RankLogStageModelBM25Feature BM25Feature { get; set; }

        internal RankLogStageModel()
        {
        }

        internal override void Parse(XmlNode node)
        {
            Type = node.Attributes["type"].Value;
            NumHiddenNodes = int.Parse(node.Attributes["num_hidden_nodes"].Value);

            var layer2 = node.SelectSingleNode("layer2");

            IsLayer2 = layer2 != null;

            if (IsLayer2)
            {
                Layer2Threshold = ParseDoubleList(layer2["threshold"].Attributes["values"].Value, NumHiddenNodes);
                Layer2Weight = ParseDoubleList(layer2["neurons_weight"].Attributes["values"].Value, NumHiddenNodes);
            }

            var bm25_feature_node = node.SelectSingleNode("bm25_feature");

            if (bm25_feature_node != null)
            {
                var bm25Feature = new RankLogStageModelBM25Feature();

                bm25Feature.Parse(bm25_feature_node);

                BM25Feature = bm25Feature;
            }

            // Don't need rest for now. Adds are duplicated under the feature itself

            //Elements = new List<RankLogElement>();

            //foreach (XmlNode childNode in node.ChildNodes)
            //{
            //    RankLogElement element = Create(childNode);

            //    Elements.Add(element);

            //}
        }

    }
}