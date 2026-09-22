using System;
using System.Collections.Generic;
using System.Data;
using CenIT.ReportTourism.Models.Report;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Models
{
    public class ReportDataImportViewModel
    {
        [CustomDisplayName("Enterprise_Title")]
        public int? EnterpriseId { get; set; }

        [CustomDisplayName("DataImport_Label_ForMonth")]
        public DateTime? ForMonth { get; set; }

        public DataTable DataImports { get; set; }
        public List<ReportDataImportModel> ListDataImports { get; set; }

        public string ReponseMessage { get; set; }
    }
}