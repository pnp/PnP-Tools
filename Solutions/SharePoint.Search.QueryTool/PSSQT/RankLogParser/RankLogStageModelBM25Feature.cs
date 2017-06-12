using System;
using System.Collections.Generic;
using System.Xml;

namespace PSSQT.RankLogParser
{
    public class RankLogStageModelBM25Feature : RankLogElement
    {
        // only parse attributes common in all proximity features
        public List<BM25FeatureProperty> Properties { get; set; } = new List<BM25FeatureProperty>();

        internal RankLogStageModelBM25Feature()
        {
        }

        internal override void Parse(XmlNode node)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name.Equals("property", StringComparison.CurrentCultureIgnoreCase))
                {
                    var property = new BM25FeatureProperty();

                    property.Pid = int.Parse(childNode.Attributes["pid"].Value);
                    property.Weight = double.Parse(childNode.Attributes["w"].Value);
                    property.Bias = double.Parse(childNode.Attributes["b"].Value);

                    Properties.Add(property);
                }
            }
        }

        public BM25FeatureProperty GetPropertyByPid(int pid)
        {
            foreach (var property in Properties)
            {
                if (property.Pid == pid)
                {
                    return property;
                }
            }

            throw new Exception(String.Format("Property with pid=\"{0}\" not found.", pid));
        }

    }
}
