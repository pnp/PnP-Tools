using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using SearchQueryTool.Helpers;

namespace SearchQueryTool.Model
{
    /// <summary>
    /// One search preset item. These are the individual entries shown in the preset dropdown list. Each entry 
    /// includes an object with a full search query with all settings and a link to where this preset is stored
    /// as XML on disk. The name is just the filename without any extension.
    /// </summary>
    public class SearchPreset
    {
        /// <summary>
        /// The name of the search preset (generated automatically from filename).
        /// </summary>
        public string Name { get; set; }

        public string Annotation { get; set; }

        /// <summary>
        /// The path to the XML file on disk that represents this search preset.
        /// </summary>
        [XmlIgnore] 
        public string Path { get; set; }

        /// <summary>
        /// All options for a search request.
        /// </summary>
        public SearchQueryRequest Request { get; set; }

        public SearchResultPresentationSettings PresentationSettings { get; set; }

        /// <summary>
        /// All connection related options for a search query. 
        /// </summary>
        public SearchConnection Connection { get; set; }

        public SearchPreset()
        {
            Name = "New preset";
        }

        public SearchPreset(string path, bool rethrowException = false)
        {
            Load(path, rethrowException);
        }

        public bool Save()
        {
            bool r;
            try
            {
                var serializer = new XmlSerializer(typeof(SearchPreset));
                using (var writer = new StringWriter())
                {
                    serializer.Serialize(writer, this);
                    var xml = writer.ToString();
                    File.WriteAllText(Path, XmlHelper.PrettyXml(xml));
                }
                r = true;
            }
            catch (Exception)
            {
                r = false;
            }
            return r;
        }

        public bool Include(string filter)
        {
            var result = true;
            if (!string.IsNullOrWhiteSpace(filter))
            {
                var name = this.Name;
                filter = NormalizeWhitespace(filter);
                var nameTokens = GetTokens(name.ToLowerInvariant());
                var filterTokens = GetTokens(filter.ToLowerInvariant());
                result = ContainsAllItems(nameTokens, filterTokens);
            }
            return result;
        }

        private void Load(string path, bool rethrowException = false)
        {
            if (!String.IsNullOrWhiteSpace(path))
            {
                Path = path;
                try
                {
                    Name = System.IO.Path.GetFileNameWithoutExtension(Path);
                    DeserializeSearchPreset(Path);
                }
                catch (Exception)
                {
                    if (rethrowException)
                    {
                        throw;
                    }

                    Name = Name + " (failed)";
                }
            }
        }

        private void DeserializeSearchPreset(string xmlFilePath)
        {
            var serializer = new XmlSerializer(typeof(SearchPreset));
            using (var reader = new StreamReader(xmlFilePath))
            {
                var preset = serializer.Deserialize(reader) as SearchPreset;
                if (preset != null)
                {
                    Request = preset.Request;
                    Connection = preset.Connection;
                    Path = xmlFilePath;
                    Name = preset.Name;
                }
            }
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
