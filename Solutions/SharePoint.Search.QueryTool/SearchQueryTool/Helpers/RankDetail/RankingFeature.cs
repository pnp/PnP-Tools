using System.Xml;

namespace SearchQueryTool.Helpers
{
    public abstract class RankingFeature
    {
        // Fields
        private float[] _HiddenNodeWeights;
        private string _Name;
        private float _mean;
        private float _stddev;

        // Methods
        public RankingFeature(string name)
        {
            _Name = name;
        }

        // Properties
        public float[] HNWeights
        {
            get { return _HiddenNodeWeights; }
        }

        public void SetFeatureValue(XmlNode xmlFeature, XmlNode xmlRankingModel, XmlNode xmlRankingModelFeature)
        {
            if (((xmlFeature != null) && (xmlRankingModel != null)) && (xmlRankingModelFeature != null))
            {
                _HiddenNodeWeights = new float[int.Parse(xmlRankingModel.Attributes["num_hidden_nodes"].Value)];
                var separator = new[] {' '};
                XmlNode node = xmlFeature.Attributes["hidden_nodes_adds"];
                if (node != null)
                {
                    string[] strArray = node.Value.Trim().Split(separator);
                    for (int i = 0; i < _HiddenNodeWeights.Length; i++)
                    {
                        _HiddenNodeWeights[i] = float.Parse(strArray[i]);
                    }
                }
                XmlNode node2 = xmlRankingModelFeature["normalization"];
                if (node2 != null)
                {
                    _stddev = float.Parse(node2.Attributes["sdev"].Value);
                    _mean = float.Parse(node2.Attributes["mean"].Value);
                }
                else
                {
                    _stddev = 0f;
                    _mean = 0f;
                }
            }
            if (_HiddenNodeWeights == null)
            {
                _HiddenNodeWeights = new float[0];
                _stddev = 0f;
                _mean = 0f;
            }
        }
    }
}