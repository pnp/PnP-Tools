using SharePoint.Modernization.Scanner.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SharePoint.Client
{
    public static class UserCustomActionCollectionExtensions
    {

        /// <summary>
        /// Analyses the user custom actions
        /// </summary>
        /// <param name="userCustomActions">UserCustomAction collection</param>
        /// <param name="siteCollectionUrl">Url of the site collection</param>
        /// <param name="siteUrl">Url of the site</param>
        /// <returns>Collection of UserCustomActions that are not respected in the modern UI</returns>
        public static List<UserCustomActionResult> Analyze(this UserCustomActionCollection userCustomActions, string siteCollectionUrl, string siteUrl)
        {
            List<UserCustomActionResult> issues = new List<UserCustomActionResult>();

            foreach (UserCustomAction uca in userCustomActions)
            {
                bool add = false;
                UserCustomActionResult result = new UserCustomActionResult()
                {
                    SiteURL = siteUrl,
                    SiteColUrl = siteCollectionUrl,
                    Title = uca.Title,
                    Name = uca.Name,
                    Location = uca.Location,
                    RegistrationType = uca.RegistrationType,
                    RegistrationId = uca.RegistrationId,
                    ScriptBlock = "",
                    ScriptSrc = "",
                };

                if (!string.IsNullOrEmpty(uca.Location))
                {
                    if (!(uca.Location.Equals("EditControlBlock", StringComparison.InvariantCultureIgnoreCase) ||
                          uca.Location.StartsWith("ClientSideExtension.", StringComparison.InvariantCultureIgnoreCase) ||
                          uca.Location.Equals("CommandUI.Ribbon", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        add = true;
                        result.ScriptBlock = uca.ScriptBlock != null ? uca.ScriptBlock : "";
                        result.ScriptSrc = uca.ScriptSrc != null ? uca.ScriptSrc : "";
                        result.Problem = "InvalidLocation";
                    }
                }

                if (!string.IsNullOrEmpty(uca.CommandUIExtension))
                {
                    XmlDocument doc = new XmlDocument();
                    string xmlString = uca.CommandUIExtension;
                    xmlString = xmlString.Replace("http://schemas.microsoft.com/sharepoint/", "");
                    doc.LoadXml(xmlString);

                    XmlNodeList handlers = doc.SelectNodes("/CommandUIExtension/CommandUIHandlers/CommandUIHandler");
                    foreach (XmlNode handler in handlers)
                    {
                        if (handler.Attributes["CommandAction"] != null && handler.Attributes["CommandAction"].Value.ToLower().Contains("javascript"))
                        {
                            result.CommandAction = handler.Attributes["CommandAction"].Value;
                            result.Problem = !String.IsNullOrEmpty(result.Problem) ? $"{result.Problem}, JavaScriptEmbedded" : "JavaScriptEmbedded";
                            add = true;
                            break;
                        }
                    }
                }

                if (add)
                {
                    issues.Add(result);
                }
            }

            return issues;
        }

    }
}
