using SearchQueryTool.Helpers;
using SearchQueryTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PSSQT.Helpers
{
    public class PSWebAuthentication : WebAuthentication
    {
        public PSCmdlet Cmdlet { get; set; }

        public PSWebAuthentication(PSCmdlet cmdlet, string targetSiteUrl, AuthenticationType authenticationType) :
            base(targetSiteUrl, authenticationType)
        {
            this.Cmdlet = cmdlet;
        }

        protected override void WriteLine(string text)
        {
            Cmdlet.WriteDebug(text);
        }

        public static CookieCollection GetAuthenticatedCookies(PSCmdlet cmdlet, string targetSiteUrl, AuthenticationType authenticationType)
        {
            CookieCollection cookies = null;
            using (PSWebAuthentication webAuth = new PSWebAuthentication(cmdlet, targetSiteUrl, authenticationType))
            {
                cookies = webAuth.Show();
            }
            return cookies;
        }

    }
}
