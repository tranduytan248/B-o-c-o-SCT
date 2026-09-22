using System.Collections;
using System.IO;
using System.Xml;

namespace CenIT.ReportTourism.Core.Helpers
{
    public static class ConfigHelper
    {
        public static Hashtable GetSettingsByPath(string path, string settingKey = "appSettings")
        {
            var ret = new Hashtable();
            if (!File.Exists(path)) return ret;
            var reader = new StreamReader
            (
                new FileStream(
                    path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read)
            );
            var doc = new XmlDocument();
            var xmlIn = reader.ReadToEnd();
            reader.Close();
            doc.LoadXml(xmlIn);
            foreach (XmlNode child in doc.ChildNodes)
                if (child.Name.Equals(settingKey))
                    foreach (XmlNode node in child.ChildNodes)
                        if (node.Name.Equals("add"))
                            ret.Add
                            (
                                node.Attributes?["key"].Value,
                                node.Attributes?["value"].Value
                            );
            return ret;
        }
    }
}