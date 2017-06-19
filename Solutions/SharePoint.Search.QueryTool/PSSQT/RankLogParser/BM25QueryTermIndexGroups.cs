using System.Collections.Generic;
using System.Xml;

namespace PSSQT.RankLogParser
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    public class BM25QueryTermIndexGroups : Dictionary<string, BM25QueryTermIndexGroup>
    {
        public BM25QueryTermIndexGroups(XmlNode xmlNode)  // index node
        {
            var groups = xmlNode.SelectNodes("group");

            foreach (var group in groups)
            {
                var bm25QueryTermIndexGroup = new BM25QueryTermIndexGroup((XmlNode) group);

                Add(bm25QueryTermIndexGroup.Id, bm25QueryTermIndexGroup);
            }
        }
    }

    
}