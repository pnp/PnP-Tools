namespace SharePoint.Scanning.Framework
{
    /// <summary>
    /// Generic class to hold scan errors
    /// </summary>
    public class ScanError: Scan
    {
        public string Error { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
    }
}
