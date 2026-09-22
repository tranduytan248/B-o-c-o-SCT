using System.Threading.Tasks;
using System.Web.Mvc;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Models;
using CenIT.ReportTourism.Modules.ReportModule.Processor;
using CenIT.ReportTourism.Modules.ReportModule.Providers;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Controllers
{
    public class ReportController : AppController
    {
        private readonly string _reportTitle = AppProcessor.Messagor.GetMessage("Report_Title");

        public ActionResult Index()
        {
            ViewBag.Title = _reportTitle;
            var lstReports = ReportProcessor.Reports;
            return View(lstReports);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        [AjaxOnly]
        public async Task<ActionResult> ViewReport(string report)
        {
            var pReport = ReportProcessor.GetReportByKey(report);
            ViewBag.Title = pReport.ReportName;

            var reportModel = new ReportViewModel
            {
                ReportKey = pReport.ReportKey,
                ReportName = pReport.ReportName,
                ViewName = pReport.ViewName,
                Reporter = User.FullName
            };
            return await Task.Run(() => PartialView("_Report", reportModel));
        }

        [ActionType(Type = EnumActionType.View)]
        [AjaxOnly]
        [HttpPost]
        public async Task<ActionResult> RenderReport()
        {
            var sReportKey = Request.Form["ReportKey"];
            ViewBag.Title = Request.Form["ReportName"];

            ViewBag.ReportViewer =
                ReportProvider.CreateViewExport(sReportKey, Request.Form,
                    Server.MapPath("~/Contents/Modules/Report/Templates/"));
            return await Task.Run(() => PartialView("_Viewer"));
        }
    }
}