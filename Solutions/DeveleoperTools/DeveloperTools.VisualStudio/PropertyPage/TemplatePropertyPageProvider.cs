using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Editors.PropertyPages;

namespace SharePointPnP.DeveloperTools.VisualStudio
{
	[ComVisible(true)]
	[Guid("109F4BC1-4DE4-4DCA-A0B1-EF18B8AF9742")]
	[ProvideObject(typeof(TemplatePropertyPageProvider))]
	public class TemplatePropertyPageProvider : PropPageBase
	{
		protected override Type ControlType
		{
			get { return typeof(TemplatePropertyPageControl); }
		}
		protected override string Title
		{
			get { return "Provisioning template"; }
		}
		protected override Control CreateControl()
		{
			return new TemplatePropertyPageControl();
		}
	}
}
