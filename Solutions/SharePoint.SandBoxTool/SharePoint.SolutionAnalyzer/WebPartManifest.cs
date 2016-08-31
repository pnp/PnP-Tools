using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

/// <summary>
/// Semi automatically generated classes that represent web part xml
/// </summary>
/// 
namespace SharePoint.SolutionAnalyzer
{

  /// <remarks/>
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/WebPart/v2/Manifest")]
  [XmlRoot(Namespace = "http://schemas.microsoft.com/WebPart/v2/Manifest", IsNullable = false)]
  public partial class WebPartManifest
  {

    private object[] assemblies;
    private object[] dwpFiles;

    /// <remarks/>
    [XmlElement("Assemblies", typeof(WebPartManifestAssemblies))]
    public object[] Assemblies
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


    [XmlElement("DwpFiles", typeof(WebPartManifestDwpFiles))]
    public object[] DWPFiles
    {
      get
      {
        return this.dwpFiles;
      }
      set
      {
        this.dwpFiles = value;
      }
    }

  }

  /// <remarks/>
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/WebPart/v2/Manifest")]
  public partial class WebPartManifestAssemblies
  {

    private WebPartManifestAssembliesAssembly[] assemblyField;

    /// <remarks/>
    [XmlElement("Assembly")]
    public WebPartManifestAssembliesAssembly[] Assemblies
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
  }

  /// <remarks/>
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/WebPart/v2/Manifest")]
  public partial class WebPartManifestAssembliesAssembly
  {

    private string fileNameField;

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
  [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/WebPart/v2/Manifest")]
  public partial class WebPartManifestDwpFiles
  {

    private WebPartManifestDwpFilesDwpFile[] dwpFileField;

    /// <remarks/>
    [XmlElement("DwpFile")]
    public WebPartManifestDwpFilesDwpFile[] DwpFiles
    {
      get
      {
        return this.dwpFileField;
      }
      set
      {
        this.dwpFileField = value;
      }
    }
  }

  /// <remarks/>
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [Serializable()]
  [DebuggerStepThrough()]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/WebPart/v2/Manifest")]
  public partial class WebPartManifestDwpFilesDwpFile
  {

    private string fileNameField;

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
}
