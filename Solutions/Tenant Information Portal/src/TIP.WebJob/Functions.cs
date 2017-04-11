using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Online.Applications.Core.Clients;
using TIP.Common.Configuration;
using Microsoft.Online.Applications.Core.Configuration;
using Microsoft.Online.Applications.Core;
using TIP.Common.Services.Applications;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TIP.Common.Services.Principals;


namespace TIP.WebJob
{
	public class Functions
	{
		static WebJobConfig appConfig = ConfigurationFactory.Instance.GetWebJobConfiguration();

		const string ExpiredImage = "data:image/gif;base64,R0lGODlhVgBWAHcAACH5BAEAAAAALAAAAABWAFYAh/8A//8AAP8EBP8ICP8MDP8QEP8YGP8gIP8kJP80NP9ERP9ISP9QUP9cXP9gYP9sbP94eP+Dg/+Li/+Tk/+Xl/+jo/+zs/+7u//Hx//j4//n5//v7//z8//39////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAj+AD0IHEiwoMGDCBMqXMiwocOHECNKnEixosWLGDNq3MixY8cOFyY8WIDggIEAAQwcQLDgwYQLHTzKJLihAgMCKHPq3ImSAIMKG2ZuzEBBgQCeSJMGEKCAQgahFTM4OKq0alIBDp5CfbgBwgCrYJUOgBB0q8IOEgqEXau0gISYZg1aOMC2rtIDFuISjEDVrt+dAiLo5dDgr2GkDeAK1ZDgsOOdCTQIxXDysWWUBjDIxIDzsmcCmjlqqOzZswHJGjk0Ls06geKLhVnLbpAxguzbAQRbtNAXt2cBeSl2oOtb9oHXECUUxy1h4ga1y2UXKAsRQnTcECJm+Hpd9gCtDh3+dMft4GGG3uN/g19IIT1uCg4VuL+toOEG9PMtC6CesEL+2xUwxMB/sjGwUAedEfgZcgVdoKBsFyg0wYOsTaDQAxSW9oBCC2To2QIKIaCUXhIphYBCxCFFYkR3KUQaTytCpJQBClUV40M2JpTjjQzteNCLO/HY0IwojijkQi0mJGJSRyKZ1IkJdeihZSAmhOGUj22Y0IRYOmZhQg52eViECSEo5l8EMFjQgGfaZeBC/rVZV4AL3SfnWvs1JN+dYNXXUHt8WgVfQ+cFqpQA6y0knqFIlWced4zm9F1E1kWaU3YRPWdpANNNpJylzU00XKTHVcSbocBdZFugusHGJ21rGakmp2sbjXbmaR1x1iVomwGpYGaLrfZgZFsR9mBievGVX2AxzuUeXjyiBd1ybqlJYleQekdWkwRJhZ9jWCXKrUBEGQVuU+KOW1BNN9XlE1DqQgSSSCSZhNlKLb1kbbz89uvvvwAHLDBHAQEAOw==";
		const string ExpiringSoonImage = "data:image/gif;base64,R0lGODlhVgBWAHcAACH5BAEAAAAALAAAAABWAFYAh/8A////AP//BP//CP//DP//EP//FP//GP//HP//IP//JP//KP//LP//MP//NP//OP//PP//QP//RP//SP//TP//UP//VP//WP//XP//YP//ZP//aP//bP//cP//dP//eP//fP//f///g///h///i///k///l///m///n///o///p///q///s///t///u///v///w///x///y///z///0///1///2///3///4///5///6///7///8///9///+////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAj+AH8IHEiwoMGDCBMqXMiwocOHECNKnEixosWLGDNq3Mixo8eCOTYsSJAhx8eTEHMsCMAywAKTKGMqvNCy5QOZOA3CqFkzRc6fPhrwbImAx0+cJ4bWBHE05o4DSlsOuNH05IeoNS9U9WhDANaaMLZypPC1ZgMfYjO6KMvzRNqLPlaybXlgx9uKJeby/HB34o4CemsOsNE3IofAPCkUfkjDK+KaLhY3lPCYJwO0khOyqDzURGaEPRRgNYi1gN3PBUd8JY2VA2qCOQCPLvhVQI3XAjWUZY11Am4ZjmcTZNviNQS2vLEqwCxZxdzkWElk7pHgOW22BXRIDqEXOtYNi3H+DOh+na2AGYUxBPaONULfGIjZY11x10H88nMV9EiL4rF8rCOIxQNU9w2HGAEwNXWVf/jplUFVNozHoIGPydCUBZz99xUER73AWQAafuUTTj4w8GGIWCGwn0wmfAhig4iFINMOBriIIlYD4BBTBy6+SCFnWp1UQ3AZwvhYWB9N0KOPAy3pwEctLMmkQFKi0FFcUt5Y1gFGbUSClFP+ACZfGukg25JaljWYRhuAGaabFmQ0A5E9psnWCxhF4Oabbl5m0Qp78ummZxSFFqidbBlwmkQiBCqomx5MlAMBjlbKkwCERZSBpZy25BtE8HUqanEPPSCqqAswt1AKnELAgg6xOrCgp6UlNNQDApZ2oKoPIFiaHUO9VgqBqgPN6ih4Ct0goaMsILRZpQLQMBOn2h2kA6cSJLQTtQhdy2mzBgXVKbgGPWupfgYl1WkExApkrKUBEvTUqUwVxJ2oBST4w4KnRvBqrO92qsFAXZ1qMFYCWPgDWQc3PBSHazkscU0qVDDxxQFEUCPGEhNAKccOC6AkyA0/QAOBJItKQFg1aFBdypUikIHCuNVs880456zzRQEBADs=";

