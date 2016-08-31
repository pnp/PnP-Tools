using System;

namespace SharePoint.SandBoxTool
{
    /// <summary>
    /// Object holding scan results
    /// </summary>
    public class SBScanResult
    {
        public string SiteURL { get; set; }
        public string SiteOwner { get; set; }
        public string WSPName { get; set; }
        public string Author { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Activated { get; set; }
        public bool HasAssemblies { get; set; }
        public string SolutionHash { get; set; }
        public string SiteId { get; set; }
        public bool? IsEmptyAssembly { get; set; }
        public bool? IsInfoPath { get; set; }
        public bool? IsEmptyInfoPathAssembly { get; set; }
        public bool? HasWebParts { get; set; }
        public bool? HasWebTemplate { get; set; }
        public bool? HasFeatureReceivers { get; set; }
        public bool? HasEventReceivers { get; set; }
        public bool? HasListDefinition { get; set; }
        public bool? HasWorkflowAction { get; set; }
    }
}
