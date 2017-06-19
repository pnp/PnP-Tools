using System;
using System.Xml;

namespace SearchQueryTool.Helpers
{
    internal class Dynamic : StaticRank
    {
        // Methods
        public Dynamic(XmlNode xmlFeature, XmlNode xmlRankingModel, XmlNode xmlRankingModelFeature)
            : base(xmlFeature, xmlRankingModel, xmlRankingModelFeature)
        {
            foreach (XmlNode node in xmlFeature.ChildNodes)
            {
                if (node.Name == "property_name")
                {
                    foreach (XmlNode node2 in node.ChildNodes)
                    {
                        if (node2.Name == "property_value")
                        {
                            foreach (XmlAttribute attribute in node2.Attributes)
                            {
                                string name = attribute.Name;
                                if (name != null)
                                {
                                    if (!(name == "raw_value"))
                                    {
                                        if (name == "transformed")
                                        {
                                            goto Label_00D5;
                                        }
                                        if (name == "normalized")
                                        {
                                            goto Label_00E8;
                                        }
                                    }
                                    else
                                    {
                                        base._val = float.Parse(attribute.Value);
                                    }
                                }
                                continue;
                            Label_00D5:
                                base._valT = float.Parse(attribute.Value);
                                continue;
                            Label_00E8:
                                base._valN = float.Parse(attribute.Value);
                            }
                        }
                    }
                }
            }
        }
    }

}