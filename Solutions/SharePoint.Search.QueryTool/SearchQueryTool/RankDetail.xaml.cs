using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows;
using SearchQueryTool.Helpers;
using SearchQueryTool.Helpers.RankDetail;

namespace SearchQueryTool
{
    /// <summary>
    ///     Interaction logic for RankDetail.xaml
    /// </summary>
    public partial class RankDetail : Window
    {
        #region template
        private string html = @"
<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"" >
<HTML>
    <HEAD>
        <title>MOSS 2012 - RankLog</title>
        <meta name=""GENERATOR"" Content=""Microsoft Visual Studio .NET 7.1"">
        <meta name=""CODE_LANGUAGE"" Content=""C#"">
        <meta name=""vs_defaultClientScript"" content=""JavaScript"">
        <meta name=""vs_targetSchema"" content=""http://schemas.microsoft.com/intellisense/ie5"">
        <meta charset=""UTF-8"">
        <STYLE>BODY { FONT-FAMILY: segoe ui light,segoe,arial,sans-serif }
	TD { FONT-FAMILY: segoe ui light,segoe,arial,sans-serif }
	DIV { FONT-FAMILY: segoe ui light,segoe,arial,sans-serif }
	.p { FONT-FAMILY: segoe ui light,segoe,arial,sans-serif }
	A { FONT-FAMILY: segoe ui light,segoe,arial,sans-serif }
	DIV { COLOR: #000 }
	TD { COLOR: #000 }
	.f { COLOR: #6f6f6f }
	.fl:link { COLOR: #6f6f6f }
	A:link { COLOR: #00c }
	.w { COLOR: #00c }
	A.w:link { COLOR: #00c }
	.w A:link { COLOR: #00c }
	A:visited { COLOR: #551a8b }
	.fl:visited { COLOR: #551a8b }
	A:active { COLOR: #f00 }
	.fl:active { COLOR: #f00 }
	.t A:link { COLOR: #ffffff }
	.t A:active { COLOR: #ffffff }
	.t A:visited { COLOR: #ffffff }
	.t { COLOR: #ffffff }
	.t { BACKGROUND-COLOR: #F2AB26 }
	.h { COLOR: #F2AB26 }
	.i { COLOR: #a90a08 }
	.i:link { COLOR: #a90a08 }
	.a { COLOR: #008000 }
	.a:link { COLOR: #008000 }
	.z { DISPLAY: none }
	DIV.n { MARGIN-TOP: 1ex }
	.n A { FONT-SIZE: 10pt; COLOR: #000 }
	.n .i { FONT-WEIGHT: bold; FONT-SIZE: 10pt }
	.q A:visited { COLOR: #00c; TEXT-DECORATION: none }
	.q A:link { COLOR: #00c; TEXT-DECORATION: none }
	.q A:active { COLOR: #00c; TEXT-DECORATION: none }
	.q { COLOR: #00c; TEXT-DECORATION: none }
	.b { FONT-WEIGHT: bold; FONT-SIZE: 12pt; COLOR: #00c }
	.ch { CURSOR: hand }
	.e { MARGIN-TOP: 0.75em; MARGIN-BOTTOM: 0.75em }
	.g { MARGIN-TOP: 1em; MARGIN-BOTTOM: 1em }
    .header { FONT-SIZE: 1.2em; FONT-WEIGHT: bold; }
        </STYLE>
    </HEAD>
    <body bgColor=""#ffffff"">
        <form id=""Form1"" method=""post"" runat=""server"">
            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                <TBODY>
                    <TR>
                        <TD noWrap bgColor=""#F2AB26"">
                            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                                <TBODY>
                                    <TR>
                                        <TD noWrap bgColor=""#F2AB26""><FONT id=""SearchDescription"" color=""#ffffff"" size=""-1"" runat=""server"" class=""header"">
                                            Statistics for the terms&nbsp;</FONT>
                                        </TD>
                                        <TD noWrap align=""right"" bgColor=""#F2AB26""><FONT color=""#ffffff"" size=""-1""></FONT></TD>
                                    </TR>
                                </TBODY>
                            </TABLE>
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""0"" border=""0"">
                <TBODY>
                    <TR>
                        <TD vAlign=""middle"">
                            {Values1}
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                <TBODY>
                    <TR>
                        <TD noWrap bgColor=""#F2AB26"">
                            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                                <TBODY>
                                    <TR>
                                        <TD noWrap bgColor=""#F2AB26""><FONT id=""Font1"" color=""#ffffff"" size=""-1"" runat=""server"" class=""header""> 
                                                Hits in the document&nbsp; </FONT>
                                        </TD>
                                        <TD noWrap align=""right"" bgColor=""#F2AB26""><FONT color=""#ffffff"" size=""-1""></FONT></TD>
                                    </TR>
                                </TBODY>
                            </TABLE>
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""0"" border=""0"">
                <TBODY>
                    <TR>
                        <TD vAlign=""middle"">
                            {DocHits1}
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                <TBODY>
                    <TR>
                        <TD noWrap bgColor=""#F2AB26"">
                            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                                <TBODY>
                                    <TR>
                                        <TD noWrap bgColor=""#F2AB26""><FONT id=""Font2"" color=""#ffffff"" size=""-1"" runat=""server"" class=""header""> 
                                                Document Properties and Statistics&nbsp; </FONT>
                                        </TD>
                                        <TD noWrap align=""right"" bgColor=""#F2AB26""><FONT color=""#ffffff"" size=""-1""></FONT></TD>
                                    </TR>
                                </TBODY>
                            </TABLE>
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""0"" border=""0"">
                <TBODY>
                    <TR>
                        <TD vAlign=""middle"">
                            {DocProps1}
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                <TBODY>
                    <TR>
                        <TD noWrap bgColor=""#F2AB26"">
                            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                                <TBODY>
                                    <TR>
                                        <TD noWrap bgColor=""#F2AB26""><FONT id=""Font4"" color=""#ffffff"" size=""-1"" runat=""server"" class=""header""> 
                                                Ranking Features&nbsp; </FONT>
                                        </TD>
                                        <TD noWrap align=""right"" bgColor=""#F2AB26""><FONT color=""#ffffff"" size=""-1""></FONT></TD>
                                    </TR>
                                </TBODY>
                            </TABLE>
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""0"" border=""0"">
                <TBODY>
                    <TR>
                        <TD vAlign=""middle"">
                            {RankingFeatures1}
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                <TBODY>
                    <TR>
                        <TD noWrap bgColor=""#F2AB26"">
                            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                                <TBODY>
                                    <TR>
                                        <TD noWrap bgColor=""#F2AB26""><FONT id=""FONT5"" color=""#ffffff"" size=""-1"" runat=""server"" class=""header"">
                                                Statistics for the terms&nbsp; </FONT>
                                        </TD>
                                        <TD noWrap align=""right"" bgColor=""#F2AB26""><FONT color=""#ffffff"" size=""-1""></FONT></TD>
                                    </TR>
                                </TBODY>
                            </TABLE>
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""0"" border=""0"">
                <TBODY>
                    <TR>
                        <TD vAlign=""middle"">
                            {Values2}
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                <TBODY>
                    <TR>
                        <TD noWrap bgColor=""#F2AB26"">
                            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                                <TBODY>
                                    <TR>
                                        <TD noWrap bgColor=""#F2AB26""><FONT id=""Font6"" color=""#ffffff"" size=""-1"" runat=""server"" class=""header""> 
                                                Hits in the document&nbsp; </FONT>
                                        </TD>
                                        <TD noWrap align=""right"" bgColor=""#F2AB26""><FONT color=""#ffffff"" size=""-1""></FONT></TD>
                                    </TR>
                                </TBODY>
                            </TABLE>
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""0"" border=""0"">
                <TBODY>
                    <TR>
                        <TD vAlign=""middle"">
                            {DocHits2}
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                <TBODY>
                    <TR>
                        <TD noWrap bgColor=""#F2AB26"">
                            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                                <TBODY>
                                    <TR>
                                        <TD noWrap bgColor=""#F2AB26""><FONT id=""Font7"" color=""#ffffff"" size=""-1"" runat=""server"" class=""header""> 
                                                Document Properties and Statistics&nbsp; </FONT>
                                        </TD>
                                        <TD noWrap align=""right"" bgColor=""#F2AB26""><FONT color=""#ffffff"" size=""-1""></FONT></TD>
                                    </TR>
                                </TBODY>
                            </TABLE>
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""0"" border=""0"">
                <TBODY>
                    <TR>
                        <TD vAlign=""middle"">
                            {DocProps2}
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                <TBODY>
                    <TR>
                        <TD noWrap bgColor=""#F2AB26"">
                            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                                <TBODY>
                                    <TR>
                                        <TD noWrap bgColor=""#F2AB26""><FONT id=""Font8"" color=""#ffffff"" size=""-1"" runat=""server"" class=""header""> 
                                                Ranking Features&nbsp; </FONT>
                                        </TD>
                                        <TD noWrap align=""right"" bgColor=""#F2AB26""><FONT color=""#ffffff"" size=""-1""></FONT></TD>
                                    </TR>
                                </TBODY>
                            </TABLE>
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""0"" border=""0"">
                <TBODY>
                    <TR>
                        <TD vAlign=""middle"">
                            {RankingFeatures2}
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                <TBODY>
                    <TR>
                        <TD noWrap bgColor=""#F2AB26"">
                            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                                <TBODY>
                                    <TR>
                                        <TD noWrap bgColor=""#F2AB26""><FONT id=""Font10"" color=""#ffffff"" size=""-1"" runat=""server"" class=""header""> 
                                                Ranklog XML&nbsp; </FONT>
                                        </TD>
                                        <TD noWrap align=""right"" bgColor=""#F2AB26""><FONT color=""#ffffff"" size=""-1""></FONT></TD>
                                    </TR>
                                </TBODY>
                            </TABLE>
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""0"" border=""0"">
                <TBODY>
                    <TR>
                        <TD vAlign=""middle"">
<PRE>{RANKLOGXML}</PRE>
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                <TBODY>
                    <TR>
                        <TD noWrap bgColor=""#F2AB26"">
                            <TABLE cellSpacing=""0"" cellPadding=""1"" width=""100%"" bgColor=""#F2AB26"" border=""0"">
                                <TBODY>
                                    <TR>
                                        <TD noWrap bgColor=""#F2AB26""><FONT id=""Font3"" color=""#ffffff"" size=""-1"" runat=""server"" class=""header""> 
                                                Explanation of the variables&nbsp; </FONT>
                                        </TD>
                                        <TD noWrap align=""right"" bgColor=""#F2AB26""><FONT color=""#ffffff"" size=""-1""></FONT></TD>
                                    </TR>
                                </TBODY>
                            </TABLE>
                        </TD>
                    </TR>
                </TBODY>
            </TABLE>
            <TABLE cellSpacing=""0"" cellPadding=""3"" width=""100%"" border=""0"" style=""border-width:0px;"">
                <TBODY>
                    <TR>
                        <TD class=""q"">N</TD>
                        <TD>Total number of documents in the collection</TD>
                    </TR>
                    <TR>
                        <TD class=""q"">n</TD>
                        <TD>Document Frequency - number of documents containing the term</TD>
                    </TR>
                    <TR>
                        <TD class=""q"">BM25 Weight</TD>
                        <TD>log(N/n) - this is the global okapi weight for the term</TD>
                    </TR>
                    <TR>
                        <TD class=""q"">Weighted TF</TD>
                        <TD>Weighted sum of term frequencies in each property for the given term</TD>
                    </TR>
                    <TR>
                        <TD class=""q"">Normalized TF</TD>
                        <TD>Term frequency normalized by length TFW/((1-b)+b*dl)/avdl</TD>
                    </TR>
                    <TR>
                        <TD class=""q"">term Factor</TD>
                        <TD>Term frequency squashed tfw/(k1+tfw)</TD>
                    </TR>
                    <TR>
                        <TD class=""q"">Term Score</TD>
                        <TD>BM25 score of the term, this includes only query dependent components</TD>
                    </TR>
                    <TR>
                        <TD class=""q"">Original TF</TD>
                        <TD>Term frequency before weighting is applied - number of times the term occurs in the property</TD>
                    </TR>
                    <TR>
                        <TD class=""q"">Length</TD>
                        <TD>Number of terms in the property</TD>
                    </TR>
                    <TR>
                        <TD class=""q"">Doc Length</TD>
                        <TD>Number of terms in the document across all properties</TD>
                    </TR>
                    <TR>
                        <TD class=""q"">AVDL</TD>
                        <TD>Average number of terms in the property</TD>
                    </TR>
                    <TR>
                        <TD class=""q"">Doc AVDL</TD>
                        <TD>Average number of terms in the document</TD>
                    </TR>
                    <TR>
                        <TD class=""q"">DL/AVDL</TD>
                        <TD>Ratio between DL and AVDL</TD>
                    </TR>
                    <TR>
                        <TD class=""q"">DL Factor</TD>
                        <TD>Factor used to normalize TF - (1-b)+b(dl/avdl)</TD>
                    </TR>
                    <TR>
                        <TD class=""q"">Click Distance</TD>
                        <TD>Shortest path in the web graph from the central authority to this URL</TD>
                    </TR>
                    <TR>
                        <TD class=""q"">URL Depth</TD>
                        <TD>Number of slashes in the URL</TD>
                    </TR>
                </TBODY>
            </TABLE>
        </form>
    </body>
</HTML>";
        #endregion
        public RankDetail()
        {
            InitializeComponent();
            Loaded += RankDetail_Loaded;
        }

        private void RankDetail_Loaded(object sender, RoutedEventArgs e)
        {
            var resultItem = (ResultItem)DataContext;
            var parser = new RankLogParser(resultItem.Xml);

            html = html.Replace("{RANKLOGXML}", HttpUtility.HtmlEncode(resultItem.Xml));

            for (int i = 0; i < 2; i++)
            {
                string docPropsKey = "{DocProps" + (i + 1) + "}";
                string rankingFeaturesKey = "{RankingFeatures" + (i + 1) + "}";
                string valuesKey = "{Values" + (i + 1) + "}";
                string docHitsKey = "{DocHits" + (i + 1) + "}";
                if (parser.ScoreDetails[i] != null)
                {
                    var props = new DocProps
                                    {
                                        _ModelId = parser.ScoreDetails[i].ModelId,
                                        _Score = parser.ScoreDetails[i].Score,
                                        _OriginalScore = parser.ScoreDetails[i].OriginalScore,
                                        _ModelType = parser.ScoreDetails[i].ModelType
                                    };

                    var list = new List<RenderableRankingFeature>();
                    BM25 bm = null;
                    foreach (RankingFeature feature in parser.ScoreDetails[i].RankingFeatures)
                    {
                        RenderableRankingFeature item = null;
                        if (feature is BM25)
                        {
                            bm = (BM25)feature;
                            item = new RenderableBM25F(bm);
                        }
                        else if (feature is BucketedStatic)
                        {
                            item = new RenderableBucketedStatic((BucketedStatic)feature);
                        }
                        else if (feature is Dynamic)
                        {
                            item = new RenderableDynamicFeature((Dynamic)feature);
                        }
                        else if (feature is StaticRank)
                        {
                            item = new RenderableStaticFeature((StaticRank)feature);
                        }
                        else if (feature is MinSpan)
                        {
                            item = new RenderableMinSpan((MinSpan)feature);
                        }
                        if (item != null)
                        {
                            list.Add(item);
                        }
                    }
                    props._iExternalDocId = bm.ExternalDocId;
                    props._iInternalDocId = bm.InternalDocId;

                    props._Language = resultItem.Language;
                    if (i == 0)
                    {
                        props._Title = resultItem.Title;
                        props._Path = resultItem.Path;
                    }

                    ReplaceDocumentProperties(docPropsKey, props);

                    var ft = new RankingFeatures { _RankingFeatures = list };
                    ReplaceDocumentProperties(rankingFeaturesKey, ft);

                    var values = new RankValuesList();
                    values._bm25Feature = new RenderableBM25F(bm);
                    ReplaceDocumentProperties(valuesKey, values);

                    var docHits = new RankDocHitList();
                    docHits._bm25Feature = new RenderableBM25F(bm);
                    ReplaceDocumentProperties(docHitsKey, docHits);
                }
                else
                {
                    html = html.Replace(docPropsKey, "");
                    html = html.Replace(rankingFeaturesKey, "");
                    html = html.Replace(valuesKey, "");
                    html = html.Replace(docHitsKey, "");
                }
            }

            htmlControl.NavigateToString(html);
        }

        private void ReplaceDocumentProperties(string key, WebControl ctrl)
        {
            StringBuilder sb = RenderControl(ctrl);
            html = html.Replace(key, sb.ToString());
        }

        private static StringBuilder RenderControl(WebControl props)
        {
            var sb = new StringBuilder();
            using (var htmlWriter = new HtmlTextWriter(new StringWriter(sb)))
            {
                props.RenderControl(htmlWriter);
            }
            return sb;
        }
    }
}