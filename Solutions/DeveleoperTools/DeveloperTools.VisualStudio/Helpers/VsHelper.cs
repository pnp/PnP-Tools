using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;

namespace SharePointPnP.DeveloperTools.VisualStudio
{
	class VsHelper
	{
		internal static Project GetActiveProject()
		{
			Project res = null;
			DTE dte = Package.GetGlobalService(typeof(SDTE)) as DTE;

			Array projects = dte?.ActiveSolutionProjects as Array;
			if (projects?.Length > 0)
			{
				res = projects.GetValue(0) as Project;
			}

			return res;
		}

		internal static string GetActiveProjectDirectory()
		{
			var project = VsHelper.GetActiveProject();
			var path = project != null ? Path.GetDirectoryName(project.FullName) : string.Empty;
			return path;
		}

	}
}
