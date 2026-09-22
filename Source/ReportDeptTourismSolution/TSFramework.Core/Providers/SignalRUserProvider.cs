using Microsoft.AspNet.SignalR;

namespace TSFramework.Core.Providers
{
    public class SignalRUserProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            return request.User?.Identity != null && request.User.Identity.IsAuthenticated
                ? request.User.Identity.Name
                : string.Empty;
        }
    }
}