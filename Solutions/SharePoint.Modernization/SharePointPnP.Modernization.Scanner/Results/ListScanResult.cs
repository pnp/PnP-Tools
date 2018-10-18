using Microsoft.SharePoint.Client;
using SharePoint.Scanning.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Scanner.Results
{
    public class ListScanResult: Scan
    {
        public ListScanResult()
        {
            this.XsltViewWebPartCompatibility = new XsltViewWebPartCompatibility();
        }

        public string ListUrl { get; set; }
        public string ListTitle { get; set; }
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

        public bool OnlyBlockedByOOBReasons
        {
            get
            {
                if ((this.XsltViewWebPartCompatibility.BlockedByManagedMetadataNavFeature ||
                    this.XsltViewWebPartCompatibility.BlockedByBusinessDataField ||
                    this.XsltViewWebPartCompatibility.BlockedByGeoLocationField ||
                    this.XsltViewWebPartCompatibility.BlockedByPublishingField ||
                    this.XsltViewWebPartCompatibility.BlockedByTaskOutcomeField ||
                    this.XsltViewWebPartCompatibility.BlockedByListBaseTemplate ||
                    this.XsltViewWebPartCompatibility.BlockedByViewType)
                    && !(this.BlockedAtSiteLevel ||
                         this.BlockedAtWebLevel ||
                         this.BlockedAtListLevel ||
                         this.BlockedByNotBeingAbleToLoadPage ||
                         this.BlockedByZeroOrMultipleWebParts ||
                         this.XsltViewWebPartCompatibility.BlockedByJSLinkField ||
                         this.XsltViewWebPartCompatibility.BlockedByListCustomAction ||
                         this.XsltViewWebPartCompatibility.BlockedByJSLink ||
                         this.XsltViewWebPartCompatibility.BlockedByXsl ||
                         this.XsltViewWebPartCompatibility.BlockedByXslLink)
                   )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}
