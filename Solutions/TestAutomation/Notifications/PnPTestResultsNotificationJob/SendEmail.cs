using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;
using PnPTestResultsNotificationJob.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnPTestResultsNotificationJob
{
    public class SendEmail
    {
        public static void SendNotiifcationEmail(string emailBody, List<string> recepients)
        {
            try
            {
                string rootUrl = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
                // Create a new Exchange service object
                ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
                StringBuilder sbUserStatus = new StringBuilder();
                string ExchangeServerEmail = ConfigurationManager.AppSettings["ExchangeServerEmailID"];
                string ExchangeServerpwd = ConfigurationManager.AppSettings["ExchangeServerpwd"];
                foreach (string toEmail in recepients)
                {
                    string encryptedEmail = EncryptionUtility.Encrypt(toEmail);
                    string url = string.Format(rootUrl + "Usersettings/Unsubscribe?validateToken={0}", encryptedEmail);
                    emailBody = emailBody.Replace("{unsubribelink}", url);
                    // Set user login credentials
                    service.Credentials = new WebCredentials(ExchangeServerEmail, ExchangeServerpwd);
                    try
                    {
                        //Set Office 365 Exchange Webserivce Url
                        string serviceUrl = "https://outlook.office365.com/ews/exchange.asmx";
                        service.Url = new Uri(serviceUrl);
                        EmailMessage emailMessage = new EmailMessage(service);
                        emailMessage.Subject = "PnP TestAutomation daily summary report";
                        emailMessage.Body = new MessageBody(BodyType.HTML, emailBody);
                        emailMessage.ToRecipients.Add(toEmail);
                        //emailMessage.BccRecipients.Add(toEmail);
                        emailMessage.Send();

                    }
                    catch (AutodiscoverRemoteException exception)
                    {
                        Console.WriteLine("Erroe while sending the mails to user" + toEmail + exception.Message);
                        throw;

                    }
                }
            }
            catch (Exception ex)
            {                
                Console.WriteLine("Erroe while sending the mails to users" + ex.Message);
            }
            
        }

    }
}
