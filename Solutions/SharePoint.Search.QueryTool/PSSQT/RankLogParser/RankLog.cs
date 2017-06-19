using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PSSQT.RankLogParser
{
    class RankLog : RankLogElement
    {
        public string Version { get; set; }
        public Guid ModelID { get; set; }
        public string InstanceId { get; set; }
        public Guid CorrId { get; set; }

        public List<RankLogElement> Elements { get; set; }

        public RankLogStage FinalRankStage {
            get
            {
                RankLogStage finalStage = null;

                foreach (var element in Elements)
                {
                    if (element is RankLogStage)
                    {
                        finalStage = (RankLogStage)element;
                    }
                }

                if (finalStage == null)
                {
                    throw new Exception("Could not find any stages in the rank log.");
                }
                return finalStage;
            }

        }

        internal RankLog()
        {
        }

        internal override void Parse(XmlNode node)
        {
            // RankLog Header
            ModelID = Guid.Parse(node.Attributes["id"].Value);
            CorrId = Guid.Parse(node.Attributes["corr_id"].Value);
            InstanceId = node.Attributes["instance_id"].Value;

            // RankLog Elements
            Elements = new List<RankLogElement>();

            foreach (XmlNode childNode in node.ChildNodes)
            {
                RankLogElement element = CreateChildElement(childNode);

                element.Parse(childNode);

                Elements.Add(element);
            }
        }

        public static RankLog CreateRankLogFromXml(string ranklogXML)
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;


            var document = new XmlDocument();
            var settings = new XmlReaderSettings
            {
                CloseInput = true,
                XmlResolver = null,
                DtdProcessing = DtdProcessing.Prohibit
            };

            using (XmlReader reader = XmlReader.Create(new StringReader(ranklogXML), settings))
            {
                document.Load(reader);
            }

            var rankLog = new RankLog();

            rankLog.Parse(document.DocumentElement);

            return rankLog;
        }

        
   
        protected override RankLogElement CreateChildElement(XmlNode node)
        {
            if (node.Name.Equals("query",StringComparison.CurrentCultureIgnoreCase))
            {
                return new RankLogQuery();
            }
            else if (node.Name.Equals("stage", StringComparison.CurrentCultureIgnoreCase))
            {
                return new RankLogStage();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}

