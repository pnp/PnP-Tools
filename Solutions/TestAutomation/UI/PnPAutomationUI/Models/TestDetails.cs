using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PnPAutomationUI.Models
{
    public class TestDetails
    {
        public string ConfiguratioName { get; set; }
        public string Computer { get; set; }
        public string Description { get; set; }
        public string VSBuildConfiguration { get; set; }
        public string GithubBranch { get; set; }
        public int PlatformType { get; set; }
        public string Platform { get; set; }
        public string Build { get; set; }
        public DateTime Testdate { get; set; }
        public Nullable<System.TimeSpan> TestDuration { get; set; }
        public int Status { get; set; }
        public Nullable<int> Tests { get; set; }
        public Nullable<int> Passed { get; set; }
        public Nullable<int> Skipped { get; set; }
        public Nullable<int> Failed { get; set; }
        public Nullable<int> NotFound { get; set; }
        public bool Aborted { get; set; }
        public int TestRunId { get; set; }
        public int ConfigurationId { get; set; }

        public bool AnonymousAccess { get; set; }
    }
    public class TestDetailsSet
    {
        public TestDetails detailedReport { get; set; }
        public List<GraphData> detailedConfigurationReport { get; set; }
        public string rootSiteURL { get; set; }
    }

    public class GraphData
    {
        public DateTime Testdate { get; set; }
        public Nullable<int> Passed { get; set; }
        public Nullable<int> Failed { get; set; }
        public int TestRunID { get; set; }
    }
}