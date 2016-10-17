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
    public class TestsummaryController : Controller
    {
        // GET: Testsummary
        public ActionResult AllTests(int Id, int OutCome, string branch, int page = 1, bool isPrevSelected = false, bool isNextSelected = false)
        {
            using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
            {
                TestSummarySet model = new TestSummarySet();
                int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString());
                List<TestSummary> TestResultsSummary = null;
                int TotalPages = 0;
                string searchTxt = string.Empty;

                model.passedtests = getTestresults(Id, 0, branch, searchTxt, page, PageSize, out TestResultsSummary, out TotalPages);
                ViewBag.PassedTotalPages = (int)Math.Ceiling((double)TotalPages / PageSize);

                model.skippedtests = getTestresults(Id, 1, branch, searchTxt, page, PageSize, out TestResultsSummary, out TotalPages);
                ViewBag.SkippedTotalPages = (int)Math.Ceiling((double)TotalPages / PageSize);

                model.failedtests = getTestresults(Id, 2, branch, searchTxt, page, PageSize, out TestResultsSummary, out TotalPages);
                ViewBag.FailedTotalPages = (int)Math.Ceiling((double)TotalPages / PageSize);

                model.notfoundtests = getTestresults(Id, 3, branch, searchTxt, page, PageSize, out TestResultsSummary, out TotalPages);
                ViewBag.NotfoundTotalPages = (int)Math.Ceiling((double)TotalPages / PageSize);

                SetViewBagProperties(page, PageSize, Id, OutCome, branch, isPrevSelected, isNextSelected, searchTxt);

                model.testConfigDetails = getTesummaryConfigDetails(Id);
                return View(model);
            }
        }

        public void SetViewBagProperties(int page, int PageSize, int Id, int OutCome, string branch, bool isPrevSelected, bool isNextSelected, string searchTxt)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = PageSize;
            ViewBag.Id = Id;
            ViewBag.Outcome = OutCome;
            ViewBag.Branch = branch;

            ViewBag.PrevPage = page;
            if (page > 1)
            {
                ViewBag.PrevPage = page - 1;
            }
            ViewBag.NextPage = page + 1;
            ViewBag.PrevSelected = isPrevSelected;
            ViewBag.NextSelected = isNextSelected;
            ViewBag.TestCaseSeacrhString = searchTxt;
        }
        public ActionResult AllTestsPassed(int Id, int OutCome, string branch, string searchTestcaseString, int page = 1
            , bool isPrevSelected = false, bool isNextSelected = false)
        {
            using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
            {

                List<TestSummary> TestResultsSummary = null;
                int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString());
                int TotalPages = 0;

                getTestresults(Id, 0, branch, searchTestcaseString, page, PageSize, out TestResultsSummary, out TotalPages);
                SetViewBagProperties(page, PageSize, Id, OutCome, branch, isPrevSelected, isNextSelected, searchTestcaseString);
                ViewBag.PassedTotalPages = (int)Math.Ceiling((double)TotalPages / PageSize);
                ViewBag.TestCaseSeacrhString = searchTestcaseString;
                return PartialView("AllTestsPassed", TestResultsSummary);
            }
        }

        public ActionResult AllTestsFailed(int Id, int OutCome, string branch, string searchTestcaseString, int page = 1
            , bool isPrevSelected = false, bool isNextSelected = false)
        {
            using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
            {
                List<TestSummary> TestResultsSummary = null;
                int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString()); ;
                int TotalPages = 0;

                getTestresults(Id, 2, branch, searchTestcaseString, page, PageSize, out TestResultsSummary, out TotalPages);
                SetViewBagProperties(page, PageSize, Id, OutCome, branch, isPrevSelected, isNextSelected, searchTestcaseString);
                ViewBag.FailedTotalPages = (int)Math.Ceiling((double)TotalPages / PageSize);
                ViewBag.TestCaseSeacrhString = searchTestcaseString;

                return PartialView("AllTestsFailed", TestResultsSummary);
            }
        }

        public ActionResult AllTestsNotFound(int Id, int OutCome, string branch, string searchTestcaseString, int page = 1
            , bool isPrevSelected = false, bool isNextSelected = false)
        {
            using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
            {

                List<TestSummary> TestResultsSummary = null;
                int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString()); ;
                int TotalPages = 0;

                getTestresults(Id, 3, branch, searchTestcaseString, page, PageSize, out TestResultsSummary, out TotalPages);
                SetViewBagProperties(page, PageSize, Id, OutCome, branch, isPrevSelected, isNextSelected, searchTestcaseString);
                ViewBag.NotfoundTotalPages = (int)Math.Ceiling((double)TotalPages / PageSize);
                ViewBag.TestCaseSeacrhString = searchTestcaseString;

                return PartialView("AllTestsNotFound", TestResultsSummary);
            }
        }

        public ActionResult AllTestsSkipped(int Id, int OutCome, string branch, string searchTestcaseString, int page = 1
            , bool isPrevSelected = false, bool isNextSelected = false)
        {
            using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
            {
                List<TestSummary> TestResultsSummary = null;
                int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString()); ;
                int TotalPages = 0;

                getTestresults(Id, 1, branch, searchTestcaseString, page, PageSize, out TestResultsSummary, out TotalPages);
                SetViewBagProperties(page, PageSize, Id, OutCome, branch, isPrevSelected, isNextSelected, searchTestcaseString);
                ViewBag.SkippedTotalPages = (int)Math.Ceiling((double)TotalPages / PageSize);
                ViewBag.TestCaseSeacrhString = searchTestcaseString;

                return PartialView("AllTestsSkipped", TestResultsSummary);
            }
        }
        public List<TestSummary> getTestresults(int Id, int outcome, string branch, string searchTestcaseString, int page, int PageSize, out List<TestSummary> TestResults, out int TotalPages)
        {
            using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
            {
                TestResults = new List<TestSummary>();
                TotalPages = 0;

                var configDetails = from configset in dc.TestConfigurationSets
                                    select new
                                    {
                                        configset.Id,
                                        configset.AnonymousAccess
                                    };

                if (!Request.IsAuthenticated || !AuthorizationManager.IsCoreTeamMember(User))
                {
                    configDetails = configDetails.Where(c => c.AnonymousAccess == true);
                }

                List<TestSummary> data = (from testreultset in dc.TestResultSets
                                          join testrunset in dc.TestRunSets on testreultset.TestRunId equals testrunset.Id
                                          join configset in configDetails on testrunset.TestConfigurationId equals configset.Id
                                          where testreultset.TestRunId == Id && testreultset.Outcome == outcome
                                          && ((!string.IsNullOrEmpty(searchTestcaseString) ? testreultset.TestCaseName.Contains(searchTestcaseString) : true))
                                          select new TestSummary
                                          {
                                              Outcome = testreultset.Outcome,
                                              Error = testreultset.ErrorMessage,
                                              StackTrace = testreultset.ErrorStackTrace,
                                              TestDuration = testreultset.Duration,
                                              TestCasename = testreultset.TestCaseName,
                                              Id = testreultset.Id,
                                              Branch = branch
                                          }).OrderByDescending(d => d.Id).Skip((page - 1) * PageSize).Take(PageSize).ToList();

                if (data != null && data.Any())
                {
                    var totaltests = (from testreultset in dc.TestResultSets
                                      where testreultset.TestRunId == Id && testreultset.Outcome == outcome && (!string.IsNullOrEmpty(searchTestcaseString) ? testreultset.TestCaseName.Contains(searchTestcaseString) : true)
                                      select new { testreultset }).ToList();
                    if (totaltests != null && totaltests.Any())
                        TotalPages = totaltests.Count;

                    TestResults = (from testreult in data
                                   select new TestSummary
                                   {
                                       Outcome = testreult.Outcome,
                                       Error = testreult.Error,
                                       StackTrace = testreult.StackTrace,
                                       TestDuration = testreult.TestDuration,
                                       TestCasename = testreult.TestCasename,
                                       Id = testreult.Id,
                                       Branch = branch
                                   }).OrderByDescending(t => t.Id).ToList();
                }
                return TestResults;

            }
        }
        public TestSummaryConfigDetails getTesummaryConfigDetails(int TestRunId)
        {
            using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
            {
                TestSummaryConfigDetails TestConfigDetails = new TestSummaryConfigDetails();


                var configDetails = from configset in dc.TestConfigurationSets
                                    select new
                                    {
                                        configset.Id,
                                        configset.Name,
                                        configset.AnonymousAccess
                                    };

                if (!Request.IsAuthenticated || !AuthorizationManager.IsCoreTeamMember(User))
                {
                    configDetails = configDetails.Where(c => c.AnonymousAccess == true);
                }


                TestConfigDetails = (from testreultset in dc.TestResultSets
                                     join testrunset in dc.TestRunSets on testreultset.TestRunId equals testrunset.Id
                                     join configset in configDetails on testrunset.TestConfigurationId equals configset.Id
                                     where testreultset.TestRunId == TestRunId
                                     select new TestSummaryConfigDetails
                                     {
                                         ConfiguratioName = configset.Name,
                                         Testdate = testrunset.TestDate
                                     }).FirstOrDefault();
                return TestConfigDetails;

            }
        }

    }
}