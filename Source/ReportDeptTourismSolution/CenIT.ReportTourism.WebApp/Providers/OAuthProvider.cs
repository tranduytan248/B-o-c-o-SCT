using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Security;
using CenIT.ReportTourism.Caches.Sys;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.WebApp.Providers
{
    public class OAuthProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        ///     Public client ID property.
        /// </summary>
        private readonly string _publicClientId;

        private readonly SysUserCache _userCache;

        public OAuthProvider(string publicClientId)
        {
            _userCache = new SysUserCache();
            //TODO: Pull from configuration  
            if (publicClientId == null) throw new ArgumentNullException(nameof(publicClientId));

            // Settings.  
            _publicClientId = publicClientId;
        }

        #region[GrantResourceOwnerCredentials]

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                var allowedOrigin = "*";
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] {allowedOrigin});

                var isValid = Membership.ValidateUser(context.UserName, context.Password);
                if (!isValid)
                {
                    // Settings.  
                    context.SetError("invalid_grant", AppProcessor.Messagor.GetMessage("Auth_Response_Incorrect"));
                    return;
                }


                var userModel = _userCache.GetByUserName(context.UserName);

                if (userModel == null) return;
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Sid, Convert.ToString(userModel.UserName)),
                    new Claim(ClaimTypes.Name, userModel.UserName),
                    new Claim(ClaimTypes.Email, userModel.Email)
                };

                // Setting Claim Identities for OAUTH 2 protocol.  

                var oAuthClaimIdentity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                var cookiesClaimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType);

                var properties = CreateProperties(new Dictionary<string, string>
                {
                    {"userName", userModel.UserName},
                    {"fullname", userModel.FullName},
                    {"email", userModel.Email},
                    {"encKey", context.Password}
                });
                var ticket = new AuthenticationTicket(oAuthClaimIdentity, properties);
                context.Validated(ticket);
                context.Request.Context.Authentication.SignIn(cookiesClaimIdentity);
            });
        }

        #endregion

        #region[ValidateClientAuthentication]

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
                context.Validated();

            return Task.FromResult<object>(null);
        }

        #endregion

        #region Validate client redirect URI override method

        /// <summary>
        ///     Validate client redirect URI override method
        /// </summary>
        /// <param name="context">Context parmeter</param>
        /// <returns>Returns validation of client redirect URI</returns>
        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            // Verification.  
            if (context.ClientId != _publicClientId) return Task.FromResult<object>(null);
            // Initialization.  
            var expectedRootUri = new Uri(context.Request.Uri, "/");

            // Verification.  
            if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                // Validating.  
                context.Validated();

            // Return info.  
            return Task.FromResult<object>(null);
        }

        #endregion

        #region[TokenEndpoint]

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (var property in context.Properties.Dictionary)
                context.AdditionalResponseParameters.Add(property.Key, property.Value);

            return Task.FromResult<object>(null);
        }

        #endregion

        #region[CreateProperties]

        public static AuthenticationProperties CreateProperties(Dictionary<string, string> dataPropAuthen)
        {
            return new AuthenticationProperties(dataPropAuthen);
        }

        //public static AuthenticationProperties CreateProperties(string userName, string fullName, string email)
        //{
        //    IDictionary<string, string> data = new Dictionary<string, string>
        //    {
        //        {"username", userName},
        //        {"fullname", fullName},
        //        {"email", email}
        //    };
        //    return new AuthenticationProperties(data);
        //}

        #endregion
    }
}