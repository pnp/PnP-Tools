using SearchQueryTool.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSSQT
{
    public interface IRankingFeatureFormatter
    {
        string Format();
    }

    public static class RankingFeatureFormatterFactory
    {

        public static IRankingFeatureFormatter SelectFormatter(RankingFeature rankFeature)
        {
            if (rankFeature is BM25)
            {
                return new BM25Formatter((BM25)rankFeature);
            }
            else if (rankFeature is BucketedStatic)
            {
                return new BucketedStaticFormatter((BucketedStatic)rankFeature);
            }
            else if (rankFeature is Dynamic)
            {
                return new DynamicFeatureFormatter((Dynamic)rankFeature);
            }
            else if (rankFeature is StaticRank)
            {
                return new StaticRankFormatter((StaticRank)rankFeature);
            }
            else if (rankFeature is MinSpan)
            {
                return new MinSpanRankFormatter((MinSpan)rankFeature);
            }
            else
            {
                return new NullRankingFeatureFormatter();
            }
        }

    }

    internal class MinSpanRankFormatter : IRankingFeatureFormatter
    {
        private MinSpan rankFeature;

        public MinSpanRankFormatter(MinSpan rankFeature)
        {
            this.rankFeature = rankFeature;
        }

        public string Format()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(String.Format("{0}({1}:{2})", rankFeature.Type, rankFeature.Property.Name, rankFeature.Value));

            return sb.ToString();
        }
    }

    internal class NullRankingFeatureFormatter : IRankingFeatureFormatter
    {
        public string Format()
        {
            return String.Empty;
        }
    }

    internal class DynamicFeatureFormatter : IRankingFeatureFormatter
    {
        private Dynamic rankFeature;

        public DynamicFeatureFormatter(Dynamic rankFeature)
        {
            this.rankFeature = rankFeature;
        }

        public string Format()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(String.Format("{0}:{1}", rankFeature.Property.Name, rankFeature.Val));

            return sb.ToString();
        }
    }

    internal class BucketedStaticFormatter : IRankingFeatureFormatter
    {
        private BucketedStatic rankFeature;

        public BucketedStaticFormatter(BucketedStatic rankFeature)
        {
            this.rankFeature = rankFeature;
        }

        public string Format()
        {
            StringBuilder sb = new StringBuilder();

            if (rankFeature.Property.Name == "InternalFileType")
            {
                sb.Append(String.Format("{0}:{1}", rankFeature.Property.Name, InternalFileTypeName(rankFeature.Val)));
            }
            else
            {
                sb.Append(String.Format("{0}:{1}", rankFeature.Property.Name, rankFeature.Val));
            }

            return sb.ToString();
        }

        public static string InternalFileTypeName(int value)
        {
            string text = String.Empty;

            switch (value)
            {
                case 0:
                    text = "Undefined/html";
                    break;
                case 1:
                    text = "Word Doc";
                    break;
                case 2:
                    text = "Power Point";
                    break;
                case 3:
                    text = "Excel";
                    break;
                case 4:
                    text = "Xml";
                    break;
                case 5:
                    text = "Text";
                    break;
                case 6:
                    text = "List Item";
                    break;
                case 7:
                    text = "Email";
                    break;
                case 8:
                    text = "Image";
                    break;
                case 9:
                    text = "Person";
                    break;
                case 10:
                    text = "Video";
                    break;
                case 11:
                    text = "SharePoint Site";
                    break;
                case 12:
                    text = "OneNote";
                    break;
                case 13:
                    text = "Publisher";
                    break;
                case 14:
                    text = "Visio";
                    break;
                case 15:
                    text = "PDF";
                    break;
                case 0x10:
                    text = "Webpage";
                    break;
                case 0x11:
                    text = "Zip";
                    break;
                case 0x12:
                    text = "Blog";
                    break;
                case 0x13:
                    text = "Discussion Board";
                    break;
                case 20:
                    text = "Document Library";
                    break;
                case 0x15:
                    text = "List";
                    break;
                case 0x16:
                    text = "Picture Library";
                    break;
                case 0x17:
                    text = "Picture Library Item";
                    break;
                case 0x18:
                    text = "Survey";
                    break;
                case 0x19:
                    text = "Wiki";
                    break;
                case 0x1a:
                    text = "Access";
                    break;
                case 0x1b:
                    text = "MicroBlog Post";
                    break;
                case 0x1c:
                    text = "Community";
                    break;
                case 0x1d:
                    text = "Discussion";
                    break;
                case 30:
                    text = "Reply";
                    break;
            }

            return text;
        }
    }

    internal class StaticRankFormatter : IRankingFeatureFormatter
    {
        private StaticRank rankFeature;

        public StaticRankFormatter(StaticRank rankFeature)
        {
            this.rankFeature = rankFeature;
        }

        public string Format()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(String.Format("{0}:{1}", rankFeature.Property.Name, rankFeature.Val));

            return sb.ToString();
        }
    }

    internal class BM25Formatter : IRankingFeatureFormatter
    {
        internal BM25 BM25Feature { get; private set; }

        public BM25Formatter(BM25 bm25)
        {
            BM25Feature = bm25;
        }

        public string Format()
        {
            StringBuilder sb = new StringBuilder();

            foreach (QueryTerm term in BM25Feature.Terms)
            {
                sb.Append(term.TermName);
                sb.Append(" HITS(");

                List<string> pids = new List<string>();

                foreach (Pid pid in term.Pids)
                {
                    pids.Add(String.Format("{0}", pid.Name));
                }
                sb.Append(String.Join(",", pids));
                sb.Append(")");

                sb.Append(",BM25:");
                sb.Append(term.Score);
            }

            return sb.ToString();
        }

    }

}
