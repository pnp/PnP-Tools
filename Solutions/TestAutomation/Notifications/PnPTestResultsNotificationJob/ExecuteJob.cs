using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;
using PnPTestResultsNotificationJob.Models;
using PnPTestResultsNotificationJob.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnPTestResultsNotificationJob
{
    public class ExecuteJob
    {
        public static void SendNotificationsToSubscribedUsers()
        {
            #region variables declaration
            List<string> lstAdminEmails = new List<string>();
            List<string> lstUserEmails = new List<string>();
            #endregion

            List<EmailVerifiedUsers> users = SubscribedUsers.GetAllEmailVerifiedUsers();
            foreach (EmailVerifiedUsers user in users)
            {
                if (user.isAdmin || user.isCoreMember)
                {
                    if (!lstAdminEmails.Contains(user.Email))
                    {
                        lstAdminEmails.Add(user.Email);
                    }
                }
                else
                {
                    if (!lstUserEmails.Contains(user.Email))
                    {
                        lstUserEmails.Add(user.Email);
                    }
                }
            }
            
            #region Send emails to subscribed users
            bool isAdmin = true;
            DateTime currentTime = DateTime.Now;
            DateTime last24Hours = currentTime.AddHours(-24);

            SendNotifications(lstAdminEmails, currentTime, last24Hours, isAdmin);            

            isAdmin = false; // community users
            SendNotifications(lstUserEmails, currentTime, last24Hours, isAdmin);
            #endregion
        }

        private static void SendNotifications(List<string> lstEmails,DateTime currentTime, DateTime last24Hours, bool isAdmin)
        {
            int outCome = 2; // Failed test results

            if (lstEmails.Count > 0)
            {
                List<AllRuns> completedRuns = TestRunDetails.GetCompletedTestRuns(currentTime,last24Hours, isAdmin);
                int[] GetAllTestRunsIds = GetAllTestRunsId(completedRuns);
                List<TestSummary> failedTestResults =  FailedTestRuns.GetFailedTestResults(GetAllTestRunsIds,currentTime, last24Hours, outCome, isAdmin);
                
                string emailBody = HtmlTemplates.GetEmailBody(completedRuns, failedTestResults);
                SendEmail.SendNotiifcationEmail(emailBody, lstEmails);
            }
        }
        private static int[] GetAllTestRunsId(List<AllRuns> completedRuns)
        {
            List<int> completedTestRunId = new List<int>();
            foreach(AllRuns allruns in completedRuns)
            {
                completedTestRunId.Add(allruns.TestRunSetId);
            }
            return completedTestRunId.ToArray();
        }
    }
}
