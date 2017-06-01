using Microsoft.SharePoint.Client.WebParts;
using SharePoint.UIExperience.Scanner;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.SharePoint.Client
{
    public static partial class FileExtensions
    {
        // Modern list experience - Web block feature that can be enabled to prevent modern library experience in the web
        private static readonly Guid FeatureId_Web_Modern = new Guid("52E14B6F-B1BB-4969-B89B-C4FAA56745EF");
        // Modern list experience - Site block feature that can be enabled to prevent modern library experience in the complete site collection
        private static readonly Guid FeatureId_Site_Modern = new Guid("E3540C7D-6BEA-403C-A224-1A12EAFEE4C4");
        // Managed metadata navigation feature
        private static readonly Guid FeatureId_Web_MetaDataNav = new Guid("7201d6a4-a5d3-49a1-8c19-19c4bac6e668");

        // View flags
        private static readonly uint ViewFlag_Gantt = 0x04000000;
        private static readonly uint ViewFlag_Calendar = 0x00080000;
        private static readonly uint ViewFlag_Grid = 0x00000800;

        /// <summary>
        /// Scans a list for "modern" compatibility
        /// </summary>
        /// <param name="file">List form page to start the scan from</param>
        /// <param name="list">List linked to the form page</param>
        /// <returns>Object describing modern compatiblity</returns>
        public static ListResult ModernCompatability(this File file, List list, ref ConcurrentStack<UIExperienceScanError> UIExpScanErrors)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            ClientContext cc = file.Context as ClientContext;

            ListResult result = new ListResult();

            // Load properties
            file.EnsureProperties(p => p.PageRenderType);

            // If it works in modern, we're good
            if (file.PageRenderType == ListPageRenderType.Modern)
            {
                // let's return since we know it will work
                return result;
            }
            else
            {
                result.PageRenderType = file.PageRenderType;
            }

            // Hmmm...it's not working, so let's list *all* reasons why it's not working in modern

            // Step 1: load the tenant / site / web / list level blocking options
            // Tenant
            // Currently we've no API to detect tenant setting...but since we anyhow should scan all lists this does not matter that much

            // Site
            Site site = cc.Site;
            site.EnsureProperties(p => p.Features, p => p.Url);
            result.BlockedAtSiteLevel = site.Features.Where(f => f.DefinitionId == FeatureId_Site_Modern).Count() > 0;
            // Web
            cc.Web.EnsureProperties(p => p.Features, p => p.Url);
            result.BlockedAtWebLevel = cc.Web.Features.Where(f => f.DefinitionId == FeatureId_Web_Modern).Count() > 0;
            // List
            list.EnsureProperties(p => p.ListExperienceOptions, p => p.UserCustomActions, p => p.BaseTemplate);
            result.ListExperience = list.ListExperienceOptions;
            if (list.ListExperienceOptions == ListExperience.ClassicExperience)
            {
                result.BlockedAtListLevel = true;
            }

            // Step 2: verify we can load a web part manager and ensure there's only one web part of the page            
            LimitedWebPartManager wpm;
            try
            {
                wpm = file.GetLimitedWebPartManager(PersonalizationScope.Shared);
                file.Context.Load(wpm.WebParts, wps => wps.Include(wp => wp.WebPart.Title, wp => wp.WebPart.Properties));
                file.Context.ExecuteQueryRetry();
            }
            catch (Exception ex)
            {
                result.BlockedByNotBeingAbleToLoadPage = true;
                result.BlockedByNotBeingAbleToLoadPageException = ex.ToString();
                return result;
            }

            if (wpm.WebParts.Count != 1)
            {
                result.BlockedByZeroOrMultipleWebParts = true;
                return result;
            }

            var webPart = wpm.WebParts[0].WebPart;

            // Step 3: Inspect the web part used to render the list
            // Step 3.1: JSLink web part check
            if (webPart.Properties.FieldValues.Keys.Contains("JSLink"))
            {
                if (webPart.Properties["JSLink"] != null && !String.IsNullOrEmpty(webPart.Properties["JSLink"].ToString()) && webPart.Properties["JSLink"].ToString().ToLower() != "clienttemplates.js")
                {
                    result.XsltViewWebPartCompatibility.BlockedByJSLink = true;
                    result.XsltViewWebPartCompatibility.JSLink = webPart.Properties["JSLink"].ToString();
                }
            }

            // Step 3.2: XslLink web part check
            if (webPart.Properties.FieldValues.Keys.Contains("XslLink"))
            {
                if (webPart.Properties["XslLink"] != null && !String.IsNullOrEmpty(webPart.Properties["XslLink"].ToString()) && webPart.Properties["XslLink"].ToString().ToLower() != "main.xsl")
                {
                    result.XsltViewWebPartCompatibility.BlockedByXslLink = true;
                    result.XsltViewWebPartCompatibility.XslLink = webPart.Properties["XslLink"].ToString();
                }
            }

            // Step 3.3: Xsl web part check
            if (webPart.Properties.FieldValues.Keys.Contains("Xsl"))
            {
                if (webPart.Properties["Xsl"] != null && !String.IsNullOrEmpty(webPart.Properties["Xsl"].ToString()))
                {
                    result.XsltViewWebPartCompatibility.BlockedByXsl = true;
                }
            }

            // Step 3.4: Process fields in view
            if (webPart.Properties.FieldValues.Keys.Contains("XmlDefinition"))
            {
                if (webPart.Properties["XmlDefinition"] != null && !String.IsNullOrEmpty(webPart.Properties["XmlDefinition"].ToString()))
                {
                    try
                    {
                        // Get the fields in this view
                        var viewFields = GetViewFields(webPart.Properties["XmlDefinition"].ToString());

                        // Load fields in one go
                        List<Field> fieldsToProcess = new List<Field>(viewFields.Count);
                        try
                        {
                            foreach (var viewField in viewFields)
                            {
                                Field field = list.Fields.GetByInternalNameOrTitle(viewField);
                                cc.Load(field, p => p.JSLink, p => p.TypeAsString, p => p.FieldTypeKind, p => p.InternalName);
                                fieldsToProcess.Add(field);
                            }
                            cc.ExecuteQueryRetry();
                        }
                        catch
                        {
                            // try to load the fields again, but now individually so we can collect the needed errors + evaulate the fields that do load
                            fieldsToProcess.Clear();
                            foreach (var viewField in viewFields)
                            {
                                try
                                {
                                    Field field = list.Fields.GetByInternalNameOrTitle(viewField);
                                    cc.Load(field, p => p.JSLink, p => p.TypeAsString, p => p.FieldTypeKind);
                                    cc.ExecuteQueryRetry();
                                    fieldsToProcess.Add(field);
                                }
                                catch(Exception ex)
                                {
                                    UIExperienceScanError error = new UIExperienceScanError()
                                    {
                                        Error = ex.Message,
                                        SiteURL = cc.Web.Url,
                                        SiteColUrl = site.Url
                                    };
                                    UIExpScanErrors.Push(error);
                                    Console.WriteLine("Error for site {1}: {0}", ex.Message, cc.Web.Url);
                                }
                            }
                        }

                        // Verify the fields
                        foreach(var field in fieldsToProcess)
                        {
                            try
                            {
                                // JSLink on field
                                if (!string.IsNullOrEmpty(field.JSLink) && field.JSLink != "clienttemplates.js" &&
                                    field.JSLink != "sp.ui.reputation.js" && !field.IsTaxField())
                                {
                                    result.XsltViewWebPartCompatibility.BlockedByJSLinkField = true;
                                    result.XsltViewWebPartCompatibility.JSLinkFields = string.IsNullOrEmpty(result.XsltViewWebPartCompatibility.JSLinkFields) ? $"{field.InternalName}" : $"{result.XsltViewWebPartCompatibility.JSLinkFields},{field.InternalName}";
                                }

                                //Business data field
                                if (field.IsBusinessDataField())
                                {
                                    result.XsltViewWebPartCompatibility.BlockedByBusinessDataField = true;
                                    result.XsltViewWebPartCompatibility.BusinessDataFields = string.IsNullOrEmpty(result.XsltViewWebPartCompatibility.BusinessDataFields) ? $"{field.InternalName}" : $"{result.XsltViewWebPartCompatibility.BusinessDataFields},{field.InternalName}";
                                }

                                // Geolocation field
                                if (field.FieldTypeKind == FieldType.Geolocation)
                                {
                                    result.XsltViewWebPartCompatibility.BlockedByGeoLocationField = true;
                                    result.XsltViewWebPartCompatibility.GeoLocationFields = string.IsNullOrEmpty(result.XsltViewWebPartCompatibility.GeoLocationFields) ? $"{field.InternalName}" : $"{result.XsltViewWebPartCompatibility.GeoLocationFields},{field.InternalName}";
                                }

                                // TaskOutcome field
                                if (field.IsTaskOutcomeField())
                                {
                                    result.XsltViewWebPartCompatibility.BlockedByTaskOutcomeField = true;
                                    result.XsltViewWebPartCompatibility.TaskOutcomeFields = string.IsNullOrEmpty(result.XsltViewWebPartCompatibility.TaskOutcomeFields) ? $"{field.InternalName}" : $"{result.XsltViewWebPartCompatibility.TaskOutcomeFields},{field.InternalName}";
                                }

                                // Publishing field
                                if (field.IsPublishingField())
                                {
                                    result.XsltViewWebPartCompatibility.BlockedByPublishingField = true;
                                    result.XsltViewWebPartCompatibility.PublishingFields = string.IsNullOrEmpty(result.XsltViewWebPartCompatibility.PublishingFields) ? $"{field.InternalName}" : $"{result.XsltViewWebPartCompatibility.PublishingFields},{field.InternalName}";
                                }
                            }
                            catch (Exception ex)
                            {
                                UIExperienceScanError error = new UIExperienceScanError()
                                {
                                    Error = ex.Message,
                                    SiteURL = cc.Web.Url,
                                    SiteColUrl = site.Url,
                                };
                                UIExpScanErrors.Push(error);
                                Console.WriteLine("Error for site {1}: {0}", ex.Message, cc.Web.Url);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        UIExperienceScanError error = new UIExperienceScanError()
                        {
                            Error = ex.Message,
                            SiteURL = cc.Web.Url,
                            SiteColUrl = site.Url
                        };
                        UIExpScanErrors.Push(error);
                        Console.WriteLine("Error for site {1}: {0}", ex.Message, cc.Web.Url);
                    }
                }
            }

            // Step 3.5: Process list custom actions
            if (list.UserCustomActions.Count > 0)
            {
                foreach(var customAction in list.UserCustomActions)
                {
                    if (!string.IsNullOrEmpty(customAction.Location) && customAction.Location.Equals("scriptlink", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(customAction.ScriptSrc))
                        {
                            result.XsltViewWebPartCompatibility.BlockedByListCustomAction = true;
                            result.XsltViewWebPartCompatibility.ListCustomActions = string.IsNullOrEmpty(result.XsltViewWebPartCompatibility.ListCustomActions) ? $"{customAction.Name}" : $"{result.XsltViewWebPartCompatibility.ListCustomActions},{customAction.Name}";
                        }
                    }
                }
            }

            // Step 3.6: Check for managed metadata navigation feature
            if (!Constants.IsExcludedFromScan(Constants.ManagedMetadataNavigationSupport))
            {
                cc.Web.EnsureProperties(p => p.Features);
                result.XsltViewWebPartCompatibility.BlockedByManagedMetadataNavFeature = cc.Web.Features.Where(f => f.DefinitionId == FeatureId_Web_MetaDataNav).Count() > 0;
            }

            // Step 4: check the view
            if (webPart.Properties.FieldValues.Keys.Contains("ViewFlags") && webPart.Properties["ViewFlags"] != null && !String.IsNullOrEmpty(webPart.Properties["ViewFlags"].ToString()))
            {
                uint flags;
                if (uint.TryParse(webPart.Properties["ViewFlags"].ToString(), out flags))
                {
                    if ((flags & ViewFlag_Gantt) != 0 || (flags & ViewFlag_Calendar) != 0 || (flags & ViewFlag_Grid) != 0)
                    {
                        result.XsltViewWebPartCompatibility.BlockedByViewType = true;
                        if ((flags & ViewFlag_Gantt) != 0)
                        {
                            result.XsltViewWebPartCompatibility.ViewType = "Gantt";
                        }
                        else if ((flags & ViewFlag_Calendar) != 0)
                        {
                            result.XsltViewWebPartCompatibility.ViewType = "Calendar";
                        }
                        else if ((flags & ViewFlag_Grid) != 0)
                        {
                            result.XsltViewWebPartCompatibility.ViewType = "Grid";
                        }
                    }
                }
            }

            // Step 5: check the list
            // Step 5.1: check the base template
            if (!list.CanRenderNewExperience())
            {
                result.XsltViewWebPartCompatibility.BlockedByListBaseTemplate = true;
                result.XsltViewWebPartCompatibility.ListBaseTemplate = list.BaseTemplate;
            }

            return result;
        }

        private static List<string> GetViewFields(string xmlDefinition)
        {
            List<string> viewFields = new List<string>(10);

            var viewXml = XElement.Parse(xmlDefinition);

            var fields = viewXml.Descendants("FieldRef");
            foreach(var field in fields)
            {
                viewFields.Add(field.Attribute("Name").Value);
            }            

            return viewFields;
        }

    }
}
