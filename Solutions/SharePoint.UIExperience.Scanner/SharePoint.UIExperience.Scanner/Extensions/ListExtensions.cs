using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SharePoint.Client
{
    public static partial class ListExtensions
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
                list.BaseTemplate == (int)ListTemplateType.Links ||
                list.BaseTemplate == 851 || // Assets
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
        /// Searches for the content type with the closest match to the value of the specified content type ID. 
        /// If the search finds two matches, the shorter ID is returned. 
        /// </summary>
        /// <param name="contentTypes">Content type collection to search</param>
        /// <param name="contentTypeId">Complete ID for the content type to search</param>
        /// <returns>Content type Id object or null if was not found</returns>
        public static ContentTypeId BestMatch(this ContentTypeCollection contentTypes, string contentTypeId)
        {
            var res = contentTypes.Where(c => c.Id.StringValue.StartsWith(contentTypeId)).OrderBy(c => c.Id.StringValue.Length).FirstOrDefault();
            if (res != null)
            {
                return res.Id;
            }
            return null;
        }

    }
}
