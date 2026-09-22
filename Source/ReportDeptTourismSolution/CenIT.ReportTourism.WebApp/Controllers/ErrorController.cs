using System.Web.Mvc;
using CenIT.ReportTourism.Core.Apps;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.WebApp.Controllers
{
    [AllowAnyPermission]
    public class ErrorController : AppController
    {
        [AllowAnyPermission]
        public ActionResult AccessDenied()
        {
            Response.StatusCode = 405;
            return View();
        }

        [AllowAnyPermission]
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        [AllowAnyPermission]
        public ActionResult Error()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}