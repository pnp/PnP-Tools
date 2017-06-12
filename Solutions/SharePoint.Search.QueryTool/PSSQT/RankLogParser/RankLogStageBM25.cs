using System.Xml;

namespace PSSQT.RankLogParser
{
    internal class RankLogStageBM25 : RankLogStageFeature
    {
        public BM25Schema Schema { get; set; }
        public BM25QueryTerm QueryTerm { get; set; }

        public double Score { get; set; }
        public double Transformed { get; set; }
        public double Normalized { get; set; }

        public PidMappings PidMappings { get; set; }

        internal RankLogStageBM25(RankLogStage parent) : base(parent)
        {

        }

        internal override void Parse(XmlNode node)
        {
            base.Parse(node);

            XmlNode final = node.SelectSingleNode("final");

            if (final != null)
            {
                Score = double.Parse(final.Attributes["score"].Value);
                Transformed = double.Parse(final.Attributes["transformed"].Value);
                Normalized = double.Parse(final.Attributes["normalized"].Value);
            }

            XmlNode schema = node.SelectSingleNode("schema");

            if (schema != null)
            {
                var pid_mapping = schema.Attributes["pid_mapping"].Value;
                PidMappings = new PidMappings(pid_mapping);
            }

            XmlNode query = node.SelectSingleNode("query_term");

            if (query != null)
            {
                QueryTerm = new BM25QueryTerm(query);
            }
        }


        protected override string SelectHiddenNodes(XmlNode node)
        {
            return node.SelectSingleNode("final").Attributes["hidden_nodes_adds"].Value;
        }

    }
}