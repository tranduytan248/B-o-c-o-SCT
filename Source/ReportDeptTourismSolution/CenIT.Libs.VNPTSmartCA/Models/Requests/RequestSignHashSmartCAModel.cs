using System.Collections.Generic;
using Newtonsoft.Json;

namespace CenIT.Libs.VNPTSmartCA.Models.Requests
{
    public class DataSignHash
    {
        [JsonProperty(PropertyName = "name")] public string Name { get; set; }

        [JsonProperty(PropertyName = "hash")] public string Hash { get; set; }
    }

    public class DataSign
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "dataBase64")]
        public string DataBase64 { get; set; }
    }

    public class RequestSignHashSmartCAModel
    {
        [JsonProperty(PropertyName = "credentialId")]
        public string CredentialId { get; set; }

        [JsonProperty(PropertyName = "refTranId")]
        public string RefTranId { get; set; }

        [JsonProperty(PropertyName = "notifyUrl")]
        public string NotifyUrl { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "datas")] public List<DataSignHash> Datas { get; set; }
    }

    public class RequestSignSmartCAModel
    {
        [JsonProperty(PropertyName = "credentialId")]
        public string CredentialId { get; set; }

        //[JsonProperty(PropertyName = "refTranId")]
        //public string RefTranId { get; set; }

        //[JsonProperty(PropertyName = "notifyUrl")]
        //public string NotifyUrl { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "datas")]
        public List<DataSign> Datas { get; set; }
    }
}