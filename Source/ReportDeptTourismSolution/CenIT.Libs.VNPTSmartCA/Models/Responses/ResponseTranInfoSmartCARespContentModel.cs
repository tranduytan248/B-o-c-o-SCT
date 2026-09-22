using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CenIT.Libs.VNPTSmartCA.Models.Responses
{
    public class DocumentResp
    {
        [JsonProperty(PropertyName = "name")] public string Name { get; set; }

        [JsonProperty(PropertyName = "type")] public string Type { get; set; }

        [JsonProperty(PropertyName = "size")] public string Size { get; set; }

        [JsonProperty(PropertyName = "data")] public string Data { get; set; }

        [JsonProperty(PropertyName = "hash")] public string Hash { get; set; }

        [JsonProperty(PropertyName = "sig")] public string Sig { get; set; }

        [JsonProperty(PropertyName = "signature")]
        public string Signature { get; set; }

        [JsonProperty(PropertyName = "dataSigned")]
        public string DataSigned { get; set; }

        [JsonProperty(PropertyName = "url")] public string Url { get; set; }
    }

    public class ResponseTranInfoSmartCARespContentModel
    {
        [JsonProperty(PropertyName = "refTranId")]
        public string RefTranId { get; set; }

        [JsonProperty(PropertyName = "tranId")]
        public string TranId { get; set; }

        [JsonProperty(PropertyName = "sub")] public string Sub { get; set; }

        [JsonProperty(PropertyName = "credentialId")]
        public string CredentialId { get; set; }

        [JsonProperty(PropertyName = "tranType")]
        public int TranType { get; set; }

        [JsonProperty(PropertyName = "tranTypeDesc")]
        public string TranTypeDesc { get; set; }

        [JsonProperty(PropertyName = "tranStatus")]
        public int TranStatus { get; set; }

        [JsonProperty(PropertyName = "transStatusDesc")]
        public string TransStatusDesc { get; set; }

        [JsonProperty(PropertyName = "reqTime")]
        public DateTime? ReqTime { get; set; }

        [JsonProperty(PropertyName = "documents")]
        public List<DocumentResp> Documents { get; set; }
    }
}