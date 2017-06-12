using System;
using System.Collections.Generic;
using System.Xml;
using static PSSQT.RankLogHelper;

namespace PSSQT.RankLogParser
{
    public class BM25QueryTermIndex : RankLogElement
    {

        public List<double> Avdl { get; private set; }
        public string n { get; private set; }
        public string N { get; private set; }
        public string Name { get; private set; }

        public BM25QueryTermIndexGroups Groups { get; private set; }


        internal BM25QueryTermIndex()
        {
        }

        internal override void Parse(XmlNode xmlNode)
        {
            Name = xmlNode.Attributes["name"].Value;
            N = xmlNode.Attributes["N"].Value;
            n = xmlNode.Attributes["n"].Value;

            var avdl_s = xmlNode.Attributes["avdl"].Value;

            Avdl = ParseDoubleList(avdl_s);
        }
    }
}