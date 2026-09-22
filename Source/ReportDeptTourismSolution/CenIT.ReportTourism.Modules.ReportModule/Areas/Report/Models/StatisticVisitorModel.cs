using System;
using System.Collections.Generic;
using CenIT.ReportTourism.Models.Report;

namespace CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Models
{
    public class StatisticVisitorModel
    {
        public DateTime ForMonth { get; set; }
        public List<ReportStatisticVisitorModel> DataStatistic { get; set; }
    }
}