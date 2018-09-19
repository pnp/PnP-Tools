using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharePointPnP.Modernization.Framework.Transform;

namespace SharePointPnP.Modernization.Framework.Tests.Transform
{
    [TestClass]
    public class Text
    {

        [TestMethod]
        public void TextTest()
        {
            string source = "<p><span class=\"ms-rteStyle-Normal\">Norm</span>​al<br></p><h1>Hea​​ding1<br></h1><h2>hea​​​ding2<br></h2><h3>Hea​​ding3<br></h3><h4>​heading4​​​​<br></h4><p>Quote<br></p><p>Text in <strong>bold</strong>, in <em>italic</em>, in <span style=\"text-decoration&#58;underline;\">underline</span>, in <span class=\"ms-rteThemeForeColor-5-0\" style=\"\">red</span> with <span class=\"ms-rteBackColor-4\">yellow</span> highlight<br></p><p>with <span style=\"text-decoration&#58;line-through;\">striked</span>, with <sup>superscript</sup> with <sub>lowerscript</sub> and with a different <span class=\"ms-rteFontSize-5\">size</span><br></p><p>left centered<br></p><p style=\"text-align&#58;center;\">Middle centered</p><p style=\"text-align&#58;right;\">Right centered<br></p><p style=\"text-align&#58;justify;\">spread<br></p><blockquote style=\"margin&#58;0px 0px 0px 40px;border&#58;none;padding&#58;0px;\"><p>Indent1</p></blockquote><blockquote style=\"margin&#58;0px 0px 0px 40px;border&#58;none;padding&#58;0px;\"><blockquote style=\"margin&#58;0px 0px 0px 40px;border&#58;none;padding&#58;0px;\"><p>Indent2</p></blockquote></blockquote><ul><li>Bullet 1<br></li><li>Bullet2<br></li><ul><li>Bullet 2.1</li></ul></ul><ol><li>Numbered1<br></li><li>Numbered2<br></li></ol><p>with a link to <a href=\"https&#58;//www.microsoft.com/\">microsoft.com​</a></p><p>table centered</p><p></p><table cellspacing=\"0\" class=\"ms-rteTable-1 \" style=\"width&#58;100%;\"><tbody><tr class=\"ms-rteTableHeaderRow-1\"><th class=\"ms-rteTableHeaderEvenCol-1\" rowspan=\"1\" colspan=\"1\" style=\"width&#58;33.3333%;\">​​H1<br></th><th class=\"ms-rteTableHeaderOddCol-1\" rowspan=\"1\" colspan=\"1\" style=\"width&#58;33.3333%;\">​H2<br></th><th class=\"ms-rteTableHeaderEvenCol-1\" rowspan=\"1\" colspan=\"1\" style=\"width&#58;33.3333%;\">​H3<br></th></tr><tr class=\"ms-rteTableOddRow-1\"><td class=\"ms-rteTableEvenCol-1\">​v1<br></td><td class=\"ms-rteTableOddCol-1\">​v2<br></td><td class=\"ms-rteTableEvenCol-1\">​v3<br></td></tr><tr class=\"ms-rteTableEvenRow-1\"><td class=\"ms-rteTableEvenCol-1\">​v12<br></td><td class=\"ms-rteTableOddCol-1\">​v22<br></td><td class=\"ms-rteTableEvenCol-1\">​v32<br></td></tr><tr class=\"ms-rteTableOddRow-1\"><td class=\"ms-rteTableEvenCol-1\">​v13<br></td><td class=\"ms-rteTableOddCol-1\">​​v23<br></td><td class=\"ms-rteTableEvenCol-1\">​v33<br></td></tr></tbody></table><p><br><br></p><p><br></p>";

            HtmlTransformator transformator = new HtmlTransformator();
            System.IO.File.WriteAllText(@"C:\github\BertPnPTools\Solutions\SharePoint.Modernization\SharePoint.Modernization.Framework.Tests\Transform\rewrittenhtml.html", transformator.Transform(source, true));
        }

    }
}
