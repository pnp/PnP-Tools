using GenericParsing;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
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
        // Page report variables
        private const string PageCSV = "PageScanResults.csv";
        private const string PageMasterFile = "pagemaster.xlsx";
        private const string PageReport = "Office 365 Page Transformation Readiness.xlsx";

        public void CreatePageReport(IList<string> exportPaths)
        {
            DataTable readyForPageTransformationTable = null;
            DataTable unmappedWebPartsTable = null;
            ScanSummary scanSummary = null;


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
                if (File.Exists(PageMasterFile))
                {
                    File.Delete(PageMasterFile);
                }

                using (var fileStream = File.Create(PageMasterFile))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
            }

            // Push the data to Excel, starting from an Excel template
            //using (var excel = new ExcelPackage(new FileInfo(PageMasterFile)))
            using (var excel = new ExcelPackage(new FileInfo(PageMasterFile), false))
            {

                if (scanSummary != null)
                {
                    var dashboardSheet = excel.Workbook.Worksheets["Dashboard"];
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

                //var readyForPageTransformationSheet = excel.Workbook.Worksheets.Add("ReadyForPageTransformation");
                //var insertedRange = readyForPageTransformationSheet.Cells["A1"].LoadFromDataTable(readyForPageTransformationTable, true);

                //var unmappedWebPartsSheet = excel.Workbook.Worksheets.Add("UnmappedWebParts");
                //insertedRange = unmappedWebPartsSheet.Cells["A1"].LoadFromDataTable(unmappedWebPartsTable, true);

                var readyForPageTransformationSheet = excel.Workbook.Worksheets["ReadyForPageTransformation"];
                InsertTableData(readyForPageTransformationSheet.Tables[0], readyForPageTransformationTable);

                var unmappedWebPartsSheet = excel.Workbook.Worksheets["UnmappedWebParts"];
                InsertTableData(unmappedWebPartsSheet.Tables[0], unmappedWebPartsTable);

                // Save the resulting file
                if (File.Exists(PageReport))
                {
                    File.Delete(PageReport);
                }
                excel.SaveAs(new FileInfo(PageReport));
            }

            // Clean the template file
            if (File.Exists(PageMasterFile))
            {
                File.Delete(PageMasterFile);
            }
        }

        public void CreateGroupifyReport(IList<string> exportPaths)
        {
            DataTable readyForGroupifyTable = null;
            DataTable blockersTable = null;
            DataTable warningsTable = null;
            DataTable modernUIWarningsTable = null;
            DataTable permissionWarningsTable = null;
            ScanSummary scanSummary = null;

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
                if (File.Exists(GroupifyMasterFile))
                {
                    File.Delete(GroupifyMasterFile);
                }

                using (var fileStream = File.Create(GroupifyMasterFile))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
            }

            // Push the data to Excel, starting from an Excel template
            using (var excel = new ExcelPackage(new FileInfo(GroupifyMasterFile), false))
            {
                if (scanSummary != null)
                {
                    var dashboardSheet = excel.Workbook.Worksheets["Dashboard"];
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
                if (File.Exists(GroupifyReport))
                {
                    File.Delete(GroupifyReport);
                }
                excel.SaveAs(new FileInfo(GroupifyReport));
            }

            // Clean the template file
            if (File.Exists(GroupifyMasterFile))
            {
                File.Delete(GroupifyMasterFile);
            }

        }

        #region Helper methods
        private void InsertTableData(ExcelTable table, DataTable data)
        {
            // Insert new table data
            var start = table.Address.Start;
            var body = table.WorkSheet.Cells[start.Row + 1, start.Column];

            var outRange = body.LoadFromDataTable(data, false);

            // Refresh the table ranges so that Excel understands the current size of the table
            var newRange = string.Format("{0}:{1}", start.Address, outRange.End.Address);
            var tableElement = table.TableXml.DocumentElement;
            tableElement.Attributes["ref"].Value = newRange;
            tableElement["autoFilter"].Attributes["ref"].Value = newRange;
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
