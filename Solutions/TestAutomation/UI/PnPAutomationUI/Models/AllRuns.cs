using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PnPAutomationUI.Models
{
    public class AllRuns
    {
        public int Status { get; set; }
        public string ConfiguratioName { get; set; }
        public string GithubBranch { get; set; }
        public string AppOnly { get; set; }
        public string Environment { get; set; }
        public DateTime Testdate { get; set; }
        public string TestDuration { get; set; }
        public Nullable<int> Tests { get; set; }
        public Nullable<int> Passed { get; set; }
        public Nullable<int> Skipped { get; set; }
        public Nullable<int> Failed { get; set; }
        public bool Log { get; set; }
        public int TestRunSetId { get; set; }
        public int Count { get; set; }

        public int CId { get; set; }
        public string CategoryName { get; set; }
    }
    public enum EnvironmentType
    {
        SharepointOnline = 0,
        SharePoint2013 = 1,
        SharePoint2016 = 2
    }
    public enum AppOnly
    {
        Credentials = 0,
        AppOnly = 1
    }
    public class AllRunsSet
    {
        public List<AllRuns> pnptestresults { get; set; }

        [DisplayName("From")]
        public DateTime FromDate { get; set; }

        [DisplayName("To")]
        public DateTime ToDate { get; set; }

        public int TestResultsCount { get; set; }
        public IList<ConfigurationList> TestConfigurations { get; set; }
        public int[] SelectedConfigurations { get; set; }

        public IList<CategoryList> TestCategory { get; set; }
        public int[] SelectedCategory { get; set; }

    }
}