using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Semi automatically generated classes that represent feature xml
/// </summary>

namespace SharePoint.SolutionAnalyzer
{
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    [XmlRoot("Feature", Namespace = "http://schemas.microsoft.com/sharepoint/", IsNullable = false)]
    public partial class FeatureDefinition
    {

        private ElementManifestReferences elementManifestsField;

        private FeaturePropertyDefinition[] propertiesField;

        private FeatureActivationDependencyDefinition[] activationDependenciesField;

        private string idField;

        private string titleField;

        private string descriptionField;

        private string versionField;

        private FeatureScope scopeField;

        private string receiverAssemblyField;

        private string receiverClassField;

        private string creatorField;

        private string defaultResourceFileField;

        private TRUEFALSE hiddenField;

        private bool hiddenFieldSpecified;

        private string solutionIdField;

        private TRUEFALSE activateOnDefaultField;

        private bool activateOnDefaultFieldSpecified;

        private TRUEFALSE autoActivateInCentralAdminField;

        private bool autoActivateInCentralAdminFieldSpecified;

        private TRUEFALSE alwaysForceInstallField;

        private bool alwaysForceInstallFieldSpecified;

        private TRUEFALSE requireResourcesField;

        private bool requireResourcesFieldSpecified;

        private string imageUrlField;

        private string imageUrlAltTextField;

