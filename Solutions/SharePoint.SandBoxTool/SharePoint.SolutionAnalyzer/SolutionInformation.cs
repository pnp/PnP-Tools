using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

/// <summary>
/// Semi automatically generated classes that represent the scanned solution
/// </summary>
/// 
namespace SharePoint.SolutionAnalyzer
{

    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class SolutionInformation
    {

        private string name;

        private string deploymentStatus;

        private DateTime deploymentDate;

        private int classResourceCount;

        private int safeControlCount;

        private int featureReceiversCount;

        private int fileSize;

        private string fileDate;

        private bool infoPathSolution;

        private List<FeatureInfo> features;

        private List<AssemblyInfo> assemblies;

        private List<string> siteTemplates;
        private List<string> listTemplates;

        private List<WebPartInfo> webParts;

        private List<string> aspxPages;

        private List<string> xslFiles;

        private List<string> files;

        private List<string> cssFiles;

        private List<string> jSFiles;

        private string error;

        private List<string> timerJobs;

        private List<string> eventHandlers;

        private List<string> userControls;

        private List<string> webServices;

        private List<SiteDefinitionInformation> siteDefinitions;

        private List<CustomFieldType> customFieldTypes;

        private int masterpagesCount;

        private int pagelayoutsCount;

        private List<ActionDetail> actionDetails;

        public SolutionInformation()
        {
            this.assemblies = new List<AssemblyInfo>(50);
            this.webParts = new List<WebPartInfo>(50);

            this.aspxPages = new List<string>(100);
            this.CSSFiles = new List<string>(100);
            this.jSFiles = new List<string>(100);
            this.siteTemplates = new List<string>(100);
            this.listTemplates = new List<string>(100);

            this.siteDefinitions = new List<SiteDefinitionInformation>(50);
            this.customFieldTypes = new List<CustomFieldType>(50);
            this.features = new List<FeatureInfo>(100);// FeatureCollection();
            this.files = new List<string>(100);
            this.timerJobs = new List<string>(100);
            this.eventHandlers = new List<string>(100);

            this.userControls = new List<string>(100);
            this.webServices = new List<string>(100);
            this.xslFiles = new List<string>(5);

            this.actionDetails = new List<ActionDetail>(100);
        }

        /// <remarks/>
        [XmlArray("ActionDetails", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("ActionDetail", Form = XmlSchemaForm.Unqualified)]
        public List<ActionDetail> ActionDetails
        {
            get
            {
                return this.actionDetails;
            }
            set
            {
                this.actionDetails = value;
            }
        }
        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string DeploymentStatus
        {
            get
            {
                return this.deploymentStatus;
            }
            set
            {
                this.deploymentStatus = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public DateTime DeploymentDate
        {
            get
            {
                return this.deploymentDate;
            }
            set
            {
                this.deploymentDate = value;
            }
        }
        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string FileDate
        {
            get
            {
                return this.fileDate;
            }
            set
            {
                this.fileDate = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public int FileSize
        {
            get
            {
                return this.fileSize;
            }
            set
            {
                this.fileSize = value;
            }
        }

        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public bool InfoPathSolution
        {
            get
            {
                return this.infoPathSolution;
            }
            set
            {
                this.infoPathSolution = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("Features", Form = XmlSchemaForm.Unqualified)]
        public int FeaturesCount
        {
            get
            {
                return this.Features.Count;
            }
            set
            {
                int a = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("Assemblies", Form = XmlSchemaForm.Unqualified)]
        public int AssemblyCount
        {
            get
            {
                return this.Assemblies.Count;
            }
            set
            {
                int a = value;
            }
        }


        /// <remarks/>
        [XmlAttribute("WebParts", Form = XmlSchemaForm.Unqualified)]
        public int WebPartsCount
        {
            get
            {
                return this.WebParts.Count;
            }
            set
            {
                int a = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("Site Definitions", Form = XmlSchemaForm.Unqualified)]
        public int SiteDefinitionsCount
        {
            get
            {
                return this.SiteDefinitions.Count;
            }
            set
            {
                int a = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("SiteTemplates", Form = XmlSchemaForm.Unqualified)]
        public int SiteTemplatesCount
        {
            get
            {
                return this.SiteTemplates.Count;
            }
            set
            {
                int a = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("ListTemplates", Form = XmlSchemaForm.Unqualified)]
        public int ListTemplatesCount
        {
            get
            {
                return this.ListTemplates.Count;
            }
            set
            {
                int a = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("Masterpages", Form = XmlSchemaForm.Unqualified)]
        public int MasterpagesCount
        {
            get
            {
                return this.masterpagesCount;
            }
            set
            {
                this.masterpagesCount = value;
            }
        }


        /// <remarks/>
        [XmlAttribute("Pagelayouts", Form = XmlSchemaForm.Unqualified)]
        public int PagelayoutsCount
        {
            get
            {
                return this.pagelayoutsCount;
            }
            set
            {
                this.pagelayoutsCount = value;
            }
        }


        /// <remarks/>
        [XmlAttribute("AspxPages", Form = XmlSchemaForm.Unqualified)]
        public int AspxPagesCount
        {
            get
            {
                return this.AspxPages.Count;
            }
            set
            {
                int a = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("UserControls", Form = XmlSchemaForm.Unqualified)]
        public int UserControlsCount
        {
            get
            {
                return this.UserControls.Count;
            }
            set
            {
                int a = value;
            }
        }
        /// <remarks/>
        [XmlAttribute("CSSFiles", Form = XmlSchemaForm.Unqualified)]
        public int CSSFilesCount
        {
            get
            {
                return this.CSSFiles.Count;
            }
            set
            {
                int a = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("XSLFiles", Form = XmlSchemaForm.Unqualified)]
        public int XSLFilesCount
        {
            get
            {
                return this.xslFiles.Count;
            }
            set
            {
                int a = value;
            }
        }


        /// <remarks/>
        [XmlAttribute("JSFiles", Form = XmlSchemaForm.Unqualified)]
        public int JSFilesCount
        {
            get
            {
                return this.JSFiles.Count;
            }
            set
            {
                int a = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("FeatureReceivers", Form = XmlSchemaForm.Unqualified)]
        public int FeatureReceiversCount
        {
            get
            {
                return this.featureReceiversCount;
            }
            set
            {
                this.featureReceiversCount = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("TimerJobs", Form = XmlSchemaForm.Unqualified)]
        public int TimerJobsCount
        {
            get
            {
                return this.TimerJobs.Count;
            }
            set
            {
                int a = value;
            }

        }

        /// <remarks/>
        [XmlAttribute("EventHandlers", Form = XmlSchemaForm.Unqualified)]
        public int EventHandlersCount
        {
            get
            {
                return this.EventHandlers.Count;
            }
            set
            {
                int a = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("ClassResources", Form = XmlSchemaForm.Unqualified)]
        public int ClassResourcesCount
        {
            get
            {
                return this.classResourceCount;
            }
            set
            {
                this.classResourceCount = value;
            }
        }


        /// <remarks/>
        [XmlAttribute("SafeControlEntries", Form = XmlSchemaForm.Unqualified)]
        public int SafeControlEntriesCount
        {
            get
            {
                return this.safeControlCount;
            }
            set
            {
                this.safeControlCount = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("WebServices", Form = XmlSchemaForm.Unqualified)]
        public int WebServicesCount
        {
            get
            {
                return this.WebServices.Count;
            }
            set
            {
                int a = value;
            }
        }


        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Error
        {
            get
            {
                return this.error;
            }
            set
            {
                this.error = value;
            }
        }

        /// <remarks/>
        [XmlArray("Features", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("Feature", Form = XmlSchemaForm.Unqualified)]
        public List<FeatureInfo> Features
        {
            get
            {
                return this.features;
            }
            set
            {
                this.features = value;
            }
        }

        /// <remarks/>
        [XmlArray("CustomFieldTypes", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("CustomFieldType", Form = XmlSchemaForm.Unqualified)]
        public List<CustomFieldType> CustomFieldTypes
        {
            get
            {
                return this.customFieldTypes;
            }
            set
            {
                this.customFieldTypes = value;
            }
        }

        /// <remarks/>
        [XmlArray("Assemblies", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("Assembly", Form = XmlSchemaForm.Unqualified)]
        public List<AssemblyInfo> Assemblies
        {
            get
            {
                return this.assemblies;
            }
            set
            {
                this.assemblies = value;
            }
        }


        /// <remarks/>
        [XmlArray("Timerjobs", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("TimerJob", Form = XmlSchemaForm.Unqualified)]
        public List<string> TimerJobs
        {
            get
            {
                return this.timerJobs;
            }
            set
            {
                this.timerJobs = value;
            }
        }

        /// <remarks/>
        [XmlArray("Eventhandlers", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("EventHandler", Form = XmlSchemaForm.Unqualified)]
        public List<string> EventHandlers
        {
            get
            {
                return this.eventHandlers;
            }
            set
            {
                this.eventHandlers = value;
            }
        }

        /// <remarks/>
        [XmlArray("UserControls", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("UserControl", Form = XmlSchemaForm.Unqualified)]
        public List<string> UserControls
        {
            get
            {
                return this.userControls;
            }
            set
            {
                this.userControls = value;
            }
        }



        /// <remarks/>
        [XmlArray("SiteDefinitions", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("SiteDefinition", Form = XmlSchemaForm.Unqualified)]
        public List<SiteDefinitionInformation> SiteDefinitions
        {
            get
            {
                return this.siteDefinitions;
            }
            set
            {
                this.siteDefinitions = value;
            }
        }

        /// <remarks/>
        [XmlArray("SiteTemplates", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("SiteTemplate", Form = XmlSchemaForm.Unqualified)]
        public List<string> SiteTemplates
        {
            get
            {
                return this.siteTemplates;
            }
            set
            {
                this.siteTemplates = value;
            }
        }


        /// <remarks/>
        [XmlArray("ListTemplates", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("ListTemplate", Form = XmlSchemaForm.Unqualified)]
        public List<string> ListTemplates
        {
            get
            {
                return this.listTemplates;
            }
            set
            {
                this.listTemplates = value;
            }
        }

        /// <remarks/>
        [XmlArray("WebParts", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("WebPart", Form = XmlSchemaForm.Unqualified)]
        public List<WebPartInfo> WebParts
        {
            get
            {
                return this.webParts;
            }
            set
            {
                this.webParts = value;
            }
        }

        /// <remarks/>
        [XmlArray("AspxPages", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("AspxPage", Form = XmlSchemaForm.Unqualified)]
        public List<string> AspxPages
        {
            get
            {
                return this.aspxPages;
            }
            set
            {
                this.aspxPages = value;
            }
        }

        /// <remarks/>
        [XmlArray("WebServices", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("WebService", Form = XmlSchemaForm.Unqualified)]
        public List<string> WebServices
        {
            get
            {
                return this.webServices;
            }
            set
            {
                this.webServices = value;
            }
        }
        /// <remarks/>
        [XmlArray("CSSFiles", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("CSSFile", Form = XmlSchemaForm.Unqualified)]
        public List<string> CSSFiles
        {
            get
            {
                return this.cssFiles;
            }
            set
            {
                this.cssFiles = value;
            }
        }

        /// <remarks/>
        [XmlArray("XSLFiles", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("XSLFile", Form = XmlSchemaForm.Unqualified)]
        public List<string> XSLFiles
        {
            get
            {
                return this.xslFiles;
            }
            set
            {
                this.xslFiles = value;
            }
        }


        /// <remarks/>
        [XmlArray("JSFiles", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("JSFile", Form = XmlSchemaForm.Unqualified)]
        public List<string> JSFiles
        {
            get
            {
                return this.jSFiles;
            }
            set
            {
                this.jSFiles = value;
            }
        }

        /// <remarks/>
        [XmlArray("Files", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("File", Form = XmlSchemaForm.Unqualified)]
        public List<string> Files
        {
            get
            {
                return this.files;
            }
            set
            {
                this.files = value;
            }
        }


    }
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class EventReceiver
    {
        private string name;

        private string type;

        private string assembly;

        private string classOfReceiver;

        private string sequenceNumber;

        private int listTemplateId;

        private string listUrl;

        private string scope;


        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Assembly
        {
            get
            {
                return this.assembly;
            }
            set
            {
                this.assembly = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }


        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string ClassOfReceiver
        {
            get
            {
                return this.classOfReceiver;
            }
            set
            {
                this.classOfReceiver = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string SequenceNumber
        {
            get
            {
                return this.sequenceNumber;
            }
            set
            {
                this.sequenceNumber = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public int ListTemplateId
        {
            get
            {
                return this.listTemplateId;
            }
            set
            {
                this.listTemplateId = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
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
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Scope
        {
            get
            {
                return this.scope;
            }
            set
            {
                this.scope = value;
            }
        }


    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class ListTemplate
    {
        private string name;

        private string displayName;

        private string type;

        private string basetype;

        private string description;


        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
            set
            {
                this.displayName = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }


        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string BaseType
        {
            get
            {
                return this.basetype;
            }
            set
            {
                this.basetype = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }


    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class ListInst
    {
        private string title;

        private string templateType;

        private string description;

        private string url;


        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string TemplateType
        {
            get
            {
                return this.templateType;
            }
            set
            {
                this.templateType = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string URL
        {
            get
            {
                return this.url;
            }
            set
            {
                this.url = value;
            }
        }


    }
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class FeatureReceiver
    {
        private string id;
        private string name;
        private string receiverAssembly;
        private string classOfReceiver;
        private string scope;

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string ReceiverAssembly
        {
            get
            {
                return this.receiverAssembly;
            }
            set
            {
                this.receiverAssembly = value;
            }
        }
        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string ClassOfReceiver
        {
            get
            {
                return this.classOfReceiver;
            }
            set
            {
                this.classOfReceiver = value;
            }
        }
        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Scope
        {
            get
            {
                return this.scope;
            }
            set
            {
                this.scope = value;
            }
        }
    }


    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class WebPartInfo
    {

        private string file;

        private string type;

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string File
        {
            get
            {
                return this.file;
            }
            set
            {
                this.file = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }
    }
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class MasterPageInfo
    {

        private string url;

        private string name;

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string URL
        {
            get
            {
                return this.url;
            }
            set
            {
                this.url = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class PageLayoutInfo
    {

        private string url;

        private string name;

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string URL
        {
            get
            {
                return this.url;
            }
            set
            {
                this.url = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class PageInfo
    {

        private string url;

        private string name;

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string URL
        {
            get
            {
                return this.url;
            }
            set
            {
                this.url = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class ContentTypeInfo
    {

        private string typeID;
        private string name;
        private string description;
        private int fieldCount;
        private List<SharedFieldInfo> sharedFields;
        private bool isDocumentSetContentType;

        public ContentTypeInfo()
        {
            sharedFields = new List<SharedFieldInfo>(10);
        }
        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public bool IsDocumentSetContentType
        {
            get
            {
                return this.isDocumentSetContentType;
            }
            set
            {
                this.isDocumentSetContentType = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string TypeID
        {
            get
            {
                return this.typeID;
            }
            set
            {
                this.typeID = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public int FieldCount
        {
            get
            {
                return this.fieldCount;
            }
            set
            {
                this.fieldCount = value;
            }
        }

        /// <remarks/>
        [XmlArray("SharedFields", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("SharedField", Form = XmlSchemaForm.Unqualified)]
        public List<SharedFieldInfo> SharedFields
        {
            get
            {
                return this.sharedFields;
            }
            set
            {
                this.sharedFields = value;
            }
        }


    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class SharedFieldInfo
    {

        private string type;
        private string id;
        private string name;
        private string description;
        private bool needsUpdating;

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }
        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public bool NeedsToBeUpdated
        {
            get
            {
                return this.needsUpdating;
            }
            set
            {
                this.needsUpdating = value;
            }
        }


    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class Class
    {

        private string name;

        private string inheritsFrom;

        private int nrOfMethods;

        private List<string> methods;

        private int nrOfProperties;

        private List<string> properties;

        public Class()
        {
            properties = new List<string>(100);
            methods = new List<string>(100);
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string InheritsFrom
        {
            get
            {
                return this.inheritsFrom;
            }
            set
            {
                this.inheritsFrom = value;
            }
        }


        /// <remarks/>
        [XmlAttribute("Methods", Form = XmlSchemaForm.Unqualified)]
        public int NrOfMethods
        {
            get
            {
                return this.nrOfMethods;
            }
            set
            {
                this.nrOfMethods = value;
            }
        }

        /// <remarks/>
        [XmlArray("Methods", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("Method", Form = XmlSchemaForm.Unqualified)]
        public List<string> Methods
        {
            get
            {
                return this.methods;
            }
            set
            {
                this.methods = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("Properties", Form = XmlSchemaForm.Unqualified)]
        public int NrOfProperties
        {
            get
            {
                return this.nrOfProperties;
            }
            set
            {
                this.nrOfProperties = value;
            }
        }



        /// <remarks/>
        [XmlArray("Properties", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("Property", Form = XmlSchemaForm.Unqualified)]
        public List<string> Properties
        {
            get
            {
                return this.properties;
            }
            set
            {
                this.properties = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class AssemblyInfo
    {

        private string file;

        private int size;

        private List<string> referencedAssemblies;

        private List<string> safeConfigEntries;

        private List<string> modules;

        private List<Class> classes;

        private int nrOfResources;

        private string version;


        private List<string> resources;

        public AssemblyInfo()
        {
            classes = new List<Class>(50);
            referencedAssemblies = new List<string>(100);

            safeConfigEntries = new List<string>(100);

            modules = new List<string>(100);

            resources = new List<string>(100);

        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string File
        {
            get
            {
                return this.file;
            }
            set
            {
                this.file = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Version
        {
            get
            {
                return this.version;
            }
            set
            {
                this.version = value;
            }
        }


        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public int Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
            }
        }

        /// <remarks/>
        [XmlArray("ReferencedAssemblies", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("ReferencedAssembly", Form = XmlSchemaForm.Unqualified)]
        public List<string> ReferencedAssemblies
        {
            get
            {
                return this.referencedAssemblies;
            }
            set
            {
                this.referencedAssemblies = value;
            }
        }

        /// <remarks/>
        [XmlArray("SafeConfigEntries", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("SafeConfigEntry", Form = XmlSchemaForm.Unqualified)]
        public List<string> SafeConfigEntries
        {
            get
            {
                return this.safeConfigEntries;
            }
            set
            {
                this.safeConfigEntries = value;
            }
        }

        /// <remarks/>
        [XmlArray("Modules", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("Module", Form = XmlSchemaForm.Unqualified)]
        public List<string> Modules
        {
            get
            {
                return this.modules;
            }
            set
            {
                this.modules = value;
            }
        }

        /// <remarks/>
        [XmlArray("Classes", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("Class", Form = XmlSchemaForm.Unqualified)]
        public List<Class> Classes
        {
            get
            {
                return this.classes;
            }
            set
            {
                this.classes = value;
            }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public int NrOfResources
        {
            get
            {
                return this.nrOfResources;
            }
            set
            {
                this.nrOfResources = value;
            }
        }
        /// <remarks/>
        [XmlArray("Resources", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("Resource", Form = XmlSchemaForm.Unqualified)]
        public List<string> Resources
        {
            get
            {
                return this.resources;
            }
            set
            {
                this.resources = value;
            }
        }

    }

    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class FeatureInfo
    {

        private string nameField;

        private string idField;

        private string scope;

        private string version;

        private int fileSize;

        private string fileDate;

        private string receiverAssembly;
        private string receiverClass;

        private List<ContentTypeInfo> contentTypes;
        private List<SharedFieldInfo> sharedFields;

        private List<string> files;

        private List<MasterPageInfo> masterPages;

        private List<WebPartInfo> webParts;

        private List<ContentTypeBindingDefinition> contentTypeBindings;


        private List<PageLayoutInfo> pageLayouts;

        private List<PageInfo> pages;

        private List<FeatureReceiver> featureReceivers;

        private List<ListTemplate> listTemplates;

        private List<ListInst> listInsts;

        private List<EventReceiver> eventReceivers;

        private List<WorkFlowDetail> workFlowDetails;

        private List<ActionDetail> actionDetails;

        private List<WebTemplate> webTemplateDetails;

        private List<WorkflowAction> workflowActionDetails;

        public FeatureInfo()
        {
            this.files = new List<string>(100);
            this.contentTypes = new List<ContentTypeInfo>(10);
            this.contentTypeBindings = new List<ContentTypeBindingDefinition>(10);
            this.masterPages = new List<MasterPageInfo>(10);
            this.pageLayouts = new List<PageLayoutInfo>(10);
            this.pages = new List<PageInfo>(100);
            this.webParts = new List<WebPartInfo>(10);
            this.featureReceivers = new List<FeatureReceiver>(5);
            this.listTemplates = new List<ListTemplate>(50);
            this.listInsts = new List<ListInst>(50);
            this.sharedFields = new List<SharedFieldInfo>(50);
            this.eventReceivers = new List<EventReceiver>(100);
            this.workFlowDetails = new List<WorkFlowDetail>(10);
            this.actionDetails = new List<ActionDetail>(100);
            this.webTemplateDetails = new List<WebTemplate>(10);
            this.workflowActionDetails = new List<WorkflowAction>(10);
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
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
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Scope
        {
            get
            {
                return this.scope;
            }
            set
            {
                this.scope = value;
            }
        }


        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Version
        {
            get
            {
                return this.version;
            }
            set
            {
                this.version = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
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
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string FileDate
        {
            get
            {
                return this.fileDate;
            }
            set
            {
                this.fileDate = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public int FileSize
        {
            get
            {
                return this.fileSize;
            }
            set
            {
                this.fileSize = value;
            }
        }
        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string ReceiverAssembly
        {
            get
            {
                return this.receiverAssembly;
            }
            set
            {
                this.receiverAssembly = value;
            }
        }/// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string ReceiverClass
        {
            get
            {
                return this.receiverClass;
            }
            set
            {
                this.receiverClass = value;
            }
        }

        /// <remarks/>
        [XmlArray("MasterPages", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("MasterPage", Form = XmlSchemaForm.Unqualified)]
        public List<MasterPageInfo> MasterPages
        {
            get
            {
                return this.masterPages;
            }
            set
            {
                this.masterPages = value;
            }
        }

        /// <remarks/>
        [XmlArray("PageLayouts", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("PageLayout", Form = XmlSchemaForm.Unqualified)]
        public List<PageLayoutInfo> PageLayouts
        {
            get
            {
                return this.pageLayouts;
            }
            set
            {
                this.pageLayouts = value;
            }
        }

        /// <remarks/>
        [XmlArray("Pages", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("Page", Form = XmlSchemaForm.Unqualified)]
        public List<PageInfo> Pages
        {
            get
            {
                return this.pages;
            }
            set
            {
                this.pages = value;
            }
        }

        /// <remarks/>
        [XmlArray("WebParts", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("WebPart", Form = XmlSchemaForm.Unqualified)]
        public List<WebPartInfo> WebParts
        {
            get
            {
                return this.webParts;
            }
            set
            {
                this.webParts = value;
            }
        }

        /// <remarks/>
        [XmlArray("Featurereceivers", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("Featurereceiver", Form = XmlSchemaForm.Unqualified)]
        public List<FeatureReceiver> FeatureReceivers
        {
            get
            {
                return this.featureReceivers;
            }
            set
            {
                this.featureReceivers = value;
            }
        }


        /// <remarks/>
        [XmlArray("EventReceivers", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("EventReceiver", Form = XmlSchemaForm.Unqualified)]
        public List<EventReceiver> EventReceivers
        {
            get
            {
                return this.eventReceivers;
            }
            set
            {
                this.eventReceivers = value;
            }
        }

        /// <remarks/>
        [XmlArray("ListTemplates", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("ListTemplate", Form = XmlSchemaForm.Unqualified)]
        public List<ListTemplate> ListTemplates
        {
            get
            {
                return this.listTemplates;
            }
            set
            {
                this.listTemplates = value;
            }
        }

        /// <remarks/>
        [XmlArray("ListInsts", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("ListInst", Form = XmlSchemaForm.Unqualified)]
        public List<ListInst> ListInsts
        {
            get
            {
                return this.listInsts;
            }
            set
            {
                this.listInsts = value;
            }
        }



        /// <remarks/>
        [XmlArray("Files", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("File", Form = XmlSchemaForm.Unqualified)]
        public List<string> Files
        {
            get
            {
                return this.files;
            }
            set
            {
                this.files = value;
            }
        }

        /// <remarks/>
        [XmlArray("ContentTypes", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("ContentType", Form = XmlSchemaForm.Unqualified)]
        public List<ContentTypeInfo> ContentTypes
        {
            get
            {
                return this.contentTypes;
            }
            set
            {
                this.contentTypes = value;
            }
        }

        /// <remarks/>
        [XmlArray("SharedFields", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("SharedField", Form = XmlSchemaForm.Unqualified)]
        public List<SharedFieldInfo> SharedFields
        {
            get
            {
                return this.sharedFields;
            }
            set
            {
                this.sharedFields = value;
            }
        }

        /// <remarks/>
        [XmlArray("ContentTypeBindings", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("ContentTypeBinding", Form = XmlSchemaForm.Unqualified)]
        public List<ContentTypeBindingDefinition> ContentTypeBindings
        {
            get
            {
                return this.contentTypeBindings;
            }
            set
            {
                this.contentTypeBindings = value;
            }
        }
        /// <remarks/>
        [XmlArray("WorkFlowDetails", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("WorkFlowDetail", Form = XmlSchemaForm.Unqualified)]
        public List<WorkFlowDetail> WorkFlowDetails
        {
            get
            {
                return this.workFlowDetails;
            }
            set
            {
                this.workFlowDetails = value;
            }
        }
        [XmlArray("ActionDetails", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("ActionDetail", Form = XmlSchemaForm.Unqualified)]
        public List<ActionDetail> ActionDetails
        {
            get
            {
                return this.actionDetails;
            }
            set
            {
                this.actionDetails = value;
            }
        }

        [XmlArray("WebTemplateDetails", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("WebTemplate", Form = XmlSchemaForm.Unqualified)]
        public List<WebTemplate> WebTemplateDetails
        {
            get
            {
                return this.webTemplateDetails;
            }
            set
            {
                this.webTemplateDetails = value;
            }
        }

        [XmlArray("WorkflowActionDetails", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("WorkflowAction", Form = XmlSchemaForm.Unqualified)]
        public List<WorkflowAction> WorkflowActionDetails
        {
            get
            {
                return this.workflowActionDetails;
            }
            set
            {
                this.workflowActionDetails = value;
            }
        }
    }


    public partial class SiteDefinitionInformation
    {
        private string nameField;

        private int nrOfLocalesField;

        private List<string> siteFeaturesField;

        private List<string> webFeaturesField;

        public SiteDefinitionInformation()
        {
            siteFeaturesField = new List<string>(100);
            webFeaturesField = new List<string>(100);
        }

        /// <remarks/>
        [XmlAttribute("Name", Form = XmlSchemaForm.Unqualified)]
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
        [XmlAttribute("Locales", Form = XmlSchemaForm.Unqualified)]
        public int NrOfLocales
        {
            get
            {
                return this.nrOfLocalesField;
            }
            set
            {
                this.nrOfLocalesField = value;
            }
        }

        /// <remarks/>
        [XmlArray("SiteFeatures", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("SiteFeature", Form = XmlSchemaForm.Unqualified)]
        public List<string> SiteFeatures
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
        [XmlArray("WebFeatures", Form = XmlSchemaForm.Unqualified)]
        [XmlArrayItem("WebFeature", Form = XmlSchemaForm.Unqualified)]
        public List<string> WebFeatures
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
    }

    public partial class CustomFieldType
    {
        private string typeName;

        private string parentType;

        private string fieldTypeClass;

        private string fieldTypeXMLFile;

        private string typeShortDescription;

        private string typeDisplayName;

        public string TypeShortDescription
        {
            get { return this.typeShortDescription; }
            set { this.typeShortDescription = value; }
        }

        public string TypeDisplayName
        {
            get { return this.typeDisplayName; }
            set { this.typeDisplayName = value; }
        }


        public string FieldTypeXMLFile
        {
            get
            {
                return this.fieldTypeXMLFile;
            }
            set
            {
                this.fieldTypeXMLFile = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("TypeName", Form = XmlSchemaForm.Unqualified)]
        public string TypeName
        {
            get
            {
                return this.typeName;
            }
            set
            {
                this.typeName = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("ParentType", Form = XmlSchemaForm.Unqualified)]
        public string ParentType
        {
            get
            {
                return this.parentType;
            }
            set
            {
                this.parentType = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("FieldTypeClass", Form = XmlSchemaForm.Unqualified)]
        public string FieldTypeClass
        {
            get
            {
                return this.fieldTypeClass;
            }
            set
            {
                this.fieldTypeClass = value;
            }
        }
    }

    public partial class v2WebPart
    {
        private string guid;

        private string nameField;

        /// <remarks/>
        [XmlAttribute("Name", Form = XmlSchemaForm.Unqualified)]
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
        [XmlAttribute("ID", Form = XmlSchemaForm.Unqualified)]
        public string ID
        {
            get
            {
                return this.guid;
            }
            set
            {
                this.guid = value;
            }
        }

    }

    public partial class WebConfigurationModifications
    {

        private string nameField;
        private string webAppNameField;
        private string ownerField;
        private string pathField;
        private uint sequenceField;
        private string valueField;

        /// <remarks/>
        [XmlAttribute("WebApplication", Form = XmlSchemaForm.Unqualified)]
        public string WebApplication
        {
            get
            {
                return this.webAppNameField;
            }
            set
            {
                this.webAppNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("Name", Form = XmlSchemaForm.Unqualified)]
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
        [XmlAttribute("Owner", Form = XmlSchemaForm.Unqualified)]
        public string Owner
        {
            get
            {
                return this.ownerField;
            }
            set
            {
                this.ownerField = value;
            }
        }
        /// <remarks/>
        [XmlAttribute("Path", Form = XmlSchemaForm.Unqualified)]
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
        [XmlAttribute("Value", Form = XmlSchemaForm.Unqualified)]
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

    //------------------------------------------------------------------------------
    // <auto-generated>
    //     This code was generated by a tool.
    //     Runtime Version:4.0.30319.19455
    //
    //     Changes to this file may cause incorrect behavior and will be lost if
    //     the code is regenerated.
    // </auto-generated>
    //------------------------------------------------------------------------------

    // 
    // This source code was auto-generated by xsd, Version=4.0.30319.33440.
    // 

    #region WorkFlow - ActionFile XML
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class WorkflowInfo
    {

        private WorkflowInfoConditions conditionsField;

        private WorkflowInfoActions actionsField;

        private string languageField;

        /// <remarks/>
        public WorkflowInfoConditions Conditions
        {
            get
            {
                return this.conditionsField;
            }
            set
            {
                this.conditionsField = value;
            }
        }

        /// <remarks/>
        public WorkflowInfoActions Actions
        {
            get
            {
                return this.actionsField;
            }
            set
            {
                this.actionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Language
        {
            get
            {
                return this.languageField;
            }
            set
            {
                this.languageField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("xsd", "4.0.30319.33440")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class WorkFlowDetail
    {
        private string name;

        private string description;

        private string id;

        private string codeBesideAssembly;

        private string codeBesideClass;

        private string associationUrl;
        private string associationType;

        private string workFlowDisplayName;
        private string restrictToScope;
        private string workFlowVersion;
        private string workFlowComment;

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }
        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string CodeBesideAssembly
        {
            get
            {
                return this.codeBesideAssembly;
            }
            set
            {
                this.codeBesideAssembly = value;
            }
        }
        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string CodeBesideClass
        {
            get
            {
                return this.codeBesideClass;
            }
            set
            {
                this.codeBesideClass = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string AssociationUrl
        {
            get
            {
                return this.associationUrl;
            }
            set
            {
                this.associationUrl = value;
            }
        }
        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string AssociationType
        {
            get
            {
                return this.associationType;
            }
            set
            {
                this.associationType = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string WorkFlowDisplayName
        {
            get
            {
                return this.workFlowDisplayName;
            }
            set
            {
                this.workFlowDisplayName = value;
            }
        }
        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string RestrictToScope
        {
            get
            {
                return this.restrictToScope;
            }
            set
            {
                this.restrictToScope = value;
            }
        }
        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string WorkFlowVersion
        {
            get
            {
                return this.workFlowVersion;
            }
            set
            {
                this.workFlowVersion = value;
            }
        }
        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Unqualified)]
        public string WorkFlowComment
        {
            get
            {
                return this.workFlowComment;
            }
            set
            {
                this.workFlowComment = value;
            }
        }
    }

    public partial class ActionDetail
    {
        private string name;
        private string className;
        private string assembly;
        private string appliesTo;
        private string category;
        private string actionXMLFile;

        [XmlAttribute("Name", Form = XmlSchemaForm.Unqualified)]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        [XmlAttribute("ClassName", Form = XmlSchemaForm.Unqualified)]
        public string ClassName
        {
            get { return this.className; }
            set { this.className = value; }
        }

        public string ActionXMLFile
        {
            get
            {
                return this.actionXMLFile;
            }
            set
            {
                this.actionXMLFile = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("appliesTo", Form = XmlSchemaForm.Unqualified)]
        public string AppliesTo
        {
            get
            {
                return this.appliesTo;
            }
            set
            {
                this.appliesTo = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("Category", Form = XmlSchemaForm.Unqualified)]
        public string Category
        {
            get
            {
                return this.category;
            }
            set
            {
                this.category = value;
            }
        }

        /// <remarks/>
        [XmlAttribute("Assembly", Form = XmlSchemaForm.Unqualified)]
        public string Assembly
        {
            get
            {
                return this.assembly;
            }
            set
            {
                this.assembly = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class WorkflowInfoConditions
    {

        private WorkflowInfoConditionsCondition[] conditionField;

        private string andField;

        private string orField;

        private string notField;

        private string whenField;

        private string elseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Condition")]
        public WorkflowInfoConditionsCondition[] Condition
        {
            get
            {
                return this.conditionField;
            }
            set
            {
                this.conditionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string And
        {
            get
            {
                return this.andField;
            }
            set
            {
                this.andField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Or
        {
            get
            {
                return this.orField;
            }
            set
            {
                this.orField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Not
        {
            get
            {
                return this.notField;
            }
            set
            {
                this.notField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string When
        {
            get
            {
                return this.whenField;
            }
            set
            {
                this.whenField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Else
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class WorkflowInfoConditionsCondition
    {

        private WorkflowInfoConditionsConditionRuleDesigner ruleDesignerField;

        private WorkflowInfoConditionsConditionParameter[] parametersField;

        private string nameField;

        private string functionNameField;

        private string classNameField;

        private string assemblyField;

        private string appliesToField;

        private bool usesCurrentItemField;

        /// <remarks/>
        public WorkflowInfoConditionsConditionRuleDesigner RuleDesigner
        {
            get
            {
                return this.ruleDesignerField;
            }
            set
            {
                this.ruleDesignerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Parameter", IsNullable = false)]
        public WorkflowInfoConditionsConditionParameter[] Parameters
        {
            get
            {
                return this.parametersField;
            }
            set
            {
                this.parametersField = value;
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
        public bool UsesCurrentItem
        {
            get
            {
                return this.usesCurrentItemField;
            }
            set
            {
                this.usesCurrentItemField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class WorkflowInfoConditionsConditionRuleDesigner
    {

        private WorkflowInfoConditionsConditionRuleDesignerFieldBind[] fieldBindField;

        private string sentenceField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FieldBind")]
        public WorkflowInfoConditionsConditionRuleDesignerFieldBind[] FieldBind
        {
            get
            {
                return this.fieldBindField;
            }
            set
            {
                this.fieldBindField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Sentence
        {
            get
            {
                return this.sentenceField;
            }
            set
            {
                this.sentenceField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class WorkflowInfoConditionsConditionRuleDesignerFieldBind
    {

        private byte idField;

        private string fieldField;

        private string textField;

        private string designerTypeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Id
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Field
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Text
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DesignerType
        {
            get
            {
                return this.designerTypeField;
            }
            set
            {
                this.designerTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class WorkflowInfoConditionsConditionParameter
    {

        private string nameField;

        private string typeField;

        private string directionField;

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
        [System.Xml.Serialization.XmlAttributeAttribute()]
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

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class WorkflowInfoActions
    {

        private WorkflowInfoActionsAction[] actionField;

        private string sequentialField;

        private string parallelField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Action")]
        public WorkflowInfoActionsAction[] Action
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Sequential
        {
            get
            {
                return this.sequentialField;
            }
            set
            {
                this.sequentialField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Parallel
        {
            get
            {
                return this.parallelField;
            }
            set
            {
                this.parallelField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class WorkflowInfoActionsAction
    {

        private WorkflowInfoActionsActionRuleDesigner ruleDesignerField;

        private WorkflowInfoActionsActionParameter[] parametersField;

        private string nameField;

        private string classNameField;

        private string assemblyField;

        private string appliesToField;

        private string categoryField;

        /// <remarks/>
        public WorkflowInfoActionsActionRuleDesigner RuleDesigner
        {
            get
            {
                return this.ruleDesignerField;
            }
            set
            {
                this.ruleDesignerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Parameter", IsNullable = false)]
        public WorkflowInfoActionsActionParameter[] Parameters
        {
            get
            {
                return this.parametersField;
            }
            set
            {
                this.parametersField = value;
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class WorkflowInfoActionsActionRuleDesigner
    {

        private WorkflowInfoActionsActionRuleDesignerFieldBind[] fieldBindField;

        private string sentenceField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FieldBind")]
        public WorkflowInfoActionsActionRuleDesignerFieldBind[] FieldBind
        {
            get
            {
                return this.fieldBindField;
            }
            set
            {
                this.fieldBindField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Sentence
        {
            get
            {
                return this.sentenceField;
            }
            set
            {
                this.sentenceField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class WorkflowInfoActionsActionRuleDesignerFieldBind
    {

        private WorkflowInfoActionsActionRuleDesignerFieldBindOption[] optionField;

        private string fieldField;

        private string designerTypeField;

        private string textField;

        private byte idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Option")]
        public WorkflowInfoActionsActionRuleDesignerFieldBindOption[] Option
        {
            get
            {
                return this.optionField;
            }
            set
            {
                this.optionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Field
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DesignerType
        {
            get
            {
                return this.designerTypeField;
            }
            set
            {
                this.designerTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Text
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Id
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class WorkflowInfoActionsActionRuleDesignerFieldBindOption
    {

        private string nameField;

        private string valueField;

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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class WorkflowInfoActionsActionParameter
    {

        private string nameField;

        private string typeField;

        private string directionField;

        private bool initialValueField;

        private bool initialValueFieldSpecified;

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
        [System.Xml.Serialization.XmlAttributeAttribute()]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool InitialValue
        {
            get
            {
                return this.initialValueField;
            }
            set
            {
                this.initialValueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool InitialValueSpecified
        {
            get
            {
                return this.initialValueFieldSpecified;
            }
            set
            {
                this.initialValueFieldSpecified = value;
            }
        }
    }

    #endregion  WorkFlow - ActionFile XML


}
