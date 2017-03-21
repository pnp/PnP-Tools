using System;
using System.Collections.Generic;
using Microsoft.Online.SharePoint.TenantAdministration;
using SharePoint.UIExperience.Scanner;

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
        public static IList<SiteEntity> GetSiteCollections(this Tenant tenant, int startIndex = 0, int endIndex = 500000, bool includeDetail = true, bool includeOD4BSites = false)
        {
            var sites = new List<SiteEntity>();
            SPOSitePropertiesEnumerable props = null;

            while (props == null || props.NextStartIndexFromSharePoint != null)
            {

                // approach to be used as of Feb 2017
                SPOSitePropertiesEnumerableFilter filter = new SPOSitePropertiesEnumerableFilter()
                {
                    IncludePersonalSite = includeOD4BSites ? PersonalSiteFilter.Include : PersonalSiteFilter.UseServerDefault,
                    StartIndex = props == null ? null : props.NextStartIndexFromSharePoint,
                    IncludeDetail = includeDetail
                };
                props = tenant.GetSitePropertiesFromSharePointByFilters(filter);

                // Previous approach, being replaced by GetSitePropertiesFromSharePointByFilters which also allows to fetch OD4B sites
                //props = tenant.GetSitePropertiesFromSharePoint(props == null ? null : props.NextStartIndexFromSharePoint, includeDetail);
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
                    SiteLockState lockState;
                    if (Enum.TryParse(prop.LockState, out lockState))
                    {
                        siteEntity.LockState = lockState;
                    }
                    sites.Add(siteEntity);
                }
            }

            return sites;
        }
        #endregion

        #region Private helper methods
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
