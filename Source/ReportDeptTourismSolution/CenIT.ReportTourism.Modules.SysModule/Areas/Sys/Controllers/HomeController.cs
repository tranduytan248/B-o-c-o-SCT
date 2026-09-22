using System.Web.Mvc;
using CenIT.ReportTourism.Core.Apps;

namespace CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Controllers
{
    public class HomeController : AppController
    {
        // GET: Sys/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}