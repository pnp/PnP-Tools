using System.Collections.Generic;
using System.Xml;

namespace SearchQueryTool.Helpers
{
    internal class BM25 : RankingFeature
    {
        // Fields
        private readonly float _Score;
        private readonly List<QueryTerm> _Terms;
        private readonly long _iExternalDocId;
        private readonly long _iInternalDocId;

        // Methods
        public BM25(XmlNode xmlBM25, XmlNode xmlRankingModel, XmlNode xmlRankingModelBM25) : base("BM25")
        {
            var dictionary = new Dictionary<int, PropertyContext>();
            XmlNodeList list = xmlBM25.SelectNodes("query_term");
            XmlNode node = xmlBM25["schema"];
            XmlNode xmlFeature = xmlBM25["final"];
            string str = node.Attributes["pid_mapping"].Value;
            var separator = new[] {'[', ']', ':'};
            string[] strArray = str.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i].Trim().Length != 0)
                {
                    int num2 = int.Parse(strArray[i]);
                    i += 3;
                    int key = int.Parse(strArray[i]);
                    i++;
                    string strUpdateGroup = strArray[i];
                    if (!dictionary.ContainsKey(key))
                    {
                        dictionary.Add(key, new PropertyContext(key, num2, strUpdateGroup));
                    }
                    else if (dictionary[key].Pid > num2)
                    {
                        dictionary.Remove(key);
                        dictionary.Add(key, new PropertyContext(key, num2, strUpdateGroup));
                    }
                }
            }
            base.SetFeatureValue(xmlFeature, xmlRankingModel, xmlRankingModelBM25);
            var dictionary2 = new Dictionary<int, Bm25PropertyWeights>();
            foreach (XmlNode node3 in xmlRankingModelBM25.SelectNodes("property"))
            {
                int num4 = int.Parse(node3.Attributes["pid"].Value);
                float w = float.Parse(node3.Attributes["w"].Value);
                float b = float.Parse(node3.Attributes["b"].Value);
                dictionary2.Add(num4, new Bm25PropertyWeights(w, b));
            }
            if (list.Count == 0)
            {
                base.SetFeatureValue(null, null, null);
            }
            _Score = float.Parse(xmlBM25["final"].Attributes["score"].Value);
            _Terms = new List<QueryTerm>();
            foreach (XmlNode node4 in list)
            {
                XmlNode node5 = node4["rank"];
                string strTerm = node4.Attributes["term"].Value;
                float score = float.Parse(node5.Attributes["score"].Value);
                XmlNode node6 = node4["index"];
                if (node6 == null) continue;
                int n = int.Parse(node6.Attributes["n"].Value);
                int num9 = int.Parse(node6.Attributes["N"].Value);
                float rW = float.Parse(node5.Attributes["term_weight"].Value);
                var item = new QueryTerm(strTerm, score, n, num9, rW);
                Terms.Add(item);
                var chArray2 = new[] {' '};
                float num11 = 0f;
                string[] strArray2 = node6.Attributes["avdl"].Value.Trim().Split(chArray2);
                foreach (XmlNode node7 in node6.SelectNodes("group"))
                {
                    if (node7.Attributes["ext_doc_id"] != null)
                    {
                        _iExternalDocId = long.Parse(node7.Attributes["ext_doc_id"].Value);
                    }
                    if (node7.Attributes["int_doc_id"] != null)
                    {
                        _iInternalDocId = long.Parse(node7.Attributes["int_doc_id"].Value);
                    }
                    if (node7.Attributes["tf_prime"] != null)
                    {
                        float num12 = float.Parse(node7.Attributes["tf_prime"].Value);
                        string[] strArray3 = node7.Attributes["tf"].Value.Trim().Split(chArray2);
                        string[] strArray4 = node7.Attributes["dl"].Value.Trim().Split(chArray2);
                        num11 += num12;
                        foreach (int num13 in dictionary.Keys)
                        {
                            float tf = float.Parse(strArray3[num13]);
                            if (tf != 0f)
                            {
                                int num15 = dictionary[num13].Pid;
                                float weight = dictionary2[num15].W;
                                float tfw = tf*weight;
                                float dl = float.Parse(strArray4[num13]);
                                float avdl = float.Parse(strArray2[num13]);
                                float dlavdl = dl/avdl;
                                float num21 = dictionary2[num15].B;
                                float dlnorm = (1f - num21) + (num21*dlavdl);
                                float tfnorm = tf/dlnorm;
                                var pid = new Pid(num15, weight, tf, tfw, dl, avdl, dlavdl, dlnorm, tfnorm, num21);
                                item.AddPid(pid);
                            }
                        }
                    }
                }
                item.TFW = num11;
            }
        }

        // Properties
        public long ExternalDocId
        {
            get { return _iExternalDocId; }
        }

        public long InternalDocId
        {
            get { return _iInternalDocId; }
        }

        public float Score
        {
            get { return _Score; }
        }

        public List<QueryTerm> Terms
        {
            get { return _Terms; }
        }
    }
}