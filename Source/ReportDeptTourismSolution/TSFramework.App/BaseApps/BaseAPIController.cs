using System.Web.Http;
using TSFramework.App.Attributes;

namespace TSFramework.App.BaseApps
{
    [CustomApiAuthorize]
    [ApiActionFilter]
    public class BaseApiController : ApiController
    {
    }
}