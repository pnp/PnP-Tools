using System;
using System.Management.Automation;
using SearchQueryTool.Model;
using SearchQueryTool.Helpers;
using System.Security.Principal;
using System.Net;
using System.IO;
using System.Linq;
using System.Collections.Specialized;
using System.Collections.Generic;
using PSSQT.Helpers;
using PSSQT.Helpers.Authentication;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

/**
 * <ParameterSetName	P1	P2
 * Site                 X   X
 * Query                X   X 
 * LoadPreset	        	X
 **/

namespace PSSQT
{
    public abstract class AbstractSearchSPCmdlet<TSearchRequest>
         : PSCmdlet where TSearchRequest : SearchRequest, new()
    {
        #region PrivateMembers


        private static readonly Dictionary<Guid, CookieCollection> Tokens = new Dictionary<Guid, CookieCollection>();   // SPO Auth tokens

        private static bool SkipSSLValidation;

        #endregion

        #region ScriptParameters


        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            Position = 0,
            HelpMessage = "Query terms.",
            ParameterSetName = "P1"
        )]
        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            Position = 0,
            HelpMessage = "Query terms.",
            ParameterSetName = "P2"
        )]
        public string[] Query { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Credentials."
        )]
        public PSCredential Credential { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Accept Type. Accept XML or JSON."
        )]
        public AcceptType? AcceptType { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Method Type. Use GET or POST."
        )]
        public HttpMethodType? MethodType { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "SharePoint site to connect to. If it starts with http(s)//, use directly, otherwise load from connection file. See -SaveSite",
            ParameterSetName = "P1"
        )]
        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "SharePoint site to connect to. If it starts with http(s)//, use directly, otherwise load from connection file. See -SaveSite",
            ParameterSetName = "P2"
        )]
        public string Site { get; set; }


        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Load parameters from file. Use Search-SPIndex -SavePreset to save a preset. Script parameters on the command line overrides.",
            ParameterSetName = "P2"
        )]
        [Alias("Preset")]
        public string LoadPreset { get; set; }


        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Specify authentication mode."
        )]


        public PSAuthenticationMethod? AuthenticationMethod { get; set; }  // Environment variable can be used to set default


        [Parameter(
             ValueFromPipelineByPropertyName = false,
             ValueFromPipeline = false,
             HelpMessage = "Force a login prompt when you are using -AuthenticationMode SPOManagement."
         )]
        public SwitchParameter ForceLoginPrompt { get; set; }

        [Parameter(
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Skip validation of SSL certificate."
        )]
        public SwitchParameter SkipServerCertificateValidation { get; set; }
        #endregion

        #region Methods


        protected AbstractSearchSPCmdlet()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                new RemoteCertificateValidationCallback(ValidateServerCertificate);
        }

        protected bool UsingPreset
        {
            get
            {
                return ParameterSetName == "P2" && !String.IsNullOrWhiteSpace(LoadPreset);
            }
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                SkipSSLValidation = SkipServerCertificateValidation.IsPresent;

                WriteDebug($"Enter {GetType().Name} ProcessRecord");

                SearchConnection searchConnection = new SearchConnection();
                TSearchRequest searchRequest = new TSearchRequest();

                // Load Preset
                if (ParameterSetName == "P2")
                {
                    SearchPreset preset = LoadPresetFromFile();

                    searchConnection = preset.Connection;

                    PresetLoaded(ref searchRequest, preset);
                }

                // additional command line argument validation. Throw an error if not valid
                ValidateCommandlineArguments();

                // Set Script Parameters from Command Line. Override in deriving classes

                SetRequestParameters(searchRequest);

                // Save Site/Preset

                if (IsSavePreset())
                {
                    SaveRequestPreset(searchConnection, searchRequest);
                }
                else
                {
                    EnsureValidQuery(searchRequest);

                    ExecuteRequest(searchRequest);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex,
                           GetErrorId(),
                           ErrorCategory.NotSpecified,
                           null)
                          );

                WriteDebug(ex.StackTrace);
            }
        }

        protected virtual void ValidateCommandlineArguments()
        {
            return;       // override if necessary
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            if (!SkipSSLValidation)
            {
                Console.WriteLine($"Certificate error: {sslPolicyErrors}");
            }

            // Do not allow this client to communicate with unauthenticated servers unless Skip validation is set.
            return SkipSSLValidation;
        }

        protected abstract void SaveRequestPreset(SearchConnection searchConnection, TSearchRequest searchRequest);

        protected abstract bool IsSavePreset();

        protected abstract void PresetLoaded(ref TSearchRequest searchRequest, SearchPreset preset);

        protected abstract void ExecuteRequest(TSearchRequest searchRequest);

        protected virtual string GetErrorId()
        {
            return GetType().Name;
        }

        protected SearchPreset LoadPresetFromFile()
        {
            string path = GetPresetFilename(LoadPreset, true);

            WriteVerbose("Loading preset " + path);

            return new SearchPreset(path, true);
        }

        protected virtual void EnsureValidQuery(TSearchRequest searchRequest)
        {
            if (String.IsNullOrWhiteSpace(searchRequest.QueryText))
            {
                throw new Exception("Query text cannot be null.");
            }
        }

        protected virtual void SetRequestParameters(TSearchRequest searchRequest)
        {
            searchRequest.SharePointSiteUrl = GetSPSite() ?? searchRequest.SharePointSiteUrl;
            //searchConnection.SpSiteUrl = searchRequest.SharePointSiteUrl;

            searchRequest.QueryText = GetQuery() ?? searchRequest.QueryText;

            searchRequest.HttpMethodType = MethodType.HasValue ? MethodType.Value : searchRequest.HttpMethodType;
            //searchConnection.HttpMethod = searchRequest.HttpMethodType.ToString();

            searchRequest.AcceptType = AcceptType.HasValue ? AcceptType.Value : searchRequest.AcceptType;

            SetRequestAutheticationType(searchRequest);
        }

        protected virtual void SetRequestAutheticationType(SearchRequest searchRequest)
        {
            if (AuthenticationMethod != null)       // User specified AuthenticationMethod on command line. Always use that. Overrides preset
            {
                SwitchAuthenticationMethod(searchRequest);

            }
            else if (UsingPreset)                   // AuthenticationMethod == null, use value from preset file 
            {
                switch (searchRequest.AuthenticationType)
                {
                    case AuthenticationType.CurrentUser:
                        CurrentUserLogin(searchRequest);
                        break;
                    case AuthenticationType.Windows:
                        WindowsLogin(searchRequest);
                        break;
                    case AuthenticationType.SPO:
                        SPOLegacyLogin(searchRequest);
                        break;
                    case AuthenticationType.SPOManagement:
                        SPOManagementLogin(searchRequest);
                        break;

                    case AuthenticationType.Anonymous:
                    case AuthenticationType.Forefront:
                    case AuthenticationType.Forms:
                    default:
                        throw new NotImplementedException($"PSSQT does not support AuthenticationType {Enum.GetName(typeof(AuthenticationType), searchRequest.AuthenticationType)}. You can override on the command line.");
                }

            }
            else // No AthenticationMethod specified and no preset used
            {
                // Use default method set in environment or if not set, let's try to guess based on the site URL. Does hostname end with sharepoint.com? Yes, then assume SPOManagement
                AuthenticationMethod =  PSAuthenticationMethodFactory.DefaultAutenticationMethod() ?? GuessAuthenticationMethod(searchRequest) ?? PSAuthenticationMethod.CurrentUser;

                WriteVerbose($"Using authentication method {Enum.GetName(typeof(PSAuthenticationMethod), AuthenticationMethod)}");

                SwitchAuthenticationMethod(searchRequest);
            }
        }

        private void SwitchAuthenticationMethod(SearchRequest searchRequest)
        {
            switch (AuthenticationMethod)
            {
                case PSAuthenticationMethod.CurrentUser:
                    CurrentUserLogin(searchRequest);
                    break;
                case PSAuthenticationMethod.Windows:
                    WindowsLogin(searchRequest);
                    break;
                case PSAuthenticationMethod.SPO:
                    SPOLegacyLogin(searchRequest);
                    break;
                case PSAuthenticationMethod.SPOManagement:
                    SPOManagementLogin(searchRequest);
                    break;
                default:
                    throw new NotImplementedException($"Unsupported PSAuthenticationMethod {Enum.GetName(typeof(PSAuthenticationMethod), AuthenticationMethod)}");
            }
        }

        protected virtual void SPOManagementLogin(SearchRequest searchRequest)
        {
            if (Credential != null)
            {
                AdalLogin(new AdalUserCredentialAuthentication(new UserPasswordCredential(Credential.UserName, Credential.Password)), searchRequest, ForceLoginPrompt.IsPresent);
            }
            else
            {
                AdalLogin(searchRequest, ForceLoginPrompt.IsPresent);
            }
        }

        protected virtual void CurrentUserLogin(SearchRequest searchRequest)
        {
            searchRequest.AuthenticationType = AuthenticationType.CurrentUser;
            WindowsIdentity currentWindowsIdentity = WindowsIdentity.GetCurrent();
            searchRequest.UserName = currentWindowsIdentity.Name;
        }

        protected virtual void WindowsLogin(SearchRequest searchRequest)
        {
            if (Credential == null)
            {
                var userName = searchRequest.UserName;

                Credential = this.Host.UI.PromptForCredential("Enter username/password", "", userName, "");
            }

            searchRequest.AuthenticationType = AuthenticationType.Windows;
            searchRequest.UserName = Credential.UserName;
            searchRequest.SecurePassword = Credential.Password;
        }

        internal virtual void SPOLegacyLogin(SearchRequest searchRequest)
        {
            Guid runspaceId = Guid.Empty;
            using (var ps = PowerShell.Create(RunspaceMode.CurrentRunspace))
            {
                runspaceId = ps.Runspace.InstanceId;

                CookieCollection cc;

                bool found = Tokens.TryGetValue(runspaceId, out cc);

                if (!found)
                {
                    cc = PSWebAuthentication.GetAuthenticatedCookies(this, searchRequest.SharePointSiteUrl, AuthenticationType.SPO);

                    if (cc == null)
                    {
                        throw new RuntimeException("Authentication cookie returned is null! Authentication failed. Please try again.");  // TODO find another exception
                    }
                    else
                    {
                        Tokens.Add(runspaceId, cc);
                    }
                }

                searchRequest.AuthenticationType = AuthenticationType.SPO;
                searchRequest.Cookies = cc;
                //searchSuggestionsRequest.Cookies = cc;
            }
        }

        protected virtual PSAuthenticationMethod? GuessAuthenticationMethod(SearchRequest searchRequest)
        {
            // AuthenticationMethod == null; User did not specify one

            PSAuthenticationMethod? result = null;


            if (Credential != null)    // SPOManagemnt or Windows
            {
                result = GuessAuthenticationMethodFromHostname(searchRequest, PSAuthenticationMethod.Windows);
            }
            else
            {                           // SPOManagement or CurrentUser
                result = GuessAuthenticationMethodFromHostname(searchRequest, PSAuthenticationMethod.CurrentUser);
            }

            return result;
        }

        private static PSAuthenticationMethod? GuessAuthenticationMethodFromHostname(SearchRequest searchRequest, PSAuthenticationMethod alternateMethod)
        {
            PSAuthenticationMethod? result = null;

            var siteUrl = searchRequest.SharePointSiteUrl;

            if (!String.IsNullOrWhiteSpace(siteUrl))
            {
                if (Uri.TryCreate(siteUrl, UriKind.Absolute, out Uri uri))
                {
                    if (uri.Host.ToLower().EndsWith("sharepoint.com"))
                    {
                        result = PSAuthenticationMethod.SPOManagement;
                    }
                    else
                    {
                        result = alternateMethod;
                    }
                }
            }

            return result;
        }

        internal static void AdalLogin(SearchRequest searchRequest, bool forceLogin)
        {
            AdalLogin(new AdalAuthentication(), searchRequest, forceLogin);
        }

        internal static void AdalLogin(AdalAuthentication adalAuth, SearchRequest searchRequest, bool forceLogin)
        {
            var task = adalAuth.Login(searchRequest.SharePointSiteUrl, forceLogin);

            if (!task.Wait(300000))
            {
                throw new TimeoutException("Prompt for user credentials timed out after 5 minutes.");
            }

            var token = task.Result;

            searchRequest.AuthenticationType = AuthenticationType.SPOManagement;
            searchRequest.Token = token;
        }

        private string GetQuery()
        {
            return Query == null ? null : String.Join(" ", Query);
        }


        protected string GetPresetFilename(string presetName, bool searchPath = false)
        {
            string path = presetName;

            if (!path.EndsWith(".xml"))
            {
                path += ".xml";
            }

            if (searchPath && !Path.IsPathRooted(path))
            {
                // always check current directory first
                var rootedPath = GetRootedPath(path);

                if (!File.Exists(rootedPath))
                {
                    var environmentVariable = Environment.GetEnvironmentVariable("PSSQT_PresetsPath");

                    if (environmentVariable != null)
                    {
                        var result = environmentVariable
                            .Split(';')
                            .Where(s => File.Exists(Path.Combine(s, path)))
                            .FirstOrDefault();

                        if (result == null)
                        {
                            throw new ArgumentException(String.Format("File \"{0}\" not found in current directory or PSSQT_PresetsPath", path));
                        }

                        return Path.Combine(result, path);
                    }
                }

                return rootedPath;
            }


            return GetRootedPath(path);
        }

        internal string GetRootedPath(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, path);
            }

            return path;
        }


        // Hoping that ExclusionSets will be available soon. Would clean this up.
        protected static bool? GetThreeWaySwitchValue(SwitchParameter enable, SwitchParameter disable)
        {
            bool? result = null;

            if (enable) result = true;
            if (disable) result = false;    // disable overrides enable
                                            // else  result = null which means use default value
            return result;
        }


        protected string GetSPSite()
        {
            if (String.IsNullOrWhiteSpace(Site) || Site.StartsWith("http://") || Site.StartsWith("https://"))
            {
                return Site;
            }


            var fileName = GetPresetFilename(Site);

            if (!File.Exists(fileName))
            {
                throw new RuntimeException($"File not found: \"{fileName}\"");
            }

            SearchConnection sc = new SearchConnection();

            sc.Load(fileName);

            if (sc.SpSiteUrl == null)
            {
                throw new ArgumentException($"Unable to load valid saved site information from the file \"{fileName}\"");
            }

            return sc.SpSiteUrl;
        }



        #endregion
    }
}
