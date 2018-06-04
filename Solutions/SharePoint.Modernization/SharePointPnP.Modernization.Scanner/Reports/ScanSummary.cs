namespace SharePoint.Modernization.Scanner.Reports
{
    public class ScanSummary
    {
        public int? SiteCollections { get; set; }
        public int? Webs { get; set; }
        public int? Lists { get; set; }
        public string Duration { get; set; }
        public string Version { get; set; }
    }
}
