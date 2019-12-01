namespace SearchQueryTool.Model
{
    public class SearchResultPresentationSettings
    {
        public const string DefaultFormat = "{counter}. {Title}";

        public string PrimaryResultsTitleFormat { get; set; } = DefaultFormat;
    }
}
