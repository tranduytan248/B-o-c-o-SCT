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
        public ActionResult Index(int? year, int? month, string areaId, string economicSectorId, string industryId, string enterpriseId)
        {
            var filters = CreateFilters(year, month, areaId, economicSectorId, industryId, enterpriseId);
            ViewBag.Title = "Tổng quan sản xuất công nghiệp, xuất nhập khẩu";
            return View(_dashboardCache.GetDashboard(filters));
        }

        [ActionType(Type = EnumActionType.View)]
        public ActionResult Analysis(int? year, int? month, string areaId, string economicSectorId, string industryId,
            string enterpriseId, string metric)
        {
            var filters = CreateFilters(year, month, areaId, economicSectorId, industryId, enterpriseId, metric);
            ViewBag.Title = "Phân tích chỉ tiêu";
            return View(_dashboardCache.GetAnalysis(filters));
        }

        [ActionType(Type = EnumActionType.View)]
        public ActionResult Warnings(int? year, int? month, string areaId, string economicSectorId, string industryId, string enterpriseId)
        {
            var filters = CreateFilters(year, month, areaId, economicSectorId, industryId, enterpriseId);
            ViewBag.Title = "Cảnh báo doanh nghiệp";
            return View(_dashboardCache.GetWarnings(filters));
        }

        [ActionType(Type = EnumActionType.View)]
        public ActionResult Progress(int? year, int? month, string areaId, string economicSectorId, string industryId, string enterpriseId)
        {
            var filters = CreateFilters(year, month, areaId, economicSectorId, industryId, enterpriseId);
            ViewBag.Title = "Tiến độ nộp báo cáo";
            return View(_dashboardCache.GetProgress(filters));
        }

        [ActionType(Type = EnumActionType.View)]
        public ActionResult Quality(int? year, int? month, string areaId, string economicSectorId, string industryId, string enterpriseId)
        {
            var filters = CreateFilters(year, month, areaId, economicSectorId, industryId, enterpriseId);
            ViewBag.Title = "Chất lượng và độ bao phủ dữ liệu";
            return View(_dashboardCache.GetQuality(filters));
        }

        // Existing dashboard partial action names are retained while their tourism widgets are repurposed.
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult StatictisEnterprise(int? year, int? month, string areaId, string economicSectorId, string industryId, string enterpriseId)
        {
            return PartialView("_StatisticEnterprise", _dashboardCache.GetDashboard(
                CreateFilters(year, month, areaId, economicSectorId, industryId, enterpriseId)));
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult StatisticTypeBusiness(int? year, int? month, string areaId, string economicSectorId, string industryId, string enterpriseId)
        {
            return PartialView("_StatisticTypeBusiness", _dashboardCache.GetDashboard(
                CreateFilters(year, month, areaId, economicSectorId, industryId, enterpriseId)));
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult StatisticVisitor(int? year, int? month, string areaId, string economicSectorId, string industryId, string enterpriseId)
        {
            return PartialView("_StatisticVisitor", _dashboardCache.GetDashboard(
                CreateFilters(year, month, areaId, economicSectorId, industryId, enterpriseId)));
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult StatisticIncome(int? year, int? month, string areaId, string economicSectorId, string industryId, string enterpriseId)
        {
            return PartialView("_StatisticIncome", _dashboardCache.GetDashboard(
                CreateFilters(year, month, areaId, economicSectorId, industryId, enterpriseId)));
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult EnterpriseMovement(int? year, int? month, string areaId, string economicSectorId, string industryId, string enterpriseId)
        {
            return PartialView("_EnterpriseMovement", _dashboardCache.GetDashboard(
                CreateFilters(year, month, areaId, economicSectorId, industryId, enterpriseId)));
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult DashboardAlerts(int? year, int? month, string areaId, string economicSectorId, string industryId, string enterpriseId)
        {
            return PartialView("_DashboardAlerts", _dashboardCache.GetDashboard(
                CreateFilters(year, month, areaId, economicSectorId, industryId, enterpriseId)));
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult SubmissionDetails(int? year, int? month, string areaId, string economicSectorId, string industryId, string enterpriseId)
        {
            var filters = CreateFilters(year, month, areaId, economicSectorId, industryId, enterpriseId);
            return PartialView("_DashboardSubmissionDetails", new DashboardSubmissionDetailsModel
            {
                Submission = _dashboardCache.GetDashboard(filters).Submission,
                Filters = filters,
                ShowDetailsLink = true
            });
        }

        private static DashboardFilters CreateFilters(int? year, int? month, string areaId, string economicSectorId,
            string industryId, string enterpriseId, string metric = null)
        {
            return new DashboardFilters
            {
                Year = year ?? DateTime.Today.Year,
                Month = month ?? DateTime.Today.Month,
                AreaId = areaId ?? "all",
                EconomicSectorId = economicSectorId ?? "all",
                IndustryId = industryId ?? "all",
                EnterpriseId = enterpriseId ?? "all",
                Metric = metric ?? "industrial"
            };
        }
    }
}
