using System.Xml;

namespace PSSQT.RankLogParser
{
    public class BM25QueryTerm
    {
        public BM25QueryTerm(XmlNode query)
        {
            Term = query.Attributes["term"].Value;

            var bm25QueryTermIndex = new BM25QueryTermIndex();

            bm25QueryTermIndex.Parse(query.SelectSingleNode("index"));

            BM25QueryTermIndex = bm25QueryTermIndex;
        }

        public string Term { get; private set; }
        public BM25QueryTermIndex BM25QueryTermIndex { get; private set; }
    }
}