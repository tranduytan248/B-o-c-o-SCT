using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using CenIT.ReportTourism.Core.Attributes;
using CenIT.ReportTourism.Core.Interfaces;
using Microsoft.Reporting.WebForms;

namespace _03.BaoCao.ThuongMaiDichVu.TheoDoanhNghiep
{
    [ReportPlugin("_03.BaoCao.ThuongMaiDichVu.TheoDoanhNghiep", "03 - Báo cáo tình hình hoạt động thương mại, dịch vụ theo Doanh nghiệp",
        "03 - Báo cáo tình hình hoạt động thương mại, dịch vụ theo Doanh nghiệp")]
    public class Report : IPlugableReport
    {
        public int IdentifyReport => 1;

        private string TemplateReport => "_03ThuongMaiDichVu_DoanhNghiep.rdlc";

        private object[] ReportParams { get; set; }

        public string Description => "03 - Báo cáo tình hình hoạt động thương mại, dịch vụ theo Doanh nghiệp";

        public string ReportKey => "_03ThuongMaiDichVu_DoanhNghiep";

        public string ReportName => "03 - Báo cáo tình hình hoạt động thương mại, dịch vụ theo Doanh nghiệp";

        public string ViewName => "_03ThuongMaiDichVu_DoanhNghiep";

        public string StoreName => "usp_Report_KTXH_Type2_TheoEnterprise";

        public object[] CreateParams(NameValueCollection param)
        {
            ReportParams = new object[] { param["OnMonth"], param["EnterpriseName"], param["TaxCode"] };
            return new object[]
            {
                DateTime.ParseExact(param["OnMonth"], "MM/yyyy", CultureInfo.CurrentCulture),
                //int.Parse(param["TypeReport"]),
                int.Parse(param["EnterpriseId"])
            };
        }

        private readonly string _dataSourceName = "TinhHinhHoatDong_TMDV";

        public void Export(HttpResponseBase response, DataTable data, string urlPathReport)
        {
            var fullPathRdlc = Path.Combine(urlPathReport, TemplateReport);

            var onMonth = DateTime.ParseExact(ReportParams[0] as string, "MM/yyyy", CultureInfo.CurrentCulture);
            var enterpriseName = ReportParams[1] as string;
            var taxCode = ReportParams[2] as string;

            var reportFilename = $"{ReportName}_{onMonth:yyyyMMdd}";

            #region Repair Report Parameter

            var listParams = new List<ReportParameter>
            {
                new ReportParameter("p_OnMonth", onMonth.ToString()),
                new ReportParameter("p_EnterpriseName", enterpriseName),
                new ReportParameter("p_TaxCode", taxCode)
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
            var reportExcel = new ReportViewer { ProcessingMode = ProcessingMode.Local };

            reportExcel.LocalReport.ReportPath = fullPathRdlc;
            reportExcel.LocalReport.DataSources.Clear();
            reportExcel.LocalReport.DataSources.Add(new ReportDataSource(_dataSourceName, data));
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
            var enterpriseName = ReportParams[1] as string;
            var taxCode = ReportParams[2] as string;

            #region Repair Report Parameter

            var listParams = new List<ReportParameter>
            {
                new ReportParameter("p_OnMonth", onMonth.ToString()),
                new ReportParameter("p_EnterpriseName", enterpriseName),
                new ReportParameter("p_TaxCode", taxCode)
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
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource(_dataSourceName, data));
            reportViewer.LocalReport.SetParameters(listParams);

            #endregion

            return reportViewer;
        }
    }
}
