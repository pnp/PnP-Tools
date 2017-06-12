using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchQueryTool.Helpers
{
    /// <summary>
    /// Provides means to authenticate a user via a pop up login form.
    /// </summary>
    public class SPOClientWebAuth : IDisposable
    {
        private const int DEFAULT_WEBBROWSER_POP_UP_WIDTH = 495;
        private const int DEFAULT_WEBBROWSER_POP_UP_HEIGHT = 390;
        private static string CLAIM_HEADER_RETURN_URL = "X-Forms_Based_Auth_Return_Url";
        private static string CLAIM_HEADER_AUTH_REQUIRED = "X-FORMS_BASED_AUTH_REQUIRED";
        
        #region Construction

        /// <summary>
        /// Displays a pop up window to authenticate the user
        /// </summary>
        /// <param name="targetSiteUrl"></param>
        /// <param name="popUpWidth"></param>
        /// <param name="popUpHeight"></param>
        public SPOClientWebAuth(string targetSiteUrl)
        {
            if (string.IsNullOrEmpty(targetSiteUrl)) 
                throw new ArgumentException("SharePoint Site Url is required.");
            
            this.fldTargetSiteUrl = targetSiteUrl;

            // set login page url and success url from target site
            this.GetClaimParams(this.fldTargetSiteUrl, out this.fldLoginPageUrl, out  this.fldNavigationEndUrl);
            
            this.webBrowser = new WebBrowser();
            this.webBrowser.Navigated += new WebBrowserNavigatedEventHandler(ClaimsWebBrowser_Navigated);
            this.webBrowser.ScriptErrorsSuppressed = true;
            this.webBrowser.Dock = DockStyle.Fill;
        }

        #endregion

        #region private Fields
        private WebBrowser webBrowser;

        private CookieCollection fldCookies;
        private Form DisplayLoginForm;

        #endregion

        #region Public Properties

        private string fldLoginPageUrl;
        /// <summary>
        /// Login form Url
        /// </summary>
        public string LoginPageUrl
        {
            get { return fldLoginPageUrl; }
            set { fldLoginPageUrl = value; }
        }

        private Uri fldNavigationEndUrl;
        /// <summary>
        /// Success Url
        /// </summary>
        public Uri NavigationEndUrl
        {
            get { return fldNavigationEndUrl; }
            set { fldNavigationEndUrl = value; }
        }

        /// <summary>
        /// Target site Url
        /// </summary>
        private string fldTargetSiteUrl = null;
        public string TargetSiteUrl
        {
            get { return fldTargetSiteUrl; }
            set { fldTargetSiteUrl = value; }
        }

        /// <summary>
        /// Cookies returned from CLAIM server.
        /// </summary>
        public CookieCollection AuthCookies
        {
            get { return fldCookies; }
        }

        private bool fldIsCLAIMSite = false;
        /// <summary>
        /// Is set to true if the CLAIM site did not return the proper headers -- hence it's not an CLAIM site or does not support CLAIM style authentication
        /// </summary>
        public bool IsCLAIMSite
        {
            get { return fldIsCLAIMSite; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens a Windows Forms Web Browser control to authenticate the user against an CLAIM site.
        /// </summary>
        /// <param name="popUpWidth"></param>
        /// <param name="popUpHeight"></param>
        public CookieCollection Show()
        {
            if (string.IsNullOrEmpty(this.LoginPageUrl))
                throw new ApplicationException("The requested site does not appear to have claims enabled or the Site Url has not been set.");

            // navigate to the login page url.
            this.webBrowser.Navigate(this.LoginPageUrl);

            DisplayLoginForm = new Form();
            DisplayLoginForm.SuspendLayout();


            DisplayLoginForm.Width = DEFAULT_WEBBROWSER_POP_UP_WIDTH;
            DisplayLoginForm.Height = DEFAULT_WEBBROWSER_POP_UP_HEIGHT;
            DisplayLoginForm.Text = this.fldTargetSiteUrl;

            DisplayLoginForm.Controls.Add(this.webBrowser);
            DisplayLoginForm.ResumeLayout(false);

            Application.Run(DisplayLoginForm);

            // see ClaimsWebBrowser_Navigated event
            return this.fldCookies;
        }

        #endregion

        public static CookieCollection GetAuthenticatedCookies(string targetSiteUrl)
        {
            CookieCollection authCookie = null;

            using (SPOClientWebAuth webAuth = new SPOClientWebAuth(targetSiteUrl))
            {
                authCookie = webAuth.Show();
            }

            return authCookie;
        }

        #region Privatee Methods

        private void GetClaimParams(string targetUrl, out string loginUrl, out Uri navigationEndUrl)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(targetUrl);
            webRequest.Method = "OPTIONS";

            try
            {
                WebResponse response = (WebResponse)webRequest.GetResponse();
                ExtraHeadersFromResponse(response, out loginUrl, out navigationEndUrl);
            }
            catch (WebException webEx)
            {
                ExtraHeadersFromResponse(webEx.Response, out loginUrl, out navigationEndUrl);
            }
        }

        private bool ExtraHeadersFromResponse(WebResponse response, out string loginUrl, out Uri navigationEndUrl)
        {
            loginUrl = null;
            navigationEndUrl = null;

            try
            {
                // For some reason, SharePoint Online Wave 15 Preview seems to return these responses in triplicate, separated by commas
                // Let's just get the first one

                string returnUrl = response.Headers[CLAIM_HEADER_RETURN_URL];
                if (returnUrl != null && returnUrl.Contains(","))
                {
                    returnUrl = returnUrl.Substring(0, returnUrl.IndexOf(",", StringComparison.InvariantCultureIgnoreCase));
                }
                navigationEndUrl = new Uri(returnUrl);

                loginUrl = (response.Headers[CLAIM_HEADER_AUTH_REQUIRED]);
                if (loginUrl != null && loginUrl.Contains(","))
                {
                    loginUrl = loginUrl.Substring(0, loginUrl.IndexOf(",", StringComparison.InvariantCultureIgnoreCase));
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private CookieCollection ExtractAuthCookiesFromUrl(string url)
        {
            Uri uriBase = new Uri(url);
            Uri uri = new Uri(uriBase, "/");
            
            // call WinInet.dll to get cookie.
            string stringCookie = CookieReader.GetCookie(uri.ToString());
            
            if (string.IsNullOrEmpty(stringCookie)) return null;
            
            stringCookie = stringCookie.Replace("; ", ",").Replace(";", ",");
            
            // use CookieContainer to parse the string cookie to CookieCollection
            CookieContainer cookieContainer = new CookieContainer();
            cookieContainer.SetCookies(uri, stringCookie);
            
            return cookieContainer.GetCookies(uri);
        }

        #endregion

        #region Private Events

        private void ClaimsWebBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            // check whether the url is same as the navigationEndUrl.
            if (fldNavigationEndUrl != null && fldNavigationEndUrl.Equals(e.Url))
            {
                this.fldCookies = ExtractAuthCookiesFromUrl(this.LoginPageUrl);
                this.DisplayLoginForm.Close();
            }
        }

        #endregion

        #region IDisposable Methods
        /// <summary> 
        /// Disposes of this instance. 
        /// </summary> 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.webBrowser != null) this.webBrowser.Dispose();

                if (this.DisplayLoginForm != null) this.DisplayLoginForm.Dispose();
            }
        }

        #endregion
    }
}
