using PnPAutomationUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Optimization;
using System.Web.Mvc;
using System.Data.Entity;
using PnPAutomationUI.Helpers;

namespace PnPAutomationUI.Controllers
{
    public class TestRunsController : Controller
    {
        // GET: TestRuns
        [HttpGet]
        public ActionResult AllRuns(DateTime? fromDate, DateTime? toDate, string ConfigurationId, int page = 1, bool isPrevSelected = false, bool isNextSelected = false)
        {
            int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString());

            if (fromDate == null && toDate == null)
            {
                fromDate = DateTime.Today.AddDays(-7);
                toDate = DateTime.Today;
            }

            int[] configuration = null;
            if (ConfigurationId != null)
            {
                configuration = StringToIntArray(ConfigurationId);                
            }

            var model = GetTestRunDetails(fromDate, toDate, configuration, page, PageSize);
            if (configuration != null)
            {
                model.SelectedConfigurations = configuration;
            }

            ViewBag.CurrentPage = page;
            ViewBag.PrevPage = page;
            if (page > 1)
            {
                ViewBag.PrevPage = page - 1;
            }
            ViewBag.NextPage = page + 1;
            ViewBag.PrevSelected = isPrevSelected;
            ViewBag.NextSelected = isNextSelected;
            ViewBag.PageSize = PageSize;

            if (model.TestResultsCount > 0)
            {
                ViewBag.TotalPages = (int)Math.Ceiling((double)model.TestResultsCount / PageSize);
            }

            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Error(string message)
        {
            throw new Exception(message);
        }
        private static int[] StringToIntArray(string myNumbers)
        {
            List<int> myIntegers = new List<int>();
            Array.ForEach(myNumbers.Split(",".ToCharArray()), s =>
            {
                int currentInt;
                if (Int32.TryParse(s, out currentInt))
                    myIntegers.Add(currentInt);
            });
            return myIntegers.ToArray();
        }
       
        public ActionResult PrintLog(int Id)
        {
            GenerateLog TestResults = null;

            using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
            {
                if (Request.IsAuthenticated && AuthorizationManager.IsCoreTeamMember(User))
                {
                    TestResults = (from testrunsets in dc.TestRunSets
                                   where testrunsets.Id == Id
                                   select new GenerateLog
                                   {
                                       Log = testrunsets.MSBuildLog
                                   }).SingleOrDefault();
                }
                else
                {
                    TestResults = (from testrunsets in dc.TestRunSets
                                   join configset in dc.TestConfigurationSets on testrunsets.TestConfigurationId equals configset.Id
                                   where testrunsets.Id == Id && configset.AnonymousAccess == true
                                   select new GenerateLog
                                   {
                                       Log = testrunsets.MSBuildLog
                                   }).SingleOrDefault();
                }
            }

            return View(TestResults);
        }
        public static DateTime Trunc(DateTime D)
        {
            return new DateTime(D.Ticks - (D.Ticks % TimeSpan.TicksPerDay));
        }

        public AllRunsSet GetTestRunDetails(DateTime? fromDate, DateTime? toDate, int[] configIDs, int page, int PageSize)
        {
            AllRunsSet model = new AllRunsSet();
            model.FromDate = Convert.ToDateTime(fromDate);
            model.ToDate = Convert.ToDateTime(toDate);

            using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
            {
                var configDetails = from configset in dc.TestConfigurationSets
                                    select new
                                    {
                                        configset.Id,
                                        configset.Name,
                                        configset.Type,
                                        configset.AnonymousAccess,
                                        configset.TestAuthentication_Id,
                                        configset.Branch
                                    };

                if (!Request.IsAuthenticated || !AuthorizationManager.IsCoreTeamMember(User))
                {
                    configDetails = configDetails.Where(c => c.AnonymousAccess == true);
                }
                List<ConfigurationList> testConfigurations = new List<ConfigurationList>();
                foreach (var config in configDetails.ToList())
                {
                    testConfigurations.Add(new ConfigurationList { ID = config.Id, Name = config.Name });
                }

                if (configIDs != null && configIDs.Any())
                {
                    configDetails = configDetails.Where(c => configIDs.Contains(c.Id));
                }


                var testResults = (from configset in configDetails
                                   join authenticationset in dc.TestAuthenticationSets on configset.TestAuthentication_Id equals authenticationset.Id
                                   join testrunsets in dc.TestRunSets on configset.Id equals testrunsets.TestConfigurationId
                                   where DbFunctions.TruncateTime(testrunsets.TestDate) >= DbFunctions.TruncateTime(fromDate)
                                   && DbFunctions.TruncateTime(testrunsets.TestDate) <= DbFunctions.TruncateTime(toDate)
                                   select new
                                   {
                                       Status = testrunsets.Status,
                                       Testdate = testrunsets.TestDate,
                                       TestDuration = testrunsets.TestTime,
                                       Passed = testrunsets.TestsPassed,
                                       Skipped = testrunsets.TestsSkipped,
                                       Failed = testrunsets.TestsFailed,
                                       Log = (testrunsets.MSBuildLog != null ? true : false),
                                       TestRunSetId = testrunsets.Id,
                                       CId = testrunsets.TestConfigurationId,
                                       cName = configset.Name,
                                       appOnly = authenticationset.AppOnly,
                                       cType = configset.Type,
                                       GithubBranch=configset.Branch
                                   }).OrderByDescending(t => t.Testdate).Skip((page - 1) * PageSize).Take(PageSize).ToList();

                List<AllRuns> configTestRuns = (from testrunsets in testResults
                                                select new AllRuns
                                                {
                                                    Status = testrunsets.Status,
                                                    ConfiguratioName = testrunsets.cName,
                                                    GithubBranch=testrunsets.GithubBranch,
                                                    AppOnly = Enum.GetName(typeof(AppOnly), testrunsets.appOnly),
                                                    Environment = Enum.GetName(typeof(EnvironmentType), testrunsets.cType),
                                                    Testdate = testrunsets.Testdate,
                                                    TestDuration = (testrunsets.TestDuration != null ? Convert.ToString(testrunsets.TestDuration).Remove(Convert.ToString(testrunsets.TestDuration).IndexOf('.')) : "N/A"),
                                                    Passed = (testrunsets.Passed != null ? testrunsets.Passed : 0),
                                                    Skipped = (testrunsets.Skipped != null ? testrunsets.Skipped : 0),
                                                    Failed = (testrunsets.Failed != null ? testrunsets.Failed : 0),
                                                    Log = testrunsets.Log,
                                                    TestRunSetId = testrunsets.TestRunSetId,
                                                    Tests = ((testrunsets.Passed != null ? testrunsets.Passed : 0)
                                                      + (testrunsets.Skipped != null ? testrunsets.Skipped : 0)
                                                      + (testrunsets.Failed != null ? testrunsets.Failed : 0))
                                                }).OrderByDescending(d => d.Testdate.Date).ThenByDescending(t => t.Testdate.TimeOfDay).ToList();

                model.pnptestresults = configTestRuns;
                model.TestConfigurations = testConfigurations;

                model.TestResultsCount = (from testrunsets in dc.TestRunSets
                                          join config in configDetails on testrunsets.TestConfigurationId equals config.Id
                                          where DbFunctions.TruncateTime(testrunsets.TestDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(testrunsets.TestDate) <= DbFunctions.TruncateTime(toDate)
                                          select new { testrunsets.Id }).ToList().Count;

            }

            return model;
        }
    }
}