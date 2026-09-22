using System.Collections.Specialized;
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

            if (string.IsNullOrEmpty(pReport.StoreName)) return;
            var lstParram = ps.ToList();
            //lstParram.Insert(0, currentUser);

            var data = _report.GetDataReport(pReport.StoreName, lstParram.ToArray());
            pReport.Export(reponse, data, urlPath);
        }

        public static ReportViewer CreateViewExport(string reportKey, NameValueCollection form, string urlPath)
        {
            var pReport = ReportPlugableProvider.GetReportByKey(reportKey);
            var ps = pReport.CreateParams(form);

            var lstParram = ps.ToList();
            //lstParram.Insert(0, currentUser);

            var dataReport = _report.GetDataReport(pReport.StoreName, lstParram.ToArray());
            return pReport.CreateReport(dataReport, urlPath);
        }
    }
}