        /// <remarks/>
        public ElementManifestReferences ElementManifests
        {
            get
            {
                return this.elementManifestsField;
            }
            set
            {
                this.elementManifestsField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("Property", IsNullable = false)]
        public FeaturePropertyDefinition[] Properties
        {
            get
            {
                return this.propertiesField;
            }
            set
            {
                this.propertiesField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("ActivationDependency", IsNullable = false)]
        public FeatureActivationDependencyDefinition[] ActivationDependencies
        {
            get
            {
                return this.activationDependenciesField;
            }
            set
            {
                this.activationDependenciesField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public FeatureScope Scope
        {
            get
            {
                return this.scopeField;
            }
            set
            {
                this.scopeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ReceiverAssembly
        {
            get
            {
                return this.receiverAssemblyField;
            }
            set
            {
                this.receiverAssemblyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ReceiverClass
        {
            get
            {
                return this.receiverClassField;
            }
            set
            {
                this.receiverClassField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Creator
        {
            get
            {
                return this.creatorField;
            }
            set
            {
                this.creatorField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string DefaultResourceFile
        {
            get
            {
                return this.defaultResourceFileField;
            }
            set
            {
                this.defaultResourceFileField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Hidden
        {
            get
            {
                return this.hiddenField;
            }
            set
            {
                this.hiddenField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HiddenSpecified
        {
            get
            {
                return this.hiddenFieldSpecified;
            }
            set
            {
                this.hiddenFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string SolutionId
        {
            get
            {
                return this.solutionIdField;
            }
            set
            {
                this.solutionIdField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ActivateOnDefault
        {
            get
            {
                return this.activateOnDefaultField;
            }
            set
            {
                this.activateOnDefaultField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ActivateOnDefaultSpecified
        {
            get
            {
                return this.activateOnDefaultFieldSpecified;
            }
            set
            {
                this.activateOnDefaultFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE AutoActivateInCentralAdmin
        {
            get
            {
                return this.autoActivateInCentralAdminField;
            }
            set
            {
                this.autoActivateInCentralAdminField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AutoActivateInCentralAdminSpecified
        {
            get
            {
                return this.autoActivateInCentralAdminFieldSpecified;
            }
            set
            {
                this.autoActivateInCentralAdminFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE AlwaysForceInstall
        {
            get
            {
                return this.alwaysForceInstallField;
            }
            set
            {
                this.alwaysForceInstallField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AlwaysForceInstallSpecified
        {
            get
            {
                return this.alwaysForceInstallFieldSpecified;
            }
            set
            {
                this.alwaysForceInstallFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE RequireResources
        {
            get
            {
                return this.requireResourcesField;
            }
            set
            {
                this.requireResourcesField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RequireResourcesSpecified
        {
            get
            {
                return this.requireResourcesFieldSpecified;
            }
            set
            {
                this.requireResourcesFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ImageUrl
        {
            get
            {
                return this.imageUrlField;
            }
            set
            {
                this.imageUrlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ImageUrlAltText
        {
            get
            {
                return this.imageUrlAltTextField;
            }
            set
            {
                this.imageUrlAltTextField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ElementManifestReferences
    {

        private ElementManifestReference[] itemsFiles;
        private ElementManifestReference[] itemsManifests;

        private ItemsChoiceType[] itemsElementNameField;

        /// <remarks/>
        [XmlElement("ElementFile", typeof(ElementManifestReference))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public ElementManifestReference[] ItemFiles
        {
            get
            {
                return this.itemsFiles;
            }
            set
            {
                this.itemsFiles = value;
            }
        }

        [XmlElement("ElementManifest", typeof(ElementManifestReference))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public ElementManifestReference[] ItemManifests
        {
            get
            {
                return this.itemsManifests;
            }
            set
            {
                this.itemsManifests = value;
            }
        }

        /// <remarks/>
        [XmlElement("ItemsElementName")]
        [XmlIgnore()]
        public ItemsChoiceType[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ElementManifestReference
    {

        private string locationField;

        /// <remarks/>
        [XmlAttribute()]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ContentTypeDocumentTemplateDefinition
    {

        private string targetNameField;

        private string valueField;

        /// <remarks/>
        [XmlAttribute()]
        public string TargetName
        {
            get
            {
                return this.targetNameField;
            }
            set
            {
                this.targetNameField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class URLDefinition
    {

        private string cmdField;

        private TRUEFALSE noIDField;

        private bool noIDFieldSpecified;

        private string valueField;

        /// <remarks/>
        [XmlAttribute()]
        public string Cmd
        {
            get
            {
                return this.cmdField;
            }
            set
            {
                this.cmdField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE NoID
        {
            get
            {
                return this.noIDField;
            }
            set
            {
                this.noIDField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool NoIDSpecified
        {
            get
            {
                return this.noIDFieldSpecified;
            }
            set
            {
                this.noIDFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum TRUEFALSE
    {

        /// <remarks/>
        TRUE,

        /// <remarks/>
        FALSE,

        /// <remarks/>
        True,

        /// <remarks/>
        False,

        /// <remarks/>
        @true,

        /// <remarks/>
        @false,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ContentTypeBindingDefinition
    {

        private string contentTypeIdField;

        private string listUrlField;

        /// <remarks/>
        [XmlAttribute()]
        public string ContentTypeId
        {
            get
            {
                return this.contentTypeIdField;
            }
            set
            {
                this.contentTypeIdField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ListUrl
        {
            get
            {
                return this.listUrlField;
            }
            set
            {
                this.listUrlField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FeatureSiteTemplateAssociationDefinition
    {

        private FeaturePropertyDefinition[] propertyField;

        private string idField;

        private string templateNameField;

        /// <remarks/>
        [XmlElement("Property")]
        public FeaturePropertyDefinition[] Property
        {
            get
            {
                return this.propertyField;
            }
            set
            {
                this.propertyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string TemplateName
        {
            get
            {
                return this.templateNameField;
            }
            set
            {
                this.templateNameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FeaturePropertyDefinition
    {

        private string keyField;

        private string valueField;

        /// <remarks/>
        [XmlAttribute()]
        public string Key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class DocumentConverterDefinition
    {

        private string idField;

        private string nameField;

        private string appField;

        private string fromField;

        private string toField;

        private string converterUIPageField;

        private string converterSpecificSettingsUIField;

        private string converterSettingsForContentTypeField;

        /// <remarks/>
        [XmlAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string App
        {
            get
            {
                return this.appField;
            }
            set
            {
                this.appField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string From
        {
            get
            {
                return this.fromField;
            }
            set
            {
                this.fromField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string To
        {
            get
            {
                return this.toField;
            }
            set
            {
                this.toField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ConverterUIPage
        {
            get
            {
                return this.converterUIPageField;
            }
            set
            {
                this.converterUIPageField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ConverterSpecificSettingsUI
        {
            get
            {
                return this.converterSpecificSettingsUIField;
            }
            set
            {
                this.converterSpecificSettingsUIField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ConverterSettingsForContentType
        {
            get
            {
                return this.converterSettingsForContentTypeField;
            }
            set
            {
                this.converterSettingsForContentTypeField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class WorkflowDefinition
    {

        private object associationDataField;

        private object metaDataField;

        private object categoriesField;

        private string[] textField;

        private string titleField;

        private string nameField;

        private string codeBesideAssemblyField;

        private string codeBesideClassField;

        private string descriptionField;

        private string idField;

        private string engineClassField;

        private string engineAssemblyField;

        private string associationUrlField;

        private string instantiationUrlField;

        private string modificationUrlField;

        private string statusUrlField;

        private string taskListContentTypeIdField;

        /// <remarks/>
        public object AssociationData
        {
            get
            {
                return this.associationDataField;
            }
            set
            {
                this.associationDataField = value;
            }
        }

        /// <remarks/>
        public object MetaData
        {
            get
            {
                return this.metaDataField;
            }
            set
            {
                this.metaDataField = value;
            }
        }

        /// <remarks/>
        public object Categories
        {
            get
            {
                return this.categoriesField;
            }
            set
            {
                this.categoriesField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string CodeBesideAssembly
        {
            get
            {
                return this.codeBesideAssemblyField;
            }
            set
            {
                this.codeBesideAssemblyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string CodeBesideClass
        {
            get
            {
                return this.codeBesideClassField;
            }
            set
            {
                this.codeBesideClassField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string EngineClass
        {
            get
            {
                return this.engineClassField;
            }
            set
            {
                this.engineClassField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string EngineAssembly
        {
            get
            {
                return this.engineAssemblyField;
            }
            set
            {
                this.engineAssemblyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string AssociationUrl
        {
            get
            {
                return this.associationUrlField;
            }
            set
            {
                this.associationUrlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string InstantiationUrl
        {
            get
            {
                return this.instantiationUrlField;
            }
            set
            {
                this.instantiationUrlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ModificationUrl
        {
            get
            {
                return this.modificationUrlField;
            }
            set
            {
                this.modificationUrlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string StatusUrl
        {
            get
            {
                return this.statusUrlField;
            }
            set
            {
                this.statusUrlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string TaskListContentTypeId
        {
            get
            {
                return this.taskListContentTypeIdField;
            }
            set
            {
                this.taskListContentTypeIdField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ListInstanceDefinition
    {

        private DataDefinition[] dataField;

        private string[] textField;

        private string descriptionField;

        private string featureIdField;

        private string idField;

        private string titleField;

        private TRUEFALSE onQuickLaunchField;

        private bool onQuickLaunchFieldSpecified;

        private TRUEFALSE rootWebOnlyField;

        private bool rootWebOnlyFieldSpecified;

        private string quickLaunchUrlField;

        private string documentTemplateField;

        private int templateTypeField;

        private bool templateTypeFieldSpecified;

        private string urlField;

        /// <remarks/>
        [XmlElement("Data")]
        public DataDefinition[] Data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string FeatureId
        {
            get
            {
                return this.featureIdField;
            }
            set
            {
                this.featureIdField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE OnQuickLaunch
        {
            get
            {
                return this.onQuickLaunchField;
            }
            set
            {
                this.onQuickLaunchField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool OnQuickLaunchSpecified
        {
            get
            {
                return this.onQuickLaunchFieldSpecified;
            }
            set
            {
                this.onQuickLaunchFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE RootWebOnly
        {
            get
            {
                return this.rootWebOnlyField;
            }
            set
            {
                this.rootWebOnlyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RootWebOnlySpecified
        {
            get
            {
                return this.rootWebOnlyFieldSpecified;
            }
            set
            {
                this.rootWebOnlyFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string QuickLaunchUrl
        {
            get
            {
                return this.quickLaunchUrlField;
            }
            set
            {
                this.quickLaunchUrlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string DocumentTemplate
        {
            get
            {
                return this.documentTemplateField;
            }
            set
            {
                this.documentTemplateField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int TemplateType
        {
            get
            {
                return this.templateTypeField;
            }
            set
            {
                this.templateTypeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TemplateTypeSpecified
        {
            get
            {
                return this.templateTypeFieldSpecified;
            }
            set
            {
                this.templateTypeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class DataDefinition
    {

        private FieldDataDefinition[][] rowsField;

        /// <remarks/>
        [XmlArrayItem("Row", IsNullable = false)]
        [XmlArrayItem("Field", IsNullable = false, NestingLevel = 1)]
        public FieldDataDefinition[][] Rows
        {
            get
            {
                return this.rowsField;
            }
            set
            {
                this.rowsField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FieldDataDefinition
    {

        private string nameField;

        private string valueField;

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class GroupMigratorDefinition
    {

        private string assemblyField;

        private string classField;

        /// <remarks/>
        [XmlAttribute()]
        public string Assembly
        {
            get
            {
                return this.assemblyField;
            }
            set
            {
                this.assemblyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class UserMigratorDefinition
    {

        private string assemblyField;

        private string classField;

        /// <remarks/>
        [XmlAttribute()]
        public string Assembly
        {
            get
            {
                return this.assemblyField;
            }
            set
            {
                this.assemblyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ReceiverDefinition
    {

        private string nameField;

        private string typeField;

        private string sequenceNumberField;

        private string assemblyField;

        private string classField;

        private string dataField;

        private string filterField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public string SequenceNumber
        {
            get
            {
                return this.sequenceNumberField;
            }
            set
            {
                this.sequenceNumberField = value;
            }
        }

        /// <remarks/>
        public string Assembly
        {
            get
            {
                return this.assemblyField;
            }
            set
            {
                this.assemblyField = value;
            }
        }

        /// <remarks/>
        public string Class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }

        /// <remarks/>
        public string Data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }

        /// <remarks/>
        public string Filter
        {
            get
            {
                return this.filterField;
            }
            set
            {
                this.filterField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ReceiverDefinitionCollection
    {

        private ReceiverDefinition[] receiverField;

        private string[] textField;

        private int listTemplateIdField;

        private bool listTemplateIdFieldSpecified;

        private string listTemplateOwnerField;

        private string listUrl;

        /// <remarks/>
        [XmlElement("Receiver")]
        public ReceiverDefinition[] Receiver
        {
            get
            {
                return this.receiverField;
            }
            set
            {
                this.receiverField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int ListTemplateId
        {
            get
            {
                return this.listTemplateIdField;
            }
            set
            {
                this.listTemplateIdField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ListUrl
        {
            get
            {
                return this.listUrl;
            }
            set
            {
                this.listUrl = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ListTemplateIdSpecified
        {
            get
            {
                return this.listTemplateIdFieldSpecified;
            }
            set
            {
                this.listTemplateIdFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ListTemplateOwner
        {
            get
            {
                return this.listTemplateOwnerField;
            }
            set
            {
                this.listTemplateOwnerField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class SimplePropertyDefinition
    {

        private string nameField;

        private string valueField;

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class DelegateControlDefinition
    {

        private SimplePropertyDefinition[] propertyField;

        private string[] textField;

        private int sequenceField;

        private bool sequenceFieldSpecified;

        private string idField;

        private string controlAssemblyField;

        private string controlClassField;

        private string controlSrcField;

        /// <remarks/>
        [XmlElement("Property")]
        public SimplePropertyDefinition[] Property
        {
            get
            {
                return this.propertyField;
            }
            set
            {
                this.propertyField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int Sequence
        {
            get
            {
                return this.sequenceField;
            }
            set
            {
                this.sequenceField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool SequenceSpecified
        {
            get
            {
                return this.sequenceFieldSpecified;
            }
            set
            {
                this.sequenceFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ControlAssembly
        {
            get
            {
                return this.controlAssemblyField;
            }
            set
            {
                this.controlAssemblyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ControlClass
        {
            get
            {
                return this.controlClassField;
            }
            set
            {
                this.controlClassField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ControlSrc
        {
            get
            {
                return this.controlSrcField;
            }
            set
            {
                this.controlSrcField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class XmlDocumentDefinitions
    {
        public object[] itemsField;

        [XmlElement("XmlDocument", typeof(XmlDocumentDefinition))]

        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        //private string[] textField;
        ///// <remarks/>
        //[XmlText()]
        //public string[] Text
        //{
        //    get
        //    {
        //        return this.textField;
        //    }
        //    set
        //    {
        //        this.textField = value;
        //    }
        //}
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ContentTypeLink
    {

        private string idField;

        /// <remarks/>
        [XmlAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FieldExpressionFilterDefinition
    {

        private string fieldIdField;

        private string expressionField;

        private string fieldValueField;

        /// <remarks/>
        [XmlAttribute()]
        public string FieldId
        {
            get
            {
                return this.fieldIdField;
            }
            set
            {
                this.fieldIdField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Expression
        {
            get
            {
                return this.expressionField;
            }
            set
            {
                this.expressionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string FieldValue
        {
            get
            {
                return this.fieldValueField;
            }
            set
            {
                this.fieldValueField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FieldChangedFilterDefinition
    {

        private string fieldIdField;

        /// <remarks/>
        [XmlAttribute()]
        public string FieldId
        {
            get
            {
                return this.fieldIdField;
            }
            set
            {
                this.fieldIdField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ContentTypeFilterDefinition
    {

        private string contentTypeIdField;

        /// <remarks/>
        [XmlAttribute()]
        public string ContentTypeId
        {
            get
            {
                return this.contentTypeIdField;
            }
            set
            {
                this.contentTypeIdField = value;
            }
        }
    }

    /// <remarks/>
    [XmlInclude(typeof(ItemEventDefinition))]
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class EventDefinition
    {

        private string receiverAssemblyField;

        private string receiverClassField;

        private int sequenceField;

        private bool sequenceFieldSpecified;

        private string receiverDataField;

        /// <remarks/>
        [XmlAttribute()]
        public string ReceiverAssembly
        {
            get
            {
                return this.receiverAssemblyField;
            }
            set
            {
                this.receiverAssemblyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ReceiverClass
        {
            get
            {
                return this.receiverClassField;
            }
            set
            {
                this.receiverClassField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int Sequence
        {
            get
            {
                return this.sequenceField;
            }
            set
            {
                this.sequenceField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool SequenceSpecified
        {
            get
            {
                return this.sequenceFieldSpecified;
            }
            set
            {
                this.sequenceFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ReceiverData
        {
            get
            {
                return this.receiverDataField;
            }
            set
            {
                this.receiverDataField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ItemEventDefinition : EventDefinition
    {

        private object itemField;

        private ItemEventScope scopeField;

        private bool scopeFieldSpecified;

        private ItemEventType eventTypeField;

        private bool eventTypeFieldSpecified;

        /// <remarks/>
        [XmlElement("ContentTypeFilter", typeof(ContentTypeFilterDefinition))]
        [XmlElement("FieldChangedFilter", typeof(FieldChangedFilterDefinition))]
        [XmlElement("FieldExpressionFilter", typeof(FieldRefDefinition))]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public ItemEventScope Scope
        {
            get
            {
                return this.scopeField;
            }
            set
            {
                this.scopeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ScopeSpecified
        {
            get
            {
                return this.scopeFieldSpecified;
            }
            set
            {
                this.scopeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public ItemEventType EventType
        {
            get
            {
                return this.eventTypeField;
            }
            set
            {
                this.eventTypeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool EventTypeSpecified
        {
            get
            {
                return this.eventTypeFieldSpecified;
            }
            set
            {
                this.eventTypeFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FieldRefDefinition
    {

        private string aliasField;

        private TRUEFALSE ascendingField;

        private bool ascendingFieldSpecified;

        private string createURLField;

        private string displayNameField;

        private TRUEFALSE explicitField;

        private bool explicitFieldSpecified;

        private string idField;

        private string keyField;

        private string nameField;

        private string refTypeField;

        private string showFieldField;

        private TRUEFALSE textOnlyField;

        private bool textOnlyFieldSpecified;

        private ReferenceType typeField;

        private bool typeFieldSpecified;

        private string[] textField;

        /// <remarks/>
        [XmlAttribute()]
        public string Alias
        {
            get
            {
                return this.aliasField;
            }
            set
            {
                this.aliasField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Ascending
        {
            get
            {
                return this.ascendingField;
            }
            set
            {
                this.ascendingField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AscendingSpecified
        {
            get
            {
                return this.ascendingFieldSpecified;
            }
            set
            {
                this.ascendingFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string CreateURL
        {
            get
            {
                return this.createURLField;
            }
            set
            {
                this.createURLField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Explicit
        {
            get
            {
                return this.explicitField;
            }
            set
            {
                this.explicitField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ExplicitSpecified
        {
            get
            {
                return this.explicitFieldSpecified;
            }
            set
            {
                this.explicitFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string RefType
        {
            get
            {
                return this.refTypeField;
            }
            set
            {
                this.refTypeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ShowField
        {
            get
            {
                return this.showFieldField;
            }
            set
            {
                this.showFieldField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE TextOnly
        {
            get
            {
                return this.textOnlyField;
            }
            set
            {
                this.textOnlyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TextOnlySpecified
        {
            get
            {
                return this.textOnlyFieldSpecified;
            }
            set
            {
                this.textOnlyFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public ReferenceType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TypeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum ReferenceType
    {

        /// <remarks/>
        AVG,

        /// <remarks/>
        COUNT,

        /// <remarks/>
        MAX,

        /// <remarks/>
        MIN,

        /// <remarks/>
        SUM,

        /// <remarks/>
        STDEV,

        /// <remarks/>
        VAR,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum ItemEventScope
    {

        /// <remarks/>
        Web,

        /// <remarks/>
        List,

        /// <remarks/>
        ContentType,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum ItemEventType
    {

        /// <remarks/>
        ItemAdding,

        /// <remarks/>
        ItemAdded,

        /// <remarks/>
        ItemUpdating,

        /// <remarks/>
        ItemUpdated,

        /// <remarks/>
        ItemDeleting,

        /// <remarks/>
        ItemDeleted,

        /// <remarks/>
        ItemCheckingIn,

        /// <remarks/>
        ItemCheckedIn,

        /// <remarks/>
        ItemCheckingOut,

        /// <remarks/>
        ItemUncheckingIn,

        /// <remarks/>
        ItemUncheckingOut,

        /// <remarks/>
        ItemMoving,

        /// <remarks/>
        ItemMoved,

        /// <remarks/>
        ItemFileUpdating,

        /// <remarks/>
        ItemFileUpdated,

        /// <remarks/>
        ItemFileRenaming,

        /// <remarks/>
        ItemFileRenamed,

        /// <remarks/>
        ItemFileReceiving,

        /// <remarks/>
        ItemFileReceived,

        /// <remarks/>
        ItemAttachmentAdding,

        /// <remarks/>
        ItemAttachmentAdded,

        /// <remarks/>
        ItemAttachmentDeleting,

        /// <remarks/>
        ItemAttachmentDeleted,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "4.0.30319.33440")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class FieldTypes
    {

        private FieldTypesFieldType[] fieldTypeField;

        /// <remarks/>
        [XmlElement("FieldType")]
        public FieldTypesFieldType[] FieldType
        {
            get
            {
                return this.fieldTypeField;
            }
            set
            {
                this.fieldTypeField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "4.0.30319.33440")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class FieldTypesFieldType
    {

        private FieldTypesFieldTypeField[] fieldField;

        private FieldTypesFieldTypePropertySchema propertySchemaField;

        private FieldTypesFieldTypeRenderPattern renderPatternField;

        /// <remarks/>
        [XmlElement("Field")]
        public FieldTypesFieldTypeField[] Field
        {
            get
            {
                return this.fieldField;
            }
            set
            {
                this.fieldField = value;
            }
        }

        /// <remarks/>
        public FieldTypesFieldTypePropertySchema PropertySchema
        {
            get
            {
                return this.propertySchemaField;
            }
            set
            {
                this.propertySchemaField = value;
            }
        }

        /// <remarks/>
        public FieldTypesFieldTypeRenderPattern RenderPattern
        {
            get
            {
                return this.renderPatternField;
            }
            set
            {
                this.renderPatternField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "4.0.30319.33440")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class FieldTypesFieldTypeField
    {

        private FieldTypeNameEnum nameField;

        private string valueField;

        /// <remarks/>
        [XmlAttribute()]
        public FieldTypeNameEnum Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "4.0.30319.33440")]
    [Serializable()]
    public enum FieldTypeNameEnum
    {

        /// <remarks/>
        AllowBaseTypeRendering,

        /// <remarks/>
        CAMLRendering,

        /// <remarks/>
        FieldTypeClass,

        /// <remarks/>
        FieldEditorUserControl,

        /// <remarks/>
        Filterable,

        /// <remarks/>
        InternalType,

        /// <remarks/>
        ParentType,

        /// <remarks/>
        BaseRenderingTypeName,

        /// <remarks/>
        ShowOnListAuthoringPages,

        /// <remarks/>
        ShowOnListCreate,

        /// <remarks/>
        ShowInListCreate,

        /// <remarks/>
        ShowOnDocumentLibrary,

        /// <remarks/>
        ShowOnDocumentLibraryAuthoringPages,

        /// <remarks/>
        ShowOnDocumentLibraryCreate,

        /// <remarks/>
        ShowInDocumentLibraryCreate,

        /// <remarks/>
        ShowOnSurveyAuthoringPages,

        /// <remarks/>
        ShowOnSurveyCreate,

        /// <remarks/>
        ShowInSurveyCreate,

        /// <remarks/>
        ShowOnColumnTemplateAuthoringPages,

        /// <remarks/>
        ShowOnColumnTemplateCreate,

        /// <remarks/>
        ShowInColumnTemplateCreate,

        /// <remarks/>
        SQLType,

        /// <remarks/>
        Sortable,

        /// <remarks/>
        TypeDisplayName,

        /// <remarks/>
        TypeName,

        /// <remarks/>
        TypeShortDescription,

        /// <remarks/>
        UserCreatable,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "4.0.30319.33440")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class FieldTypesFieldTypePropertySchema
    {

        private FieldTypesFieldTypePropertySchemaField[] fieldsField;

        /// <remarks/>
        [XmlArrayItem("Field", IsNullable = false)]
        public FieldTypesFieldTypePropertySchemaField[] Fields
        {
            get
            {
                return this.fieldsField;
            }
            set
            {
                this.fieldsField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "4.0.30319.33440")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class FieldTypesFieldTypePropertySchemaField
    {

        private string defaultField;

        private string nameField;

        private string displayNameField;

        private string hiddenField;

        private string typeField;

        private ushort maxLengthField;

        private byte displaySizeField;

        /// <remarks/>
        public string Default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Hidden
        {
            get
            {
                return this.hiddenField;
            }
            set
            {
                this.hiddenField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public ushort MaxLength
        {
            get
            {
                return this.maxLengthField;
            }
            set
            {
                this.maxLengthField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public byte DisplaySize
        {
            get
            {
                return this.displaySizeField;
            }
            set
            {
                this.displaySizeField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "4.0.30319.33440")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class FieldTypesFieldTypeRenderPattern
    {

        private FieldTypesFieldTypeRenderPatternProperty propertyField;

        private string nameField;

        /// <remarks/>
        public FieldTypesFieldTypeRenderPatternProperty Property
        {
            get
            {
                return this.propertyField;
            }
            set
            {
                this.propertyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "4.0.30319.33440")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class FieldTypesFieldTypeRenderPatternProperty
    {

        private string selectField;

        private string hTMLEncodeField;

        /// <remarks/>
        [XmlAttribute()]
        public string Select
        {
            get
            {
                return this.selectField;
            }
            set
            {
                this.selectField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string HTMLEncode
        {
            get
            {
                return this.hTMLEncodeField;
            }
            set
            {
                this.hTMLEncodeField = value;
            }
        }
    }

    /// <remarks/>
    [XmlInclude(typeof(FieldTypeDefinition))]
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class SubFieldTypeDefinition
    {

        private string typeNameField;

        private string sqlTypeField;

        private string internalTypeField;

        private TRUEFALSE sortableField;

        private bool sortableFieldSpecified;

        private TRUEFALSE filterableField;

        private bool filterableFieldSpecified;

        private TRUEFALSE hasValidationField;

        private bool hasValidationFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public string TypeName
        {
            get
            {
                return this.typeNameField;
            }
            set
            {
                this.typeNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string SqlType
        {
            get
            {
                return this.sqlTypeField;
            }
            set
            {
                this.sqlTypeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string InternalType
        {
            get
            {
                return this.internalTypeField;
            }
            set
            {
                this.internalTypeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Sortable
        {
            get
            {
                return this.sortableField;
            }
            set
            {
                this.sortableField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool SortableSpecified
        {
            get
            {
                return this.sortableFieldSpecified;
            }
            set
            {
                this.sortableFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Filterable
        {
            get
            {
                return this.filterableField;
            }
            set
            {
                this.filterableField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FilterableSpecified
        {
            get
            {
                return this.filterableFieldSpecified;
            }
            set
            {
                this.filterableFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE HasValidation
        {
            get
            {
                return this.hasValidationField;
            }
            set
            {
                this.hasValidationField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HasValidationSpecified
        {
            get
            {
                return this.hasValidationFieldSpecified;
            }
            set
            {
                this.hasValidationFieldSpecified = value;
            }
        }
    }


    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FieldTypeDefinition : SubFieldTypeDefinition
    {

        private RenderPatternDefinition[] renderPatternField;

        private string assemblyField;

        private string classField;

        /// <remarks/>
        [XmlElement("RenderPattern")]
        public RenderPatternDefinition[] RenderPattern
        {
            get
            {
                return this.renderPatternField;
            }
            set
            {
                this.renderPatternField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Assembly
        {
            get
            {
                return this.assemblyField;
            }
            set
            {
                this.assemblyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class RenderPatternDefinition
    {

        private string displayNameField;

        private RenderPatternType typeField;

        private bool typeFieldSpecified;

        private RenderPatternName nameField;

        private bool nameFieldSpecified;

        private TRUEFALSE tallField;

        private bool tallFieldSpecified;

        private FieldControlImplementationType implementationField;

        private bool implementationFieldSpecified;

        private string controlClassField;

        /// <remarks/>
        [XmlAttribute()]
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public RenderPatternType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TypeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public RenderPatternName Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool NameSpecified
        {
            get
            {
                return this.nameFieldSpecified;
            }
            set
            {
                this.nameFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Tall
        {
            get
            {
                return this.tallField;
            }
            set
            {
                this.tallField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TallSpecified
        {
            get
            {
                return this.tallFieldSpecified;
            }
            set
            {
                this.tallFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public FieldControlImplementationType Implementation
        {
            get
            {
                return this.implementationField;
            }
            set
            {
                this.implementationField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ImplementationSpecified
        {
            get
            {
                return this.implementationFieldSpecified;
            }
            set
            {
                this.implementationFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ControlClass
        {
            get
            {
                return this.controlClassField;
            }
            set
            {
                this.controlClassField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum RenderPatternType
    {

        /// <remarks/>
        Boolean,

        /// <remarks/>
        Choice,

        /// <remarks/>
        Counter,

        /// <remarks/>
        Currency,

        /// <remarks/>
        DateTime,

        /// <remarks/>
        Integer,

        /// <remarks/>
        Lookup,

        /// <remarks/>
        Note,

        /// <remarks/>
        Number,

        /// <remarks/>
        Text,

        /// <remarks/>
        Threading,

        /// <remarks/>
        URL,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum RenderPatternName
    {

        /// <remarks/>
        DisplayBidiPattern,

        /// <remarks/>
        DisplayPattern,

        /// <remarks/>
        EditBidiPattern,

        /// <remarks/>
        EditPattern,

        /// <remarks/>
        HeaderBidiPattern,

        /// <remarks/>
        HeaderPattern,

        /// <remarks/>
        NewBidiPattern,

        /// <remarks/>
        NewPattern,

        /// <remarks/>
        PreviewDisplayPattern,

        /// <remarks/>
        PreviewNewPattern,

        /// <remarks/>
        PreviewEditPattern,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum FieldControlImplementationType
    {

        /// <remarks/>
        Caml,

        /// <remarks/>
        Control,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class CustomActionGroupDefinition
    {

        private int sequenceField;

        private bool sequenceFieldSpecified;

        private string titleField;

        private string descriptionField;

        private string idField;

        private string locationField;

        private string[] textField;

        /// <remarks/>
        [XmlAttribute()]
        public int Sequence
        {
            get
            {
                return this.sequenceField;
            }
            set
            {
                this.sequenceField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool SequenceSpecified
        {
            get
            {
                return this.sequenceFieldSpecified;
            }
            set
            {
                this.sequenceFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class HideCustomActionDefinition
    {

        private string hideActionIdField;

        private string idField;

        private string locationField;

        private string groupIdField;

        private string[] textField;

        /// <remarks/>
        [XmlAttribute()]
        public string HideActionId
        {
            get
            {
                return this.hideActionIdField;
            }
            set
            {
                this.hideActionIdField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string GroupId
        {
            get
            {
                return this.groupIdField;
            }
            set
            {
                this.groupIdField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class UrlActionDefinition
    {

        private string urlField;

        /// <remarks/>
        [XmlAttribute()]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class CustomActionDefinition
    {

        private UrlActionDefinition urlActionField;

        private string[] textField;

        private string controlAssemblyField;

        private string controlClassField;

        private string controlSrcField;

        private string descriptionField;

        private string groupIdField;

        private string idField;

        private string imageUrlField;

        private string locationField;

        private CustomActionRegistrationType registrationTypeField;

        private bool registrationTypeFieldSpecified;

        private string registrationIdField;

        private TRUEFALSE requireSiteAdministratorField;

        private bool requireSiteAdministratorFieldSpecified;

        private string rightsField;

        private int sequenceField;

        private bool sequenceFieldSpecified;

        private TRUEFALSE showInListsField;

        private bool showInListsFieldSpecified;

        private TRUEFALSE showInReadOnlyContentTypesField;

        private bool showInReadOnlyContentTypesFieldSpecified;

        private TRUEFALSE showInSealedContentTypesField;

        private bool showInSealedContentTypesFieldSpecified;

        private string titleField;

        /// <remarks/>
        public UrlActionDefinition UrlAction
        {
            get
            {
                return this.urlActionField;
            }
            set
            {
                this.urlActionField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ControlAssembly
        {
            get
            {
                return this.controlAssemblyField;
            }
            set
            {
                this.controlAssemblyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ControlClass
        {
            get
            {
                return this.controlClassField;
            }
            set
            {
                this.controlClassField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ControlSrc
        {
            get
            {
                return this.controlSrcField;
            }
            set
            {
                this.controlSrcField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string GroupId
        {
            get
            {
                return this.groupIdField;
            }
            set
            {
                this.groupIdField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ImageUrl
        {
            get
            {
                return this.imageUrlField;
            }
            set
            {
                this.imageUrlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public CustomActionRegistrationType RegistrationType
        {
            get
            {
                return this.registrationTypeField;
            }
            set
            {
                this.registrationTypeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RegistrationTypeSpecified
        {
            get
            {
                return this.registrationTypeFieldSpecified;
            }
            set
            {
                this.registrationTypeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string RegistrationId
        {
            get
            {
                return this.registrationIdField;
            }
            set
            {
                this.registrationIdField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE RequireSiteAdministrator
        {
            get
            {
                return this.requireSiteAdministratorField;
            }
            set
            {
                this.requireSiteAdministratorField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RequireSiteAdministratorSpecified
        {
            get
            {
                return this.requireSiteAdministratorFieldSpecified;
            }
            set
            {
                this.requireSiteAdministratorFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Rights
        {
            get
            {
                return this.rightsField;
            }
            set
            {
                this.rightsField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int Sequence
        {
            get
            {
                return this.sequenceField;
            }
            set
            {
                this.sequenceField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool SequenceSpecified
        {
            get
            {
                return this.sequenceFieldSpecified;
            }
            set
            {
                this.sequenceFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ShowInLists
        {
            get
            {
                return this.showInListsField;
            }
            set
            {
                this.showInListsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowInListsSpecified
        {
            get
            {
                return this.showInListsFieldSpecified;
            }
            set
            {
                this.showInListsFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ShowInReadOnlyContentTypes
        {
            get
            {
                return this.showInReadOnlyContentTypesField;
            }
            set
            {
                this.showInReadOnlyContentTypesField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowInReadOnlyContentTypesSpecified
        {
            get
            {
                return this.showInReadOnlyContentTypesFieldSpecified;
            }
            set
            {
                this.showInReadOnlyContentTypesFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ShowInSealedContentTypes
        {
            get
            {
                return this.showInSealedContentTypesField;
            }
            set
            {
                this.showInSealedContentTypesField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowInSealedContentTypesSpecified
        {
            get
            {
                return this.showInSealedContentTypesFieldSpecified;
            }
            set
            {
                this.showInSealedContentTypesFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum CustomActionRegistrationType
    {

        /// <remarks/>
        List,

        /// <remarks/>
        ContentType,

        /// <remarks/>
        FileType,

        /// <remarks/>
        ProgId,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class SharedFieldSetDefinition
    {

        private SharedFieldDefinition[] fieldsField;

        /// <remarks/>
        [XmlArrayItem("Field", IsNullable = false)]
        public SharedFieldDefinition[] Fields
        {
            get
            {
                return this.fieldsField;
            }
            set
            {
                this.fieldsField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class SharedFieldDefinition : FieldDefinition
    {

        private string group1Field;


        /// <remarks/>
        [XmlIgnore()]
        public string Group1
        {
            get
            {
                return this.group1Field;
            }
            set
            {
                this.group1Field = value;
            }
        }

    }

    /// <remarks/>
    [XmlInclude(typeof(SharedFieldDefinition))]
    [XmlInclude(typeof(SharedFieldReference))]
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FieldDefinition
    {

        private FieldRefDefinitions fieldRefsField;

        private CamlViewRoot displayPatternField;

        private CamlViewRoot displayBidiPatternField;

        private CHOICEDEFINITIONS cHOICESField;

        private MAPPINGDEFINITION[] mAPPINGSField;

        private string defaultField;

        private string formulaField;

        private string defaultFormulaField;

        private string[] textField;

        private TRUEFALSE allowDeletionField;

        private bool allowDeletionFieldSpecified;

        private TRUEFALSE allowHyperlinkField;

        private bool allowHyperlinkFieldSpecified;

        private TRUEFALSE allowMultiVoteField;

        private bool allowMultiVoteFieldSpecified;

        private TRUEFALSE appendOnlyField;

        private bool appendOnlyFieldSpecified;

        private string authoringInfoField;

        private string baseTypeField;

        private int calTypeField;

        private bool calTypeFieldSpecified;

        private TRUEFALSE canToggleHiddenField;

        private bool canToggleHiddenFieldSpecified;

        private string classInfoField;

        private string colNameField;

        private TRUEFALSE commasField;

        private bool commasFieldSpecified;

        private int decimalsField;

        private bool decimalsFieldSpecified;

        private string descriptionField;

        private string dirField;

        private string displayImageField;

        private string displayNameField;

        private string displayNameSrcFieldField;

        private int displaySizeField;

        private bool displaySizeFieldSpecified;

        private string divField;

        private TRUEFALSE enableLookupField;

        private bool enableLookupFieldSpecified;

        private string exceptionImageField;

        private string fieldRefField;

        private TRUEFALSE fillInChoiceField;

        private bool fillInChoiceFieldSpecified;

        private TRUEFALSE filterableField;

        private bool filterableFieldSpecified;

        private TRUEFALSE filterableNoRecurrenceField;

        private bool filterableNoRecurrenceFieldSpecified;

        private string forcedDisplayField;

        private DisplayFormat formatField;

        private bool formatFieldSpecified;

        private TRUEFALSE fromBaseTypeField;

        private bool fromBaseTypeFieldSpecified;

        private string headerImageField;

        private int heightField;

        private bool heightFieldSpecified;

        private TRUEFALSE hiddenField;

        private bool hiddenFieldSpecified;

        private string hTMLEncodeField;

        private string idField;

        private IMEMode iMEModeField;

        private bool iMEModeFieldSpecified;

        private TRUEFALSE indexedField;

        private bool indexedFieldSpecified;

        private TRUEFALSE isolateStylesField;

        private bool isolateStylesFieldSpecified;

        private string joinColNameField;

        private JoinType joinTypeField;

        private bool joinTypeFieldSpecified;

        private string lCIDField;

        private string listField;

        private float maxField;

        private bool maxFieldSpecified;

        private string minField;

        private TRUEFALSE multField;

        private bool multFieldSpecified;

        private string nameField;

        private string negativeFormatField;

        private string nodeField;

        private TRUEFALSE noEditFormBreakField;

        private bool noEditFormBreakFieldSpecified;

        private int numLinesField;

        private bool numLinesFieldSpecified;

        private TRUEFALSE percentageField;

        private bool percentageFieldSpecified;

        private string pIAttributeField;

        private string pITargetField;

        private string primaryPIAttributeField;

        private string primaryPITargetField;

        private TRUEFALSE presenceField;

        private bool presenceFieldSpecified;

        private TRUEFALSE primaryKeyField;

        private bool primaryKeyFieldSpecified;

        private TRUEFALSE readOnlyField;

        private bool readOnlyFieldSpecified;

        private TRUEFALSE readOnlyEnforcedField;

        private bool readOnlyEnforcedFieldSpecified;

        private TRUEFALSE renderXMLUsingPatternField;

        private bool renderXMLUsingPatternFieldSpecified;

        private TRUEFALSE requiredField;

        private bool requiredFieldSpecified;

        private TRUEFALSE restrictedModeField;

        private bool restrictedModeFieldSpecified;

        private string resultTypeField;

        private string richTextModeField;

        private TRUEFALSE richTextField;

        private bool richTextFieldSpecified;

        private TRUEFALSE sealedField;

        private bool sealedFieldSpecified;

        private TRUEFALSE seperateLineField;

        private bool seperateLineFieldSpecified;

        private string setAsField;

        private TRUEFALSE showAddressBookButtonField;

        private bool showAddressBookButtonFieldSpecified;

        private string showFieldField;

        private TRUEFALSE showInDisplayFormField;

        private bool showInDisplayFormFieldSpecified;

        private TRUEFALSE showInEditFormField;

        private bool showInEditFormFieldSpecified;

        private TRUEFALSE showInFileDlgField;

        private bool showInFileDlgFieldSpecified;

        private TRUEFALSE showInListSettingsField;

        private bool showInListSettingsFieldSpecified;

        private TRUEFALSE showInNewFormField;

        private bool showInNewFormFieldSpecified;

        private TRUEFALSE showInViewFormsField;

        private bool showInViewFormsFieldSpecified;

        private TRUEFALSE sortableField;

        private bool sortableFieldSpecified;

        private string storageTZField;

        private string stripWSField;

        private TRUEFALSE suppressNameDisplayField;

        private bool suppressNameDisplayFieldSpecified;

        private TRUEFALSE textOnlyField;

        private bool textOnlyFieldSpecified;

        private string titleField;

        private string typeField;

        private string uniqueIdField;

        private TRUEFALSE unlimitedLengthInDocumentLibraryField;

        private bool unlimitedLengthInDocumentLibraryFieldSpecified;

        private TRUEFALSE uRLEncodeField;

        private bool uRLEncodeFieldSpecified;

        private TRUEFALSE uRLEncodeAsURLField;

        private bool uRLEncodeAsURLFieldSpecified;

        private TRUEFALSE viewableField;

        private bool viewableFieldSpecified;

        private TRUEFALSE wikiLinkingField;

        private bool wikiLinkingFieldSpecified;

        private int widthField;

        private bool widthFieldSpecified;

        private string xNameField;

        private string idFieldx;

        private string groupField;

        private int maxLengthField;

        private bool maxLengthFieldSpecified;

        private string sourceIDField;

        private string staticNameField;

        private int joinRowOrdinalField;

        private bool joinRowOrdinalFieldSpecified;

        private int rowOrdinalField;

        private bool rowOrdinalFieldSpecified;

        private TRUEFALSE showInVersionHistoryField;

        private bool showInVersionHistoryFieldSpecified;

        private TRUEFALSE prependIdField;

        private bool prependIdFieldSpecified;

        private TRUEFALSE displaceOnUpgradeField;

        private bool displaceOnUpgradeFieldSpecified;

        private int userSelectionModeField;

        private bool userSelectionModeFieldSpecified;

        private int userSelectionScopeField;

        private bool userSelectionScopeFieldSpecified;

        private XmlAttribute[] anyAttrField;

        /// <remarks/>
        /// 
        public FieldRefDefinitions FieldRefs
        {
            get
            {
                return this.fieldRefsField;
            }
            set
            {
                this.fieldRefsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public CamlViewRoot DisplayPattern
        {
            get
            {
                return this.displayPatternField;
            }
            set
            {
                this.displayPatternField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public CamlViewRoot DisplayBidiPattern
        {
            get
            {
                return this.displayBidiPatternField;
            }
            set
            {
                this.displayBidiPatternField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public CHOICEDEFINITIONS CHOICES
        {
            get
            {
                return this.cHOICESField;
            }
            set
            {
                this.cHOICESField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlArrayItemAttribute("MAPPING", IsNullable=false)]
        [XmlIgnore()]
        public MAPPINGDEFINITION[] MAPPINGS
        {
            get
            {
                return this.mAPPINGSField;
            }
            set
            {
                this.mAPPINGSField = value;
            }
        }

        /// <remarks/>
        public string Default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }

        /// <remarks/>
        public string Formula
        {
            get
            {
                return this.formulaField;
            }
            set
            {
                this.formulaField = value;
            }
        }

        /// <remarks/>
        public string DefaultFormula
        {
            get
            {
                return this.defaultFormulaField;
            }
            set
            {
                this.defaultFormulaField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE AllowDeletion
        {
            get
            {
                return this.allowDeletionField;
            }
            set
            {
                this.allowDeletionField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AllowDeletionSpecified
        {
            get
            {
                return this.allowDeletionFieldSpecified;
            }
            set
            {
                this.allowDeletionFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE AllowHyperlink
        {
            get
            {
                return this.allowHyperlinkField;
            }
            set
            {
                this.allowHyperlinkField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AllowHyperlinkSpecified
        {
            get
            {
                return this.allowHyperlinkFieldSpecified;
            }
            set
            {
                this.allowHyperlinkFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE AllowMultiVote
        {
            get
            {
                return this.allowMultiVoteField;
            }
            set
            {
                this.allowMultiVoteField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AllowMultiVoteSpecified
        {
            get
            {
                return this.allowMultiVoteFieldSpecified;
            }
            set
            {
                this.allowMultiVoteFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE AppendOnly
        {
            get
            {
                return this.appendOnlyField;
            }
            set
            {
                this.appendOnlyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AppendOnlySpecified
        {
            get
            {
                return this.appendOnlyFieldSpecified;
            }
            set
            {
                this.appendOnlyFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string AuthoringInfo
        {
            get
            {
                return this.authoringInfoField;
            }
            set
            {
                this.authoringInfoField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string BaseType
        {
            get
            {
                return this.baseTypeField;
            }
            set
            {
                this.baseTypeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int CalType
        {
            get
            {
                return this.calTypeField;
            }
            set
            {
                this.calTypeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool CalTypeSpecified
        {
            get
            {
                return this.calTypeFieldSpecified;
            }
            set
            {
                this.calTypeFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE CanToggleHidden
        {
            get
            {
                return this.canToggleHiddenField;
            }
            set
            {
                this.canToggleHiddenField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool CanToggleHiddenSpecified
        {
            get
            {
                return this.canToggleHiddenFieldSpecified;
            }
            set
            {
                this.canToggleHiddenFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ClassInfo
        {
            get
            {
                return this.classInfoField;
            }
            set
            {
                this.classInfoField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ColName
        {
            get
            {
                return this.colNameField;
            }
            set
            {
                this.colNameField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE Commas
        {
            get
            {
                return this.commasField;
            }
            set
            {
                this.commasField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool CommasSpecified
        {
            get
            {
                return this.commasFieldSpecified;
            }
            set
            {
                this.commasFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int Decimals
        {
            get
            {
                return this.decimalsField;
            }
            set
            {
                this.decimalsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DecimalsSpecified
        {
            get
            {
                return this.decimalsFieldSpecified;
            }
            set
            {
                this.decimalsFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Dir
        {
            get
            {
                return this.dirField;
            }
            set
            {
                this.dirField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string DisplayImage
        {
            get
            {
                return this.displayImageField;
            }
            set
            {
                this.displayImageField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string DisplayNameSrcField
        {
            get
            {
                return this.displayNameSrcFieldField;
            }
            set
            {
                this.displayNameSrcFieldField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int DisplaySize
        {
            get
            {
                return this.displaySizeField;
            }
            set
            {
                this.displaySizeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DisplaySizeSpecified
        {
            get
            {
                return this.displaySizeFieldSpecified;
            }
            set
            {
                this.displaySizeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Div
        {
            get
            {
                return this.divField;
            }
            set
            {
                this.divField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE EnableLookup
        {
            get
            {
                return this.enableLookupField;
            }
            set
            {
                this.enableLookupField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool EnableLookupSpecified
        {
            get
            {
                return this.enableLookupFieldSpecified;
            }
            set
            {
                this.enableLookupFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ExceptionImage
        {
            get
            {
                return this.exceptionImageField;
            }
            set
            {
                this.exceptionImageField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string FieldRef
        {
            get
            {
                return this.fieldRefField;
            }
            set
            {
                this.fieldRefField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE FillInChoice
        {
            get
            {
                return this.fillInChoiceField;
            }
            set
            {
                this.fillInChoiceField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FillInChoiceSpecified
        {
            get
            {
                return this.fillInChoiceFieldSpecified;
            }
            set
            {
                this.fillInChoiceFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE Filterable
        {
            get
            {
                return this.filterableField;
            }
            set
            {
                this.filterableField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FilterableSpecified
        {
            get
            {
                return this.filterableFieldSpecified;
            }
            set
            {
                this.filterableFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE FilterableNoRecurrence
        {
            get
            {
                return this.filterableNoRecurrenceField;
            }
            set
            {
                this.filterableNoRecurrenceField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FilterableNoRecurrenceSpecified
        {
            get
            {
                return this.filterableNoRecurrenceFieldSpecified;
            }
            set
            {
                this.filterableNoRecurrenceFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ForcedDisplay
        {
            get
            {
                return this.forcedDisplayField;
            }
            set
            {
                this.forcedDisplayField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public DisplayFormat Format
        {
            get
            {
                return this.formatField;
            }
            set
            {
                this.formatField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FormatSpecified
        {
            get
            {
                return this.formatFieldSpecified;
            }
            set
            {
                this.formatFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE FromBaseType
        {
            get
            {
                return this.fromBaseTypeField;
            }
            set
            {
                this.fromBaseTypeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FromBaseTypeSpecified
        {
            get
            {
                return this.fromBaseTypeFieldSpecified;
            }
            set
            {
                this.fromBaseTypeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string HeaderImage
        {
            get
            {
                return this.headerImageField;
            }
            set
            {
                this.headerImageField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int Height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HeightSpecified
        {
            get
            {
                return this.heightFieldSpecified;
            }
            set
            {
                this.heightFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE Hidden
        {
            get
            {
                return this.hiddenField;
            }
            set
            {
                this.hiddenField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HiddenSpecified
        {
            get
            {
                return this.hiddenFieldSpecified;
            }
            set
            {
                this.hiddenFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string HTMLEncode
        {
            get
            {
                return this.hTMLEncodeField;
            }
            set
            {
                this.hTMLEncodeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public IMEMode IMEMode
        {
            get
            {
                return this.iMEModeField;
            }
            set
            {
                this.iMEModeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool IMEModeSpecified
        {
            get
            {
                return this.iMEModeFieldSpecified;
            }
            set
            {
                this.iMEModeFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE Indexed
        {
            get
            {
                return this.indexedField;
            }
            set
            {
                this.indexedField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool IndexedSpecified
        {
            get
            {
                return this.indexedFieldSpecified;
            }
            set
            {
                this.indexedFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE IsolateStyles
        {
            get
            {
                return this.isolateStylesField;
            }
            set
            {
                this.isolateStylesField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool IsolateStylesSpecified
        {
            get
            {
                return this.isolateStylesFieldSpecified;
            }
            set
            {
                this.isolateStylesFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string JoinColName
        {
            get
            {
                return this.joinColNameField;
            }
            set
            {
                this.joinColNameField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public JoinType JoinType
        {
            get
            {
                return this.joinTypeField;
            }
            set
            {
                this.joinTypeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool JoinTypeSpecified
        {
            get
            {
                return this.joinTypeFieldSpecified;
            }
            set
            {
                this.joinTypeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string LCID
        {
            get
            {
                return this.lCIDField;
            }
            set
            {
                this.lCIDField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string List
        {
            get
            {
                return this.listField;
            }
            set
            {
                this.listField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public float Max
        {
            get
            {
                return this.maxField;
            }
            set
            {
                this.maxField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool MaxSpecified
        {
            get
            {
                return this.maxFieldSpecified;
            }
            set
            {
                this.maxFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Min
        {
            get
            {
                return this.minField;
            }
            set
            {
                this.minField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE Mult
        {
            get
            {
                return this.multField;
            }
            set
            {
                this.multField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool MultSpecified
        {
            get
            {
                return this.multFieldSpecified;
            }
            set
            {
                this.multFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string NegativeFormat
        {
            get
            {
                return this.negativeFormatField;
            }
            set
            {
                this.negativeFormatField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Node
        {
            get
            {
                return this.nodeField;
            }
            set
            {
                this.nodeField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE NoEditFormBreak
        {
            get
            {
                return this.noEditFormBreakField;
            }
            set
            {
                this.noEditFormBreakField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool NoEditFormBreakSpecified
        {
            get
            {
                return this.noEditFormBreakFieldSpecified;
            }
            set
            {
                this.noEditFormBreakFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int NumLines
        {
            get
            {
                return this.numLinesField;
            }
            set
            {
                this.numLinesField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool NumLinesSpecified
        {
            get
            {
                return this.numLinesFieldSpecified;
            }
            set
            {
                this.numLinesFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE Percentage
        {
            get
            {
                return this.percentageField;
            }
            set
            {
                this.percentageField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool PercentageSpecified
        {
            get
            {
                return this.percentageFieldSpecified;
            }
            set
            {
                this.percentageFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string PIAttribute
        {
            get
            {
                return this.pIAttributeField;
            }
            set
            {
                this.pIAttributeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string PITarget
        {
            get
            {
                return this.pITargetField;
            }
            set
            {
                this.pITargetField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string PrimaryPIAttribute
        {
            get
            {
                return this.primaryPIAttributeField;
            }
            set
            {
                this.primaryPIAttributeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string PrimaryPITarget
        {
            get
            {
                return this.primaryPITargetField;
            }
            set
            {
                this.primaryPITargetField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Presence
        {
            get
            {
                return this.presenceField;
            }
            set
            {
                this.presenceField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool PresenceSpecified
        {
            get
            {
                return this.presenceFieldSpecified;
            }
            set
            {
                this.presenceFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE PrimaryKey
        {
            get
            {
                return this.primaryKeyField;
            }
            set
            {
                this.primaryKeyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool PrimaryKeySpecified
        {
            get
            {
                return this.primaryKeyFieldSpecified;
            }
            set
            {
                this.primaryKeyFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE ReadOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ReadOnlySpecified
        {
            get
            {
                return this.readOnlyFieldSpecified;
            }
            set
            {
                this.readOnlyFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE ReadOnlyEnforced
        {
            get
            {
                return this.readOnlyEnforcedField;
            }
            set
            {
                this.readOnlyEnforcedField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ReadOnlyEnforcedSpecified
        {
            get
            {
                return this.readOnlyEnforcedFieldSpecified;
            }
            set
            {
                this.readOnlyEnforcedFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE RenderXMLUsingPattern
        {
            get
            {
                return this.renderXMLUsingPatternField;
            }
            set
            {
                this.renderXMLUsingPatternField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RenderXMLUsingPatternSpecified
        {
            get
            {
                return this.renderXMLUsingPatternFieldSpecified;
            }
            set
            {
                this.renderXMLUsingPatternFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE Required
        {
            get
            {
                return this.requiredField;
            }
            set
            {
                this.requiredField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RequiredSpecified
        {
            get
            {
                return this.requiredFieldSpecified;
            }
            set
            {
                this.requiredFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE RestrictedMode
        {
            get
            {
                return this.restrictedModeField;
            }
            set
            {
                this.restrictedModeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RestrictedModeSpecified
        {
            get
            {
                return this.restrictedModeFieldSpecified;
            }
            set
            {
                this.restrictedModeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ResultType
        {
            get
            {
                return this.resultTypeField;
            }
            set
            {
                this.resultTypeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string RichTextMode
        {
            get
            {
                return this.richTextModeField;
            }
            set
            {
                this.richTextModeField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE RichText
        {
            get
            {
                return this.richTextField;
            }
            set
            {
                this.richTextField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RichTextSpecified
        {
            get
            {
                return this.richTextFieldSpecified;
            }
            set
            {
                this.richTextFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE Sealed
        {
            get
            {
                return this.sealedField;
            }
            set
            {
                this.sealedField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool SealedSpecified
        {
            get
            {
                return this.sealedFieldSpecified;
            }
            set
            {
                this.sealedFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE SeperateLine
        {
            get
            {
                return this.seperateLineField;
            }
            set
            {
                this.seperateLineField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool SeperateLineSpecified
        {
            get
            {
                return this.seperateLineFieldSpecified;
            }
            set
            {
                this.seperateLineFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string SetAs
        {
            get
            {
                return this.setAsField;
            }
            set
            {
                this.setAsField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE ShowAddressBookButton
        {
            get
            {
                return this.showAddressBookButtonField;
            }
            set
            {
                this.showAddressBookButtonField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowAddressBookButtonSpecified
        {
            get
            {
                return this.showAddressBookButtonFieldSpecified;
            }
            set
            {
                this.showAddressBookButtonFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ShowField
        {
            get
            {
                return this.showFieldField;
            }
            set
            {
                this.showFieldField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE ShowInDisplayForm
        {
            get
            {
                return this.showInDisplayFormField;
            }
            set
            {
                this.showInDisplayFormField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowInDisplayFormSpecified
        {
            get
            {
                return this.showInDisplayFormFieldSpecified;
            }
            set
            {
                this.showInDisplayFormFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE ShowInEditForm
        {
            get
            {
                return this.showInEditFormField;
            }
            set
            {
                this.showInEditFormField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowInEditFormSpecified
        {
            get
            {
                return this.showInEditFormFieldSpecified;
            }
            set
            {
                this.showInEditFormFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE ShowInFileDlg
        {
            get
            {
                return this.showInFileDlgField;
            }
            set
            {
                this.showInFileDlgField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowInFileDlgSpecified
        {
            get
            {
                return this.showInFileDlgFieldSpecified;
            }
            set
            {
                this.showInFileDlgFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE ShowInListSettings
        {
            get
            {
                return this.showInListSettingsField;
            }
            set
            {
                this.showInListSettingsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowInListSettingsSpecified
        {
            get
            {
                return this.showInListSettingsFieldSpecified;
            }
            set
            {
                this.showInListSettingsFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE ShowInNewForm
        {
            get
            {
                return this.showInNewFormField;
            }
            set
            {
                this.showInNewFormField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowInNewFormSpecified
        {
            get
            {
                return this.showInNewFormFieldSpecified;
            }
            set
            {
                this.showInNewFormFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE ShowInViewForms
        {
            get
            {
                return this.showInViewFormsField;
            }
            set
            {
                this.showInViewFormsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowInViewFormsSpecified
        {
            get
            {
                return this.showInViewFormsFieldSpecified;
            }
            set
            {
                this.showInViewFormsFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE Sortable
        {
            get
            {
                return this.sortableField;
            }
            set
            {
                this.sortableField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool SortableSpecified
        {
            get
            {
                return this.sortableFieldSpecified;
            }
            set
            {
                this.sortableFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string StorageTZ
        {
            get
            {
                return this.storageTZField;
            }
            set
            {
                this.storageTZField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string StripWS
        {
            get
            {
                return this.stripWSField;
            }
            set
            {
                this.stripWSField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE SuppressNameDisplay
        {
            get
            {
                return this.suppressNameDisplayField;
            }
            set
            {
                this.suppressNameDisplayField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool SuppressNameDisplaySpecified
        {
            get
            {
                return this.suppressNameDisplayFieldSpecified;
            }
            set
            {
                this.suppressNameDisplayFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE TextOnly
        {
            get
            {
                return this.textOnlyField;
            }
            set
            {
                this.textOnlyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TextOnlySpecified
        {
            get
            {
                return this.textOnlyFieldSpecified;
            }
            set
            {
                this.textOnlyFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string UniqueId
        {
            get
            {
                return this.uniqueIdField;
            }
            set
            {
                this.uniqueIdField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE UnlimitedLengthInDocumentLibrary
        {
            get
            {
                return this.unlimitedLengthInDocumentLibraryField;
            }
            set
            {
                this.unlimitedLengthInDocumentLibraryField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool UnlimitedLengthInDocumentLibrarySpecified
        {
            get
            {
                return this.unlimitedLengthInDocumentLibraryFieldSpecified;
            }
            set
            {
                this.unlimitedLengthInDocumentLibraryFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE URLEncode
        {
            get
            {
                return this.uRLEncodeField;
            }
            set
            {
                this.uRLEncodeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool URLEncodeSpecified
        {
            get
            {
                return this.uRLEncodeFieldSpecified;
            }
            set
            {
                this.uRLEncodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE URLEncodeAsURL
        {
            get
            {
                return this.uRLEncodeAsURLField;
            }
            set
            {
                this.uRLEncodeAsURLField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool URLEncodeAsURLSpecified
        {
            get
            {
                return this.uRLEncodeAsURLFieldSpecified;
            }
            set
            {
                this.uRLEncodeAsURLFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE Viewable
        {
            get
            {
                return this.viewableField;
            }
            set
            {
                this.viewableField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ViewableSpecified
        {
            get
            {
                return this.viewableFieldSpecified;
            }
            set
            {
                this.viewableFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE WikiLinking
        {
            get
            {
                return this.wikiLinkingField;
            }
            set
            {
                this.wikiLinkingField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool WikiLinkingSpecified
        {
            get
            {
                return this.wikiLinkingFieldSpecified;
            }
            set
            {
                this.wikiLinkingFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int Width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool WidthSpecified
        {
            get
            {
                return this.widthFieldSpecified;
            }
            set
            {
                this.widthFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string XName
        {
            get
            {
                return this.xNameField;
            }
            set
            {
                this.xNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Group
        {
            get
            {
                return this.groupField;
            }
            set
            {
                this.groupField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int MaxLength
        {
            get
            {
                return this.maxLengthField;
            }
            set
            {
                this.maxLengthField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool MaxLengthSpecified
        {
            get
            {
                return this.maxLengthFieldSpecified;
            }
            set
            {
                this.maxLengthFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string SourceID
        {
            get
            {
                return this.sourceIDField;
            }
            set
            {
                this.sourceIDField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string StaticName
        {
            get
            {
                return this.staticNameField;
            }
            set
            {
                this.staticNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int JoinRowOrdinal
        {
            get
            {
                return this.joinRowOrdinalField;
            }
            set
            {
                this.joinRowOrdinalField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool JoinRowOrdinalSpecified
        {
            get
            {
                return this.joinRowOrdinalFieldSpecified;
            }
            set
            {
                this.joinRowOrdinalFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int RowOrdinal
        {
            get
            {
                return this.rowOrdinalField;
            }
            set
            {
                this.rowOrdinalField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RowOrdinalSpecified
        {
            get
            {
                return this.rowOrdinalFieldSpecified;
            }
            set
            {
                this.rowOrdinalFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE ShowInVersionHistory
        {
            get
            {
                return this.showInVersionHistoryField;
            }
            set
            {
                this.showInVersionHistoryField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowInVersionHistorySpecified
        {
            get
            {
                return this.showInVersionHistoryFieldSpecified;
            }
            set
            {
                this.showInVersionHistoryFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE PrependId
        {
            get
            {
                return this.prependIdField;
            }
            set
            {
                this.prependIdField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool PrependIdSpecified
        {
            get
            {
                return this.prependIdFieldSpecified;
            }
            set
            {
                this.prependIdFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        [XmlIgnore()]
        public TRUEFALSE DisplaceOnUpgrade
        {
            get
            {
                return this.displaceOnUpgradeField;
            }
            set
            {
                this.displaceOnUpgradeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DisplaceOnUpgradeSpecified
        {
            get
            {
                return this.displaceOnUpgradeFieldSpecified;
            }
            set
            {
                this.displaceOnUpgradeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int UserSelectionMode
        {
            get
            {
                return this.userSelectionModeField;
            }
            set
            {
                this.userSelectionModeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool UserSelectionModeSpecified
        {
            get
            {
                return this.userSelectionModeFieldSpecified;
            }
            set
            {
                this.userSelectionModeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int UserSelectionScope
        {
            get
            {
                return this.userSelectionScopeField;
            }
            set
            {
                this.userSelectionScopeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool UserSelectionScopeSpecified
        {
            get
            {
                return this.userSelectionScopeFieldSpecified;
            }
            set
            {
                this.userSelectionScopeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAnyAttribute()]
        public XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FieldRefDefinitions
    {

        private FieldRefDefinition[] fieldRefField;

        private string[] textField;

        /// <remarks/>
        [XmlElement("FieldRef")]
        public FieldRefDefinition[] FieldRef
        {
            get
            {
                return this.fieldRefField;
            }
            set
            {
                this.fieldRefField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [XmlInclude(typeof(ToolbarDefinition))]
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class CamlViewRoot : CamlViewElement
    {
    }

    /// <remarks/>
    [XmlInclude(typeof(ForEachElement))]
    [XmlInclude(typeof(ScriptQuoteDefinition))]
    [XmlInclude(typeof(IfNewDefinition))]
    [XmlInclude(typeof(CamlViewRoot))]
    [XmlInclude(typeof(ToolbarDefinition))]
    [XmlInclude(typeof(CaseDefinition))]
    [XmlInclude(typeof(SetVarDefinition))]
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class CamlViewElement
    {

        private object[] itemsField;

        private ItemsChoiceType4[] itemsElementNameField;

        private string[] textField;

        /// <remarks/>
        [XmlElement("Column", typeof(QueryColumnDefinition))]
        [XmlElement("Column2", typeof(QueryColumnDefinition))]
        [XmlElement("ContentTypes", typeof(CamlViewElement))]
        [XmlElement("Counter", typeof(CounterDefinition))]
        [XmlElement("CurrentRights", typeof(EmptyElement))]
        [XmlElement("Field", typeof(FieldViewReferenceDefinition))]
        [XmlElement("FieldPrefix", typeof(EmptyElement))]
        [XmlElement("FieldProperty", typeof(FieldPropertyDefinition))]
        [XmlElement("FieldSortParams", typeof(HtmlRenderingElement))]
        [XmlElement("FieldSwitch", typeof(SwitchDefinition))]
        [XmlElement("Fields", typeof(CamlViewElement))]
        [XmlElement("FilterLink", typeof(FilterLinkDefinition))]
        [XmlElement("ForEach", typeof(ForEachElement))]
        [XmlElement("GetFileExtension", typeof(object))]
        [XmlElement("GetVar", typeof(GetVarDefinition))]
        [XmlElement("HTML", typeof(string))]
        [XmlElement("HttpHost", typeof(HttpHostDefinition))]
        [XmlElement("HttpPath", typeof(HttpPathDefinition))]
        [XmlElement("HttpVDir", typeof(HttpVDirDefinition))]
        [XmlElement("ID", typeof(EmptyElement))]
        [XmlElement("Identity", typeof(EmptyElement))]
        [XmlElement("IfEqual", typeof(IfEqualDefinition))]
        [XmlElement("IfHasRights", typeof(IfHasRightsDefinition))]
        [XmlElement("IfNeg", typeof(IfNegDefinition))]
        [XmlElement("IfNew", typeof(IfNewDefinition))]
        [XmlElement("IfSubString", typeof(IfSubStringDefinition))]
        [XmlElement("Length", typeof(object))]
        [XmlElement("Limit", typeof(LimitDefinition))]
        [XmlElement("List", typeof(EmptyElement))]
        [XmlElement("ListProperty", typeof(ListPropertyDefinition))]
        [XmlElement("ListUrl", typeof(EmptyElement))]
        [XmlElement("ListUrlDir", typeof(HttpPathDefinition))]
        [XmlElement("LookupColumn", typeof(LookupColumnDefinition))]
        [XmlElement("MapToAll", typeof(CamlViewElement))]
        [XmlElement("MapToControl", typeof(CamlViewElement))]
        [XmlElement("MapToIcon", typeof(CamlViewElement))]
        [XmlElement("MeetingProperty", typeof(MeetingPropertyDefinition))]
        [XmlElement("PageUrl", typeof(HtmlRenderingElement))]
        [XmlElement("ProjectProperty", typeof(ProjectPropertyDefinition))]
        [XmlElement("Property", typeof(FieldPropertyDefinition))]
        [XmlElement("ScriptQuote", typeof(ScriptQuoteDefinition))]
        [XmlElement("SelectionOptions", typeof(SelectOptionsDefinition))]
        [XmlElement("ServerProperty", typeof(ServerPropertyDefinition))]
        [XmlElement("SetVar", typeof(SetVarDefinition))]
        [XmlElement("Switch", typeof(SwitchDefinition))]
        [XmlElement("ThreadStamp", typeof(ThreadStampDefinition))]
        [XmlElement("URL", typeof(URLDefinition))]
        [XmlElement("UrlBaseName", typeof(UrlBaseNameDefinition))]
        [XmlElement("UrlDirName", typeof(object))]
        [XmlElement("UserID", typeof(UserIDDefinition))]
        [XmlElement("WebQueryInfo", typeof(EmptyElement))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [XmlElement("ItemsElementName")]
        [XmlIgnore()]
        public ItemsChoiceType4[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class QueryColumnDefinition : LookupColumnDefinition
    {

        private string[] textField;

        private string defaultField;

        private TRUEFALSE uRLEncodeField;

        private bool uRLEncodeFieldSpecified;

        private string formatField;

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE URLEncode
        {
            get
            {
                return this.uRLEncodeField;
            }
            set
            {
                this.uRLEncodeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool URLEncodeSpecified
        {
            get
            {
                return this.uRLEncodeFieldSpecified;
            }
            set
            {
                this.uRLEncodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Format
        {
            get
            {
                return this.formatField;
            }
            set
            {
                this.formatField = value;
            }
        }
    }

    /// <remarks/>
    [XmlInclude(typeof(QueryColumnDefinition))]
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class LookupColumnDefinition
    {

        private TRUEFALSE hTMLEncodeField;

        private bool hTMLEncodeFieldSpecified;

        private TRUEFALSE stripWSField;

        private bool stripWSFieldSpecified;

        private string nameField;

        private string showFieldField;

        private TRUEFALSE uRLEncodeAsURLField;

        private bool uRLEncodeAsURLFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE HTMLEncode
        {
            get
            {
                return this.hTMLEncodeField;
            }
            set
            {
                this.hTMLEncodeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HTMLEncodeSpecified
        {
            get
            {
                return this.hTMLEncodeFieldSpecified;
            }
            set
            {
                this.hTMLEncodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE StripWS
        {
            get
            {
                return this.stripWSField;
            }
            set
            {
                this.stripWSField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool StripWSSpecified
        {
            get
            {
                return this.stripWSFieldSpecified;
            }
            set
            {
                this.stripWSFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ShowField
        {
            get
            {
                return this.showFieldField;
            }
            set
            {
                this.showFieldField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE URLEncodeAsURL
        {
            get
            {
                return this.uRLEncodeAsURLField;
            }
            set
            {
                this.uRLEncodeAsURLField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool URLEncodeAsURLSpecified
        {
            get
            {
                return this.uRLEncodeAsURLFieldSpecified;
            }
            set
            {
                this.uRLEncodeAsURLFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class CounterDefinition
    {

        private CounterType typeField;

        private bool typeFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public CounterType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TypeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum CounterType
    {

        /// <remarks/>
        View,
    }

    /// <remarks/>
    [XmlInclude(typeof(HttpVDirDefinition))]
    [XmlInclude(typeof(HttpPathDefinition))]
    [XmlInclude(typeof(UserIDDefinition))]
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class EmptyElement
    {
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class HttpVDirDefinition : EmptyElement
    {

        private TRUEFALSE currentWebField;

        private bool currentWebFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE CurrentWeb
        {
            get
            {
                return this.currentWebField;
            }
            set
            {
                this.currentWebField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool CurrentWebSpecified
        {
            get
            {
                return this.currentWebFieldSpecified;
            }
            set
            {
                this.currentWebFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class HttpPathDefinition : EmptyElement
    {

        private TRUEFALSE serverRelField;

        private bool serverRelFieldSpecified;

        private TRUEFALSE uRLEncodeAsURLField;

        private bool uRLEncodeAsURLFieldSpecified;

        private TRUEFALSE hTMLEncodeField;

        private bool hTMLEncodeFieldSpecified;

        private TRUEFALSE forInstanceField;

        private bool forInstanceFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ServerRel
        {
            get
            {
                return this.serverRelField;
            }
            set
            {
                this.serverRelField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ServerRelSpecified
        {
            get
            {
                return this.serverRelFieldSpecified;
            }
            set
            {
                this.serverRelFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE URLEncodeAsURL
        {
            get
            {
                return this.uRLEncodeAsURLField;
            }
            set
            {
                this.uRLEncodeAsURLField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool URLEncodeAsURLSpecified
        {
            get
            {
                return this.uRLEncodeAsURLFieldSpecified;
            }
            set
            {
                this.uRLEncodeAsURLFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE HTMLEncode
        {
            get
            {
                return this.hTMLEncodeField;
            }
            set
            {
                this.hTMLEncodeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HTMLEncodeSpecified
        {
            get
            {
                return this.hTMLEncodeFieldSpecified;
            }
            set
            {
                this.hTMLEncodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ForInstance
        {
            get
            {
                return this.forInstanceField;
            }
            set
            {
                this.forInstanceField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ForInstanceSpecified
        {
            get
            {
                return this.forInstanceFieldSpecified;
            }
            set
            {
                this.forInstanceFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class UserIDDefinition : EmptyElement
    {

        private TRUEFALSE allowAnonymousField;

        private bool allowAnonymousFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE AllowAnonymous
        {
            get
            {
                return this.allowAnonymousField;
            }
            set
            {
                this.allowAnonymousField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AllowAnonymousSpecified
        {
            get
            {
                return this.allowAnonymousFieldSpecified;
            }
            set
            {
                this.allowAnonymousFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FieldViewReferenceDefinition
    {

        private TRUEFALSE autoHyperLinkField;

        private bool autoHyperLinkFieldSpecified;

        private TRUEFALSE autoHyperLinkNoEncodingField;

        private bool autoHyperLinkNoEncodingFieldSpecified;

        private TRUEFALSE autoNewLineField;

        private bool autoNewLineFieldSpecified;

        private string nameField;

        private TRUEFALSE stripWSField;

        private bool stripWSFieldSpecified;

        private TRUEFALSE uRLEncodeField;

        private bool uRLEncodeFieldSpecified;

        private TRUEFALSE uRLEncodeAsURLField;

        private bool uRLEncodeAsURLFieldSpecified;

        private TRUEFALSE hTMLEncodeField;

        private bool hTMLEncodeFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE AutoHyperLink
        {
            get
            {
                return this.autoHyperLinkField;
            }
            set
            {
                this.autoHyperLinkField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AutoHyperLinkSpecified
        {
            get
            {
                return this.autoHyperLinkFieldSpecified;
            }
            set
            {
                this.autoHyperLinkFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE AutoHyperLinkNoEncoding
        {
            get
            {
                return this.autoHyperLinkNoEncodingField;
            }
            set
            {
                this.autoHyperLinkNoEncodingField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AutoHyperLinkNoEncodingSpecified
        {
            get
            {
                return this.autoHyperLinkNoEncodingFieldSpecified;
            }
            set
            {
                this.autoHyperLinkNoEncodingFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE AutoNewLine
        {
            get
            {
                return this.autoNewLineField;
            }
            set
            {
                this.autoNewLineField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AutoNewLineSpecified
        {
            get
            {
                return this.autoNewLineFieldSpecified;
            }
            set
            {
                this.autoNewLineFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE StripWS
        {
            get
            {
                return this.stripWSField;
            }
            set
            {
                this.stripWSField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool StripWSSpecified
        {
            get
            {
                return this.stripWSFieldSpecified;
            }
            set
            {
                this.stripWSFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE URLEncode
        {
            get
            {
                return this.uRLEncodeField;
            }
            set
            {
                this.uRLEncodeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool URLEncodeSpecified
        {
            get
            {
                return this.uRLEncodeFieldSpecified;
            }
            set
            {
                this.uRLEncodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE URLEncodeAsURL
        {
            get
            {
                return this.uRLEncodeAsURLField;
            }
            set
            {
                this.uRLEncodeAsURLField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool URLEncodeAsURLSpecified
        {
            get
            {
                return this.uRLEncodeAsURLFieldSpecified;
            }
            set
            {
                this.uRLEncodeAsURLFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE HTMLEncode
        {
            get
            {
                return this.hTMLEncodeField;
            }
            set
            {
                this.hTMLEncodeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HTMLEncodeSpecified
        {
            get
            {
                return this.hTMLEncodeFieldSpecified;
            }
            set
            {
                this.hTMLEncodeFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FieldPropertyDefinition : HtmlRenderingElement
    {

        private string selectField;

        private string nameField;

        private TRUEFALSE hTMLEncode1Field;

        private bool hTMLEncode1FieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public string Select
        {
            get
            {
                return this.selectField;
            }
            set
            {
                this.selectField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("HTMLEncode")]
        public TRUEFALSE HTMLEncode1
        {
            get
            {
                return this.hTMLEncode1Field;
            }
            set
            {
                this.hTMLEncode1Field = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HTMLEncode1Specified
        {
            get
            {
                return this.hTMLEncode1FieldSpecified;
            }
            set
            {
                this.hTMLEncode1FieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [XmlInclude(typeof(ProjectPropertyDefinition))]
    [XmlInclude(typeof(MeetingPropertyDefinition))]
    [XmlInclude(typeof(FilterLinkDefinition))]
    [XmlInclude(typeof(LimitDefinition))]
    [XmlInclude(typeof(FieldPropertyDefinition))]
    [XmlInclude(typeof(ListPropertyDefinition))]
    [XmlInclude(typeof(GetVarDefinition))]
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class HtmlRenderingElement
    {

        private TRUEFALSE autoHyperLinkField;

        private bool autoHyperLinkFieldSpecified;

        private TRUEFALSE uRLEncodeField;

        private bool uRLEncodeFieldSpecified;

        private TRUEFALSE hTMLEncodeField;

        private bool hTMLEncodeFieldSpecified;

        private TRUEFALSE autoNewLineField;

        private bool autoNewLineFieldSpecified;

        private string uRLEncodeAsURLField;

        private string defaultField;

        private TRUEFALSE stripWSField;

        private bool stripWSFieldSpecified;

        private TRUEFALSE expandXMLField;

        private bool expandXMLFieldSpecified;

        private TRUEFALSE autoHyperLinkNoEncodingField;

        private bool autoHyperLinkNoEncodingFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE AutoHyperLink
        {
            get
            {
                return this.autoHyperLinkField;
            }
            set
            {
                this.autoHyperLinkField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AutoHyperLinkSpecified
        {
            get
            {
                return this.autoHyperLinkFieldSpecified;
            }
            set
            {
                this.autoHyperLinkFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE URLEncode
        {
            get
            {
                return this.uRLEncodeField;
            }
            set
            {
                this.uRLEncodeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool URLEncodeSpecified
        {
            get
            {
                return this.uRLEncodeFieldSpecified;
            }
            set
            {
                this.uRLEncodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE HTMLEncode
        {
            get
            {
                return this.hTMLEncodeField;
            }
            set
            {
                this.hTMLEncodeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HTMLEncodeSpecified
        {
            get
            {
                return this.hTMLEncodeFieldSpecified;
            }
            set
            {
                this.hTMLEncodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE AutoNewLine
        {
            get
            {
                return this.autoNewLineField;
            }
            set
            {
                this.autoNewLineField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AutoNewLineSpecified
        {
            get
            {
                return this.autoNewLineFieldSpecified;
            }
            set
            {
                this.autoNewLineFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string URLEncodeAsURL
        {
            get
            {
                return this.uRLEncodeAsURLField;
            }
            set
            {
                this.uRLEncodeAsURLField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE StripWS
        {
            get
            {
                return this.stripWSField;
            }
            set
            {
                this.stripWSField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool StripWSSpecified
        {
            get
            {
                return this.stripWSFieldSpecified;
            }
            set
            {
                this.stripWSFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ExpandXML
        {
            get
            {
                return this.expandXMLField;
            }
            set
            {
                this.expandXMLField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ExpandXMLSpecified
        {
            get
            {
                return this.expandXMLFieldSpecified;
            }
            set
            {
                this.expandXMLFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE AutoHyperLinkNoEncoding
        {
            get
            {
                return this.autoHyperLinkNoEncodingField;
            }
            set
            {
                this.autoHyperLinkNoEncodingField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AutoHyperLinkNoEncodingSpecified
        {
            get
            {
                return this.autoHyperLinkNoEncodingFieldSpecified;
            }
            set
            {
                this.autoHyperLinkNoEncodingFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ProjectPropertyDefinition : HtmlRenderingElement
    {

        private string selectField;

        /// <remarks/>
        [XmlAttribute()]
        public string Select
        {
            get
            {
                return this.selectField;
            }
            set
            {
                this.selectField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class MeetingPropertyDefinition : HtmlRenderingElement
    {

        private string selectField;

        /// <remarks/>
        [XmlAttribute()]
        public string Select
        {
            get
            {
                return this.selectField;
            }
            set
            {
                this.selectField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FilterLinkDefinition : HtmlRenderingElement
    {

        private string default1Field;

        private TRUEFALSE pagedField;

        private bool pagedFieldSpecified;

        /// <remarks/>
        [XmlAttribute("Default")]
        public string Default1
        {
            get
            {
                return this.default1Field;
            }
            set
            {
                this.default1Field = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Paged
        {
            get
            {
                return this.pagedField;
            }
            set
            {
                this.pagedField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool PagedSpecified
        {
            get
            {
                return this.pagedFieldSpecified;
            }
            set
            {
                this.pagedFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class LimitDefinition : HtmlRenderingElement
    {

        private object textField;

        private object moreField;

        private ColumnDefinition columnField;

        private TRUEFALSE ignoreTagsField;

        private bool ignoreTagsFieldSpecified;

        private int lenField;

        private bool lenFieldSpecified;

        private string moreTextField;

        /// <remarks/>
        public object Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        public object More
        {
            get
            {
                return this.moreField;
            }
            set
            {
                this.moreField = value;
            }
        }

        /// <remarks/>
        public ColumnDefinition Column
        {
            get
            {
                return this.columnField;
            }
            set
            {
                this.columnField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE IgnoreTags
        {
            get
            {
                return this.ignoreTagsField;
            }
            set
            {
                this.ignoreTagsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool IgnoreTagsSpecified
        {
            get
            {
                return this.ignoreTagsFieldSpecified;
            }
            set
            {
                this.ignoreTagsFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int Len
        {
            get
            {
                return this.lenField;
            }
            set
            {
                this.lenField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool LenSpecified
        {
            get
            {
                return this.lenFieldSpecified;
            }
            set
            {
                this.lenFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string MoreText
        {
            get
            {
                return this.moreTextField;
            }
            set
            {
                this.moreTextField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ColumnDefinition
    {

        private string nameField;

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ListPropertyDefinition : HtmlRenderingElement
    {

        private string selectField;

        private TRUEFALSE forInstanceField;

        private bool forInstanceFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public string Select
        {
            get
            {
                return this.selectField;
            }
            set
            {
                this.selectField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ForInstance
        {
            get
            {
                return this.forInstanceField;
            }
            set
            {
                this.forInstanceField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ForInstanceSpecified
        {
            get
            {
                return this.forInstanceFieldSpecified;
            }
            set
            {
                this.forInstanceFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class GetVarDefinition : HtmlRenderingElement
    {

        private string nameField;

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class SwitchDefinition
    {

        private object[] itemsField;

        private TrueFalseMixed stripWSField;

        private bool stripWSFieldSpecified;

        /// <remarks/>
        [XmlElement("Case", typeof(CaseDefinition))]
        [XmlElement("Default", typeof(CamlViewElement))]
        [XmlElement("Expr", typeof(ExprDefinition))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TrueFalseMixed StripWS
        {
            get
            {
                return this.stripWSField;
            }
            set
            {
                this.stripWSField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool StripWSSpecified
        {
            get
            {
                return this.stripWSFieldSpecified;
            }
            set
            {
                this.stripWSFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class CaseDefinition : CamlViewElement
    {

        private string valueField;

        /// <remarks/>
        [XmlAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ExprDefinition
    {

        private string[] textField;

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum TrueFalseMixed
    {

        /// <remarks/>
        True,

        /// <remarks/>
        False,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ForEachElement : CamlViewElement
    {

        private string selectField;

        /// <remarks/>
        [XmlAttribute()]
        public string Select
        {
            get
            {
                return this.selectField;
            }
            set
            {
                this.selectField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class HttpHostDefinition
    {

        private TRUEFALSE uRLEncodeAsURLField;

        private bool uRLEncodeAsURLFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE URLEncodeAsURL
        {
            get
            {
                return this.uRLEncodeAsURLField;
            }
            set
            {
                this.uRLEncodeAsURLField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool URLEncodeAsURLSpecified
        {
            get
            {
                return this.uRLEncodeAsURLFieldSpecified;
            }
            set
            {
                this.uRLEncodeAsURLFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class IfEqualDefinition
    {

        private CamlQueryRoot expr1Field;

        private CamlQueryRoot expr2Field;

        private CamlViewRoot thenField;

        private CamlViewRoot elseField;

        /// <remarks/>
        public CamlQueryRoot Expr1
        {
            get
            {
                return this.expr1Field;
            }
            set
            {
                this.expr1Field = value;
            }
        }

        /// <remarks/>
        public CamlQueryRoot Expr2
        {
            get
            {
                return this.expr2Field;
            }
            set
            {
                this.expr2Field = value;
            }
        }

        /// <remarks/>
        public CamlViewRoot Then
        {
            get
            {
                return this.thenField;
            }
            set
            {
                this.thenField = value;
            }
        }

        /// <remarks/>
        public CamlViewRoot Else
        {
            get
            {
                return this.elseField;
            }
            set
            {
                this.elseField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class CamlQueryRoot
    {

        private LogicalJoinDefinition whereField;

        private FieldRefDefinitions orderByField;

        private LogicalTestDefinition eqField;

        private SimpleFieldRef fieldField;

        private ListPropertyDefinition listPropertyField;

        private LookupColumnDefinition lookupColumnField;

        private QueryColumnDefinition columnField;

        private QueryGetVarDefinition getVarField;

        private GroupByDefinition groupByField;

        private LogicalTestDefinition containsField;

        private LogicalTestDefinition beginsWithField;

        private string[] textField;

        /// <remarks/>
        public LogicalJoinDefinition Where
        {
            get
            {
                return this.whereField;
            }
            set
            {
                this.whereField = value;
            }
        }

        /// <remarks/>
        public FieldRefDefinitions OrderBy
        {
            get
            {
                return this.orderByField;
            }
            set
            {
                this.orderByField = value;
            }
        }

        /// <remarks/>
        public LogicalTestDefinition Eq
        {
            get
            {
                return this.eqField;
            }
            set
            {
                this.eqField = value;
            }
        }

        /// <remarks/>
        public SimpleFieldRef Field
        {
            get
            {
                return this.fieldField;
            }
            set
            {
                this.fieldField = value;
            }
        }

        /// <remarks/>
        public ListPropertyDefinition ListProperty
        {
            get
            {
                return this.listPropertyField;
            }
            set
            {
                this.listPropertyField = value;
            }
        }

        /// <remarks/>
        public LookupColumnDefinition LookupColumn
        {
            get
            {
                return this.lookupColumnField;
            }
            set
            {
                this.lookupColumnField = value;
            }
        }

        /// <remarks/>
        public QueryColumnDefinition Column
        {
            get
            {
                return this.columnField;
            }
            set
            {
                this.columnField = value;
            }
        }

        /// <remarks/>
        public QueryGetVarDefinition GetVar
        {
            get
            {
                return this.getVarField;
            }
            set
            {
                this.getVarField = value;
            }
        }

        /// <remarks/>
        public GroupByDefinition GroupBy
        {
            get
            {
                return this.groupByField;
            }
            set
            {
                this.groupByField = value;
            }
        }

        /// <remarks/>
        public LogicalTestDefinition Contains
        {
            get
            {
                return this.containsField;
            }
            set
            {
                this.containsField = value;
            }
        }

        /// <remarks/>
        public LogicalTestDefinition BeginsWith
        {
            get
            {
                return this.beginsWithField;
            }
            set
            {
                this.beginsWithField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class LogicalJoinDefinition
    {

        private object[] itemsField;

        private ItemsChoiceType3[] itemsElementNameField;

        /// <remarks/>
        [XmlElement("And", typeof(ExtendedLogicalJoinDefinition))]
        [XmlElement("BeginsWith", typeof(LogicalTestDefinition))]
        [XmlElement("Contains", typeof(LogicalTestDefinition))]
        [XmlElement("DateRangesOverlap", typeof(UnlimitedLogicalTestDefinition))]
        [XmlElement("Eq", typeof(LogicalTestDefinition))]
        [XmlElement("Geq", typeof(LogicalTestDefinition))]
        [XmlElement("Gt", typeof(LogicalTestDefinition))]
        [XmlElement("IsNotNull", typeof(LogicalTestDefinition))]
        [XmlElement("IsNull", typeof(LogicalTestDefinition))]
        [XmlElement("Leq", typeof(LogicalTestDefinition))]
        [XmlElement("Lt", typeof(LogicalTestDefinition))]
        [XmlElement("Membership", typeof(MembershipDefinition))]
        [XmlElement("Neq", typeof(LogicalTestDefinition))]
        [XmlElement("Or", typeof(ExtendedLogicalJoinDefinition))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [XmlElement("ItemsElementName")]
        [XmlIgnore()]
        public ItemsChoiceType3[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ExtendedLogicalJoinDefinition
    {

        private object[] itemsField;

        private ItemsChoiceType2[] itemsElementNameField;

        /// <remarks/>
        [XmlElement("And", typeof(ExtendedLogicalJoinDefinition))]
        [XmlElement("BeginsWith", typeof(LogicalTestDefinition))]
        [XmlElement("Contains", typeof(LogicalTestDefinition))]
        [XmlElement("DateRangesOverlap", typeof(LogicalTestDefinition))]
        [XmlElement("Eq", typeof(LogicalTestDefinition))]
        [XmlElement("Geq", typeof(LogicalTestDefinition))]
        [XmlElement("Gt", typeof(LogicalTestDefinition))]
        [XmlElement("IsNotNull", typeof(LogicalNullDefinition))]
        [XmlElement("IsNull", typeof(LogicalNullDefinition))]
        [XmlElement("Leq", typeof(LogicalTestDefinition))]
        [XmlElement("Lt", typeof(LogicalTestDefinition))]
        [XmlElement("Membership", typeof(MembershipDefinition))]
        [XmlElement("Neq", typeof(LogicalTestDefinition))]
        [XmlElement("Or", typeof(ExtendedLogicalJoinDefinition))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [XmlElement("ItemsElementName")]
        [XmlIgnore()]
        public ItemsChoiceType2[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }
    }

    /// <remarks/>
    [XmlInclude(typeof(GroupByDefinition))]
    [XmlInclude(typeof(MembershipDefinition))]
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class LogicalTestDefinition
    {

        private object[] itemsField;

        /// <remarks/>
        [XmlElement("FieldRef", typeof(FieldRefDefinition))]
        [XmlElement("Value", typeof(ValueDefinition))]
        [XmlElement("XML", typeof(string))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ValueDefinition
    {

        private string[] textField;

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class GroupByDefinition : LogicalTestDefinition
    {

        private TRUEFALSE collapseField;

        private bool collapseFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Collapse
        {
            get
            {
                return this.collapseField;
            }
            set
            {
                this.collapseField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool CollapseSpecified
        {
            get
            {
                return this.collapseFieldSpecified;
            }
            set
            {
                this.collapseFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class MembershipDefinition : LogicalTestDefinition
    {

        private string typeField;

        /// <remarks/>
        [XmlAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class LogicalNullDefinition
    {

        private FieldRefDefinition fieldRefField;

        /// <remarks/>
        public FieldRefDefinition FieldRef
        {
            get
            {
                return this.fieldRefField;
            }
            set
            {
                this.fieldRefField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/", IncludeInSchema = false)]
    public enum ItemsChoiceType2
    {

        /// <remarks/>
        And,

        /// <remarks/>
        BeginsWith,

        /// <remarks/>
        Contains,

        /// <remarks/>
        DateRangesOverlap,

        /// <remarks/>
        Eq,

        /// <remarks/>
        Geq,

        /// <remarks/>
        Gt,

        /// <remarks/>
        IsNotNull,

        /// <remarks/>
        IsNull,

        /// <remarks/>
        Leq,

        /// <remarks/>
        Lt,

        /// <remarks/>
        Membership,

        /// <remarks/>
        Neq,

        /// <remarks/>
        Or,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class UnlimitedLogicalTestDefinition
    {

        private object[] itemsField;

        /// <remarks/>
        [XmlElement("FieldRef", typeof(FieldRefDefinition))]
        [XmlElement("Value", typeof(ValueDefinition))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/", IncludeInSchema = false)]
    public enum ItemsChoiceType3
    {

        /// <remarks/>
        And,

        /// <remarks/>
        BeginsWith,

        /// <remarks/>
        Contains,

        /// <remarks/>
        DateRangesOverlap,

        /// <remarks/>
        Eq,

        /// <remarks/>
        Geq,

        /// <remarks/>
        Gt,

        /// <remarks/>
        IsNotNull,

        /// <remarks/>
        IsNull,

        /// <remarks/>
        Leq,

        /// <remarks/>
        Lt,

        /// <remarks/>
        Membership,

        /// <remarks/>
        Neq,

        /// <remarks/>
        Or,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class SimpleFieldRef
    {

        private string nameField;

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class QueryGetVarDefinition
    {

        private TRUEFALSE hTMLEncodeField;

        private bool hTMLEncodeFieldSpecified;

        private string nameField;

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE HTMLEncode
        {
            get
            {
                return this.hTMLEncodeField;
            }
            set
            {
                this.hTMLEncodeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HTMLEncodeSpecified
        {
            get
            {
                return this.hTMLEncodeFieldSpecified;
            }
            set
            {
                this.hTMLEncodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class IfHasRightsDefinition
    {

        private RightsGroupDefinition[] rightsChoicesField;

        private CamlViewElement thenField;

        private CamlViewElement elseField;

        /// <remarks/>
        [XmlArrayItem("RightsGroup", IsNullable = false)]
        public RightsGroupDefinition[] RightsChoices
        {
            get
            {
                return this.rightsChoicesField;
            }
            set
            {
                this.rightsChoicesField = value;
            }
        }

        /// <remarks/>
        public CamlViewElement Then
        {
            get
            {
                return this.thenField;
            }
            set
            {
                this.thenField = value;
            }
        }

        /// <remarks/>
        public CamlViewElement Else
        {
            get
            {
                return this.elseField;
            }
            set
            {
                this.elseField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class RightsGroupDefinition
    {

        private PermissionState permAddListItemsField;

        private bool permAddListItemsFieldSpecified;

        private PermissionState permEditListItemsField;

        private bool permEditListItemsFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public PermissionState PermAddListItems
        {
            get
            {
                return this.permAddListItemsField;
            }
            set
            {
                this.permAddListItemsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool PermAddListItemsSpecified
        {
            get
            {
                return this.permAddListItemsFieldSpecified;
            }
            set
            {
                this.permAddListItemsFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public PermissionState PermEditListItems
        {
            get
            {
                return this.permEditListItemsField;
            }
            set
            {
                this.permEditListItemsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool PermEditListItemsSpecified
        {
            get
            {
                return this.permEditListItemsFieldSpecified;
            }
            set
            {
                this.permEditListItemsFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum PermissionState
    {

        /// <remarks/>
        required,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class IfNegDefinition
    {

        private CamlQueryRoot expr1Field;

        private CamlQueryRoot expr2Field;

        /// <remarks/>
        public CamlQueryRoot Expr1
        {
            get
            {
                return this.expr1Field;
            }
            set
            {
                this.expr1Field = value;
            }
        }

        /// <remarks/>
        public CamlQueryRoot Expr2
        {
            get
            {
                return this.expr2Field;
            }
            set
            {
                this.expr2Field = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class IfNewDefinition : CamlViewElement
    {

        private string nameField;

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class IfSubStringDefinition
    {

        private CamlQueryRoot expr1Field;

        private CamlQueryRoot expr2Field;

        private CamlViewRoot thenField;

        private CamlViewRoot elseField;

        /// <remarks/>
        public CamlQueryRoot Expr1
        {
            get
            {
                return this.expr1Field;
            }
            set
            {
                this.expr1Field = value;
            }
        }

        /// <remarks/>
        public CamlQueryRoot Expr2
        {
            get
            {
                return this.expr2Field;
            }
            set
            {
                this.expr2Field = value;
            }
        }

        /// <remarks/>
        public CamlViewRoot Then
        {
            get
            {
                return this.thenField;
            }
            set
            {
                this.thenField = value;
            }
        }

        /// <remarks/>
        public CamlViewRoot Else
        {
            get
            {
                return this.elseField;
            }
            set
            {
                this.elseField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ScriptQuoteDefinition : CamlViewElement
    {

        private TRUEFALSE notAddingQuoteField;

        private bool notAddingQuoteFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE NotAddingQuote
        {
            get
            {
                return this.notAddingQuoteField;
            }
            set
            {
                this.notAddingQuoteField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool NotAddingQuoteSpecified
        {
            get
            {
                return this.notAddingQuoteFieldSpecified;
            }
            set
            {
                this.notAddingQuoteFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class SelectOptionsDefinition
    {

        private int lenField;

        private bool lenFieldSpecified;

        private string blankPatternField;

        private string moreTextField;

        /// <remarks/>
        [XmlAttribute()]
        public int Len
        {
            get
            {
                return this.lenField;
            }
            set
            {
                this.lenField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool LenSpecified
        {
            get
            {
                return this.lenFieldSpecified;
            }
            set
            {
                this.lenFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string BlankPattern
        {
            get
            {
                return this.blankPatternField;
            }
            set
            {
                this.blankPatternField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string MoreText
        {
            get
            {
                return this.moreTextField;
            }
            set
            {
                this.moreTextField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ServerPropertyDefinition
    {

        private QueryColumnDefinition[] columnField;

        private string selectField;

        /// <remarks/>
        [XmlElement("Column")]
        public QueryColumnDefinition[] Column
        {
            get
            {
                return this.columnField;
            }
            set
            {
                this.columnField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Select
        {
            get
            {
                return this.selectField;
            }
            set
            {
                this.selectField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class SetVarDefinition : CamlViewElement
    {

        private string idField;

        private RequestParameter scopeField;

        private bool scopeFieldSpecified;

        private string valueField;

        private string nameField;

        /// <remarks/>
        [XmlAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public RequestParameter Scope
        {
            get
            {
                return this.scopeField;
            }
            set
            {
                this.scopeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ScopeSpecified
        {
            get
            {
                return this.scopeFieldSpecified;
            }
            set
            {
                this.scopeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum RequestParameter
    {

        /// <remarks/>
        Request,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ThreadStampDefinition
    {

        private TRUEFALSE stripWSField;

        private bool stripWSFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE StripWS
        {
            get
            {
                return this.stripWSField;
            }
            set
            {
                this.stripWSField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool StripWSSpecified
        {
            get
            {
                return this.stripWSFieldSpecified;
            }
            set
            {
                this.stripWSFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class UrlBaseNameDefinition
    {

        private object[] itemsField;

        private TRUEFALSE hTMLEncodeField;

        private bool hTMLEncodeFieldSpecified;

        /// <remarks/>
        [XmlElement("Field", typeof(object))]
        [XmlElement("LookupColumn", typeof(LookupColumnDefinition))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE HTMLEncode
        {
            get
            {
                return this.hTMLEncodeField;
            }
            set
            {
                this.hTMLEncodeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HTMLEncodeSpecified
        {
            get
            {
                return this.hTMLEncodeFieldSpecified;
            }
            set
            {
                this.hTMLEncodeFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/", IncludeInSchema = false)]
    public enum ItemsChoiceType4
    {

        /// <remarks/>
        Column,

        /// <remarks/>
        Column2,

        /// <remarks/>
        ContentTypes,

        /// <remarks/>
        Counter,

        /// <remarks/>
        CurrentRights,

        /// <remarks/>
        Field,

        /// <remarks/>
        FieldPrefix,

        /// <remarks/>
        FieldProperty,

        /// <remarks/>
        FieldSortParams,

        /// <remarks/>
        FieldSwitch,

        /// <remarks/>
        Fields,

        /// <remarks/>
        FilterLink,

        /// <remarks/>
        ForEach,

        /// <remarks/>
        GetFileExtension,

        /// <remarks/>
        GetVar,

        /// <remarks/>
        HTML,

        /// <remarks/>
        HttpHost,

        /// <remarks/>
        HttpPath,

        /// <remarks/>
        HttpVDir,

        /// <remarks/>
        ID,

        /// <remarks/>
        Identity,

        /// <remarks/>
        IfEqual,

        /// <remarks/>
        IfHasRights,

        /// <remarks/>
        IfNeg,

        /// <remarks/>
        IfNew,

        /// <remarks/>
        IfSubString,

        /// <remarks/>
        Length,

        /// <remarks/>
        Limit,

        /// <remarks/>
        List,

        /// <remarks/>
        ListProperty,

        /// <remarks/>
        ListUrl,

        /// <remarks/>
        ListUrlDir,

        /// <remarks/>
        LookupColumn,

        /// <remarks/>
        MapToAll,

        /// <remarks/>
        MapToControl,

        /// <remarks/>
        MapToIcon,

        /// <remarks/>
        MeetingProperty,

        /// <remarks/>
        PageUrl,

        /// <remarks/>
        ProjectProperty,

        /// <remarks/>
        Property,

        /// <remarks/>
        ScriptQuote,

        /// <remarks/>
        SelectionOptions,

        /// <remarks/>
        ServerProperty,

        /// <remarks/>
        SetVar,

        /// <remarks/>
        Switch,

        /// <remarks/>
        ThreadStamp,

        /// <remarks/>
        URL,

        /// <remarks/>
        UrlBaseName,

        /// <remarks/>
        UrlDirName,

        /// <remarks/>
        UserID,

        /// <remarks/>
        WebQueryInfo,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ToolbarDefinition : CamlViewRoot
    {

        private ToolbarPosition positionField;

        private bool positionFieldSpecified;

        private ToolbarType typeField;

        private bool typeFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public ToolbarPosition Position
        {
            get
            {
                return this.positionField;
            }
            set
            {
                this.positionField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool PositionSpecified
        {
            get
            {
                return this.positionFieldSpecified;
            }
            set
            {
                this.positionFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public ToolbarType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TypeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum ToolbarPosition
    {

        /// <remarks/>
        After,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum ToolbarType
    {

        /// <remarks/>
        Standard,

        /// <remarks/>
        FreeForm,

        /// <remarks/>
        RelatedTasks,

        /// <remarks/>
        Freeform,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class CHOICEDEFINITIONS
    {

        private string[] cHOICEField;

        private string[] textField;

        /// <remarks/>
        [XmlElement("CHOICE")]
        public string[] CHOICE
        {
            get
            {
                return this.cHOICEField;
            }
            set
            {
                this.cHOICEField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class MAPPINGDEFINITION
    {

        private string valueField;

        private string value1Field;

        /// <remarks/>
        [XmlAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string Value1
        {
            get
            {
                return this.value1Field;
            }
            set
            {
                this.value1Field = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum DisplayFormat
    {

        /// <remarks/>
        DateOnly,

        /// <remarks/>
        DateTime,

        /// <remarks/>
        EventList,

        /// <remarks/>
        ISO8601,

        /// <remarks/>
        ISO8601Basic,

        /// <remarks/>
        ISO8601Gregorian,

        /// <remarks/>
        Dropdown,

        /// <remarks/>
        RadioButtons,

        /// <remarks/>
        Hyperlink,

        /// <remarks/>
        Image,

        /// <remarks/>
        TRUE,

        /// <remarks/>
        FALSE,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum IMEMode
    {

        /// <remarks/>
        inactive,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum JoinType
    {

        /// <remarks/>
        INNER,

        /// <remarks/>
        [XmlEnum("LEFT OUTER")]
        LEFTOUTER,

        /// <remarks/>
        [XmlEnum("RIGHT OUTER")]
        RIGHTOUTER,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class SharedFieldReference : FieldDefinition
    {
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class PolicyAssemblyDefinition
    {

        private string nameField;

        private string versionField;

        private string publicKeyBlobField;

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string PublicKeyBlob
        {
            get
            {
                return this.publicKeyBlobField;
            }
            set
            {
                this.publicKeyBlobField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class PermissionSetDefinition
    {

        private object[] iPermissionField;

        private PermssionSetClassAttr classField;

        private string versionField;

        private string descriptionField;

        private string nameField;

        /// <remarks/>
        [XmlElement("IPermission")]
        public object[] IPermission
        {
            get
            {
                return this.iPermissionField;
            }
            set
            {
                this.iPermissionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public PermssionSetClassAttr @class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum PermssionSetClassAttr
    {

        /// <remarks/>
        NamedPermissionSet,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class PolicyItemDefinition
    {

        private PermissionSetDefinition permissionSetField;

        private PolicyAssemblyDefinition[] assembliesField;

        /// <remarks/>
        public PermissionSetDefinition PermissionSet
        {
            get
            {
                return this.permissionSetField;
            }
            set
            {
                this.permissionSetField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("Assembly", IsNullable = false)]
        public PolicyAssemblyDefinition[] Assemblies
        {
            get
            {
                return this.assembliesField;
            }
            set
            {
                this.assembliesField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ResourceDefinition
    {

        private string locationField;

        /// <remarks/>
        [XmlAttribute()]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ApplicationResourceFileDefinition
    {

        private string locationField;

        /// <remarks/>
        [XmlAttribute()]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class RootFileReference
    {

        private string locationField;

        /// <remarks/>
        [XmlAttribute()]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class TemplateFileReference
    {

        private string locationField;

        /// <remarks/>
        [XmlAttribute()]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class SafeControlDefinition
    {

        private string assemblyField;

        private string namespaceField;

        private string typeNameField;

        private TrueFalseMixed safeField;

        private bool safeFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public string Assembly
        {
            get
            {
                return this.assemblyField;
            }
            set
            {
                this.assemblyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Namespace
        {
            get
            {
                return this.namespaceField;
            }
            set
            {
                this.namespaceField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string TypeName
        {
            get
            {
                return this.typeNameField;
            }
            set
            {
                this.typeNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TrueFalseMixed Safe
        {
            get
            {
                return this.safeField;
            }
            set
            {
                this.safeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool SafeSpecified
        {
            get
            {
                return this.safeFieldSpecified;
            }
            set
            {
                this.safeFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class AssemblyFileReference
    {

        private SafeControlDefinition[] safeControlsField;

        private ClassResourceDefinition[] classResourcesField;

        private string locationField;

        private SolutionDeploymentTargetType deploymentTargetField;

        private bool deploymentTargetFieldSpecified;

        /// <remarks/>
        [XmlArrayItem("SafeControl", IsNullable = false)]
        public SafeControlDefinition[] SafeControls
        {
            get
            {
                return this.safeControlsField;
            }
            set
            {
                this.safeControlsField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("ClassResource", IsNullable = false)]
        public ClassResourceDefinition[] ClassResources
        {
            get
            {
                return this.classResourcesField;
            }
            set
            {
                this.classResourcesField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public SolutionDeploymentTargetType DeploymentTarget
        {
            get
            {
                return this.deploymentTargetField;
            }
            set
            {
                this.deploymentTargetField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DeploymentTargetSpecified
        {
            get
            {
                return this.deploymentTargetFieldSpecified;
            }
            set
            {
                this.deploymentTargetFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ClassResourceDefinition
    {

        private string locationField;

        private string fileNameField;

        /// <remarks/>
        [XmlAttribute()]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string FileName
        {
            get
            {
                return this.fileNameField;
            }
            set
            {
                this.fileNameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum SolutionDeploymentTargetType
    {

        /// <remarks/>
        GlobalAssemblyCache,

        /// <remarks/>
        WebApplication,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class WebTempFileDefinition
    {

        private string locationField;

        /// <remarks/>
        [XmlAttribute()]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class SiteDefinitionManifestFileReference
    {

        private WebTempFileDefinition[] webTempFileField;

        private string locationField;

        /// <remarks/>
        [XmlElement("WebTempFile")]
        public WebTempFileDefinition[] WebTempFile
        {
            get
            {
                return this.webTempFileField;
            }
            set
            {
                this.webTempFileField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FeatureManifestReference
    {

        private string locationField;

        /// <remarks/>
        [XmlAttribute()]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class DwpFileDefinition
    {

        private string locationField;

        private string fileNameField;

        /// <remarks/>
        [XmlAttribute()]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string FileName
        {
            get
            {
                return this.fileNameField;
            }
            set
            {
                this.fileNameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ModuleReference
    {

        private string nameField;

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class BaseListDefinition
    {

        private ListMetaDataDefinition metaDataField;

        private string[] textField;

        private string nameField;

        private string titleField;

        private string urlField;

        private string defaultField;

        private long webImageWidthField;

        private bool webImageWidthFieldSpecified;

        private TRUEFALSE disableAttachmentsField;

        private bool disableAttachmentsFieldSpecified;

        private string eventSinkAssemblyField;

        private string eventSinkClassField;

        private string eventSinkDataField;

        private TRUEFALSE orderedListField;

        private bool orderedListFieldSpecified;

        private TRUEFALSE privateListField;

        private bool privateListFieldSpecified;

        private string quickLaunchUrlField;

        private TRUEFALSE rootWebOnlyField;

        private bool rootWebOnlyFieldSpecified;

        private int thumbnailSizeField;

        private bool thumbnailSizeFieldSpecified;

        private int typeField;

        private bool typeFieldSpecified;

        private TRUEFALSE uRLEncodeField;

        private bool uRLEncodeFieldSpecified;

        private long webImageHeightField;

        private bool webImageHeightFieldSpecified;

        /// <remarks/>
        public ListMetaDataDefinition MetaData
        {
            get
            {
                return this.metaDataField;
            }
            set
            {
                this.metaDataField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public long WebImageWidth
        {
            get
            {
                return this.webImageWidthField;
            }
            set
            {
                this.webImageWidthField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool WebImageWidthSpecified
        {
            get
            {
                return this.webImageWidthFieldSpecified;
            }
            set
            {
                this.webImageWidthFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE DisableAttachments
        {
            get
            {
                return this.disableAttachmentsField;
            }
            set
            {
                this.disableAttachmentsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DisableAttachmentsSpecified
        {
            get
            {
                return this.disableAttachmentsFieldSpecified;
            }
            set
            {
                this.disableAttachmentsFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string EventSinkAssembly
        {
            get
            {
                return this.eventSinkAssemblyField;
            }
            set
            {
                this.eventSinkAssemblyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string EventSinkClass
        {
            get
            {
                return this.eventSinkClassField;
            }
            set
            {
                this.eventSinkClassField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string EventSinkData
        {
            get
            {
                return this.eventSinkDataField;
            }
            set
            {
                this.eventSinkDataField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE OrderedList
        {
            get
            {
                return this.orderedListField;
            }
            set
            {
                this.orderedListField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool OrderedListSpecified
        {
            get
            {
                return this.orderedListFieldSpecified;
            }
            set
            {
                this.orderedListFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE PrivateList
        {
            get
            {
                return this.privateListField;
            }
            set
            {
                this.privateListField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool PrivateListSpecified
        {
            get
            {
                return this.privateListFieldSpecified;
            }
            set
            {
                this.privateListFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string QuickLaunchUrl
        {
            get
            {
                return this.quickLaunchUrlField;
            }
            set
            {
                this.quickLaunchUrlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE RootWebOnly
        {
            get
            {
                return this.rootWebOnlyField;
            }
            set
            {
                this.rootWebOnlyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RootWebOnlySpecified
        {
            get
            {
                return this.rootWebOnlyFieldSpecified;
            }
            set
            {
                this.rootWebOnlyFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int ThumbnailSize
        {
            get
            {
                return this.thumbnailSizeField;
            }
            set
            {
                this.thumbnailSizeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ThumbnailSizeSpecified
        {
            get
            {
                return this.thumbnailSizeFieldSpecified;
            }
            set
            {
                this.thumbnailSizeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TypeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE URLEncode
        {
            get
            {
                return this.uRLEncodeField;
            }
            set
            {
                this.uRLEncodeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool URLEncodeSpecified
        {
            get
            {
                return this.uRLEncodeFieldSpecified;
            }
            set
            {
                this.uRLEncodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public long WebImageHeight
        {
            get
            {
                return this.webImageHeightField;
            }
            set
            {
                this.webImageHeightField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool WebImageHeightSpecified
        {
            get
            {
                return this.webImageHeightFieldSpecified;
            }
            set
            {
                this.webImageHeightFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ListMetaDataDefinition
    {

        private FieldDefinitions fieldsField;

        private FormDefinition[] formsField;

        private ListMetaDataDefault defaultField;

        private string defaultDescriptionField;

        private ViewDefinition[] viewsField;

        private ToolbarDefinition toolbarField;

        private ContentTypeReferences contentTypesField;

        /// <remarks/>
        public FieldDefinitions Fields
        {
            get
            {
                return this.fieldsField;
            }
            set
            {
                this.fieldsField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("Form", IsNullable = false)]
        public FormDefinition[] Forms
        {
            get
            {
                return this.formsField;
            }
            set
            {
                this.formsField = value;
            }
        }

        /// <remarks/>
        public ListMetaDataDefault Default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }

        /// <remarks/>
        public string DefaultDescription
        {
            get
            {
                return this.defaultDescriptionField;
            }
            set
            {
                this.defaultDescriptionField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("View", IsNullable = false)]
        public ViewDefinition[] Views
        {
            get
            {
                return this.viewsField;
            }
            set
            {
                this.viewsField = value;
            }
        }

        /// <remarks/>
        public ToolbarDefinition Toolbar
        {
            get
            {
                return this.toolbarField;
            }
            set
            {
                this.toolbarField = value;
            }
        }

        /// <remarks/>
        public ContentTypeReferences ContentTypes
        {
            get
            {
                return this.contentTypesField;
            }
            set
            {
                this.contentTypesField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FieldDefinitions
    {

        private FieldDefinition[] fieldField;

        private int revisionField;

        private bool revisionFieldSpecified;

        /// <remarks/>
        [XmlElement("Field")]
        public FieldDefinition[] Field
        {
            get
            {
                return this.fieldField;
            }
            set
            {
                this.fieldField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int Revision
        {
            get
            {
                return this.revisionField;
            }
            set
            {
                this.revisionField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RevisionSpecified
        {
            get
            {
                return this.revisionFieldSpecified;
            }
            set
            {
                this.revisionFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FormDefinition
    {

        private CamlViewRoot listFormOpeningField;

        private CamlViewRoot listFormButtonsField;

        private CamlViewRoot listFormBodyField;

        private CamlViewRoot listFormClosingField;

        private string pathField;

        private string setupPathField;

        private string urlField;

        private FormType typeField;

        private bool typeFieldSpecified;

        private TRUEFALSE useLegacyFormField;

        private bool useLegacyFormFieldSpecified;

        private string templateField;

        private string webPartZoneIDField;

        /// <remarks/>
        public CamlViewRoot ListFormOpening
        {
            get
            {
                return this.listFormOpeningField;
            }
            set
            {
                this.listFormOpeningField = value;
            }
        }

        /// <remarks/>
        public CamlViewRoot ListFormButtons
        {
            get
            {
                return this.listFormButtonsField;
            }
            set
            {
                this.listFormButtonsField = value;
            }
        }

        /// <remarks/>
        public CamlViewRoot ListFormBody
        {
            get
            {
                return this.listFormBodyField;
            }
            set
            {
                this.listFormBodyField = value;
            }
        }

        /// <remarks/>
        public CamlViewRoot ListFormClosing
        {
            get
            {
                return this.listFormClosingField;
            }
            set
            {
                this.listFormClosingField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string SetupPath
        {
            get
            {
                return this.setupPathField;
            }
            set
            {
                this.setupPathField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public FormType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TypeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE UseLegacyForm
        {
            get
            {
                return this.useLegacyFormField;
            }
            set
            {
                this.useLegacyFormField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool UseLegacyFormSpecified
        {
            get
            {
                return this.useLegacyFormFieldSpecified;
            }
            set
            {
                this.useLegacyFormFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Template
        {
            get
            {
                return this.templateField;
            }
            set
            {
                this.templateField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string WebPartZoneID
        {
            get
            {
                return this.webPartZoneIDField;
            }
            set
            {
                this.webPartZoneIDField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum FormType
    {

        /// <remarks/>
        DisplayForm,

        /// <remarks/>
        EditForm,

        /// <remarks/>
        NewForm,

        /// <remarks/>
        NewFormDialog,

        /// <remarks/>
        [XmlEnum("")]
        Item,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ListMetaDataDefault
    {

        private FormDefinition[] formsField;

        /// <remarks/>
        [XmlArrayItem("Form", IsNullable = false)]
        public FormDefinition[] Forms
        {
            get
            {
                return this.formsField;
            }
            set
            {
                this.formsField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ViewDefinition
    {

        private CamlViewRoot pagedRowsetField;

        private ToolbarDefinition toolbarField;

        private CamlQueryRoot queryField;

        private FieldRefDefinitions viewFieldsField;

        private CamlViewRoot groupByHeaderField;

        private CamlViewRoot groupByFooterField;

        private CamlViewRoot viewHeaderField;

        private CamlViewRoot viewBodyField;

        private CamlViewRoot viewFooterField;

        private CamlViewRoot rowLimitExceededField;

        private CamlViewRoot viewEmptyField;

        private CamlViewRoot pagedRecurrenceRowsetField;

        private CamlViewRoot pagedClientCallbackRowsetField;

        private RowLimitDefinition rowLimitField;

        private ViewStyleReference viewStyleField;

        private ViewDataFieldRefDefinitions viewDataField;

        private TRUEFALSE aggregateViewField;

        private bool aggregateViewFieldSpecified;

        private int baseViewIDField;

        private bool baseViewIDFieldSpecified;

        private TRUEFALSE defaultViewField;

        private bool defaultViewFieldSpecified;

        private string displayNameField;

        private TRUEFALSE failIfEmptyField;

        private bool failIfEmptyFieldSpecified;

        private TRUEFALSE fileDialogField;

        private bool fileDialogFieldSpecified;

        private TRUEFALSE fPModifiedField;

        private bool fPModifiedFieldSpecified;

        private TRUEFALSE hiddenField;

        private bool hiddenFieldSpecified;

        private int listField;

        private bool listFieldSpecified;

        private string nameField;

        private string contentTypeIDField;

        private TRUEFALSE orderedViewField;

        private bool orderedViewFieldSpecified;

        private TRUEFALSE defaultViewForContentTypeField;

        private bool defaultViewForContentTypeFieldSpecified;

        private TRUEFALSE includeRootFolderField;

        private bool includeRootFolderFieldSpecified;

        private string pageTypeField;

        private string pathField;

        private TRUEFALSE readOnlyField;

        private bool readOnlyFieldSpecified;

        private TRUEFALSE recurrenceRowsetField;

        private bool recurrenceRowsetFieldSpecified;

        private TRUEFALSE requiresClientIntegrationField;

        private bool requiresClientIntegrationFieldSpecified;

        private int rowLimit1Field;

        private bool rowLimit1FieldSpecified;

        private string scopeField;

        private TRUEFALSE showHeaderUIField;

        private bool showHeaderUIFieldSpecified;

        private TRUEFALSE threadedField;

        private bool threadedFieldSpecified;

        private string typeField;

        private string urlField;

        private int webPartOrderField;

        private bool webPartOrderFieldSpecified;

        private string webPartZoneIDField;

        private TRUEFALSE freeFormField;

        private bool freeFormFieldSpecified;

        private string imageUrlField;

        private string setupPathField;

        private string moderationTypeField;

        private string toolbarTemplateField;

        private TRUEFALSE mobileViewField;

        private bool mobileViewFieldSpecified;

        private TRUEFALSE mobileDefaultViewField;

        private bool mobileDefaultViewFieldSpecified;

        private TRUEFALSE reqAuthField;

        private bool reqAuthFieldSpecified;

        private ViewRegistrationType targetTypeField;

        private bool targetTypeFieldSpecified;

        private string targetIdField;

        /// <remarks/>
        public CamlViewRoot PagedRowset
        {
            get
            {
                return this.pagedRowsetField;
            }
            set
            {
                this.pagedRowsetField = value;
            }
        }

        /// <remarks/>
        public ToolbarDefinition Toolbar
        {
            get
            {
                return this.toolbarField;
            }
            set
            {
                this.toolbarField = value;
            }
        }

        /// <remarks/>
        public CamlQueryRoot Query
        {
            get
            {
                return this.queryField;
            }
            set
            {
                this.queryField = value;
            }
        }

        /// <remarks/>
        public FieldRefDefinitions ViewFields
        {
            get
            {
                return this.viewFieldsField;
            }
            set
            {
                this.viewFieldsField = value;
            }
        }

        /// <remarks/>
        public CamlViewRoot GroupByHeader
        {
            get
            {
                return this.groupByHeaderField;
            }
            set
            {
                this.groupByHeaderField = value;
            }
        }

        /// <remarks/>
        public CamlViewRoot GroupByFooter
        {
            get
            {
                return this.groupByFooterField;
            }
            set
            {
                this.groupByFooterField = value;
            }
        }

        /// <remarks/>
        public CamlViewRoot ViewHeader
        {
            get
            {
                return this.viewHeaderField;
            }
            set
            {
                this.viewHeaderField = value;
            }
        }

        /// <remarks/>
        public CamlViewRoot ViewBody
        {
            get
            {
                return this.viewBodyField;
            }
            set
            {
                this.viewBodyField = value;
            }
        }

        /// <remarks/>
        public CamlViewRoot ViewFooter
        {
            get
            {
                return this.viewFooterField;
            }
            set
            {
                this.viewFooterField = value;
            }
        }

        /// <remarks/>
        public CamlViewRoot RowLimitExceeded
        {
            get
            {
                return this.rowLimitExceededField;
            }
            set
            {
                this.rowLimitExceededField = value;
            }
        }

        /// <remarks/>
        public CamlViewRoot ViewEmpty
        {
            get
            {
                return this.viewEmptyField;
            }
            set
            {
                this.viewEmptyField = value;
            }
        }

        /// <remarks/>
        public CamlViewRoot PagedRecurrenceRowset
        {
            get
            {
                return this.pagedRecurrenceRowsetField;
            }
            set
            {
                this.pagedRecurrenceRowsetField = value;
            }
        }

        /// <remarks/>
        public CamlViewRoot PagedClientCallbackRowset
        {
            get
            {
                return this.pagedClientCallbackRowsetField;
            }
            set
            {
                this.pagedClientCallbackRowsetField = value;
            }
        }

        /// <remarks/>
        public RowLimitDefinition RowLimit
        {
            get
            {
                return this.rowLimitField;
            }
            set
            {
                this.rowLimitField = value;
            }
        }

        /// <remarks/>
        public ViewStyleReference ViewStyle
        {
            get
            {
                return this.viewStyleField;
            }
            set
            {
                this.viewStyleField = value;
            }
        }

        /// <remarks/>
        public ViewDataFieldRefDefinitions ViewData
        {
            get
            {
                return this.viewDataField;
            }
            set
            {
                this.viewDataField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE AggregateView
        {
            get
            {
                return this.aggregateViewField;
            }
            set
            {
                this.aggregateViewField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AggregateViewSpecified
        {
            get
            {
                return this.aggregateViewFieldSpecified;
            }
            set
            {
                this.aggregateViewFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int BaseViewID
        {
            get
            {
                return this.baseViewIDField;
            }
            set
            {
                this.baseViewIDField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool BaseViewIDSpecified
        {
            get
            {
                return this.baseViewIDFieldSpecified;
            }
            set
            {
                this.baseViewIDFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE DefaultView
        {
            get
            {
                return this.defaultViewField;
            }
            set
            {
                this.defaultViewField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DefaultViewSpecified
        {
            get
            {
                return this.defaultViewFieldSpecified;
            }
            set
            {
                this.defaultViewFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE FailIfEmpty
        {
            get
            {
                return this.failIfEmptyField;
            }
            set
            {
                this.failIfEmptyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FailIfEmptySpecified
        {
            get
            {
                return this.failIfEmptyFieldSpecified;
            }
            set
            {
                this.failIfEmptyFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE FileDialog
        {
            get
            {
                return this.fileDialogField;
            }
            set
            {
                this.fileDialogField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FileDialogSpecified
        {
            get
            {
                return this.fileDialogFieldSpecified;
            }
            set
            {
                this.fileDialogFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE FPModified
        {
            get
            {
                return this.fPModifiedField;
            }
            set
            {
                this.fPModifiedField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FPModifiedSpecified
        {
            get
            {
                return this.fPModifiedFieldSpecified;
            }
            set
            {
                this.fPModifiedFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Hidden
        {
            get
            {
                return this.hiddenField;
            }
            set
            {
                this.hiddenField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HiddenSpecified
        {
            get
            {
                return this.hiddenFieldSpecified;
            }
            set
            {
                this.hiddenFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int List
        {
            get
            {
                return this.listField;
            }
            set
            {
                this.listField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ListSpecified
        {
            get
            {
                return this.listFieldSpecified;
            }
            set
            {
                this.listFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ContentTypeID
        {
            get
            {
                return this.contentTypeIDField;
            }
            set
            {
                this.contentTypeIDField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE OrderedView
        {
            get
            {
                return this.orderedViewField;
            }
            set
            {
                this.orderedViewField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool OrderedViewSpecified
        {
            get
            {
                return this.orderedViewFieldSpecified;
            }
            set
            {
                this.orderedViewFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE DefaultViewForContentType
        {
            get
            {
                return this.defaultViewForContentTypeField;
            }
            set
            {
                this.defaultViewForContentTypeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DefaultViewForContentTypeSpecified
        {
            get
            {
                return this.defaultViewForContentTypeFieldSpecified;
            }
            set
            {
                this.defaultViewForContentTypeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE IncludeRootFolder
        {
            get
            {
                return this.includeRootFolderField;
            }
            set
            {
                this.includeRootFolderField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool IncludeRootFolderSpecified
        {
            get
            {
                return this.includeRootFolderFieldSpecified;
            }
            set
            {
                this.includeRootFolderFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string PageType
        {
            get
            {
                return this.pageTypeField;
            }
            set
            {
                this.pageTypeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ReadOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ReadOnlySpecified
        {
            get
            {
                return this.readOnlyFieldSpecified;
            }
            set
            {
                this.readOnlyFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE RecurrenceRowset
        {
            get
            {
                return this.recurrenceRowsetField;
            }
            set
            {
                this.recurrenceRowsetField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RecurrenceRowsetSpecified
        {
            get
            {
                return this.recurrenceRowsetFieldSpecified;
            }
            set
            {
                this.recurrenceRowsetFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE RequiresClientIntegration
        {
            get
            {
                return this.requiresClientIntegrationField;
            }
            set
            {
                this.requiresClientIntegrationField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RequiresClientIntegrationSpecified
        {
            get
            {
                return this.requiresClientIntegrationFieldSpecified;
            }
            set
            {
                this.requiresClientIntegrationFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("RowLimit")]
        public int RowLimit1
        {
            get
            {
                return this.rowLimit1Field;
            }
            set
            {
                this.rowLimit1Field = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RowLimit1Specified
        {
            get
            {
                return this.rowLimit1FieldSpecified;
            }
            set
            {
                this.rowLimit1FieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Scope
        {
            get
            {
                return this.scopeField;
            }
            set
            {
                this.scopeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ShowHeaderUI
        {
            get
            {
                return this.showHeaderUIField;
            }
            set
            {
                this.showHeaderUIField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowHeaderUISpecified
        {
            get
            {
                return this.showHeaderUIFieldSpecified;
            }
            set
            {
                this.showHeaderUIFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Threaded
        {
            get
            {
                return this.threadedField;
            }
            set
            {
                this.threadedField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ThreadedSpecified
        {
            get
            {
                return this.threadedFieldSpecified;
            }
            set
            {
                this.threadedFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int WebPartOrder
        {
            get
            {
                return this.webPartOrderField;
            }
            set
            {
                this.webPartOrderField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool WebPartOrderSpecified
        {
            get
            {
                return this.webPartOrderFieldSpecified;
            }
            set
            {
                this.webPartOrderFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string WebPartZoneID
        {
            get
            {
                return this.webPartZoneIDField;
            }
            set
            {
                this.webPartZoneIDField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE FreeForm
        {
            get
            {
                return this.freeFormField;
            }
            set
            {
                this.freeFormField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FreeFormSpecified
        {
            get
            {
                return this.freeFormFieldSpecified;
            }
            set
            {
                this.freeFormFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ImageUrl
        {
            get
            {
                return this.imageUrlField;
            }
            set
            {
                this.imageUrlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string SetupPath
        {
            get
            {
                return this.setupPathField;
            }
            set
            {
                this.setupPathField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ModerationType
        {
            get
            {
                return this.moderationTypeField;
            }
            set
            {
                this.moderationTypeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ToolbarTemplate
        {
            get
            {
                return this.toolbarTemplateField;
            }
            set
            {
                this.toolbarTemplateField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE MobileView
        {
            get
            {
                return this.mobileViewField;
            }
            set
            {
                this.mobileViewField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool MobileViewSpecified
        {
            get
            {
                return this.mobileViewFieldSpecified;
            }
            set
            {
                this.mobileViewFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE MobileDefaultView
        {
            get
            {
                return this.mobileDefaultViewField;
            }
            set
            {
                this.mobileDefaultViewField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool MobileDefaultViewSpecified
        {
            get
            {
                return this.mobileDefaultViewFieldSpecified;
            }
            set
            {
                this.mobileDefaultViewFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ReqAuth
        {
            get
            {
                return this.reqAuthField;
            }
            set
            {
                this.reqAuthField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ReqAuthSpecified
        {
            get
            {
                return this.reqAuthFieldSpecified;
            }
            set
            {
                this.reqAuthFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public ViewRegistrationType TargetType
        {
            get
            {
                return this.targetTypeField;
            }
            set
            {
                this.targetTypeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TargetTypeSpecified
        {
            get
            {
                return this.targetTypeFieldSpecified;
            }
            set
            {
                this.targetTypeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string TargetId
        {
            get
            {
                return this.targetIdField;
            }
            set
            {
                this.targetIdField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class RowLimitDefinition
    {

        private TRUEFALSE pagedField;

        private bool pagedFieldSpecified;

        private int valueField;

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Paged
        {
            get
            {
                return this.pagedField;
            }
            set
            {
                this.pagedField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool PagedSpecified
        {
            get
            {
                return this.pagedFieldSpecified;
            }
            set
            {
                this.pagedFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public int Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ViewStyleReference
    {

        private int idField;

        private bool idFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public int ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool IDSpecified
        {
            get
            {
                return this.idFieldSpecified;
            }
            set
            {
                this.idFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ViewDataFieldRefDefinitions
    {

        private ViewDataFieldRefDefinition[] fieldRefField;

        private string[] textField;

        /// <remarks/>
        [XmlElement("FieldRef")]
        public ViewDataFieldRefDefinition[] FieldRef
        {
            get
            {
                return this.fieldRefField;
            }
            set
            {
                this.fieldRefField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ViewDataFieldRefDefinition
    {

        private string nameField;

        private string typeField;

        private string[] textField;

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum ViewRegistrationType
    {

        /// <remarks/>
        List,

        /// <remarks/>
        ContentType,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ContentTypeReferences
    {

        private object[] itemsField;

        /// <remarks/>
        [XmlElement("ContentType", typeof(ContentTypeDefinition))]
        [XmlElement("ContentTypeRef", typeof(ContentTypeReference))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ContentTypeDefinition
    {

        private CTFolderDefinition folderField;

        private CTFieldRefDefinitions fieldRefsField;

        private FormDefinition[] formsField;

        private XmlDocumentDefinitions xmlDocumentsField;

        private ContentTypeDocumentTemplateDefinition documentTemplateField;

        private string[] textField;

        private string baseTypeField;

        private string idField;

        private string nameField;

        private string groupField;

        private string documentTemplate1Field;

        private string resourceFolderField;

        private TRUEFALSE readOnlyField;

        private bool readOnlyFieldSpecified;

        private TRUEFALSE hiddenField;

        private bool hiddenFieldSpecified;

        private string descriptionField;

        private TRUEFALSE sealedField;

        private bool sealedFieldSpecified;

        private string v2ListTemplateNameField;

        private long versionField;

        private bool versionFieldSpecified;

        private string featureIdField;

        private string progIdField;

        /// <remarks/>
        [XmlIgnore()]
        public CTFolderDefinition Folder
        {
            get
            {
                return this.folderField;
            }
            set
            {
                this.folderField = value;
            }
        }

        /// <remarks/>
        public CTFieldRefDefinitions FieldRefs
        {
            get
            {
                return this.fieldRefsField;
            }
            set
            {
                this.fieldRefsField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlArrayAttribute(IsNullable=true)]
        //[System.Xml.Serialization.XmlArrayItemAttribute("Form", IsNullable=false)]
        [XmlIgnore()]
        public FormDefinition[] Forms
        {
            get
            {
                return this.formsField;
            }
            set
            {
                this.formsField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlArrayItemAttribute("XmlDocument", IsNullable=false)]        
        public XmlDocumentDefinitions XmlDocuments
        {
            get
            {
                return this.xmlDocumentsField;
            }
            set
            {
                this.xmlDocumentsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public ContentTypeDocumentTemplateDefinition DocumentTemplate
        {
            get
            {
                return this.documentTemplateField;
            }
            set
            {
                this.documentTemplateField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string BaseType
        {
            get
            {
                return this.baseTypeField;
            }
            set
            {
                this.baseTypeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Group
        {
            get
            {
                return this.groupField;
            }
            set
            {
                this.groupField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ProgId
        {
            get
            {
                return this.progIdField;
            }
            set
            {
                this.progIdField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("DocumentTemplate")]
        public string DocumentTemplate1
        {
            get
            {
                return this.documentTemplate1Field;
            }
            set
            {
                this.documentTemplate1Field = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ResourceFolder
        {
            get
            {
                return this.resourceFolderField;
            }
            set
            {
                this.resourceFolderField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        public TRUEFALSE ReadOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ReadOnlySpecified
        {
            get
            {
                return this.readOnlyFieldSpecified;
            }
            set
            {
                this.readOnlyFieldSpecified = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        public TRUEFALSE Hidden
        {
            get
            {
                return this.hiddenField;
            }
            set
            {
                this.hiddenField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HiddenSpecified
        {
            get
            {
                return this.hiddenFieldSpecified;
            }
            set
            {
                this.hiddenFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        public TRUEFALSE Sealed
        {
            get
            {
                return this.sealedField;
            }
            set
            {
                this.sealedField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool SealedSpecified
        {
            get
            {
                return this.sealedFieldSpecified;
            }
            set
            {
                this.sealedFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string V2ListTemplateName
        {
            get
            {
                return this.v2ListTemplateNameField;
            }
            set
            {
                this.v2ListTemplateNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public long Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool VersionSpecified
        {
            get
            {
                return this.versionFieldSpecified;
            }
            set
            {
                this.versionFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string FeatureId
        {
            get
            {
                return this.featureIdField;
            }
            set
            {
                this.featureIdField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class CTFolderDefinition
    {

        private string[] textField;

        private string targetNameField;

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string TargetName
        {
            get
            {
                return this.targetNameField;
            }
            set
            {
                this.targetNameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class CTFieldRefDefinitions
    {

        private object[] itemsField;

        private ItemsChoiceType1[] itemsElementNameField;

        /// <remarks/>
        [XmlElement("DocumentTemplate", typeof(string))]
        [XmlElement("FieldRef", typeof(CTFieldRefDefinition))]
        [XmlElement("RemoveFieldRef", typeof(CTFieldRefDefinition))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [XmlElement("ItemsElementName")]
        [XmlIgnore()]
        public ItemsChoiceType1[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class CTFieldRefDefinition
    {

        private string defaultValueField;

        private string descriptionField;

        private string displayNameField;

        private string formatField;

        private TRUEFALSE filterableField;

        private bool filterableFieldSpecified;

        private TRUEFALSE filterableNoRecurrenceField;

        private bool filterableNoRecurrenceFieldSpecified;

        private TRUEFALSE fromBaseTypeField;

        private bool fromBaseTypeFieldSpecified;

        private TRUEFALSEorResource hiddenField;

        private bool hiddenFieldSpecified;

        private string idField;

        private TRUEFALSE lockedField;

        private bool lockedFieldSpecified;

        private string nameField;

        private string nodeField;

        private long numLinesField;

        private bool numLinesFieldSpecified;

        private TRUEFALSE readOnlyField;

        private bool readOnlyFieldSpecified;

        private TRUEFALSE readOnlyClientField;

        private bool readOnlyClientFieldSpecified;

        private TRUEFALSE requiredField;

        private bool requiredFieldSpecified;

        private TRUEFALSE sealedField;

        private bool sealedFieldSpecified;

        private TRUEFALSE showInDisplayFormField;

        private bool showInDisplayFormFieldSpecified;

        private TRUEFALSE showInEditFormField;

        private bool showInEditFormFieldSpecified;

        private TRUEFALSE showInFileDlgField;

        private bool showInFileDlgFieldSpecified;

        private TRUEFALSE showInListSettingsField;

        private bool showInListSettingsFieldSpecified;

        private TRUEFALSE showInNewFormField;

        private bool showInNewFormFieldSpecified;

        private TRUEFALSE sortableField;

        private bool sortableFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public string DefaultValue
        {
            get
            {
                return this.defaultValueField;
            }
            set
            {
                this.defaultValueField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Format
        {
            get
            {
                return this.formatField;
            }
            set
            {
                this.formatField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Filterable
        {
            get
            {
                return this.filterableField;
            }
            set
            {
                this.filterableField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FilterableSpecified
        {
            get
            {
                return this.filterableFieldSpecified;
            }
            set
            {
                this.filterableFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE FilterableNoRecurrence
        {
            get
            {
                return this.filterableNoRecurrenceField;
            }
            set
            {
                this.filterableNoRecurrenceField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FilterableNoRecurrenceSpecified
        {
            get
            {
                return this.filterableNoRecurrenceFieldSpecified;
            }
            set
            {
                this.filterableNoRecurrenceFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE FromBaseType
        {
            get
            {
                return this.fromBaseTypeField;
            }
            set
            {
                this.fromBaseTypeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FromBaseTypeSpecified
        {
            get
            {
                return this.fromBaseTypeFieldSpecified;
            }
            set
            {
                this.fromBaseTypeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSEorResource Hidden
        {
            get
            {
                return this.hiddenField;
            }
            set
            {
                this.hiddenField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HiddenSpecified
        {
            get
            {
                return this.hiddenFieldSpecified;
            }
            set
            {
                this.hiddenFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Locked
        {
            get
            {
                return this.lockedField;
            }
            set
            {
                this.lockedField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool LockedSpecified
        {
            get
            {
                return this.lockedFieldSpecified;
            }
            set
            {
                this.lockedFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Node
        {
            get
            {
                return this.nodeField;
            }
            set
            {
                this.nodeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public long NumLines
        {
            get
            {
                return this.numLinesField;
            }
            set
            {
                this.numLinesField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool NumLinesSpecified
        {
            get
            {
                return this.numLinesFieldSpecified;
            }
            set
            {
                this.numLinesFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ReadOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ReadOnlySpecified
        {
            get
            {
                return this.readOnlyFieldSpecified;
            }
            set
            {
                this.readOnlyFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ReadOnlyClient
        {
            get
            {
                return this.readOnlyClientField;
            }
            set
            {
                this.readOnlyClientField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ReadOnlyClientSpecified
        {
            get
            {
                return this.readOnlyClientFieldSpecified;
            }
            set
            {
                this.readOnlyClientFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Required
        {
            get
            {
                return this.requiredField;
            }
            set
            {
                this.requiredField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RequiredSpecified
        {
            get
            {
                return this.requiredFieldSpecified;
            }
            set
            {
                this.requiredFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Sealed
        {
            get
            {
                return this.sealedField;
            }
            set
            {
                this.sealedField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool SealedSpecified
        {
            get
            {
                return this.sealedFieldSpecified;
            }
            set
            {
                this.sealedFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ShowInDisplayForm
        {
            get
            {
                return this.showInDisplayFormField;
            }
            set
            {
                this.showInDisplayFormField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowInDisplayFormSpecified
        {
            get
            {
                return this.showInDisplayFormFieldSpecified;
            }
            set
            {
                this.showInDisplayFormFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ShowInEditForm
        {
            get
            {
                return this.showInEditFormField;
            }
            set
            {
                this.showInEditFormField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowInEditFormSpecified
        {
            get
            {
                return this.showInEditFormFieldSpecified;
            }
            set
            {
                this.showInEditFormFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ShowInFileDlg
        {
            get
            {
                return this.showInFileDlgField;
            }
            set
            {
                this.showInFileDlgField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowInFileDlgSpecified
        {
            get
            {
                return this.showInFileDlgFieldSpecified;
            }
            set
            {
                this.showInFileDlgFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ShowInListSettings
        {
            get
            {
                return this.showInListSettingsField;
            }
            set
            {
                this.showInListSettingsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowInListSettingsSpecified
        {
            get
            {
                return this.showInListSettingsFieldSpecified;
            }
            set
            {
                this.showInListSettingsFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ShowInNewForm
        {
            get
            {
                return this.showInNewFormField;
            }
            set
            {
                this.showInNewFormField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowInNewFormSpecified
        {
            get
            {
                return this.showInNewFormFieldSpecified;
            }
            set
            {
                this.showInNewFormFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Sortable
        {
            get
            {
                return this.sortableField;
            }
            set
            {
                this.sortableField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool SortableSpecified
        {
            get
            {
                return this.sortableFieldSpecified;
            }
            set
            {
                this.sortableFieldSpecified = value;
            }
        }
    }
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class XmlDocumentDefinition
    {
        public object[] itemsField;

        [XmlElement("Receivers", typeof(XmlDocumentDefinitionReceivers))]

        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
        public string namespaceURIField;

        /// <remarks/>
        [XmlAttribute()]
        public string NamespaceURI
        {
            get
            {
                return this.namespaceURIField;
            }
            set
            {
                this.namespaceURIField = value;
            }
        }

    }
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class XmlDocumentDefinitionReceivers
    {
        public object[] itemsField;

        [XmlElement("Receiver", typeof(XmlDocumentDefinitionReceiver))]

        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

    }
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class XmlDocumentDefinitionReceiver
    {
        public object[] itemsField;

        [XmlElement("Name", typeof(XmlDocumentDefinitionReceiverName))]
        [XmlElement("Type", typeof(XmlDocumentDefinitionReceiverType))]
        [XmlElement("Assembly", typeof(XmlDocumentDefinitionReceiverAssembly))]
        [XmlElement("SequenceNumber", typeof(XmlDocumentDefinitionReceiverSequenceNumber))]
        [XmlElement("Class", typeof(XmlDocumentDefinitionReceiverClass))]

        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

    }
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class XmlDocumentDefinitionReceiverClass
    {
        private string valueField;

        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

    }
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class XmlDocumentDefinitionReceiverAssembly
    {
        private string valueField;

        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

    }
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class XmlDocumentDefinitionReceiverSequenceNumber
    {
        private string valueField;

        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class XmlDocumentDefinitionReceiverName
    {
        private string valueField;

        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

    }
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class XmlDocumentDefinitionReceiverType
    {
        private string valueField;

        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

    }


    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum TRUEFALSEorResource
    {

        /// <remarks/>
        TRUE,

        /// <remarks/>
        FALSE,

        /// <remarks/>
        @true,

        /// <remarks/>
        @false,

        /// <remarks/>
        [XmlEnum("$Resources:core,True_Unless_Jpn")]
        ResourcescoreTrue_Unless_Jpn,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/", IncludeInSchema = false)]
    public enum ItemsChoiceType1
    {

        /// <remarks/>
        DocumentTemplate,

        /// <remarks/>
        FieldRef,

        /// <remarks/>
        RemoveFieldRef,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ContentTypeReference
    {

        private FolderReference folderField;

        private string idField;

        /// <remarks/>
        public FolderReference Folder
        {
            get
            {
                return this.folderField;
            }
            set
            {
                this.folderField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FolderReference
    {

        private string targetNameField;

        /// <remarks/>
        [XmlAttribute()]
        public string TargetName
        {
            get
            {
                return this.targetNameField;
            }
            set
            {
                this.targetNameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ServerEmailFooterDefinition
    {

        private string nameField;

        private string valueField;

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FileDialogPostProcessorDefinition
    {

        private string idField;

        /// <remarks/>
        [XmlAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ExternalSecurityProviderDefinition
    {

        private string idField;

        private string typeField;

        /// <remarks/>
        [XmlAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ComponentsDefinition
    {

        private ExternalSecurityProviderDefinition externalSecurityProviderField;

        private FileDialogPostProcessorDefinition fileDialogPostProcessorField;

        /// <remarks/>
        public ExternalSecurityProviderDefinition ExternalSecurityProvider
        {
            get
            {
                return this.externalSecurityProviderField;
            }
            set
            {
                this.externalSecurityProviderField = value;
            }
        }

        /// <remarks/>
        public FileDialogPostProcessorDefinition FileDialogPostProcessor
        {
            get
            {
                return this.fileDialogPostProcessorField;
            }
            set
            {
                this.fileDialogPostProcessorField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FeatureTemplateReference
    {

        private FeaturePropertyDefinition[] propertiesField;

        private string idField;

        /// <remarks/>
        [XmlArrayItem("Property", IsNullable = false)]
        public FeaturePropertyDefinition[] Properties
        {
            get
            {
                return this.propertiesField;
            }
            set
            {
                this.propertiesField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class PropertyValueAttributeDefinition
    {

        private string nameField;

        private string valueField;

        private string value1Field;

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string Value1
        {
            get
            {
                return this.value1Field;
            }
            set
            {
                this.value1Field = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class NavBarPageDefinition
    {

        private string[] textField;

        private string positionField;

        private int idField;

        private bool idFieldSpecified;

        private string nameField;

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Position
        {
            get
            {
                return this.positionField;
            }
            set
            {
                this.positionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool IDSpecified
        {
            get
            {
                return this.idFieldSpecified;
            }
            set
            {
                this.idFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [XmlInclude(typeof(ViewWebPartDefinition))]
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class WebPartDefinition
    {

        private string[] textField;

        private int webPartOrderField;

        private bool webPartOrderFieldSpecified;

        private string webPartZoneIDField;

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int WebPartOrder
        {
            get
            {
                return this.webPartOrderField;
            }
            set
            {
                this.webPartOrderField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool WebPartOrderSpecified
        {
            get
            {
                return this.webPartOrderFieldSpecified;
            }
            set
            {
                this.webPartOrderFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string WebPartZoneID
        {
            get
            {
                return this.webPartZoneIDField;
            }
            set
            {
                this.webPartZoneIDField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ViewWebPartDefinition : WebPartDefinition
    {

        private TRUEFALSE aggregateViewField;

        private bool aggregateViewFieldSpecified;

        private int baseViewIDField;

        private bool baseViewIDFieldSpecified;

        private TRUEFALSE defaultViewField;

        private bool defaultViewFieldSpecified;

        private string displayNameField;

        private TRUEFALSE failIfEmptyField;

        private bool failIfEmptyFieldSpecified;

        private TRUEFALSE fileDialogField;

        private bool fileDialogFieldSpecified;

        private TRUEFALSE fPModifiedField;

        private bool fPModifiedFieldSpecified;

        private TRUEFALSE freeFormField;

        private bool freeFormFieldSpecified;

        private TRUEFALSE hiddenField;

        private bool hiddenFieldSpecified;

        private string listField;

        private string nameField;

        private TRUEFALSE orderedViewField;

        private bool orderedViewFieldSpecified;

        private string pathField;

        private string pageTypeField;

        private TRUEFALSE readOnlyField;

        private bool readOnlyFieldSpecified;

        private TRUEFALSE recurrenceRowsetField;

        private bool recurrenceRowsetFieldSpecified;

        private int rowLimitField;

        private bool rowLimitFieldSpecified;

        private string scopeField;

        private TRUEFALSE showHeaderUIField;

        private bool showHeaderUIFieldSpecified;

        private TRUEFALSE threadedField;

        private bool threadedFieldSpecified;

        private ViewPartType typeField;

        private bool typeFieldSpecified;

        private string urlField;

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE AggregateView
        {
            get
            {
                return this.aggregateViewField;
            }
            set
            {
                this.aggregateViewField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AggregateViewSpecified
        {
            get
            {
                return this.aggregateViewFieldSpecified;
            }
            set
            {
                this.aggregateViewFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int BaseViewID
        {
            get
            {
                return this.baseViewIDField;
            }
            set
            {
                this.baseViewIDField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool BaseViewIDSpecified
        {
            get
            {
                return this.baseViewIDFieldSpecified;
            }
            set
            {
                this.baseViewIDFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE DefaultView
        {
            get
            {
                return this.defaultViewField;
            }
            set
            {
                this.defaultViewField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DefaultViewSpecified
        {
            get
            {
                return this.defaultViewFieldSpecified;
            }
            set
            {
                this.defaultViewFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE FailIfEmpty
        {
            get
            {
                return this.failIfEmptyField;
            }
            set
            {
                this.failIfEmptyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FailIfEmptySpecified
        {
            get
            {
                return this.failIfEmptyFieldSpecified;
            }
            set
            {
                this.failIfEmptyFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE FileDialog
        {
            get
            {
                return this.fileDialogField;
            }
            set
            {
                this.fileDialogField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FileDialogSpecified
        {
            get
            {
                return this.fileDialogFieldSpecified;
            }
            set
            {
                this.fileDialogFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE FPModified
        {
            get
            {
                return this.fPModifiedField;
            }
            set
            {
                this.fPModifiedField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FPModifiedSpecified
        {
            get
            {
                return this.fPModifiedFieldSpecified;
            }
            set
            {
                this.fPModifiedFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE FreeForm
        {
            get
            {
                return this.freeFormField;
            }
            set
            {
                this.freeFormField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FreeFormSpecified
        {
            get
            {
                return this.freeFormFieldSpecified;
            }
            set
            {
                this.freeFormFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Hidden
        {
            get
            {
                return this.hiddenField;
            }
            set
            {
                this.hiddenField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HiddenSpecified
        {
            get
            {
                return this.hiddenFieldSpecified;
            }
            set
            {
                this.hiddenFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string List
        {
            get
            {
                return this.listField;
            }
            set
            {
                this.listField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE OrderedView
        {
            get
            {
                return this.orderedViewField;
            }
            set
            {
                this.orderedViewField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool OrderedViewSpecified
        {
            get
            {
                return this.orderedViewFieldSpecified;
            }
            set
            {
                this.orderedViewFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string PageType
        {
            get
            {
                return this.pageTypeField;
            }
            set
            {
                this.pageTypeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ReadOnly
        {
            get
            {
                return this.readOnlyField;
            }
            set
            {
                this.readOnlyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ReadOnlySpecified
        {
            get
            {
                return this.readOnlyFieldSpecified;
            }
            set
            {
                this.readOnlyFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE RecurrenceRowset
        {
            get
            {
                return this.recurrenceRowsetField;
            }
            set
            {
                this.recurrenceRowsetField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RecurrenceRowsetSpecified
        {
            get
            {
                return this.recurrenceRowsetFieldSpecified;
            }
            set
            {
                this.recurrenceRowsetFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int RowLimit
        {
            get
            {
                return this.rowLimitField;
            }
            set
            {
                this.rowLimitField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RowLimitSpecified
        {
            get
            {
                return this.rowLimitFieldSpecified;
            }
            set
            {
                this.rowLimitFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Scope
        {
            get
            {
                return this.scopeField;
            }
            set
            {
                this.scopeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ShowHeaderUI
        {
            get
            {
                return this.showHeaderUIField;
            }
            set
            {
                this.showHeaderUIField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ShowHeaderUISpecified
        {
            get
            {
                return this.showHeaderUIFieldSpecified;
            }
            set
            {
                this.showHeaderUIFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Threaded
        {
            get
            {
                return this.threadedField;
            }
            set
            {
                this.threadedField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ThreadedSpecified
        {
            get
            {
                return this.threadedFieldSpecified;
            }
            set
            {
                this.threadedFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public ViewPartType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TypeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum ViewPartType
    {

        /// <remarks/>
        HTML,

        /// <remarks/>
        Chart,

        /// <remarks/>
        Pivot,

        /// <remarks/>
        GANTT,

        /// <remarks/>
        CALENDAR,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FileDefinition
    {

        private object[] itemsField;

        private TRUEFALSE ignoreIfAlreadyExistsField;

        private bool ignoreIfAlreadyExistsFieldSpecified;

        private string nameField;

        private TrueFalseMixed navBarHomeField;

        private bool navBarHomeFieldSpecified;

        private string pathField;

        private FileGhostType typeField;

        private bool typeFieldSpecified;

        private FileLevelType levelField;

        private bool levelFieldSpecified;

        private string urlField;

        /// <remarks/>
        [XmlElement("AllUsersWebPart", typeof(WebPartDefinition))]
        [XmlElement("NavBarPage", typeof(NavBarPageDefinition))]
        [XmlElement("Property", typeof(PropertyValueAttributeDefinition))]
        [XmlElement("View", typeof(ViewWebPartDefinition))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE IgnoreIfAlreadyExists
        {
            get
            {
                return this.ignoreIfAlreadyExistsField;
            }
            set
            {
                this.ignoreIfAlreadyExistsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool IgnoreIfAlreadyExistsSpecified
        {
            get
            {
                return this.ignoreIfAlreadyExistsFieldSpecified;
            }
            set
            {
                this.ignoreIfAlreadyExistsFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TrueFalseMixed NavBarHome
        {
            get
            {
                return this.navBarHomeField;
            }
            set
            {
                this.navBarHomeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool NavBarHomeSpecified
        {
            get
            {
                return this.navBarHomeFieldSpecified;
            }
            set
            {
                this.navBarHomeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public FileGhostType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TypeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public FileLevelType Level
        {
            get
            {
                return this.levelField;
            }
            set
            {
                this.levelField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool LevelSpecified
        {
            get
            {
                return this.levelFieldSpecified;
            }
            set
            {
                this.levelFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum FileGhostType
    {

        /// <remarks/>
        Ghostable,

        /// <remarks/>
        GhostableInLibrary,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum FileLevelType
    {

        /// <remarks/>
        Draft,
        Published
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ModuleDefinition
    {

        private FileDefinition[] fileField;

        private string[] textField;

        private string urlField;

        private TRUEFALSE rootWebOnlyField;

        private bool rootWebOnlyFieldSpecified;

        private string pathField;

        private string nameField;

        private int listField;

        private bool listFieldSpecified;

        private string includeFoldersField;

        private string setupPathField;

        /// <remarks/>
        [XmlElement("File")]
        public FileDefinition[] File
        {
            get
            {
                return this.fileField;
            }
            set
            {
                this.fileField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE RootWebOnly
        {
            get
            {
                return this.rootWebOnlyField;
            }
            set
            {
                this.rootWebOnlyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RootWebOnlySpecified
        {
            get
            {
                return this.rootWebOnlyFieldSpecified;
            }
            set
            {
                this.rootWebOnlyFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int List
        {
            get
            {
                return this.listField;
            }
            set
            {
                this.listField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ListSpecified
        {
            get
            {
                return this.listFieldSpecified;
            }
            set
            {
                this.listFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string IncludeFolders
        {
            get
            {
                return this.includeFoldersField;
            }
            set
            {
                this.includeFoldersField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string SetupPath
        {
            get
            {
                return this.setupPathField;
            }
            set
            {
                this.setupPathField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ListInstance
    {

        private DataDefinition dataField;

        private string urlField;

        private string quickLaunchUrlField;

        private int typeField;

        private bool typeFieldSpecified;

        private string titleField;

        private string featureIdField;

        private string emailAliasField;

        private string descriptionField;

        private TRUEFALSE versioningEnabledField;

        private bool versioningEnabledFieldSpecified;

        private TRUEFALSE enableMinorVersionsField;

        private bool enableMinorVersionsFieldSpecified;

        private TRUEFALSE enableContentTypesField;

        private bool enableContentTypesFieldSpecified;

        private TRUEFALSE forceCheckoutField;

        private bool forceCheckoutFieldSpecified;

        private TRUEFALSE rootWebOnlyField;

        private bool rootWebOnlyFieldSpecified;

        /// <remarks/>
        public DataDefinition Data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string QuickLaunchUrl
        {
            get
            {
                return this.quickLaunchUrlField;
            }
            set
            {
                this.quickLaunchUrlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TypeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string FeatureId
        {
            get
            {
                return this.featureIdField;
            }
            set
            {
                this.featureIdField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string EmailAlias
        {
            get
            {
                return this.emailAliasField;
            }
            set
            {
                this.emailAliasField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE VersioningEnabled
        {
            get
            {
                return this.versioningEnabledField;
            }
            set
            {
                this.versioningEnabledField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool VersioningEnabledSpecified
        {
            get
            {
                return this.versioningEnabledFieldSpecified;
            }
            set
            {
                this.versioningEnabledFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE EnableMinorVersions
        {
            get
            {
                return this.enableMinorVersionsField;
            }
            set
            {
                this.enableMinorVersionsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool EnableMinorVersionsSpecified
        {
            get
            {
                return this.enableMinorVersionsFieldSpecified;
            }
            set
            {
                this.enableMinorVersionsFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE EnableContentTypes
        {
            get
            {
                return this.enableContentTypesField;
            }
            set
            {
                this.enableContentTypesField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool EnableContentTypesSpecified
        {
            get
            {
                return this.enableContentTypesFieldSpecified;
            }
            set
            {
                this.enableContentTypesFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ForceCheckout
        {
            get
            {
                return this.forceCheckoutField;
            }
            set
            {
                this.forceCheckoutField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ForceCheckoutSpecified
        {
            get
            {
                return this.forceCheckoutFieldSpecified;
            }
            set
            {
                this.forceCheckoutFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE RootWebOnly
        {
            get
            {
                return this.rootWebOnlyField;
            }
            set
            {
                this.rootWebOnlyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RootWebOnlySpecified
        {
            get
            {
                return this.rootWebOnlyFieldSpecified;
            }
            set
            {
                this.rootWebOnlyFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ExecuteUrlDefinition
    {

        private string urlField;

        /// <remarks/>
        [XmlAttribute()]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ConfigurationDefinition
    {

        private ExecuteUrlDefinition executeUrlField;

        private ListInstance[] listsField;

        private ModuleDefinition[] modulesField;

        private FeatureTemplateReference[] webFeaturesField;

        private FeatureTemplateReference[] siteFeaturesField;

        private string descriptionField;

        private TRUEFALSE hiddenField;

        private bool hiddenFieldSpecified;

        private int idField;

        private bool idFieldSpecified;

        private string imageUrlField;

        private string nameField;

        private string titleField;

        private string typeField;

        private string masterUrlField;

        private string customMasterUrlField;

        /// <remarks/>
        public ExecuteUrlDefinition ExecuteUrl
        {
            get
            {
                return this.executeUrlField;
            }
            set
            {
                this.executeUrlField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("List", IsNullable = false)]
        public ListInstance[] Lists
        {
            get
            {
                return this.listsField;
            }
            set
            {
                this.listsField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("Module", IsNullable = false)]
        public ModuleDefinition[] Modules
        {
            get
            {
                return this.modulesField;
            }
            set
            {
                this.modulesField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("Feature", IsNullable = false)]
        public FeatureTemplateReference[] WebFeatures
        {
            get
            {
                return this.webFeaturesField;
            }
            set
            {
                this.webFeaturesField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("Feature", IsNullable = false)]
        public FeatureTemplateReference[] SiteFeatures
        {
            get
            {
                return this.siteFeaturesField;
            }
            set
            {
                this.siteFeaturesField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Hidden
        {
            get
            {
                return this.hiddenField;
            }
            set
            {
                this.hiddenField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HiddenSpecified
        {
            get
            {
                return this.hiddenFieldSpecified;
            }
            set
            {
                this.hiddenFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool IDSpecified
        {
            get
            {
                return this.idFieldSpecified;
            }
            set
            {
                this.idFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ImageUrl
        {
            get
            {
                return this.imageUrlField;
            }
            set
            {
                this.imageUrlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string MasterUrl
        {
            get
            {
                return this.masterUrlField;
            }
            set
            {
                this.masterUrlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string CustomMasterUrl
        {
            get
            {
                return this.customMasterUrlField;
            }
            set
            {
                this.customMasterUrlField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class BaseTypeDefinition
    {

        private ListMetaDataDefinition[] metaDataField;

        private string imageField;

        private string titleField;

        private int typeField;

        private bool typeFieldSpecified;

        /// <remarks/>
        [XmlElement("MetaData")]
        public ListMetaDataDefinition[] MetaData
        {
            get
            {
                return this.metaDataField;
            }
            set
            {
                this.metaDataField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Image
        {
            get
            {
                return this.imageField;
            }
            set
            {
                this.imageField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TypeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
            }
        }
    }

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/")]
    //public enum BaseType {

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlEnumAttribute("0")]
    //    Item0,

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlEnumAttribute("1")]
    //    Item1,

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlEnumAttribute("5")]
    //    Item5,

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlEnumAttribute("3")]
    //    Item3,

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlEnumAttribute("4")]
    //    Item4,
    //}

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class DocumentTemplateFileDefinition
    {

        private string targetNameField;

        private string nameField;

        private TRUEFALSE defaultField;

        private bool defaultFieldSpecified;

        /// <remarks/>
        [XmlAttribute()]
        public string TargetName
        {
            get
            {
                return this.targetNameField;
            }
            set
            {
                this.targetNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DefaultSpecified
        {
            get
            {
                return this.defaultFieldSpecified;
            }
            set
            {
                this.defaultFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class DocumentTemplateDefinition
    {

        private DocumentTemplateFileDefinition[] documentTemplateFilesField;

        private string displayNameField;

        private TRUEFALSE xMLFormField;

        private bool xMLFormFieldSpecified;

        private int typeField;

        private bool typeFieldSpecified;

        private string pathField;

        private string nameField;

        private string descriptionField;

        private TRUEFALSE defaultField;

        private bool defaultFieldSpecified;

        /// <remarks/>
        [XmlArrayItem("DocumentTemplateFile", IsNullable = false)]
        public DocumentTemplateFileDefinition[] DocumentTemplateFiles
        {
            get
            {
                return this.documentTemplateFilesField;
            }
            set
            {
                this.documentTemplateFilesField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE XMLForm
        {
            get
            {
                return this.xMLFormField;
            }
            set
            {
                this.xMLFormField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool XMLFormSpecified
        {
            get
            {
                return this.xMLFormFieldSpecified;
            }
            set
            {
                this.xMLFormFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TypeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DefaultSpecified
        {
            get
            {
                return this.defaultFieldSpecified;
            }
            set
            {
                this.defaultFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ListTemplateDefinition
    {

        private TRUEFALSE uniqueField;

        private bool uniqueFieldSpecified;

        private int typeField;

        private bool typeFieldSpecified;

        private string securityBitsField;

        private TRUEFALSE rootWebOnlyField;

        private bool rootWebOnlyFieldSpecified;

        private TRUEFALSE catalogField;

        private bool catalogFieldSpecified;

        private TRUEFALSE defaultField;

        private bool defaultFieldSpecified;

        private string displayNameField;

        private string descriptionField;

        private int documentTemplateField;

        private bool documentTemplateFieldSpecified;

        private TRUEFALSE dontSaveInTemplateField;

        private bool dontSaveInTemplateFieldSpecified;

        private TRUEFALSE allowDeletionField;

        private bool allowDeletionFieldSpecified;

        private TRUEFALSE disableAttachmentsField;

        private bool disableAttachmentsFieldSpecified;

        private TrueFalseMixed enableModerationField;

        private bool enableModerationFieldSpecified;

        private TRUEFALSE hiddenField;

        private bool hiddenFieldSpecified;

        private TRUEFALSE hiddenListField;

        private bool hiddenListFieldSpecified;

        private string imageField;

        private TRUEFALSE mustSaveRootFilesField;

        private bool mustSaveRootFilesFieldSpecified;

        private string nameField;

        private TRUEFALSE onQuickLaunchField;

        private bool onQuickLaunchFieldSpecified;

        private TRUEFALSE cacheSchemaField;

        private bool cacheSchemaFieldSpecified;

        private TRUEFALSE noCrawlField;

        private bool noCrawlFieldSpecified;

        private TRUEFALSE allowEveryoneViewItemsField;

        private bool allowEveryoneViewItemsFieldSpecified;

        private TRUEFALSE alwaysIncludeContentField;

        private bool alwaysIncludeContentFieldSpecified;

        private string pathField;

        private string syncTypeField;

        private string setupPathField;

        private int baseTypeField;

        private bool baseTypeFieldSpecified;

        private string editPageField;

        private string featureIdField;

        private TRUEFALSE folderCreationField;

        private bool folderCreationFieldSpecified;

        private TRUEFALSE multipleTypesField;

        private bool multipleTypesFieldSpecified;

        private TRUEFALSE disallowContentTypesField;

        private bool disallowContentTypesFieldSpecified;

        private string newPageField;

        private long sequenceField;

        private bool sequenceFieldSpecified;

        private TRUEFALSE versioningEnabledField;

        private bool versioningEnabledFieldSpecified;

        private ListTemplateCategoryType categoryField;

        private bool categoryFieldSpecified;

        private TRUEFALSE useRootFolderForNavigationField;

        private bool useRootFolderForNavigationFieldSpecified;

        private string[] textField;

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Unique
        {
            get
            {
                return this.uniqueField;
            }
            set
            {
                this.uniqueField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool UniqueSpecified
        {
            get
            {
                return this.uniqueFieldSpecified;
            }
            set
            {
                this.uniqueFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TypeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string SecurityBits
        {
            get
            {
                return this.securityBitsField;
            }
            set
            {
                this.securityBitsField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE RootWebOnly
        {
            get
            {
                return this.rootWebOnlyField;
            }
            set
            {
                this.rootWebOnlyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RootWebOnlySpecified
        {
            get
            {
                return this.rootWebOnlyFieldSpecified;
            }
            set
            {
                this.rootWebOnlyFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Catalog
        {
            get
            {
                return this.catalogField;
            }
            set
            {
                this.catalogField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool CatalogSpecified
        {
            get
            {
                return this.catalogFieldSpecified;
            }
            set
            {
                this.catalogFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DefaultSpecified
        {
            get
            {
                return this.defaultFieldSpecified;
            }
            set
            {
                this.defaultFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int DocumentTemplate
        {
            get
            {
                return this.documentTemplateField;
            }
            set
            {
                this.documentTemplateField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DocumentTemplateSpecified
        {
            get
            {
                return this.documentTemplateFieldSpecified;
            }
            set
            {
                this.documentTemplateFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE DontSaveInTemplate
        {
            get
            {
                return this.dontSaveInTemplateField;
            }
            set
            {
                this.dontSaveInTemplateField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DontSaveInTemplateSpecified
        {
            get
            {
                return this.dontSaveInTemplateFieldSpecified;
            }
            set
            {
                this.dontSaveInTemplateFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE AllowDeletion
        {
            get
            {
                return this.allowDeletionField;
            }
            set
            {
                this.allowDeletionField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AllowDeletionSpecified
        {
            get
            {
                return this.allowDeletionFieldSpecified;
            }
            set
            {
                this.allowDeletionFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE DisableAttachments
        {
            get
            {
                return this.disableAttachmentsField;
            }
            set
            {
                this.disableAttachmentsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DisableAttachmentsSpecified
        {
            get
            {
                return this.disableAttachmentsFieldSpecified;
            }
            set
            {
                this.disableAttachmentsFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TrueFalseMixed EnableModeration
        {
            get
            {
                return this.enableModerationField;
            }
            set
            {
                this.enableModerationField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool EnableModerationSpecified
        {
            get
            {
                return this.enableModerationFieldSpecified;
            }
            set
            {
                this.enableModerationFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Hidden
        {
            get
            {
                return this.hiddenField;
            }
            set
            {
                this.hiddenField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HiddenSpecified
        {
            get
            {
                return this.hiddenFieldSpecified;
            }
            set
            {
                this.hiddenFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE HiddenList
        {
            get
            {
                return this.hiddenListField;
            }
            set
            {
                this.hiddenListField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool HiddenListSpecified
        {
            get
            {
                return this.hiddenListFieldSpecified;
            }
            set
            {
                this.hiddenListFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Image
        {
            get
            {
                return this.imageField;
            }
            set
            {
                this.imageField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE MustSaveRootFiles
        {
            get
            {
                return this.mustSaveRootFilesField;
            }
            set
            {
                this.mustSaveRootFilesField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool MustSaveRootFilesSpecified
        {
            get
            {
                return this.mustSaveRootFilesFieldSpecified;
            }
            set
            {
                this.mustSaveRootFilesFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE OnQuickLaunch
        {
            get
            {
                return this.onQuickLaunchField;
            }
            set
            {
                this.onQuickLaunchField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool OnQuickLaunchSpecified
        {
            get
            {
                return this.onQuickLaunchFieldSpecified;
            }
            set
            {
                this.onQuickLaunchFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE CacheSchema
        {
            get
            {
                return this.cacheSchemaField;
            }
            set
            {
                this.cacheSchemaField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool CacheSchemaSpecified
        {
            get
            {
                return this.cacheSchemaFieldSpecified;
            }
            set
            {
                this.cacheSchemaFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE NoCrawl
        {
            get
            {
                return this.noCrawlField;
            }
            set
            {
                this.noCrawlField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool NoCrawlSpecified
        {
            get
            {
                return this.noCrawlFieldSpecified;
            }
            set
            {
                this.noCrawlFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE AllowEveryoneViewItems
        {
            get
            {
                return this.allowEveryoneViewItemsField;
            }
            set
            {
                this.allowEveryoneViewItemsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AllowEveryoneViewItemsSpecified
        {
            get
            {
                return this.allowEveryoneViewItemsFieldSpecified;
            }
            set
            {
                this.allowEveryoneViewItemsFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE AlwaysIncludeContent
        {
            get
            {
                return this.alwaysIncludeContentField;
            }
            set
            {
                this.alwaysIncludeContentField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AlwaysIncludeContentSpecified
        {
            get
            {
                return this.alwaysIncludeContentFieldSpecified;
            }
            set
            {
                this.alwaysIncludeContentFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string SyncType
        {
            get
            {
                return this.syncTypeField;
            }
            set
            {
                this.syncTypeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string SetupPath
        {
            get
            {
                return this.setupPathField;
            }
            set
            {
                this.setupPathField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int BaseType
        {
            get
            {
                return this.baseTypeField;
            }
            set
            {
                this.baseTypeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool BaseTypeSpecified
        {
            get
            {
                return this.baseTypeFieldSpecified;
            }
            set
            {
                this.baseTypeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string EditPage
        {
            get
            {
                return this.editPageField;
            }
            set
            {
                this.editPageField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string FeatureId
        {
            get
            {
                return this.featureIdField;
            }
            set
            {
                this.featureIdField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE FolderCreation
        {
            get
            {
                return this.folderCreationField;
            }
            set
            {
                this.folderCreationField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FolderCreationSpecified
        {
            get
            {
                return this.folderCreationFieldSpecified;
            }
            set
            {
                this.folderCreationFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE MultipleTypes
        {
            get
            {
                return this.multipleTypesField;
            }
            set
            {
                this.multipleTypesField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool MultipleTypesSpecified
        {
            get
            {
                return this.multipleTypesFieldSpecified;
            }
            set
            {
                this.multipleTypesFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE DisallowContentTypes
        {
            get
            {
                return this.disallowContentTypesField;
            }
            set
            {
                this.disallowContentTypesField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DisallowContentTypesSpecified
        {
            get
            {
                return this.disallowContentTypesFieldSpecified;
            }
            set
            {
                this.disallowContentTypesFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string NewPage
        {
            get
            {
                return this.newPageField;
            }
            set
            {
                this.newPageField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public long Sequence
        {
            get
            {
                return this.sequenceField;
            }
            set
            {
                this.sequenceField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool SequenceSpecified
        {
            get
            {
                return this.sequenceFieldSpecified;
            }
            set
            {
                this.sequenceFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE VersioningEnabled
        {
            get
            {
                return this.versioningEnabledField;
            }
            set
            {
                this.versioningEnabledField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool VersioningEnabledSpecified
        {
            get
            {
                return this.versioningEnabledFieldSpecified;
            }
            set
            {
                this.versioningEnabledFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public ListTemplateCategoryType Category
        {
            get
            {
                return this.categoryField;
            }
            set
            {
                this.categoryField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool CategorySpecified
        {
            get
            {
                return this.categoryFieldSpecified;
            }
            set
            {
                this.categoryFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE UseRootFolderForNavigation
        {
            get
            {
                return this.useRootFolderForNavigationField;
            }
            set
            {
                this.useRootFolderForNavigationField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool UseRootFolderForNavigationSpecified
        {
            get
            {
                return this.useRootFolderForNavigationFieldSpecified;
            }
            set
            {
                this.useRootFolderForNavigationFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum ListTemplateCategoryType
    {

        /// <remarks/>
        Libraries,

        /// <remarks/>
        Communications,

        /// <remarks/>
        Tracking,

        /// <remarks/>
        [XmlEnum("Custom Lists")]
        CustomLists,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class ListTemplateDefinitions
    {

        private ListTemplateDefinition[] listTemplateField;

        private string[] textField;

        /// <remarks/>
        [XmlElement("ListTemplate")]
        public ListTemplateDefinition[] ListTemplate
        {
            get
            {
                return this.listTemplateField;
            }
            set
            {
                this.listTemplateField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class NavBarLinkDefinition
    {

        private string[] textField;

        private string nameField;

        private string urlField;

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class NavBarDefinition
    {

        private NavBarLinkDefinition[] navBarLinkField;

        private string[] textField;

        private string suffixField;

        private string separatorField;

        private string prefixField;

        private string nameField;

        private string urlField;

        private string bodyField;

        private int idField;

        private bool idFieldSpecified;

        /// <remarks/>
        [XmlElement("NavBarLink")]
        public NavBarLinkDefinition[] NavBarLink
        {
            get
            {
                return this.navBarLinkField;
            }
            set
            {
                this.navBarLinkField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Suffix
        {
            get
            {
                return this.suffixField;
            }
            set
            {
                this.suffixField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Separator
        {
            get
            {
                return this.separatorField;
            }
            set
            {
                this.separatorField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Prefix
        {
            get
            {
                return this.prefixField;
            }
            set
            {
                this.prefixField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool IDSpecified
        {
            get
            {
                return this.idFieldSpecified;
            }
            set
            {
                this.idFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class SetListDefinition
    {

        private XmlElement anyField;

        private RequestParameter scopeField;

        private bool scopeFieldSpecified;

        private TRUEFALSE preserveContextField;

        private bool preserveContextFieldSpecified;

        private string nameField;

        /// <remarks/>
        [XmlAnyElement()]
        public XmlElement Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public RequestParameter Scope
        {
            get
            {
                return this.scopeField;
            }
            set
            {
                this.scopeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ScopeSpecified
        {
            get
            {
                return this.scopeFieldSpecified;
            }
            set
            {
                this.scopeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE PreserveContext
        {
            get
            {
                return this.preserveContextField;
            }
            set
            {
                this.preserveContextField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool PreserveContextSpecified
        {
            get
            {
                return this.preserveContextFieldSpecified;
            }
            set
            {
                this.preserveContextFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class MethodDefinition
    {

        private SetVarDefinition setVarField;

        private SetListDefinition setListField;

        private string idField;

        private UpdateMethod cmdField;

        private bool cmdFieldSpecified;

        /// <remarks/>
        public SetVarDefinition SetVar
        {
            get
            {
                return this.setVarField;
            }
            set
            {
                this.setVarField = value;
            }
        }

        /// <remarks/>
        public SetListDefinition SetList
        {
            get
            {
                return this.setListField;
            }
            set
            {
                this.setListField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public UpdateMethod Cmd
        {
            get
            {
                return this.cmdField;
            }
            set
            {
                this.cmdField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool CmdSpecified
        {
            get
            {
                return this.cmdFieldSpecified;
            }
            set
            {
                this.cmdFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum UpdateMethod
    {

        /// <remarks/>
        Delete,

        /// <remarks/>
        New,

        /// <remarks/>
        Update,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class BatchDefinition
    {

        private MethodDefinition methodField;

        private ErrorHandling onErrorField;

        private bool onErrorFieldSpecified;

        private int listVersionField;

        private bool listVersionFieldSpecified;

        private string versionField;

        private string viewNameField;

        /// <remarks/>
        public MethodDefinition Method
        {
            get
            {
                return this.methodField;
            }
            set
            {
                this.methodField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public ErrorHandling OnError
        {
            get
            {
                return this.onErrorField;
            }
            set
            {
                this.onErrorField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool OnErrorSpecified
        {
            get
            {
                return this.onErrorFieldSpecified;
            }
            set
            {
                this.onErrorFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int ListVersion
        {
            get
            {
                return this.listVersionField;
            }
            set
            {
                this.listVersionField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ListVersionSpecified
        {
            get
            {
                return this.listVersionFieldSpecified;
            }
            set
            {
                this.listVersionFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ViewName
        {
            get
            {
                return this.viewNameField;
            }
            set
            {
                this.viewNameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum ErrorHandling
    {

        /// <remarks/>
        Return,

        /// <remarks/>
        Continue,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FieldRefViewDefinition
    {

        private TRUEFALSE ascendingField;

        private bool ascendingFieldSpecified;

        private ReferenceType typeField;

        private bool typeFieldSpecified;

        private TRUEFALSE textOnlyField;

        private bool textOnlyFieldSpecified;

        private string createURLField;

        private string displayNameField;

        private TRUEFALSE explicitField;

        private bool explicitFieldSpecified;

        private string keyField;

        private string nameField;

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Ascending
        {
            get
            {
                return this.ascendingField;
            }
            set
            {
                this.ascendingField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool AscendingSpecified
        {
            get
            {
                return this.ascendingFieldSpecified;
            }
            set
            {
                this.ascendingFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public ReferenceType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TypeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE TextOnly
        {
            get
            {
                return this.textOnlyField;
            }
            set
            {
                this.textOnlyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TextOnlySpecified
        {
            get
            {
                return this.textOnlyFieldSpecified;
            }
            set
            {
                this.textOnlyFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string CreateURL
        {
            get
            {
                return this.createURLField;
            }
            set
            {
                this.createURLField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE Explicit
        {
            get
            {
                return this.explicitField;
            }
            set
            {
                this.explicitField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ExplicitSpecified
        {
            get
            {
                return this.explicitFieldSpecified;
            }
            set
            {
                this.explicitFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class HtmlDefinition
    {

        private XmlElement anyField;

        private string idField;

        /// <remarks/>
        [XmlAnyElement()]
        public XmlElement Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class QueryListProperty
    {

        private TRUEFALSE uRLEncodeAsURLField;

        private bool uRLEncodeAsURLFieldSpecified;

        private string selectField;

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE URLEncodeAsURL
        {
            get
            {
                return this.uRLEncodeAsURLField;
            }
            set
            {
                this.uRLEncodeAsURLField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool URLEncodeAsURLSpecified
        {
            get
            {
                return this.uRLEncodeAsURLFieldSpecified;
            }
            set
            {
                this.uRLEncodeAsURLFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Select
        {
            get
            {
                return this.selectField;
            }
            set
            {
                this.selectField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class EmptyQueryDefinition
    {
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class FeatureActivationDependencyDefinition
    {

        private string featureIdField;

        /// <remarks/>
        [XmlAttribute()]
        public string FeatureId
        {
            get
            {
                return this.featureIdField;
            }
            set
            {
                this.featureIdField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/", IncludeInSchema = false)]
    public enum ItemsChoiceType
    {

        /// <remarks/>
        ElementFile,

        /// <remarks/>
        ElementManifest,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum FeatureScope
    {

        /// <remarks/>
        Farm,

        /// <remarks/>
        WebApplication,

        /// <remarks/>
        Site,

        /// <remarks/>
        Web,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class WebTemplate
    {

        private string adjustHijriDaysField;

        private string alternateCssUrlField;

        private string alternateHeaderField;

        private string baseTemplateIDField;

        private string baseTemplateNameField;

        private string baseConfigurationIDField;

        private string calendarTypeField;

        private string collationField;

        private string containsDefaultListsField;

        private string customizedCssFilesField;

        private string customJSUrlField;

        private string excludeFromOfflineClientField;

        private string localeField;

        private string nameField;

        private string parserEnabledField;

        private string portalNameField;

        private string portalUrlField;

        private string presenceEnabledField;

        private string productVersionField;

        private string quickLaunchEnabledField;

        private string hideSiteContentsLinkField;

        private string enableMinimalDownloadField;

        private string subwebField;

        private string syndicationEnabledField;

        private string time24Field;

        private string timeZoneField;

        private string titleField;

        private string treeViewEnabledField;

        private string uIVersionConfigurationEnabledField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AdjustHijriDays
        {
            get
            {
                return this.adjustHijriDaysField;
            }
            set
            {
                this.adjustHijriDaysField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AlternateCssUrl
        {
            get
            {
                return this.alternateCssUrlField;
            }
            set
            {
                this.alternateCssUrlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AlternateHeader
        {
            get
            {
                return this.alternateHeaderField;
            }
            set
            {
                this.alternateHeaderField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BaseTemplateID
        {
            get
            {
                return this.baseTemplateIDField;
            }
            set
            {
                this.baseTemplateIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BaseTemplateName
        {
            get
            {
                return this.baseTemplateNameField;
            }
            set
            {
                this.baseTemplateNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BaseConfigurationID
        {
            get
            {
                return this.baseConfigurationIDField;
            }
            set
            {
                this.baseConfigurationIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CalendarType
        {
            get
            {
                return this.calendarTypeField;
            }
            set
            {
                this.calendarTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Collation
        {
            get
            {
                return this.collationField;
            }
            set
            {
                this.collationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ContainsDefaultLists
        {
            get
            {
                return this.containsDefaultListsField;
            }
            set
            {
                this.containsDefaultListsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CustomizedCssFiles
        {
            get
            {
                return this.customizedCssFilesField;
            }
            set
            {
                this.customizedCssFilesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CustomJSUrl
        {
            get
            {
                return this.customJSUrlField;
            }
            set
            {
                this.customJSUrlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ExcludeFromOfflineClient
        {
            get
            {
                return this.excludeFromOfflineClientField;
            }
            set
            {
                this.excludeFromOfflineClientField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Locale
        {
            get
            {
                return this.localeField;
            }
            set
            {
                this.localeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ParserEnabled
        {
            get
            {
                return this.parserEnabledField;
            }
            set
            {
                this.parserEnabledField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PortalName
        {
            get
            {
                return this.portalNameField;
            }
            set
            {
                this.portalNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PortalUrl
        {
            get
            {
                return this.portalUrlField;
            }
            set
            {
                this.portalUrlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PresenceEnabled
        {
            get
            {
                return this.presenceEnabledField;
            }
            set
            {
                this.presenceEnabledField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ProductVersion
        {
            get
            {
                return this.productVersionField;
            }
            set
            {
                this.productVersionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string QuickLaunchEnabled
        {
            get
            {
                return this.quickLaunchEnabledField;
            }
            set
            {
                this.quickLaunchEnabledField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string HideSiteContentsLink
        {
            get
            {
                return this.hideSiteContentsLinkField;
            }
            set
            {
                this.hideSiteContentsLinkField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string EnableMinimalDownload
        {
            get
            {
                return this.enableMinimalDownloadField;
            }
            set
            {
                this.enableMinimalDownloadField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Subweb
        {
            get
            {
                return this.subwebField;
            }
            set
            {
                this.subwebField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SyndicationEnabled
        {
            get
            {
                return this.syndicationEnabledField;
            }
            set
            {
                this.syndicationEnabledField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Time24
        {
            get
            {
                return this.time24Field;
            }
            set
            {
                this.time24Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TimeZone
        {
            get
            {
                return this.timeZoneField;
            }
            set
            {
                this.timeZoneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TreeViewEnabled
        {
            get
            {
                return this.treeViewEnabledField;
            }
            set
            {
                this.treeViewEnabledField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string UIVersionConfigurationEnabled
        {
            get
            {
                return this.uIVersionConfigurationEnabledField;
            }
            set
            {
                this.uIVersionConfigurationEnabledField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class WorkflowActions
    {

        private WorkflowAction[] actionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Action")]
        public WorkflowAction[] Action
        {
            get
            {
                return this.actionField;
            }
            set
            {
                this.actionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public partial class WorkflowAction
    {

        private string nameField;

        private string sandboxedFunctionField;

        private string assemblyField;

        private string classNameField;

        private string functionNameField;

        private string appliesToField;

        private string categoryField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SandboxedFunction
        {
            get
            {
                return this.sandboxedFunctionField;
            }
            set
            {
                this.sandboxedFunctionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Assembly
        {
            get
            {
                return this.assemblyField;
            }
            set
            {
                this.assemblyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ClassName
        {
            get
            {
                return this.classNameField;
            }
            set
            {
                this.classNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string FunctionName
        {
            get
            {
                return this.functionNameField;
            }
            set
            {
                this.functionNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AppliesTo
        {
            get
            {
                return this.appliesToField;
            }
            set
            {
                this.appliesToField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Category
        {
            get
            {
                return this.categoryField;
            }
            set
            {
                this.categoryField = value;
            }
        }
    }


    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    [XmlRoot("Elements", Namespace = "http://schemas.microsoft.com/sharepoint/", IsNullable = false)]
    public partial class ElementDefinitionCollection
    {

        private object[] itemsField;

        private string idField;

        /// <remarks/>
        [XmlElement("ContentType", typeof(ContentTypeDefinition))]
        [XmlElement("ContentTypeBinding", typeof(ContentTypeBindingDefinition))]
        [XmlElement("Control", typeof(DelegateControlDefinition))]
        [XmlElement("CustomAction", typeof(CustomActionDefinition))]
        [XmlElement("CustomActionGroup", typeof(CustomActionGroupDefinition))]
        [XmlElement("DocumentConverter", typeof(DocumentConverterDefinition))]
        [XmlElement("FeatureSiteTemplateAssociation", typeof(FeatureSiteTemplateAssociationDefinition))]
        [XmlElement("Field", typeof(SharedFieldDefinition))]
        [XmlElement("GroupMigrator", typeof(GroupMigratorDefinition))]
        [XmlElement("HideCustomAction", typeof(HideCustomActionDefinition))]
        [XmlElement("ListInstance", typeof(ListInstanceDefinition))]
        [XmlElement("ListTemplate", typeof(ListTemplateDefinition))]
        [XmlElement("Module", typeof(ModuleDefinition))]
        [XmlElement("Receivers", typeof(ReceiverDefinitionCollection))]
        [XmlElement("UserMigrator", typeof(UserMigratorDefinition))]
        [XmlElement("Workflow", typeof(WorkflowDefinition))]
        [XmlElement("WebTemplate", typeof(WebTemplate))]
        [XmlElement("WorkflowActions", typeof(WorkflowActions))]

        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    [XmlRoot(Namespace = "http://schemas.microsoft.com/sharepoint/", IsNullable = false)]
    public partial class Solution
    {

        private FeatureManifestReference[] featureManifestsField;

        private SiteDefinitionManifestFileReference[] siteDefinitionManifestsField;

        private AssemblyFileReference[] assembliesField;

        private TemplateFileReference[] templateFilesField;

        private RootFileReference[] rootFilesField;

        private ApplicationResourceFileDefinition[] applicationResourceFilesField;

        private ResourceDefinition[] resourcesField;

        private DwpFileDefinition[] dwpFilesField;

        private PolicyItemDefinition[] codeAccessSecurityField;

        private string solutionIdField;

        private DeploymentServerTypeAttr deploymentServerTypeField;

        private bool deploymentServerTypeFieldSpecified;

        private TRUEFALSE resetWebServerField;

        private bool resetWebServerFieldSpecified;

        private ResetWebServerModeOnUpgradeAttr resetWebServerModeOnUpgradeField;

        private bool resetWebServerModeOnUpgradeFieldSpecified;

        /// <remarks/>
        [XmlArrayItem("FeatureManifest", IsNullable = false)]
        public FeatureManifestReference[] FeatureManifests
        {
            get
            {
                return this.featureManifestsField;
            }
            set
            {
                this.featureManifestsField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("SiteDefinitionManifest", IsNullable = false)]
        public SiteDefinitionManifestFileReference[] SiteDefinitionManifests
        {
            get
            {
                return this.siteDefinitionManifestsField;
            }
            set
            {
                this.siteDefinitionManifestsField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("Assembly", IsNullable = false)]
        public AssemblyFileReference[] Assemblies
        {
            get
            {
                return this.assembliesField;
            }
            set
            {
                this.assembliesField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("TemplateFile", IsNullable = false)]
        public TemplateFileReference[] TemplateFiles
        {
            get
            {
                return this.templateFilesField;
            }
            set
            {
                this.templateFilesField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("RootFile", IsNullable = false)]
        public RootFileReference[] RootFiles
        {
            get
            {
                return this.rootFilesField;
            }
            set
            {
                this.rootFilesField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("ApplicationResourceFile", IsNullable = false)]
        public ApplicationResourceFileDefinition[] ApplicationResourceFiles
        {
            get
            {
                return this.applicationResourceFilesField;
            }
            set
            {
                this.applicationResourceFilesField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("Resource", IsNullable = false)]
        public ResourceDefinition[] Resources
        {
            get
            {
                return this.resourcesField;
            }
            set
            {
                this.resourcesField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("DwpFile", IsNullable = false)]
        public DwpFileDefinition[] DwpFiles
        {
            get
            {
                return this.dwpFilesField;
            }
            set
            {
                this.dwpFilesField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("PolicyItem", IsNullable = false)]
        public PolicyItemDefinition[] CodeAccessSecurity
        {
            get
            {
                return this.codeAccessSecurityField;
            }
            set
            {
                this.codeAccessSecurityField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string SolutionId
        {
            get
            {
                return this.solutionIdField;
            }
            set
            {
                this.solutionIdField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public DeploymentServerTypeAttr DeploymentServerType
        {
            get
            {
                return this.deploymentServerTypeField;
            }
            set
            {
                this.deploymentServerTypeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DeploymentServerTypeSpecified
        {
            get
            {
                return this.deploymentServerTypeFieldSpecified;
            }
            set
            {
                this.deploymentServerTypeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ResetWebServer
        {
            get
            {
                return this.resetWebServerField;
            }
            set
            {
                this.resetWebServerField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ResetWebServerSpecified
        {
            get
            {
                return this.resetWebServerFieldSpecified;
            }
            set
            {
                this.resetWebServerFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public ResetWebServerModeOnUpgradeAttr ResetWebServerModeOnUpgrade
        {
            get
            {
                return this.resetWebServerModeOnUpgradeField;
            }
            set
            {
                this.resetWebServerModeOnUpgradeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ResetWebServerModeOnUpgradeSpecified
        {
            get
            {
                return this.resetWebServerModeOnUpgradeFieldSpecified;
            }
            set
            {
                this.resetWebServerModeOnUpgradeFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum DeploymentServerTypeAttr
    {

        /// <remarks/>
        ApplicationServer,

        /// <remarks/>
        WebFrontEnd,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    public enum ResetWebServerModeOnUpgradeAttr
    {

        /// <remarks/>
        Recycle,

        /// <remarks/>
        StartStop,
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    [XmlRoot("Project", Namespace = "http://schemas.microsoft.com/sharepoint/", IsNullable = false)]
    public partial class SiteDefinitionInfo
    {

        private NavBarDefinition[] navBarsField;

        private ListTemplateDefinitions listTemplatesField;

        private DocumentTemplateDefinition[] documentTemplatesField;

        private BaseTypeDefinition[] baseTypesField;

        private ConfigurationDefinition[] configurationsField;

        private ModuleDefinition[] modulesField;

        private ComponentsDefinition componentsField;

        private ServerEmailFooterDefinition serverEmailFooterField;

        private string titleField;

        private string listDirField;

        private string alternateURLField;

        private string alternateCSSField;

        private int revisionField;

        private bool revisionFieldSpecified;

        private string disableWebDesignFeaturesField;

        private XmlAttribute[] anyAttrField;

        /// <remarks/>
        [XmlArrayItem("NavBar", IsNullable = false)]
        public NavBarDefinition[] NavBars
        {
            get
            {
                return this.navBarsField;
            }
            set
            {
                this.navBarsField = value;
            }
        }

        /// <remarks/>
        public ListTemplateDefinitions ListTemplates
        {
            get
            {
                return this.listTemplatesField;
            }
            set
            {
                this.listTemplatesField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("DocumentTemplate", IsNullable = false)]
        public DocumentTemplateDefinition[] DocumentTemplates
        {
            get
            {
                return this.documentTemplatesField;
            }
            set
            {
                this.documentTemplatesField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore] // .XmlArrayItemAttribute("BaseType", IsNullable=false)]
        public BaseTypeDefinition[] BaseTypes
        {
            get
            {
                return this.baseTypesField;
            }
            set
            {
                this.baseTypesField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("Configuration", IsNullable = false)]
        public ConfigurationDefinition[] Configurations
        {
            get
            {
                return this.configurationsField;
            }
            set
            {
                this.configurationsField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItem("Module", IsNullable = false)]
        public ModuleDefinition[] Modules
        {
            get
            {
                return this.modulesField;
            }
            set
            {
                this.modulesField = value;
            }
        }

        /// <remarks/>
        public ComponentsDefinition Components
        {
            get
            {
                return this.componentsField;
            }
            set
            {
                this.componentsField = value;
            }
        }

        /// <remarks/>
        public ServerEmailFooterDefinition ServerEmailFooter
        {
            get
            {
                return this.serverEmailFooterField;
            }
            set
            {
                this.serverEmailFooterField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string ListDir
        {
            get
            {
                return this.listDirField;
            }
            set
            {
                this.listDirField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string AlternateURL
        {
            get
            {
                return this.alternateURLField;
            }
            set
            {
                this.alternateURLField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string AlternateCSS
        {
            get
            {
                return this.alternateCSSField;
            }
            set
            {
                this.alternateCSSField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int Revision
        {
            get
            {
                return this.revisionField;
            }
            set
            {
                this.revisionField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RevisionSpecified
        {
            get
            {
                return this.revisionFieldSpecified;
            }
            set
            {
                this.revisionFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string DisableWebDesignFeatures
        {
            get
            {
                return this.disableWebDesignFeaturesField;
            }
            set
            {
                this.disableWebDesignFeaturesField = value;
            }
        }

        /// <remarks/>
        [XmlAnyAttribute()]
        public XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/")]
    [XmlRoot("List", Namespace = "http://schemas.microsoft.com/sharepoint/", IsNullable = false)]
    public partial class ListDefinition
    {

        private ListMetaDataDefinition metaDataField;

        private string idField;

        private string nameField;

        private string titleField;

        private string urlField;

        private string defaultField;

        private long webImageWidthField;

        private bool webImageWidthFieldSpecified;

        private TRUEFALSE enableThumbnailsField;

        private bool enableThumbnailsFieldSpecified;

        private TRUEFALSE enableContentTypesField;

        private bool enableContentTypesFieldSpecified;

        private TRUEFALSE folderCreationField;

        private bool folderCreationFieldSpecified;

        private TRUEFALSE disableAttachmentsField;

        private bool disableAttachmentsFieldSpecified;

        private string eventSinkAssemblyField;

        private string eventSinkClassField;

        private string eventSinkDataField;

        private TRUEFALSE orderedListField;

        private bool orderedListFieldSpecified;

        private TRUEFALSE privateListField;

        private bool privateListFieldSpecified;

        private string quickLaunchUrlField;

        private TRUEFALSE rootWebOnlyField;

        private bool rootWebOnlyFieldSpecified;

        private TRUEFALSE moderatedListField;

        private bool moderatedListFieldSpecified;

        private int draftVersionVisibilityField;

        private bool draftVersionVisibilityFieldSpecified;

        private int thumbnailSizeField;

        private bool thumbnailSizeFieldSpecified;

        private int defaultItemOpenField;

        private bool defaultItemOpenFieldSpecified;

        private TRUEFALSE versioningEnabledField;

        private bool versioningEnabledFieldSpecified;

        private TRUEFALSE enableMinorVersionsField;

        private bool enableMinorVersionsFieldSpecified;

        private TRUEFALSE moderationTypeField;

        private bool moderationTypeFieldSpecified;

        private int typeField;

        private bool typeFieldSpecified;

        private TRUEFALSE uRLEncodeField;

        private bool uRLEncodeFieldSpecified;

        private long webImageHeightField;

        private bool webImageHeightFieldSpecified;

        private int baseTypeField;

        private bool baseTypeFieldSpecified;

        private string directionField;

        /// <remarks/>
        public ListMetaDataDefinition MetaData
        {
            get
            {
                return this.metaDataField;
            }
            set
            {
                this.metaDataField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public long WebImageWidth
        {
            get
            {
                return this.webImageWidthField;
            }
            set
            {
                this.webImageWidthField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool WebImageWidthSpecified
        {
            get
            {
                return this.webImageWidthFieldSpecified;
            }
            set
            {
                this.webImageWidthFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE EnableThumbnails
        {
            get
            {
                return this.enableThumbnailsField;
            }
            set
            {
                this.enableThumbnailsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool EnableThumbnailsSpecified
        {
            get
            {
                return this.enableThumbnailsFieldSpecified;
            }
            set
            {
                this.enableThumbnailsFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE EnableContentTypes
        {
            get
            {
                return this.enableContentTypesField;
            }
            set
            {
                this.enableContentTypesField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool EnableContentTypesSpecified
        {
            get
            {
                return this.enableContentTypesFieldSpecified;
            }
            set
            {
                this.enableContentTypesFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE FolderCreation
        {
            get
            {
                return this.folderCreationField;
            }
            set
            {
                this.folderCreationField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool FolderCreationSpecified
        {
            get
            {
                return this.folderCreationFieldSpecified;
            }
            set
            {
                this.folderCreationFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE DisableAttachments
        {
            get
            {
                return this.disableAttachmentsField;
            }
            set
            {
                this.disableAttachmentsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DisableAttachmentsSpecified
        {
            get
            {
                return this.disableAttachmentsFieldSpecified;
            }
            set
            {
                this.disableAttachmentsFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string EventSinkAssembly
        {
            get
            {
                return this.eventSinkAssemblyField;
            }
            set
            {
                this.eventSinkAssemblyField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string EventSinkClass
        {
            get
            {
                return this.eventSinkClassField;
            }
            set
            {
                this.eventSinkClassField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string EventSinkData
        {
            get
            {
                return this.eventSinkDataField;
            }
            set
            {
                this.eventSinkDataField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE OrderedList
        {
            get
            {
                return this.orderedListField;
            }
            set
            {
                this.orderedListField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool OrderedListSpecified
        {
            get
            {
                return this.orderedListFieldSpecified;
            }
            set
            {
                this.orderedListFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE PrivateList
        {
            get
            {
                return this.privateListField;
            }
            set
            {
                this.privateListField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool PrivateListSpecified
        {
            get
            {
                return this.privateListFieldSpecified;
            }
            set
            {
                this.privateListFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string QuickLaunchUrl
        {
            get
            {
                return this.quickLaunchUrlField;
            }
            set
            {
                this.quickLaunchUrlField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE RootWebOnly
        {
            get
            {
                return this.rootWebOnlyField;
            }
            set
            {
                this.rootWebOnlyField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool RootWebOnlySpecified
        {
            get
            {
                return this.rootWebOnlyFieldSpecified;
            }
            set
            {
                this.rootWebOnlyFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ModeratedList
        {
            get
            {
                return this.moderatedListField;
            }
            set
            {
                this.moderatedListField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ModeratedListSpecified
        {
            get
            {
                return this.moderatedListFieldSpecified;
            }
            set
            {
                this.moderatedListFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int DraftVersionVisibility
        {
            get
            {
                return this.draftVersionVisibilityField;
            }
            set
            {
                this.draftVersionVisibilityField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DraftVersionVisibilitySpecified
        {
            get
            {
                return this.draftVersionVisibilityFieldSpecified;
            }
            set
            {
                this.draftVersionVisibilityFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int ThumbnailSize
        {
            get
            {
                return this.thumbnailSizeField;
            }
            set
            {
                this.thumbnailSizeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ThumbnailSizeSpecified
        {
            get
            {
                return this.thumbnailSizeFieldSpecified;
            }
            set
            {
                this.thumbnailSizeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int DefaultItemOpen
        {
            get
            {
                return this.defaultItemOpenField;
            }
            set
            {
                this.defaultItemOpenField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool DefaultItemOpenSpecified
        {
            get
            {
                return this.defaultItemOpenFieldSpecified;
            }
            set
            {
                this.defaultItemOpenFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE VersioningEnabled
        {
            get
            {
                return this.versioningEnabledField;
            }
            set
            {
                this.versioningEnabledField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool VersioningEnabledSpecified
        {
            get
            {
                return this.versioningEnabledFieldSpecified;
            }
            set
            {
                this.versioningEnabledFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE EnableMinorVersions
        {
            get
            {
                return this.enableMinorVersionsField;
            }
            set
            {
                this.enableMinorVersionsField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool EnableMinorVersionsSpecified
        {
            get
            {
                return this.enableMinorVersionsFieldSpecified;
            }
            set
            {
                this.enableMinorVersionsFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE ModerationType
        {
            get
            {
                return this.moderationTypeField;
            }
            set
            {
                this.moderationTypeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool ModerationTypeSpecified
        {
            get
            {
                return this.moderationTypeFieldSpecified;
            }
            set
            {
                this.moderationTypeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool TypeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public TRUEFALSE URLEncode
        {
            get
            {
                return this.uRLEncodeField;
            }
            set
            {
                this.uRLEncodeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool URLEncodeSpecified
        {
            get
            {
                return this.uRLEncodeFieldSpecified;
            }
            set
            {
                this.uRLEncodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public long WebImageHeight
        {
            get
            {
                return this.webImageHeightField;
            }
            set
            {
                this.webImageHeightField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool WebImageHeightSpecified
        {
            get
            {
                return this.webImageHeightFieldSpecified;
            }
            set
            {
                this.webImageHeightFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public int BaseType
        {
            get
            {
                return this.baseTypeField;
            }
            set
            {
                this.baseTypeField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool BaseTypeSpecified
        {
            get
            {
                return this.baseTypeFieldSpecified;
            }
            set
            {
                this.baseTypeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Direction
        {
            get
            {
                return this.directionField;
            }
            set
            {
                this.directionField = value;
            }
        }
    }
}
