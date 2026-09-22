using System;
using Newtonsoft.Json;

namespace CenIT.Libs.VNPTSmartCA.Models.Responses
{
    public class UserInfoModel
    {
        [JsonProperty(PropertyName = "accType")]
        public int? AccType { get; set; }

        [JsonProperty(PropertyName = "accTypeDesc")]
        public object AccTypeDesc { get; set; }

        [JsonProperty(PropertyName = "uidPre")]
        public object UidPre { get; set; }

        [JsonProperty(PropertyName = "uid")] public string Uid { get; set; }

        [JsonProperty(PropertyName = "username")]
        public object Username { get; set; }

        [JsonProperty(PropertyName = "email")] public string Email { get; set; }

        [JsonProperty(PropertyName = "phone")] public string Phone { get; set; }

        [JsonProperty(PropertyName = "fullName")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "gender")]
        public int? Gender { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonProperty(PropertyName = "status")]
        public int? Status { get; set; }

        [JsonProperty(PropertyName = "statusDesc")]
        public string StatusDesc { get; set; }

        [JsonProperty(PropertyName = "device")]
        public object Device { get; set; }
    }

    public class ResponseUserInfoModel : BaseResponseModel
    {
        [JsonProperty(PropertyName = "content")]
        public UserInfoModel Content { get; set; }
    }
}