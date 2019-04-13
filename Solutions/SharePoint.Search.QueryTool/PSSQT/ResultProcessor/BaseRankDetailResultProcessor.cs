using SearchQueryTool.Model;
using System;

namespace PSSQT.ResultProcessor
{
    public class BaseRankDetailResultProcessor : PrimaryResultsResultProcessor
    {
        protected BaseRankDetailResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {
        }

        protected override void UserNotification()
        {
            base.UserNotification();

            Cmdlet.WriteWarning("Please note that RankDetail is experimental and only an approximation.");
        }

        protected override void EnsurePropertiesPresent()
        {
            base.EnsurePropertiesPresent();
  
            Cmdlet.AddSelectProperty("WorkId");
            Cmdlet.AddSelectProperty("Rank");
            Cmdlet.AddSelectProperty("RankDetail");
        }

        public override bool HandleException(Exception ex, SearchQueryRequest searchQueryRequest)
        {
            //Cmdlet.WriteWarning(">>>" + ex.GetType().ToString());

            if (ex.Message.Contains("HTTP 500"))
            {
                Cmdlet.WriteWarning("Please note that you need Search Service Application administrative rights to use rank result processors.");
            }

            return base.HandleException(ex, searchQueryRequest);
        }

    }
}
