using System;
using System.Web.Mvc;
using CenIT.ReportTourism.Caches.Report;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Models.Report;
using TSFramework.App.Attributes;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Controllers
{
    public class DashboardController : AppController
    {
        private readonly ReportDashboardCache _dashboardCache = new ReportDashboardCache();

        [ActionType(Type = EnumActionType.View)]
        public ActionResult Index(int? year, int? month, int? reportType, string areaId,
            string economicSectorId, string industryId, string enterpriseId)
        {
            var filters = CreateFilters(year, month, reportType, areaId, economicSectorId, industryId, enterpriseId);
            ViewBag.Title = "Tổng quan báo cáo Công Thương";
            return View(_dashboardCache.GetDashboard(filters));
        }

        [ActionType(Type = EnumActionType.View)]
        public ActionResult Analysis(int? year, int? month, int? reportType, string areaId,
            string economicSectorId, string industryId, string enterpriseId, string metric)
        {
            var filters = CreateFilters(year, month, reportType, areaId, economicSectorId, industryId, enterpriseId, metric);
            ViewBag.Title = "Phân tích chỉ tiêu";
            return View(_dashboardCache.GetAnalysis(filters));
        }

        [ActionType(Type = EnumActionType.View)]
        public ActionResult Warnings(int? year, int? month, int? reportType, string areaId,
            string economicSectorId, string industryId, string enterpriseId)
        {
            var filters = CreateFilters(year, month, reportType, areaId, economicSectorId, industryId, enterpriseId);
            ViewBag.Title = "Tín hiệu biến động";
            return View(_dashboardCache.GetWarnings(filters));
        }

        [ActionType(Type = EnumActionType.View)]
        public ActionResult Progress(int? year, int? month, int? reportType, string areaId,
            string economicSectorId, string industryId, string enterpriseId)
        {
            var filters = CreateFilters(year, month, reportType, areaId, economicSectorId, industryId, enterpriseId);
            ViewBag.Title = "Theo dõi dữ liệu";
            return View(_dashboardCache.GetProgress(filters));
        }

        private static DashboardFilters CreateFilters(int? year, int? month, int? reportType,
            string areaId, string economicSectorId, string industryId, string enterpriseId, string metric = null)
        {
            var previous = DateTime.Today.AddMonths(-1);
            return new DashboardFilters
            {
                Year = year ?? previous.Year,
                Month = month ?? previous.Month,
                ReportType = reportType ?? 1,
                AreaId = areaId ?? "all",
                EconomicSectorId = economicSectorId ?? "all",
                IndustryId = industryId ?? "all",
                EnterpriseId = enterpriseId ?? "all",
                Metric = metric ?? "primary"
            };
        }
    }
}
