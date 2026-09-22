using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Models
{
    public class ReportDataImportSearchModel
    {
        public ReportDataImportSearchModel()
        {
            ListEnterprises = new List<ListItem>();
        }

        [CustomDisplayName("Enterprise_Title")]
        public string EnterpriseIds { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        public List<int> ListEnterpriseId { get; set; }

        [CustomDisplayName("DataImport_Label_ForMonth")]
        public DateTime? FromMonth { get; set; } = DateTime.Now;

        [CustomDisplayName("Enterprise_Label_TypeBusiness")]
        public List<int> ListTypeBusinessId { get; set; }

        [CustomDisplayName("Enterprise_Label_TypeBusiness")]
        public string TypeBusinessIds { get; set; }

        [CustomDisplayName("DataImport_Label_ForMonth")]
        public DateTime? ToMonth { get; set; } = DateTime.Now;

        [CustomDisplayName("Enterprise_Title")]
        public List<ListItem> ListEnterprises { get; set; }

        [CustomDisplayName("Enterprise_Label_TypeBusiness")]
        public List<ListItem> ListTypeBusiness { get; set; }

        public int DayDeadlineSendReport { get; set; } = 0;
        public int DayDeadlineSendReportLate { get; set; } = 0;

        public bool ExistEnterpriseSubmitReportYet { get; set; }

        public bool EnableSignDigitalDoc { get; set; } = false;
    }

    public class ReportDataImportViewSearchModel
    {
        [CustomDisplayName("Enterprise_Title")]
        public int? EnterpriseId { get; set; }

        [CustomDisplayName("DataImport_Label_ForMonth")]
        public DateTime? ForMonth { get; set; }
    }
}