using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PnPAutomationUI.Models
{
    public class Configuration
    {
        public string ConfiguratioName { get; set; }
        public string Description { get; set; }
        public string VSBuildConfiguration { get; set; }
        public string GithubBranch { get; set; }
        public string AppType { get; set; }
        public bool Type { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public int PlatformType { get; set; }
        public string Platform { get; set; }
        public string Testsitecollection { get; set; }
        public string Tenantadminsitecollection { get; set; }
        public List<ConfigurationpropertySet> ConfigCustomProperties { get; set; }
        public int ConfigurationId { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Domain { get; set; }
        public string CredentialManagerLabel { get; set; }
        public string IdentityUserNameValue { get; set; }
        public string IdentityPassWordValue { get; set; }
        public string IdentityUserNameDisplayText { get; set; }
        public string IdentityPassWordDisplayText { get; set; }
        public bool AnonymousAccess { get; set; }
        public int TestRunId { get; set; }
    }

    public class ConfigurationList
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class ConfigurationpropertySet
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

    }
}