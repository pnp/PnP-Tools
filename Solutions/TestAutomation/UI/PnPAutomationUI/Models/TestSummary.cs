using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PnPAutomationUI.Models
{
    public class TestSummary
    {
        public int Outcome { get; set; }
        public string TestCasename { get; set; }
        public Nullable<System.TimeSpan> TestDuration { get; set; }
        public string Error { get; set; }
        public string StackTrace { get; set; }
        public DateTimeOffset Testdate { get; set; }
        public int Id { get; set; }
        public string Branch { get; set; }
    }
    public class TestSummarySet
    {
        public TestSummary testsummary { get; set; }
        public List<TestSummary> passedtests { get; set; }
        public List<TestSummary> skippedtests { get; set; }
        public List<TestSummary> failedtests { get; set; }
        public List<TestSummary> notfoundtests { get; set; }
        public TestSummaryConfigDetails testConfigDetails { get; set; }

    }
    public class TestSummaryConfigDetails
    {
        public string ConfiguratioName { get; set; }
        public DateTime Testdate { get; set; }
    }
}