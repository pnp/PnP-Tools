using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Owin;
using PnPAutomationUI.Models;
using PnPAutomationUI.Helpers;
using System.IdentityModel.Tokens;

namespace PnPAutomationUI
{
    public partial class Startup
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

        public static readonly string Authority = aadInstance + tenantId;

        private PnPTestAutomationEntities dbContext;

        public Startup()
        {
            dbContext = new PnPTestAutomationEntities(ConfigurationManager.ConnectionStrings["PnPTestAutomationEntities"].ConnectionString);
        }


        public void ConfigureAuth(IAppBuilder app)
        {
            //ApplicationDbContext db = new ApplicationDbContext();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            // Default OWIN cookie manager fails, hence the need for a custom cookiemanager. See http://blogs.msdn.com/b/kaevans/archive/2015/03/23/using-openid-connect-with-sharepoint-apps.aspx and 
            // https://katanaproject.codeplex.com/workitem/197 for more details.
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                CookieManager = new SystemWebCookieManager()
            });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = Authority,
                    PostLogoutRedirectUri = postLogoutRedirectUri,
                    TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                    {
                        // instead of using the default validation (validating against a single issuer value, as we do in line of business apps), 
                        // we inject our own multitenant validation logic
                        ValidateIssuer = false,
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        RedirectToIdentityProvider = (context) =>
                        {
                            // This ensures that the address used for sign in and sign out is picked up dynamically from the request 
                            // this allows you to deploy your app (to Azure Web Sites, for example)without having to change settings 
                            // Remember that the base URL of the address used here must be provisioned in Azure AD beforehand. 
                            string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
                            context.ProtocolMessage.RedirectUri = appBaseUrl + "/";
                            context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;
                            return Task.FromResult(0);
                        },

                        // we use this notification for injecting our custom logic
                        SecurityTokenValidated = (context) =>
                        {
                            // retriever caller data from the incoming principal
                            string issuer = context.AuthenticationTicket.Identity.FindFirst("iss").Value;
                            string UPN = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.Name).Value;
                            string Name= context.AuthenticationTicket.Identity.FindFirst("name").Value;
                            string tenantID = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;

                            // add logic to validate which accounts are allowed in
                            var user = dbContext.UsersSets.Where(u => u.UPN.Equals(UPN, StringComparison.InvariantCultureIgnoreCase));
                            if (user == null || user.Count<UsersSet>() != 1)
                            {
                                var newuser = new UsersSet();
                                newuser.Name = Name;
                                newuser.UPN = UPN;
                                newuser.IsCoreMember = false;
                                newuser.SendTestResults = false;
                                newuser.IsEmailVerified = false;
                                newuser.IsAdmin = false;
                                dbContext.UsersSets.Add(newuser);
                                dbContext.SaveChanges();
                            }

                            return Task.FromResult(0);
                        },

                        AuthenticationFailed = (context) =>
                        {
                            // Call error action on home controller to have the default MVC error handling pick up the error
                            context.OwinContext.Response.Redirect("/TestRuns/Error?message=" + context.Exception.Message);
                            context.HandleResponse(); // Suppress the exception
                            return Task.FromResult(0);
                        }

                    }
                });
        }
    }
}