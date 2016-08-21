using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Resources;
using System.Linq;
using Microsoft.Deployment.Compression.Cab;
using Mono.Cecil;

namespace SharePoint.SolutionAnalyzer
{
    /// <summary>
    /// Enumeration identifying the type of CAB file
    /// </summary>
    public enum CabType
    {
        WebPart = 0,
        ListTemplate = 1,
        SiteTemplate = 2
    };

    /// <summary>
    /// Scanner that can open a WSP package and look inside
    /// </summary>
    public class Analyzer
    {
        private int _numberOfAssemblies;
        private int _numberOfClassResources;
        private int _numberOfFeatures;
        private int _numberOfSafeControlEntries;
        private bool _verbose;

        /// <summary>
        /// Look inside an assembly
        /// </summary>
        private void ProcessAssembly(AssemblyFileReference assemblyReference, SolutionInformation solutionInfo, string folder)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                FileInfo fi = new FileInfo(Path.Combine(folder, assemblyReference.Location));
                if (_verbose) { Console.WriteLine("Processing assembly :{0}", fi.FullName); }
                sb.AppendFormat("{0} size {1}\r\n", assemblyReference.Location, fi.Length);
                _numberOfAssemblies = _numberOfAssemblies + 1;

                Mono.Cecil.ModuleDefinition module = Mono.Cecil.ModuleDefinition.ReadModule(Path.Combine(folder, assemblyReference.Location));

                AssemblyInfo assemblyInfo = new AssemblyInfo();
                assemblyInfo.File = fi.Name;
                assemblyInfo.Size = (int)fi.Length;
                int pos = module.FullyQualifiedName.IndexOf("Version=");
                if (pos > 0)
                {
                    assemblyInfo.Version = module.FullyQualifiedName.Substring(pos + 8);
                    pos = assemblyInfo.Version.IndexOf(",");
                    assemblyInfo.Version = assemblyInfo.Version.Substring(0, pos);
                }
                solutionInfo.Assemblies.Add(assemblyInfo);

