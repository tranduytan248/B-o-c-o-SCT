using Newtonsoft.Json;

namespace CenIT.Libs.VNPTSmartCA.Models
{
    /// <summary>
    ///     access_token request parameter mapping
    /// </summary>
    public class TokenBodyModel
    {
        [JsonProperty(PropertyName = "grant_type")]
        public string GrantType { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "client_id")]
        public string ClientID { get; set; }

        [JsonProperty(PropertyName = "client_secret")]
        public string ClientSecret { get; set; }
    }

    /// <summary>
    ///     refresh_token request parameter mapping
    /// </summary>
    public class RefreshTokenBodyModel
    {
        [JsonProperty(PropertyName = "grant_type")]
        public string GrantType { get; set; }

        [JsonProperty(PropertyName = "client_id")]
        public string ClientID { get; set; }

        [JsonProperty(PropertyName = "client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }
    }
}