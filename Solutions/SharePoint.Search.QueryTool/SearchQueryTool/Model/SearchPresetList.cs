using System;
using System.Collections.Generic;
using System.IO;

namespace SearchQueryTool.Model
{
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

        private void ReadFromFolderPath(string folderPath = @".\Presets")
        {
            try
            {
                foreach (var file in Directory.EnumerateFiles(folderPath, "*.xml"))
                {
                    AddPreset(file);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void AddPreset(string file)
        {

            var preset = new SearchPreset(file);
            Presets.Add(preset);
        }
    }
}
