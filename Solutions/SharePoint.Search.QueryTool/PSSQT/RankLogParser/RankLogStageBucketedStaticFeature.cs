using System.Xml;

namespace PSSQT.RankLogParser
{
    internal class RankLogStageBucketedStaticFeature : RankLogStageBaseStaticFeature
    {
        public double RawValueIn { get; set; }

        public RankLogStageBucketedStaticFeature(RankLogStage parent) : base(parent)
        {
        }

        internal override void Parse(XmlNode node)
        {
            base.Parse(node);

            RawValueIn = double.Parse(node.Attributes["raw_value_in"].Value);
        }


    }
}