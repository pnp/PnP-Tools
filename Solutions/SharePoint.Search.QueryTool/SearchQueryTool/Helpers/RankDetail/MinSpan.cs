using System;
using System.Xml;
using SearchQueryTool.Helpers;

namespace SearchQueryTool.Helpers
{
    internal class MinSpan : RankingFeature
    {
        // Fields
        private float _fNormalized;
        private float _fTransformed;
        private float _fValue;
        private int _iBestDiffTerms;
        private int _iBestMinSpan;
        private int _iBestSpanCount;
        private int _iRarestTermCount;
        private Pid _pid;
        private string _type;

        // Methods
        public MinSpan(XmlNode xmlFeature, XmlNode xmlRankingModel, XmlNode xmlRankingModelFeature)
            : base("MinSpan")
        {
            base.SetFeatureValue(xmlFeature, xmlRankingModel, xmlRankingModelFeature);
            if (xmlFeature.Attributes["raw_value"] != null)
            {
                this._fValue = float.Parse(xmlFeature.Attributes["raw_value"].Value);
            }
            else
            {
                this._fValue = 0f;
            }
            if (xmlFeature.Attributes["transformed"] != null)
            {
                this._fTransformed = float.Parse(xmlFeature.Attributes["transformed"].Value);
            }
            else
            {
                this._fTransformed = 0f;
            }
            if (xmlFeature.Attributes["normalized"] != null)
            {
                this._fNormalized = float.Parse(xmlFeature.Attributes["normalized"].Value);
            }
            else
            {
                this._fNormalized = 0f;
            }
            if (xmlFeature.Attributes["best_min_span"] != null)
            {
                this._iBestMinSpan = int.Parse(xmlFeature.Attributes["best_min_span"].Value);
            }
            else
            {
                this._iBestMinSpan = 0;
            }
            if (xmlFeature.Attributes["best_span_count"] != null)
            {
                this._iBestSpanCount = int.Parse(xmlFeature.Attributes["best_span_count"].Value);
            }
            else
            {
                this._iBestSpanCount = 0;
            }
            if (xmlFeature.Attributes["best_diff_terms_count"] != null)
            {
                this._iBestDiffTerms = int.Parse(xmlFeature.Attributes["best_diff_terms_count"].Value);
            }
            else
            {
                this._iBestDiffTerms = 1;
            }
            if (xmlFeature.Attributes["min_term_count"] != null)
            {
                this._iRarestTermCount = int.Parse(xmlFeature.Attributes["min_term_count"].Value);
            }
            else
            {
                this._iRarestTermCount = 1;
            }
            this._type = xmlFeature.Attributes["proximity_type"].Value;
            this._pid = new Pid(int.Parse(xmlRankingModelFeature.Attributes["pid"].Value));
        }

        // Properties
        public int BestDiffTerms
        {
            get
            {
                return this._iBestDiffTerms;
            }
        }

        public int BestMinSpan
        {
            get
            {
                return this._iBestMinSpan;
            }
        }

        public int BestSpanCount
        {
            get
            {
                return this._iBestSpanCount;
            }
        }

        public float Normalized
        {
            get
            {
                return this._fNormalized;
            }
        }

        public Pid Property
        {
            get
            {
                return this._pid;
            }
        }

        public int RarestTermCount
        {
            get
            {
                return this._iRarestTermCount;
            }
        }

        public float Transformed
        {
            get
            {
                return this._fTransformed;
            }
        }

        public string Type
        {
            get
            {
                return this._type;
            }
        }

        public float Value
        {
            get
            {
                return this._fValue;
            }
        }
    }


}