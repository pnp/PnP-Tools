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
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PnPTestResultsNotificationJob
{
    class Program
    {
        static void Main(string[] args)
        {
            ExecuteJob.SendNotificationsToSubscribedUsers();
        }
    }
}
