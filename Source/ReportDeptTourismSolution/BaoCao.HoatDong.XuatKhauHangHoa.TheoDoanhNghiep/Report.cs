using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using CenIT.ReportTourism.Caches.Cate;
using CenIT.ReportTourism.Core.Attributes;
using CenIT.ReportTourism.Core.Interfaces;
using Microsoft.Reporting.WebForms;

namespace BaoCao.HoatDong.XuatKhauHangHoa.TheoDoanhNghiep
{
    [ReportPlugin("_01HoatDong_XuatKhauHangHoa_DoanhNghiep", "01 - CS/XKHH - Báo cáo hoạt động xuất khẩu hàng hóa",
        "01 - CS/XKHH - Báo cáo hoạt động xuất khẩu hàng hóa theo Doanh nghiệp")]
    public class Report : IPlugableReport
    {
        private readonly CateEnterpriseCache _enterpriseCache = new CateEnterpriseCache();
        private readonly CateBusinessIndustryCache _industryCache = new CateBusinessIndustryCache();

        public int IdentifyReport => 3;

        private string TemplateReport => "_01HoatDong_XuatKhauHangHoa_DoanhNghiep.rdlc";

        private object[] ReportParams { get; set; }

        public string Description => "01 - CS/XKHH - Báo cáo hoạt động xuất khẩu hàng hóa theo Doanh nghiệp";

        public string ReportKey => "_01HoatDong_XuatKhauHangHoa_DoanhNghiep";

        public string ReportName => "01 - CS/XKHH - Báo cáo hoạt động xuất khẩu hàng hóa";

        public string ViewName => "_01HoatDong_XuatKhauHangHoa_DoanhNghiep";

        public string StoreName => "usp_Report_HoatDongXuatKhau_TheoEnterprise";

        private readonly string _dataSourceName = "HoatDong_XuatKhauHangHoa";

        public object[] CreateParams(NameValueCollection param)
        {
            var onMonthStr = param["OnMonth"];
            var onMonth = DateTime.ParseExact(onMonthStr, "MM/yyyy", CultureInfo.CurrentCulture);

            var enterpriseIdStr = param["EnterpriseId"];
            int.TryParse(enterpriseIdStr, out var enterpriseId);

            var enterpriseName = param["EnterpriseName"] ?? string.Empty;
            var taxCode = param["TaxCode"] ?? string.Empty;
            var businessAddress = param["BusinessAddress"] ?? string.Empty;
            var phone = param["Phone"] ?? string.Empty;
            var email = param["Email"] ?? string.Empty;
            var economicSectorName = param["EconomicSectorName"] ?? string.Empty;
            var economicSectorCode = param["EconomicSectorCode"] ?? string.Empty;
            var mainIndustryName = param["MainIndustryName"] ?? string.Empty;
            var mainIndustryCode = param["MainIndustryCode"] ?? string.Empty;

