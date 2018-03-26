using Microsoft.SharePoint.Client;
using SharePoint.Scanning.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Scanner.Results
{
    public class UserCustomActionResult: Scan
    {
        public string Title { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public UserCustomActionRegistrationType RegistrationType { get; set; }

        public string RegistrationId { get; set; }

        public string CommandAction { get; set; }

        public string ScriptBlock { get; set; }

        public string ScriptSrc { get; set; }

        public string Problem { get; set; }
    }
}
