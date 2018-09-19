using CamlBuilder;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharePointPnP.Modernization.Framework.Transform
{

    #region Highlighted content properties model
    public enum ContentRollupLayout
    {
        Card = 1,
        List = 2,
        Carousel = 3,
        FilmStrip = 4,
        Masonry = 5,
        Custom = 999
    }

    public class ContentRollupWebPartProperties
    {
        [JsonProperty(PropertyName = "displayMaps")]
        public string DisplayMaps { get; set; } // will be populated from json string
        [JsonProperty(PropertyName = "query")]
        public SearchQuery Query { get; set; }
        [JsonProperty(PropertyName = "listId")]
        public string ListId { get; set; } // selected list to view the contents of
        [JsonProperty(PropertyName = "lastListId")]
        public string LastListId { get; set; } // used to detect list view selection changes
        [JsonProperty(PropertyName = "listTitle")]
        public string ListTitle { get; set; } // selected document library to view the contents of
        [JsonProperty(PropertyName = "isDefaultDocumentLibrary")]
        public bool? IsDefaultDocumentLibrary { get; set; }
        [JsonProperty(PropertyName = "documentLibrarySortField")]
        public string DocumentLibrarySortField { get; set; }
        [JsonProperty(PropertyName = "caml")]
        public string Caml { get; set; }
        [JsonProperty(PropertyName = "templateId")]
        public ContentRollupLayout? TemplateId { get; set; }
        [JsonProperty(PropertyName = "maxItemsPerPage")]
        public int? MaxItemsPerPage { get; set; }
        [JsonProperty(PropertyName = "isSeeAllPage")]
        public bool? IsSeeAllPage { get; set; }
        [JsonProperty(PropertyName = "isAdvancedFilterMode")]
        public bool? IsAdvancedFilterMode { get; set; }
        [JsonProperty(PropertyName = "sites")]
        public List<SiteMetadata> Sites { get; set; } // The list of sites metadata added by user as content source
        // IBaseCollectionWebPartProperties props
        [JsonProperty(PropertyName = "layoutId")] // Set when there are more than one layouts to indicate which layout is in use.
        public string LayoutId { get; set; }
        [JsonProperty(PropertyName = "dataProviderId")] // Set when there are more than one data providers to indicate which data provider is in use.
        public string DataProviderId { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "addToPageScreenReaderLabel")]
        public string AddToPageScreenReaderLabel { get; set; }
        [JsonProperty(PropertyName = "hideWebPartWhenEmpty")]
        public bool? HideWebPartWhenEmpty { get; set; }
        [JsonProperty(PropertyName = "webId")]
        public string WebId { get; set; }
        [JsonProperty(PropertyName = "siteId")]
        public string SiteId { get; set; }
        [JsonProperty(PropertyName = "baseUrl")]
        public string BaseUrl { get; set; } // base url to locate web part resources such has file icons

        public ContentRollupWebPartProperties()
        {
            this.DisplayMaps = "%%DisplayMapsPlaceholder%%";
        }

    }

    #region ISearchQuery enums and classes
    public enum ContentLocation
    {
        CurrentSite = 1,
        CurrentSiteCollection = 2,
        AllSites = 3,
        CurrentSiteDocumentLibrary = 4,
        AllSitesInTheHub = 5,
        CurrentSitePageLibrary = 6,
        SelectedSites = 99
    }

    public enum DocumentType
    {
        Word = 1,
        Excel = 2,
        PowerPoint = 3,
        OneNote = 4,
        Visio = 5,
        PDF = 10,
        Any = 99
    }

    public enum FilterType
    {
        TitleContaining = 1,            // title like "*value*"
        AnyTextContaining = 2,          // any field like "*value*
        TaggedWith = 3,                 // value in list (tags)
        CreatedBy = 4,                  // created by = value
        ModifiedBy = 5,                 // created by = value
        Field = 6,                      // field operator value(s)
        RecentlyChanged = 7,
        RecentlyAdded = 8
    }

    public enum ContentType
    {
        Document = 1,
        Page = 2,
        Video = 3,
        Event = 4,
        Issue = 5,
        Task = 6,
        Link = 7,
        Contact = 8,
        Image = 9,
        News = 10,
        All = 99
    }

    public enum UserType
    {
        CurrentUser = 1, //Indicates user is the current session user.
        SpecificUser = 2 //Indicates user is who match the given match text (e.g. name).
    }

    public enum FilterOperator
    {
        Equals = 1,
        NotEqual = 2,
        BeginsWith = 3,
        EndsWith = 4,
        Contains = 5,
        DoesNotContain = 6,
        LessThan = 7,
        GreaterThan = 8,
        Between = 9
    }

    public enum SortType
    {
        MostRecent = 1,
        MostViewed = 2,
        Trending = 3,
        FieldAscending = 4,
        FieldDescending = 5
    }

    public class ManagedPropertiesRefinerOptions
    {
        [JsonProperty(PropertyName = "number")]
        public int? Number { get; set; } // How many unique managed properties to return
        [JsonProperty(PropertyName = "managedPropertyMatchText")]
        public string ManagedPropertyMatchText { get; set; } // Text to filter managed properties by name
    }

    public class SearchQueryFilter
    {
        [JsonProperty(PropertyName = "filterType")]
        public FilterType? FilterType { get; set; }
        [JsonProperty(PropertyName = "userType")]
        public UserType? UserType { get; set; }
        [JsonProperty(PropertyName = "fieldNameMatchText")]
        public string FieldNameMatchText { get; set; }
        [JsonProperty(PropertyName = "lastFieldNameMatchText")]
        public string LastFieldNameMatchText { get; set; }
        [JsonProperty(PropertyName = "fieldname")]
        public string Fieldname { get; set; }
        [JsonProperty(PropertyName = "op")]
        public FilterOperator? Op { get; set; }
        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }
        [JsonProperty(PropertyName = "value2")]
        public object Value2 { get; set; }
        [JsonProperty(PropertyName = "values")]
        public List<object> Values { get; set; }
        [JsonIgnore]
        public FilterChainingOperator? ChainingOperatorUsedInCQWP { get; set; } 

        public SearchQueryFilter()
        {
            this.Values = new List<object>();
        }

    }

    public class SearchQuery
    {
        [JsonProperty(PropertyName = "advancedQueryText")]
        public string AdvancedQueryText { get; set; }
        [JsonProperty(PropertyName = "contentLocation")]
        public ContentLocation? ContentLocation { get; set; } // search scope
        [JsonProperty(PropertyName = "contentTypes")] // content types to include in query result
        public List<ContentType> ContentTypes { get; set; }
        [JsonProperty(PropertyName = "documentTypes")]
        public List<DocumentType> DocumentTypes { get; set; }
        [JsonProperty(PropertyName = "filters")]
        public List<SearchQueryFilter> Filters { get; set; }
        [JsonProperty(PropertyName = "sortField")]
        public string SortField { get; set; }
        [JsonProperty(PropertyName = "sortFieldMatchText")]
        public string SortFieldMatchText { get; set; }
        [JsonProperty(PropertyName = "sortType")]
        public SortType? SortType { get; set; }
        [JsonProperty(PropertyName = "managedPropertiesRefinerOptions")]
        public ManagedPropertiesRefinerOptions ManagedPropertiesRefinerOptions { get; set; }

        public SearchQuery()
        {
            this.ContentTypes = new List<ContentType>();
            this.DocumentTypes = new List<DocumentType>();
            this.Filters = new List<SearchQueryFilter>();
        }
    }
    #endregion

    #region ISiteMetadata enums and classes
    public class SiteReference
    {
        [JsonProperty(PropertyName = "siteId")]
        public string SiteId { get; set; }
        [JsonProperty(PropertyName = "webId")]
        public string WebId { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } //'SiteReference' | 'GroupReference' | string;
        [JsonProperty(PropertyName = "indexId")]
        public int IndexId { get; set; }
        [JsonProperty(PropertyName = "groupId")]
        public string GroupId { get; set; } // The id of the group when site reference type is 'GroupReference, It's not available for other site reference types
    }

    public class SiteMetadata
    {
        [JsonProperty(PropertyName = "acronym")]
        public string Acronym { get; set; } // The acronym of the site. Used in banner image if the banner image url is not available
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; } // Site title
        [JsonProperty(PropertyName = "bannerColor")] 
        public string BannerColor { get; set; } // The color represents the site theme
        [JsonProperty(PropertyName = "bannerImageUrl")]
        public string BannerImageUrl { get; set; } // The url of the site logo
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; } // The url points to the site
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } //Type of the site. E.g. 'Site', 'Group'.
        [JsonProperty(PropertyName = "itemReference")]
        public SiteReference ItemReference { get; set; } // Identifiers of the site
    }
    #endregion

    #endregion

    #region ContentByQuery model
    public enum SortDirection
    {
        Asc,
        Desc
    }

    public enum FilterChainingOperator
    {
        /// <summary>
        /// Filter is chained using an And operator
        /// </summary>
        And,

        /// <summary>
        /// Filter is chained using an Or operator
        /// </summary>
        Or
    }

    public enum FilterFieldQueryOperator
    {
        /// <summary>
        /// Equal to
        /// </summary>
        Eq,

        /// <summary>
        /// Not equal to
        /// </summary>
        Neq,

        /// <summary>
        /// Greater than
        /// </summary>
        Gt,

        /// <summary>
        /// Greater than or equal to
        /// </summary>
        Geq,

        /// <summary>
        /// Less than
        /// </summary>
        Lt,

        /// <summary>
        /// Less than or equal to
        /// </summary>
        Leq,

        /// <summary>
        /// Begins with
        /// </summary>
        BeginsWith,

        /// <summary>
        /// Contains
        /// </summary>
        Contains,

        /// <summary>
        /// Contains any of
        /// </summary>
        ContainsAny,

        /// <summary>
        /// Contains all of
        /// </summary>
        ContainsAll
    }

    public class ContentByQuery
    {
        // query scope

        // ~sitecollection, ~site, ~sitecollection/sub1 to indicate scope from a given subsite or empty (= current site)    
        public string WebUrl { get; set; }
        // will be set whenever the query is scoped to a list in the current web
        public string ListName { get; set; }
        // will be set whenever the query is scoped to a list in the current web
        public string ListGuid { get; set; }
        // contains list template which we're filtering on (e.g. 119 for wiki list). Must be set!
        public string ServerTemplate { get; set; }
        // contains prefix for content type filtering (e.g. 0x010108 for all wiki page content types and descendants)
        public string ContentTypeBeginsWithId { get; set; }

        // extra filters

        // e.g.: {8553196d-ec8d-4564-9861-3dbe931050c8}
        public string FilterField1 { get; set; }
        // e.g.: d
        public string FilterField1Value { get; set; }
        // e.g.: BeginsWith
        public FilterFieldQueryOperator FilterOperator1 { get; set; }
        public FilterChainingOperator Filter1ChainingOperator { get; set; }
        public string FilterField2 { get; set; }
        public string FilterField2Value { get; set; }
        public FilterFieldQueryOperator FilterOperator2 { get; set; }
        public FilterChainingOperator Filter2ChainingOperator { get; set; }
        public string FilterField3 { get; set; }
        public string FilterField3Value { get; set; }
        public FilterFieldQueryOperator FilterOperator3 { get; set; }
        //public FilterChainingOperator Filter3ChainingOperator { get; set; }

        // sorting & grouping

        // e.g. {8553196d-ec8d-4564-9861-3dbe931050c8}
        public string SortBy { get; set; }
        public SortDirection SortByDirection { get; set; }
        // e.g. {8553196d-ec8d-4564-9861-3dbe931050c8}
        public string GroupBy { get; set; }
        public SortDirection GroupByDirection { get; set; }

        // number of items to display (e.g. 15)
        public int ItemLimit { get; set; }

        // number of columns used to present the data (e.g. 1)
        public int DisplayColumns { get; set; }

        //DataMappings (= fields used for the respective Description, ImageUrl, Title and LinkUrl slots) E.g.:
        // 
        //Description:{691b9a4b-512e-4341-b3f1-68914130d5b2},ShortComment,Text;|
        //ImageUrl:{b9e6f3ae-5632-4b13-b636-9d1a2bd67120},EncodedAbsThumbnailUrl,Computed;{543bc2cf-1f30-488e-8f25-6fe3b689d9ac},PublishingRollupImage,Image;|
        //Title:{fa564e0f-0c70-4ab9-b863-0177e6ddd247},Title,Text;|
        //LinkUrl:{94f89715-e097-4e8b-ba79-ea02aa8b7adb},FileRef,Lookup;|              
        public string DataMappings { get; set; }

    }
    #endregion

    /// <summary>
    /// Class used to generate contentrollup (=highlighted content) web part properties coming from either a content by query or content by search web part
    /// </summary>
    public class ContentByQuerySearchTransformator
    {
        private ContentRollupWebPartProperties properties;
        private ClientContext clientContext;
        private const string displayMapJson = "{\"1\":{\"headingText\":{\"sources\":[\"SiteTitle\"]},\"headingUrl\":{\"sources\":[\"SitePath\"]},\"title\":{\"sources\":[\"UserName\",\"Title\"]},\"personImageUrl\":{\"sources\":[\"ProfileImageSrc\"]},\"name\":{\"sources\":[\"Name\"]},\"initials\":{\"sources\":[\"Initials\"]},\"itemUrl\":{\"sources\":[\"WebPath\"]},\"activity\":{\"sources\":[\"ModifiedDate\"]},\"previewUrl\":{\"sources\":[\"PreviewUrl\",\"PictureThumbnailURL\"]},\"iconUrl\":{\"sources\":[\"IconUrl\"]},\"accentColor\":{\"sources\":[\"AccentColor\"]},\"cardType\":{\"sources\":[\"CardType\"]},\"tipActionLabel\":{\"sources\":[\"TipActionLabel\"]},\"tipActionButtonIcon\":{\"sources\":[\"TipActionButtonIcon\"]}},\"2\":{\"column1\":{\"heading\":\"\",\"sources\":[\"FileExtension\"],\"width\":34},\"column2\":{\"heading\":\"Title\",\"sources\":[\"Title\"],\"linkUrls\":[\"WebPath\"],\"width\":250},\"column3\":{\"heading\":\"Modified\",\"sources\":[\"ModifiedDate\"],\"width\":100},\"column4\":{\"heading\":\"Modified By\",\"sources\":[\"Name\"],\"width\":150}},\"3\":{\"id\":{\"sources\":[\"UniqueID\"]},\"edit\":{\"sources\":[\"edit\"]},\"DefaultEncodingURL\":{\"sources\":[\"DefaultEncodingURL\"]},\"FileExtension\":{\"sources\":[\"FileExtension\"]},\"FileType\":{\"sources\":[\"FileType\"]},\"Path\":{\"sources\":[\"Path\"]},\"PictureThumbnailURL\":{\"sources\":[\"PictureThumbnailURL\"]},\"SiteID\":{\"sources\":[\"SiteID\"]},\"SiteTitle\":{\"sources\":[\"SiteTitle\"]},\"Title\":{\"sources\":[\"Title\"]},\"UniqueID\":{\"sources\":[\"UniqueID\"]},\"WebId\":{\"sources\":[\"WebId\"]},\"WebPath\":{\"sources\":[\"WebPath\"]}},\"4\":{\"headingText\":{\"sources\":[\"SiteTitle\"]},\"headingUrl\":{\"sources\":[\"SitePath\"]},\"title\":{\"sources\":[\"UserName\",\"Title\"]},\"personImageUrl\":{\"sources\":[\"ProfileImageSrc\"]},\"name\":{\"sources\":[\"Name\"]},\"initials\":{\"sources\":[\"Initials\"]},\"itemUrl\":{\"sources\":[\"WebPath\"]},\"activity\":{\"sources\":[\"ModifiedDate\"]},\"previewUrl\":{\"sources\":[\"PreviewUrl\",\"PictureThumbnailURL\"]},\"iconUrl\":{\"sources\":[\"IconUrl\"]},\"accentColor\":{\"sources\":[\"AccentColor\"]},\"cardType\":{\"sources\":[\"CardType\"]},\"tipActionLabel\":{\"sources\":[\"TipActionLabel\"]},\"tipActionButtonIcon\":{\"sources\":[\"TipActionButtonIcon\"]}}}";
        private List<Field> queryFields;

        #region Construction
        /// <summary>
        /// Instantiates the class
        /// </summary>
        /// <param name="cc">Client context for the web holding the source page</param>
        public ContentByQuerySearchTransformator(ClientContext cc)
        {
            this.clientContext = cc;
            this.properties = new ContentRollupWebPartProperties();
            this.queryFields = new List<Field>();

            cc.Web.EnsureProperties(p => p.Id, p => p.Url);
            cc.Site.EnsureProperties(p => p.Id);

            // Default initialization of the configuration
            this.properties.WebId = cc.Web.Id.ToString();
            this.properties.SiteId = cc.Site.Id.ToString();
            this.properties.MaxItemsPerPage = 8;
            this.properties.HideWebPartWhenEmpty = false;
            this.properties.Sites = new List<SiteMetadata>();
        }
        #endregion

        /// <summary>
        /// Generate contentrollup (=highlighted content) web part properties coming from a content by query web part
        /// </summary>
        /// <param name="cbq">Properties coming from the content by query web part</param>
        /// <returns>Properties for highlighted content web part</returns>
        public string TransformContentByQueryWebPartToHighlightedContent(ContentByQuery cbq)
        {
            // Transformation logic

            // Scoped to list?
            if (!Guid.TryParse(cbq.ListGuid, out Guid listId))
            {
                listId = Guid.Empty;
            }

            if (!string.IsNullOrEmpty(cbq.ListName) || listId != Guid.Empty)
            {
                // Scope to list
                List list = null;
                if (listId != Guid.Empty)
                {
                    list = this.clientContext.Web.GetListById(listId);
                }
                else
                {
                    list = this.clientContext.Web.GetListByTitle(cbq.ListName);
                }

                var defaultDocLib = this.clientContext.Web.DefaultDocumentLibrary();
                this.clientContext.Load(defaultDocLib, p => p.Id);
                this.clientContext.Load(list, p => p.BaseType, p => p.Title, p => p.Id, p => p.Fields);
                this.clientContext.ExecuteQueryRetry();

                // todo: bail out if not a document library --> should be handled by a selector


                // Set basic list properties
                this.properties.ListId = list.Id.ToString();
                this.properties.LastListId = this.properties.ListId;
                this.properties.ListTitle = list.Title;
                this.properties.DataProviderId = "List";
                //this.properties.DataProviderId = "Search";

                this.properties.IsDefaultDocumentLibrary = defaultDocLib.Id.Equals(list.Id);

                // TODO: verify upper limit bound
                if (cbq.ItemLimit < 1)
                {
                    cbq.ItemLimit = 1;
                }
                this.properties.MaxItemsPerPage = cbq.ItemLimit;

                // Layout properties
                if (cbq.DisplayColumns > 1)
                {
                    SetLayoutTemplate(ContentRollupLayout.Card);
                }
                else
                {
                    SetLayoutTemplate(ContentRollupLayout.List);
                }

                // construct query
                SearchQuery query = new SearchQuery();

                // Libraries always equal to this
                query.ContentLocation = ContentLocation.CurrentSiteDocumentLibrary;

                // There's no document type filtering in CWQP
                query.DocumentTypes.Add(DocumentType.Any);

                // Map contenttypeid to 'default' content types if possible
                query.ContentTypes.AddRange(MapToContentTypes(cbq.ContentTypeBeginsWithId));

                // Filtering
                var filter1 = MapToFilter(list, cbq.FilterOperator1, cbq.FilterField1, cbq.FilterField1Value, FilterChainingOperator.And);
                if (filter1 != null)
                {
                    query.Filters.Add(filter1);
                }

                var filter2 = MapToFilter(list, cbq.FilterOperator2, cbq.FilterField2, cbq.FilterField2Value, cbq.Filter1ChainingOperator);
                if (filter2 != null)
                {
                    query.Filters.Add(filter2);
                }

                var filter3 = MapToFilter(list, cbq.FilterOperator3, cbq.FilterField3, cbq.FilterField3Value, cbq.Filter2ChainingOperator);
                if (filter3 != null)
                {
                    query.Filters.Add(filter3);
                }

                query.AdvancedQueryText = "";

                // Set sort field 
                // Possible sort fields are: Title, FileLeafRef, Author and Modified. Sort direction is always the same (Ascending=\"true\") except for sorting on Modified
                if (!string.IsNullOrEmpty(cbq.SortBy))
                {
                    if (cbq.SortBy.Equals("Modified") || cbq.SortBy.Equals("Title") || cbq.SortBy.Equals("FileLeafRef") || cbq.SortBy.Equals("Author"))
                    {
                        this.properties.DocumentLibrarySortField = cbq.SortBy;
                    }
                    else
                    {
                        // Fall back to default if the original CBQ uses sorting that was not allowed
                        this.properties.DocumentLibrarySortField = "Modified";
                    }
                }

                // assign query
                this.properties.Query = query;

                if (this.properties.Query.Filters.Any())
                {
                    this.properties.Caml = CamlQueryBuilder(list, cbq);
                    
                    // ContentRollup web part first needs to be fixed to have it handle the CAML query generation 
                    //this.properties.Caml = "";
                }
            }
            else
            {
                // scope to site(s)

                // Set basic site properties
                this.properties.DataProviderId = "Search";

                // TODO: verify upper limit bound
                if (cbq.ItemLimit < 1)
                {
                    cbq.ItemLimit = 1;
                }
                this.properties.MaxItemsPerPage = cbq.ItemLimit;

                // Layout properties
                if (cbq.DisplayColumns > 1)
                {
                    SetLayoutTemplate(ContentRollupLayout.Card);
                }
                else
                {
                    SetLayoutTemplate(ContentRollupLayout.List);
                }

                // construct query
                SearchQuery query = new SearchQuery();

                // Libraries always equal to this
                query.ContentLocation = MapToContentLocation(cbq.WebUrl);

                // There's no document type filtering in CWQP
                query.DocumentTypes.Add(DocumentType.Any);

                // Map contenttypeid to 'default' content types if possible
                query.ContentTypes.AddRange(MapToContentTypes(cbq.ContentTypeBeginsWithId));

                // Add default filter element (needed to show up the filters pane in the web part)
                query.Filters.Add(new SearchQueryFilter()
                {
                    FilterType = FilterType.TitleContaining,
                    Value = "",
                });

                // Set sort type - default is MostRecent
                query.SortType = SortType.MostRecent;

                query.AdvancedQueryText = "";

                // assign query
                this.properties.Query = query;
            }

            // Return the json properties for the converted web part
            return HighlightedContentProperties();
        }

        #region Helper methods

        #region CAML Query Builder
        private string CamlQueryBuilder(List list, ContentByQuery cbq)
        {            
            // Copy the CBQW filters
            List<SearchQueryFilter> filters = new List<SearchQueryFilter>();
            filters.AddRange(this.properties.Query.Filters);
            // Add the default filter
            filters.Add(new SearchQueryFilter()
            {
                ChainingOperatorUsedInCQWP = FilterChainingOperator.And,
                Fieldname = "FSObjType",
                Op = FilterOperator.Equals,
                Value = 0
            });

            // Sorting: if CBQW was sorted on one of the 4 allowed fields then take over the setting, else fall back to default sort (= Modified)
            string sortField = "Modified";
            if (!string.IsNullOrEmpty(cbq.SortBy))
            {
                if (cbq.SortBy.Equals("Title") || cbq.SortBy.Equals("FileLeafRef") || cbq.SortBy.Equals("Author"))
                {
                    sortField = cbq.SortBy;
                }
            }

            // Sort order cannot be choosen: Modified = descending, others are ascending
            string sortOrder = "True";
            if (sortField == "Modified")
            {
                sortOrder = "False";
            }

            string query = "";
            Query queryCaml = null;
            var and = LogicalJoin.And();
            var or = LogicalJoin.Or();

            // Do we have filters to apply?
            if (filters.Any())
            {
                for (int i = 0; i < filters.Count; i++)
                {
                    var queryFilter = filters[i];
                    var nextQueryFilter = filters[i];
                    if (i < filters.Count - 1)
                    {
                        nextQueryFilter = filters[i+1];
                    }

                    if (queryFilter.ChainingOperatorUsedInCQWP == FilterChainingOperator.And && nextQueryFilter.ChainingOperatorUsedInCQWP == FilterChainingOperator.And)
                    {
                        and.AddStatement(CamlFilterBuilder(queryFilter));
                    }
                    else
                    {
                        or.AddStatement(CamlFilterBuilder(queryFilter));
                    }
                }

                if (or.HasStatements())
                {
                    and.AddStatement(or);
                }
            }

            queryCaml = Query.Build(and);
            query = queryCaml.GetCaml(true).Replace("\r", "").Replace("\n", "");
            return $"<View Scope=\"RecursiveAll\"><Query>{query}<OrderBy><FieldRef Name=\"{sortField}\" Ascending=\"{sortOrder}\" /></OrderBy></Query><ViewFields><FieldRef Name=\"Editor\" /><FieldRef Name=\"FileLeafRef\" /><FieldRef Name=\"File_x0020_Type\" /><FieldRef Name=\"ID\" /><FieldRef Name=\"Modified\" /><FieldRef Name=\"Title\" /><FieldRef Name=\"UniqueID\" /></ViewFields><RowLimit Paged=\"false\">{cbq.ItemLimit}</RowLimit></View>";
        }

        private string CamlFilterValueBuilder(string fieldName, string fieldValue)
        {
            // default to Text
            string value = fieldValue;

            var field = this.queryFields.Where(p => p.InternalName == fieldName).FirstOrDefault();
            if (field != null)
            {
                if (field.FieldTypeKind == FieldType.User)
                {
                    if (fieldValue.Equals("[me]", StringComparison.InvariantCultureIgnoreCase))
                    {
                        value = "<UserID Type=\"Integer\" />";
                    }
                }
                else if (field.FieldTypeKind == FieldType.DateTime && fieldValue.StartsWith("[today]", StringComparison.InvariantCultureIgnoreCase))
                {
                    string days = fieldValue.ToLower().Replace("[today]", "");

                    if (string.IsNullOrEmpty(days) || !int.TryParse(days, out int offset))
                    {
                        offset = 0;
                    }

                    value = $"<Today OffsetDays=\"{offset}\" />";                    
                }
            }

            return value;
        }

        private CamlBuilder.ValueType CamlFilterValueTypeBuilder(string fieldName, string fieldValue)
        {
            // default to Text
            CamlBuilder.ValueType valueType = CamlBuilder.ValueType.Text;            

            var field = this.queryFields.Where(p => p.InternalName == fieldName).FirstOrDefault();
            if (field != null)
            {
                if (field.FieldTypeKind == FieldType.User)
                {
                    if (fieldValue.Equals("[me]", StringComparison.InvariantCultureIgnoreCase))
                    {
                        valueType = CamlBuilder.ValueType.Integer;
                    }
                    else
                    {
                        valueType = CamlBuilder.ValueType.User;
                    }
                }
                else if (field.FieldTypeKind == FieldType.DateTime && fieldValue.StartsWith("[today]", StringComparison.InvariantCultureIgnoreCase))
                {
                    valueType = CamlBuilder.ValueType.DateTime;
                }
            }

            // Special case
            if (fieldName == "FSObjType")
            {
                valueType = CamlBuilder.ValueType.Integer;
            }

            return valueType;
        }

        private Operator CamlFilterBuilder(SearchQueryFilter queryFilter)
        {
            if (queryFilter.Op == FilterOperator.BeginsWith)
            {
                return Operator.BeginsWith(queryFilter.Fieldname, CamlFilterValueTypeBuilder(queryFilter.Fieldname, queryFilter.Value.ToString()), CamlFilterValueBuilder(queryFilter.Fieldname, queryFilter.Value.ToString()));
            }
            else if (queryFilter.Op == FilterOperator.Contains)
            {
                return Operator.Contains(queryFilter.Fieldname, CamlFilterValueTypeBuilder(queryFilter.Fieldname, queryFilter.Value.ToString()), CamlFilterValueBuilder(queryFilter.Fieldname, queryFilter.Value.ToString()));
            }
            else if (queryFilter.Op == FilterOperator.Equals)
            {
                return Operator.Equal(queryFilter.Fieldname, CamlFilterValueTypeBuilder(queryFilter.Fieldname, queryFilter.Value.ToString()), CamlFilterValueBuilder(queryFilter.Fieldname, queryFilter.Value.ToString()));
            }
            else if (queryFilter.Op == FilterOperator.NotEqual)
            {
                return Operator.NotEqual(queryFilter.Fieldname, CamlFilterValueTypeBuilder(queryFilter.Fieldname, queryFilter.Value.ToString()), CamlFilterValueBuilder(queryFilter.Fieldname, queryFilter.Value.ToString()));
            }
            else if (queryFilter.Op == FilterOperator.GreaterThan)
            {
                return Operator.GreaterThan(queryFilter.Fieldname, CamlFilterValueTypeBuilder(queryFilter.Fieldname, queryFilter.Value.ToString()), CamlFilterValueBuilder(queryFilter.Fieldname, queryFilter.Value.ToString()));
            }
            else if (queryFilter.Op == FilterOperator.LessThan)
            {
                return Operator.LowerThan(queryFilter.Fieldname, CamlFilterValueTypeBuilder(queryFilter.Fieldname, queryFilter.Value.ToString()), CamlFilterValueBuilder(queryFilter.Fieldname, queryFilter.Value.ToString()));
            }

            return null;
        }
        #endregion
        
        private ContentLocation MapToContentLocation(string webUrl)
        {
            // ~sitecollection/sub1 scoping is not supported...fall back to complete site collection
            // TODO: add to log?
            if (string.IsNullOrEmpty(webUrl) || webUrl.StartsWith("~sitecollection", StringComparison.InvariantCultureIgnoreCase))
            {
                return ContentLocation.CurrentSiteCollection;
            }
            else if (webUrl.Equals("~site", StringComparison.InvariantCultureIgnoreCase))
            {
                return ContentLocation.CurrentSite;
            }

            return ContentLocation.CurrentSiteCollection;
        }

        private List<ContentType> MapToContentTypes(string contentTypeId)
        {
            List<ContentType> cts = new List<ContentType>();

            // some easy matching
            if (contentTypeId.StartsWith("0x0101"))
            {
                // Base document content type
                cts.Add(ContentType.Document);
            }
            else
            {
                cts.Add(ContentType.All);
            }

            return cts;
        }

        private SearchQueryFilter MapToFilter(List list, FilterFieldQueryOperator filterOperator, string filterField, string filterFieldValue, FilterChainingOperator? chainingOperatorUsedInCQWP)
        {
            if (string.IsNullOrEmpty(filterField))
            {
                return null;
            }

            var foundFields = list.Context.LoadQuery(list.Fields.Where(item => item.InternalName == filterField));
            list.Context.ExecuteQueryRetry();

            if (foundFields.FirstOrDefault() != null)
            {
                // Store field for future reference
                this.queryFields.Add(foundFields.FirstOrDefault());

                // Resolve user ID to name
                if (foundFields.FirstOrDefault().FieldTypeKind == FieldType.User)
                {
                    //[11;#i:0#.f|membership|kevinc@bertonline.onmicrosoft.com]
                    if (filterFieldValue.Contains("|"))
                    {
                        // grab UPN
                        string[] accountParts = filterFieldValue.Replace("[", "").Replace("]", "").Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        //string upn = accountParts[accountParts.Length - 1];
                        if (int.TryParse(accountParts[0], out int userId))
                        {
                            var user = this.clientContext.Web.GetUserById(userId);
                            this.clientContext.Load(user, p => p.Title);
                            this.clientContext.ExecuteQueryRetry();
                            filterFieldValue = user.Title;
                        }
                    }
                }

                // Build the search query filter object
                var s = new SearchQueryFilter
                {
                    Value = filterFieldValue,
                    FilterType = FilterType.Field,
                    Op = MapQueryFilterOperator(filterOperator),
                    Fieldname = foundFields.FirstOrDefault().InternalName,
                    ChainingOperatorUsedInCQWP = chainingOperatorUsedInCQWP,
                    FieldNameMatchText = ""
                };

                return s;
            }

            return null;
        }

        private FilterOperator? MapQueryFilterOperator(FilterFieldQueryOperator filterOperator)
        {
            switch(filterOperator)
            {
                case FilterFieldQueryOperator.BeginsWith:
                    {
                        return FilterOperator.BeginsWith;
                    }
                case FilterFieldQueryOperator.Contains:
                    {
                        return FilterOperator.Contains;
                    }
                case FilterFieldQueryOperator.Eq:
                    {
                        return FilterOperator.Equals;
                    }
                case FilterFieldQueryOperator.Neq:
                    {
                        return FilterOperator.NotEqual;
                    }
                case FilterFieldQueryOperator.Gt:
                case FilterFieldQueryOperator.Geq:
                    {
                        return FilterOperator.GreaterThan;
                    }
                case FilterFieldQueryOperator.Lt:
                case FilterFieldQueryOperator.Leq:
                    {
                        return FilterOperator.LessThan;
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        private void SetLayoutTemplate(ContentRollupLayout template)
        {
            this.properties.TemplateId = template;
            this.properties.LayoutId = template.ToString();
        }

        internal string HighlightedContentProperties()
        {
            // Don't serialize null values
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(this.properties, jsonSerializerSettings);

            // Embed DisplayMaps via a placeholder replace
            json = json.Replace("\"%%DisplayMapsPlaceholder%%\"", displayMapJson);
            return json;
        }
        #endregion

    }
}
