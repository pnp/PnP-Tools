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
    public class FailedTestRuns
    {
        public static List<TestSummary> GetFailedTestResults(int[] TestRunIDs, DateTime? toDate, DateTime? fromDate, int outcome, bool isAdmin)
        {
            List<TestSummary> testResults = null;

            using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
            {
                var configDetails = from configset in dc.TestConfigurationSets
                                    select new
                                    {
                                        configset.Id,
                                        configset.AnonymousAccess,
                                        configset.Branch,
                                        configset.Name
                                    };

                if (!isAdmin)
                {
                    configDetails = configDetails.Where(c => c.AnonymousAccess == true);
                }

                var filteredTestRunSets = dc.TestRunSets.Where(c => TestRunIDs.Contains(c.Id));
                var data = (from testrunset in filteredTestRunSets
                            join testresultset in dc.TestResultSets on testrunset.Id equals testresultset.TestRunId
                            join configset in configDetails on testrunset.TestConfigurationId equals configset.Id
                            where testresultset.Outcome == outcome
                            select new
                            {
                                Outcome = testresultset.Outcome,
                                Error = testresultset.ErrorMessage,
                                StackTrace = testresultset.ErrorStackTrace,
                                TestDuration = testresultset.Duration,
                                TestCasename = testresultset.TestCaseName,
                                Id = testresultset.Id,
                                Branch = configset.Branch,
                                Configuration = configset.Name,
                                Testdate = testrunset.TestDate,
                                TestRunId = testrunset.Id
                            }).ToList();

                if (data != null && data.Any())
                {
                    testResults = (from testresult in data
                                   select new TestSummary
                                   {
                                       Outcome = testresult.Outcome,
                                       Error = testresult.Error,
                                       StackTrace = testresult.StackTrace,
                                       TestDuration = Convert.ToString(testresult.TestDuration),
                                       TestCasename = testresult.TestCasename,
                                       Id = testresult.Id,
                                       Branch = testresult.Branch,
                                       Configuration = testresult.Configuration,
                                       Testdate = testresult.Testdate,
                                       TestRunId = testresult.TestRunId
                                   }).OrderByDescending(t => t.Id).ToList();
                }

            }

            return testResults;
        }


    }
}
