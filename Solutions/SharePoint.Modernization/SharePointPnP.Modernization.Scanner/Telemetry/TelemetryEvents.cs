namespace SharePoint.Modernization.Scanner.Telemetry
{
    /// <summary>
    /// Main telemetry events
    /// </summary>
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

    /// <summary>
    /// Properties that we collect for the start event
    /// </summary>
    public enum ScanStart
    {
        Mode,
        AuthenticationModel,
        SiteSelectionModel
    }

    /// <summary>
    /// Properties that we collect for the crash event
    /// </summary>
    public enum ScanCrash
    {
        ExceptionMessage,
        StackTrace
    }

    /// <summary>
    /// Properties that we collect for the done event
    /// </summary>
    public enum ScanDone
    {
        Duration
    }

    /// <summary>
    /// Measures we collect for group connect
    /// </summary>
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

    /// <summary>
    /// Measures we collect for page transformation
    /// </summary>
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

    /// <summary>
    /// Measures we collect for modern list and libraries
    /// </summary>
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

    /// <summary>
    /// Measures we collect for publishing portals
    /// </summary>
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
