using GenericParsing;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.UIExperience.Scanner.Reports
{
    public class Generator
    {
        private const string ScannerSummaryCSV = "ScannerSummary.csv";
        // List readiness report variables
        private const string ListCSV = "ModernListBlocked.csv";
        private const string ListMasterFile = "listmaster.xlsx";
        private const string ListReport = "Modern UI List Readiness.xlsx";

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
            using (Stream stream = typeof(Generator).Assembly.GetManifestResourceStream($"SharePoint.UIExperience.Scanner.Reports.{ListMasterFile}"))
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
