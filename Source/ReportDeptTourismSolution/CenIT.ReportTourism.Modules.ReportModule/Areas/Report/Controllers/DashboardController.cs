using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using CenIT.ReportTourism.Caches.Report;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Models.Report;
using CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Models;
using Microsoft.Reporting.WebForms;
using TSFramework.App.Attributes;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Controllers
{
    public class DashboardController : AppController

    {
        private readonly ReportDashboardCache _dashboardCache = new ReportDashboardCache();
        private readonly ReportCache _reportCache = new ReportCache();

        [ActionType(Type = EnumActionType.View)]
        public ActionResult Index()
        {
            var model = new DashboardModel
            {
                ForMonth = DateTime.UtcNow
            };
            return View(model);
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult StatictisEnterprise(DateTime forMonth)
        {
            var statictisEnterprise = _dashboardCache.GetStatisticEnterprise(forMonth);
            return PartialView("_StatisticEnterprise", statictisEnterprise);
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult StatisticTypeBusiness(DateTime forMonth)
        {
            var statictisEnterprise = _dashboardCache.GetStatisticTypeBusiness(forMonth);
            return PartialView("_StatisticTypeBusiness", statictisEnterprise);
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult StatisticVisitor(DateTime forMonth)
        {
            var statictisVisitor = _dashboardCache.GetStatisticVisitor(forMonth);
            return PartialView("_StatisticVisitor", new StatisticVisitorModel
            {
                ForMonth = forMonth,
                DataStatistic = statictisVisitor
            });
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult StatisticMapVisitor(DateTime forMonth)
        {
            var statictisVisitor = _dashboardCache.GetStatisticMapVisitor(forMonth);
            return PartialView("_StatisticMapVisitor", new StatisticMapVisitorViewModel
            {
                OnMonth = forMonth,
                DataStatistic = statictisVisitor
            });
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult StatisticIncome(DateTime forMonth, int? typeStatistic = 1)
        {
            var statictisVisitor = _dashboardCache.GetStatisticIncome(forMonth, typeStatistic);
            return PartialView("_StatisticIncome", new StatisticIncomeViewModel
            {
                OnMonth = forMonth,
                Title = typeStatistic == 1 ? "Doanh thu lưu trú" :
                    typeStatistic == 2 ? "Doanh thu theo thị trường khách" :
                    typeStatistic == 3 ? "Doanh thu theo loại hình dịch vụ" : "",
                DataStatisticIncome = statictisVisitor,
                TypeStatistic = typeStatistic
            });
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult EnterpiseNotSendReportYet(DateTime? forMonth)
        {
            const string fileTemplateName = "Report_Enterprise.rdlc";
            const string procedureName = "Report_Reports_03_DoanhNghiepChuaGuiBaoCao";

            var dataEnterprises = _reportCache.GetDataReport(procedureName, forMonth ?? DateTime.Now);
            var fullPathRdlc = Path.Combine(Server.MapPath("~/Contents/Modules/Report/Templates/"), fileTemplateName);
            var reportFileName =
                $"Doanh nghiệp chưa gửi báo cáo tháng {forMonth ?? DateTime.Now:MM/yyyy}";

            #region Create Report

            var listParams = new List<ReportParameter>
            {
                new ReportParameter("P_ForMonth", (forMonth ?? DateTime.Now).ToString())
            };

            // Variables
            Warning[] warnings;
            string[] streamIds;
            string mimeType;
            string encoding;
            string extension;

            // Setup the report viewer object and get the array of bytes
            var reportExcel = new ReportViewer { ProcessingMode = ProcessingMode.Local };

            reportExcel.LocalReport.ReportPath = fullPathRdlc;
            reportExcel.LocalReport.DataSources.Clear();
            reportExcel.LocalReport.DataSources.Add(new ReportDataSource("Enterprise", dataEnterprises));
            reportExcel.LocalReport.SetParameters(listParams);

            //Chuyển sang Excel
            var bytes = reportExcel.LocalReport.Render("EXCELOPENXML", null, out mimeType, out encoding, out extension,
                out streamIds, out warnings);

            #endregion

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                reportFileName + "." + extension);
        }
    }
}