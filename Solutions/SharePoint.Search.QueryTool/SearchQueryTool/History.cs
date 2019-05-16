using System;
using System.IO;
using System.Linq;
using System.Windows;
using SearchQueryTool.Model;

namespace SearchQueryTool
{
    public class History
    {
        private MainWindow _mainWindow;

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
                    // Initialize the user interface 
                    _mainWindow.LoadPreset(latestHistory.FullName);

                    var status = String.Format("Loaded history from {0}", folderPath);
                    _mainWindow.StateBarTextBlock.Text = status;
                }
            }
            catch (Exception ex)
            {
                var status = String.Format("Failed to load history from folder {0}: {1}", folderPath, ex.Message);
                _mainWindow.StateBarTextBlock.Text = status;
            }
        }

        internal void PruneHistoryDir(string folderPath, int maxHistoryFiles = 1000)
        {
            // No pruning if max is 0 or lower (this means we should have infinite history)
            if (maxHistoryFiles <= 0)
                return;

            // Delete the N oldest files
            if (!String.IsNullOrEmpty(folderPath))
            {
                try
                {
                    const string pattern = "*.xml";
                    var numHistoryFiles = Directory.GetFiles(folderPath, pattern, SearchOption.TopDirectoryOnly).Length;
                    if (numHistoryFiles > maxHistoryFiles)
                    {
                        foreach (
                            var file in
                            new DirectoryInfo(folderPath).GetFiles(pattern, SearchOption.TopDirectoryOnly)
                                .OrderByDescending(x => x.LastWriteTime)
                                .Skip(maxHistoryFiles))
                        {
                            file.Delete();
                        }

                    }
                }
                catch (Exception ex)
                {
                    var status = String.Format("Failed to clean up history folder {0}: {1}", folderPath, ex.Message);
                    _mainWindow.StateBarTextBlock.Text = status;
                }
            }
        }

        internal void SaveHistoryItem()
        {
            var filename = string.Format("history-{0:yyyy-MM-dd_HH-mm-ss}.xml", DateTime.Now);
            var path = Path.Combine(_mainWindow.HistoryFolderPath, filename);
            var preset = new SearchPreset()
            {
                Request = _mainWindow.GetSearchQueryRequestFromUi(),
                Connection = _mainWindow.GetSearchConnectionFromUi(),
                Path = path,
                Name = Path.GetFileNameWithoutExtension(filename)
            };

            var r = preset.Save();
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
    }
}