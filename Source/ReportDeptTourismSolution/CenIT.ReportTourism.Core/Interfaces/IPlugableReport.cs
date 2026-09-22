using System.Collections.Specialized;
using System.Data;
using System.Web;
using Microsoft.Reporting.WebForms;

namespace CenIT.ReportTourism.Core.Interfaces
{
    public interface IPlugableReport
    {
        string ReportKey { get; }
        string ReportName { get; }
        string StoreName { get; }
        string ViewName { get; }
        string Description { get; }

        void Export(HttpResponseBase response, DataTable data, string urlPathReport);
        ReportViewer CreateReport(DataTable data, string urlPathReport);
        object[] CreateParams(NameValueCollection param);
    }
}