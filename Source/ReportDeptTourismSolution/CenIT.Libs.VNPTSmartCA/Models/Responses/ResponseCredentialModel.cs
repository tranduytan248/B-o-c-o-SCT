using System.Collections.Generic;
using Newtonsoft.Json;

namespace CenIT.Libs.VNPTSmartCA.Models.Responses
{
    public class ResponseCredentialModel : BaseResponseModel
    {
        [JsonProperty(PropertyName = "content")]
        public List<string> Content { get; set; }
    }
}