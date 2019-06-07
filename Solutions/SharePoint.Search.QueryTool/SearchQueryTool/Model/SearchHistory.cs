using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SearchQueryTool.Model
{
    public class SearchHistory
    {
        /// <summary>
        /// Path to the folder where we find History items.
        /// </summary>
        public string HistoryFolderPath;
        
        /// <summary>
        /// List of history items loaded from the HistoryFolderPath. These are actually Preset objects that 
        /// can populate the user interface. 
        /// </summary>
        public List<SearchPreset> Presets;

        /// <summary>
        /// Path to the currently selected history item. Use NavigateBack and NavigateForward to 
        /// traverse the list. 
        /// </summary>
        public string Current { get; set; }

        /// <summary>
        /// Constructor. Create a search history based on the given folder. 
        /// </summary>
        /// <param name="historyFolderPath">The folder to read history items from.</param>
        public SearchHistory(string historyFolderPath)
        {
            HistoryFolderPath = historyFolderPath;
            Presets = new List<SearchPreset>();
            ReadFromFolderPath(HistoryFolderPath);

            Current = (Presets.Count > 0) ? Presets.Last().Path : null;
        }

        /// <summary>
        /// Check if we can navigate backwards. We can navigate backwards until we 
        /// reach the 0th position. 
        /// </summary>
        /// <returns>True if we can navigate backwards, false otherwise.</returns>
        public bool CanNavigateBack()
        {
            if (null == Current)
                return false; 

            var currentIdx = Presets.FindIndex(item => (item.Path == Current));
            return (currentIdx != 0);
        }

        
        /// <summary>
        /// Navigate backwards in the current search history. 
        /// </summary>
        public void NavigateBack()
        {
            if (CanNavigateBack())
            {
                var currentIdx = Presets.FindIndex(item => (item.Path == Current));
                currentIdx--;
                Current = Presets[currentIdx].Path;
            }
        }

        /// <summary>
        /// Check if we can navigate forward from the current position. We can navigate
        /// forward as long as we are not in the last position. 
        /// </summary>
        /// <returns>True if we can navigate forwards, false otherwise.</returns>
        public bool CanNavigateForward()
        {
            if (null == Current)
                return false;
            
            var currentIdx = Presets.FindIndex(item => (item.Path == Current));
            var lastIdx = (Presets.Count - 1);
            return (currentIdx != lastIdx);
        }

        /// <summary>
        /// Navigate forward in the current search history. 
        /// </summary>
        public void NavigateForward()
        {
            if (CanNavigateForward())
            {
                var currentIdx = Presets.FindIndex(item => (item.Path == Current));
                currentIdx++;
                Current = Presets[currentIdx].Path;
            }
        }

        /// <summary>
        /// Traverse a directory and load all history XML files stored there into a list of presets.
        /// </summary>
        /// <param name="folderPath">Path to load history from. Default is .\History.</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool ReadFromFolderPath(string folderPath = @".\History")
        {
            bool ret;
            try
            {
                foreach (var file in Directory.EnumerateFiles(folderPath, "*.xml"))
                {
                    var preset = new SearchPreset(file);
                    Presets.Add(preset);
                }
                ret = true;
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }
    }
}
