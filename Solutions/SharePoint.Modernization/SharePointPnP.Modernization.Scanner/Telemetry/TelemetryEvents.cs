using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Scanner.Telemetry
{
    public enum TelemetryEvents
    {
        ScanStart,
        ScanDone,
        ScanCrash,
        GroupConnect,
        List,
        Pages,
        PublishingPortals
    }

    public enum ScanStart
    {
        Mode,
        AuthenticationModel,
        SiteSelectionModel
    }

    public enum ScanCrash
    {
        ExceptionMessage,
        StackTrace
    }

    public enum ScanDone
    {
        Duration
    }

    public enum GroupConnectResults
    {
        Sites,
        Webs,
        ReadyForGroupConnect,
        BlockingReason,
        WebTemplate,
        Warning,
        ModernUIWarning,
        PermissionWarning
    }

    public enum PagesResults
    {
        Sites,
        Webs,
        Pages,
        PageType,
        PageLayout,
        IsHomePage,
        WebPartMapping,
        UnMappedWebParts
    }

    public enum ListResults
    {
        Sites,
        Webs,
        Lists,
        OnlyBlockedByOOB,
        RenderType,
        ListExperience,
        BaseTemplateNotWorking,
        ViewTypeNotWorking,
        MultipleWebParts,
        JSLinkWebPart,
        JSLinkField,
        XslLink,
        Xsl,
        ListCustomAction,
        PublishingField,
        SiteBlocking,
        WebBlocking,
        ListBlocking
    }

    public enum PublishingResults
    {
        Sites,
        Webs,
        Pages,
        SiteLevel,
        Complexity,
        WebTemplates,
        GlobalNavigation,
        CurrentNavigation,
        CustomSiteMasterPage,
        CustomSystemMasterPage,
        AlternateCSS,
        IncompatibleUserCustomActions,
        CustomPageLayouts,
        PageApproval,
        PageApprovalWorkflow,
        ScheduledPublishing,
        AudienceTargeting,
        Languages,
        VariationLabels
    }

}
