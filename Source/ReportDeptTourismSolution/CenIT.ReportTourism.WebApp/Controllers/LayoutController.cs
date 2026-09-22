using System.Web.Mvc;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.WebApp.Models;

namespace CenIT.ReportTourism.WebApp.Controllers
{
    public class LayoutController : AppController
    {
        // GET: Layout
        public ActionResult Index()
        {
            return View(new ProfileModel());
        }
    }
}