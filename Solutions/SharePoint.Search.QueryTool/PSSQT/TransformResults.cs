using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace PSSQT
{

    class TransformResults
    {
        private static readonly string defaultXslt = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<xsl:stylesheet version=""1.0""
	xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">

  <xsl:template match=""/"">
    <html>
      <head>
        <script type=""text/javascript"" src=""https://www.gstatic.com/charts/loader.js"">&#160;</script>
        <script type=""text/javascript"">
          google.charts.load(""current"", {
          packages: [""corechart""]
          });
          google.charts.setOnLoadCallback(drawChart);

          function drawChart() {
            var data = google.visualization.arrayToDataTable([
            [
				'Rank Feature',

				<xsl:for-each select=""results/rankcontributions/result[1]"">
				  <xsl:for-each select=""@*"">
					<xsl:if test=""name() != 'pos'"">
						'<xsl:value-of select=""name()""/>',
					</xsl:if>
				  </xsl:for-each>

				</xsl:for-each>
				  {
				  role: 'annotation'
				  }
			],
            <xsl:for-each select=""results/rankcontributions/result"">
			[   
			  '<xsl:value-of select=""position()""/>',
              <xsl:for-each select=""@*"">
					<xsl:if test=""name() != 'pos'"">
						<xsl:value-of select="".""/>,
					</xsl:if>
              </xsl:for-each>

              '<xsl:value-of select="".""/>']
              <xsl:if test=""position() != last()"">
                ,
              </xsl:if>
            </xsl:for-each>

            ]);

            var view = new google.visualization.DataView(data);

            var options = {
            width: 1200,
            height: 300,
            legend: {
            position: 'top',
            maxLines: 10
            },
            bar: {
            groupWidth: '75%'
            },
            isStacked: true
            };

            var chart = new google.visualization.BarChart(document.getElementById(""barchart_values""));
            chart.draw(view, options);
          }



          function toggleDetails(id)
          {
          var div = document.getElementById(id);
          if (div.style.display !== ""none"") {
                              div.style.display = (!div.style.display) ? ""block"" : ""none"";
          }
          else {
                              div.style.display = ""block"";
          }
          }

        </script>

        <style>
          table .statTerms {border-spacing: 5px;}
          th, td {
          padding: 10px;
          }


          .resposition {
          color: blue;
          font-size: 24px;
          }

          .restitle {
          font-size: 24px;
          }

          .resrank {
          margin-left: 10px;
          font-size: 18px;
          }

          .resxmlrank {
          margin-left: 10px;
          font-size: 16px;
          color: gray;
          }

          .detailBtn {
          margin-left: 10px;
          }

          .respath {
          margin-left: 28px;
          color: gray;
          font-size: 16px;
          }

          .collapse {
          display: none;
          }

          h4   {background-color: blue; color:white;}

          .rank {
          color: blue;
          font-size: 16px;
          position: absolute;
          left: 150px;
          }

          .rawrankxml {
          width: 100%;
          height: 200px;
          }



        </style>

      </head>
      <body>
        <h1>Rank Log</h1>
        <span>(Click on Title to expand details.)</span>
        <div id=""resultset"">
          <xsl:variable name=""rankmax"" select=""results/result[1]/Rank""/>


          <xsl:for-each select=""results/result"">
            <br/>
            <xsl:variable name=""i"" select=""position()""/>
            <xsl:variable name=""path"" select=""path""/>
            <xsl:variable name=""originalrank"" select=""rank_log/stage[last()]/@rank_after"" />

            <span class=""resposition"">
              <xsl:value-of select=""@pos""/>.
            </span>
            <span class=""restitle"" onclick=""toggleDetails('rankdetails-{$i}')"">
              <xsl:value-of select=""title""/>
            </span>
            <span class=""resrank"" title=""Rank"">
              (<xsl:value-of select='format-number(Rank,""#.00"")'/>)
            </span>
            <span class=""resxmlrank"" title=""Original Rank (w/o XRANK)"">
              [<xsl:value-of select='format-number($originalrank,""#.00"")'/>]
            </span>
            <br/>
            <span class=""respath"">
              <a href=""{$path}"">
                <xsl:value-of select=""path""/>
              </a>
            </span>
            <br/>
            <br />

            <div id=""rankdetails-{$i}"" class=""collapse"" style=""height:auto"">

              <!-- rank log -->
              <!-- <br/> -->
              <div class=""btndiv"">
                <button class=""detailBtn"" onclick=""toggleDetails('querytree-{$i}')"">Query Tree</button>

                <div id=""querytree-{$i}"" class=""collapse"">
                  <xsl:value-of select=""rank_log/query/@tree""></xsl:value-of>
                </div>

                <br/>
                <br/>

                <button class=""detailBtn"" onclick=""toggleDetails('queryproperties-{$i}')"">Query Properties</button>

                <div id=""queryproperties-{$i}"" class=""collapse"">
                  <xsl:value-of select=""rank_log/query/@properties""></xsl:value-of>
                </div>

                <br/>


                <xsl:for-each select=""rank_log/stage"">

                  <br/>

                  <xsl:variable name=""rs"" select=""position()""/>

                  <xsl:variable name=""stageName"">
                    <xsl:value-of select=""@type""/>
                  </xsl:variable>


                  <button class=""detailBtn"" onclick=""toggleDetails('{$stageName}-{$rs}-{$i}')"">
                    <xsl:value-of select=""@type""/>
                  </button>

                  <span class=""rank"">
                    Rank: <xsl:value-of select=""@rank_after""/>
                  </span>

                  <div id=""{$stageName}-{$rs}-{$i}"" class=""collapse"">
                    <h4>Term Statistics</h4>

                    <table class=""statTerms"">
                      <tr>
                        <td>Query:</td>
                        <td>
                          <xsl:value-of select=""bm25/query_term/@term""/>
                        </td>
                      </tr>
                      <tr>
                        <td>n:</td>
                        <td>
                          <xsl:value-of select=""bm25/query_term/index/@n""/>
                        </td>
                      </tr>
                      <tr>
                        <td>N:</td>
                        <td>
                          <xsl:value-of select=""bm25/query_term/index/@N""/>
                        </td>
                      </tr>
                      <tr>
                        <td>BM25 Weight:</td>
                        <td>
                          <xsl:value-of select=""bm25/query_term/rank/@term_weight""/>
                        </td>
                      </tr>
                      <tr>
                        <td>Weighted TF:</td>
                        <td>TBD</td>
                      </tr>
                      <tr>
                        <td>Term Score:</td>
                        <td>
                          <xsl:value-of select=""bm25/final/@normalized""/>
                        </td>
                      </tr>
                    </table>

                    <h4>Hits in Document</h4>

                    <table class=""statTerms"">
                    </table>

                    <h4>Document Properties</h4>

                    <table class=""statTerms"">
                      <tr>
                        <td>External Doc Id</td>
                        <td>
                          <xsl:value-of select=""bm25/query_term/index/group/@ext_doc_id""/>
                        </td>
                      </tr>
                      <tr>
                        <td>Internal Doc Id</td>
                        <td>
                          <xsl:value-of select=""bm25/query_term/index/group/@int_doc_id""/>
                        </td>
                      </tr>
                      <tr>
                        <td>Normalized Rank</td>
                        <td>
                          <xsl:value-of select=""@rank_after""/>
                        </td>
                      </tr>
                    </table>

                    <h4>Ranking Features</h4>

                    <table class=""statTerms"">
                      <tr>
                        <th>Name</th>
                        <th>Raw Value</th>
                        <!-- <th>Raw Value Transformed</th> -->
                        <th>Transformed</th>
                        <th>Normalized</th>
                      </tr>
                      <!-- bm25 -->
                      <xsl:for-each select=""bm25"">
                        <tr>
                          <td>BM25</td>
                          <td>
                            <xsl:value-of select=""final/@score""/>
                          </td>
                          <!-- <td><xsl:value-of select=""@raw_value_transformed""/></td> -->
                          <td>
                            <xsl:value-of select=""final/@transformed""/>
                          </td>
                          <td>
                            <xsl:value-of select=""final/@normalized""/>
                          </td>

                        </tr>
                      </xsl:for-each>


                      <!-- static_feature -->
                      <xsl:for-each select=""static_feature"">
                        <tr>
                          <td>
                            <xsl:value-of select=""@name""/>
                          </td>
                          <td>
                            <xsl:value-of select=""@raw_value""/>
                          </td>
                          <!-- <td><xsl:value-of select=""@raw_value_transformed""/></td> -->
                          <td>
                            <xsl:value-of select=""@transformed""/>
                          </td>
                          <td>
                            <xsl:value-of select=""@normalized""/>
                          </td>

                        </tr>
                      </xsl:for-each>

                      <!-- bucketed_static_feature -->
                      <xsl:for-each select=""bucketed_static_feature"">
                        <tr>
                          <td>
                            <xsl:value-of select=""@name""/>
                          </td>
                          <td>
                            <xsl:value-of select=""@raw_value""/>
                          </td>
                          <!-- <td><xsl:value-of select=""@raw_value_transformed""/></td> -->
                          <td></td>
                          <td></td>

                        </tr>
                      </xsl:for-each>

                      <!-- proximity_feature -->
                      <xsl:for-each select=""proximity_feature"">
                        <tr>
                          <td>
                            <xsl:value-of select=""@name""/>
                          </td>
                          <td>
                            <xsl:value-of select=""@raw_value""/>
                          </td>
                          <!-- <td></td> -->
                          <td>
                            <xsl:value-of select=""@transformed""/>
                          </td>
                          <td>
                            <xsl:value-of select=""@normalized""/>
                          </td>

                        </tr>
                      </xsl:for-each>

                      <!-- dynamic -->
                    </table>


                  </div>

                  <br/>
                </xsl:for-each>

                <br/>
                <button class=""detailBtn"" onclick=""toggleDetails('rankxml-{$i}')"" title=""Copy to favorite editor and format. E.g. Notepad++ with XML tools installed."">Rank XML</button>

                <div id=""rankxml-{$i}"" class=""collapse"">
                  <textarea class=""rawrankxml"">
                    <xsl:copy-of select=""rank_log""/>
                  </textarea>
                </div>

                <br/>
              </div>
            </div>
          </xsl:for-each>

        </div>

		<hr/>
		<br/>
        <!-- Identify where the chart should be drawn. -->
        <div id=""barchart_values""></div>
        
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
";

        public static string Transform(string resultsAsXml)
        {
            return Transform(resultsAsXml, defaultXslt);
        }


        public static string Transform(string resultsAsXml, string xsltString)
        {
            XslCompiledTransform xslt = new XslCompiledTransform();

            using (XmlReader reader = XmlReader.Create(new StringReader(xsltString)))
            {
                xslt.Load(reader);

                using (StringReader strReader = new StringReader(resultsAsXml))
                {
                    //Create a new XPathDocument and load the XML data to be transformed.
                    XPathDocument mydata = new XPathDocument(strReader);

                    //Create an XmlTextWriter which outputs to a string.
                    using (StringWriter strWriter = new StringWriter())
                    {
                        using (XmlWriter writer = new XmlTextWriter(strWriter))
                        {
                            //Transform the data and send the output to the console.
                            xslt.Transform(mydata, null, writer, null);

                            return strWriter.ToString();

                        }

                    }
                }

            }

        }

        public static string DefaultXSLT
        {
            get
            {
                return defaultXslt;
            }
            
        }
    }
}
