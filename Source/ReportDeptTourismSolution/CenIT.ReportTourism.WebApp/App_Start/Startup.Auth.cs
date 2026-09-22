using System;
using System.Configuration;
using CenIT.ReportTourism.WebApp.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace CenIT.ReportTourism.WebApp
{
    public partial class Startup
    {
        static Startup()
        {
            ExpireTimeSpan = int.Parse(ConfigurationManager.AppSettings["AuthenticationExpire"] ?? "1");
        }

        public static int ExpireTimeSpan { get; set; }

        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        /// <summary>
        ///     Public client ID property.
        /// </summary>
        public static string PublicClientId { get; private set; }

        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user  
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider  
            // Configure the sign in cookie  
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/Logout"),
                ExpireTimeSpan = TimeSpan.FromMinutes(ExpireTimeSpan)
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            // Configure the application for OAuth based flow  
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/auth/token"),
                Provider = new OAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(ExpireTimeSpan),
                AllowInsecureHttp = true
            };
            app.UseOAuthBearerTokens(OAuthOptions);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.  
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie,
                TimeSpan.FromMinutes(ExpireTimeSpan));

            // Enables the application to remember the second login verification factor such as phone or email.  
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.  
            // This is similar to the RememberMe option when you log in.  
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);


            //Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //app.UseFacebookAuthentication(
            //    appId: "",
            //    appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
    }
}