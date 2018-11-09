using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using SharePoint.Modernization.Scanner.Results;
using SharePoint.Scanning.Framework;
using SharePointPnP.Modernization.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Scanner.Telemetry
{
    /// <summary>
    /// Class that's responsible for handling telemetry
    /// </summary>
    public class ScannerTelemetry
    {
        private readonly TelemetryClient telemetryClient;

        #region Construction
        /// <summary>
        /// Instantiates the telemetry client
        /// </summary>
        public ScannerTelemetry()
        {
            try
            {
                this.telemetryClient = new TelemetryClient
                {
                    InstrumentationKey = "70f0a42a-e1ae-4dc5-ad9a-380ce98dc30a"
                };
                
                // Setting this is needed to make metric tracking work
                TelemetryConfiguration.Active.InstrumentationKey = this.telemetryClient.InstrumentationKey;

                this.telemetryClient.Context.Session.Id = Guid.NewGuid().ToString();
                this.telemetryClient.Context.Cloud.RoleInstance = "SharePointPnPModernizationScanner";
                this.telemetryClient.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
                var coreAssembly = Assembly.GetExecutingAssembly();
                this.telemetryClient.Context.GlobalProperties.Add("Version", ((AssemblyFileVersionAttribute)coreAssembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute))).Version.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Telemetry setup failed: {ex.Message}. Continuing without telemetry");
            }
        }
        #endregion

        /// <summary>
        /// Scan start logging
        /// </summary>
        /// <param name="options">Scanner options</param>
        public void LogScanStart(Options options)
        {
            if (this.telemetryClient == null)
            {
                return;
            }

            try
            {
                // Prepare event data
                Dictionary<string, string> properties = new Dictionary<string, string>();
                Dictionary<string, double> metrics = new Dictionary<string, double>();

                // Mode
                properties.Add(ScanStart.Mode.ToString(), options.Mode.ToString());
                // Used authentication approach
                string authenticationModel = "";
                if (!string.IsNullOrEmpty(options.CertificatePfx))
                {
                    authenticationModel = "AzureADAppOnly";
                }
                else if (!string.IsNullOrEmpty(options.ClientSecret))
                {
                    authenticationModel = "AzureACSAppOnly";
                }
                else
                {
                    authenticationModel = "Credentials";
                }
                properties.Add(ScanStart.AuthenticationModel.ToString(), authenticationModel);
                // Used site selection model
                string siteSelectionModel = "";
                if (options.Urls != null && options.Urls.Count > 0)
                {
                    siteSelectionModel = "Urls";
                }
                else if (!string.IsNullOrEmpty(options.CsvFile))
                {
                    siteSelectionModel = "CSVFile";
                }
                else
                {
                    siteSelectionModel = "Tenant";
                }
                properties.Add(ScanStart.SiteSelectionModel.ToString(), siteSelectionModel);

                // track event
                this.telemetryClient.TrackEvent(TelemetryEvents.ScanStart.ToString(), properties, metrics);
            }
            catch
            {
                // Eat all exceptions 
            }

        }

        /// <summary>
        /// Page scan results log
        /// </summary>
        /// <param name="scannedSites">Scanned sites</param>
        /// <param name="scannedWebs">Scanned webs</param>
        /// <param name="pageScanResults">Scanned page results</param>
        /// <param name="pageTransformation">Page transformation data</param>
        public void LogPageScan(int scannedSites, int scannedWebs, ConcurrentDictionary<string, PageScanResult> pageScanResults, PageTransformation pageTransformation)
        {
            if (this.telemetryClient == null)
            {
                return;
            }

            try
            {
                Dictionary<string, string> properties = new Dictionary<string, string>();
                Dictionary<string, double> metrics = new Dictionary<string, double>();

                properties.Add(PagesResults.Sites.ToString(), scannedSites.ToString());
                properties.Add(PagesResults.Webs.ToString(), scannedWebs.ToString());
                properties.Add(PagesResults.Pages.ToString(), pageScanResults.Count.ToString());

                this.telemetryClient.TrackEvent(TelemetryEvents.Pages.ToString(), properties, metrics);

                this.telemetryClient.GetMetric($"Pages.{PagesResults.Sites.ToString()}").TrackValue(scannedSites);
                this.telemetryClient.GetMetric($"Pages.{PagesResults.Webs.ToString()}").TrackValue(scannedWebs);
                this.telemetryClient.GetMetric($"Pages.{PagesResults.Pages.ToString()}").TrackValue(pageScanResults.Count);

                Metric pageType = this.telemetryClient.GetMetric($"Pages.{PagesResults.PageType.ToString()}", "Pages.PageType");
                Metric pageLayout = this.telemetryClient.GetMetric($"Pages.{PagesResults.PageLayout.ToString()}", "Pages.PageLayout");
                Metric isHomePage = this.telemetryClient.GetMetric($"Pages.{PagesResults.IsHomePage.ToString()}", "Pages.IsHomePage");
                Metric webPartMapping = this.telemetryClient.GetMetric($"Pages.{PagesResults.WebPartMapping.ToString()}", "Pages.WebPartMapping");
                Metric unMappedWebParts = this.telemetryClient.GetMetric($"Pages.{PagesResults.UnMappedWebParts.ToString()}", "Pages.UnMappedWebParts");

                foreach (var item in pageScanResults)
                {
                    WriteMetric(pageType, item.Value.PageType);
                    WriteMetric(pageLayout, item.Value.Layout);
                    WriteMetric(isHomePage, item.Value.HomePage);

                    if (item.Value.WebParts != null)
                    {
                        int webPartsOnPage = item.Value.WebParts.Count();
                        int webPartsOnPageMapped = 0;
                        List<string> nonMappedWebParts = new List<string>();
                        foreach (var webPart in item.Value.WebParts.OrderBy(p => p.Row).ThenBy(p => p.Column).ThenBy(p => p.Order))
                        {
                            var found = pageTransformation.WebParts.Where(p => p.Type.Equals(webPart.Type, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            if (found != null && found.Mappings != null)
                            {
                                webPartsOnPageMapped++;
                            }
                            else
                            {
                                var t = webPart.Type.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0];
                                if (!nonMappedWebParts.Contains(t))
                                {
                                    nonMappedWebParts.Add(t);
                                }
                            }
                        }

                        WriteMetric(webPartMapping, String.Format("{0:0}", (((double)webPartsOnPageMapped / (double)webPartsOnPage) * 100)));
                        foreach(var w in nonMappedWebParts)
                        {
                            WriteMetric(unMappedWebParts, w);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Eat all exceptions 
            }
            finally
            {
                this.telemetryClient.Flush();
            }
        }

        /// <summary>
        /// Log list scanning results
        /// </summary>
        /// <param name="scannedSites">Scanned sites</param>
        /// <param name="scannedWebs">Scannned webs</param>
        /// <param name="listScanResults">List scan results</param>
        /// <param name="scannedLists">Scanned lists</param>
        public void LogListScan(int scannedSites, int scannedWebs, ConcurrentDictionary<string, ListScanResult> listScanResults, int scannedLists)
        {
            if (this.telemetryClient == null)
            {
                return;
            }

            try
            {
                Dictionary<string, string> properties = new Dictionary<string, string>();
                Dictionary<string, double> metrics = new Dictionary<string, double>();

                properties.Add(ListResults.Sites.ToString(), scannedSites.ToString());
                properties.Add(ListResults.Webs.ToString(), scannedWebs.ToString());
                properties.Add(ListResults.Lists.ToString(), scannedLists.ToString());

                this.telemetryClient.TrackEvent(TelemetryEvents.List.ToString(), properties, metrics);

                this.telemetryClient.GetMetric($"List.{ListResults.Sites.ToString()}").TrackValue(scannedSites);
                this.telemetryClient.GetMetric($"List.{ListResults.Webs.ToString()}").TrackValue(scannedWebs);
                this.telemetryClient.GetMetric($"List.{ListResults.Lists.ToString()}").TrackValue(scannedLists);

                Metric onlyBlockedByOOB = this.telemetryClient.GetMetric($"List.{ListResults.OnlyBlockedByOOB.ToString()}", "List.OnlyBlockedByOOB");
                Metric renderType = this.telemetryClient.GetMetric($"List.{ListResults.RenderType.ToString()}", "List.RenderType");
                Metric listExperience = this.telemetryClient.GetMetric($"List.{ListResults.ListExperience.ToString()}", "List.ListExperience");
                Metric baseTemplateNotWorking = this.telemetryClient.GetMetric($"List.{ListResults.BaseTemplateNotWorking.ToString()}", "List.BaseTemplateNotWorking");
                Metric viewTypeNotWorking = this.telemetryClient.GetMetric($"List.{ListResults.ViewTypeNotWorking.ToString()}", "List.ViewTypeNotWorking");
                Metric multipleWebParts = this.telemetryClient.GetMetric($"List.{ListResults.MultipleWebParts.ToString()}", "List.MultipleWebParts");
                Metric jsLinkWebPart = this.telemetryClient.GetMetric($"List.{ListResults.JSLinkWebPart.ToString()}", "List.JSLinkWebPart");
                Metric jsLinkField = this.telemetryClient.GetMetric($"List.{ListResults.JSLinkField.ToString()}", "List.JSLinkField");
                Metric xslLink = this.telemetryClient.GetMetric($"List.{ListResults.XslLink.ToString()}", "List.XslLink");
                Metric xsl = this.telemetryClient.GetMetric($"List.{ListResults.Xsl.ToString()}", "List.Xsl");
                Metric listCustomAction = this.telemetryClient.GetMetric($"List.{ListResults.ListCustomAction.ToString()}", "List.ListCustomAction");
                Metric publishingField = this.telemetryClient.GetMetric($"List.{ListResults.PublishingField.ToString()}", "List.PublishingField");
                Metric siteBlocking = this.telemetryClient.GetMetric($"List.{ListResults.SiteBlocking.ToString()}", "List.SiteBlocking");
                Metric webBlocking = this.telemetryClient.GetMetric($"List.{ListResults.WebBlocking.ToString()}", "List.WebBlocking");
                Metric listBlocking = this.telemetryClient.GetMetric($"List.{ListResults.ListBlocking.ToString()}", "List.ListBlocking");

                foreach(var item in listScanResults)
                {
                    WriteMetric(onlyBlockedByOOB, item.Value.OnlyBlockedByOOBReasons);
                    WriteMetric(renderType, item.Value.PageRenderType.ToString());
                    WriteMetric(listExperience, item.Value.ListExperience.ToString());
                    WriteMetric(baseTemplateNotWorking, item.Value.XsltViewWebPartCompatibility.ListBaseTemplate);
                    WriteMetric(viewTypeNotWorking, item.Value.XsltViewWebPartCompatibility.BlockedByViewType);
                    WriteMetric(multipleWebParts, item.Value.BlockedByZeroOrMultipleWebParts);
                    WriteMetric(jsLinkWebPart, item.Value.XsltViewWebPartCompatibility.BlockedByJSLink);
                    WriteMetric(jsLinkField, item.Value.XsltViewWebPartCompatibility.BlockedByJSLinkField);
                    WriteMetric(xslLink, item.Value.XsltViewWebPartCompatibility.BlockedByXslLink);
                    WriteMetric(xsl, item.Value.XsltViewWebPartCompatibility.BlockedByXsl);
                    WriteMetric(listCustomAction, item.Value.XsltViewWebPartCompatibility.BlockedByListCustomAction);
                    WriteMetric(publishingField, item.Value.XsltViewWebPartCompatibility.BlockedByPublishingField);
                    WriteMetric(siteBlocking, item.Value.BlockedAtSiteLevel);
                    WriteMetric(webBlocking, item.Value.BlockedAtSiteLevel);
                    WriteMetric(listBlocking, item.Value.BlockedAtListLevel);
                }

            }
            catch (Exception ex)
            {
                // Eat all exceptions 
            }
            finally
            {
                this.telemetryClient.Flush();
            }
        }

        /// <summary>
        /// Log group connnect results
        /// </summary>
        /// <param name="siteScanResults">Scanned sites results</param>
        /// <param name="webScanResults">Scanned web results</param>
        /// <param name="everyoneClaim">Eveyone claim value</param>
        /// <param name="everyoneExceptExternalUsersClaim">EveryoneExceptExternalUsers claim value</param>
        public void LogGroupConnectScan(ConcurrentDictionary<string, SiteScanResult> siteScanResults, ConcurrentDictionary<string, WebScanResult> webScanResults, string everyoneClaim,string everyoneExceptExternalUsersClaim)
        {
            if (this.telemetryClient == null)
            {
                return;
            }

            try
            {
                // Prepare event data
                Dictionary<string, string> properties = new Dictionary<string, string>();
                Dictionary<string, double> metrics = new Dictionary<string, double>();

                properties.Add(GroupConnectResults.Sites.ToString(), siteScanResults.Count.ToString());
                properties.Add(GroupConnectResults.Webs.ToString(), webScanResults.Count.ToString());

                this.telemetryClient.TrackEvent(TelemetryEvents.GroupConnect.ToString(), properties, metrics);

                this.telemetryClient.GetMetric($"GroupConnect.{GroupConnectResults.Sites.ToString()}").TrackValue(siteScanResults.Count);
                this.telemetryClient.GetMetric($"GroupConnect.{GroupConnectResults.Webs.ToString()}").TrackValue(webScanResults.Count);

                Metric readyForGroupConnect = this.telemetryClient.GetMetric($"GroupConnect.{GroupConnectResults.ReadyForGroupConnect.ToString()}", "GroupConnect.ReadyForGroupConnect");
                Metric blockingReason = this.telemetryClient.GetMetric($"GroupConnect.{GroupConnectResults.BlockingReason.ToString()}", "GroupConnect.BlockingReason");
                Metric webTemplate = this.telemetryClient.GetMetric($"GroupConnect.{GroupConnectResults.WebTemplate.ToString()}", "GroupConnect.WebTemplate");
                Metric warning = this.telemetryClient.GetMetric($"GroupConnect.{GroupConnectResults.Warning.ToString()}", "GroupConnect.Warning");
                Metric modernUIWarning = this.telemetryClient.GetMetric($"GroupConnect.{GroupConnectResults.ModernUIWarning.ToString()}", "GroupConnect.ModernUIWarning");
                Metric permissionWarning = this.telemetryClient.GetMetric($"GroupConnect.{GroupConnectResults.PermissionWarning.ToString()}", "GroupConnect.PermissionWarning");

                foreach (var item in siteScanResults)
                {
                    var groupifyBlockers = item.Value.GroupifyBlockers();
                    var groupifyWarnings = item.Value.GroupifyWarnings(everyoneClaim, everyoneExceptExternalUsersClaim);
                    var modernWarnings = item.Value.ModernWarnings();
                    var groupSecurity = item.Value.PermissionModel(everyoneClaim, everyoneExceptExternalUsersClaim);

                    WriteMetric(readyForGroupConnect, groupifyBlockers.Count > 0);
                    
                    foreach(var blocker in groupifyBlockers)
                    {
                        WriteMetric(blockingReason, blocker);
                    }

                    WriteMetric(webTemplate, item.Value.WebTemplate);

                    foreach(var w in groupifyWarnings)
                    {
                        WriteMetric(warning, w);
                    }

                    foreach(var w in modernWarnings)
                    {
                        WriteMetric(modernUIWarning, w);
                    }

                    foreach (var w in groupSecurity.Item2)
                    {
                        WriteMetric(permissionWarning, w);
                    }

                }
            }
            catch (Exception ex)
            {
                // Eat all exceptions 
            }
            finally
            {
                this.telemetryClient.Flush();
            }
        }

        /// <summary>
        /// Log publishing portal scan
        /// </summary>
        /// <param name="publishingSiteScanResults">Scanned publishing portals</param>
        /// <param name="publishingWebScanResults">Scanned publishing webs</param>
        /// <param name="publishingPageScanResults">Scanned publishing pages</param>
        public void LogPublishingScan(Dictionary<string, PublishingSiteScanResult> publishingSiteScanResults, 
                                      ConcurrentDictionary<string, PublishingWebScanResult> publishingWebScanResults,
                                      ConcurrentDictionary<string, PublishingPageScanResult> publishingPageScanResults)
        {
            if (this.telemetryClient == null)
            {
                return;
            }

            try
            {
                // Prepare event data
                Dictionary<string, string> properties = new Dictionary<string, string>();
                Dictionary<string, double> metrics = new Dictionary<string, double>();

                properties.Add(PublishingResults.Sites.ToString(), publishingSiteScanResults.Count.ToString());
                properties.Add(PublishingResults.Webs.ToString(), publishingWebScanResults.Count.ToString());
                properties.Add(PublishingResults.Pages.ToString(), (publishingSiteScanResults != null ? publishingPageScanResults.Count : 0).ToString());

                this.telemetryClient.TrackEvent(TelemetryEvents.PublishingPortals.ToString(), properties, metrics);

                this.telemetryClient.GetMetric($"Publishing.{PublishingResults.Sites.ToString()}").TrackValue(publishingSiteScanResults.Count);
                this.telemetryClient.GetMetric($"Publishing.{PublishingResults.Webs.ToString()}").TrackValue(publishingWebScanResults.Count);
                this.telemetryClient.GetMetric($"Publishing.{PublishingResults.Pages.ToString()}").TrackValue(publishingSiteScanResults != null ? publishingPageScanResults.Count : 0);

                Metric siteLevel = this.telemetryClient.GetMetric($"Publishing.{PublishingResults.SiteLevel.ToString()}", "Publishing.SiteLevel");                
                Metric complexity = this.telemetryClient.GetMetric($"Publishing.{PublishingResults.Complexity.ToString()}", "Publishing.Complexity");
                Metric webTemplates = this.telemetryClient.GetMetric($"Publishing.{PublishingResults.WebTemplates.ToString()}", "Publishing.WebTemplates");
                Metric globalNavigation = this.telemetryClient.GetMetric($"Publishing.{PublishingResults.GlobalNavigation.ToString()}", "Publishing.GlobalNavigation");
                Metric currentNavigation = this.telemetryClient.GetMetric($"Publishing.{PublishingResults.CurrentNavigation.ToString()}", "Publishing.CurrentNavigation");
                Metric customSiteMasterPage = this.telemetryClient.GetMetric($"Publishing.{PublishingResults.CustomSiteMasterPage.ToString()}", "Publishing.CustomSiteMasterPage");
                Metric customSystemMasterPage = this.telemetryClient.GetMetric($"Publishing.{PublishingResults.CustomSystemMasterPage.ToString()}", "Publishing.CustomSystemMasterPage");
                Metric alternateCSS = this.telemetryClient.GetMetric($"Publishing.{PublishingResults.AlternateCSS.ToString()}", "Publishing.AlternateCSS");
                Metric incompatibleUserCustomActions = this.telemetryClient.GetMetric($"Publishing.{PublishingResults.IncompatibleUserCustomActions.ToString()}", "Publishing.IncompatibleUserCustomActions");
                Metric customPageLayouts = this.telemetryClient.GetMetric($"Publishing.{PublishingResults.CustomPageLayouts.ToString()}", "Publishing.CustomPageLayouts");
                Metric pageApproval = this.telemetryClient.GetMetric($"Publishing.{PublishingResults.PageApproval.ToString()}", "Publishing.PageApproval");
                Metric pageApprovalWorkflow = this.telemetryClient.GetMetric($"Publishing.{PublishingResults.PageApprovalWorkflow.ToString()}", "Publishing.PageApprovalWorkflow");
                Metric scheduledPublishing = this.telemetryClient.GetMetric($"Publishing.{PublishingResults.ScheduledPublishing.ToString()}", "Publishing.ScheduledPublishing");
                Metric audienceTargeting = this.telemetryClient.GetMetric($"Publishing.{PublishingResults.AudienceTargeting.ToString()}", "Publishing.AudienceTargeting");
                Metric languages = this.telemetryClient.GetMetric($"Publishing.{PublishingResults.Languages.ToString()}", "Publishing.Languages");
                Metric variationLabels = this.telemetryClient.GetMetric($"Publishing.{PublishingResults.VariationLabels.ToString()}", "Publishing.VariationLabels");

                // The metrics automatically aggregate and only send once a minute to app insights...so this approach does not result in extra traffic
                foreach (var item in publishingWebScanResults)
                {
                    WriteMetric(siteLevel, item.Value.Level);
                    WriteMetric(complexity, item.Value.WebClassification.ToString());
                    WriteMetric(webTemplates, item.Value.WebTemplate);
                    WriteMetric(globalNavigation, item.Value.GlobalNavigationType);
                    WriteMetric(currentNavigation, item.Value.CurrentNavigationType);
                    WriteMetric(customSiteMasterPage, string.IsNullOrEmpty(item.Value.SiteMasterPage) ? true.ToString() : false.ToString());
                    WriteMetric(customSystemMasterPage, string.IsNullOrEmpty(item.Value.SystemMasterPage) ? true.ToString() : false.ToString());
                    WriteMetric(alternateCSS, string.IsNullOrEmpty(item.Value.AlternateCSS) ? true.ToString() : false.ToString());
                    WriteMetric(incompatibleUserCustomActions, item.Value.UserCustomActions);
                    WriteMetric(pageApproval, item.Value.LibraryEnableModeration);
                    WriteMetric(pageApprovalWorkflow, item.Value.LibraryApprovalWorkflowDefined);
                    WriteMetric(scheduledPublishing, item.Value.LibraryItemScheduling);
                    WriteMetric(languages, item.Value.Language);
                    WriteMetric(variationLabels, item.Value.VariationSourceLabel);
                }

                if (publishingSiteScanResults != null)
                {
                    foreach(var item in publishingPageScanResults)
                    {
                        WriteMetric(customPageLayouts, item.Value.PageLayoutWasCustomized);
                        WriteMetric(audienceTargeting, (item.Value.SecurityGroupAudiences != null && item.Value.SecurityGroupAudiences.Count > 0) ||
                                                       (item.Value.SharePointGroupAudiences != null && item.Value.SharePointGroupAudiences.Count > 0) ||
                                                       (item.Value.GlobalAudiences != null && item.Value.GlobalAudiences.Count > 0));
                    }
                }
            }
            catch(Exception ex)
            {
                // Eat all exceptions 
            }
            finally
            {
                this.telemetryClient.Flush();
            }
        }

        /// <summary>
        /// Log the scan done event
        /// </summary>
        /// <param name="duration">Scan duration</param>
        public void LogScanDone(TimeSpan duration)
        {
            if (this.telemetryClient == null)
            {
                return;
            }

            try
            {
                // Prepare event data
                Dictionary<string, string> properties = new Dictionary<string, string>();
                Dictionary<string, double> metrics = new Dictionary<string, double>();

                if (duration != null)
                {
                    properties.Add(ScanDone.Duration.ToString(), duration.Minutes.ToString());
                }

                this.telemetryClient.TrackEvent(TelemetryEvents.ScanDone.ToString(), properties, metrics);
            }
            catch
            {
                // Eat all exceptions 
            }
        }

        public void LogScanError(Exception ex, ScanError error)
        {
            if (this.telemetryClient == null || ex == null)
            {
                return;
            }

            try
            {
                // Prepare event data
                Dictionary<string, string> properties = new Dictionary<string, string>();
                Dictionary<string, double> metrics = new Dictionary<string, double>();

                if (error != null)
                {
                    // Field 1 never contain PII data
                    if (!string.IsNullOrEmpty(error.Field1))
                    {
                        properties.Add(ScanCrash.StackTrace.ToString(), error.Field1);
                    }
                }

                this.telemetryClient.TrackException(ex, properties, metrics);
            }
            catch(Exception ex2)
            {
                // Eat all exceptions 
            }
        }


        /// <summary>
        /// Log an unexpected scanner crash
        /// </summary>
        /// <param name="crash">Object holdingu unexpected error</param>
        public void LogScanCrash(Object crash)
        {
            if (this.telemetryClient == null)
            {
                return;
            }

            try
            {
                // Prepare event data
                Dictionary<string, string> properties = new Dictionary<string, string>();
                Dictionary<string, double> metrics = new Dictionary<string, double>();

                if (crash != null)
                {
                    if (crash is Exception)
                    {
                        if (!string.IsNullOrEmpty((crash as Exception).Message))
                        {
                            properties.Add(ScanCrash.ExceptionMessage.ToString(), (crash as Exception).Message);
                        }
                        if (!string.IsNullOrEmpty((crash as Exception).StackTrace))
                        {
                            properties.Add(ScanCrash.StackTrace.ToString(), (crash as Exception).StackTrace);
                        }
                    }
                }

                this.telemetryClient.TrackEvent(TelemetryEvents.ScanCrash.ToString(), properties, metrics);
            }
            catch
            {
                // Eat all exceptions 
            }
            finally
            {
                this.Flush();
            }
        }

        /// <summary>
        /// Ensure telemetry data is send to server
        /// </summary>
        public void Flush()
        {
            try
            {
                // before exit, flush the remaining data
                this.telemetryClient.Flush();

                // flush is not blocking so wait a bit
                Task.Delay(5000).Wait();
            }
            catch
            {
                // Eat all exceptions
            }
        }

        #region Helper methods
        private void WriteMetric(Metric metric, string measure, int value = 1)
        {
            if (!string.IsNullOrEmpty(measure))
            {
                metric.TrackValue(value, measure);
            }
        }

        private void WriteMetric(Metric metric, bool measure, int value = 1)
        {
            metric.TrackValue(value, measure.ToString());
        }

        private void WriteMetric(Metric metric, int measure, int value = 1)
        {
            metric.TrackValue(value, measure.ToString());
        }

        private void WriteMetric(Metric metric, uint measure, int value = 1)
        {
            metric.TrackValue(value, measure.ToString());
        }

        private void WriteMetric<T>(Metric metric, IList<T> measure, int value = 1)
        {
            if (measure != null)
            {
                WriteMetric(metric, measure.Count > 0);
            }

        }
        #endregion

    }
}
