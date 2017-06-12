using System.Collections.Generic;
using System.Xml;

namespace PSSQT.RankLogParser
{
    internal class RankLogStageStaticFeature : RankLogStageBaseStaticFeature
    {
        public double Transformed { get; set; }
        public double Normalized { get; set; }

        internal RankLogStageStaticFeature(RankLogStage parent) : base(parent)
        {
        }

        internal override void Parse(XmlNode node)
        {
            base.Parse(node);

            //< static_feature name = "UrlDepth" property_name = "UrlDepth" used_default = "0" raw_value = "4" raw_value_transformed = "4" transformed = "0.557406" normalized = "-0.295484" hidden_nodes_adds = "0.175067 0.0734096 -0.0250903 0.0451885 0.0956468 0.00821871 " />

            Transformed = double.Parse(node.Attributes["transformed"].Value);
            Normalized = double.Parse(node.Attributes["normalized"].Value);
        }


 
    }
}