using PSSQT.RankLogParser;
using System;
using System.Management.Automation;

namespace PSSQT.ResultProcessor
{
    public class RankContributionResultProcessor : RankXMLResultProcessor
    {
        public RankContributionResultProcessor(SearchSPIndexCmdlet cmdlet) : 
            base(cmdlet)
        {
        }

        protected override void AddItemProperty(PSObject item, string key, object value)
        {
            if (key.Equals("RankDetail", StringComparison.InvariantCultureIgnoreCase))
            {
                var rlog = RankLog.CreateRankLogFromXml((string) value);

                RankLogStage stage = rlog.FinalRankStage;

                foreach (var feature in stage.Features)
                {
                    item.Properties.Add(
                        new PSVariableProperty(
                            new PSVariable(feature.Name, feature.EstimatedRankContribution())));
                }
            }
            else
            {
                base.AddItemProperty(item, key, value);
            }

        }
    }
}
