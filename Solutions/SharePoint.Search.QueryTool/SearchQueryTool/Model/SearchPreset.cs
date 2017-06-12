using System;
using System.IO;
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

        /// <summary>
        /// The path to the XML file on disk that represents this search preset.
        /// </summary>
        [XmlIgnore] 
        public string Path { get; set; }

        /// <summary>
        /// All options for a search request.
        /// </summary>
        public SearchQueryRequest Request { get; set; }

        /// <summary>
        /// All connection related options for a search query. 
        /// </summary>
        public SearchConnection Connection { get; set; }

        public SearchPreset()
        {
            Name = "New preset";
        }

        /// <summary>
        /// Constructor. Generate a preset object from a file found in the given relative or absolute path.
        /// </summary>
        /// <param name="path"></param>
        public SearchPreset(string path, bool rethrowException = false)
        {
            Load(path, rethrowException);
        }

        /// <summary>
        /// Save a preset to disk to the Path indicated in the Preset object. 
        /// </summary>
        /// <returns>True if successfully saved, false otherwise</returns>
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

        /// <summary>
        /// Load a preset object as human-readable XML from disk.
        /// </summary>
        /// <param name="xmlFilePath">Path to load XML from</param>
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
    }
}
