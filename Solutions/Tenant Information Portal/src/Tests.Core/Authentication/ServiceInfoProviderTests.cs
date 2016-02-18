using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Online.Applications.Core.Configuration;
using Microsoft.Online.Applications.Core;
using System.Threading.Tasks;
using Microsoft.Online.Applications.Core.Authentication;

namespace Tests.Core.Authentication
{
    [TestClass]
    public class ServiceInfoProviderTests
    {
        private AppConfig appConfig;
        private ServiceProvider serviceProvider;


        [TestInitialize]
        public void Setup()
        {
            this.appConfig = new AppConfig
            {
                ClientID = "1234567",
                ClientSecret = "supersecret",
                TenantDomain = "contoso"
            };

            this.serviceProvider = new ServiceProvider();
        }

        [TestMethod]
        public async Task GetServiceInformationClientProvider()
        {
            var serviceInfo = await this.serviceProvider.CreateServiceInfo(this.appConfig, CredentialType.Client);

            Assert.AreEqual(this.appConfig.ClientID, serviceInfo.ClientID, "Unexpected clientID set.");
            Assert.AreEqual(this.appConfig.ClientSecret, serviceInfo.ClientSecret, "Unexpected clientSecret set.");
            Assert.AreEqual(this.appConfig.TenantDomain, serviceInfo.Tenant, "Unexpected Tenant set.");
            Assert.IsTrue(serviceInfo.AuthenticationProvider is AdalClientAuthenticationProvider, "Unexpected authentication provider type.");
        }


        [TestMethod]
        public async Task GetServiceInformationCertificateProvider()
        {
            var serviceInfo = await this.serviceProvider.CreateServiceInfo(this.appConfig, CredentialType.Certificate);

            Assert.AreEqual(this.appConfig.ClientID, serviceInfo.ClientID, "Unexpected clientID set.");
            Assert.AreEqual(this.appConfig.ClientSecret, serviceInfo.ClientSecret, "Unexpected clientSecret set.");
            Assert.AreEqual(this.appConfig.TenantDomain, serviceInfo.Tenant, "Unexpected Tenant set.");
            Assert.IsTrue(serviceInfo.AuthenticationProvider is AdalCertificateAuthenticationProvider, "Unexpected authentication provider type.");
        }

        [TestMethod]
        public void Test()
        {
            var _t = this.serviceProvider.CreateServiceInfo(this.appConfig, CredentialType.Client).Result;

           
        }
    }
}
