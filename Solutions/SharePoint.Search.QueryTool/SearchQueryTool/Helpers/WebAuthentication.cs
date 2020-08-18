using SearchQueryTool.Model;
using System;
using System.Net;
using System.Windows.Forms;

namespace SearchQueryTool.Helpers
{
    /// <summary>
    /// Provides means to authenticate a user via a pop up login form.
    /// </summary>
    public class WebAuthentication : IDisposable
    {
        private const int DEFAULT_WEBBROWSER_POP_UP_WIDTH = 800;
        private const int DEFAULT_WEBBROWSER_POP_UP_HEIGHT = 600;
        private static string CLAIM_HEADER_RETURN_URL = "X-Forms_Based_Auth_Return_Url";
        private static string CLAIM_HEADER_AUTH_REQUIRED = "X-FORMS_BASED_AUTH_REQUIRED";

        [System.Runtime.InteropServices.DllImport("wininet.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetOption(int hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        private static unsafe void SuppressWininetBehavior()
        {
            /* SOURCE: http://msdn.microsoft.com/en-us/library/windows/desktop/aa385328%28v=vs.85%29.aspx
                * INTERNET_OPTION_SUPPRESS_BEHAVIOR (81):
                *      A general purpose option that is used to suppress behaviors on a process-wide basis. 
                *      The lpBuffer parameter of the function must be a pointer to a DWORD containing the specific behavior to suppress. 
                *      This option cannot be queried with InternetQueryOption. 
                *      
                * INTERNET_SUPPRESS_COOKIE_PERSIST (3):
                *      Suppresses the persistence of cookies, even if the server has specified them as persistent.
                *      Version:  Requires Internet Explorer 8.0 or later.
                */

            int option = (int)3/* INTERNET_SUPPRESS_COOKIE_PERSIST*/;
            int* optionPtr = &option;

            bool success = InternetSetOption(0, 81/*INTERNET_OPTION_SUPPRESS_BEHAVIOR*/, new IntPtr(optionPtr), sizeof(int));
            if (!success)
            {
                MessageBox.Show("Something went wrong");
            }
        }

        #region Construction

        /// <summary>
        /// Displays a pop up window to authenticate the user
        /// </summary>
        /// <param name="targetSiteUrl"></param>
        /// <param name="popUpWidth"></param>
        /// <param name="popUpHeight"></param>
        public WebAuthentication(string targetSiteUrl, AuthenticationType authenticationType)
        {
            if (string.IsNullOrEmpty(targetSiteUrl))
                throw new ArgumentException("SharePoint Site Url is required.");

            this.fldTargetSiteUrl = targetSiteUrl;
            this.authenticationType = authenticationType;

            // set login page url and success url from target site
            if (AuthenticationType.SPO == authenticationType)
            {
                this.GetClaimParams(this.fldTargetSiteUrl, out this.fldLoginPageUrl, out this.fldNavigationEndUrl);
            }
            else if (AuthenticationType.Forefront == authenticationType)
            {
                this.fldLoginPageUrl = fldTargetSiteUrl;
            }

            this.webBrowser = new WebBrowser();
            this.webBrowser.Navigated += new WebBrowserNavigatedEventHandler(ClaimsWebBrowser_Navigated);
            this.webBrowser.ScriptErrorsSuppressed = false;
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

        private readonly AuthenticationType authenticationType;


        #endregion

        #region Public Methods

        /// <summary>
        /// Opens a Windows Forms Web Browser control to authenticate the user against an CLAIM site.
        /// </summary>
        /// <param name="popUpWidth"></param>
        /// <param name="popUpHeight"></param>
        public CookieCollection Show()
        {
            SuppressWininetBehavior();
            if (AuthenticationType.SPO == authenticationType && string.IsNullOrEmpty(this.LoginPageUrl))
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

        public static CookieCollection GetAuthenticatedCookies(string targetSiteUrl, AuthenticationType authenticationType)
        {
            CookieCollection cookies = null;
            using (WebAuthentication webAuth = new WebAuthentication(targetSiteUrl, authenticationType))
            {
                cookies = webAuth.Show();
            }
            return cookies;
        }

        #region Private Methods

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

        private CookieCollection GetCookiesFor(Uri url)
        {
            Uri uri = new Uri(url, "/");

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
            //Console.WriteLine("Navigated to " + e.Url);
            WriteLine("Navigated to " + e.Url);             // overridden in PSWebAuthetication to avoid printout to console

            var url = e.Url.AbsoluteUri;
            if(url.StartsWith("https://login.") && url.Contains("authorize") && !url.Contains("login_hint"))
            {
                int index = url.IndexOf("?");
                if (index == -1) index = url.Length;
                url = url.Insert(index + 1, "login_hint=@" + new Uri(this.fldLoginPageUrl).Host + "&");
                this.webBrowser.Navigate(url);
            }

            // check whether the url is same as the navigationEndUrl.
            if (fldNavigationEndUrl != null && fldNavigationEndUrl.Equals(e.Url))
            {
                this.fldCookies = GetCookiesFor(new Uri(this.LoginPageUrl));
                this.DisplayLoginForm.Close();
            }
            else if (AuthenticationType.Forefront == authenticationType && !(e.Url.ToString().Contains("Login.asp")))
            {
                this.fldCookies = GetCookiesFor(e.Url);
                if(null != this.fldCookies)
                {
                    this.DisplayLoginForm.Close();
                }
            }            
        }

        protected virtual void WriteLine(string text)
        {
            Console.WriteLine(text);
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
