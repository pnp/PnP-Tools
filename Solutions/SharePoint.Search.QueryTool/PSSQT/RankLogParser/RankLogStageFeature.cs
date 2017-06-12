using System;
using System.Collections.Generic;
using System.Xml;
using static PSSQT.RankLogHelper;

namespace PSSQT.RankLogParser
{
    public abstract class RankLogStageFeature : RankLogElement
    {
        public string Name { get; set; }
 
        public List<double> HiddenNodesAdds { get; set; }

        public bool IsDynamic { get; protected set; }

        internal RankLogStage Parent { get; set; }

        internal RankLogStageFeature(RankLogStage parent)
        {
            Parent = parent;
        }

        internal override void Parse(XmlNode node)
        {
            Name = node.Attributes["name"].Value;

            HiddenNodesAdds = ParseDoubleList(SelectHiddenNodes(node));
        }


        protected virtual string SelectHiddenNodes(XmlNode node)
        {
            return node.Attributes["hidden_nodes_adds"].Value;
        }

        public virtual double EstimatedRankContribution()
        {
            double score = 0;
            int i = 0;
            var model = Parent.StageModel;

            foreach (var num in HiddenNodesAdds)
            {
                if (model.IsLayer2)
                {
                    score += num * model.Layer2Weight[i++];
                }
                else
                {
                    score += num;
                }
            }

            return score;
        }

        internal virtual void AllFeaturesComplete()
        {
            // most features do nothing. Dynamic Features need to do some adjustments;
        }
    }
}