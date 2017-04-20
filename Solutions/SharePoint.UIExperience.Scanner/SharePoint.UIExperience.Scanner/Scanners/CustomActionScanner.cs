using Microsoft.SharePoint.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharePoint.UIExperience.Scanner.Scanners
{
    /// <summary>
    /// Scans the site for ignored user custom actions
    /// </summary>
    public class CustomActionScanner
    {
        private string url;
        private string siteColUrl;

        public CustomActionScanner(string url, string siteColUrl)
        {
            this.url = url;
            this.siteColUrl = siteColUrl;
        }

        /// <summary>
        /// Analyze passed site for ignored user custom actions
        /// </summary>
        /// <param name="cc">ClientContext object of the site to scan</param>
        /// <param name="customActions">Custom action details</param>
        /// <param name="customizationResults">Customization details</param>
        public void Analyze(ClientContext cc, ref ConcurrentStack<CustomActionsResult> customActions, ref ConcurrentDictionary<string, CustomizationResult> customizationResults, ref ConcurrentStack<UIExperienceScanError> UIExpScanErrors)
        {
            Console.WriteLine("Custom actions... " + url);

            // List scoped user custom actions
            var lists = cc.Web.GetListsToScan();
            foreach (var list in lists)
            {
                AddCustomActionsToResult(list.UserCustomActions, ref customActions, ref customizationResults, ref UIExpScanErrors, list.RootFolder.ServerRelativeUrl, list.Title);
            }

            Web web = cc.Web;
            if (!web.IsSubSite())
            {
                // Site scoped user custom actions
                Site site = cc.Site;
                site.EnsureProperty(p => p.UserCustomActions);
                AddCustomActionsToResult(site.UserCustomActions, ref customActions, ref customizationResults, ref UIExpScanErrors);
            }

            // Web scoped user custom actions
            web.EnsureProperty(p => p.UserCustomActions);
            AddCustomActionsToResult(web.UserCustomActions, ref customActions, ref customizationResults, ref UIExpScanErrors);
        }

        private void AddCustomActionsToResult(UserCustomActionCollection coll, ref ConcurrentStack<CustomActionsResult> customActions, ref ConcurrentDictionary<string, CustomizationResult> customizationResults, ref ConcurrentStack<UIExperienceScanError> UIExpScanErrors, string listUrl = "", string listTitle = "")
        {
            var baseUri = new Uri(this.url);
            var webAppUrl = baseUri.Scheme + "://" + baseUri.Host;

            foreach (UserCustomAction uca in coll)
            {
                try
                {
                    bool add = false;
                    CustomActionsResult result = new CustomActionsResult()
                    {
                        SiteUrl = this.url,
                        Url = !String.IsNullOrEmpty(listUrl) ? $"{webAppUrl}{listUrl}" : this.url,
                        SiteColUrl = this.siteColUrl,
                        ListTitle = listUrl,
                        Title = uca.Title,
                        Name = uca.Name,
                        Location = uca.Location,
                        RegistrationType = uca.RegistrationType,
                        RegistrationId = uca.RegistrationId,
                        CommandActions = "",
                        //ImageMaps = "",
                        ScriptBlock = "",
                        ScriptSrc = "",
                    };

                    if (!(uca.Location.Equals("EditControlBlock", StringComparison.InvariantCultureIgnoreCase) ||
                          uca.Location.Equals("CommandUI.Ribbon", StringComparison.InvariantCultureIgnoreCase) ))
                    {
                        add = true;
                        result.ScriptBlock = uca.ScriptBlock != null ? uca.ScriptBlock : "";
                        result.ScriptSrc = uca.ScriptSrc != null ? uca.ScriptSrc : "";
                        result.Problem = "Invalid location";
                    }

                    // List scoped custom actions registered to a specific listid do work in "modern" 
                    //Guid registrationIDGuid;
                    //if (Guid.TryParse(uca.RegistrationId, out registrationIDGuid))
                    //{
                    //    result.Problem = !String.IsNullOrEmpty(result.Problem) ? $"{result.Problem}, Specific list registration" : "Specific list registration";
                    //    add = true;
                    //}

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
                                result.CommandActions = "JS Found";
                                result.Problem = !String.IsNullOrEmpty(result.Problem) ? $"{result.Problem}, JavaScript embedded" : "JavaScript embedded";
                                add = true;
                                break;
                            }
                        }

                        // Skipping image maps as these UCA do show, but without image
                        //XmlNodeList imageButtons = doc.SelectNodes("//Button");
                        //foreach (XmlNode btn in imageButtons)
                        //{
                        //    //Image16by16Left, Image16by16Top, Image32by32Left, Image32by32Top
                        //    if (btn.Attributes["Image16by16"] != null || btn.Attributes["Image32by32"] != null)
                        //    {
                        //        result.ImageMaps = "Found";
                        //        result.Problem = !String.IsNullOrEmpty(result.Problem) ? $"{result.Problem}, ImageMap used" : "ImageMap used";
                        //        add = true;
                        //        break;
                        //    }
                        //}
                    }

                    if (add)
                    {
                        customActions.Push(result);

                        if (customizationResults.ContainsKey(result.Url))
                        {
                            var customizationResult = customizationResults[result.Url];
                            customizationResult.IgnoredCustomAction = true;
                            if (!customizationResults.TryUpdate(result.Url, customizationResult, customizationResult))
                            {
                                UIExperienceScanError error = new UIExperienceScanError()
                                {
                                    Error = $"Could not update custom action scan result for {customizationResult.Url}",
                                    SiteURL = this.url,
                                    SiteColUrl = this.siteColUrl
                                };
                                UIExpScanErrors.Push(error);
                                Console.WriteLine($"Could not update custom action scan result for {customizationResult.Url}");
                            }
                        }
                        else
                        {
                            var customizationResult = new CustomizationResult()
                            {
                                SiteUrl = result.SiteUrl,
                                Url = result.Url,
                                SiteColUrl = this.siteColUrl,
                                IgnoredCustomAction = true
                            };

                            if (!customizationResults.TryAdd(customizationResult.Url, customizationResult))
                            {
                                UIExperienceScanError error = new UIExperienceScanError()
                                {
                                    Error = $"Could not add custom action scan result for {customizationResult.Url}",
                                    SiteURL = url,
                                    SiteColUrl = siteColUrl
                                };
                                UIExpScanErrors.Push(error);
                                Console.WriteLine($"Could not add custom action scan result for {customizationResult.Url}");
                            }
                        }

                    }
                }
                catch(Exception ex)
                {
                    UIExperienceScanError error = new UIExperienceScanError()
                    {
                        Error = ex.Message,
                        SiteURL = this.url,
                        SiteColUrl = this.siteColUrl
                    };
                    UIExpScanErrors.Push(error);
                    Console.WriteLine("Error for site {1}: {0}", ex.Message, this.url);
                }
            }
        }
    }
}
