using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using CenIT.ReportTourism.Core.Conts;
using CenIT.ReportTourism.Models.Report;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Models
{
    public class TourismReportModel
    {
        [CustomDisplayName("DataImport_Label_Enterprise")]
        [CustomRequired]
        public int? EnterpriseId { get; set; }

        [CustomDisplayName("DataImport_Label_Enterprise")]
        public string EnterpriseName { get; set; }

        [CustomDisplayName("DataImport_Label_ForMonth")]
        [CustomRequired]
        public DateTime ForMonth { get; set; } = DateTime.Now;

        public int DayDeadlineSendReport { get; set; } = 0;
        public int DayDeadlineSendReportLate { get; set; } = 0;

        public string SavedBy { get; set; }

        public List<ListItem> ListEnterprises { get; set; }

        public DataTable DataImports { get; set; }
        public List<ReportDataImportModel> ListDataImports { get; set; }

        [CustomDisplayName("DataImport_Label_TypeReport")]
        public EnumTypeBusiness TypeReport { get; set; }

        [CustomDisplayName("DataImport_Label_TypeReport")]
        public string TypeReportName { get; set; }

        public bool IsEdit { get; set; } = false;

        //[CustomDisplayName("Reason_Title")]
        //[CustomRequired]
        public string Reason { get; set; }

        public string AccessToken { get; set; }

        public string ReportFile { get; set; }

        public bool EnableSignDigitalDoc { get; set; } = false;
    }
}