using System.Web.Mvc;

namespace CenIT.ReportTourism.Modules.CateModule.Areas.Cate
{
    public class CateAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Cate";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Cate_default",
                "Cate/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[]
                {
                    "CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Controllers",
                    "CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Controllers.TypeBusiness"
                }
            );
        }
    }
}