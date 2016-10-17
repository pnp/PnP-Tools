using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using PnPAutomationUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PnPAutomationUI.Utilities;
using System.Web.Routing;

namespace PnPAutomationUI.Controllers
{
    [Authorize]
    public class UsersettingsController : Controller
    {
        // GET: Usersettings
        public ActionResult Index()
        {
            UserSettings userSettings = null;
            if (Request.IsAuthenticated)
            {
                var username = User.Identity.Name;
                StringBuilder sbUserStatus = new StringBuilder();
                using (PnPTestAutomationEntities dbContext = new PnPTestAutomationEntities())
                {
                    userSettings = (from userset in dbContext.UsersSets
                                    where userset.UPN.ToLower() == username.ToLower()
                                    select new UserSettings
                                    {
                                        UserPrinciplename = userset.UPN,
                                        isAdmin = userset.IsAdmin,
                                        isCommunityMember = true,
                                        isCoreMember = userset.IsCoreMember,
                                        TestSummaryEmail = userset.Email,
                                        SendTestResults = userset.SendTestResults,
                                        isEmailverified = userset.IsEmailVerified
                                    }).SingleOrDefault();
                    if (userSettings != null)
                    {
                        if (userSettings.isCommunityMember)
                            sbUserStatus.Append("Community Member");
                        if (userSettings.isCoreMember)
                            sbUserStatus.Append(" | Core Member");
                        if (userSettings.isAdmin)
                            sbUserStatus.Append(" | Administrator");

                        userSettings.Memberstatus = sbUserStatus.ToString();
                    }
                }

            }
            else
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/Usersettings/Index" },
                   OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }            

            return View(userSettings);
        }

        public ActionResult SaveSettings(string email)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    var username = User.Identity.Name;
                    StringBuilder sbUserStatus = new StringBuilder();
                    using (PnPTestAutomationEntities dbContext = new PnPTestAutomationEntities())
                    {
                        var user = dbContext.UsersSets.SingleOrDefault(u => u.UPN.Equals(username, StringComparison.InvariantCultureIgnoreCase));

                        if (user != null)
                        {
                            user.SendTestResults = true;
                            user.Email = email;
                            dbContext.SaveChanges();

                            SendEmail(email);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            
            return View();
        }

        private void SendEmail(string email)
        {
            string encryptedEmail = EncryptionUtility.Encrypt(email);
            string rootUrl = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
            string url = string.Format(rootUrl + "/Usersettings/confirmEmail?validateToken={0}", encryptedEmail);
            // Create a new Exchange service object
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
            // Set user login credentials
            string ExchangeServerEmail = ConfigurationManager.AppSettings["ExchangeServerEmailID"];
            string ExchangeServerpwd = ConfigurationManager.AppSettings["ExchangeServerpwd"];
            service.Credentials = new WebCredentials(ExchangeServerEmail, ExchangeServerpwd);

            try
            {
                //Set Office 365 Exchange Webserivce Url
                string serviceUrl = "https://outlook.office365.com/ews/exchange.asmx";
                service.Url = new Uri(serviceUrl);
                EmailMessage emailMessage = new EmailMessage(service);
                emailMessage.Subject = "Email Confirmation for daily summary updates";
                emailMessage.Body = new MessageBody("Thanks for subscribing pnp test results, please click <a href='" + url + "'> here </a> to confirm your email address ");
                emailMessage.ToRecipients.Add(email);
                emailMessage.Send();
            }
            catch (AutodiscoverRemoteException exception)
            {
                throw;
            }

        }

        public ActionResult UpdateSettings(string email, bool isSendTestResults)
        {
            EmailConfirmation emailConfirmation = new EmailConfirmation();

            try
            {
                if (Request.IsAuthenticated)
                {
                    var username = User.Identity.Name;
                    StringBuilder sbUserStatus = new StringBuilder();
                    using (PnPTestAutomationEntities dbContext = new PnPTestAutomationEntities())
                    {
                        var user = dbContext.UsersSets.SingleOrDefault(u => u.UPN.Equals(username, StringComparison.InvariantCultureIgnoreCase));

                        if (user != null)
                        {
                            if (!isSendTestResults)
                            {
                                user.SendTestResults = false;
                                user.Email = email;
                                user.IsEmailVerified = false;
                                dbContext.SaveChanges();

                                emailConfirmation.IsSettingsUpdated = true;
                            }
                            else if (isSendTestResults && user.Email != email)
                            {
                                user.SendTestResults = true;
                                user.Email = email;
                                dbContext.SaveChanges();

                                SendEmail(email);
                                emailConfirmation.IsEmailSent = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
         
            return View(emailConfirmation);
        }

        public ActionResult ConfirmEmail(string validateToken)
        {
            EmailConfirmation emailconf = new EmailConfirmation();

            try
            {
                if (Request.IsAuthenticated)
                {
                    var username = User.Identity.Name;
                    string decryptedEmail = EncryptionUtility.Decrypt(validateToken);
                    emailconf.Email = decryptedEmail;
                    using (PnPTestAutomationEntities dbContext = new PnPTestAutomationEntities())
                    {
                        var user = dbContext.UsersSets.FirstOrDefault(
                        u => u.Email.Equals(decryptedEmail, StringComparison.InvariantCultureIgnoreCase)
                        && u.UPN.Equals(username, StringComparison.InvariantCultureIgnoreCase));
                        if (user != null)
                        {
                            user.IsEmailVerified = true;
                            user.SendTestResults = true;
                            dbContext.SaveChanges();
                            emailconf.IsEmailVerified = true;
                        }
                    }
                }
                else
                {
                    HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/Usersettings/ConfirmEmail/" + validateToken },
                  OpenIdConnectAuthenticationDefaults.AuthenticationType);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return View(emailconf);
        }

        public ActionResult Unsubscribe(string validateToken)
        {
            EmailConfirmation emailconf = new EmailConfirmation();
            try
            {
                if (Request.IsAuthenticated)
                {
                    string decryptedEmail = EncryptionUtility.Decrypt(validateToken);
                    var username = User.Identity.Name;

                    using (PnPTestAutomationEntities dbContext = new PnPTestAutomationEntities())
                    {
                        var user = dbContext.UsersSets.FirstOrDefault(
                        u => u.Email.Equals(decryptedEmail, StringComparison.InvariantCultureIgnoreCase)
                        && u.UPN.Equals(username, StringComparison.InvariantCultureIgnoreCase));
                        if (user != null)
                        {
                            user.IsEmailVerified = false;
                            user.SendTestResults = false;
                            dbContext.SaveChanges();
                            emailconf.Email = decryptedEmail;
                        }
                    }
                }
                else
                {
                    HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/Usersettings/Unsubscribe/" + validateToken },
                  OpenIdConnectAuthenticationDefaults.AuthenticationType);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return View(emailconf);

        }

    }
}