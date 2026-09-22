using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using TSFramework.App.Processors;

namespace TSFramework.App.OAuth
{
    public class AppOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public AppOAuthProvider(string publicClientId)
        {
            if (publicClientId == null) throw new ArgumentNullException(nameof(publicClientId));

            _publicClientId = publicClientId;
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                var allowedOrigin = "*";
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

                var dataUser = AppProcessor.Author.Token(context.UserName, context.Password);
                if (dataUser == null)
                {
                    context.SetError("invalid_grant", AppProcessor.Messagor.GetMessage("Auth_Response_Incorrect"));
                    return;
                }

                var identity = new ClaimsIdentity("JWT");
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));

                var properties = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "audience", context.ClientId ?? string.Empty
                    }
                });
                var dataExtend = new Dictionary<string, object>
                {
                    {nameof(dataUser.Email), dataUser.Email},
                    {nameof(dataUser.FullName), dataUser.FullName},
                    {nameof(dataUser.UserId), dataUser.UserId},
                    {nameof(dataUser.UserName), dataUser.UserName}
                };

                foreach (var key in dataExtend.Keys)
                    if (dataExtend[key] != null)
                        properties.Dictionary.Add(
                            key, dataExtend[key].ToString()
                        );

                var ticket = new AuthenticationTicket(identity, properties);
                context.Validated(ticket);
                context.Request.Context.Authentication.SignIn(identity);
            });
            //return Task.FromResult<object>(null);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (var property in context.Properties.Dictionary)
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
                context.TryGetFormCredentials(out clientId, out clientSecret);

            if (context.ClientId == null)
            {
                context.SetError("invalid_clientId", "client_Id is not set");
                return Task.FromResult<object>(null);
            }

            var audience = AudiencesStore.FindAudience(context.ClientId);

            if (audience == null)
            {
                context.SetError("invalid_clientId", $"Invalid client_id '{context.ClientId}'");
                return Task.FromResult<object>(null);
            }

            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId != _publicClientId) return Task.FromResult<object>(null);
            var expectedRootUri = new Uri(context.Request.Uri, "/");

            if (expectedRootUri.AbsoluteUri == context.RedirectUri) context.Validated();

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName, string clientId)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                {"userName", userName},
                {"audience", clientId}
            };
            return new AuthenticationProperties(data);
        }
    }
}