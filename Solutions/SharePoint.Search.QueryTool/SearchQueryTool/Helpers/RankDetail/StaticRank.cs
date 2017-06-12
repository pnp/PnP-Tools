using System;
using System.Xml;

namespace SearchQueryTool.Helpers
{
    internal class StaticRank : RankingFeature
{
    // Fields
    protected Pid _pid;
    protected float _val;
    protected float _valN;
    protected double _valRT;
    protected float _valT;

    // Methods
    public StaticRank(XmlNode xmlFeature, XmlNode xmlRankingModel, XmlNode xmlRankingModelFeature) : base("Static")
    {
        base.SetFeatureValue(xmlFeature, xmlRankingModel, xmlRankingModelFeature);
        if (xmlRankingModelFeature.Attributes["pid"] != null)
        {
            this._pid = new Pid(int.Parse(xmlRankingModelFeature.Attributes["pid"].Value));
        }
        else if (xmlFeature.Attributes["property_name"] != null)
        {
            this._pid = new Pid(xmlFeature.Attributes["property_name"].Value);
        }
        foreach (XmlAttribute attribute in xmlFeature.Attributes)
        {
            string name = attribute.Name;
            if (name != null)
            {
                if (!(name == "raw_value"))
                {
                    if (name == "transformed")
                    {
                        goto Label_00F2;
                    }
                    if (name == "normalized")
                    {
                        goto Label_0105;
                    }
                    if (name == "raw_value_transformed")
                    {
                        goto Label_0118;
                    }
                }
                else
                {
                    this._val = float.Parse(attribute.Value);
                }
            }
            continue;
        Label_00F2:
            this._valT = float.Parse(attribute.Value);
            continue;
        Label_0105:
            this._valN = float.Parse(attribute.Value);
            continue;
        Label_0118:
            this._valRT = double.Parse(attribute.Value);
        }
    }

    // Properties
    public Pid Property
    {
        get
        {
            return this._pid;
        }
    }

    public float Val
    {
        get
        {
            return this._val;
        }
    }

    public float ValN
    {
        get
        {
            return this._valN;
        }
    }

    public double ValRT
    {
        get
        {
            return this._valRT;
        }
    }

    public float ValT
    {
        get
        {
            return this._valT;
        }
    }
}

}