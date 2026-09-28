using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Models
{
    public class ReportViewModel
    {
        public string ReportKey { get; set; } = string.Empty;
        public string ReportName { get; set; } = string.Empty;
        public string ViewName { get; set; } = string.Empty;
        public string Reporter { get; set; } = string.Empty;

        #region Enterprise

        public int? TypeReport { get; set; } = 1;

        [CustomDisplayName("Enterprise_Title")]
        public int? EnterpriseId { get; set; }

        public string EnterpriseName { get; set; } = string.Empty;

        [CustomDisplayName("Enterprise_Title")]
        public List<ListItem> ListEnterprises { get; set; } = new List<ListItem>();

        #endregion
    }
}