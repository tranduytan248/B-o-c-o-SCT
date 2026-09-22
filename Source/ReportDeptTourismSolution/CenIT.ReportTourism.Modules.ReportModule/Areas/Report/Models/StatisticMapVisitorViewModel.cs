using System;
using System.Collections.Generic;
using CenIT.ReportTourism.Models.Report;

namespace CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Models
{
    public class StatisticMapVisitorViewModel
    {
        public DateTime OnMonth { get; set; }

        public List<ReportStatisticMapVisitorModel> DataStatistic { get; set; }
    }
}