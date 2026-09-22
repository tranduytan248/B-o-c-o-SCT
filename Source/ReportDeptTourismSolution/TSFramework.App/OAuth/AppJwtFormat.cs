using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;

namespace TSFramework.App.OAuth
{
    public class AppJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private const string AudiencePropertyKey = "audience";

        private readonly string _issuer;

        public AppJwtFormat(string issuer)
        {
            _issuer = issuer;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var audienceId = data.Properties.Dictionary.ContainsKey(AudiencePropertyKey)
                ? data.Properties.Dictionary[AudiencePropertyKey]
                : null;

            if (string.IsNullOrWhiteSpace(audienceId))
                throw new InvalidOperationException("AuthenticationTicket.Properties does not include audience");

            var audience = AudiencesStore.FindAudience(audienceId);

            var symmetricKeyAsBase64 = audience.Base64Secret;

            var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);
            var securityKey = new SymmetricSecurityKey(keyByteArray);
            var signingKey = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;

            var token = new JwtSecurityToken(_issuer, audienceId, data.Identity.Claims, issued?.UtcDateTime,
                expires?.UtcDateTime, signingKey);

            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.WriteToken(token);

            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}