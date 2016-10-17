using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnPTestResultsNotificationJob.Models
{
    public class TestSummary
    {
        public int Outcome { get; set; }
        public string Configuration { get; set; }
        public string TestCasename { get; set; }
        public string TestDuration { get; set; }
        public string Error { get; set; }
        public string StackTrace { get; set; }
        public DateTime Testdate { get; set; }
        public int Id { get; set; }
        public string Branch { get; set; }
        public int TestRunId { get; set; }
    }
    public class TestSummarySet
    {
        public TestSummary testsummary { get; set; }
        public List<TestSummary> passedtests { get; set; }
        public List<TestSummary> skippedtests { get; set; }
        public List<TestSummary> failedtests { get; set; }
        public List<TestSummary> notfoundtests { get; set; }
    }
}
