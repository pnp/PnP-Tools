using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Framework
{
    public static class Constants
    {
        // Fields
        public const string FileRefField = "FileRef";
        public const string FileLeafRefField = "FileLeafRef";
        public const string FileTitleField = "Title";
        public const string ClientSideApplicationIdField = "ClientSideApplicationId";
        public const string HtmlFileTypeField = "HTML_x0020_File_x0020_Type";
        public const string WikiField = "WikiField";
        public const string ModifiedField = "Modified";
        public const string ModifiedByField = "Editor";

        // Features
        public static readonly Guid FeatureId_Web_ModernPage = new Guid("B6917CB1-93A0-4B97-A84D-7CF49975D4EC");


    }
}
