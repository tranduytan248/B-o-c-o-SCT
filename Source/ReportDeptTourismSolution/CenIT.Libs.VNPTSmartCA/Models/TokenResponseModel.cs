using Newtonsoft.Json;

namespace CenIT.Libs.VNPTSmartCA.Models
{
    /// <summary>
    ///     access_token response mapping
    /// </summary>
    public class TokenResponseModel
    {
        // access_token value
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        // refresh_token to get new access_token (see RefreshToken method)
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        // access_token valid time. when expired, using refresh_token to get new or require user re-authorize
        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }
    }
}