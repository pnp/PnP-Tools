using GenericParsing;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SharePoint.Modernization.Scanner.Reports
{
    /// <summary>
    /// Generates Excel based reports that make it easier to consume the collected data
    /// </summary>
    public class Generator
    {
        private const string ScannerSummaryCSV = "ScannerSummary.csv";
        // Groupify report variables
        private const string GroupifyCSV = "ModernizationSiteScanResults.csv";
        private const string GroupifyMasterFile = "groupifymaster.xlsx";
        private const string GroupifyReport = "Office 365 Group Connection Readiness.xlsx";
        // List report variables
        private const string ListCSV = "ModernizationListScanResults.csv";
        private const string ListMasterFile = "listmaster.xlsx";
        private const string ListReport = "Office 365 List Readiness.xlsx";
        // Page report variables
        private const string PageCSV = "PageScanResults.csv";
        private const string PageMasterFile = "pagemaster.xlsx";
        private const string PageReport = "Office 365 Page Transformation Readiness.xlsx";
        // Publishing report variables
        private const string PublishingSiteCSV = "ModernizationPublishingSiteScanResults.csv";
        private const string PublishingWebCSV = "ModernizationPublishingWebScanResults.csv";
        private const string PublishingPageCSV = "ModernizationPublishingPageScanResults.csv";
        private const string PublishingMasterFile = "publishingmaster.xlsx";
        private const string PublishingReport = "Office 365 Publishing Portal Transformation Readiness.xlsx";


        /// <summary>
        /// Create the list dashboard
        /// </summary>
        /// <param name="exportPaths">Paths to read data from</param>
        public void CreateListReport(IList<string> exportPaths)
        {
            DataTable blockedListsTable = null;
            ScanSummary scanSummary = null;

            var outputfolder = ".";
            DateTime dateCreationTime = DateTime.MinValue;
            if (exportPaths.Count == 1)
            {
                outputfolder = new DirectoryInfo(exportPaths[0]).FullName;
                var pathToUse = exportPaths[0].TrimEnd(new char[] { '\\' });
                dateCreationTime = File.GetCreationTime($"{pathToUse}\\{ListCSV}");
            }

            // import the data and "clean" it
            foreach (var path in exportPaths)
            {
                var pathToUse = path.TrimEnd(new char[] { '\\' });

                string csvToLoad = $"{pathToUse}\\{ListCSV}";

                if (!File.Exists(csvToLoad))
                {
                    // Skipping as one does not always have this report 
                    continue;
                }

                Console.WriteLine($"Generating Modern UI List Readiness report based upon data coming from {path}");

                using (GenericParserAdapter parser = new GenericParserAdapter(csvToLoad))
                {
                    parser.FirstRowHasHeader = true;
                    parser.MaxBufferSize = 200000;
                    parser.ColumnDelimiter = DetectUsedDelimiter(csvToLoad);

                    // Read the file                    
                    var baseTable = parser.GetDataTable();

                    // Handle "wrong" column name used in older versions
                    if (baseTable.Columns.Contains("Only blocked by OOB reaons"))
                    {
                        baseTable.Columns["Only blocked by OOB reaons"].ColumnName = "Only blocked by OOB reasons";
                    }

                    // Table 1
                    var blockedListsTable1 = baseTable.Copy();
                    // clean table
                    string[] columnsToKeep = new string[] { "Url", "Site Url", "Site Collection Url", "List Title", "Only blocked by OOB reasons", "Blocked at site level", "Blocked at web level", "Blocked at list level", "List page render type", "List experience", "Blocked by not being able to load Page", "Blocked by view type", "View type", "Blocked by list base template", "List base template", "Blocked by zero or multiple web parts", "Blocked by JSLink", "Blocked by XslLink", "Blocked by Xsl", "Blocked by JSLink field", "Blocked by business data field", "Blocked by task outcome field", "Blocked by publishingField", "Blocked by geo location field", "Blocked by list custom action" };
                    blockedListsTable1 = DropTableColumns(blockedListsTable1, columnsToKeep);

                    if (blockedListsTable == null)
                    {
                        blockedListsTable = blockedListsTable1;
                    }
                    else
                    {
                        blockedListsTable.Merge(blockedListsTable1);
                    }

                    // Read scanner summary data
                    var scanSummary1 = DetectScannerSummary($"{pathToUse}\\{ScannerSummaryCSV}");

                    if (scanSummary == null)
                    {
                        scanSummary = scanSummary1;
                    }
                    else
                    {
                        MergeScanSummaries(scanSummary, scanSummary1);
                    }
                }
            }

            if (blockedListsTable.Rows.Count == 0)
            {
                Console.WriteLine($"No blocked lists found...skipping report generation");
                return;
            }

            // Get the template Excel file
            using (Stream stream = typeof(Generator).Assembly.GetManifestResourceStream($"SharePoint.Modernization.Scanner.Reports.{ListMasterFile}"))
            {
                if (File.Exists($"{outputfolder}\\{ListMasterFile}"))
                {
                    File.Delete($"{outputfolder}\\{ListMasterFile}");
                }

                using (var fileStream = File.Create($"{outputfolder}\\{ListMasterFile}"))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
            }

            // Push the data to Excel, starting from an Excel template
            using (var excel = new ExcelPackage(new FileInfo($"{outputfolder}\\{ListMasterFile}"), false))
            //using (var excel = new ExcelPackage())
            {
                var dashboardSheet = excel.Workbook.Worksheets["Dashboard"];

                if (scanSummary != null)
                {
                    if (scanSummary.SiteCollections.HasValue)
                    {
                        dashboardSheet.SetValue("AA7", scanSummary.SiteCollections.Value);
                    }
                    if (scanSummary.Webs.HasValue)
                    {
                        dashboardSheet.SetValue("AC7", scanSummary.Webs.Value);
                    }
                    if (scanSummary.Lists.HasValue)
                    {
                        dashboardSheet.SetValue("AE7", scanSummary.Lists.Value);
                    }
                    if (scanSummary.Duration != null)
                    {
                        dashboardSheet.SetValue("AA8", scanSummary.Duration);
                    }
                    if (scanSummary.Version != null)
                    {
                        dashboardSheet.SetValue("AA9", scanSummary.Version);
                    }
                }

                if (dateCreationTime != DateTime.MinValue)
                {
                    dashboardSheet.SetValue("AA6", dateCreationTime.ToString("G", DateTimeFormatInfo.InvariantInfo));
                }
                else
                {
                    dashboardSheet.SetValue("AA6", "-");
                }

                var blockedListsSheet = excel.Workbook.Worksheets["BlockedLists"];
                //var blockedListsSheet = excel.Workbook.Worksheets.Add("BlockedLists");
                InsertTableData(blockedListsSheet.Tables[0], blockedListsTable);
                //blockedListsSheet.Cells["A1"].LoadFromDataTable(blockedListsTable, true);

                // Save the resulting file $"{outputfolder}\\{ListMasterFile}"
                if (File.Exists($"{outputfolder}\\{ListReport}"))
                {
                    File.Delete($"{outputfolder}\\{ListReport}");
                }
                excel.SaveAs(new FileInfo($"{outputfolder}\\{ListReport}"));
                //excel.SaveAs(new FileInfo(ListMasterFile));
            }

            // Clean the template file
            if (File.Exists($"{outputfolder}\\{ListMasterFile}"))
            {
                File.Delete($"{outputfolder}\\{ListMasterFile}");
            }
        }


        /// <summary>
        /// Create the publishing dashboard
        /// </summary>
        /// <param name="exportPaths">Paths to read data from</param>
        public void CreatePublishingReport(IList<string> exportPaths)
        {
            DataTable pubWebsBaseTable = null;
            DataTable pubPagesBaseTable = null;
            DataTable pubWebsTable = null;
            DataTable pubPagesTable = null;
            ScanSummary scanSummary = null;

            var outputfolder = ".";
            DateTime dateCreationTime = DateTime.MinValue;
            if (exportPaths.Count == 1)
            {
                outputfolder = new DirectoryInfo(exportPaths[0]).FullName;
                var pathToUse = exportPaths[0].TrimEnd(new char[] { '\\' });
                dateCreationTime = File.GetCreationTime($"{pathToUse}\\{PublishingWebCSV}");
            }

            // import the data and "clean" it
            foreach (var path in exportPaths)
            {
                var pathToUse = path.TrimEnd(new char[] { '\\' });

                // Load the webs CSV file
                string csvToLoad = $"{pathToUse}\\{PublishingWebCSV}";

                if (!File.Exists(csvToLoad))
                {
                    // Skipping as one does not always have this report 
                    continue;
                }

                Console.WriteLine($"Generating Publishing transformation report based upon data coming from {path}");

                using (GenericParserAdapter parser = new GenericParserAdapter(csvToLoad))
                {
                    parser.FirstRowHasHeader = true;
                    parser.MaxBufferSize = 200000;
                    parser.ColumnDelimiter = DetectUsedDelimiter(csvToLoad);

                    // Read the file                    
                    pubWebsBaseTable = parser.GetDataTable();

                    var pubWebsTable1 = pubWebsBaseTable.Copy();
                    // clean table
                    string[] columnsToKeep = new string[] { "SiteCollectionUrl", "SiteUrl", "WebRelativeUrl", "SiteCollectionComplexity", "WebTemplate", "Level", "PageCount", "Language", "VariationLabels", "VariationSourceLabel", "SiteMasterPage", "SystemMasterPage", "AlternateCSS", "HasIncompatibleUserCustomActions", "AllowedPageLayouts", "PageLayoutsConfiguration", "DefaultPageLayout", "GlobalNavigationType", "GlobalStructuralNavigationShowSubSites", "GlobalStructuralNavigationShowPages", "GlobalStructuralNavigationShowSiblings", "GlobalStructuralNavigationMaxCount", "GlobalManagedNavigationTermSetId", "CurrentNavigationType", "CurrentStructuralNavigationShowSubSites", "CurrentStructuralNavigationShowPages", "CurrentStructuralNavigationShowSiblings", "CurrentStructuralNavigationMaxCount", "CurrentManagedNavigationTermSetId", "ManagedNavigationAddNewPages", "ManagedNavigationCreateFriendlyUrls", "LibraryItemScheduling", "LibraryEnableModeration", "LibraryEnableVersioning", "LibraryEnableMinorVersions", "LibraryApprovalWorkflowDefined", "BrokenPermissionInheritance" };
                    pubWebsTable1 = DropTableColumns(pubWebsTable1, columnsToKeep);

                    if (pubWebsTable == null)
                    {
                        pubWebsTable = pubWebsTable1;
                    }
                    else
                    {
                        pubWebsTable.Merge(pubWebsTable1);
                    }

                    // Read scanner summary data
                    var scanSummary1 = DetectScannerSummary($"{pathToUse}\\{ScannerSummaryCSV}");

                    if (scanSummary == null)
                    {
                        scanSummary = scanSummary1;
                    }
                    else
                    {
                        MergeScanSummaries(scanSummary, scanSummary1);
                    }
                }

                // Load the site CSV file to count the site collection rows
                csvToLoad = $"{pathToUse}\\{PublishingSiteCSV}";
                if (File.Exists(csvToLoad))
                {
                    using (GenericParserAdapter parser = new GenericParserAdapter(csvToLoad))
                    {
                        parser.FirstRowHasHeader = true;
                        parser.MaxBufferSize = 200000;
                        parser.ColumnDelimiter = DetectUsedDelimiter(csvToLoad);

                        var siteData = parser.GetDataTable();
                        if (siteData != null)
                        {
                            scanSummary.SiteCollections = siteData.Rows.Count;
                        }
                    }
                }

                // Load the pages CSV file, if available
                csvToLoad = $"{pathToUse}\\{PublishingPageCSV}";
                if (File.Exists(csvToLoad))
                {
                    using (GenericParserAdapter parser = new GenericParserAdapter(csvToLoad))
                    {
                        parser.FirstRowHasHeader = true;
                        parser.MaxBufferSize = 200000;
                        parser.ColumnDelimiter = DetectUsedDelimiter(csvToLoad);

                        // Read the file                    
                        pubPagesBaseTable = parser.GetDataTable();

                        var pubPagesTable1 = pubPagesBaseTable.Copy();
                        // clean table
                        string[] columnsToKeep = new string[] { "SiteCollectionUrl", "SiteUrl", "WebRelativeUrl", "PageRelativeUrl", "PageName", "ContentType", "ContentTypeId", "PageLayout", "PageLayoutFile", "PageLayoutWasCustomized", "GlobalAudiences", "SecurityGroupAudiences", "SharePointGroupAudiences", "ModifiedAt", "Mapping %" };
                        pubPagesTable1 = DropTableColumns(pubPagesTable1, columnsToKeep);

                        if (pubPagesTable == null)
                        {
                            pubPagesTable = pubPagesTable1;
                        }
                        else
                        {
                            pubPagesTable.Merge(pubPagesTable1);
                        }
                    }
                }

                // Get the template Excel file
                using (Stream stream = typeof(Generator).Assembly.GetManifestResourceStream($"SharePoint.Modernization.Scanner.Reports.{PublishingMasterFile}"))
                {
                    if (File.Exists($"{outputfolder}\\{PublishingMasterFile}"))
                    {
                        File.Delete($"{outputfolder}\\{PublishingMasterFile}");
                    }

                    using (var fileStream = File.Create($"{outputfolder}\\{PublishingMasterFile}"))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.CopyTo(fileStream);
                    }
                }

                // Push the data to Excel, starting from an Excel template
                using (var excel = new ExcelPackage(new FileInfo($"{outputfolder}\\{PublishingMasterFile}"), false))
                {

                    var dashboardSheet = excel.Workbook.Worksheets["Dashboard"];
                    if (scanSummary != null)
                    {
                        if (scanSummary.SiteCollections.HasValue)
                        {
                            dashboardSheet.SetValue("U7", scanSummary.SiteCollections.Value);
                        }
                        if (scanSummary.Duration != null)
                        {
                            dashboardSheet.SetValue("U8", scanSummary.Duration);
                        }
                        if (scanSummary.Version != null)
                        {
                            dashboardSheet.SetValue("U9", scanSummary.Version);
                        }
                    }

                    if (dateCreationTime > DateTime.Now.Subtract(new TimeSpan(5*365,0,0,0,0)))
                    {
                        dashboardSheet.SetValue("U6", dateCreationTime.ToString("G", DateTimeFormatInfo.InvariantInfo));
                    }
                    else
                    {
                        dashboardSheet.SetValue("U6", "-");
                    }

                    var pubWebsSheet = excel.Workbook.Worksheets["PubWebs"];
                    InsertTableData(pubWebsSheet.Tables[0], pubWebsTable);

                    var pubPagesSheet = excel.Workbook.Worksheets["PubPages"];
                    if (pubPagesTable != null)
                    {
                        InsertTableData(pubPagesSheet.Tables[0], pubPagesTable);
                    }

                    // Save the resulting file
                    if (File.Exists($"{outputfolder}\\{PublishingReport}"))
                    {
                        File.Delete($"{outputfolder}\\{PublishingReport}");
                    }
                    excel.SaveAs(new FileInfo($"{outputfolder}\\{PublishingReport}"));
                }

                // Clean the template file
                if (File.Exists($"{outputfolder}\\{PublishingMasterFile}"))
                {
                    File.Delete($"{outputfolder}\\{PublishingMasterFile}");
                }
            }
        }

        /// <summary>
        /// Create the site page dashboard
        /// </summary>
        /// <param name="exportPaths">Paths to read data from</param>
        public void CreatePageReport(IList<string> exportPaths)
        {
            DataTable readyForPageTransformationTable = null;
            DataTable unmappedWebPartsTable = null;
            ScanSummary scanSummary = null;

            var outputfolder = ".";
            DateTime dateCreationTime = DateTime.MinValue;
            if (exportPaths.Count == 1)
            {
                outputfolder = new DirectoryInfo(exportPaths[0]).FullName;
                var pathToUse = exportPaths[0].TrimEnd(new char[] { '\\' });
                dateCreationTime = File.GetCreationTime($"{pathToUse}\\{PageCSV}");
            }

            // import the data and "clean" it
            foreach (var path in exportPaths)
            {
                var pathToUse = path.TrimEnd(new char[] { '\\' });

                string csvToLoad = $"{pathToUse}\\{PageCSV}";

                if (!File.Exists(csvToLoad))
                {
                    // Skipping as one does not always have this report 
                    continue;
                }

                Console.WriteLine($"Generating Page Transformation Readiness report based upon data coming from {path}");

                using (GenericParserAdapter parser = new GenericParserAdapter(csvToLoad))
                {
                    parser.FirstRowHasHeader = true;
                    parser.MaxBufferSize = 200000;
                    parser.ColumnDelimiter = DetectUsedDelimiter(csvToLoad);

                    // Read the file                    
                    var baseTable = parser.GetDataTable();

                    // Table 1
                    var readyForPageTransformationTable1 = baseTable.Copy();
                    // clean table
                    string[] columnsToKeep = new string[] { "SiteUrl", "PageUrl", "HomePage", "Type", "Layout", "Mapping %" };
                    readyForPageTransformationTable1 = DropTableColumns(readyForPageTransformationTable1, columnsToKeep);

                    if (readyForPageTransformationTable == null)
                    {
                        readyForPageTransformationTable = readyForPageTransformationTable1;
                    }
                    else
                    {
                        readyForPageTransformationTable.Merge(readyForPageTransformationTable1);
                    }

                    // Table 2
                    var unmappedWebPartsTable1 = baseTable.Copy();

                    // clean table
                    columnsToKeep = new string[] { "SiteUrl", "PageUrl", "Unmapped web parts" };
                    unmappedWebPartsTable1 = DropTableColumns(unmappedWebPartsTable1, columnsToKeep);
                    // expand rows
                    unmappedWebPartsTable1 = ExpandRows(unmappedWebPartsTable1, "Unmapped web parts");
                    // delete "unneeded" rows
                    for (int i = unmappedWebPartsTable1.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = unmappedWebPartsTable1.Rows[i];
                        if (string.IsNullOrEmpty(dr["Unmapped web parts"].ToString()))
                        {
                            dr.Delete();
                        }
                    }

                    if (unmappedWebPartsTable == null)
                    {
                        unmappedWebPartsTable = unmappedWebPartsTable1;
                    }
                    else
                    {
                        unmappedWebPartsTable.Merge(unmappedWebPartsTable1);
                    }

                    // Read scanner summary data
                    var scanSummary1 = DetectScannerSummary($"{pathToUse}\\{ScannerSummaryCSV}");

                    if (scanSummary == null)
                    {
                        scanSummary = scanSummary1;
                    }
                    else
                    {
                        MergeScanSummaries(scanSummary, scanSummary1);
                    }
                }
            }

            // Get the template Excel file
            using (Stream stream = typeof(Generator).Assembly.GetManifestResourceStream($"SharePoint.Modernization.Scanner.Reports.{PageMasterFile}"))
            {
                if (File.Exists($"{outputfolder}\\{PageMasterFile}"))
                {
                    File.Delete($"{outputfolder}\\{PageMasterFile}");
                }

                using (var fileStream = File.Create($"{outputfolder}\\{PageMasterFile}"))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
            }

            // Push the data to Excel, starting from an Excel template
            //using (var excel = new ExcelPackage(new FileInfo(PageMasterFile)))
            using (var excel = new ExcelPackage(new FileInfo($"{outputfolder}\\{PageMasterFile}"), false))
            {

                var dashboardSheet = excel.Workbook.Worksheets["Dashboard"];
                if (scanSummary != null)
                {
                    if (scanSummary.SiteCollections.HasValue)
                    {
                        dashboardSheet.SetValue("X7", scanSummary.SiteCollections.Value);
                    }
                    if (scanSummary.Webs.HasValue)
                    {
                        dashboardSheet.SetValue("Z7", scanSummary.Webs.Value);
                    }
                    if (scanSummary.Lists.HasValue)
                    {
                        dashboardSheet.SetValue("AB7", scanSummary.Lists.Value);
                    }
                    if (scanSummary.Duration != null)
                    {
                        dashboardSheet.SetValue("X8", scanSummary.Duration);
                    }
                    if (scanSummary.Version != null)
                    {
                        dashboardSheet.SetValue("X9", scanSummary.Version);
                    }
                }

                if (dateCreationTime > DateTime.Now.Subtract(new TimeSpan(5 * 365, 0, 0, 0, 0)))
                {
                    dashboardSheet.SetValue("X6", dateCreationTime.ToString("G", DateTimeFormatInfo.InvariantInfo));
                }
                else
                {
                    dashboardSheet.SetValue("X6", "-");
                }

                //var readyForPageTransformationSheet = excel.Workbook.Worksheets.Add("ReadyForPageTransformation");
                //var insertedRange = readyForPageTransformationSheet.Cells["A1"].LoadFromDataTable(readyForPageTransformationTable, true);

                //var unmappedWebPartsSheet = excel.Workbook.Worksheets.Add("UnmappedWebParts");
                //insertedRange = unmappedWebPartsSheet.Cells["A1"].LoadFromDataTable(unmappedWebPartsTable, true);

                var readyForPageTransformationSheet = excel.Workbook.Worksheets["ReadyForPageTransformation"];
                InsertTableData(readyForPageTransformationSheet.Tables[0], readyForPageTransformationTable);

                var unmappedWebPartsSheet = excel.Workbook.Worksheets["UnmappedWebParts"];
                InsertTableData(unmappedWebPartsSheet.Tables[0], unmappedWebPartsTable);

                // Save the resulting file
                if (File.Exists($"{outputfolder}\\{PageReport}"))
                {
                    File.Delete($"{outputfolder}\\{PageReport}");
                }
                excel.SaveAs(new FileInfo($"{outputfolder}\\{PageReport}"));
            }

            // Clean the template file
            if (File.Exists($"{outputfolder}\\{PageMasterFile}"))
            {
                File.Delete($"{outputfolder}\\{PageMasterFile}");
            }
        }

        /// <summary>
        /// Create the groupify dashboard
        /// </summary>
        /// <param name="exportPaths">Paths to read data from</param>
        public void CreateGroupifyReport(IList<string> exportPaths)
        {
            DataTable readyForGroupifyTable = null;
            DataTable blockersTable = null;
            DataTable warningsTable = null;
            DataTable modernUIWarningsTable = null;
            DataTable permissionWarningsTable = null;
            ScanSummary scanSummary = null;

            var outputfolder = ".";
            DateTime dateCreationTime = DateTime.MinValue;
            if (exportPaths.Count == 1)
            {
                outputfolder = new DirectoryInfo(exportPaths[0]).FullName;
                var pathToUse = exportPaths[0].TrimEnd(new char[] { '\\' });
                dateCreationTime = File.GetCreationTime($"{pathToUse}\\{GroupifyCSV}");
            }

            // import the data and "clean" it
            foreach (var path in exportPaths)
            {
                var pathToUse = path.TrimEnd(new char[] {'\\'});

                string csvToLoad = $"{pathToUse}\\{GroupifyCSV}";

                if (!File.Exists(csvToLoad))
                {
                    throw new Exception($"File {csvToLoad} does not exist.");
                }

                Console.WriteLine($"Generating Group Connection Readiness report based upon data coming from {path}");

                using (GenericParserAdapter parser = new GenericParserAdapter(csvToLoad))
                {
                    parser.FirstRowHasHeader = true;
                    parser.MaxBufferSize = 200000;
                    parser.ColumnDelimiter = DetectUsedDelimiter(csvToLoad);

                    // Read the file                    
                    var baseTable = parser.GetDataTable();

                    // Table 1: Ready for Groupify
                    var readyForGroupifyTable1 = baseTable.Copy();
                    // clean table
                    string[] columnsToKeep = new string[] { "SiteUrl", "ReadyForGroupify", "GroupMode", "ModernHomePage", "WebTemplate", "MasterPage", "AlternateCSS", "UserCustomActions", "SubSites", "SubSitesWithBrokenPermissionInheritance", "ModernPageWebFeatureDisabled", "ModernPageFeatureWasEnabledBySPO", "ModernListSiteBlockingFeatureEnabled", "ModernListWebBlockingFeatureEnabled", "SitePublishingFeatureEnabled", "WebPublishingFeatureEnabled", "Everyone(ExceptExternalUsers)Claim", "UsesADGroups", "ExternalSharing" };
                    readyForGroupifyTable1 = DropTableColumns(readyForGroupifyTable1, columnsToKeep);

                    if (readyForGroupifyTable == null)
                    {
                        readyForGroupifyTable = readyForGroupifyTable1;
                    }
                    else
                    {
                        readyForGroupifyTable.Merge(readyForGroupifyTable1);
                    }

                    // Table 2: Groupify blockers
                    var blockersTable1 = baseTable.Copy();
                    // clean table
                    columnsToKeep = new string[] { "SiteUrl", "ReadyForGroupify", "GroupifyBlockers" };
                    blockersTable1 = DropTableColumns(blockersTable1, columnsToKeep);
                    // expand rows
                    blockersTable1 = ExpandRows(blockersTable1, "GroupifyBlockers");

                    if (blockersTable == null)
                    {
                        blockersTable = blockersTable1;
                    }
                    else
                    {
                        blockersTable.Merge(blockersTable1);
                    }

                    // Table 3: Groupify warnings
                    var warningsTable1 = baseTable.Copy();
                    // clean table
                    columnsToKeep = new string[] { "SiteUrl", "ReadyForGroupify", "GroupifyWarnings" };
                    warningsTable1 = DropTableColumns(warningsTable1, columnsToKeep);
                    // expand rows
                    warningsTable1 = ExpandRows(warningsTable1, "GroupifyWarnings");

                    if (warningsTable == null)
                    {
                        warningsTable = warningsTable1;
                    }
                    else
                    {
                        warningsTable.Merge(warningsTable1);
                    }

                    // Table 4: modern ui warnings
                    var modernUIWarningsTable1 = baseTable.Copy();
                    // clean table
                    columnsToKeep = new string[] { "SiteUrl", "ReadyForGroupify", "ModernUIWarnings" };
                    modernUIWarningsTable1 = DropTableColumns(modernUIWarningsTable1, columnsToKeep);
                    // expand rows
                    modernUIWarningsTable1 = ExpandRows(modernUIWarningsTable1, "ModernUIWarnings");

                    if (modernUIWarningsTable == null)
                    {
                        modernUIWarningsTable = modernUIWarningsTable1;
                    }
                    else
                    {
                        modernUIWarningsTable.Merge(modernUIWarningsTable1);
                    }

                    // Table 5: Groupify warnings
                    var permissionWarningsTable1 = baseTable.Copy();
                    // clean table
                    columnsToKeep = new string[] { "SiteUrl", "ReadyForGroupify", "PermissionWarnings" };
                    permissionWarningsTable1 = DropTableColumns(permissionWarningsTable1, columnsToKeep);
                    // expand rows
                    permissionWarningsTable1 = ExpandRows(permissionWarningsTable1, "PermissionWarnings");

                    if (permissionWarningsTable == null)
                    {
                        permissionWarningsTable = permissionWarningsTable1;
                    }
                    else
                    {
                        permissionWarningsTable.Merge(permissionWarningsTable1);
                    }

                    // Read scanner summary data
                    var scanSummary1 = DetectScannerSummary($"{pathToUse}\\{ScannerSummaryCSV}");

                    if (scanSummary == null)
                    {
                        scanSummary = scanSummary1;
                    }
                    else
                    {
                        MergeScanSummaries(scanSummary, scanSummary1);
                    }
                }
            }

            // Get the template Excel file
            using (Stream stream = typeof(Generator).Assembly.GetManifestResourceStream($"SharePoint.Modernization.Scanner.Reports.{GroupifyMasterFile}"))
            {
                if (File.Exists($"{outputfolder}\\{GroupifyMasterFile}"))
                {
                    File.Delete($"{outputfolder}\\{GroupifyMasterFile}");
                }

                using (var fileStream = File.Create($"{outputfolder}\\{GroupifyMasterFile}"))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
            }

            // Push the data to Excel, starting from an Excel template
            using (var excel = new ExcelPackage(new FileInfo($"{outputfolder}\\{GroupifyMasterFile}"), false))
            {
                var dashboardSheet = excel.Workbook.Worksheets["Dashboard"];

                if (scanSummary != null)
                {
                    if (scanSummary.SiteCollections.HasValue)
                    {
                        dashboardSheet.SetValue("U7", scanSummary.SiteCollections.Value);
                    }
                    if (scanSummary.Webs.HasValue)
                    {
                        dashboardSheet.SetValue("W7", scanSummary.Webs.Value);
                    }
                    if (scanSummary.Lists.HasValue)
                    {
                        dashboardSheet.SetValue("Y7", scanSummary.Lists.Value);
                    }
                    if (scanSummary.Duration != null)
                    {
                        dashboardSheet.SetValue("U8", scanSummary.Duration);
                    }
                    if (scanSummary.Version != null)
                    {
                        dashboardSheet.SetValue("U9", scanSummary.Version);
                    }
                }

                if (dateCreationTime > DateTime.Now.Subtract(new TimeSpan(5 * 365, 0, 0, 0, 0)))
                {
                    dashboardSheet.SetValue("U6", dateCreationTime.ToString("G", DateTimeFormatInfo.InvariantInfo));
                }
                else
                {
                    dashboardSheet.SetValue("U6", "-");
                }
                var readyForGroupifySheet = excel.Workbook.Worksheets["ReadyForGroupify"];
                InsertTableData(readyForGroupifySheet.Tables[0], readyForGroupifyTable);

                var blockersSheet = excel.Workbook.Worksheets["Blockers"];
                InsertTableData(blockersSheet.Tables[0], blockersTable);

                var warningsSheet = excel.Workbook.Worksheets["Warnings"];
                InsertTableData(warningsSheet.Tables[0], warningsTable);

                var modernUIWarningsSheet = excel.Workbook.Worksheets["ModernUIWarnings"];
                InsertTableData(modernUIWarningsSheet.Tables[0], modernUIWarningsTable);

                var permissionsWarningsSheet = excel.Workbook.Worksheets["PermissionsWarnings"];
                InsertTableData(permissionsWarningsSheet.Tables[0], permissionWarningsTable);

                // Save the resulting file
                if (File.Exists($"{outputfolder}\\{GroupifyReport}"))
                {
                    File.Delete($"{outputfolder}\\{GroupifyReport}");
                }
                excel.SaveAs(new FileInfo($"{outputfolder}\\{GroupifyReport}"));
            }

            // Clean the template file
            if (File.Exists($"{outputfolder}\\{GroupifyMasterFile}"))
            {
                File.Delete($"{outputfolder}\\{GroupifyMasterFile}");
            }

        }

        #region Helper methods
        private void InsertTableData(ExcelTable table, DataTable data)
        {
            // Insert new table data
            var start = table.Address.Start;
            var body = table.WorkSheet.Cells[start.Row + 1, start.Column];

            var outRange = body.LoadFromDataTable(data, false);

            if (outRange != null)
            {
                // Refresh the table ranges so that Excel understands the current size of the table
                var newRange = string.Format("{0}:{1}", start.Address, outRange.End.Address);
                var tableElement = table.TableXml.DocumentElement;
                tableElement.Attributes["ref"].Value = newRange;
                tableElement["autoFilter"].Attributes["ref"].Value = newRange;
            }
        }

        private ScanSummary MergeScanSummaries(ScanSummary baseSummary, ScanSummary summaryToAdd)
        {
            if (summaryToAdd.SiteCollections.HasValue)
            {
                if (baseSummary.SiteCollections.HasValue)
                {                    
                    baseSummary.SiteCollections = baseSummary.SiteCollections.Value + summaryToAdd.SiteCollections.Value;                    
                }
                else
                {
                    baseSummary.SiteCollections = summaryToAdd.SiteCollections.Value;
                }
            }

            if (summaryToAdd.Webs.HasValue)
            {
                if (baseSummary.Webs.HasValue)
                {
                    baseSummary.Webs = baseSummary.Webs.Value + summaryToAdd.Webs.Value;
                }
                else
                {
                    baseSummary.Webs = summaryToAdd.Webs.Value;
                }
            }

            if (summaryToAdd.Lists.HasValue)
            {
                if (baseSummary.Lists.HasValue)
                {
                    baseSummary.Lists = baseSummary.Lists.Value + summaryToAdd.Lists.Value;
                }
                else
                {
                    baseSummary.Lists = summaryToAdd.Lists.Value;
                }
            }

            baseSummary.Duration = "";

            return baseSummary;
        }

        private ScanSummary DetectScannerSummary(string fileName)
        {
            ScanSummary summary = new ScanSummary();

            try
            {
                if (System.IO.File.Exists(fileName))
                {
                    using (GenericParserAdapter parser = new GenericParserAdapter(fileName))
                    {
                        parser.FirstRowHasHeader = true;
                        parser.ColumnDelimiter = DetectUsedDelimiter(fileName);

                        var baseTable = parser.GetDataTable();
                        List<object> data = new List<object>();
                        if (!string.IsNullOrEmpty(baseTable.Rows[0][0].ToString()) && string.IsNullOrEmpty(baseTable.Rows[0][1].ToString()))
                        {
                            // all might be pushed to first column
                            string[] columns = baseTable.Rows[0][0].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            if (columns.Length < 3)
                            {
                                columns = baseTable.Rows[0][0].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            }

                            foreach (string column in columns)
                            {
                                data.Add(column);
                            }
                        }
                        else
                        {
                            foreach (DataColumn column in baseTable.Columns)
                            {
                                data.Add(baseTable.Rows[0][column]);
                            }
                        }

                        // move the data into the object instance
                        if (int.TryParse(data[0].ToString(), out int sitecollections))
                        {
                            summary.SiteCollections = sitecollections;
                        }
                        if (int.TryParse(data[1].ToString(), out int webs))
                        {
                            summary.Webs = webs;
                        }
                        if (int.TryParse(data[2].ToString(), out int lists))
                        {
                            summary.Lists = lists;
                        }
                        if (!string.IsNullOrEmpty(data[3].ToString()))
                        {
                            summary.Duration = data[3].ToString();
                        }
                        if (!string.IsNullOrEmpty(data[4].ToString()))
                        {
                            summary.Version = data[4].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // eat all exceptions here as this is not critical 
            }

            return summary;
        }

        private DataTable ExpandRows(DataTable table, string column)
        {
            List<DataRow> rowsToAdd = new List<DataRow>();
            foreach (DataRow row in table.Rows)
            {
                if (!string.IsNullOrEmpty(row[column].ToString()) && row[column].ToString().Contains(","))
                {
                    string[] columnToExpand = row[column].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    row[column] = columnToExpand[0];

                    for (int i = 1; i < columnToExpand.Length; i++)
                    {
                        var expandedRow = table.NewRow();
                        expandedRow.ItemArray = row.ItemArray;
                        expandedRow[column] = columnToExpand[i];
                        rowsToAdd.Add(expandedRow);
                    }
                }
            }

            foreach (var row in rowsToAdd)
            {
                table.Rows.Add(row);
            }

            return table;
        }

        private DataTable DropTableColumns(DataTable table, string[] columnsToKeep)
        {
            // Get the columns that we don't need
            List<string> toDelete = new List<string>();
            foreach (DataColumn column in table.Columns)
            {
                if (!columnsToKeep.Contains(column.ColumnName))
                {
                    toDelete.Add(column.ColumnName);
                }
            }

            // Delete the unwanted columns
            foreach (var column in toDelete)
            {
                table.Columns.Remove(column);
            }

            // Verify we have all the needed columns
            int i = 0;
            foreach (var column in columnsToKeep)
            {
                if (table.Columns[i].ColumnName == column)
                {
                    i++;
                }
                else
                {
                    throw new Exception($"Required column {column} does not appear in the provided dataset. Did you use a very old version of the scanner or rename/delete columns in the CSV file?");
                }
            }

            return table;
        }

        private char? DetectUsedDelimiter(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                string line1 = System.IO.File.ReadLines(fileName).First();

                if (line1.IndexOf(',') > 0)
                {
                    return ',';
                }
                else if (line1.IndexOf(';') > 0)
                {
                    return ';';
                }
                else if (line1.IndexOf('|') > 0)
                {
                    return '|';
                }
                else
                {
                    throw new Exception("CSV file delimiter was not detected");
                }
            }
            else
            {
                throw new Exception($"File {fileName} does not exist.");
            }
        }
        #endregion

    }
}
