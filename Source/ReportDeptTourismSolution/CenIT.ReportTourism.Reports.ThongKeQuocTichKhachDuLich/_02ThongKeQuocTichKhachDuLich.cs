using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using CenIT.ReportTourism.Caches.Sys;
using CenIT.ReportTourism.Core.Attributes;
using CenIT.ReportTourism.Core.Interfaces;
using Microsoft.Reporting.WebForms;

namespace CenIT.ReportTourism.Reports.ThongKeQuocTichKhachDuLich
{
    [ReportPlugin("_02ThongKeQuocTichKhachDuLich", "02 - Thống Kê Quốc Tịch Khách Du Lịch",
        "02 - Thống Kê Quốc Tịch Khách Du Lịch")]
    public class _02ThongKeQuocTichKhachDuLich : IPlugableReport
    {
        private readonly SysConfigsCache _configCache;

        public _02ThongKeQuocTichKhachDuLich()
        {
            _configCache = new SysConfigsCache();
        }

        public int IdentifyReport => 1;

        private string TemplateReport => "_02ThongKeQuocTichKhachDuLich.rdlc";

        private object[] ReportParams { get; set; }

        public string Description => "02 - Thống Kê Quốc Tịch Khách Du Lịch";

        public string ReportKey => "_02ThongKeQuocTichKhachDuLich";

        public string ReportName => "02 - Thống Kê Quốc Tịch Khách Du Lịch";

        public string ViewName => "_02ThongKeQuocTichKhachDuLich";

        public string StoreName => "Report_Reports_02_ThongKeQuocTichKhachDuLich";

        public object[] CreateParams(NameValueCollection param)
        {
            var manager = _configCache.GetViaKey("Report_02_Manager")?.ConfigValue;
            ReportParams = new object[] {param["OnMonth"], param["Reporter"], manager};
            return new object[]
                {DateTime.ParseExact(ReportParams[0] as string, "MM/yyyy", CultureInfo.CurrentCulture)};
        }

        public void Export(HttpResponseBase response, DataTable data, string urlPathReport)
        {
            var fullPathRdlc = Path.Combine(urlPathReport, TemplateReport);

            var onMonth = DateTime.ParseExact(ReportParams[0] as string, "MM/yyyy", CultureInfo.CurrentCulture);
            var sReporter = ReportParams[1] as string;
            var sManager = ReportParams[2] as string;

            var reportFilename = $"{ReportName}_{onMonth:yyyyMMdd}";

            #region Repair Report Parameter

            var listParams = new List<ReportParameter>
            {
                new ReportParameter("P_OnMonth", onMonth.ToString()),
                new ReportParameter("P_Reporter", sReporter),
                new ReportParameter("P_Manager", sManager)
            };

            #endregion

            #region Create Report

            // Variables
            Warning[] warnings;
            string[] streamIds;
            string mimeType;
            string encoding;
            string extension;

            // Setup the report viewer object and get the array of bytes
            var reportExcel = new ReportViewer {ProcessingMode = ProcessingMode.Local};

            reportExcel.LocalReport.ReportPath = fullPathRdlc;
            reportExcel.LocalReport.DataSources.Clear();
            reportExcel.LocalReport.DataSources.Add(new ReportDataSource("ThongKeQuocTichKhachDuLich", data));
            reportExcel.LocalReport.SetParameters(listParams);

            //Chuyển sang Excel
            var bytes = reportExcel.LocalReport.Render("EXCELOPENXML", null, out mimeType, out encoding, out extension,
                out streamIds, out warnings);

            #endregion

            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.AddHeader("Content-Disposition", "attachment; filename=" + reportFilename + "." + extension);
            response.BinaryWrite(bytes);
            response.Flush();
            response.End();
        }

        public ReportViewer CreateReport(DataTable data, string urlPathReport)
        {
            var fullPathRdlc = Path.Combine(urlPathReport, TemplateReport);

            var onMonth = DateTime.ParseExact(ReportParams[0] as string, "MM/yyyy", CultureInfo.CurrentCulture);
            var sReporter = ReportParams[1] as string;
            var sManager = ReportParams[2] as string;

            #region Repair Report Parameter

            var listParams = new List<ReportParameter>
            {
                new ReportParameter("P_OnMonth", onMonth.ToString()),
                new ReportParameter("P_Reporter", sReporter),
                new ReportParameter("P_Manager", sManager)
            };

            #endregion

            #region Create Report

            // Setup the report viewer object and get the array of bytes
            var reportViewer = new ReportViewer
            {
                ProcessingMode = ProcessingMode.Local,
                SizeToReportContent = true,
                ZoomMode = ZoomMode.PageWidth,
                Width = Unit.Percentage(99),
                Height = Unit.Pixel(1000),
                AsyncRendering = false,
                PageCountMode = PageCountMode.Estimate
            };

            //reportViewer.ServerReport
            reportViewer.LocalReport.ReportPath = fullPathRdlc;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("ThongKeQuocTichKhachDuLich", data));
            reportViewer.LocalReport.SetParameters(listParams);

            #endregion

            return reportViewer;
        }
    }
}