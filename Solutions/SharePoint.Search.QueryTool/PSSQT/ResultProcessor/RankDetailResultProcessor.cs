using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace PSSQT.ResultProcessor
{
    public class RankDetailResultProcessor : RankXMLResultProcessor
    {
        public RankDetailResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {
        }

        protected override void AddItemProperty(PSObject item, string key, object value)
        {
            if (key.Equals("RankDetail", StringComparison.InvariantCultureIgnoreCase))
            {
                var rlogparser = new SearchQueryTool.Helpers.RankLogParser((string) value);

                List<string> rankFeatures = new List<string>();

                int n = rlogparser.ScoreDetails.Length;

                // only look at last stage

                if (n > 0)
                {
                    var scoreDetail = rlogparser.ScoreDetails[n - 1];

                    foreach (var rankFeature in scoreDetail.RankingFeatures)
                    {
                        IRankingFeatureFormatter formatter = RankingFeatureFormatterFactory.SelectFormatter(rankFeature);

                        rankFeatures.Add(formatter.Format());
                    }
                }

                base.AddItemProperty(item, key, String.Join(",", rankFeatures));
            }
            else
            {
                base.AddItemProperty(item, key, value);
            }
        }
    }
}
