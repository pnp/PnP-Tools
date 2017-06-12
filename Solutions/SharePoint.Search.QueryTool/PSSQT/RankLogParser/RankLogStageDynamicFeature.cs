using System.Collections.Generic;
using System.Xml;
using static PSSQT.RankLogHelper;

namespace PSSQT.RankLogParser
{
    internal class RankLogStageDynamicFeature : RankLogStageFeature
    {
        public List<double> FinalRank { get; set; }


        internal RankLogStageDynamicFeature(RankLogStage parent) : base(parent)
        {
            IsDynamic = true;
        }

        internal override void Parse(XmlNode node)
        {
            base.Parse(node);

            XmlNode final = node.SelectSingleNode("final");

            FinalRank = ParseDoubleList(final.Attributes["rank"].Value);
        }

        protected override string SelectHiddenNodes(XmlNode node)
        {
            XmlNode final = node.SelectSingleNode("final");

            return final.Attributes["rank"].Value;
        }

        internal override void AllFeaturesComplete()
        {
            // adjust hidden nodes based on sum of all other nodes since anchor text complete only contains final rank

            int i = 0;
            var sum = Parent.SumHiddenNodesAdds(true);

            var newValues = new List<double>(Parent.StageModel.NumHiddenNodes);

            foreach (var node in HiddenNodesAdds)
            {
                newValues.Add(node - sum[i++]);
            }

            HiddenNodesAdds = newValues;
        }
    }
}