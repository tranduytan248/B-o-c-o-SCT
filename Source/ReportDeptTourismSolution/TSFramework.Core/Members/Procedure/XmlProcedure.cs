using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TSFramework.Core.Members.Procedure
{
    [XmlRoot("Provider")]
    [Serializable]
    public class XmlProviders
    {
        [XmlElement(ElementName = "StoredProcedures")]
        public List<XmlProcedure> Procedures { get; set; }
    }

    [XmlRoot("XmlProcedure")]
    [Serializable]
    public class XmlProcedure
    {
        [XmlAttribute("ProviderName")] public string ProviderName { get; set; }

        [XmlElement(ElementName = "Procedure")]
        public List<Procedure> Procedures { get; set; }
    }

    [XmlRoot("Procedure")]
    [Serializable]
    public class Procedure
    {
        [XmlAttribute("Name")] public string Name { get; set; }

        [XmlAttribute("Value")] public string Value { get; set; }
    }
}