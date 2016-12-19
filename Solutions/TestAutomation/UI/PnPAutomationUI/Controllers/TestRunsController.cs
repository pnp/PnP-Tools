using PnPAutomationUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Optimization;
using System.Web.Mvc;
using System.Data.Entity;
using PnPAutomationUI.Helpers;
using System.Web;
using System.Text;

namespace PnPAutomationUI.Controllers
{
    public class TestRunsController : Controller
    {
        // GET: TestRuns
        [HttpGet]
        public ActionResult AllRuns(DateTime? fromDate, DateTime? toDate, string ConfigurationId, string CategoryId, int page = 1, bool isPrevSelected = false, bool isNextSelected = false)
        {
            HttpCookie CategorycookieValue = HttpContext.Request.Cookies.Get("Category");
            HttpCookie ConfigurationcookieValue = HttpContext.Request.Cookies.Get("Configuration");
            string[] DefaultCategories = ConfigurationManager.AppSettings["DefaultCategories"].Split(",".ToCharArray());
            int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"].ToString());
                      
            int[] category = null;
            if (CategoryId != null)
            {
                category = StringToIntArray(CategoryId);
            }
            else if (CategorycookieValue != null)
            {
                category = StringToIntArray(CategorycookieValue.Value);
            }
            else if (DefaultCategories != null)
            {
                category = Array.ConvertAll(DefaultCategories, s => int.Parse(s)); ;
            }

            int[] configuration = null;
            if (ConfigurationId != null)
            {
                configuration = StringToIntArray(ConfigurationId);
            }
            else if (ConfigurationcookieValue != null)
            {
                configuration = StringToIntArray(ConfigurationcookieValue.Value);
            }
            if (fromDate == null && toDate == null)
            {
                fromDate = DateTime.Today.AddDays(-7);
                toDate = DateTime.Today;
            }

            var model = GetTestRunDetails(fromDate, toDate, configuration, category, page, PageSize);
            if (configuration != null)
            {
                model.SelectedConfigurations = configuration;
            }
            if (category != null)
            {
                model.SelectedCategory = category;
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
            if (category != null && configuration != null)
            {
                HttpCookie Categorycookie = new HttpCookie("Category");
                HttpCookie Configurationcookie = new HttpCookie("Configuration");
                Categorycookie.Value = string.Join(",", category);
                Configurationcookie.Value = string.Join(",", configuration);
                HttpContext.Response.Cookies.Remove("Category");
                HttpContext.Response.Cookies.Remove("Configuration");

                int cookieExpire = Convert.ToInt32(ConfigurationManager.AppSettings["CookieExpireInDays"].ToString());
                Categorycookie.Expires = DateTime.Now.AddDays(cookieExpire);
                Configurationcookie.Expires = DateTime.Now.AddDays(cookieExpire);
                HttpContext.Response.SetCookie(Categorycookie);
                HttpContext.Response.SetCookie(Configurationcookie);
            }

            return View(model);
        }
        
        [HttpPost]
        public string PopulateConfigurationDetails(int[] CategoryIds)
        {
            using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
            {
                var configDetails = from configset in dc.TestConfigurationSets
                                    select new
                                    {
                                        configset.Id,
                                        configset.Name,
                                        configset.TestCategory_Id,
                                        configset.AnonymousAccess
                                    };

                if (!Request.IsAuthenticated || !AuthorizationManager.IsCoreTeamMember(User))
                {
                    configDetails = configDetails.Where(c => c.AnonymousAccess == true);
                }

                var Configurations = configDetails.Where(a => CategoryIds.Contains(a.TestCategory_Id)).ToList();
                StringBuilder sb = new StringBuilder();

                foreach (var configuration in Configurations)
                {
                    sb.Append("<option value='" + configuration.Id + "'>" + configuration.Name + "</option>");
                }

                return sb.ToString();
            }
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

        public AllRunsSet GetTestRunDetails(DateTime? fromDate, DateTime? toDate, int[] configIDs, int[] CategoryIDs, int page, int PageSize)
        {
            AllRunsSet model = new AllRunsSet();
            model.FromDate = Convert.ToDateTime(fromDate);
            model.ToDate = Convert.ToDateTime(toDate);

            using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
            {
                var categoryDetails = from Categoryset in dc.TestCategorySets
                                      select new
                                      {
                                          Categoryset.Id,
                                          Categoryset.Name
                                      };

                List<CategoryList> testCategory = new List<CategoryList>();
                foreach (var category in categoryDetails.ToList())
                {
                    testCategory.Add(new CategoryList { ID = category.Id, Name = category.Name });
                }

                var configDetails = from configset in dc.TestConfigurationSets
                                    select new
                                    {
                                        configset.Id,
                                        configset.Name,
                                        configset.Type,
                                        configset.AnonymousAccess,
                                        configset.TestCategory_Id,
                                        configset.TestAuthentication_Id,
                                        configset.Branch
                                    };

                if (!Request.IsAuthenticated || !AuthorizationManager.IsCoreTeamMember(User))
                {
                    configDetails = configDetails.Where(c => c.AnonymousAccess == true);
                }

                if (CategoryIDs != null && CategoryIDs.Any())
                {   
                    configDetails = configDetails.Where(c => CategoryIDs.Contains(c.TestCategory_Id));
                }

                List<ConfigurationList> testConfigurations = new List<ConfigurationList>();
                foreach (var config in configDetails.ToList())
                {
                    testConfigurations.Add(new ConfigurationList { ID = config.Id, Name = config.Name });
                }

                if (configIDs != null && configIDs.Any())
                {
                    configDetails = configDetails.Where(c => configIDs.Contains(c.Id));
                   
                        //model.SelectedCategory = category;
                 
                }


                var testResults = (from configset in configDetails
                                   join categorySet in dc.TestCategorySets on configset.TestCategory_Id equals categorySet.Id
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
                                       CategoryName = categorySet.Name,
                                       appOnly = authenticationset.AppOnly,
                                       cType = configset.Type,
                                       GithubBranch = configset.Branch
                                   }).OrderByDescending(t => t.Testdate).Skip((page - 1) * PageSize).Take(PageSize).ToList();

                List<AllRuns> configTestRuns = (from testrunsets in testResults
                                                select new AllRuns
                                                {
                                                    Status = testrunsets.Status,
                                                    ConfiguratioName = testrunsets.cName,
                                                    GithubBranch = testrunsets.GithubBranch,
                                                    CategoryName = testrunsets.CategoryName,
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
                model.TestCategory = testCategory;

                model.TestResultsCount = (from testrunsets in dc.TestRunSets
                                          join config in configDetails on testrunsets.TestConfigurationId equals config.Id
                                          where DbFunctions.TruncateTime(testrunsets.TestDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(testrunsets.TestDate) <= DbFunctions.TruncateTime(toDate)
                                          select new { testrunsets.Id }).ToList().Count;

            }

            return model;
        }
    }
}