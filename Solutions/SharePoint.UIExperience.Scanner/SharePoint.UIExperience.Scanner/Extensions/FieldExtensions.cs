using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SharePoint.Client
{
    public static partial class FieldExtensions
    {

        public static bool IsTaxField(this Field field)
        {
            field.EnsureProperty(p => p.TypeAsString);

            if (string.IsNullOrEmpty(field.TypeAsString))
            {
                return false;
            }
            else
            {
                return field.TypeAsString.Equals("TaxonomyFieldType", StringComparison.OrdinalIgnoreCase) ||
                   field.TypeAsString.Equals("TaxonomyFieldTypeMulti", StringComparison.OrdinalIgnoreCase);
            }
        }

        public static bool IsTaskOutcomeField(this Field field)
        {
            field.EnsureProperty(p => p.TypeAsString);

            if (string.IsNullOrEmpty(field.TypeAsString))
            {
                return false;
            }
            else
            {
                return field.TypeAsString.Equals("OutcomeChoice", StringComparison.OrdinalIgnoreCase);
            }
        }

        public static bool IsBusinessDataField(this Field field)
        {
            field.EnsureProperty(p => p.TypeAsString);

            if (string.IsNullOrEmpty(field.TypeAsString))
            {
                return false;
            }
            else
            {
                return field.TypeAsString.Equals("BusinessData", StringComparison.OrdinalIgnoreCase);
            }
        }

        public static bool IsPublishingField(this Field field)
        {
            field.EnsureProperty(p => p.TypeAsString);

            if (string.IsNullOrEmpty(field.TypeAsString))
            {
                return false;
            }
            else
            {
                string[] publishingFieldTypes = new string[] { "image", "html", "summarylinks" };
                return Array.IndexOf(publishingFieldTypes, field.TypeAsString.ToLowerInvariant()) >= 0;
            }
        }


    }
}
