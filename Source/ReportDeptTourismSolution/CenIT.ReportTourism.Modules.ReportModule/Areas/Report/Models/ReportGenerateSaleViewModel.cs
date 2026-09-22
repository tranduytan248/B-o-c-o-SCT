using System;
using System.Collections.Generic;
using CenIT.ReportTourism.Models.Report;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Models
{
    public class ReportGenerateSaleViewModel
    {
        [CustomDisplayName("DataImport_Label_OnMonth")]
        public DateTime? OnMonth { get; set; }

        public List<ReportGenerateSaleModel> ListReportGenerateSales { get; set; }
    }
}