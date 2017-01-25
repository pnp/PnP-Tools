using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SharePointPnP.DeveloperTools.Common.Helpers
{
    class XmlHelper
    {
        public static T ReadXml<T>(string filename) 
        where T : class
        {
            T res = default(T);
			try
			{
				using (StreamReader reader = new StreamReader(filename))
				{
					XDocument doc = XDocument.Load(reader);
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
					using (var xreader = doc.Root.CreateReader())
					{
						res = (T)xmlSerializer.Deserialize(xreader);
					}
				}
			}
			catch(FileNotFoundException)
			{

			}
            return res;
        }

        public static void WriteXml(object obj, string filename, XmlSerializerNamespaces ns = null)
        {
			string xml = null;

			using (StringWriter sw = new StringWriter())
			{
				XmlSerializer xs = new XmlSerializer(obj.GetType());
				if (ns != null)
				{
					xs.Serialize(sw, obj, ns);
				}
				else
				{
					xs.Serialize(sw, obj);
				}
				xml = sw.ToString();
			}

			using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.Write(xml);
                writer.Close();
            }
        }
    }
}
