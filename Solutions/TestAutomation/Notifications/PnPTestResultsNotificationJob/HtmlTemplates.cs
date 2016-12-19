using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;
using PnPTestResultsNotificationJob.Models;
using PnPTestResultsNotificationJob.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnPTestResultsNotificationJob
{
    public class HtmlTemplates
    {
        public static string GetEmailBody(List<AllRuns> allRunsSet, List<TestSummary> lstTestSummary)
        {
            StringBuilder sbRes = new StringBuilder();
            StringBuilder sbTestSummary = new StringBuilder();
            bool isAlternaterow = true;
            string templateTestRuns = "TestRunsRowTemplate.html";
            string templateFaileRuns = "TestResultsSummaryTemplate.html";

            if (allRunsSet != null && allRunsSet.Count > 0)
            {
                string testRunsRowTemplate = GetHTMLTemplate(templateTestRuns);

                foreach (AllRuns allruns in allRunsSet)
                {
                    sbRes.AppendLine(FillTestRunsTemplate(isAlternaterow, allruns, testRunsRowTemplate));
                    isAlternaterow = !isAlternaterow;
                }
            }

            if (lstTestSummary != null && lstTestSummary.Count > 0)
            {
                string testFailedRunsRowTemplate = GetHTMLTemplate(templateFaileRuns);

                foreach (TestSummary testsummary in lstTestSummary)
                {
                    sbTestSummary.AppendLine(FillFailedRunsHtmlTemplate(isAlternaterow, testsummary, testFailedRunsRowTemplate));
                    isAlternaterow = !isAlternaterow;
                }
            }

            return CombineTestRunsAndFailedRunsInTemplate(sbRes, sbTestSummary);
        }

        public static string CombineTestRunsAndFailedRunsInTemplate(StringBuilder sbTestRes, StringBuilder sbtestSummary)
        {
            string fileName = "NotificationTemplate.html";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mail Templates", fileName);
            string body = string.Empty;
            string testSummaryStyle = string.Empty;

            using (StreamReader reader = new StreamReader(path))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{tablebody}", sbTestRes.ToString());

            if (!string.IsNullOrEmpty(sbtestSummary.ToString()))
            {
                testSummaryStyle = "block";
            }
            else
            {
                testSummaryStyle = "none";
            }

            body = body.Replace("{summarystyle}", testSummaryStyle);
            body = body.Replace("{testresultssummarytablebody}", sbtestSummary.ToString());

            return body;
        }

        public static string GetHTMLTemplate(string fileName)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mail Templates", fileName);
            string body = string.Empty;

            using (StreamReader reader = new StreamReader(path))
            {
                body = reader.ReadToEnd();
            }

            return body;
        }

        public static string FillFailedRunsHtmlTemplate(bool isAlternateRow, TestSummary testSummary, string rowTemplate)
        {
            StringBuilder sbTestres = new StringBuilder();
            string[] statusIcons = { "&#252;", "&#228", "&#251;", "&#97" };
            string alternateRowCss = (isAlternateRow ? "ms-Grid-row-ms-bgColor-neutralLighterAlt" : "ms-Grid-row-ms-bgColor-neutralLighter");
            string rootUrl = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

            if (rootUrl[rootUrl.Length - 1] == '/')
            {
                rootUrl = rootUrl.Remove(rootUrl.Length - 1);
            }

            return String.Format(rowTemplate,
                                        rootUrl,
                                        testSummary.Id,
                                        statusIcons[testSummary.Outcome],
                                        testSummary.TestCasename,
                                        GetTestDuration(testSummary.TestDuration),
                                        testSummary.Error,
                                        testSummary.Branch,
                                        alternateRowCss,
                                        testSummary.Configuration,                                       
                                        GetTestDate(testSummary.Testdate),
                                        testSummary.CategoryName);

        }

        public static string FillTestRunsTemplate(bool isAlternateRow, AllRuns testResult, string rowTemplate)
        {
            StringBuilder sbTestres = new StringBuilder();
            string[] statusIcons = { "&#97;", "&#228;", "&#252;", "&#251;" };
            string Log = string.Empty;
            string alternateRowCss = (isAlternateRow ? "ms-Grid-row-ms-bgColor-neutralLighterAlt" : "ms-Grid-row-ms-bgColor-neutralLighter");

            string rootUrl = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
            if (rootUrl[rootUrl.Length - 1] == '/')
            {
                rootUrl = rootUrl.Remove(rootUrl.Length - 1);
            }

            if (testResult.Log)
            {
                Log = "<a href = \"" + rootUrl + "/TestRuns/PrintLog/" + testResult.TestRunSetId + "\">Log</a>";
            }
            else
            {
                Log = "N/A";
            }

            return String.Format(rowTemplate, rootUrl, testResult.TestRunSetId, statusIcons[testResult.Status],
                                 testResult.ConfiguratioName,
                                 testResult.CategoryName,
                                 testResult.AppOnly,
                                 testResult.Environment,
                                 GetTestDate(testResult.Testdate),
                                 GetTestDuration(testResult.TestDuration),
                                 testResult.Tests,
                                 testResult.Passed,
                                 testResult.Skipped,
                                 testResult.Failed,
                                 Log,
                                 testResult.GithubBranch,
                                 alternateRowCss);
        }

        public static string GetTestDate(DateTime dateTime)
        {
            string runat = string.Empty;

            if (dateTime != null && !string.IsNullOrEmpty(dateTime.ToString()))
            {
                runat = @String.Format("{0}", Convert.ToString(dateTime));
                if (Convert.ToString(dateTime).Contains('.'))
                {
                    runat = Convert.ToString(dateTime).Remove(Convert.ToString(dateTime).IndexOf('.'));
                }
            }
            else
            {
                runat = @String.Format("{0}", "N/A");
            }

            return runat;
        }

        public static string GetTestDuration(string testTime)
        {
            if (!string.IsNullOrEmpty(testTime))
            {
                if (testTime.Contains('.'))
                {
                    testTime = testTime.Remove(testTime.IndexOf('.'));
                }

                testTime = @String.Format("{0}", Convert.ToString(testTime));
            }
            else
            {
                testTime = @String.Format("{0}", "N/A");
            }

            return testTime;
        }
    }
}
