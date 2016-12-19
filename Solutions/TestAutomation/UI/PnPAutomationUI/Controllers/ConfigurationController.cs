using PnPAutomationUI.Helpers;
using PnPAutomationUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PnPAutomationUI.Controllers
{
    public class ConfigurationController : Controller
    {
        // GET: Configuration
        public ActionResult Overview(int Id)
        {
            Configuration configDetails = null;
            if (Request.IsAuthenticated && AuthorizationManager.IsAdmin(User))
            {
                return RedirectToAction("Edit", new { Id = Id });
            }

            configDetails = GetConfigurationData(Id, false);

            return View(configDetails);
        }

        public ActionResult Edit(int Id, bool IsConfigurationUpdated = false)
        {
            Configuration configDetails = null;
            if (!Request.IsAuthenticated && !AuthorizationManager.IsAdmin(User))
            {
                return RedirectToAction("Overview", new { Id = Id });
            }
            configDetails = GetConfigurationData(Id, true);
            ViewBag.IsUpdated = IsConfigurationUpdated;

            return View(configDetails);
        }

        private Configuration GetConfigurationData(int TestRunID, bool IsAdmin)
        {
            Configuration configDetails = null;

            using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
            {
                IEnumerable<Configuration> configurationDetails = from configset in dc.TestConfigurationSets
                                                                  join categorySet in dc.TestCategorySets on configset.TestCategory_Id equals categorySet.Id
                                                                  join testrunsets in dc.TestRunSets on configset.Id equals testrunsets.TestConfigurationId
                                                                  join testauthenticationset in dc.TestAuthenticationSets on configset.TestAuthentication_Id equals testauthenticationset.Id
                                                                  where testrunsets.Id == TestRunID
                                                                  select new Configuration
                                                                  {
                                                                      ConfigurationId = configset.Id,
                                                                      ConfiguratioName = configset.Name,
                                                                      CategoryName = categorySet.Name,
                                                                      Description = configset.Description,
                                                                      AnonymousAccess = configset.AnonymousAccess,
                                                                      VSBuildConfiguration = configset.VSBuildConfiguration,
                                                                      GithubBranch = configset.Branch,
                                                                      Type = testauthenticationset.AppOnly,
                                                                      ClientID = testauthenticationset.AppId,
                                                                      ClientSecret = testauthenticationset.AppSecret,
                                                                      UserName = testauthenticationset.User,
                                                                      PassWord = testauthenticationset.Password,
                                                                      Domain = testauthenticationset.Domain,
                                                                      CredentialManagerLabel = testauthenticationset.CredentialManagerLabel,
                                                                      PlatformType = configset.Type,
                                                                      Testsitecollection = configset.TestSiteUrl,
                                                                      Tenantadminsitecollection = configset.TenantUrl,
                                                                      TestRunId = TestRunID
                                                                  };

                if (!Request.IsAuthenticated || !AuthorizationManager.IsCoreTeamMember(User))
                {
                    configDetails = configurationDetails.Where(c => c.AnonymousAccess == true).SingleOrDefault();
                }
                else
                {
                    configDetails = configurationDetails.SingleOrDefault();
                }

                if (configDetails != null)
                {
                    var configpropertyDetails = (from configsetproperty in dc.TestConfigurationPropertySets
                                                 where configsetproperty.TestConfigurationId == configDetails.ConfigurationId
                                                 select new ConfigurationpropertySet { ID = configsetproperty.Id, Name = configsetproperty.Name, Value = configsetproperty.Value }).ToList();
                    switch (configDetails.Type)
                    {
                        case true:
                            configDetails.IdentityUserNameDisplayText = "ClientID";
                            configDetails.IdentityPassWordDisplayText = "Client Secret";
                            configDetails.IdentityUserNameValue = configDetails.ClientID;
                            configDetails.IdentityPassWordValue = configDetails.ClientSecret;
                            break;
                        case false:
                            configDetails.IdentityUserNameDisplayText = "Credential Manager";
                            configDetails.IdentityPassWordDisplayText = "Password";
                            if (configDetails.CredentialManagerLabel != null)
                            {
                                configDetails.IdentityUserNameValue = configDetails.CredentialManagerLabel;
                                configDetails.IdentityPassWordValue = string.Empty;
                            }
                            else
                            {
                                configDetails.IdentityUserNameValue = configDetails.Domain + @"\" + configDetails.UserName;
                                configDetails.IdentityPassWordValue = configDetails.PassWord;
                            }
                            break;
                    }

                    configDetails.AppType = Enum.GetName(typeof(AppOnly), configDetails.Type);
                    configDetails.Platform = Enum.GetName(typeof(EnvironmentType), configDetails.PlatformType);
                    configDetails.ConfigCustomProperties = configpropertyDetails;

                    if (!IsAdmin) // Mask data for non-admins
                    {
                        configDetails.IdentityUserNameValue = "*******************";
                        configDetails.IdentityPassWordValue = "*******************";
                        if (configDetails.CredentialManagerLabel != null)
                        {
                            configDetails.IdentityPassWordValue = string.Empty;
                        }

                        var lstConfig = new List<ConfigurationpropertySet>();
                        foreach (ConfigurationpropertySet customproperty in configDetails.ConfigCustomProperties)
                        {
                            lstConfig.Add(new ConfigurationpropertySet { ID = customproperty.ID, Name = customproperty.Name, Value = "*******************" });
                        }
                        configDetails.ConfigCustomProperties = lstConfig;
                    }
                }
            }

            return configDetails;
        }

        [HttpPost]
        public ActionResult Edit(Configuration config)
        {
            using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
            {
                var configuration = dc.TestConfigurationSets.SingleOrDefault(c => c.Id == config.ConfigurationId);
                configuration.Branch = config.GithubBranch;
                configuration.Description = config.Description;
                configuration.TenantUrl = config.Tenantadminsitecollection;
                configuration.TestSiteUrl = config.Testsitecollection;
                configuration.VSBuildConfiguration = config.VSBuildConfiguration;
                configuration.Type = (int)Enum.Parse(typeof(EnvironmentType), config.Platform);

                var testauthenticationset = dc.TestAuthenticationSets.SingleOrDefault(a => a.Id == configuration.TestAuthentication_Id);
                if (testauthenticationset != null)
                {
                    testauthenticationset.AppSecret = config.ClientSecret;
                    testauthenticationset.AppId = config.ClientID;
                    testauthenticationset.CredentialManagerLabel = config.CredentialManagerLabel;

                    testauthenticationset.User = config.UserName;
                    testauthenticationset.Domain = config.Domain;
                    testauthenticationset.Password = config.PassWord;
                }

                dc.SaveChanges();

                if (config.ConfigCustomProperties != null && config.ConfigCustomProperties.Count > 0)
                {
                    foreach (ConfigurationpropertySet configpropertySet in config.ConfigCustomProperties)
                    {
                        var configurationpropertySetData = dc.TestConfigurationPropertySets.SingleOrDefault(c => c.Id == configpropertySet.ID);
                        if (configurationpropertySetData != null)
                        {
                            configurationpropertySetData.Name = configpropertySet.Name;
                            configurationpropertySetData.Value = configpropertySet.Value;
                            dc.SaveChanges();
                        }
                    }
                }
                
            }

            return RedirectToAction("Edit", new { id = config.TestRunId, IsConfigurationUpdated = true });
        }

    }
}