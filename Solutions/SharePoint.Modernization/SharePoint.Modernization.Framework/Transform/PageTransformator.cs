using Microsoft.SharePoint.Client;
using SharePoint.Modernization.Framework.Entities;
using SharePoint.Modernization.Framework.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OfficeDevPnP.Core.Pages;

namespace SharePoint.Modernization.Framework.Transform
{
    public class PageTransformator : IPageTransformator
    {
        private const string FileRefField = "FileRef";
        private const string FileLeafRefField = "FileLeafRef";
        private const string FileTitle = "Title";
        private const string ClientSideApplicationId = "ClientSideApplicationId";
        private static readonly Guid FeatureId_Web_ModernPage = new Guid("B6917CB1-93A0-4B97-A84D-7CF49975D4EC");

        private ClientContext clientContext;
        private PageTransformation pageTransformation;

        #region Construction
        public PageTransformator(ClientContext clientContext): this(clientContext, "webpartmapping.xml")
        {
        }

        public PageTransformator(ClientContext clientContext, string pageTransformationFile)
        {
            this.clientContext = clientContext;

            // Load xml mapping data
            XmlSerializer xmlMapping = new XmlSerializer(typeof(PageTransformation));
            using (var stream = new FileStream(pageTransformationFile, FileMode.Open))
            {
                this.pageTransformation = (PageTransformation)xmlMapping.Deserialize(stream);
            }

        }
        #endregion

        public void Transform(ListItem sourcePage)
        {
            Transform(sourcePage, null);
        }

        public void Transform(ListItem sourcePage, string targetPageName)
        {
            if (sourcePage == null)
            {
                throw new ArgumentNullException("SourcePage cannot be null");
            }

            // Validate page and it's eligibility for transformation
            if (!sourcePage.FieldExistsAndUsed(FileRefField) || !sourcePage.FieldExistsAndUsed(FileLeafRefField))
            {
                throw new ArgumentException("Page is not valid due to missing FileRef or FileLeafRef value");
            }

            string pageType = sourcePage.PageType();

            if (pageType.Equals("ClientSidePage", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Page is a client side page...guess you don't want to transform it...");
            }

            if (pageType.Equals("AspxPage", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Page is an basic aspx page...can't transform that one, sorry!");
            }

            // If no targetname specified then we'll come up with one
            if (string.IsNullOrEmpty(targetPageName))
            {
                targetPageName = $"Migrated_{sourcePage[FileLeafRefField].ToString()}";
            }

            // Analyze the page
            Tuple<string, List<WebPartEntity>> pageData = null;

            if (pageType.Equals("WikiPage", StringComparison.InvariantCultureIgnoreCase))
            {
                pageData = new WikiPage(sourcePage, pageTransformation).Analyze();
            }
            else if (pageType.Equals("WebPartPage", StringComparison.InvariantCultureIgnoreCase))
            {
                pageData = new WebPartPage(sourcePage, pageTransformation).Analyze(true);
            }

            // Create a new client side page
            ClientSidePage targetPage = clientContext.Web.AddClientSidePage(targetPageName);

            // Set title
            if (pageType.Equals("WikiPage", StringComparison.InvariantCultureIgnoreCase) && sourcePage.FieldExistsAndUsed(FileTitle))
            {
                targetPage.PageTitle = sourcePage[FileTitle].ToString();
            }
            else if (pageType.Equals("WebPartPage"))
            {
                var titleBarWebPart = pageData.Item2.Where(p => p.Type == "Microsoft.SharePoint.WebPartPages.TitleBarWebPart, Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c").FirstOrDefault();
                if (titleBarWebPart != null)
                {
                    if (titleBarWebPart.Properties.ContainsKey("HeaderTitle") && !string.IsNullOrEmpty(titleBarWebPart.Properties["HeaderTitle"]))
                    {
                        targetPage.PageTitle = titleBarWebPart.Properties["HeaderTitle"];
                    }
                }
            }

            // Configure the page layout
            LayoutTransformator layoutTransformator = new LayoutTransformator(targetPage);
            layoutTransformator.ApplyLayout(pageData.Item1);

            // Transform the page contents

            // Persist the client side page
            targetPage.Save(targetPageName);
            targetPage.Publish();
        }


    }
}
