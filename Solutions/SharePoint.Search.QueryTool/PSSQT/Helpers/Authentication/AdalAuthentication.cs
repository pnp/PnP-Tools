using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Concurrent;
using System.Management.Automation;
using System.Threading.Tasks;
using ADAL = Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace PSSQT.Helpers.Authentication
{
    class AdalAuthentication
    {
        private static readonly ConcurrentDictionary<Guid, TokenCache> Tokens = new ConcurrentDictionary<Guid, TokenCache>();   // SPO Auth tokens

        protected static readonly string AuthorityUri = "https://login.windows.net/common/oauth2/authorize";
        protected static readonly string clientId = "9bc3ab49-b65d-410a-85ad-de819febfddc";
        protected static readonly string redirectUri = "https://oauth.spops.microsoft.com/";

        protected AuthenticationContext AuthContext = null; //  new ADAL.AuthenticationContext(AuthorityUri);    // use default static cache - not thread safe?;

        public AdalAuthentication()
        {
            CreateTokenCache();
        }

        //public static AdalAuthentication AdalAuthenticationFactory()
        //{
        //    return new AdalAuthentication();
        //}

        protected void CreateTokenCache(bool forceRecreate = false)
        {
            Guid runspaceId = Guid.Empty;

            using (var ps = PowerShell.Create(RunspaceMode.CurrentRunspace))
            {
                runspaceId = ps.Runspace.InstanceId;


                bool found = Tokens.TryGetValue(runspaceId, out TokenCache tc);

                if (!found || forceRecreate)
                {
                    tc = new TokenCache();

                    Tokens.AddOrUpdate(runspaceId, tc, (k, v) => v);     // Do I need to use a ConcurrentDictionary?
                }

                AuthContext = new AuthenticationContext(AuthorityUri, tc);
            }
        }

        public virtual async Task<string> Login(string sharePointSiteUrl, bool forceLogin = false)
        {
            var spUri = new Uri(sharePointSiteUrl);

            string resourceUri = spUri.Scheme + "://" + spUri.Authority;

            AuthenticationResult authenticationResult = await AquireToken(resourceUri, forceLogin);

            return authenticationResult.CreateAuthorizationHeader();
        }

        protected virtual async Task<AuthenticationResult> AquireToken(string resourceUri, bool forceLogin)
        {
            AuthenticationResult authenticationResult;

            if (forceLogin)
            {
                CreateTokenCache(true);

                var authParam = new PlatformParameters(PromptBehavior.Always);
                authenticationResult = await AuthContext.AcquireTokenAsync(resourceUri, clientId, new Uri(redirectUri), authParam);
            }
            else
            {
                try
                {
                    authenticationResult = await AuthContext.AcquireTokenSilentAsync(resourceUri, clientId);
                }
                catch (AdalSilentTokenAcquisitionException)
                {

                    try
                    {
                        // prevent flashing of login window when credentials are valid
                        var authParam = new PlatformParameters(PromptBehavior.Never);
                        authenticationResult = await AuthContext.AcquireTokenAsync(resourceUri, clientId, new Uri(redirectUri), authParam);
                    }
                    catch (AdalException /* e */)
                    {
                        //Console.WriteLine(e);

                        var authParam = new PlatformParameters(PromptBehavior.Auto);
                        authenticationResult = await AuthContext.AcquireTokenAsync(resourceUri, clientId, new Uri(redirectUri), authParam);

                    }
                }

            }

            return authenticationResult;
        }
    }

    class AdalUserCredentialAuthentication : AdalAuthentication
    {
        public AdalUserCredentialAuthentication(UserCredential credentials) :
            base()
        {
            this.Credentials = credentials;
        }

        public UserCredential Credentials { get; }

        protected override async Task<AuthenticationResult> AquireToken(string resourceUri, bool forceLogin)
        {
            return await AuthContext.AcquireTokenAsync(resourceUri, clientId, Credentials);
        }

    }
}
