using Newtonsoft.Json;

namespace CenIT.Libs.VNPTSmartCA.Models.Responses
{
    public class ResponseTranInfoSmartCAModel
    {
        [JsonProperty(PropertyName = "code")] public int Code { get; set; }

        [JsonProperty(PropertyName = "codeDesc")]
        public string CodeDesc { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "content")]
        public ResponseTranInfoSmartCARespContentModel Content { get; set; }
    }
}