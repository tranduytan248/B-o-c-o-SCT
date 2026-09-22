using System;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;
using TSFramework.App.Principals;
using TSFramework.App.Processors;

namespace TSFramework.App.BaseApps
{
    public class BaseHttpApplication : HttpApplication
    {
        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            if (exception.InnerException != null) AppProcessor.Logger.Error(exception.InnerException);

            AppProcessor.Logger.Error(exception);
            Response.Clear();
            var httpException = exception as HttpException;
            string sErrorUrlRedirect;

            if (httpException == null)
                sErrorUrlRedirect =  "~/Error/Error";
            else //It's an Http Exception, Let's handle it.
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        sErrorUrlRedirect = "~/Error/NotFound";
                        break;
                    case 500:
                        sErrorUrlRedirect = "~/Error/Error";
                        break;
                    default:
                        sErrorUrlRedirect = "~/Error/AccessDenied";
                        break;
                }

            // Clear the error on server.
            Server.ClearError();
            Response.Redirect(sErrorUrlRedirect, true);
            // Avoid IIS7 getting in the middle
            Response.TrySkipIisCustomErrors = true;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null) return;

            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            if (authTicket?.UserData == null) return;

            var serializeModel = JsonConvert.DeserializeObject<AppPrincipalSerializeModel>(authTicket.UserData);
            var userPrincipal = new AppPrincipal(authTicket.Name)
            {
                UserId = serializeModel.UserId,
                FullName = serializeModel.FullName,
                UserName = serializeModel.UserName,
                Email = serializeModel.Email,
                Token = serializeModel.Token,
                CreatedDate = serializeModel.CreatedDate
            };
            BaseAppContext.Current.User = userPrincipal;
            HttpContext.Current.User = userPrincipal;
        }
    }
}