using System;
using System.Collections.Generic;

namespace SearchQueryTool.Helpers
{
    public class ScoreDetail
    {
        // Fields
        public Guid ModelId = Guid.Empty;
        public string ModelType = string.Empty;
        public float OriginalScore;
        public List<RankingFeature> RankingFeatures = new List<RankingFeature>();
        public float Score;
    }
}