using System;
using System.Xml;

namespace SearchQueryTool.Helpers
{
    internal class BucketedStatic : RankingFeature
{
    // Fields
    private Pid _pid;
    private int _val;
    private float _valT;

    // Methods
    public BucketedStatic(XmlNode xmlFeature, XmlNode xmlRankingModel, XmlNode xmlRankingModelFeature) : base("Bucketed Static")
    {
        base.SetFeatureValue(xmlFeature, xmlRankingModel, xmlRankingModelFeature);
        this._pid = new Pid(int.Parse(xmlRankingModelFeature.Attributes["pid"].Value));
        this._val = int.Parse(xmlFeature.Attributes["raw_value"].Value);
        this._valT = float.Parse(xmlFeature.Attributes["raw_value_transformed"].InnerText);
    }

    // Properties
    public Pid Property
    {
        get
        {
            return this._pid;
        }
    }

    public int Val
    {
        get
        {
            return this._val;
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