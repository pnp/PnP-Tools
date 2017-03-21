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

            if (list.BaseTemplate == (int)ListTemplateType.DocumentLibrary ||
                list.BaseTemplate == (int)ListTemplateType.PictureLibrary ||
                list.BaseTemplate == (int)ListTemplateType.WebPageLibrary ||
                list.BaseTemplate == (int)ListTemplateType.GenericList)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
