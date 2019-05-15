using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SearchQueryTool.Model
{
    public class SearchPresetList
    {
        private string PresetFolderPath { get; set; }
        public List<SearchPreset> Presets;

        public SearchPresetList(string presetFolderPath, string presetFilter)
        {
            PresetFolderPath = presetFolderPath;
            Presets = new List<SearchPreset>();
            ReadFromFolderPath(PresetFolderPath, presetFilter);
        }

        private void ReadFromFolderPath(string folderPath = @".\Presets", string filter = null)
        {
            try
            {
                foreach (var file in Directory.EnumerateFiles(folderPath, "*.xml"))
                {
                    AddPreset(file, filter);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void AddPreset(string file, string filter)
        {
            if (HasFilter(filter))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                if (!Include(name, filter))
                    return;
            }

            var preset = new SearchPreset(file);
            Presets.Add(preset);
        }

        private static bool HasFilter(string filter)
        {
            return !string.IsNullOrEmpty(filter);
        }

        private static bool Include(string name, string filter)
        {
            filter = NormalizeWhitespace(filter);
            var nameTokens = GetTokens(name.ToLowerInvariant());
            var filterTokens = GetTokens(filter.ToLowerInvariant());
            var result = ContainsAllItems(nameTokens, filterTokens);
            return result;
        }

        public static bool ContainsAllItems(List<string> a, List<string> b)
        {
            return !b.Except(a).Any();
        }

        private static List<string> GetTokens(string input)
        {
            return input.Split(' ').ToList();
        }

        private static string NormalizeWhitespace(string presetFilter)
        {
            return Regex.Replace(presetFilter, @"\s+", " ");
        }
    }
}
