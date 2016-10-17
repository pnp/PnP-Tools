using PnPAutomationUI.Helpers;
using PnPAutomationUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PnPAutomationUI.Controllers
{
    public class TestDetailsController : Controller
    {
        // GET: TestDetails
        public ActionResult Overview(int Id)
        {
            TestDetailsSet model = new TestDetailsSet();
            TestDetails detailedReport = null;
            using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
            {
                IEnumerable<TestDetails> testDetails = from configset in dc.TestConfigurationSets
                                      join testrunsets in dc.TestRunSets on configset.Id equals testrunsets.TestConfigurationId
                                      join testauthenticationset in dc.TestAuthenticationSets on configset.TestAuthentication_Id equals testauthenticationset.Id
                                      where testrunsets.Id == Id
                                      select new TestDetails
                                      {
                                          ConfiguratioName = configset.Name,
                                          Description = configset.Description,
                                          VSBuildConfiguration = configset.VSBuildConfiguration,
                                          AnonymousAccess = configset.AnonymousAccess,
                                          GithubBranch = configset.Branch,
                                          PlatformType = configset.Type,
                                          Build = testrunsets.Build,
                                          Testdate = testrunsets.TestDate,
                                          TestDuration = testrunsets.TestTime,
                                          Status = testrunsets.Status,
                                          Passed = (testrunsets.TestsPassed != null ? testrunsets.TestsPassed : 0),
                                          Skipped = (testrunsets.TestsSkipped != null ? testrunsets.TestsSkipped : 0),
                                          Failed = (testrunsets.TestsFailed != null ? testrunsets.TestsFailed : 0),
                                          Tests = ((testrunsets.TestsPassed != null ? testrunsets.TestsPassed : 0)
                                               + (testrunsets.TestsSkipped != null ? testrunsets.TestsSkipped : 0)
                                               + (testrunsets.TestsFailed != null ? testrunsets.TestsFailed : 0)),
                                          TestRunId = Id,
                                          ConfigurationId = testrunsets.TestConfigurationId
                                      };

                if (!Request.IsAuthenticated || !AuthorizationManager.IsCoreTeamMember(User))
                {
                    detailedReport = testDetails.Where(t => t.AnonymousAccess == true).SingleOrDefault();
                }
                else
                {
                    detailedReport = testDetails.SingleOrDefault();
                }

                if (detailedReport != null)
                {
                    var computerName = (from testrunset in dc.TestResultSets
                                        where testrunset.TestRunId == Id
                                        select testrunset.ComputerName).FirstOrDefault();

                    int noOfmonths = Convert.ToInt32(ConfigurationManager.AppSettings["Graphdatatoshow"]);
                    DateTime fromDate = DateTime.Today.AddMonths(-noOfmonths);
                    DateTime toDate = DateTime.Today;

                    List<GraphData> detailedReportListGraph = (from testrunsets in dc.TestRunSets
                                                                 where ((testrunsets.TestDate >= fromDate && testrunsets.TestDate <= toDate)
                                                                 && (testrunsets.TestConfigurationId == detailedReport.ConfigurationId))
                                                                 select new GraphData
                                                                 {
                                                                     Testdate = testrunsets.TestDate,
                                                                     Passed = (testrunsets.TestsPassed != null ? testrunsets.TestsPassed : 0),
                                                                     Failed = (testrunsets.TestsFailed != null ? testrunsets.TestsFailed : 0),
                                                                     TestRunID=testrunsets.Id                                                                 
                                                                 }).ToList();

                    EnvironmentType s = (EnvironmentType)Enum.Parse(typeof(EnvironmentType), Convert.ToString(detailedReport.PlatformType));
                    detailedReport.Platform = Convert.ToString(s);
                    detailedReport.Computer = computerName;
                    model.detailedReport = detailedReport;
                    model.rootSiteURL = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
                    model.detailedConfigurationReport = detailedReportListGraph;
                }
            }

            return View(model);
        }
    }
}