using System.Collections.Generic;
using System.Xml;

namespace PSSQT.RankLogParser
{
    internal class RankLogStageBaseStaticFeature : RankLogStageFeature
    {
        public string PropertyName { get; set; }
        public bool UsedDefault { get; set; }
        public double RawValue { get; set; }
        public double RawValueTransformed { get; set; }
        protected RankLogStageBaseStaticFeature(RankLogStage parent) : base(parent)
        {
        }

        internal override void Parse(XmlNode node)
        {
            base.Parse(node);

            PropertyName = node.Attributes["property_name"].Value;

            UsedDefault = node.Attributes["used_default"].Value.Equals("0") ? false : true;
            RawValue = double.Parse(node.Attributes["raw_value"].Value);
            RawValueTransformed = double.Parse(node.Attributes["raw_value_transformed"].Value);
        }


        // <bucketed_static_feature name="InternalFileType" property_name="InternalFileType" used_default="0" raw_value_in="1" raw_value="1" raw_value_transformed="1" hidden_nodes_adds="-3.04908 2.22181 1.63769 0.139462 2.01221 1.22687" />

    }
}