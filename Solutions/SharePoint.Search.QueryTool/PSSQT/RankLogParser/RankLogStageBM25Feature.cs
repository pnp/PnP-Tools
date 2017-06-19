using System.Xml;

namespace PSSQT.RankLogParser
{
    internal class RankLogStageBM25Feature : RankLogStageFeature
    {
        public BM25Schema Schema { get; set; }
        public BM25QueryTerm QueryTerm { get; set; }

        public double Score { get; set; }
        public double Transformed { get; set; }
        public double Normalized { get; set; }

        public RankLogStageBM25Feature(RankLogStage parent, XmlNode node) : base(parent, node)
        {
            XmlNode final = node.SelectSingleNode("final");

            Score = double.Parse(final.Attributes["score"].Value);
            Transformed = double.Parse(final.Attributes["transformed"].Value);
            Normalized = double.Parse(final.Attributes["normalized"].Value);
        }

 
        protected override string SelectHiddenNodes(XmlNode node)
        {
            return node.SelectSingleNode("final").Attributes["hidden_nodes_adds"].Value;
        }

    }
}