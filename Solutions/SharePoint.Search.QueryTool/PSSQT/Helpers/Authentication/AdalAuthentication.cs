using System;
using System.Collections.Concurrent;
using System.Management.Automation;
using System.Threading.Tasks;
using ADAL = Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace PSSQT.Helpers.Authentication
{
    class AdalAuthentication
    {
        private static readonly ConcurrentDictionary<Guid, ADAL.TokenCache> Tokens = new ConcurrentDictionary<Guid, ADAL.TokenCache>();   // SPO Auth tokens

        private static readonly string AuthorityUri = "https://login.windows.net/common/oauth2/authorize";
        private static readonly string clientId = "9bc3ab49-b65d-410a-85ad-de819febfddc";
        private static readonly string redirectUri = "https://oauth.spops.microsoft.com/";

        private ADAL.AuthenticationContext AuthContext = null; //  new ADAL.AuthenticationContext(AuthorityUri);    // use default static cache - not thread safe?;

        public AdalAuthentication()
        {
            CreateTokenCache();
        }

        private void CreateTokenCache(bool forceRecreate = false)
        {
            Guid runspaceId = Guid.Empty;

            using (var ps = PowerShell.Create(RunspaceMode.CurrentRunspace))
            {
                runspaceId = ps.Runspace.InstanceId;

                ADAL.TokenCache tc;

                bool found = Tokens.TryGetValue(runspaceId, out tc);

                if (!found || forceRecreate)
                {
                    tc = new ADAL.TokenCache();

                    Tokens.AddOrUpdate(runspaceId, tc, (k, v) => v);     // Do I need to use a ConcurrentDictionary?
                }

                AuthContext = new ADAL.AuthenticationContext(AuthorityUri, tc);
            }
        }

        public async Task<string> Login(string sharePointSiteUrl, bool forceLogin = false)
        {
            var spUri = new Uri(sharePointSiteUrl);

            string resourceUri = spUri.Scheme + "://" + spUri.Authority;

            ADAL.AuthenticationResult authenticationResult;

            if (forceLogin)
            {
                CreateTokenCache(true);

                var authParam = new ADAL.PlatformParameters(ADAL.PromptBehavior.Always);
                authenticationResult = await AuthContext.AcquireTokenAsync(resourceUri, clientId, new Uri(redirectUri), authParam);
            }
            else
            {
                try
                {
                    authenticationResult = await AuthContext.AcquireTokenSilentAsync(resourceUri, clientId);
                }
                catch (ADAL.AdalSilentTokenAcquisitionException)
                {

                    try
                    {
                        // prevent flashing of login window when credentials are valid
                        var authParam = new ADAL.PlatformParameters(ADAL.PromptBehavior.Never);
                        authenticationResult = await AuthContext.AcquireTokenAsync(resourceUri, clientId, new Uri(redirectUri), authParam);
                    }
                    catch (ADAL.AdalException /* e */)
                    {
                        //Console.WriteLine(e);

                        var authParam = new ADAL.PlatformParameters(ADAL.PromptBehavior.Auto);
                        authenticationResult = await AuthContext.AcquireTokenAsync(resourceUri, clientId, new Uri(redirectUri), authParam);

                    }
                }

            }


           return authenticationResult.CreateAuthorizationHeader();
        }
    }
}
