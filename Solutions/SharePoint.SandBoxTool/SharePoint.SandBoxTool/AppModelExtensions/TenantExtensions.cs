using System;
using System.Collections.Generic;
using Microsoft.Online.SharePoint.TenantAdministration;
using SharePoint.SandBoxTool;

namespace Microsoft.SharePoint.Client
{
    public static partial class TenantExtensions
    {
        const string SITE_STATUS_ACTIVE = "Active"; 
        const string SITE_STATUS_CREATING = "Creating"; 
        const string SITE_STATUS_RECYCLED = "Recycled"; 

#if !ONPREMISES

        #region Site enumeration
        /// <summary>
        /// Returns all site collections in the current Tenant based on a startIndex. IncludeDetail adds additional properties to the SPSite object. EndIndex is the maximum number based on chunkcs of 300.
        /// </summary>
        /// <param name="tenant">Tenant object to operate against</param>
        /// <param name="startIndex">Start getting site collections from this index. Defaults to 0</param>
        /// <param name="endIndex">The index of the last site. Defaults to 100.000</param>
        /// <param name="includeDetail">Option to return a limited set of data</param>
        /// <returns>An IList of SiteEntity objects</returns>
        public static IList<SiteEntity> GetSiteCollections(this Tenant tenant, int startIndex = 0, int endIndex = 100000, bool includeDetail = true)
        {
            var sites = new List<SiteEntity>();

            // O365 Tenant Site Collection limit is 500.000 (https://support.office.com/en-us/article/SharePoint-Online-software-boundaries-and-limits-8f34ff47-b749-408b-abc0-b605e1f6d498?CTT=1&CorrelationId=1928c530-fc12-4134-ada5-8ed2c2ec01fc&ui=en-US&rs=en-US&ad=US), 
            // but let's limit to 100.000. Note that GetSiteProperties returns 300 per request.
            for (int i = startIndex; i < endIndex; i += 300)
            {
                var props = tenant.GetSiteProperties(i, includeDetail);
                tenant.Context.Load(props);
                tenant.Context.ExecuteQueryRetry();

                foreach (var prop in props)
                {
                    var siteEntity = new SiteEntity();
                    siteEntity.Lcid = prop.Lcid;
                    siteEntity.SiteOwnerLogin = prop.Owner;
                    siteEntity.StorageMaximumLevel = prop.StorageMaximumLevel;
                    siteEntity.StorageWarningLevel = prop.StorageWarningLevel;
                    siteEntity.Template = prop.Template;
                    siteEntity.TimeZoneId = prop.TimeZoneId;
                    siteEntity.Title = prop.Title;
                    siteEntity.Url = prop.Url;
                    siteEntity.UserCodeMaximumLevel = prop.UserCodeMaximumLevel;
                    siteEntity.UserCodeWarningLevel = prop.UserCodeWarningLevel;
                    siteEntity.CurrentResourceUsage = prop.CurrentResourceUsage;
                    siteEntity.LastContentModifiedDate = prop.LastContentModifiedDate;
                    siteEntity.StorageUsage = prop.StorageUsage;
                    siteEntity.WebsCount = prop.WebsCount;
                    sites.Add(siteEntity);
                }

                if (props.Count < 300) break; //exit for loop if there are no more site collections
            }

            return sites;
        }

        ///// <summary>
        ///// Get OneDrive site collections by iterating through all user profiles.
        ///// </summary>
        ///// <param name="tenant"></param>
        ///// <returns>List of <see cref="SiteEntity"/> objects containing site collection info.</returns>
        //public static IList<SiteEntity> GetOneDriveSiteCollections(this Tenant tenant)
        //{
        //    var sites = new List<SiteEntity>();
        //    var svcClient = GetUserProfileServiceClient(tenant);

        //    // get all user profiles
        //    var userProfileResult = svcClient.GetUserProfileByIndex(-1);
        //    var profileCount = svcClient.GetUserProfileCount();

        //    while (int.Parse(userProfileResult.NextValue) != -1)
        //    {
        //        var personalSpaceProperty = userProfileResult.UserProfile.FirstOrDefault(p => p.Name == "PersonalSpace");

