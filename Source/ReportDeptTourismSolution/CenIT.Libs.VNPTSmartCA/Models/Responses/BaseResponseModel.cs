using Newtonsoft.Json;

namespace CenIT.Libs.VNPTSmartCA.Models.Responses
{
    public class BaseResponseModel
    {
        [JsonProperty(PropertyName = "code")] public int Code { get; set; }

        [JsonProperty(PropertyName = "codeDesc")]
        public string CodeDesc { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        //[JsonProperty(PropertyName = "content")]
        //public virtual object Content { get; set; }
    }
}