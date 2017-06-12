using System.Collections.Generic;
using System.Xml;
using static PSSQT.RankLogHelper;

namespace PSSQT.RankLogParser
{
    public class BM25QueryTermIndexGroup
    {
        public BM25QueryTermIndexGroup(XmlNode xmlNode)
        {
            Id = xmlNode.Attributes["id"].Value;
            ExtDocId = int.Parse(xmlNode.Attributes["ext_doc_id"].Value);
            IntDocId = int.Parse(xmlNode.Attributes["int_doc_id"].Value);

            var tf = xmlNode.Attributes["tf"].Value;
            TermFrequency = ParseIntList(tf);
        }

        public string Id { get; private set; }
        public int ExtDocId { get; set; }
        public int IntDocId { get; set; }
        public List<int> TermFrequency { get; set; }
    }
}