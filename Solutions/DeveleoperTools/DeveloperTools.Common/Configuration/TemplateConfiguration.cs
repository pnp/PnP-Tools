using System;
using System.Xml.Serialization;

namespace SharePointPnP.DeveloperTools.Common.Configuration
{
	[Flags]
	public enum TargetPlatform
	{
		Undefined = 0,
		SPO  = 0x1,
		SP13 = 0x2,
		SP16 = 0x4
	}

	[Serializable]
	public class TemplateConfiguration
	{
		public string Author { get; set; }
		public string DisplayName { get; set; }
		public string ImagePreviewUrl { get; set; }
		[XmlIgnore]
		public TargetPlatform TargetPlatform { get; set; }

		[XmlElement("TargetPlatform")]
		public int TargetPlatformInt
		{
			get { return (int)TargetPlatform; }
			set { TargetPlatform = (TargetPlatform)value; }
		}
	}
}
