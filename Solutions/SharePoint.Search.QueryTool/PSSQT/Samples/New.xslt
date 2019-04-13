<xsl:stylesheet
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="html" version="5.0" indent="yes" doctype-system="about:legacy-compat"/>

  <xsl:template match="/">
    <html>
      <head>
        <title>list test</title>
        <style>
          /********************/
          /* EXPANDABLE LIST  */
          /********************/
          #listContainer{
          margin-top:15px;
          }

          #expList ul, li {
          list-style: none;
          margin:0;
          padding:0;
          cursor: pointer;
          }
          #expList p {
          margin:0;
          display:block;
          }
          #expList p:hover {
          background-color:#121212;
          }
          #expList li {
          line-height:140%;
          text-indent:0px;
          background-position: 1px 8px;
          padding-left: 20px;
          background-repeat: no-repeat;
          }

          /* Collapsed state for list element */
          #expList .collapsed {
          background-image: url(http://jasalguero.com/demos/expandableList/img/collapsed.png);
          }
          /* Expanded state for list element
          /* NOTE: This class must be located UNDER the collapsed one */
          #expList .expanded {
          background-image: url(http://jasalguero.com/demos/expandableList/img/expanded.png);
          }
          #expList {
          clear: both;
          }

          .listControl{
          margin-bottom: 15px;
          }
          .listControl a {
          border: 1px solid #555555;
          color: #555555;
          cursor: pointer;
          height: 1.5em;
          line-height: 1.5em;
          margin-right: 5px;
          padding: 4px 10px;
          }
          .listControl a:hover {
          background-color:#555555;
          color:#222222;
          font-weight:normal;
          }
        </style>
        <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>
        <script>
          /**************************************************************/
          /* Prepares the cv to be dynamically expandable/collapsible   */
          /**************************************************************/
          function prepareList() {
          $('#expList').find('li:has(ul)')
          .click( function(event) {
          if (this == event.target) {
          $(this).toggleClass('expanded');
          $(this).children('ul').toggle('medium');
          }
          return false;
          })
          .addClass('collapsed')
          .children('ul').hide();

          //Create the button funtionality
          $('#expandList')
          .unbind('click')
          .click( function() {
          $('.collapsed').addClass('expanded');
          $('.collapsed').children().show('medium');
          })
          $('#collapseList')
          .unbind('click')
          .click( function() {
          $('.collapsed').removeClass('expanded');
          $('.collapsed').children().hide('medium');
          })

          };


          /**************************************************************/
          /* Functions to execute on loading the document               */
          /**************************************************************/
          $(document).ready( function() {
          prepareList()
          });
        </script>
      </head>
      <body>
        <xsl:apply-templates/>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="results">
    <div id="listContainer">
      <div class="listControl">
        <a id="expandList">Expand All</a>
        <a id="collapseList">Collapse All</a>
      </div>
      <ul id="expList">
        <li>
          <xsl:apply-templates/>
        </li>
      </ul>
    </div>
  </xsl:template>

  <!--<xsl:template match="rank_log">
    <ul>
      <li>
        <xsl:value-of select="@instance_id"/>
        <ul>
          <xsl:apply-templates/>
        </ul>
      </li>
    </ul>
  </xsl:template>-->

  <xsl:template match="result">
    <ul>
      <li>
        Position: <xsl:value-of select="@pos"/>
        <ul>
          <xsl:apply-templates/>
        </ul>
        <hr/>
      </li>
    </ul>
  </xsl:template>

  <xsl:template match="Title">
    <li>
      Title: <xsl:apply-templates/>
    </li>
  </xsl:template>
  
  <xsl:template match="Path">
    <li>
      Path: <xsl:apply-templates/>
    </li>
  </xsl:template>
  
  <xsl:template match="Rank">
    <li>
      Rank: <xsl:apply-templates/>
    </li>
  </xsl:template>

  <xsl:template match="WorkId">
    <li>
      WorkId: <xsl:apply-templates/>
    </li>
  </xsl:template>

</xsl:stylesheet>