            try
            {
                if (enterpriseId > 0)
                {
                    var enterprise = _enterpriseCache.GetById(enterpriseId);
                    if (enterprise != null)
                    {
                        if (string.IsNullOrEmpty(enterpriseName)) enterpriseName = enterprise.BusinessName;
                        if (string.IsNullOrEmpty(taxCode)) taxCode = enterprise.TaxCode;
                        if (string.IsNullOrEmpty(businessAddress)) businessAddress = enterprise.BusinessAddress;
                        if (string.IsNullOrEmpty(phone)) phone = enterprise.Phone;
                        if (string.IsNullOrEmpty(email)) email = enterprise.Email;
                        if (string.IsNullOrEmpty(economicSectorName)) economicSectorName = enterprise.EconomicSectorName ?? enterprise.EnterpriseTypeName;
                        if (string.IsNullOrEmpty(economicSectorCode)) economicSectorCode = enterprise.EconomicSectorCode ?? enterprise.EnterpriseTypeCode;

                        if (string.IsNullOrEmpty(mainIndustryName) && !string.IsNullOrEmpty(enterprise.IndustryIds))
                        {
                            var indParts = enterprise.IndustryIds.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                            if (indParts.Length > 0 && int.TryParse(indParts[0], out var indId))
                            {
                                var indList = _industryCache.GetAll();
                                var foundInd = indList?.FirstOrDefault(x => x.IndustryId == indId);
                                if (foundInd != null)
                                {
                                    mainIndustryName = foundInd.IndustryName ?? string.Empty;
                                    mainIndustryCode = foundInd.IndustryCode ?? string.Empty;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Fallback to parameters passed from form
            }

            ReportParams = new object[]
            {
                onMonth,
                enterpriseName,
                taxCode,
                businessAddress,
                phone,
                email,
                mainIndustryName,
                mainIndustryCode,
                economicSectorName,
                economicSectorCode
            };

            return new object[]
            {
                onMonth,
                3, // TypeReport = 3 (Xuất nhập khẩu)
                enterpriseId
            };
        }

        private List<ReportParameter> BuildReportParameters()
        {
            var onMonth = ReportParams != null && ReportParams.Length > 0 && ReportParams[0] is DateTime dt ? dt : DateTime.Now;
            var enterpriseName = ReportParams != null && ReportParams.Length > 1 ? Convert.ToString(ReportParams[1]) : string.Empty;
            var taxCode = ReportParams != null && ReportParams.Length > 2 ? Convert.ToString(ReportParams[2]) : string.Empty;
            var businessAddress = ReportParams != null && ReportParams.Length > 3 ? Convert.ToString(ReportParams[3]) : string.Empty;
            var phone = ReportParams != null && ReportParams.Length > 4 ? Convert.ToString(ReportParams[4]) : string.Empty;
            var email = ReportParams != null && ReportParams.Length > 5 ? Convert.ToString(ReportParams[5]) : string.Empty;
            var mainIndustryName = ReportParams != null && ReportParams.Length > 6 ? Convert.ToString(ReportParams[6]) : string.Empty;
            var mainIndustryCode = ReportParams != null && ReportParams.Length > 7 ? Convert.ToString(ReportParams[7]) : string.Empty;
            var economicSectorName = ReportParams != null && ReportParams.Length > 8 ? Convert.ToString(ReportParams[8]) : string.Empty;
            var economicSectorCode = ReportParams != null && ReportParams.Length > 9 ? Convert.ToString(ReportParams[9]) : string.Empty;

            return new List<ReportParameter>
            {
                new ReportParameter("p_OnMonth", onMonth.ToString("dd/MM/yyyy")),
                new ReportParameter("p_Month", onMonth.Month.ToString()),
                new ReportParameter("p_Year", onMonth.Year.ToString()),
                new ReportParameter("p_EnterpriseName", enterpriseName),
                new ReportParameter("p_TaxCode", taxCode),
                new ReportParameter("p_Address", businessAddress),
                new ReportParameter("p_Phone", phone),
                new ReportParameter("p_Email", email),
                new ReportParameter("p_MainIndustryName", mainIndustryName),
                new ReportParameter("p_MainIndustryCode", mainIndustryCode),
                new ReportParameter("p_EconomicSectorName", economicSectorName),
                new ReportParameter("p_EconomicSectorCode", economicSectorCode)
            };
        }

        public void Export(HttpResponseBase response, DataTable data, string urlPathReport)
        {
            var fullPathRdlc = Path.Combine(urlPathReport, TemplateReport);
            var onMonth = ReportParams != null && ReportParams.Length > 0 && ReportParams[0] is DateTime dt ? dt : DateTime.Now;
            var reportFilename = $"{ReportName}_{onMonth:yyyyMMdd}";

            var listParams = BuildReportParameters();

            Warning[] warnings;
            string[] streamIds;
            string mimeType;
            string encoding;
            string extension;

            var reportExcel = new ReportViewer { ProcessingMode = ProcessingMode.Local };
            reportExcel.LocalReport.ReportPath = fullPathRdlc;
            reportExcel.LocalReport.DataSources.Clear();
            reportExcel.LocalReport.DataSources.Add(new ReportDataSource(_dataSourceName, data));
            reportExcel.LocalReport.SetParameters(listParams);

            var bytes = reportExcel.LocalReport.Render("EXCELOPENXML", null, out mimeType, out encoding, out extension,
                out streamIds, out warnings);

            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.AddHeader("Content-Disposition", "attachment; filename=" + reportFilename + "." + extension);
            response.BinaryWrite(bytes);
            response.Flush();
            response.End();
        }

        public ReportViewer CreateReport(DataTable data, string urlPathReport)
        {
            var fullPathRdlc = Path.Combine(urlPathReport, TemplateReport);
            var listParams = BuildReportParameters();

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

            reportViewer.LocalReport.ReportPath = fullPathRdlc;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource(_dataSourceName, data));
            reportViewer.LocalReport.SetParameters(listParams);

            return reportViewer;
        }
    }
}

