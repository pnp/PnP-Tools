using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PnPAutomationUI.Models
{
    public class UserSettings
    {
        public string UserPrinciplename { get; set; }
        public string Memberstatus { get; set; }
        public string TestSummaryEmail { get; set; }
        public bool isAdmin { get; set;}
        public bool isCoreMember { get; set; }
        public bool isCommunityMember { get; set; }
        public bool SendTestResults { get; set; }
        public bool Checked { get; set; }
        public bool isEmailverified { get; set; }

    }
    public class EmailConfirmation
    {
        public bool IsEmailSent { get; set; }
        public bool IsSettingsUpdated { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
    }
}