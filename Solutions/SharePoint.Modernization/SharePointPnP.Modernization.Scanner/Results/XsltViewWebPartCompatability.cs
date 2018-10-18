namespace SharePoint.Modernization.Scanner.Results
{
    public class XsltViewWebPartCompatibility
    {
        public bool BlockedByJSLink { get; set; }
        public string JSLink { get; set; }
        public bool BlockedByXslLink { get; set; }
        public string XslLink { get; set; }
        public bool BlockedByXsl { get; set; }
        public bool BlockedByJSLinkField { get; set; }
        public string JSLinkFields { get; set; }
        public bool BlockedByBusinessDataField { get; set; }
        public string BusinessDataFields { get; set; }
        public bool BlockedByTaskOutcomeField { get; set; }
        public string TaskOutcomeFields { get; set; }
        public bool BlockedByPublishingField { get; set; }
        public string PublishingFields { get; set; }
        public bool BlockedByGeoLocationField { get; set; }
        public string GeoLocationFields { get; set; }
        public bool BlockedByListCustomAction { get; set; }
        public string ListCustomActions { get; set; }
        public bool BlockedByManagedMetadataNavFeature { get; set; }
        public bool BlockedByViewType { get; set; }
        public string ViewType { get; set; }
        public bool BlockedByListBaseTemplate { get; set; }
        public int ListBaseTemplate { get; set; }
    }
}
