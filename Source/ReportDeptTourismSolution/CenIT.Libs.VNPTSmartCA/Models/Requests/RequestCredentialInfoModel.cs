using Newtonsoft.Json;

namespace CenIT.Libs.VNPTSmartCA.Models.Requests
{
    public class RequestCredentialInfoModel
    {
        [JsonProperty(PropertyName = "credentialId")]
        public string CredentialId { get; set; }

        [JsonProperty(PropertyName = "certificates")]
        public string Certificates { get; set; }

        [JsonProperty(PropertyName = "certInfo")]
        public bool CertInfo { get; set; } = true;

        [JsonProperty(PropertyName = "authInfo")]
        public bool AuthInfo { get; set; } = true;
    }
}