using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;
using PnPTestResultsNotificationJob.Models;
using PnPTestResultsNotificationJob.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnPTestResultsNotificationJob
{
    public class TestRunDetails
    {
        public static List<AllRuns> GetCompletedTestRuns(DateTime? toDate,DateTime? fromDate, bool isAdmin)
        {
            List<AllRuns> configTestRuns = null;

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

                if (!isAdmin)
                {
                    configDetails = configDetails.Where(c => c.AnonymousAccess == true);
                }

                var testResults = (from configset in configDetails
                                   join authenticationset in dc.TestAuthenticationSets on configset.TestAuthentication_Id equals authenticationset.Id
                                   join testrunsets in dc.TestRunSets on configset.Id equals testrunsets.TestConfigurationId
                                   //where testrunsets.TestDate >= fromDate && testrunsets.Status >= 2
                                   where DbFunctions.AddMilliseconds(testrunsets.TestDate, (DbFunctions.DiffMilliseconds(TimeSpan.Zero, testrunsets.TestTime) ?? 0)) >= fromDate
                                   && DbFunctions.AddMilliseconds(testrunsets.TestDate, (DbFunctions.DiffMilliseconds(TimeSpan.Zero, testrunsets.TestTime) ?? 0)) < toDate
                                    && testrunsets.Status >= 2
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
                                       GithubBranch = configset.Branch
                                   }).OrderByDescending(t => t.Testdate).ToList();

                configTestRuns = (from testrunsets in testResults
                                                select new AllRuns
                                                {
                                                    Status = testrunsets.Status,
                                                    ConfiguratioName = testrunsets.cName,
                                                    GithubBranch = testrunsets.GithubBranch,
                                                    AppOnly = Enum.GetName(typeof(AppOnly), testrunsets.appOnly),
                                                    Environment = Enum.GetName(typeof(EnvironmentType), testrunsets.cType),
                                                    Testdate = testrunsets.Testdate,
                                                    TestDuration = Convert.ToString(testrunsets.TestDuration),
                                                    Passed = (testrunsets.Passed != null ? testrunsets.Passed : 0),
                                                    Skipped = (testrunsets.Skipped != null ? testrunsets.Skipped : 0),
                                                    Failed = (testrunsets.Failed != null ? testrunsets.Failed : 0),
                                                    Log = testrunsets.Log,
                                                    TestRunSetId = testrunsets.TestRunSetId,
                                                    Tests = ((testrunsets.Passed != null ? testrunsets.Passed : 0)
                                                      + (testrunsets.Skipped != null ? testrunsets.Skipped : 0)
                                                      + (testrunsets.Failed != null ? testrunsets.Failed : 0))
                                                }).OrderByDescending(d => d.Testdate.Date).ThenByDescending(t => t.Testdate.TimeOfDay).ToList();
            }

            return configTestRuns;
        }

    }
}