        //        if (personalSpaceProperty != null)
        //        {
        //            if (personalSpaceProperty.Values.Any())
        //            {
        //                var usernameProperty = userProfileResult.UserProfile.FirstOrDefault(p => p.Name == "UserName");
        //                var nameProperty = userProfileResult.UserProfile.FirstOrDefault(p => p.Name == "PreferredName");
        //                var url = personalSpaceProperty.Values[0].Value as string;
        //                var name = nameProperty.Values[0].Value as string;
        //                SiteEntity siteEntity = new SiteEntity();
        //                siteEntity.Url = url;
        //                siteEntity.Title = name;
        //                siteEntity.SiteOwnerLogin = usernameProperty.Values[0].Value as string;
        //                sites.Add(siteEntity);
        //            }
        //        }

        //        userProfileResult = svcClient.GetUserProfileByIndex(int.Parse(userProfileResult.NextValue));
        //    }

        //    return sites;
        //}

        ///// <summary>
        ///// Gets the UserProfileService proxy to enable calls to the UPA web service.
        ///// </summary>
        ///// <param name="tenant"></param>
        ///// <returns>UserProfileService web service client</returns>
        //public static UserProfileService GetUserProfileServiceClient(this Tenant tenant)
        //{
        //    var client = new UserProfileService();

        //    client.Url = tenant.Context.Url + "/_vti_bin/UserProfileService.asmx";
        //    client.UseDefaultCredentials = false;
        //    client.Credentials = tenant.Context.Credentials;

        //    if (tenant.Context.Credentials is SharePointOnlineCredentials)
        //    {
        //        var creds = (SharePointOnlineCredentials)tenant.Context.Credentials;
        //        var authCookie = creds.GetAuthenticationCookie(new Uri(tenant.Context.Url));
        //        var cookieContainer = new CookieContainer();

        //        cookieContainer.SetCookies(new Uri(tenant.Context.Url), authCookie);
        //        client.CookieContainer = cookieContainer;
        //    }
        //    return client;
        //}
        #endregion

        #region Private helper methods
        //private static void WaitForIsComplete(Tenant tenant, SpoOperation op)
        //{
        //    while (!op.IsComplete)
        //    {
        //        Thread.Sleep(op.PollingInterval);
        //        op.RefreshLoad();
        //        if (!op.IsComplete)
        //        {
        //            try
        //            {
        //                tenant.Context.ExecuteQueryRetry();
        //            }
        //            catch (WebException webEx)
        //            {
        //                // Context connection gets closed after action completed.
        //                // Calling ExecuteQuery again returns an error which can be ignored
        //                Log.Warning(CoreResources.TenantExtensions_ClosedContextWarning, webEx.Message);
        //            }
        //        }
        //    }
        //}

        private static bool IsCannotGetSiteException(Exception ex)
        {
            if (ex is ServerException)
            {
                if (((ServerException)ex).ServerErrorCode == -1 && ((ServerException)ex).ServerErrorTypeName.Equals("Microsoft.Online.SharePoint.Common.SpoNoSiteException", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private static bool IsUnableToAccessSiteException(Exception ex)
        {
            if (ex is ServerException)
            {
                if (
                     (((ServerException)ex).ServerErrorCode == -2147024809 && ((ServerException)ex).ServerErrorTypeName.Equals("System.ArgumentException", StringComparison.InvariantCultureIgnoreCase)) ||
                     (((ServerException)ex).ServerErrorCode == -1 && ((ServerException)ex).ServerErrorTypeName.Equals("Microsoft.Online.SharePoint.Common.SpoNoSiteException", StringComparison.InvariantCultureIgnoreCase))                    
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private static bool IsCannotRemoveSiteException(Exception ex)
        {
            if (ex is ServerException)
            {
                if (((ServerException)ex).ServerErrorCode == -1 
                    && (
                        ((ServerException)ex).ServerErrorTypeName.Equals("Microsoft.Online.SharePoint.Common.SpoException", StringComparison.InvariantCultureIgnoreCase) ||
                        ((ServerException)ex).ServerErrorTypeName.Equals("Microsoft.Online.SharePoint.Common.SpoNoSiteException", StringComparison.InvariantCultureIgnoreCase))
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion
#else

#endif
    }
}
