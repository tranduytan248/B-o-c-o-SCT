using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using CenIT.ReportTourism.Core.Attributes;
using CenIT.ReportTourism.Core.Interfaces;
using Microsoft.Reporting.WebForms;

namespace CenIT.ReportTourism.Reports.UocKetQuaHoatDongKinhDoanh
{
    [ReportPlugin("_01UocKetQuaHoatDongKinhDoanh", "01 - Ước Kết Quả Hoạt Động Kinh Doanh",
        "01 - Ước Kết Quả Hoạt Động Kinh Doanh")]
    public class _01UocKetQuaHoatDongKinhDoanh : IPlugableReport
    {
        public int IdentifyReport => 1;

        private string TemplateReport => "_01UocKetQuaHoatDongKinhDoanh.rdlc";

        private object[] ReportParams { get; set; }

        public string Description => "01 - Ước Kết Quả Hoạt Động Kinh Doanh";

        public string ReportKey => "_01UocKetQuaHoatDongKinhDoanh";

        public string ReportName => "01 - Ước Kết Quả Hoạt Động Kinh Doanh";

        public string ViewName => "_01UocKetQuaHoatDongKinhDoanh";

        public string StoreName => "Report_Reports_01_UocKetQuaHoatDongKinhDoanh";

        public object[] CreateParams(NameValueCollection param)
        {
            ReportParams = new object[] {param["OnMonth"]};
            return new object[]
                {DateTime.ParseExact(ReportParams[0] as string, "MM/yyyy", CultureInfo.CurrentCulture)};
        }

        public void Export(HttpResponseBase response, DataTable data, string urlPathReport)
        {
            var fullPathRdlc = Path.Combine(urlPathReport, TemplateReport);

            var onMonth = DateTime.ParseExact(ReportParams[0] as string, "MM/yyyy", CultureInfo.CurrentCulture);
            var sCompanyName = ConfigurationManager.AppSettings["App_Company_Name"];

            var reportFilename = $"{ReportName}_{onMonth:yyyyMMdd}";

            #region Repair Report Parameter

            var listParams = new List<ReportParameter>
            {
                new ReportParameter("P_OnMonth", onMonth.ToString()),
                new ReportParameter("P_CompanyName", sCompanyName)
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
            reportExcel.LocalReport.DataSources.Add(new ReportDataSource("_01UocKetQuaHoatDongKinhDoanh", data));
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

            #region Repair Report Parameter

            var listParams = new List<ReportParameter>
            {
                new ReportParameter("P_OnMonth", onMonth.ToString())
                //new ReportParameter("P_CompanyName", sCompanyName)
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
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("UocKetQuaHoatDongKinhDoanh", data));
            reportViewer.LocalReport.SetParameters(listParams);

            #endregion

            return reportViewer;
        }
    }
}