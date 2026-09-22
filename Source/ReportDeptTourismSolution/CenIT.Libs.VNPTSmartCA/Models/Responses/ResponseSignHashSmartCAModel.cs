using Newtonsoft.Json;

namespace CenIT.Libs.VNPTSmartCA.Models.Responses
{
    public class ContenSignHash
    {
        [JsonProperty(PropertyName = "tranId")]
        public string TranId { get; set; }
    }

    public class ResponseSignHashSmartCAModel
    {
        [JsonProperty(PropertyName = "code")] public int Code { get; set; }

        [JsonProperty(PropertyName = "codeDesc")]
        public string CodeDesc { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "content")]
        public ContenSignHash Content { get; set; }
    }
}