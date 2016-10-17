using PnPAutomationUI.Helpers;
using PnPAutomationUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PnPAutomationUI.Controllers
{
    public class TestcaseController : Controller
    {
        // GET: Testcase
        public ActionResult Details(int Id,string branch)
        {
            using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
            {
                Testcase TestResults = null;

                IEnumerable<Testcase> testResultSets = from testreultset in dc.TestResultSets
                               join testrunset in dc.TestRunSets on testreultset.TestRunId equals testrunset.Id
                               join configset in dc.TestConfigurationSets on testrunset.TestConfigurationId equals configset.Id
                               where testreultset.Id == Id
                               select new Testcase
                               {
                                   AnonymousAccess = configset.AnonymousAccess,
                                   Outcome = testreultset.Outcome,
                                   Error = testreultset.ErrorMessage,
                                   StackTrace = testreultset.ErrorStackTrace,
                                   TestDuration = testreultset.Duration,
                                   TestCasename = testreultset.TestCaseName,
                                   Id = testreultset.Id,
                                   Testdate = testreultset.StartTime
                               };


                if (!Request.IsAuthenticated || !AuthorizationManager.IsCoreTeamMember(User))
                {
                    TestResults = testResultSets.Where(c => c.AnonymousAccess == true).SingleOrDefault();
                }
                else
                {
                    TestResults = testResultSets.SingleOrDefault();
                }

                if (TestResults != null)
                {
                    List<Testcase> testcaseConsoleOutput = (from resultmessage in dc.TestResultMessageSets
                                                            where resultmessage.TestResultId == Id
                                                            select new Testcase
                                                            {
                                                                ConsoleOutPut = resultmessage.Text
                                                            }).ToList();
                    string consoleoutput = string.Empty;
                    if (testcaseConsoleOutput != null && testcaseConsoleOutput.Any())
                    {
                        foreach (Testcase consoleOutPutText in testcaseConsoleOutput)
                        {
                            consoleoutput += consoleOutPutText.ConsoleOutPut + "\n";
                        }
                    }

                    TestResults.ConsoleOutPut = consoleoutput;

                    TestResults.Branch = String.Format(Constants.Constants.gitBranch, branch);
                }
                return View(TestResults);
            }
        }


    }
}