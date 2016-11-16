using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PnPAutomationUI.Models
{
    public class Testcase
    {
        public int Outcome { get; set; }
        public string TestCasename { get; set; }
        public Nullable<System.TimeSpan> TestDuration { get; set; }
        public string Error { get; set; }
        public string StackTrace { get; set; }
        public DateTimeOffset Testdate { get; set; }
        public int Id { get; set; }
        public string Branch { get; set; }
        public bool AnonymousAccess { get; set; }
        public string ConsoleOutPut { get; set; }
    }
}