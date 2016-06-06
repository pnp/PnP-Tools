using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Provisioning.VSTools.Models;

namespace Provisioning.VSTools.Helpers
{
    public static class ProjectHelpers
    {
        internal static T GetConfigFile<T>(string filepath, bool projectItem = true)
        {
            if (System.IO.File.Exists(filepath))
            {
                var dte = (EnvDTE.DTE)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(EnvDTE.DTE));
                var solutionItem = dte.Solution.FindProjectItem(filepath);

                if (solutionItem != null || projectItem == false)
                {
                    return XmlHelpers.DeserializeObject<T>(filepath);
                }
            }

            return default(T);
        }

        public static string GetFullPath(ProjectItem projectItem)
        {
            return Convert.ToString(projectItem.Properties.Item("FullPath").Value);
        }

        public static bool IsItemInsideFolder(string itemPath, string folderPath)
        {
            return itemPath.StartsWith(folderPath, true, CultureInfo.InvariantCulture);
        }

        public static ProjectItem GetProjectItem(this IVsHierarchy hierarchy, uint ItemID)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            Object prjItemObject = null;
            ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(
                ItemID, (int)__VSHPROPID.VSHPROPID_ExtObject, out prjItemObject));

            return prjItemObject as ProjectItem;
        }

        private static EnvDTE.Project GetProject(this IVsHierarchy hierarchy, uint ItemID)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            Object prjItemObject = null;
            ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(
                ItemID, (int)__VSHPROPID.VSHPROPID_ExtObject, out prjItemObject));

            if (prjItemObject is EnvDTE.ProjectItem)
            {
                return ((EnvDTE.ProjectItem)prjItemObject).ContainingProject;
            }

            return prjItemObject as EnvDTE.Project;
        }

        public static EnvDTE.Project GetProjectFromExplorer(EnvDTE80.DTE2 dte)
        {
            UIHierarchy hierarchy = dte.ToolWindows.SolutionExplorer;
            var selectedItems = (Array)hierarchy.SelectedItems;
            if (selectedItems != null && selectedItems.Length > 0)
            {
                foreach (UIHierarchyItem selectedItem in selectedItems)
                {
                    ProjectItem projectItem = selectedItem.Object as ProjectItem;
                    if (projectItem != null && projectItem.ContainingProject != null)
                    {
                        return projectItem.ContainingProject;
                    }
                }
            }

            return null;
        }

        //public static string GetProjectPath()
        //{
        //    //var project = GetProject();
        //    var project = GetActiveProject();
        //    return GetProjectPath(project);
        //}

        public static string GetProjectPath(Project project)
        {
            if (project != null)
            {
                return Path.GetDirectoryName(project.FullName);
            }
            else
            {
                return null;
            }
        }

        public static EnvDTE.Project GetProject()
        {
            uint projectItemId;
            var hierarchy = ProjectHelpers.GetCurrentHierarchy(out projectItemId);

            return ProjectHelpers.GetProject(hierarchy, projectItemId);
        }

        internal static Project GetActiveProject()
        {
            DTE dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
            Project activeProject = null;

            Array activeSolutionProjects = dte.ActiveSolutionProjects as Array;
            if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
            {
                activeProject = activeSolutionProjects.GetValue(0) as Project;
            }

            return activeProject;
        }

        public static IVsHierarchy GetCurrentHierarchy(out uint projectItemId)
        {
            IntPtr hierarchyPtr, selectionContainerPtr;

            IVsMultiItemSelect mis;

            IVsMonitorSelection monitorSelection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));

            monitorSelection.GetCurrentSelection(out hierarchyPtr, out projectItemId, out mis, out selectionContainerPtr);

            return Marshal.GetTypedObjectForIUnknown(hierarchyPtr, typeof(IVsHierarchy)) as IVsHierarchy;
        }

        public static EnvDTE80.DTE2 GetDTE2()
        {
            return Package.GetGlobalService(typeof(DTE)) as EnvDTE80.DTE2;
        }

        public static string GetSourceFilePath()
        {
            EnvDTE80.DTE2 _applicationObject = GetDTE2();
            UIHierarchy uih = _applicationObject.ToolWindows.SolutionExplorer;
            Array selectedItems = (Array)uih.SelectedItems;
            if (null != selectedItems)
            {
                foreach (UIHierarchyItem selItem in selectedItems)
                {
                    ProjectItem prjItem = selItem.Object as ProjectItem;
                    string filePath = prjItem.Properties.Item("FullPath").Value.ToString();
                    //System.Windows.Forms.MessageBox.Show(selItem.Name + filePath);
                    return filePath;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Checks if the supplied filePath should be included
        /// </summary>
        /// <returns>true if the file should be included</returns>
        public static bool IncludeFile(string filePath)
        {
            try
            {
                System.IO.FileInfo fileInfo = new FileInfo(filePath);
                return !Constants.ExtensionsToIgnore.Contains(fileInfo.Extension.ToLower());
            }
            catch
            {
                return false;
            }
        }

        public static ProvisioningTemplateLocationInfo GetParentProvisioningTemplateInformation(string projectItemFullPath, string projectFolderPath, ProvisioningTemplateToolsConfiguration config)
        {
            ProvisioningTemplateLocationInfo templateInfo = null;

            if (config != null && config.Templates != null)
            {
                foreach (var template in config.Templates)
                {
                    var pnpResourcesFolderPath = Path.Combine(projectFolderPath, template.ResourcesFolder);
                    var templateFilePath = Path.Combine(projectFolderPath, template.Path);
                    bool isTemplateXmlFile = string.Compare(System.IO.Path.GetFileName(projectItemFullPath), System.IO.Path.GetFileName(template.Path), true) == 0;

                    if (ProjectHelpers.IsItemInsideFolder(projectItemFullPath, pnpResourcesFolderPath) || isTemplateXmlFile)
                    {
                        templateInfo = new ProvisioningTemplateLocationInfo()
                        {
                            ResourcesPath = pnpResourcesFolderPath,
                            TemplateFolderPath = Path.GetDirectoryName(templateFilePath),
                            TemplateFileName = Path.GetFileName(templateFilePath)
                        };
                    }
                }
            }

            return templateInfo;
        }
    }
}
