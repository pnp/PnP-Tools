using System.Xml;

namespace PSSQT.RankLogParser
{
    internal class RankLogStageProximityFeature : RankLogStageFeature
    {
        // only parse attributes common in all proximity features
        public int Pid { get; set; }
        public string ProximityType { get; set; }
        public bool UsedDefault { get; set; }
        public double RawValue { get; set; }
        public double Transformed { get; set; }
        public double Normalized { get; set; }

        internal RankLogStageProximityFeature(RankLogStage parent) : base(parent)
        {
        }

        internal override void Parse(XmlNode node)
        {
            base.Parse(node);

            Pid = int.Parse(node.Attributes["pid"].Value);
            ProximityType = node.Attributes["proximity_type"].Value;
            UsedDefault = node.Attributes["used_default"].Value.Equals("0") ? false : true;
            RawValue = double.Parse(node.Attributes["raw_value"].Value);
            Transformed = double.Parse(node.Attributes["transformed"].Value);
            Normalized = double.Parse(node.Attributes["normalized"].Value);
        }

    }
}