using System.Web.Mvc;
using CenIT.ReportTourism.Core.Apps;

namespace CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Controllers
{
    public class HomeController : AppController
    {
        // GET: Report/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}