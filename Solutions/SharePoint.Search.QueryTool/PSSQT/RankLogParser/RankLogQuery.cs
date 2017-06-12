using System;
using System.Text;
using System.Xml;

namespace PSSQT.RankLogParser
{
    public class RankLogQuery : RankLogElement
    {
        public string Tree { get; set; }
        public string Properties { get; set; }

        internal RankLogQuery()
        {

        }

        internal override void Parse(XmlNode node)
        {
            Tree = node.Attributes["tree"].Value;
            Properties = node.Attributes["properties"].Value;
        }

        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(base.ToString());

            RankLogHelper.Append(sb, "Tree", Tree, 1);
            RankLogHelper.Append(sb, "Properties", Properties, 1);

            return sb.ToString();
        }
    }
}