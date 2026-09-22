using System.Xml;

namespace CenIT.ReportTourism.Core.Helpers
{
    public class XmlHelper
    {
        public void CombileContentXmlFile(string xmlSourceFilePath, string xmlDestFilePath)
        {
            var sourceDoc = new XmlDocument();
            sourceDoc.Load(xmlSourceFilePath);
            var destDoc = new XmlDocument();
            destDoc.Load(xmlDestFilePath);

            if (destDoc.DocumentElement?.ChildNodes != null)
                foreach (XmlNode childEl in destDoc.DocumentElement?.ChildNodes)
                {
                    var newNode = sourceDoc.ImportNode(childEl, true);
                    sourceDoc.DocumentElement?.AppendChild(newNode);
                }

            sourceDoc.Save(xmlDestFilePath);
        }
    }
}