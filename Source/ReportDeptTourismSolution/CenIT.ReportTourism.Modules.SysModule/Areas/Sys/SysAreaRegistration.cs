using System.Web.Mvc;

namespace CenIT.ReportTourism.Modules.SysModule.Areas.Sys
{
    public class SysAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Sys";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Sys_default",
                "Sys/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Controllers" }
            );
        }
    }
}