using System.IO;
using System.Text.RegularExpressions;
using SearchQueryTool.Helpers;
using SearchQueryTool.Model;

namespace SearchQueryTool
{
    public class Init
    {
        private readonly MainWindow _mainWindow;

        public Init(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            InitializeControls();
            InitializeHistory();
            InitializePresets();
        }

        public void InitializeControls()
        {
            _mainWindow.SetCurrentWindowsUserIdentity();

            // Only load connection properties on initial load to avoid overriding settings in preset
            if (_mainWindow._firstInit)
            {
                _mainWindow._searchConnection = _mainWindow.LoadSearchConnection();
                _mainWindow.ConnectionExpanderBox.IsExpanded = true;
                _mainWindow._firstInit = false;
            }

            _mainWindow.UpdateRequestUriStringTextBlock();
            _mainWindow.UpdateSearchQueryRequestControls(_mainWindow._searchQueryRequest);
            _mainWindow.UpdateSearchConnectionControls(_mainWindow._searchConnection);

            //Enable the cross access to this collection elsewhere - see: http://stackoverflow.com/questions/14336750/upgrading-to-net-4-5-an-itemscontrol-is-inconsistent-with-its-items-source
            _mainWindow.DebugGrid.ItemsSource = _mainWindow.ObservableQueryCollection;
        }

        private void InitializeHistory()
        {
            _mainWindow.HistoryFolderPath = _mainWindow.InitDirectoryFromSetting("HistoryFolderPath", @".\History");
            var historyMaxFiles = MainWindow.ReadSettingInt("HistoryMaxFiles", 1000);

            _mainWindow.History = new History(_mainWindow);
            _mainWindow.History.PruneHistoryDir(_mainWindow.HistoryFolderPath, historyMaxFiles);
            _mainWindow.SearchHistory = new SearchHistory(_mainWindow.HistoryFolderPath);
            _mainWindow.History.RefreshHistoryButtonState();
            _mainWindow.History.LoadLatestHistory(_mainWindow.HistoryFolderPath);
        }

        private void InitializePresets()
        {
            var tmpPath = MainWindow.ReadSetting("PresetsFolderPath");
            if (!Regex.IsMatch(tmpPath, @"^\w", RegexOptions.IgnoreCase) && !tmpPath.StartsWith(@"\"))
            {
                tmpPath = Path.GetFullPath(tmpPath);
            }

            _mainWindow.PresetFolderPath = (!string.IsNullOrEmpty(tmpPath)) ? tmpPath : Path.GetFullPath(@".\Presets");
            if (Directory.Exists(tmpPath)) return;
            try
            {
                Directory.CreateDirectory(tmpPath);
            }
            catch
            {
                // ignored
            }
        }
    }
}