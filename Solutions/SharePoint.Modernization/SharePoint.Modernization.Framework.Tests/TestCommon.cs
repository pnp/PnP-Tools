using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core;

namespace SharePoint.Modernization.Framework.Tests
{
    static class TestCommon
    {

        static TestCommon()
        {
            // Read configuration data
            TenantUrl = AppSetting("SPOTenantUrl");
            DevSiteUrl = AppSetting("SPODevSiteUrl");

#if !ONPREMISES
            if (string.IsNullOrEmpty(TenantUrl))
            {
                throw new ConfigurationErrorsException("Tenant site Url in App.config are not set up.");
            }
#endif
            if (string.IsNullOrEmpty(DevSiteUrl))
            {
                throw new ConfigurationErrorsException("Dev site url in App.config are not set up.");
            }



            // Trim trailing slashes
            TenantUrl = TenantUrl.TrimEnd(new[] { '/' });
            DevSiteUrl = DevSiteUrl.TrimEnd(new[] { '/' });

            if (!string.IsNullOrEmpty(AppSetting("SPOCredentialManagerLabel")))
            {
                var tempCred = OfficeDevPnP.Core.Utilities.CredentialManager.GetCredential(AppSetting("SPOCredentialManagerLabel"));

                // username in format domain\user means we're testing in on-premises
                if (tempCred.UserName.IndexOf("\\") > 0)
                {
                    string[] userParts = tempCred.UserName.Split('\\');
                    Credentials = new NetworkCredential(userParts[1], tempCred.SecurePassword, userParts[0]);
                }
                else
                {
                    Credentials = new SharePointOnlineCredentials(tempCred.UserName, tempCred.SecurePassword);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(AppSetting("SPOUserName")) &&
                    !String.IsNullOrEmpty(AppSetting("SPOPassword")))
                {
                    UserName = AppSetting("SPOUserName");
                    var password = AppSetting("SPOPassword");

                    Password = GetSecureString(password);
                    Credentials = new SharePointOnlineCredentials(UserName, Password);
                }
                else if (!String.IsNullOrEmpty(AppSetting("AppId")) &&
                         !String.IsNullOrEmpty(AppSetting("AppSecret")))
                {
                    AppId = AppSetting("AppId");
                    AppSecret = AppSetting("AppSecret");
                }
                else
                {
                    throw new ConfigurationErrorsException("Tenant credentials in App.config are not set up.");
                }
            }

        }

        #region properties
        public static string TenantUrl { get; set; }
        public static string DevSiteUrl { get; set; }
        static string UserName { get; set; }
        static SecureString Password { get; set; }
        public static ICredentials Credentials { get; set; }
        public static string AppId { get; set; }
        static string AppSecret { get; set; }

        public static bool AppOnlyTesting()
        {
            if (!String.IsNullOrEmpty(AppSetting("AppId")) &&
                !String.IsNullOrEmpty(AppSetting("AppSecret")) &&
                String.IsNullOrEmpty(AppSetting("SPOCredentialManagerLabel")) &&
                String.IsNullOrEmpty(AppSetting("SPOUserName")) &&
                String.IsNullOrEmpty(AppSetting("SPOPassword")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region methods
        public static string AppSetting(string key)
        {
#if !NETSTANDARD2_0
            return ConfigurationManager.AppSettings[key];
#else
            try
            {
                return configuration.AppSettings.Settings[key].Value;
            }
            catch
            {
                return null;
            }
#endif
        }

        public static ClientContext CreateClientContext()
        {
            return CreateContext(DevSiteUrl, Credentials);
        }

        public static ClientContext CreateClientContext(string url)
        {
            return CreateContext(url, Credentials);
        }

        public static ClientContext CreateTenantClientContext()
        {
            return CreateContext(TenantUrl, Credentials);
        }

        private static ClientContext CreateContext(string contextUrl, ICredentials credentials)
        {
            ClientContext context = null;
            if (!String.IsNullOrEmpty(AppId) && !String.IsNullOrEmpty(AppSecret))
            {
                OfficeDevPnP.Core.AuthenticationManager am = new OfficeDevPnP.Core.AuthenticationManager();
                context = am.GetAppOnlyAuthenticatedContext(contextUrl, AppId, AppSecret);
            }
            else
            {
                context = new ClientContext(contextUrl);
                context.Credentials = credentials;
            }

            context.RequestTimeout = 1000 * 60 * 15;
            return context;
        }

        private static SecureString GetSecureString(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input string is empty and cannot be made into a SecureString", "input");

            var secureString = new SecureString();
            foreach (char c in input.ToCharArray())
                secureString.AppendChar(c);

            return secureString;
        }
        #endregion

    }
}
