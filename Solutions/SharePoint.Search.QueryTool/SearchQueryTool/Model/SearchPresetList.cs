using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SearchQueryTool.Model
{
    /// <summary>
    /// Search preset list. Model for the dropdown with stored presets containing all search settings like
    /// QueryText, server and all search parameters. 
    /// </summary>
    public class SearchPresetList
    {
        private string PresetFolderPath { get; set; }
        public List<SearchPreset> Presets;

        public SearchPresetList(string presetFolderPath)
        {
            PresetFolderPath = presetFolderPath;
            Presets = new List<SearchPreset>();
            ReadFromFolderPath(PresetFolderPath);
        }

        /// <summary>
        /// Traverse a directory and load all preset XML files stored there into a list of presets.
        /// </summary>
        /// <param name="folderPath">Path to load presets from. Default is .\Presets.</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool ReadFromFolderPath(string folderPath=@".\Presets")
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