                // Check if this is an assembly of  Microsoft SharePoint
                if (!module.Assembly.FullName.Contains("PublicKeyToken=71e9bce111e9429c"))
                {
                    if (assemblyReference.SafeControls != null)
                    {
                        sb.AppendFormat("\tSafeControlEntries: {0}\r\n", assemblyReference.SafeControls.GetLength(0));

                        foreach (SafeControlDefinition scd in assemblyReference.SafeControls)
                        {
                            if (scd.SafeSpecified)
                            {
                                string s =
                                    String.Format(
                                        "\t\t<SafeControl Assembly=\"{0}\" Namespace=\"{1}\" TypeName=\"{2}\" Safe=\"{3}\" />\r\n",
                                        scd.Assembly, scd.Namespace, scd.TypeName, scd.Safe);
                                sb.Append(s);
                                assemblyInfo.SafeConfigEntries.Add(s);
                                solutionInfo.SafeControlEntriesCount = solutionInfo.SafeControlEntriesCount + 1;
                            }
                            else
                            {
                                string s =
                                    String.Format(
                                        "\t\t<SafeControl Assembly=\"{0}\" Namespace=\"{1}\" TypeName=\"{2}\" />\r\n",
                                        scd.Assembly, scd.Namespace, scd.TypeName);
                                sb.Append(s);
                                assemblyInfo.SafeConfigEntries.Add(s);
                            }
                            _numberOfSafeControlEntries = _numberOfSafeControlEntries + 1;
                        }
                    }


                    if (assemblyReference.ClassResources != null)
                    {
                        _numberOfClassResources += assemblyReference.ClassResources.GetLength(0);
                        solutionInfo.ClassResourcesCount += assemblyReference.ClassResources.GetLength(0);
                    }

                    var modules = module.ModuleReferences;
                    sb.AppendFormat("\tModules: {0}\r\n", modules.Count + 1);

                    sb.AppendFormat("\t\t{0}\r\n", module.Name);
                    assemblyInfo.Modules.Add(module.Name);
                    foreach (var m in module.ModuleReferences)
                    {
                        sb.AppendFormat("\t\t{0}\r\n", m.Name);
                        assemblyInfo.Modules.Add(m.Name);
                    }
                    var refs = module.AssemblyReferences;

                    sb.AppendFormat("\tReferenced assemblies: {0}\r\n", refs.Count);
                    foreach (var an in refs)
                    {
                        sb.AppendFormat("\t\t{0}\r\n", an.Name);
                        assemblyInfo.ReferencedAssemblies.Add(an.Name);
                    }

                    var types = module.Types;
                    sb.AppendFormat("\tTypes: {0}\r\n", types.Count);
                    foreach (var t in types)
                    {
                        try
                        {
                            // Skip the default module node
                            if (t.FullName.Equals("<Module>", StringComparison.InvariantCultureIgnoreCase))
                            {
                                continue;
                            }

                            Class c = new Class();
                            c.Name = t.FullName;
                            if (t.BaseType != null)
                            {
                                c.InheritsFrom = t.BaseType.FullName;
                            }

                            switch (c.InheritsFrom)
                            {
                                case "Microsoft.SharePoint.Administration.SPJobDefinition":
                                    {
                                        solutionInfo.TimerJobs.Add(c.Name);
                                        break;
                                    }
                                case "Microsoft.SharePoint.SPItemEventReceiver":
                                    {
                                        solutionInfo.EventHandlers.Add(c.Name);
                                        break;
                                    }
                                case "System.Web.UI.UserControl":
                                    {
                                        solutionInfo.UserControls.Add(c.Name);
                                        break;
                                    }
                            }

                            sb.AppendFormat("\t\t{0}\r\n", t.FullName);

                            var methods = t.Methods;
                            sb.AppendFormat("\t\t Methods:{0}\r\n", methods.Count);
                            c.NrOfMethods = methods.Count;
                            if (_verbose)
                            {
                                foreach (var m in methods)
                                {
                                    c.Methods.Add(new MethodInformation() { Name = m.Name, CodeSize = m.Body.CodeSize });
                                    sb.AppendFormat("\t\t\t{0}\r\n", m.Name);
                                }
                            }

                            var props = t.Properties;

                            sb.AppendFormat("\t\t Properties:{0}\r\n", props.Count);
                            c.NrOfProperties = props.Count;
                            if (_verbose)
                            {
                                foreach (var p in props)
                                {
                                    sb.AppendFormat("\t\t\t{0}\r\n", p.Name);
                                    c.Properties.Add(p.Name);
                                }
                            }

                            assemblyInfo.Classes.Add(c);

                        }
                        catch { }

                    }

                    var resources = module.Resources;
                    sb.AppendFormat("\tResources:{0}\r\n", resources.Count);

                    assemblyInfo.NrOfResources = resources.Count;
                    foreach (var resource in resources)
                    {
                        sb.AppendFormat("\t\t\t{0}", resource.Name);
                        assemblyInfo.Resources.Add(resource.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_verbose)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error when processing assembly {0},Error: {1}", assemblyReference.Location,
                        ex.Message);
                    Console.ResetColor();
                }
                string comments = string.Empty;
                comments = "Error when processing assembly : " + assemblyReference.Location;
                //ExceptionCSV_SolutionAnalyzer.WriteException(Path.GetFileName(solutionInfo.Name), "ProcessAssembly", ex.Message, ex.ToString(), ex.GetType().ToString(), comments);
            }
        }

        /// <summary>
        /// Determine the web part type
        /// </summary>
        private string GetTypeOfWebPart(string fileName)
        {
            string type = String.Empty;

            try
            {
                string fileextension = System.IO.Path.GetExtension(fileName);

                if (fileextension.ToLowerInvariant() == ".webpart")
                {
                    using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        FileInfo fi = new FileInfo(fileStream.Name);
                        using (StreamReader reader2 = new StreamReader(fileStream))
                        {
                            while (!reader2.EndOfStream)
                            {
                                string s = reader2.ReadLine().TrimStart();
                                if (s.StartsWith("<type "))
                                {
                                    int pos = s.IndexOf("=");
                                    if (pos > 0)
                                    {
                                        if (s.Length > pos + 2)
                                        {
                                            s = s.Substring(pos + 2);
                                            pos = s.IndexOf("\"");
                                            type = s.Substring(0, pos);
                                        }
                                        else
                                        {
                                            string nextline = reader2.ReadLine().TrimStart();
                                            if ((!nextline.StartsWith("<")) && (nextline.EndsWith(">")))
                                            {
                                                string completeline = s + nextline;
                                                int comp_pos = completeline.IndexOf("=");
                                                if (comp_pos > 0)
                                                {
                                                    completeline = completeline.Substring(comp_pos + 2);
                                                    comp_pos = completeline.IndexOf("\"");
                                                    type = completeline.Substring(0, comp_pos);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (fileextension.ToLowerInvariant() == ".dwp")
                {
                    using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        FileInfo fi = new FileInfo(fileStream.Name);
                        using (StreamReader reader2 = new StreamReader(fileStream))
                        {
                            string temptypename = string.Empty;
                            string tempassembly = string.Empty;
                            while (!reader2.EndOfStream)
                            {
                                string s = reader2.ReadLine().TrimStart();
                                if (s.StartsWith("<TypeName>"))
                                {
                                    int startindexpos = s.IndexOf("<TypeName>");
                                    int lastindexpos = s.LastIndexOf("</TypeName>");
                                    if (startindexpos >= 0 && lastindexpos > 0)
                                    {
                                        //here 10 means length of "<TypeName>"
                                        temptypename = s.Substring(startindexpos + 10, lastindexpos - (startindexpos + 10));
                                    }
                                }

                                if (s.StartsWith("<Assembly>"))
                                {
                                    int startindexpos = s.IndexOf("<Assembly>");
                                    int lastindexpos = s.LastIndexOf("</Assembly>");
                                    if (startindexpos >= 0 && lastindexpos > 0)
                                    {
                                        //here 10 means length of "<TypeName>"
                                        tempassembly = s.Substring(startindexpos + 10, lastindexpos - (startindexpos + 10));
                                    }
                                }
                            }

                            type = temptypename + ", " + tempassembly;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //ExceptionCSV_SolutionAnalyzer.WriteException(fileName, "GetTypeOfWebPart", ex.Message, ex.ToString(), ex.GetType().ToString(), "N/A");
            }

            return type;
        }

        /// <summary>
        /// Process feature xml
        /// </summary>
        public void ProcessFeatureManifest(string fileName, SolutionInformation solutionInfo, Solution sol)
        {
            if (!File.Exists(fileName))
            {
                if (_verbose) { Console.WriteLine("The manifest file {0} could not be found", fileName); }
                //ExceptionCSV_SolutionAnalyzer.WriteException(fileName, "ProcessFeatureManifest", "The manifest file " + fileName + " could not be found", Constants.NotApplicable, "Custom", Constants.NotApplicable);
                return;
            }
            // A FileStream is needed to read the XML document.
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                FileInfo fi = new FileInfo(fileStream.Name);
                using (XmlReader reader2 = new XmlTextReader(fileStream))
                {
                    // Declare an object variable of the type to be deserialized.
                    // Create an instance of the XmlSerializer specifying type and namespace.
                    XmlSerializer serializer2 = new XmlSerializer(typeof(FeatureDefinition));
                    FeatureDefinition featureDef = new FeatureDefinition();

                    try
                    {
                        featureDef = (FeatureDefinition)serializer2.Deserialize(reader2);

                        FeatureInfo feature = new FeatureInfo();
                        feature.FileDate = fi.CreationTime.ToString();
                        feature.FileSize = (int)fi.Length;
                        feature.Name = featureDef.Title;
                        feature.ID = featureDef.Id;
                        feature.Scope = featureDef.Scope.ToString();
                        feature.Version = featureDef.Version;
                        feature.ReceiverClass = featureDef.ReceiverClass;

                        string strFeatureReceiverAssembly = featureDef.ReceiverAssembly;
                        if (strFeatureReceiverAssembly != null)
                        {
                            //ReceiverAssembly="DP.Sharepoint.Workflow, Version=1.0.0.0, Culture=neutral, PublicKeyToken=0298457208daed83"
                            string[] assemblyAttribArray = strFeatureReceiverAssembly.Split(',');
                            if (assemblyAttribArray != null)
                            {
                                if (assemblyAttribArray.Length > 0)
                                {
                                    feature.ReceiverAssembly = assemblyAttribArray[0];
                                }
                            }
                            //END
                        }

                        solutionInfo.Features.Add(feature);

                        if (!String.IsNullOrEmpty(featureDef.ReceiverClass))
                        {
                            bool found = false;
                            foreach (FeatureReceiver fr in feature.FeatureReceivers)
                            {
                                if (fr.Name == featureDef.ReceiverClass)
                                {
                                    fr.ID = featureDef.Id;
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                FeatureReceiver fr = new FeatureReceiver();
                                fr.Name = featureDef.ReceiverClass;
                                fr.ID = featureDef.Id;
                                fr.ReceiverAssembly = featureDef.ReceiverAssembly;
                                fr.ClassOfReceiver = featureDef.ReceiverClass;
                                fr.Scope = featureDef.Scope.ToString();
                                feature.FeatureReceivers.Add(fr);
                            }
                        }

                        if (featureDef.ElementManifests != null)
                        {
                            ElementManifestReferences elems = featureDef.ElementManifests;
                            if (elems.ItemManifests != null)
                            {
                                foreach (ElementManifestReference elem in elems.ItemManifests)
                                {
                                    //ExceptionCSV_SolutionAnalyzer.ExceptionComments = elem.Location;

                                    File.SetAttributes(fi.DirectoryName + "\\" + elem.Location, FileAttributes.Normal);

                                    String text = File.ReadAllText(fi.DirectoryName + "\\" + elem.Location);
                                    if (text.Contains("<spe:Receivers"))
                                    {
                                        text = text.Replace("<spe:Receivers ", "<Receivers ").Replace("</spe:Receivers>", "</Receivers>");
                                        File.WriteAllText(fi.DirectoryName + "\\" + elem.Location, text);
                                    }
                                    using (FileStream elementStream = new FileStream(fi.DirectoryName + "\\" + elem.Location, FileMode.Open, FileAccess.Read))
                                    {
                                        using (XmlReader elementsReader = new XmlTextReader(elementStream))
                                        {
                                            XmlSerializer elementsSerializer = new XmlSerializer(typeof(ElementDefinitionCollection));

                                            try
                                            {
                                                ElementDefinitionCollection elements = (ElementDefinitionCollection)elementsSerializer.Deserialize(elementsReader);
                                                if (elements.Items != null)
                                                {
                                                    foreach (object oModuleDefinition in elements.Items)
                                                    {
                                                        switch (oModuleDefinition.GetType().ToString())
                                                        {
                                                            #region WorkFlowActions
                                                            case "SharePoint.SolutionAnalyzer.WorkflowActions":
                                                                {
                                                                    var actions = (oModuleDefinition as WorkflowActions);

                                                                    foreach(var action in actions.Action)
                                                                    {
                                                                        feature.WorkflowActionDetails.Add(action as WorkflowAction);
                                                                    }
                                                                    break;
                                                                }
                                                            #endregion
                                                            #region Web Templates
                                                            case "SharePoint.SolutionAnalyzer.WebTemplate":
                                                                {
                                                                    feature.WebTemplateDetails.Add(oModuleDefinition as WebTemplate);
                                                                    break;
                                                                }
                                                            #endregion
                                                            #region ContentType bindings
                                                            case "SharePoint.SolutionAnalyzer.ContentTypeBindingDefinition":
                                                                {
                                                                    feature.ContentTypeBindings.Add((ContentTypeBindingDefinition)oModuleDefinition);
                                                                    break;
                                                                }
                                                            #endregion
                                                            #region Shared fields
                                                            case "SharePoint.SolutionAnalyzer.SharedFieldDefinition":
                                                                {
                                                                    SharedFieldDefinition sharedField = (SharedFieldDefinition)oModuleDefinition;
                                                                    SharedFieldInfo sfi = new SharedFieldInfo();
                                                                    sfi.Name = sharedField.Name;
                                                                    sfi.Type = sharedField.Type;
                                                                    sfi.Description = sharedField.Description;
                                                                    sfi.ID = sharedField.ID;
                                                                    feature.SharedFields.Add(sfi);
                                                                    sfi.NeedsToBeUpdated = false;
                                                                    if (sharedField.SourceID != null)
                                                                    {
                                                                        if (sharedField.SourceID.EndsWith("/v3"))
                                                                        {
                                                                            sfi.NeedsToBeUpdated = true;
                                                                        }
                                                                    }

                                                                    break;
                                                                }
                                                            #endregion
                                                            #region ContentType definitions
                                                            case "SharePoint.SolutionAnalyzer.ContentTypeDefinition":
                                                                {
                                                                    ContentTypeDefinition ct = (ContentTypeDefinition)oModuleDefinition;

                                                                    #region ContentTypeEventReceivers
                                                                    if (ct.XmlDocuments != null)
                                                                    {
                                                                        foreach (XmlDocumentDefinition xmlDocumentDefinitions in ct.XmlDocuments.Items)
                                                                        {
                                                                            if (xmlDocumentDefinitions.Items != null)
                                                                            {
                                                                                foreach (XmlDocumentDefinitionReceivers xmlDocumentDefinitionReceivers in xmlDocumentDefinitions.Items)
                                                                                {
                                                                                    if (xmlDocumentDefinitionReceivers.Items != null)
                                                                                    {
                                                                                        foreach (XmlDocumentDefinitionReceiver xmlDocumentDefinitionReceiver in xmlDocumentDefinitionReceivers.Items)
                                                                                        {
                                                                                            string erName = string.Empty;
                                                                                            string erType = string.Empty;
                                                                                            string erAssembly = string.Empty;
                                                                                            string erSqNo = string.Empty;
                                                                                            string erClass = string.Empty;
                                                                                            if (xmlDocumentDefinitionReceiver.Items != null)
                                                                                            {
                                                                                                foreach (var item in xmlDocumentDefinitionReceiver.Items)
                                                                                                {
                                                                                                    switch (item.GetType().ToString())
                                                                                                    {
                                                                                                        case "SharePoint.SolutionAnalyzer.XmlDocumentDefinitionReceiverName":
                                                                                                            {
                                                                                                                XmlDocumentDefinitionReceiverName xmlDocumentDefinitionReceiverName = (XmlDocumentDefinitionReceiverName)item;
                                                                                                                erName = xmlDocumentDefinitionReceiverName.Value;
                                                                                                                break;
                                                                                                            }
                                                                                                        case "SharePoint.SolutionAnalyzer.XmlDocumentDefinitionReceiverType":
                                                                                                            {
                                                                                                                XmlDocumentDefinitionReceiverType xmlDocumentDefinitionReceiverType = (XmlDocumentDefinitionReceiverType)item;
                                                                                                                erType = xmlDocumentDefinitionReceiverType.Value;
                                                                                                                break;
                                                                                                            }
                                                                                                        case "SharePoint.SolutionAnalyzer.XmlDocumentDefinitionReceiverAssembly":
                                                                                                            {
                                                                                                                XmlDocumentDefinitionReceiverAssembly xmlDocumentDefinitionReceiverAssembly = (XmlDocumentDefinitionReceiverAssembly)item;
                                                                                                                erAssembly = xmlDocumentDefinitionReceiverAssembly.Value;
                                                                                                                break;
                                                                                                            }
                                                                                                        case "SharePoint.SolutionAnalyzer.XmlDocumentDefinitionReceiverSequenceNumber":
                                                                                                            {
                                                                                                                XmlDocumentDefinitionReceiverSequenceNumber xmlDocumentDefinitionReceiverSequenceNumber = (XmlDocumentDefinitionReceiverSequenceNumber)item;
                                                                                                                erSqNo = xmlDocumentDefinitionReceiverSequenceNumber.Value;
                                                                                                                break;
                                                                                                            }
                                                                                                        case "SharePoint.SolutionAnalyzer.XmlDocumentDefinitionReceiverClass":
                                                                                                            {
                                                                                                                XmlDocumentDefinitionReceiverClass xmlDocumentDefinitionReceiverClass = (XmlDocumentDefinitionReceiverClass)item;
                                                                                                                erClass = xmlDocumentDefinitionReceiverClass.Value;
                                                                                                                break;
                                                                                                            }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            if (!string.IsNullOrEmpty(erAssembly) && !string.IsNullOrEmpty(erClass))
                                                                                            {
                                                                                                EventReceiver ler = new EventReceiver();
                                                                                                ler.ListTemplateId = 0;

                                                                                                ler.Name = erName;
                                                                                                ler.Type = erType;
                                                                                                ler.ListUrl = Constants.NotApplicable;
                                                                                                ler.Assembly = erAssembly;
                                                                                                ler.ClassOfReceiver = erClass;
                                                                                                feature.EventReceivers.Add(ler);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    #endregion

                                                                    ContentTypeInfo cti = new ContentTypeInfo();
                                                                    cti.Name = ct.Name.ToString();

                                                                    string contentTypeID = ct.ID.ToString();
                                                                    string resource = string.Empty;
                                                                    string resourcefileName = string.Empty;
                                                                    string resourceKey = string.Empty;
                                                                    bool contentTypeIDModified = false;

                                                                    if (contentTypeID.Contains("$Resource"))
                                                                    {
                                                                        //Get Resource Info from ConetntTypeID
                                                                        resource = contentTypeID.Substring(0, contentTypeID.IndexOf(";"));
                                                                        if (resource != null)
                                                                        {
                                                                            if (resource.Contains(","))
                                                                            {
                                                                                //Get Resource file name
                                                                                resourcefileName = resource.Substring(resource.IndexOf(":") + 1, (resource.IndexOf(",") - resource.IndexOf(":")) - 1);
                                                                                //Get Resource Key
                                                                                resourceKey = resource.Substring(resource.IndexOf(",") + 1);
                                                                                if (resourcefileName != null)
                                                                                {
                                                                                    //Get all root files of solution into List
                                                                                    List<RootFileReference> lstRtFiles = sol.RootFiles.ToList();
                                                                                    //Get all related resource files
                                                                                    var resourceFiles = from lstRtFile in lstRtFiles
                                                                                                        where lstRtFile.Location.Contains(resourcefileName)
                                                                                                        select lstRtFile;
                                                                                    foreach (RootFileReference rtFile in resourceFiles)
                                                                                    {
                                                                                        try
                                                                                        {
                                                                                            ExtractContentTypeIDFromResourceFile(fi.Directory.Parent.FullName + "\\" + rtFile.Location, resourceKey, Path.GetFileName(solutionInfo.Name), ref contentTypeID, ref contentTypeIDModified);
                                                                                            if (contentTypeIDModified) { break; }
                                                                                        }
                                                                                        catch (Exception ex)
                                                                                        {
                                                                                            string s = ex.Message;
                                                                                            //ExceptionCSV_SolutionAnalyzer.WriteException(Path.GetFileName(solutionInfo.Name), "ProcessFeatureManifest", ex.Message, ex.ToString(), ex.GetType().ToString(), "case: CustomizationAnalyzer.ContentTypeDefinition");
                                                                                        }
                                                                                    }
                                                                                }
                                                                                if (!contentTypeIDModified)
                                                                                {
                                                                                    DirectoryInfo di = new DirectoryInfo(@"C:\Program Files\Common Files\microsoft shared\Web Server Extensions\15\Resources");
                                                                                    FileInfo[] files = di.GetFiles(resourcefileName + "*.resx");
                                                                                    foreach (var file in files)
                                                                                    {
                                                                                        try
                                                                                        {
                                                                                            ExtractContentTypeIDFromResourceFile(file.FullName, resourceKey, Path.GetFileName(solutionInfo.Name), ref contentTypeID, ref contentTypeIDModified);
                                                                                            if (contentTypeIDModified) { break; }
                                                                                        }
                                                                                        catch (Exception ex)
                                                                                        {
                                                                                            string s = ex.Message;
                                                                                            //ExceptionCSV_SolutionAnalyzer.WriteException(Path.GetFileName(solutionInfo.Name), "ProcessFeatureManifest", ex.Message, ex.ToString(), ex.GetType().ToString(), "case: CustomizationAnalyzer.ContentTypeDefinition");
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    cti.TypeID = contentTypeID;
                                                                    if (ct.ProgId != null &&
                                                                        ct.ProgId.Equals("SharePoint.DocumentSet"))
                                                                    {
                                                                        cti.IsDocumentSetContentType = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        cti.IsDocumentSetContentType = false;
                                                                    }
                                                                    try
                                                                    {
                                                                        if (ct.FieldRefs.Items != null)
                                                                        {
                                                                            for (int i = 0; i < ct.FieldRefs.Items.GetLength(0); i++)
                                                                            {
                                                                                CTFieldRefDefinition ctfrd = (CTFieldRefDefinition)ct.FieldRefs.Items[i];
                                                                                SharedFieldInfo sfi = new SharedFieldInfo();
                                                                                sfi.ID = ctfrd.ID;
                                                                                sfi.Name = ctfrd.Name;
                                                                                cti.SharedFields.Add(sfi);
                                                                            }
                                                                            cti.FieldCount = ct.FieldRefs.Items.GetLength(0);
                                                                        }
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        string s = ex.Message;
                                                                        //ExceptionCSV_SolutionAnalyzer.WriteException(Path.GetFileName(solutionInfo.Name), "ProcessFeatureManifest", ex.Message, ex.ToString(), ex.GetType().ToString(), "case: CustomizationAnalyzer.ContentTypeDefinition");
                                                                    }

                                                                    cti.Description = ct.Description;
                                                                    feature.ContentTypes.Add(cti);
                                                                    break;
                                                                }
                                                            #endregion
                                                            #region Modules
                                                            case "SharePoint.SolutionAnalyzer.ModuleDefinition":
                                                                {
                                                                    ModuleDefinition moduleDefinition = (ModuleDefinition)oModuleDefinition;
                                                                    if (moduleDefinition.Url != null)
                                                                    {
                                                                        if (moduleDefinition.Url.StartsWith("_catalogs/"))
                                                                        {
                                                                            foreach (FileDefinition fdef in moduleDefinition.File)
                                                                            {
                                                                                if (fdef.Url.ToLowerInvariant().EndsWith(".master"))
                                                                                {
                                                                                    solutionInfo.MasterpagesCount = solutionInfo.MasterpagesCount + 1;
                                                                                    MasterPageInfo mpInfo = new MasterPageInfo();
                                                                                    mpInfo.URL = moduleDefinition.Url + "/" + fdef.Url;
                                                                                    mpInfo.Name = fdef.Url;
                                                                                    feature.MasterPages.Add(mpInfo);
                                                                                }
                                                                                else if (fdef.Url.ToLowerInvariant().EndsWith(".aspx"))
                                                                                {
                                                                                    solutionInfo.PagelayoutsCount = solutionInfo.PagelayoutsCount + 1;
                                                                                    PageLayoutInfo plInfo =new PageLayoutInfo();
                                                                                    plInfo.URL = moduleDefinition.Url + "/" + fdef.Url;
                                                                                    plInfo.Name = fdef.Url;
                                                                                    feature.PageLayouts.Add(plInfo);
                                                                                }
                                                                                else if (fdef.Url.ToLowerInvariant().EndsWith(".stp"))
                                                                                {
                                                                                    CabType cb = GetStpType(fdef.Url);
                                                                                    if (cb == CabType.SiteTemplate)
                                                                                    {
                                                                                        solutionInfo.SiteTemplates.Add(fdef.Url);
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        solutionInfo.ListTemplates.Add(fdef.Url);
                                                                                    }
                                                                                }
                                                                                else if (fdef.Url.ToLowerInvariant().EndsWith(".dwp"))
                                                                                {

                                                                                }
                                                                                else if (fdef.Url.ToLowerInvariant().EndsWith(".webpart"))
                                                                                {
                                                                                }
                                                                                else if (fdef.Url.ToLowerInvariant().EndsWith(".ascx"))
                                                                                {
                                                                                    solutionInfo.UserControls.Add(fdef.Url);
                                                                                }
                                                                                else
                                                                                {
                                                                                    feature.Files.Add(fdef.Name);
                                                                                }
                                                                            }
                                                                        }
                                                                        else if (moduleDefinition.Url.Contains("Pages"))
                                                                        {
                                                                            foreach (FileDefinition fdef in moduleDefinition.File)
                                                                            {
                                                                                if (fdef.Url.ToLowerInvariant().EndsWith(".aspx"))
                                                                                {
                                                                                    PageInfo pInfo = new PageInfo();
                                                                                    pInfo.URL = "/Pages/" + fdef.Url;
                                                                                    pInfo.Name = fdef.Url;
                                                                                    feature.Pages.Add(pInfo);
                                                                                }
                                                                            }
                                                                        }
                                                                        //2013 Work Flow
                                                                        else if (moduleDefinition.Url.StartsWith("wfsvc/"))
                                                                        {
                                                                            //Get Count of File Tags
                                                                            int FileRecordsCount = 0;
                                                                            FileRecordsCount = moduleDefinition.File.Count();
                                                                            string wfDetailName = moduleDefinition.Name;
                                                                            string wfDetailDesc = Constants.NotApplicable;
                                                                            string wfDetailType = Constants.NotApplicable;
                                                                            string wfDetailScope = Constants.NotApplicable;
                                                                            string _WFNameComment = "WF Name = ModuleDefinition.Name";

                                                                            foreach (FileDefinition fdef in moduleDefinition.File)
                                                                            {
                                                                                WorkFlowDetail wfDetail = new WorkFlowDetail();

                                                                                wfDetail.WorkFlowVersion = "2013";
                                                                                wfDetail.WorkFlowComment = _WFNameComment;

                                                                                wfDetail.CodeBesideClass = Constants.NotApplicable;
                                                                                wfDetail.CodeBesideAssembly = Constants.NotApplicable;
                                                                                wfDetail.AssociationType = Constants.NotApplicable;

                                                                                if (fdef.Url.ToLower().Equals("workflow.xaml"))
                                                                                {
                                                                                    wfDetail.RestrictToScope = wfDetailScope;

                                                                                    foreach (PropertyValueAttributeDefinition WFprop in fdef.Items)
                                                                                    {
                                                                                        if (WFprop.Name.Equals("WSDisplayName"))
                                                                                        {
                                                                                            if (WFprop.Value != null && WFprop.Value != "")
                                                                                            {
                                                                                                wfDetailName = WFprop.Value.ToString();
                                                                                                wfDetail.Name = wfDetailName;
                                                                                                wfDetail.WorkFlowDisplayName = wfDetailName;
                                                                                                _WFNameComment = "WF Name = WSDisplayName";
                                                                                                wfDetail.WorkFlowComment = _WFNameComment;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                wfDetail.Name = wfDetailName;
                                                                                                wfDetail.WorkFlowDisplayName = wfDetailName;
                                                                                                _WFNameComment = "WF Name = ModuleDefinition.Name";
                                                                                                wfDetail.WorkFlowComment = _WFNameComment;
                                                                                            }
                                                                                        }
                                                                                        else if (WFprop.Name.Equals("WSDescription"))
                                                                                        {
                                                                                            if (WFprop.Value != null && WFprop.Value != "")
                                                                                            {
                                                                                                wfDetailDesc = WFprop.Value.ToString();
                                                                                                wfDetail.Description = wfDetailDesc;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                wfDetail.Description = wfDetailDesc;
                                                                                            }
                                                                                        }
                                                                                        else if (WFprop.Name.Equals("RestrictToType"))
                                                                                        {
                                                                                            if (WFprop.Value != null && WFprop.Value != "")
                                                                                            {
                                                                                                wfDetailType = WFprop.Value.ToString();
                                                                                                wfDetail.AssociationType = wfDetailType;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                wfDetail.AssociationType = wfDetailType;
                                                                                            }
                                                                                        }
                                                                                        else if (WFprop.Name.Equals("RestrictToScope"))
                                                                                        {
                                                                                            if (WFprop.Value != null && WFprop.Value != "")
                                                                                            {
                                                                                                wfDetailScope = WFprop.Value.ToString();
                                                                                                wfDetail.RestrictToScope = wfDetailScope;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                wfDetail.RestrictToScope = wfDetailScope;
                                                                                            }
                                                                                        }

                                                                                        //If There is only one File Tag
                                                                                        //Then Search for GUID as well
                                                                                        if (FileRecordsCount == 1)
                                                                                        {
                                                                                            if (WFprop.Name.Equals("WSGUID"))
                                                                                            {
                                                                                                if (WFprop.Value != null && WFprop.Value != "")
                                                                                                {
                                                                                                    if (WFprop.Value.Contains("{"))
                                                                                                    {
                                                                                                        string wfId = WFprop.Value.TrimStart('{');
                                                                                                        if (wfId.Contains("}"))
                                                                                                        {
                                                                                                            wfId = wfId.TrimEnd('}');
                                                                                                        }
                                                                                                        wfDetail.Id = wfId;
                                                                                                    }
                                                                                                    else
                                                                                                        wfDetail.Id = WFprop.Value.ToString();
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    wfDetail.Id = Constants.NotApplicable;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    //If There is only one File Tag
                                                                                    if (FileRecordsCount == 1)
                                                                                    {
                                                                                        feature.WorkFlowDetails.Add(wfDetail);
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    foreach (PropertyValueAttributeDefinition WFprop in fdef.Items)
                                                                                    {
                                                                                        wfDetail.RestrictToScope = wfDetailScope;
                                                                                        wfDetail.AssociationType = wfDetailType;
                                                                                        wfDetail.Description = wfDetailDesc;
                                                                                        wfDetail.Name = wfDetailName;
                                                                                        wfDetail.WorkFlowComment = _WFNameComment + ", WF : Associated File Tag";

                                                                                        if (WFprop.Name.Equals("WSGUID"))
                                                                                        {
                                                                                            if (WFprop.Value != null && WFprop.Value != "")
                                                                                            {
                                                                                                if (WFprop.Value.Contains("{"))
                                                                                                {
                                                                                                    string wfId = WFprop.Value.TrimStart('{');
                                                                                                    if (wfId.Contains("}"))
                                                                                                    {
                                                                                                        wfId = wfId.TrimEnd('}');
                                                                                                    }
                                                                                                    wfDetail.Id = wfId;
                                                                                                }
                                                                                                else
                                                                                                    wfDetail.Id = WFprop.Value.ToString();
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                wfDetail.Id = Constants.NotApplicable;
                                                                                            }
                                                                                        }
                                                                                        else if (WFprop.Name.Equals("WSDisplayName"))
                                                                                        {
                                                                                            if (WFprop.Value != null && WFprop.Value != "")
                                                                                            {
                                                                                                wfDetail.WorkFlowDisplayName = WFprop.Value.ToString();
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                wfDetail.WorkFlowDisplayName = Constants.NotApplicable;
                                                                                            }
                                                                                        }
                                                                                    }

                                                                                    feature.WorkFlowDetails.Add(wfDetail);
                                                                                }
                                                                            }
                                                                            //
                                                                            break;
                                                                        }
                                                                    }

                                                                    break;
                                                                }
                                                            #endregion
                                                            #region Not implemented
                                                            case "SharePoint.SolutionAnalyzer.CustomActionDefinition":
                                                            case "SharePoint.SolutionAnalyzer.CustomActionGroupDefinition":
                                                            case "SharePoint.SolutionAnalyzer.FeatureSiteTemplateAssociationDefinition":
                                                                {
                                                                    break;
                                                                }
                                                            #endregion
                                                            #region Workflow definitions
                                                            case "SharePoint.SolutionAnalyzer.WorkflowDefinition":
                                                                {
                                                                    WorkflowDefinition wfInstance = (WorkflowDefinition)oModuleDefinition;

                                                                    WorkFlowDetail wfDetail = new WorkFlowDetail();
                                                                    wfDetail.Name = wfInstance.Name;
                                                                    wfDetail.WorkFlowDisplayName = wfInstance.Name;
                                                                    wfDetail.Description = wfInstance.Description;
                                                                    wfDetail.WorkFlowVersion = "2010";
                                                                    wfDetail.WorkFlowComment = "2010 Work Flow";
                                                                    wfDetail.RestrictToScope = Constants.NotApplicable;

                                                                    if (wfInstance.Id.Contains("{"))
                                                                    {
                                                                        string wfId = wfInstance.Id.TrimStart('{');
                                                                        if (wfId.Contains("}"))
                                                                            wfId = wfId.TrimEnd('}');
                                                                        wfDetail.Id = wfId;
                                                                    }
                                                                    else
                                                                        wfDetail.Id = wfInstance.Id;

                                                                    XmlNode[] nodeList = (XmlNode[])wfInstance.MetaData;
                                                                    XmlNode WorkflowNode = nodeList[0];
                                                                    string wfType = WorkflowNode.InnerText;
                                                                    wfDetail.AssociationType = wfType;

                                                                    //wfDetail.CodeBesideAssembly = wfInstance.CodeBesideAssembly;
                                                                    string strrWFDetailCodeBesideAssembly = wfInstance.CodeBesideAssembly.ToString();
                                                                    if (strrWFDetailCodeBesideAssembly != "")
                                                                    {
                                                                        //CodeBesideAssembly="SharePointProject3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=2820397d14c7c227">
                                                                        string[] assemblyAttribArray = strrWFDetailCodeBesideAssembly.Split(',');
                                                                        if (assemblyAttribArray != null)
                                                                        {
                                                                            if (assemblyAttribArray.Length > 0)
                                                                            {
                                                                                wfDetail.CodeBesideAssembly = assemblyAttribArray[0];
                                                                            }
                                                                        }
                                                                    }
                                                                    wfDetail.CodeBesideClass = wfInstance.CodeBesideClass;
                                                                    wfDetail.AssociationUrl = wfInstance.AssociationUrl;

                                                                    feature.WorkFlowDetails.Add(wfDetail);
                                                                    break;

                                                                }
                                                            #endregion
                                                            #region List instances
                                                            case "SharePoint.SolutionAnalyzer.ListInstanceDefinition":
                                                                {
                                                                    ListInstanceDefinition listInstance =(ListInstanceDefinition)oModuleDefinition;
                                                                    ListInst li = new ListInst();
                                                                    li.Description = listInstance.Description;
                                                                    li.TemplateType = listInstance.TemplateType.ToString();
                                                                    li.URL = listInstance.Url;
                                                                    li.Title = listInstance.Title;

                                                                    feature.ListInsts.Add(li);
                                                                    break;
                                                                }
                                                            #endregion
                                                            #region List definitions
                                                            case "SharePoint.SolutionAnalyzer.ListTemplateDefinition":
                                                                {
                                                                    ListTemplateDefinition listTemplateDefinition = (ListTemplateDefinition)oModuleDefinition;
                                                                    ListTemplate lt = new ListTemplate();
                                                                    lt.BaseType = listTemplateDefinition.BaseType.ToString();
                                                                    lt.Description = listTemplateDefinition.Description;
                                                                    lt.Name = listTemplateDefinition.Name;
                                                                    lt.Type = listTemplateDefinition.Type.ToString();
                                                                    lt.DisplayName = listTemplateDefinition.DisplayName;

                                                                    feature.ListTemplates.Add(lt);
                                                                    break;
                                                                }
                                                            #endregion
                                                            #region Event receivers
                                                            case "SharePoint.SolutionAnalyzer.ReceiverDefinitionCollection":
                                                                {
                                                                    ReceiverDefinitionCollection receiverDefinitionCollection = (ReceiverDefinitionCollection)oModuleDefinition;
                                                                    foreach (ReceiverDefinition receiverDefinition in receiverDefinitionCollection.Receiver)
                                                                    {
                                                                        EventReceiver ler = new EventReceiver();
                                                                        ler.ListTemplateId = receiverDefinitionCollection.ListTemplateId;
                                                                        ler.Name = receiverDefinition.Name;
                                                                        ler.Type = receiverDefinition.Type;
                                                                        ler.ListUrl = receiverDefinitionCollection.ListUrl;
                                                                        ler.Assembly = receiverDefinition.Assembly;
                                                                        ler.ClassOfReceiver = receiverDefinition.Class;
                                                                        feature.EventReceivers.Add(ler);
                                                                    }
                                                                    break;
                                                                }
                                                            #endregion
                                                            default:
                                                                {
                                                                    if (_verbose) { Console.WriteLine("{0} - {1}", fileName, oModuleDefinition); }
                                                                    break;
                                                                }
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception featEx)
                                            {
                                                string comments = string.Empty;
                                                if (_verbose)
                                                {
                                                    Console.ForegroundColor = ConsoleColor.White;
                                                    Console.BackgroundColor = ConsoleColor.Red;
                                                    Console.WriteLine("Error when processing feature manifest. Error:{0}",
                                                        featEx.Message);

                                                    Console.ResetColor();
                                                }
                                                comments = "File Name: " + fileName + " Error when processing feature manifest. Error: " + featEx.Message;
                                                //ExceptionCSV_SolutionAnalyzer.ExceptionComments = Constants.NotApplicable;

                                                Exception ex = featEx.InnerException;
                                                while (ex != null)
                                                {
                                                    if (ex != null)
                                                    {
                                                        if (_verbose)
                                                        {
                                                            Console.WriteLine("Exception error: " + ex.Message);
                                                        }
                                                        comments = comments + "@@@###" + ex.Message;
                                                    }
                                                    ex = ex.InnerException;
                                                }

                                                //ExceptionCSV_SolutionAnalyzer.WriteException(Path.GetFileName(solutionInfo.Name), "ProcessFeatureManifest", featEx.Message, featEx.ToString(), featEx.GetType().ToString(), comments);
                                            }
                                        }
                                    }
                                }
                            }


                            if (elems.ItemFiles != null)
                            {
                                foreach (ElementManifestReference elemFile in elems.ItemFiles)
                                {
                                    if (elemFile.Location.ToLowerInvariant().EndsWith(".webpart") ||
                                        elemFile.Location.ToLowerInvariant().EndsWith(".dwp"))
                                    {
                                        string type = GetTypeOfWebPart(fi.DirectoryName + "\\" + elemFile.Location);
                                        WebPartInfo wp = new WebPartInfo();
                                        wp.File = elemFile.Location;
                                        wp.Type = type;

                                        feature.WebParts.Add(wp);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception serializationException)
                    {
                        if (_verbose)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error when processing feature manifest. Error:{0}",
                                serializationException.Message);
                            Console.ResetColor();
                        }
                        if (serializationException.InnerException != null)
                        {
                            Exception ex = serializationException.InnerException;
                            while (ex != null)
                            {
                                if (_verbose) { Console.WriteLine("Exception error: " + ex.Message); }
                                ex = ex.InnerException;
                            }
                        }

                        //ExceptionCSV_SolutionAnalyzer.WriteException(Path.GetFileName(solutionInfo.Name), "ProcessFeatureManifest", serializationException.Message, serializationException.ToString(), serializationException.GetType().ToString(), "N/A");


                    }
                }
            }
        }

        /// <summary>
        /// Process WF .Actions/.Actions4 file
        /// </summary>
        private void ProcessWorkFlowActions(string fileName, SolutionInformation solutionInfo)
        {
            if (!File.Exists(fileName))
            {
                if (_verbose) { Console.WriteLine("The xml file {0} could not be found", fileName); }
                //ExceptionCSV_SolutionAnalyzer.WriteException(Path.GetFileName(solutionInfo.Name), "ProcessWorkFlowActions", "The xml file " + fileName + " could not be found", Constants.NotApplicable, "Custom", Constants.NotApplicable);
                return;
            }
            //// A FileStream is needed to read the XML document.
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                FileInfo fi = new FileInfo(fileStream.Name);
                string _FileExtension = fi.Extension;

                //ExceptionCSV_SolutionAnalyzer.ExceptionComments = fileStream.Name;

                using (XmlReader reader = new XmlTextReader(fileStream))
                {

                    string strXmlContent = reader.ReadInnerXml();

                    //[START]File Type: .ACTIONS
                    if (_FileExtension.ToUpper() == ".ACTIONS")
                    {
                        if (reader.IsStartElement("WorkflowInfo") || strXmlContent.Contains("WorkflowInfo"))
                        {
                            // Declare an object variable of the type to be deserialized.
                            // Create an instance of the XmlSerializer specifying type and namespace.
                            XmlSerializer serializer = new XmlSerializer(typeof(WorkflowInfo));
                            WorkflowInfo workflowInfo = new WorkflowInfo();

                            try
                            {
                                workflowInfo = (WorkflowInfo)serializer.Deserialize(reader);

                                if (workflowInfo.Actions != null && workflowInfo.Actions.Action.Length > 0)
                                {
                                    foreach (WorkflowInfoActionsAction action in workflowInfo.Actions.Action)
                                    {
                                        try
                                        {
                                            ActionDetail actionDetail = new ActionDetail();

                                            actionDetail.ActionXMLFile = fileName;
                                            actionDetail.Name = action.Name;
                                            actionDetail.ClassName = action.ClassName;

                                            string strActionAssembly = action.Assembly.ToString();
                                            if (strActionAssembly != "")
                                            {
                                                //Assembly="<AssemblyName>, Version=1.0.0.0, Culture=neutral, PublicKeyToken=0298457208daed83"
                                                string[] assemblyAttribArray = strActionAssembly.Split(',');
                                                if (assemblyAttribArray != null)
                                                {
                                                    if (assemblyAttribArray.Length > 0)
                                                    {
                                                        actionDetail.Assembly = assemblyAttribArray[0];
                                                    }
                                                }
                                                //END
                                            }

                                            actionDetail.AppliesTo = action.AppliesTo;
                                            actionDetail.Category = action.Category;

                                            solutionInfo.ActionDetails.Add(actionDetail);
                                        }
                                        catch (Exception featEx)
                                        {
                                            if (_verbose)
                                            {
                                                Console.ForegroundColor = ConsoleColor.White;
                                                Console.BackgroundColor = ConsoleColor.Red;
                                                Console.WriteLine("Error when processing ACTIONS XML. Error:{0}",
                                                    featEx.Message);
                                                Console.ResetColor();
                                            }
                                            Exception ex = featEx.InnerException;
                                            while (ex != null)
                                            {
                                                if (_verbose) { Console.WriteLine("Exception error: " + ex.Message); }
                                                ex = ex.InnerException;
                                            }

                                            //ExceptionCSV_SolutionAnalyzer.WriteException(Path.GetFileName(solutionInfo.Name), "ProcessWorkflowActions", ex.Message, ex.ToString(), ex.GetType().ToString(), "N/A");
                                        }
                                    }
                                }
                            }
                            catch (Exception serializationException)
                            {
                                string comments = string.Empty;
                                //ExceptionCSV_SolutionAnalyzer.ExceptionComments = Constants.NotApplicable;

                                comments = comments + "@@@###" + " Error when processing ACTIONS XML. Error: " + serializationException.Message;
                                if (_verbose)
                                {
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.BackgroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Error when processing ACTIONS XML. Error:{0}",
                                        serializationException.Message);
                                    Console.ResetColor();
                                }
                                if (serializationException.InnerException != null)
                                {
                                    Exception ex = serializationException.InnerException;
                                    while (ex != null)
                                    {
                                        if (ex != null)
                                        {
                                            if (_verbose) { Console.WriteLine("Exception error: " + ex.Message); }
                                            comments = comments + "@@@###" + " Exception error: " + ex.Message;
                                        }
                                        ex = ex.InnerException;
                                    }
                                }

                                //ExceptionCSV_SolutionAnalyzer.WriteException(Path.GetFileName(solutionInfo.Name), "ProcessWorkflowActions", serializationException.Message, serializationException.ToString(), serializationException.GetType().ToString(), comments);
                            }
                        }
                    }
                    //[END]File Type: .ACTIONS
                    //[START]File Type: .ACTIONS4
                    else if (_FileExtension.ToUpper() == ".ACTIONS4")
                    {
                        if (reader.IsStartElement("Action") || strXmlContent.Contains("Action"))
                        {
                            WorkflowInfoActionsAction workflowAction = new WorkflowInfoActionsAction();

                            try
                            {
                                XmlDocument xmlDoc = new XmlDocument();
                                xmlDoc.Load(reader);
                                XmlNode node = xmlDoc.SelectSingleNode("Action");
                                XmlElement Xe = (XmlElement)node;

                                ActionDetail actionDetail = new ActionDetail();

                                actionDetail.ActionXMLFile = fileName;
                                actionDetail.Name = Xe.GetAttribute("Name");
                                actionDetail.ClassName = Xe.GetAttribute("ClassName");

                                string strActionAssembly = Xe.GetAttribute("Assembly").ToString();
                                if (strActionAssembly != "")
                                {
                                    //Assembly="<AssemblyName>, Version=1.0.0.0, Culture=neutral, PublicKeyToken=0298457208daed83"
                                    string[] assemblyAttribArray = strActionAssembly.Split(',');
                                    if (assemblyAttribArray != null)
                                    {
                                        if (assemblyAttribArray.Length > 0)
                                        {
                                            actionDetail.Assembly = assemblyAttribArray[0];
                                        }
                                    }
                                    //END
                                }

                                actionDetail.AppliesTo = Xe.GetAttribute("AppliesTo");
                                actionDetail.Category = Xe.GetAttribute("Category");

                                solutionInfo.ActionDetails.Add(actionDetail);
                            }
                            catch (Exception serializationException)
                            {
                                if (_verbose)
                                {
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.BackgroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Error when processing ACTIONS4 XML. Error:{0}",
                                        serializationException.Message);
                                    Console.ResetColor();
                                }
                                if (serializationException.InnerException != null)
                                {
                                    Exception ex = serializationException.InnerException;
                                    while (ex != null)
                                    {
                                        if (_verbose) { Console.WriteLine("Exception error: " + ex.Message); }
                                        ex = ex.InnerException;
                                    }
                                }
                               //ExceptionCSV_SolutionAnalyzer.WriteException(Path.GetFileName(solutionInfo.Name), "ProcessWorkflowActions", serializationException.Message, serializationException.ToString(), serializationException.GetType().ToString(), "N/A");
                            }
                        }
                    }
                    //[END]File Type: .ACTIONS4
                }
            }
        }

        /// <summary>
        /// Process custom fieldtype xml
        /// </summary>
        private void ProcessFieldType(string fileName, SolutionInformation solutionInfo)
        {
            if (!File.Exists(fileName))
            {
                if (_verbose) { Console.WriteLine("The xml file {0} could not be found", fileName); }
                //ExceptionCSV_SolutionAnalyzer.WriteException(Path.GetFileName(solutionInfo.Name), "ProcessFieldType", "The xml file " + fileName + " could not be found", Constants.NotApplicable, "Custom", Constants.NotApplicable);
                return;
            }
            //// A FileStream is needed to read the XML document.
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                FileInfo fi = new FileInfo(fileStream.Name);
                using (XmlReader reader = new XmlTextReader(fileStream))
                {
                    string strXmlContent = reader.ReadInnerXml();
                    if (reader.IsStartElement("FieldTypes") || strXmlContent.Contains("FieldTypes"))
                    {
                        // Declare an object variable of the type to be deserialized.
                        // Create an instance of the XmlSerializer specifying type and namespace.
                        XmlSerializer serializer = new XmlSerializer(typeof(FieldTypes));
                        FieldTypes fieldTypes = new FieldTypes();

                        try
                        {
                            fieldTypes = (FieldTypes)serializer.Deserialize(reader);

                            if (fieldTypes.FieldType != null && fieldTypes.FieldType.Length > 0)
                            {
                                if (fieldTypes != null)
                                {
                                    foreach (FieldTypesFieldType fieldType in fieldTypes.FieldType)
                                    {
                                        try
                                        {
                                            if (fieldType.Field != null && fieldType.Field.Length > 0)
                                            {
                                                CustomFieldType customFieldType = new CustomFieldType();
                                                customFieldType.FieldTypeXMLFile = fileName;

                                                foreach (FieldTypesFieldTypeField field in fieldType.Field)
                                                {
                                                    switch (field.Name)
                                                    {
                                                        case FieldTypeNameEnum.AllowBaseTypeRendering:
                                                            {
                                                                break;
                                                            }
                                                        case FieldTypeNameEnum.CAMLRendering:
                                                            {
                                                                break;
                                                            }
                                                        case FieldTypeNameEnum.FieldEditorUserControl:
                                                            {
                                                                break;
                                                            }
                                                        case FieldTypeNameEnum.FieldTypeClass:
                                                            {
                                                                customFieldType.FieldTypeClass = field.Value;
                                                                break;
                                                            }
                                                        case FieldTypeNameEnum.Filterable:
                                                            {
                                                                break;
                                                            }
                                                        case FieldTypeNameEnum.InternalType:
                                                            {
                                                                break;
                                                            }
                                                        case FieldTypeNameEnum.ParentType:
                                                        case FieldTypeNameEnum.BaseRenderingTypeName:
                                                            {
                                                                customFieldType.ParentType = field.Value;
                                                                break;
                                                            }
                                                        case FieldTypeNameEnum.ShowOnColumnTemplateAuthoringPages:
                                                        case FieldTypeNameEnum.ShowOnColumnTemplateCreate:
                                                        case FieldTypeNameEnum.ShowInColumnTemplateCreate:
                                                            {
                                                                break;
                                                            }
                                                        case FieldTypeNameEnum.ShowOnDocumentLibraryAuthoringPages:
                                                        case FieldTypeNameEnum.ShowOnDocumentLibraryCreate:
                                                        case FieldTypeNameEnum.ShowInDocumentLibraryCreate:
                                                            {
                                                                break;
                                                            }
                                                        case FieldTypeNameEnum.ShowOnListAuthoringPages:
                                                        case FieldTypeNameEnum.ShowOnListCreate:
                                                        case FieldTypeNameEnum.ShowInListCreate:
                                                            {
                                                                break;
                                                            }
                                                        case FieldTypeNameEnum.ShowOnSurveyAuthoringPages:
                                                        case FieldTypeNameEnum.ShowOnSurveyCreate:
                                                        case FieldTypeNameEnum.ShowInSurveyCreate:
                                                            {
                                                                break;
                                                            }
                                                        case FieldTypeNameEnum.Sortable:
                                                            {
                                                                break;
                                                            }
                                                        case FieldTypeNameEnum.SQLType:
                                                            {
                                                                break;
                                                            }
                                                        case FieldTypeNameEnum.TypeDisplayName:
                                                            {
                                                                customFieldType.TypeDisplayName = field.Value;
                                                                break;
                                                            }
                                                        case FieldTypeNameEnum.TypeName:
                                                            {
                                                                customFieldType.TypeName = field.Value;
                                                                break;
                                                            }
                                                        case FieldTypeNameEnum.TypeShortDescription:
                                                            {
                                                                customFieldType.TypeShortDescription = field.Value;
                                                                break;
                                                            }
                                                        case FieldTypeNameEnum.UserCreatable:
                                                            {
                                                                break;
                                                            }
                                                    }
                                                }
                                                solutionInfo.CustomFieldTypes.Add(customFieldType);
                                            }
                                        }
                                        catch (Exception featEx)
                                        {
                                            if (_verbose)
                                            {
                                                Console.ForegroundColor = ConsoleColor.White;
                                                Console.BackgroundColor = ConsoleColor.Red;
                                                Console.WriteLine("Error when processing FieldType XML. Error:{0}",
                                                    featEx.Message);
                                                Console.ResetColor();
                                            }
                                            Exception ex = featEx.InnerException;
                                            while (ex != null)
                                            {
                                                if (_verbose) { Console.WriteLine("Exception error: " + ex.Message); }
                                                ex = ex.InnerException;
                                            }

                                            //ExceptionCSV_SolutionAnalyzer.WriteException(Path.GetFileName(solutionInfo.Name), "ProcessFieldType", featEx.Message, featEx.ToString(), featEx.GetType().ToString(), "N/A");
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception serializationException)
                        {
                            if (_verbose)
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.BackgroundColor = ConsoleColor.Red;
                                Console.WriteLine("Error when processing FieldType XML. Error:{0}",
                                    serializationException.Message);
                                Console.ResetColor();
                            }
                            if (serializationException.InnerException != null)
                            {
                                Exception ex = serializationException.InnerException;
                                while (ex != null)
                                {
                                    if (_verbose) { Console.WriteLine("Exception error: " + ex.Message); }
                                    ex = ex.InnerException;
                                }
                            }
                            //ExceptionCSV_SolutionAnalyzer.WriteException(Path.GetFileName(solutionInfo.Name), "ProcessFieldType", serializationException.Message, serializationException.ToString(), serializationException.GetType().ToString(), "N/A");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Process ONet.xml file
        /// </summary>
        public void ProcessOnet(string fileName, SiteDefinitionInformation sdi)
        {
            try
            {
                //// A FileStream is needed to read the XML document.
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    FileInfo fi = new FileInfo(fileStream.Name);
                    bool inSiteFeatures = false;
                    bool inWebFeatures = false;

                    using (StreamReader reader2 = new StreamReader(fileStream))
                    {
                        while (!reader2.EndOfStream)
                        {
                            string s = reader2.ReadLine().Trim();
                            if (String.Compare(s, "<sitefeatures>", true) == 0)
                            {
                                inSiteFeatures = true;
                                continue;
                            }

                            if (String.Compare(s, "</sitefeatures>", true) == 0)
                            {
                                inSiteFeatures = false;
                                continue;
                            }

                            if (String.Compare(s, "<webfeatures>", true) == 0)
                            {
                                inWebFeatures = true;
                                continue;
                            }

                            if (String.Compare(s, "</webfeatures>", true) == 0)
                            {
                                inWebFeatures = false;
                                continue;
                            }

                            if (inSiteFeatures || inWebFeatures)
                            {
                                int pos = s.IndexOf("Feature ID=\"");
                                if (pos > 0)
                                {
                                    string featId = s.Substring(pos + 12);
                                    pos = featId.IndexOf("\"");
                                    featId = featId.Substring(0, pos);
                                    if (inSiteFeatures)
                                    {
                                        sdi.SiteFeatures.Add(featId);
                                    }
                                    else
                                        sdi.WebFeatures.Add(featId);
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (_verbose)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error when processing onet.xml. Error:{0}", ex.Message);
                    Console.ResetColor();
                }
                //ExceptionCSV_SolutionAnalyzer.WriteException(fileName, "ProcessOnet", ex.Message, ex.ToString(), ex.GetType().ToString(), "N/A");

            }
        }

        /// <summary>
        /// Get type of STP file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private CabType GetStpType(string fileName)
        {
            CabType ct = CabType.SiteTemplate;
            string cmd = "/e /a /y /L \"" + fileName.Replace(".", "_") + "\" \"" + fileName + "\"";
            ProcessStartInfo pI = new ProcessStartInfo("extrac32.exe", cmd);

            Process p = Process.Start(pI);
            p.WaitForExit();
            ct = GetCabType(fileName.Replace(".", "_") + "\\manifest.xml");

            return ct;
        }

        /// <summary>
        /// Get type of cab file (webpart, sitetemplate, listtemplate)
        /// </summary>
        private CabType GetCabType(string fileName)
        {
            CabType ct = CabType.SiteTemplate;

            string type = String.Empty;

            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                FileInfo fi = new FileInfo(fileStream.Name);
                using (StreamReader reader2 = new StreamReader(fileStream))
                {
                    while (!reader2.EndOfStream)
                    {
                        string s = reader2.ReadLine().TrimStart();
                        if (s.StartsWith("<WebPartManife"))
                        {
                            ct = CabType.WebPart;
                            break;
                        }

                        if (s.StartsWith("<Web "))
                        {
                            ct = CabType.SiteTemplate;
                            break;
                        }

                        if (s.StartsWith("<ListTemplate"))
                        {
                            ct = CabType.ListTemplate;
                            break;
                        }
                    }
                }
            }

            return ct;
        }

        /// <summary>
        /// Process cab file
        /// </summary>
        /// <returns></returns>
        public bool ProcessCabFile(string fileName, SolutionInformation solutionInfo)
        {
            bool returnValue = true;
            try
            {
                if (File.Exists("manifest.xml"))
                {
                    CabType ct = GetCabType(fileName);

                    switch (ct)
                    {
                        case CabType.WebPart:
                            {
                                WebPartManifest wpm = new WebPartManifest();

                                //// A FileStream is needed to read the XML document.
                                using (FileStream fs = new FileStream("manifest.xml", FileMode.Open, FileAccess.Read))
                                {
                                    using (XmlReader reader = new XmlTextReader(fs))
                                    {
                                        // Declare an object variable of the type to be deserialized.
                                        // Create an instance of the XmlSerializer specifying type and namespace.
                                        XmlSerializer serializer = new XmlSerializer(typeof(WebPartManifest));

                                        wpm = (WebPartManifest)serializer.Deserialize(reader);
                                        foreach (object o in wpm.DWPFiles)
                                        {
                                            WebPartManifestDwpFiles dwpFiles = (WebPartManifestDwpFiles)o;
                                            foreach (WebPartManifestDwpFilesDwpFile dwpFile in dwpFiles.DwpFiles)
                                            {
                                                WebPartInfo wp = new WebPartInfo();
                                                wp.File = dwpFile.FileName;
                                                wp.Type = GetTypeOfWebPart(dwpFile.FileName);
                                                solutionInfo.WebParts.Add(wp);

                                            }
                                        }
                                        foreach (object o in wpm.Assemblies)
                                        {
                                            WebPartManifestAssemblies assemblies = (WebPartManifestAssemblies)o;
                                            foreach (WebPartManifestAssembliesAssembly assembly in assemblies.Assemblies)
                                            {
                                                AssemblyInfo assInfo = new AssemblyInfo();
                                                AssemblyFileReference afr = new AssemblyFileReference();
                                                afr.Location = assembly.FileName;
                                                ProcessAssembly(afr, solutionInfo, Path.GetDirectoryName(fileName));
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                        case CabType.SiteTemplate:
                            {
                                //// A FileStream is needed to read the XML document.
                                using (FileStream fs = new FileStream("manifest.xml", FileMode.Open, FileAccess.Read))
                                {
                                    using (StreamReader reader = new StreamReader(fs))
                                    {
                                        using (StreamReader reader2 = new StreamReader(fs))
                                        {
                                            while (!reader2.EndOfStream)
                                            {
                                                string s = reader2.ReadLine().TrimStart();
                                                if (s.StartsWith("<TemplateTitle>"))
                                                {
                                                    s = s.Substring(s.IndexOf(">") + 1);
                                                    s = s.Substring(0, s.IndexOf("<"));
                                                    solutionInfo.SiteTemplates.Add(s);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                        case CabType.ListTemplate:
                            {
                                //// A FileStream is needed to read the XML document.
                                using (FileStream fs = new FileStream("manifest.xml", FileMode.Open, FileAccess.Read))
                                {
                                    using (StreamReader reader = new StreamReader(fs))
                                    {
                                        using (StreamReader reader2 = new StreamReader(fs))
                                        {
                                            while (!reader2.EndOfStream)
                                            {
                                                string s = reader2.ReadLine().TrimStart();
                                                if (s.StartsWith("<TemplateTitle>"))
                                                {
                                                    s = s.Substring(s.IndexOf(">") + 1);
                                                    s = s.Substring(0, s.IndexOf("<"));
                                                    solutionInfo.ListTemplates.Add(s);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue = false;
                solutionInfo.Error = ex.Message;

                //ExceptionCSV_SolutionAnalyzer.WriteException(Path.GetFileName(solutionInfo.Name), "ProcessCabFile", ex.Message, ex.ToString(), ex.GetType().ToString(), "N/A");
            }

            return returnValue;
        }

        /// <summary>
        /// Process WSP
        /// </summary>
        public void ProcessWsp(string fileName, SolutionInformation solutionInfo)
        {
            string folderWeExtractedTo = "";
            try
            {
                Hashtable ht = new Hashtable();
                FileInfo fi = new FileInfo(fileName);
                solutionInfo.FileSize = (int)fi.Length;
                solutionInfo.FileDate = fi.CreationTime.ToString();

                if (_verbose) { Console.WriteLine("Processing wsp :{0}", fileName); }

                folderWeExtractedTo = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().Replace("-", ""));

                UnCab(fileName, folderWeExtractedTo);

                if (File.Exists(Path.Combine(folderWeExtractedTo, "manifest.xml")))
                {
                    //// A FileStream is needed to read the XML document.
                    using (FileStream fs = new FileStream(Path.Combine(folderWeExtractedTo, "manifest.xml"), FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        FileInfo fiManifest = new FileInfo(fs.Name);
                        
                        using (XmlReader reader = new XmlTextReader(fs))
                        {
                            // Declare an object variable of the type to be deserialized.
                            // Create an instance of the XmlSerializer specifying type and namespace.
                            XmlSerializer serializer = new XmlSerializer(typeof(Solution));

                            try
                            {
                                Solution sol = new Solution();
                                // Use the Deserialize method to restore the object's state.
                                sol = (Solution)serializer.Deserialize(reader);

                                if (sol.SiteDefinitionManifests != null)
                                {
                                    foreach (SiteDefinitionManifestFileReference siteDef in sol.SiteDefinitionManifests)
                                    {
                                        //siteDefinitions.Add(siteDef.Location);
                                        SiteDefinitionInformation sd = new SiteDefinitionInformation();

                                        sd.Name = siteDef.Location;
                                        sd.NrOfLocales = siteDef.WebTempFile.GetLength(0);
                                        ProcessOnet(Path.Combine(folderWeExtractedTo, siteDef.WebTempFile[0].Location), sd);
                                        solutionInfo.SiteDefinitions.Add(sd);
                                    }
                                }

                                if (sol.FeatureManifests != null)
                                {
                                    foreach (FeatureManifestReference feat in sol.FeatureManifests)
                                    {
                                        _numberOfFeatures = _numberOfFeatures + 1;
                                        ProcessFeatureManifest(Path.Combine(folderWeExtractedTo, feat.Location), solutionInfo, sol);
                                    }
                                }

                                if (sol.Assemblies != null)
                                {
                                    foreach (AssemblyFileReference assembly in sol.Assemblies)
                                    {
                                        ProcessAssembly(assembly, solutionInfo, folderWeExtractedTo);
                                    }
                                }

                                // Look for InfoPath solutions
                                if (solutionInfo.AssemblyCount > 0)
                                {
                                    var InfoPathDependency = solutionInfo.Assemblies.Where(s => s.ReferencedAssemblies.Contains("Microsoft.Office.InfoPath") &&
                                                                                                s.ReferencedAssemblies.Contains("Microsoft.VisualStudio.Tools.Applications.Contract") &&
                                                                                                s.ReferencedAssemblies.Contains("System.AddIn.Contract")).FirstOrDefault();
                                    if (InfoPathDependency != null)
                                    {
                                        solutionInfo.InfoPathSolution = true;
                                    }
                                    else
                                    {
                                        solutionInfo.InfoPathSolution = false;
                                    }
                                }


                                if (sol.TemplateFiles != null)
                                {
                                    SiteDefinitionInformation sdi = new SiteDefinitionInformation();
                                    string lastWebTemp = String.Empty;
                                    foreach (TemplateFileReference files in sol.TemplateFiles)
                                    {
                                        string key = files.Location.Substring(0, files.Location.IndexOf("\\"));
                                        if (ht.ContainsKey(key))
                                        {
                                            int count = (int)ht[key] + 1;
                                            ht[key] = count;
                                        }
                                        else
                                        {
                                            ht.Add(key, 1);
                                        }

                                        if (files.Location.ToLowerInvariant().EndsWith(".aspx"))
                                        {
                                            solutionInfo.AspxPages.Add(files.Location);
                                        }
                                        else if (files.Location.ToLowerInvariant().EndsWith(".ascx"))
                                        {
                                            solutionInfo.UserControls.Add(files.Location);
                                        }
                                        else if (files.Location.ToLowerInvariant().EndsWith(".asmx"))
                                        {
                                            solutionInfo.WebServices.Add(files.Location);
                                        }
                                        else if (files.Location.ToLowerInvariant().EndsWith(".css"))
                                        {
                                            solutionInfo.CSSFiles.Add(files.Location);
                                        }
                                        else if (files.Location.ToLowerInvariant().EndsWith(".js"))
                                        {
                                            solutionInfo.JSFiles.Add(files.Location);
                                        }
                                        else if (files.Location.ToLowerInvariant().EndsWith(".xsl"))
                                        {
                                            solutionInfo.XSLFiles.Add(files.Location);
                                        }
                                        else if (files.Location.ToLowerInvariant().EndsWith("onet.xml"))
                                        {
                                            ProcessOnet(files.Location, sdi);
                                            solutionInfo.SiteDefinitions.Add(sdi);
                                        }
                                        else if (files.Location.ToLowerInvariant().EndsWith(".xml"))
                                        {
                                            ProcessFieldType(files.Location, solutionInfo);
                                            solutionInfo.Files.Add(files.Location);
                                        }
                                        else if (files.Location.ToLowerInvariant().EndsWith(".actions"))
                                        {
                                            ProcessWorkFlowActions(files.Location, solutionInfo);
                                            solutionInfo.Files.Add(files.Location);
                                        }
                                        else if (files.Location.ToLowerInvariant().EndsWith(".actions4"))
                                        {
                                            ProcessWorkFlowActions(files.Location, solutionInfo);
                                            solutionInfo.Files.Add(files.Location);
                                        }
                                        else
                                        {
                                            if (files.Location.Contains("WEBTEMP"))
                                            {
                                                int lastPos = files.Location.LastIndexOf("\\");
                                                string webtempname = files.Location.Substring(lastPos + 1);
                                                if (webtempname == lastWebTemp)
                                                {
                                                    sdi.NrOfLocales = sdi.NrOfLocales + 1;
                                                }
                                                else
                                                {
                                                    sdi.NrOfLocales = 1;
                                                    sdi.Name = webtempname.Substring(7);
                                                    lastWebTemp = webtempname;
                                                }
                                            }
                                            solutionInfo.Files.Add(files.Location);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.StartsWith("There is an error in XML document"))
                                {
                                    if ((fileName.ToLower().EndsWith(".cab")) || (fileName.ToLower().EndsWith(".stp")))
                                    {
                                        if (ProcessCabFile(fiManifest.FullName, solutionInfo))
                                        {
                                        }
                                        else
                                        {
                                            solutionInfo.Error = "The specified file appears to be a Web Part package";
                                        }
                                    }
                                    else
                                    {
                                        solutionInfo.Error = ex.Message;
                                    }
                                }
                                else
                                {
                                    solutionInfo.Error = ex.Message;
                                }

                                //ExceptionCSV_SolutionAnalyzer.WriteException(Path.GetFileName(solutionInfo.Name), "ProcessWSPFile", ex.Message, ex.ToString(), ex.GetType().ToString(), solutionInfo.Error);
                            }
                        }
                    }
                }
            }
            finally
            {
                try
                {
                    Directory.Delete(folderWeExtractedTo, true);
                }
                catch { }
            }
        }

        /// <summary>
        /// Initialize the scanner
        /// </summary>
        public void Init(bool verboseFlag)
        {
            _numberOfFeatures = 0;
            _numberOfAssemblies = 0;
            _numberOfSafeControlEntries = 0;
            _numberOfClassResources = 0;
            _verbose = verboseFlag;
        }

        /// <summary>
        /// Process passed file
        /// </summary>
        public SolutionInformation ProcessFileInfo(FileInfo fi)
        {
            SolutionInformation solInfo = new SolutionInformation();
            solInfo.Name = fi.FullName;
            ProcessWsp(fi.FullName, solInfo);
            return solInfo;
        }

        /// <summary>
        /// Process passed file
        /// </summary>
        public SolutionInformation ProcessFileInfo(string fileName)
        {
            SolutionInformation solInfo = new SolutionInformation();
            solInfo.Name = fileName;
            ProcessWsp(fileName, solInfo);
            return solInfo;
        }

        /// <summary>
        /// Unpack a cab file
        /// </summary>
        public void UnCab(string cabFile, string tempFolderToUse)
        {
            EnsureDirectoryExists(tempFolderToUse);
            CabInfo cabToUnpack = new CabInfo(cabFile);
            cabToUnpack.Unpack(tempFolderToUse);
        }

        /// <summary>
        /// Repack the cab file
        /// </summary>
        public void ReCab(string cabFile, string unCabFolder)
        {
            CabInfo cabToPack = new CabInfo(cabFile);
            cabToPack.Pack(unCabFolder, true, Microsoft.Deployment.Compression.CompressionLevel.Min, null);

            try
            {
                // Cleanup the temp folder with unpacked data
                System.IO.Directory.Delete(unCabFolder, true);
            }
            catch { }
        }

        private static void EnsureDirectoryExists(string dir)
        {
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
        }

        private static string Escape(string s)
        {
            if (s != null)
            {
                const string quote = "\"";
                const string escapedQuote = "\"\"";
                char[] charactersThatMustBeQuoted = { ',', '"', '\n' };
                if (s.Contains(quote))
                    s = s.Replace(quote, escapedQuote);

                if (s.IndexOfAny(charactersThatMustBeQuoted) > -1)
                    s = quote + s + quote;

                return s;
            }
            else
            {
                return string.Empty;
            }
        }

        private static string RemoveCarriageReturAndNewLine(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                const string carriageReturn = "\r";
                const string newLine = "\n";
                s = s.Replace(carriageReturn, "").Replace(newLine, "").Trim();
            }
            return s;
        }

        /// <summary>
        /// Deal with resource files
        /// </summary>
        private static void ExtractContentTypeIDFromResourceFile(string resourceFileAbsolutePath, string resourceKey, string solutionName, ref string contentTypeID, ref bool contentTypeIDModified)
        {
            IEnumerable<DictionaryEntry> resDict = null;
            try
            {
                //Read ResourceFile file from its path
                using (ResXResourceReader rr = new ResXResourceReader(resourceFileAbsolutePath))
                {
                    //Get ResourceFile key-values into IEnumerable<Dictionary>
                    resDict = from DictionaryEntry d in rr
                              where d.Key.ToString().Equals(resourceKey)
                              select d;
                    //Check current Resourcefile contains ResourceKey or not
                    if (resDict.Any())
                    {
                        //Check resourceKey is equal to Dictionary key or not
                        if (resDict.First().Key.Equals(resourceKey))
                        {
                            //Replace ContantTypeID containg Resource file key with value
                            contentTypeID = resDict.First().Value + contentTypeID.Substring(contentTypeID.IndexOf(";") + 1);
                            contentTypeIDModified = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                //ExceptionCSV_SolutionAnalyzer.WriteException(solutionName, "ExtractContentTypeIDFromResourceFile", ex.Message, ex.ToString(), ex.GetType().ToString(), "case: CustomizationAnalyzer.ContentTypeDefinition");
            }
            resDict = null;
        }
    }
}