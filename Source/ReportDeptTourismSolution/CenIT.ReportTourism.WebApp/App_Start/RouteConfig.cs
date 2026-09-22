using System.Web.Mvc;
using System.Web.Routing;

namespace CenIT.ReportTourism.WebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "LoginRoute",
                "Login",
                new {controller = "Account", action = "Login", id = UrlParameter.Optional},
                new[] {"CenIT.ReportTourism.WebApp.Controllers"}
            );

            routes.MapRoute(
                "HomeRoute",
                "Home",
                new { controller = "MyEnterprise", action = "Index", id = UrlParameter.Optional },
                new[] { "CenIT.ReportTourism.WebApp.Controllers" }
            );

            routes.MapRoute(
                "HomeIndexRoute",
                "Home/Index",
                new { controller = "MyEnterprise", action = "Index", id = UrlParameter.Optional },
                new[] { "CenIT.ReportTourism.WebApp.Controllers" }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "MyEnterprise", action = "Index", id = UrlParameter.Optional},
                new[] {"CenIT.ReportTourism.WebApp.Controllers"}
            );
        }
    }
}