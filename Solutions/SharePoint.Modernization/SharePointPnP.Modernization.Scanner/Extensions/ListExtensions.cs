using System;
using System.Linq;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class with extension methods for a SharePoint List object
    /// </summary>
    public static class ListExtensions
    {

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
