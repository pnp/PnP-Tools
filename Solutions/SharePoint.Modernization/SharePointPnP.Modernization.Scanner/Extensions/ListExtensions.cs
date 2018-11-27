using System;
using System.Linq;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class with extension methods for a SharePoint List object
    /// </summary>
    public static class ListExtensions
    {

        public static bool CanRenderNewExperience(this List list)
        {
            list.EnsureProperty(p => p.BaseTemplate);

            if (list.BaseTemplate == (int)ListTemplateType.Announcements ||
                list.BaseTemplate == (int)ListTemplateType.Links ||
                list.BaseTemplate == (int)ListTemplateType.DocumentLibrary ||
                list.BaseTemplate == (int)ListTemplateType.PictureLibrary ||
                list.BaseTemplate == (int)ListTemplateType.WebPageLibrary ||
                list.BaseTemplate == (int)ListTemplateType.Announcements ||
                list.BaseTemplate == (int)ListTemplateType.IssueTracking ||
                list.BaseTemplate == (int)ListTemplateType.Contacts ||
                list.BaseTemplate == 851 || // Assets
                list.BaseTemplate == (int)ListTemplateType.CustomGrid ||
                list.BaseTemplate == 850 || // Publishing pages library
                list.BaseTemplate == 170 || // Promoted Links
                list.BaseTemplate == (int)ListTemplateType.XMLForm ||
                list.BaseTemplate == (int)ListTemplateType.GenericList)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if item level publishing scheduling is enabled for this page library
        /// </summary>
        /// <param name="list">Page library to check</param>
        /// <returns>True if enabled, false otherwise</returns>
        public static bool ItemSchedulingEnabled(this List list)
        {
            list.EnsureProperties(l => l.EventReceivers, l => l.Fields);

            var itemUpdatingER = list.EventReceivers.Where(p => p.EventType == EventReceiverType.ItemUpdating && p.ReceiverClass == "Microsoft.SharePoint.Publishing.Internal.ScheduledItemEventReceiver").FirstOrDefault();
            var itemAddedER = list.EventReceivers.Where(p => p.EventType == EventReceiverType.ItemAdded && p.ReceiverClass == "Microsoft.SharePoint.Publishing.Internal.ScheduledItemEventReceiver").FirstOrDefault();
            if (itemUpdatingER != null && itemAddedER != null)
            {
                Guid StartDate = new Guid("51d39414-03dc-4bd0-b777-d3e20cb350f7");
                Guid EndDate = new Guid("a990e64f-faa3-49c1-aafa-885fda79de62");

                if (list.FieldExistsById(StartDate) && list.FieldExistsById(EndDate))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
