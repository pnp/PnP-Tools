using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Pages;
using SharePoint.Modernization.Framework.Entities;
using SharePoint.Modernization.Framework.Functions;

namespace SharePoint.Modernization.Framework.Transform
{
    public class ContentTransformator: IContentTransformator
    {
        private ClientSidePage page;
        private PageTransformation pageTransformation;
        private FunctionProcessor functionProcessor;
        private List<CombinedMapping> combinedMappinglist;
        private Dictionary<string, string> siteTokens;

        class CombinedMapping
        {
            public int Order { get; set; }
            public ClientSideText ClientSideText { get; set; }
            public ClientSideWebPart ClientSideWebPart { get; set; }
        }

        public ContentTransformator(ClientSidePage page, PageTransformation pageTransformation)
        {
            this.page = page ?? throw new ArgumentException("Page cannot be null");
            this.pageTransformation = pageTransformation ?? throw new ArgumentException("pageTransformation cannot be null");
            this.functionProcessor = new FunctionProcessor(this.page);
            this.siteTokens = CreateSiteTokenList(page.Context);
        }

        public void Transform(List<WebPartEntity> webParts)
        {
            if (webParts == null || webParts.Count == 0)
            {
                // nothing to transform
                return;
            }

            var defaultMapping = pageTransformation.BaseWebPart.Mappings.Where(p => p.Default == true).FirstOrDefault();
            if (defaultMapping == null)
            {
                throw new Exception("No default mapping was found int the provided mapping file");
            }

            // Load existing available controls
            var componentsToAdd = page.AvailableClientSideComponents().ToList();

            // Iterate over the web parts, important to order them by row, column and zoneindex
            foreach (var webPart in webParts.OrderBy(p => p.Row).OrderBy(p => p.Column).OrderBy(p =>p.ZoneIndex))
            {
                if (webPart.Type == WebParts.TitleBar)
                {
                    continue;
                }

                // Add site tokens to the web part
                foreach (var token in this.siteTokens)
                {
                    webPart.Properties.Add(token.Key, token.Value);
                }

                Mapping mapping = defaultMapping;
                // Does the web part have a mapping defined?
                var webPartData = pageTransformation.WebParts.Where(p => p.Type == webPart.Type).FirstOrDefault();
                if (webPartData != null && webPartData.Mappings != null)
                {
                    // Process the functions inside the mapping definition
                    functionProcessor.Process(ref webPartData, webPart);

                    var webPartMapping = webPartData.Mappings.Where(p => p.Default == true).FirstOrDefault();
                    if (webPartMapping != null)
                    {
                        mapping = webPartMapping;
                    }
                }

                


                // Use the mapping data => make one list of Text and WebParts to allow for correct ordering
                combinedMappinglist = new List<CombinedMapping>();
                if (mapping.ClientSideText != null)
                {
                    foreach (var map in mapping.ClientSideText.OrderBy(p => p.Order))
                    {
                        if (!Int32.TryParse(map.Order, out Int32 mapOrder))
                        {
                            mapOrder = 0;
                        }

                        combinedMappinglist.Add(new CombinedMapping { ClientSideText = map, ClientSideWebPart = null, Order = mapOrder });
                    }
                }
                if (mapping.ClientSideWebPart != null)
                {
                    foreach (var map in mapping.ClientSideWebPart.OrderBy(p => p.Order))
                    {
                        if (!Int32.TryParse(map.Order, out Int32 mapOrder))
                        {
                            mapOrder = 0;
                        }

                        combinedMappinglist.Add(new CombinedMapping { ClientSideText = null, ClientSideWebPart = map, Order = mapOrder });
                    }
                }

                // Get the order of the last inserted control in this column
                int order = LastColumnOrder(webPart.Row - 1, webPart.Column - 1);
                // Interate the controls for this mapping using their order
                foreach (var map in combinedMappinglist.OrderBy(p => p.Order))
                {
                    order++;

                    if (map.ClientSideText != null)
                    {
                        // Insert a Text control
                        OfficeDevPnP.Core.Pages.ClientSideText text = new OfficeDevPnP.Core.Pages.ClientSideText()
                        {
                            Text = TokenParser.ReplaceTokens(map.ClientSideText.Text, webPart)
                        };

                        page.AddControl(text, page.Sections[webPart.Row - 1].Columns[webPart.Column - 1], order);
                    }
                    else if (map.ClientSideWebPart != null)
                    {
                        ClientSideComponent baseControl = null;

                        // Insert a web part
                        if (map.ClientSideWebPart.Type == ClientSideWebPartType.Custom)
                        {
                            // TODO
                        }
                        else
                        {
                            string webPartName = "";
                            switch (map.ClientSideWebPart.Type)
                            {
                                case ClientSideWebPartType.List:
                                    {
                                        webPartName = ClientSidePage.ClientSideWebPartEnumToName(DefaultClientSideWebParts.List);
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }

                            baseControl = componentsToAdd.FirstOrDefault(p => p.Name.Equals(webPartName, StringComparison.InvariantCultureIgnoreCase));
                        }

                        if (baseControl != null)
                        {
                            OfficeDevPnP.Core.Pages.ClientSideWebPart myWebPart = new OfficeDevPnP.Core.Pages.ClientSideWebPart(baseControl)
                            {
                                Order = map.Order,
                                PropertiesJson = TokenParser.ReplaceTokens(map.ClientSideWebPart.JsonControlData, webPart)
                            };

                            page.AddControl(myWebPart, page.Sections[webPart.Row - 1].Columns[webPart.Column - 1], order);
                        }

                    }
                }



            }
        }

        #region Helper methods
        private Dictionary<string, string> CreateSiteTokenList(ClientContext cc)
        {
            Dictionary<string, string> siteTokens = new Dictionary<string, string>(5);

            cc.Web.EnsureProperties(p => p.Url, p => p.ServerRelativeUrl);
            cc.Site.EnsureProperties(p => p.RootWeb.ServerRelativeUrl);

            siteTokens.Add("site", cc.Web.ServerRelativeUrl.TrimEnd('/'));
            siteTokens.Add("sitecollection", cc.Site.RootWeb.ServerRelativeUrl.TrimEnd('/'));

            return siteTokens;
        }

        private Int32 LastColumnOrder(int row, int col)
        {
            var lastControl = page.Sections[row].Columns[col].Controls.OrderBy(p => p.Order).LastOrDefault();
            if (lastControl != null)
            {
                return lastControl.Order;
            }
            else
            {
                return -1;
            }
        }
        #endregion


    }
}
