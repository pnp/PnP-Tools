using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using static PSSQT.RankLogHelper;

namespace PSSQT.RankLogParser
{
    internal class RankLogStage : RankLogElement
    {
        public string Type { get; set; }
        public Guid StageId { get; set; }
        public double Rank { get; set; }
        public double RankAfter { get; set; }
        public string StageRankInterval { get; set; }



        public List<RankLogStageFeature> Features { get; set; }

        public RankLogStageModel StageModel { get; set; }

        internal RankLogStage()
        {
        }

        internal override void Parse(XmlNode node)
        {
            Type = node.Attributes["type"].Value;
            StageId = Guid.Parse(node.Attributes["id"].Value);
            Rank = double.Parse(node.Attributes["rank"].Value);
            RankAfter = double.Parse(node.Attributes["rank_after"].Value);
            StageRankInterval = node.Attributes["stage_rank_interval"].Value;

            // now parse features and stage model

            Features = new List<RankLogStageFeature>();

            foreach (XmlNode childNode in node.ChildNodes)
            {
                RankLogElement element = CreateChildElement(childNode);

                element.Parse(childNode);

                if (element is RankLogStageModel stageModel)
                {
                    StageModel = stageModel;
                }
                else if (element is RankLogStageFeature stageFeature)
                {
                    // everything else should be a RankLogStageRankFeature
                    Features.Add(stageFeature);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            // Give each feature a chance to do cleanup/adjustments once all features are in place
            foreach (var feature in Features)
            {
                feature.AllFeaturesComplete();
            }
        }

        protected override RankLogElement CreateChildElement(XmlNode node)
        {
            if (node.Name.Equals("bm25", StringComparison.CurrentCultureIgnoreCase))
            {
                return new RankLogStageBM25(this);
            }
            else if (node.Name.Equals("static_feature", StringComparison.CurrentCultureIgnoreCase))
            {
                return new RankLogStageStaticFeature(this);
            }
            else if (node.Name.Equals("bucketed_static_feature", StringComparison.CurrentCultureIgnoreCase))
            {
                return new RankLogStageBucketedStaticFeature(this);
            }
            else if (node.Name.Equals("proximity_feature", StringComparison.CurrentCultureIgnoreCase))
            {
                return new RankLogStageProximityFeature(this);
            }
            else if (node.Name.Equals("dynamic", StringComparison.CurrentCultureIgnoreCase))
            {
                return new RankLogStageDynamicFeature(this);
            }
            else if (node.Name.Equals("stage_model", StringComparison.CurrentCultureIgnoreCase))
            {
                return new RankLogStageModel();
            }
            else
            {
                return base.CreateChildElement(node);
            }

        }

        internal List<double> SumHiddenNodesAdds(bool excludeDynamic = false)
        {
            var sumHiddenNodes = new List<double>(StageModel.NumHiddenNodes);

            for (int i = 0; i < StageModel.NumHiddenNodes; i++)
            {
                double sum = 0;

                foreach (var feature in Features)
                {
                    if (!(excludeDynamic && feature.IsDynamic))
                    {
                        sum += feature.HiddenNodesAdds[i];
                    }
                }

                sumHiddenNodes.Add(sum);
            }

            return sumHiddenNodes;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(base.ToString());

            Append(sb, "Type", Type, 1);
            Append(sb, "Stage Id", StageId.ToString(), 1);
            Append(sb, "Rank", Rank.ToString(), 1);
            Append(sb, "Rank After", RankAfter.ToString(), 1);

            Append(sb, "Features", "", 1);

            foreach (var feature in Features)
            {
                Append(sb, feature.Name, feature.EstimatedRankContribution().ToString(), 2);
            }

            return sb.ToString();
        }
    }
}