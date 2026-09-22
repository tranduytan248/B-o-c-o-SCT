using System.Collections.Generic;
using Newtonsoft.Json;

namespace CenIT.Libs.VNPTSmartCA.Models.Responses
{
    public class CertModel
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "serialNumber")]
        public string SerialNumber { get; set; }

        [JsonProperty(PropertyName = "subjectDN")]
        public string SubjectDN { get; set; }

        [JsonProperty(PropertyName = "issuerDN")]
        public string IssuerDN { get; set; }

        [JsonProperty(PropertyName = "certificates")]
        public List<string> Certificates { get; set; }

        [JsonProperty(PropertyName = "validFrom")]
        public string ValidFrom { get; set; }

        [JsonProperty(PropertyName = "validTo")]
        public string ValidTo { get; set; }
    }

    public class KeyModel
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "alg")] public List<string> Alg { get; set; }

        [JsonProperty(PropertyName = "len")] public int Len { get; set; }
    }

    public class ResponseCredentialInfoModel
    {
        [JsonProperty(PropertyName = "cert")] public CertModel Cert { get; set; }

        [JsonProperty(PropertyName = "key")] public KeyModel Key { get; set; }

        [JsonProperty(PropertyName = "authMode")]
        public string AuthMode { get; set; }

        [JsonProperty(PropertyName = "scal")] public string Scal { get; set; }

        [JsonProperty(PropertyName = "multisign")]
        public int Multisign { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }
}