		public static void ProcessAADObjects([TimerTrigger("0 0 1 * * 1-5")] TimerInfo info, TextWriter log)  // 01:00 every weekday
		{
			try
			{
				AdalClient client = new AdalClient(appConfig, CredentialType.Client, null);

				var appFactory = new ApplicationFactory();
				var appManager = appFactory.CreateInstance(client);
				var appsExpiringSoon = appManager.GetExpiredApplicationInDays(appConfig.NotificationInterval);
				var appsExpired = appManager.GetAllExpired();

				var spFactory = new ServicePrincipalFactory();
				var spManager = spFactory.CreateInstance(client);
				var spExpiringSoon = spManager.GetExpiredPrincipalsInDays(appConfig.NotificationInterval);
				var spExpired = spManager.GetExpiredPrincipals();

				if (appsExpiringSoon.Count > 0 || appsExpired.Count > 0 ||
						spExpiringSoon.Count > 0 || spExpired.Count > 0)
				{
					// Create the Connector Card payload
					var card = new ConnectorCard
					{
						Summary = "Expiring Credential Status",
						Title = "PnP Tools - Tenant Information Portal",
						Text = "The credentials for the following have expired or will expire soon.",
						ThemeColor = "#FF0000"
					};
					card.PotentialAction.Add(
						new ViewAction
						{
							Name = "View in Tenant Information Portal",
							Target = new string[] { appConfig.PortalUrl }
						}
					);


					List<Fact> facts = null;
					if (appsExpiringSoon.Count>0)
					{
						facts = appsExpiringSoon.Select(a => new Fact { Name = a.DiplayName, Value = a.EndDate.ToString() }).ToList();
						card.Sections.Add(CreateSection($"Applications Expiring Soon ({appConfig.NotificationInterval}) days", ExpiringSoonImage, facts));
					}

					if (appsExpired.Count>0)
					{
						facts = appsExpired.Select(a => new Fact { Name = a.DiplayName, Value = a.EndDate.ToString() }).ToList();
						card.Sections.Add(CreateSection("Applications Expired", ExpiredImage, facts));
					}

					if (spExpiringSoon.Count>0)
					{
						facts = spExpiringSoon.Select(sp => new Fact { Name = sp.DisplayName, Value = sp.EndDate.ToString() }).ToList();
						card.Sections.Add(CreateSection($"Service Principals Expiring Soon ({appConfig.NotificationInterval}) days", ExpiringSoonImage, facts));
					}

					if (spExpired.Count>0)
					{
						facts = spExpired.Select(sp => new Fact { Name = sp.DisplayName, Value = sp.EndDate.ToString() }).ToList();
						card.Sections.Add(CreateSection("Service Principals Expired", ExpiredImage, facts));
					}

					var requestBody = JsonConvert.SerializeObject(card, null, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

					// Make POST to webhook URL
					var status = HttpHelper.PostJsonMessage(appConfig.ConnectorUrl, requestBody);
				}

			}
			catch (Exception ex)
			{
				log.Write(ex.ToString());
			}

		}

		private static Section CreateSection(string ActivityTitle, string ImageUrl, List<Fact> Facts)
		{
			Section appSection = new Section() { ActivityTitle = ActivityTitle, ActivityImage = ImageUrl };

			foreach (var _fact in Facts)
			{
				appSection.Facts.Add(_fact);
			}

			return appSection;
		}
	}
}
