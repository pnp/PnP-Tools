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
    public class SubscribedUsers
    {
        public static List<EmailVerifiedUsers> GetAllEmailVerifiedUsers()
        {
            List<EmailVerifiedUsers> users = null;
            using (PnPTestAutomationEntities dbContext = new PnPTestAutomationEntities())
            {
                users = (from userset in dbContext.UsersSets
                         where userset.IsEmailVerified == true && userset.SendTestResults == true
                         select new EmailVerifiedUsers
                         {
                             Email = userset.Email,
                             UPN = userset.UPN,
                             isAdmin = userset.IsAdmin,
                             isCoreMember = userset.IsCoreMember
                         }).ToList();

            }

            return users;
        }

    }
}
