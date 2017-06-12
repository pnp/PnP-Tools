using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSSQT
{
    public enum RankModelName
    {
        DefaultSearchModel,
        CatalogRankingModel,
        RecommenderRankingModel,
        PeopleSearchExpertiseSocialDistanceRankingModel,
        SiteSuggestionRankingModel,
        SearchModelWithBoostedMinspan,
        PeopleSearchSocialDistanceModel,
        PeopleSearchExpertiseRankingModel,
        SearchRankingModelWithTwoLinearStages,
        PeopleSearchNameSocialDistanceRankingModel,
        PeopleSearchNameRankingModel,
        PopularityRankingModel,
        O15DefaultSearchModel,
        O14DefaultSearchModel,
        PeopleSearchApplicationRankingModel,
        SearchModelWithoutMinspan
    }

    public static class RankModel
    {
        public static string Select(RankModelName name)
        {
            switch (name)
            {
                case RankModelName.DefaultSearchModel: return "8f6fd0bc-06f9-43cf-bbab-08c377e083f4";
                case RankModelName.CatalogRankingModel: return "ca2a10ff-6f63-4913-a125-34b5894495f4";
                case RankModelName.RecommenderRankingModel: return "b63ad0fd-3ab4-490d-b556-379dc4a52422";
                case RankModelName.PeopleSearchExpertiseSocialDistanceRankingModel: return "02b657fe-924f-4b2b-bb91-4a12baf9929a";
                case RankModelName.SiteSuggestionRankingModel: return "9e09f192-e036-422f-ad3b-534574e8f894";
                case RankModelName.SearchModelWithBoostedMinspan: return "97cbcebd-037c-4346-9bc4-582d8c560204";
                case RankModelName.PeopleSearchSocialDistanceModel: return "4790b250-e2af-4e4a-8136-600739ee3163";
                case RankModelName.PeopleSearchExpertiseRankingModel: return "c8bdd081-7379-4c71-aac8-61b6aa6e25a6";
                case RankModelName.SearchRankingModelWithTwoLinearStages: return "5e9ee87d-4a68-420a-9d58-8913beeaa6f2";
                case RankModelName.PeopleSearchNameSocialDistanceRankingModel: return "5df7ba10-55d6-4da1-b55f-896f7bcb486b";
                case RankModelName.PeopleSearchNameRankingModel: return "0bba4d7d-4f2c-4086-975a-8f9d2b6c6d53";
                case RankModelName.PopularityRankingModel: return "d4ac6500-d1d0-48aa-86d4-8fe9a57a74af";
                case RankModelName.O15DefaultSearchModel: return "9b911c3e-78e1-4b99-9b1f-a69d3691bdd1";
                case RankModelName.O14DefaultSearchModel: return "9399df62-f089-4033-bdc5-a7ea22936e8e";
                case RankModelName.PeopleSearchApplicationRankingModel: return "d9bfb1a1-9036-4627-83b2-bbd9983ac8a1";
                case RankModelName.SearchModelWithoutMinspan: return "df3c3c51-b41f-4cbc-9b1a-c3b0ed40d4f0";

                default: throw new ArgumentException(String.Format("Unknown ranking model name: {0}", Enum.GetName(typeof(RankModelName), name)));
            }
        }
    }

}
