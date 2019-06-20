using System.Windows;

namespace SearchQueryTool
{
    public partial class SearchPresentationSettings : Window
    {
        private readonly MainWindow _mainWindow;

        public SearchPresentationSettings()
        {
            InitializeComponent();
            _mainWindow = ((MainWindow)Application.Current.MainWindow);
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TitleFormatTextBox.Text = _mainWindow.SearchPresentationSettings.PrimaryResultsTitleFormat;
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            _mainWindow.SearchPresentationSettings.PrimaryResultsTitleFormat = TitleFormatTextBox.Text;
            Close();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
