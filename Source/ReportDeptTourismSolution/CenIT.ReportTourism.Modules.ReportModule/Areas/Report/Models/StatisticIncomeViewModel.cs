using System;
using System.Collections.Generic;
using CenIT.ReportTourism.Models.Report;

namespace CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Models
{
    public class StatisticIncomeViewModel
    {
        public DateTime OnMonth { get; set; }
        public int? TypeStatistic { get; set; } = 1;
        public string Title { get; set; } = "Doanh thu Lưu trú";
        public List<ReportStatisticIncomeModel> DataStatisticIncome { get; set; }
    }
}