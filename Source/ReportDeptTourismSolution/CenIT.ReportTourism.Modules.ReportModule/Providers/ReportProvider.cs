using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web;
using CenIT.ReportTourism.Caches.Report;
using Microsoft.Reporting.WebForms;

namespace CenIT.ReportTourism.Modules.ReportModule.Providers
{
    public class ReportProvider
    {
        private static readonly ReportCache _report = new ReportCache();

        public static void Export(string reportKey, NameValueCollection form, HttpResponseBase reponse, string urlPath)
        {
            var pReport = ReportPlugableProvider.GetReportByKey(reportKey);
            var ps = pReport.CreateParams(form);

            var data = GetReportData(pReport.StoreName, ps);
            pReport.Export(reponse, data, urlPath);
        }

        public static ReportViewer CreateViewExport(string reportKey, NameValueCollection form, string urlPath)
        {
            var pReport = ReportPlugableProvider.GetReportByKey(reportKey);
            var ps = pReport.CreateParams(form);

            var dataReport = GetReportData(pReport.StoreName, ps);
            return pReport.CreateReport(dataReport, urlPath);
        }

        private static DataTable GetReportData(string storeName, object[] parameters)
        {
            // A report may be purely informational and therefore have no stored procedure.
            if (string.IsNullOrEmpty(storeName)) return new DataTable();

            return _report.GetDataReport(storeName, parameters.ToList().ToArray());
        }
    }
}
