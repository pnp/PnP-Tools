using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharePointPnP.DeveloperTools.VisualStudio.Extenders
{
	[ComVisible(true)] // Important!
	public interface ISiteTemplateExtender
	{
		string DisplayName { get; set; }
	}
}
