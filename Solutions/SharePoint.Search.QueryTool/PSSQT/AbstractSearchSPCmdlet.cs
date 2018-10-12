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


        public PSAuthenticationMethod AuthenticationMethod { get; set; } = PSAuthenticationMethod.Windows;

        [Parameter(
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Force a login prompt when you are using -AuthenticationMode SPOManagement."
        )]
        public SwitchParameter ForceLoginPrompt { get; set; }

        #endregion

        #region Methods


        protected AbstractSearchSPCmdlet()
        {
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

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

        private void SetRequestAutheticationType(SearchRequest searchRequest)
        {
            if (Credential != null || searchRequest.AuthenticationType == AuthenticationType.Windows)
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
            else if (searchRequest.AuthenticationType == AuthenticationType.SPO)
            {
                SPOLegacyLogin(searchRequest);
            }
            else if (AuthenticationMethod == PSAuthenticationMethod.SPOManagement || searchRequest.AuthenticationType == AuthenticationType.SPOManagement)
            {
                AdalLogin(searchRequest, ForceLoginPrompt.IsPresent);
                //searchSuggestionsRequest.Token = token;
            }
            else
            {
                searchRequest.AuthenticationType = AuthenticationType.CurrentUser;
                WindowsIdentity currentWindowsIdentity = WindowsIdentity.GetCurrent();
                searchRequest.UserName = currentWindowsIdentity.Name;
            }
        }

        internal void SPOLegacyLogin(SearchRequest searchRequest)
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

        internal static void AdalLogin(SearchRequest searchRequest, bool forceLogin)
        {
            AdalAuthentication adalAuth = new AdalAuthentication();

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

            if (! File.Exists(fileName))
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
