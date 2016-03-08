// ------------------------------------------------------------------------------
//The MIT License(MIT)

//Copyright(c) 2015 Office Developer
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
// ------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using TIP.Dashboard.Models;
using Microsoft.Online.Applications.Core.Configuration;
using TIP.Common.Configuration;
using TIP.Dashboard.Dal;


namespace TIP.Dashboard.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        AppConfig _appConfig = ConfigurationFactory.Instance.GetApplicationConfiguration();
  
       /// <summary>
       /// Gets the User Profile
       /// </summary>
       /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            try
            {
                Uri servicePointUri = new Uri(Microsoft.Online.Applications.Core.Constants.Authentication.GraphServiceUrl);
                Uri serviceRoot = new Uri(servicePointUri, tenantID);
                ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot,
                      async () => await GetTokenForApplication());
        
                // use the token for querying the graph to get the user details
                var result = await activeDirectoryClient.Users
                    .Where(u => u.ObjectId.Equals(userObjectID))
                    .ExecuteAsync();
                IUser user = result.CurrentPage.ToList().First();

                return View(user);
            }
            catch (AdalException _ex)
            {
                ViewBag.ErrorMessage = _ex.Message;
                // Return to error page.
                 return View("Error");
            }
            // if the above failed, the user needs to explicitly re-authenticate for the app to obtain the required token
            catch (Exception)
            {             
                return View("Relogin");
            }
        }

        public void RefreshSession()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties { RedirectUri = "/UserProfile" }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        public async Task<string> GetTokenForApplication()
        {
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
            ClientCredential clientcred = new ClientCredential(_appConfig.ClientID, _appConfig.ClientSecret);

            AuthenticationContext authenticationContext = new AuthenticationContext(string.Format("https://login.microsoftonline.com/{0}", tenantID), new ADALTokenCache(signedInUserID));

            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenSilentAsync(Microsoft.Online.Applications.Core.Constants.Authentication.GraphServiceUrl, clientcred, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
            return authenticationResult.AccessToken;
        }
    }
}
