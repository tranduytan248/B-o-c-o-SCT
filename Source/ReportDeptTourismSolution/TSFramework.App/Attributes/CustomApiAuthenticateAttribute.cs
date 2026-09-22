using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;
using WebApi.AuthenticationFilter;

namespace TSFramework.App.Attributes
{
    public sealed class CustomApiAuthorizeAttribute : AuthenticationFilterAttribute
    {
        //private AppPrincipal User => HttpContext.Current.User as AppPrincipal;

        public override void OnAuthentication(HttpAuthenticationContext context)
        {
            var ci = context.Principal.Identity as ClaimsIdentity;
            var authHeader = context.Request.Headers.Authorization;
            if (authHeader == null || authHeader.Scheme.ToLower() != "bearer")
                context.ErrorResult = context.ErrorResult = new AuthenticationFailureResult("Unauthorized",
                    context.Request,
                    new
                    {
                        status = EnumStatus.AuthenticationDenied,
                        data = string.Empty,
                        message = AppProcessor.Messagor.GetMessage("Authorize_AuthenticationRequired")
                    });
            //If the token has expired the property "IsAuthenticated" would be False, then set the error
            else if (ci != null && !ci.IsAuthenticated)
                context.ErrorResult = new AuthenticationFailureResult("Unauthorized", context.Request,
                    new
                    {
                        status = EnumStatus.AuthenticationDenied,
                        data = string.Empty,
                        message = AppProcessor.Messagor.GetMessage("Authorize_AuthenticationTokenExpired")
                    });

            base.OnAuthentication(context);
        }
    }

    public class AuthenticationFailureResult : IHttpActionResult
    {
        private readonly object _responseMessage;

        public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request, object responseMessage)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
            _responseMessage = responseMessage;
        }

        public string ReasonPhrase { get; }

        public HttpRequestMessage Request { get; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
            response.Content = new ObjectContent<object>(_responseMessage, jsonFormatter);
            response.RequestMessage = Request;
            response.ReasonPhrase = ReasonPhrase;
            return response;
        }
    }
}