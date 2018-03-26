using SharePoint.Scanning.Framework;

namespace SharePoint.Visio.Scanner.Results
{
    public class VdwScanResult: Scan
    {
        public string FileExtension { get; set; }
        public string OriginalPath { get; set; }
    }
}
