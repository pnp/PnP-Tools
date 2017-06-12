using SearchQueryTool.Helpers;
using System;
using System.Management.Automation;

namespace PSSQT
{
    public abstract class RankingFeatureHelper
    {
        // Factory method
        public static RankingFeatureHelper Create(RankingFeature rankingFeature)
        {
            RankingFeatureHelper helper = null;

            if (rankingFeature is StaticRank)
            {
                helper = new StaticRankFeatureHelper((StaticRank)rankingFeature);
            }
            else if (rankingFeature is BucketedStatic)
            {
                helper = new BucketedStaticHelper((BucketedStatic) rankingFeature);
            }
            else if (rankingFeature is BM25)
            {
                helper = new BM25Helper((BM25) rankingFeature);
            }
            else if (rankingFeature is MinSpan)
            {
                helper = new MinSpanHelper((MinSpan) rankingFeature);
            }
            else if (rankingFeature is Dynamic)
            {
                helper = new DynamicHelper((Dynamic) rankingFeature);
            }
            else
            {
                throw new Exception("Unknown Ranking Feature type: " + rankingFeature.GetType().Name);
            }

            return helper;
        }

        public virtual string Name
        {
            get;
        }

        public string Key
        {
            get
            {
                return GetType().Name + "_" + Name;
            }
        }

        public virtual double CompareValue
        {
            get
            {
                return Normalized;
            }
        }


        public virtual double RawValue
        {
            get
            {
                throw new NotImplementedException(String.Format("{0} does not implement RawValue", GetType().Name));
            }
        }

        public virtual double RawValueTransformed
        {
            get
            {
                throw new NotImplementedException(String.Format("{0} does not implement RawValueTransformed", GetType().Name));
            }
        }

        public virtual double Transformed
        {
            get
            {
                throw new NotImplementedException(String.Format("{0} does not implement Transformed", GetType().Name));
            }
        }

        public virtual double Normalized
        {
            get
            {
                throw new NotImplementedException(String.Format("{0} does not implement Normalized", GetType().Name));
            }
        }

        public virtual bool IncludeAsAttribute
        {
            get
            {
                return true;
            }
        }
    }

    public class StaticRankFeatureHelper : RankingFeatureHelper
    {
        private StaticRank staticRank;

        internal StaticRankFeatureHelper(StaticRank staticRank)
        {
            this.staticRank = staticRank;
        }

        public override string Name
        {
            get
            {
                return staticRank.Property.Name;
            }
        }

        public override double RawValue
        {
            get
            {
                return staticRank.Val;
            }
        }

        public override double RawValueTransformed
        {
            get
            {
                return staticRank.ValRT;
            }
        }

        public override double Normalized
        {
            get
            {
                return staticRank.ValN;
            }
        }

        public override double Transformed
        {
            get
            {
                return staticRank.ValT;
            }
        }
    }

    public class BucketedStaticHelper : RankingFeatureHelper
    {
        private BucketedStatic bucketedStatic;

        internal BucketedStaticHelper(BucketedStatic bucketedStatic)
        {
            this.bucketedStatic = bucketedStatic;
        }

        public override string Name
        {
            get
            {
                return bucketedStatic.Property.Name;
            }
        }

        public override double CompareValue
        {
            get
            {
                return RawValue;
            }
        }

        public override double RawValue
        {
            get
            {
                return bucketedStatic.Val;
            }
        }

        public override double RawValueTransformed
        {
            get
            {
                return bucketedStatic.ValT;
            }
        }


        public override bool IncludeAsAttribute
        {
            get
            {
                return false;
            }
        }

    }

    public class BM25Helper : RankingFeatureHelper
    {
        private BM25 bm25;

        internal BM25Helper(BM25 bm25)
        {
            this.bm25 = bm25;
        }

        public override string Name
        {
            get
            {
                return bm25.GetType().Name;
            }
        }

        public override double CompareValue
        {
            get
            {
                return RawValue;
            }
        }

        public override double RawValue
        {
            get
            {
                return bm25.Score;
            }
        }

    }

    public class MinSpanHelper : RankingFeatureHelper
    {
        private MinSpan minSpan;

        internal MinSpanHelper(MinSpan minSpan)
        {
            this.minSpan = minSpan;
        }

        public override string Name
        {
            get
            {
                return minSpan.Property.Name;
            }
        }

        public override double Normalized
        {
            get
            {
                return minSpan.Normalized;
            }
        }

        public override double RawValue
        {
            get
            {
                return minSpan.Value;
            }
        }

        public override double Transformed
        {
            get
            {
                return minSpan.Transformed;
            }
        }
    }

    public class DynamicHelper : StaticRankFeatureHelper
    {
        internal DynamicHelper(Dynamic dynamic) :
            base(dynamic)
        {
        }

    }

    public class RankingFeatureAggregateValues
    {
        public RankingFeatureAggregateValues(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public double? Min
        {
            get; private set;
        }

        public double? Max
        {
            get; private set;
        }

        public void RegisterValue(double val)
        {
            if ( !Min.HasValue || val < Min )
            {
                Min = val;
            }

            if ( !Max.HasValue || val > Max )
            {
                Max = val;
            }
        }

        public double Range
        {
            get
            {
                if (!(Max.HasValue && Min.HasValue))
                {
                    throw new RuntimeException("At least one value must be registered for Rank aggregates to be calculated.");
                }

                return Max.Value - Min.Value;
            }
        }
    }
}
