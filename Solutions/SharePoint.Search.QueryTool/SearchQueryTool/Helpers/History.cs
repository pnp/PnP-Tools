using System;
using System.IO;
using System.Linq;
using System.Windows;
using SearchQueryTool.Model;

namespace SearchQueryTool.Helpers
{
    public class History
    {
        private readonly MainWindow _mainWindow;

        public History(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        internal void RefreshHistoryButtonState()
        {
            if (_mainWindow.SearchHistory.CanNavigateBack())
            {
                _mainWindow.BackButton.Opacity = 100.0;
                _mainWindow.BackButton.IsEnabled = true;
            }
            else
            {
                _mainWindow.BackButton.Opacity = 10.0;
                _mainWindow.BackButton.IsEnabled = false;
            }

            if (_mainWindow.SearchHistory.CanNavigateForward())
            {
                _mainWindow.ForwardButton.Opacity = 100.0;
                _mainWindow.ForwardButton.IsEnabled = true;
            }
            else
            {
                _mainWindow.ForwardButton.Opacity = 10.0;
                _mainWindow.ForwardButton.IsEnabled = false;
            }
        }

        internal void LoadLatestHistory(string folderPath)
        {
            try
            {
                const string pattern = "*.xml";

                var latestHistory = new DirectoryInfo(folderPath).GetFiles(pattern, SearchOption.TopDirectoryOnly)
                    .OrderByDescending(x => x.LastWriteTime)
                    .DefaultIfEmpty(null)
                    .FirstOrDefault();

                if (latestHistory != null)
                {
                    _mainWindow.LoadPreset(latestHistory.FullName);

                    var status = $"Loaded history from {folderPath}";
                    _mainWindow.StateBarTextBlock.Text = status;
                }
            }
            catch (Exception ex)
            {
                var status = $"Failed to load history from folder {folderPath}: {ex.Message}";
                _mainWindow.StateBarTextBlock.Text = status;
            }
        }

        internal void PruneHistoryDir(string folderPath, int maxHistoryFiles = 1000)
        {
            if (InfiniteHistoryEnabled(maxHistoryFiles))
                return;

            DeleteOldest(folderPath, maxHistoryFiles);
        }

        private void DeleteOldest(string folderPath, int maxHistoryFiles)
        {
            if (string.IsNullOrEmpty(folderPath)) return;

            try
            {
                const string pattern = "*.xml";
                var numHistoryFiles = Directory.GetFiles(folderPath, pattern, SearchOption.TopDirectoryOnly).Length;
                if (!CleanupNecessary(maxHistoryFiles, numHistoryFiles)) return;

                foreach (var file in new DirectoryInfo(folderPath).GetFiles(pattern, SearchOption.TopDirectoryOnly)
                        .OrderByDescending(x => x.LastWriteTime)
                        .Skip(maxHistoryFiles))
                {
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                var status = $"Failed to clean up history folder {folderPath}: {ex.Message}";
                _mainWindow.StateBarTextBlock.Text = status;
            }
        }

        private static bool CleanupNecessary(int maxHistoryFiles, int numHistoryFiles)
        {
            return numHistoryFiles > maxHistoryFiles;
        }

        private static bool InfiniteHistoryEnabled(int maxHistoryFiles)
        {
            return maxHistoryFiles <= 0;
        }

        internal void SaveHistoryItem()
        {
            var filename = $"history-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xml";
            var path = Path.Combine(_mainWindow.HistoryFolderPath, filename);
            var preset = new SearchPreset()
            {
                Request = _mainWindow.GetSearchQueryRequestFromUi(),
                Connection = _mainWindow.GetSearchConnectionFromUi(),
                Path = path,
                Name = Path.GetFileNameWithoutExtension(filename)
            };

            var unused = preset.Save();
        }

        internal void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            _mainWindow.SearchHistory.NavigateBack();
            _mainWindow.LoadPreset(_mainWindow.SearchHistory.Current);
            RefreshHistoryButtonState();
        }

        internal void ForwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            _mainWindow.SearchHistory.NavigateForward();
            _mainWindow.LoadPreset(_mainWindow.SearchHistory.Current);
            RefreshHistoryButtonState();
        }

        internal void Clear_OnClick(object sender, RoutedEventArgs e)
        {
            _mainWindow.SearchHistory.Clear();
            RefreshHistoryButtonState();
        }
    }
}