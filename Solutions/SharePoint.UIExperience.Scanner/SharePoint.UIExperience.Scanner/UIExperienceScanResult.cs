using Microsoft.SharePoint.Client;
using System;

namespace SharePoint.UIExperience.Scanner
{
    /// <summary>
    /// Object holding scan results
    /// </summary>
    public abstract class UIExperienceScanResult
    {
        public string Url { get; set; }
        public string SiteUrl { get; set; }
    }

    public class ListBaseResult : UIExperienceScanResult
    {
        public string ListTitle { get; set; }
    }

    public class CustomActionsResult : ListBaseResult
    {
        public string Title { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public UserCustomActionRegistrationType RegistrationType { get; set; }

        public string RegistrationId { get; set; }

        //public string ImageMaps { get; set; }

        public string ScriptBlock { get; set; }

        public string ScriptSrc { get; set; }

        public string CommandActions { get; set; }
        public string Problem { get; set; }

    }


    public class AlternateCSSResult : UIExperienceScanResult
    {
        public string AlternateCSS { get; set; }
    }
    public class MasterPageResult : UIExperienceScanResult
    {
        public string MasterPage { get; set; }
        public string CustomMasterPage { get; set; }
    }

    // New classes for Page / List / Customizations consolidated reports
    public class PageResult : UIExperienceScanResult
    {
        public bool BlockedViaDisabledModernPageWebFeature { get; set; }
    }

    public class CustomizationResult : UIExperienceScanResult
    {
        public string IgnoredMasterPage { get; set; }
        public string IgnoredCustomMasterPage { get; set; }
        public string IgnoredAlternateCSS { get; set; }
        public bool IgnoredCustomAction { get; set; }
    }

    public class ListResult: ListBaseResult
    {
        public ListResult()
        {
            this.XsltViewWebPartCompatibility = new XsltViewWebPartCompatibility();
        }

        public bool BlockedAtTenantLevel { get; set; }
        public bool BlockedAtSiteLevel { get; set; }
        public bool BlockedAtWebLevel { get; set; }
        public bool BlockedAtListLevel { get; set; }
        public ListExperience ListExperience { get; set; }
        public ListPageRenderType PageRenderType { get; set; }
        public bool BlockedByNotBeingAbleToLoadPage { get; set; }
        public string BlockedByNotBeingAbleToLoadPageException { get; set; }
        public bool BlockedByZeroOrMultipleWebParts { get; set; }
        public XsltViewWebPartCompatibility XsltViewWebPartCompatibility { get; set; }

        public bool WorksInModern
        {
            get
            {
                // Verify list blocking
                if (this.BlockedAtTenantLevel ||
                    this.BlockedAtSiteLevel ||
                    this.BlockedAtWebLevel ||
                    this.BlockedAtListLevel ||
                    this.BlockedByNotBeingAbleToLoadPage ||
                    this.BlockedByZeroOrMultipleWebParts)
                {
                    return false;
                }

                // Verify web part blocking
                if (this.XsltViewWebPartCompatibility != null)
                {
                    if (this.XsltViewWebPartCompatibility.BlockedByBusinessDataField ||
                        this.XsltViewWebPartCompatibility.BlockedByGeoLocationField ||
                        this.XsltViewWebPartCompatibility.BlockedByJSLinkField ||
                        this.XsltViewWebPartCompatibility.BlockedByPublishingField ||
                        this.XsltViewWebPartCompatibility.BlockedByTaskOutcomeField ||
                        this.XsltViewWebPartCompatibility.BlockedByListBaseTemplate ||
                        this.XsltViewWebPartCompatibility.BlockedByListCustomAction ||
                        this.XsltViewWebPartCompatibility.BlockedByManagedMetadataNavFeature ||
                        this.XsltViewWebPartCompatibility.BlockedByViewType ||
                        this.XsltViewWebPartCompatibility.BlockedByJSLink ||
                        this.XsltViewWebPartCompatibility.BlockedByXsl ||
                        this.XsltViewWebPartCompatibility.BlockedByXslLink)
                    {
                        return false;
                    }
                }

                // We're good
                return true;
            }
        }
    }

    public class XsltViewWebPartCompatibility
    {
        public bool BlockedByJSLink { get; set; }
        public string JSLink { get; set; }
        public bool BlockedByXslLink { get; set; }
        public string XslLink { get; set; }
        public bool BlockedByXsl { get; set; }
        public bool BlockedByJSLinkField { get; set; }
        public string JSLinkFields { get; set; }
        public bool BlockedByBusinessDataField { get; set; }
        public string BusinessDataFields { get; set; }
        public bool BlockedByTaskOutcomeField { get; set; }
        public string TaskOutcomeFields { get; set; }
        public bool BlockedByPublishingField { get; set; }
        public string PublishingFields { get; set; }
        public bool BlockedByGeoLocationField { get; set; }
        public string GeoLocationFields { get; set; }
        public bool BlockedByListCustomAction { get; set; }
        public string ListCustomActions { get; set; }
        public bool BlockedByManagedMetadataNavFeature { get; set; }
        public bool BlockedByViewType { get; set; }
        public string ViewType { get; set; }
        public bool BlockedByListBaseTemplate { get; set; }
        public int ListBaseTemplate { get; set; }
    }


}
