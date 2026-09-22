using System;

namespace CenIT.ReportTourism.Models.Report
{
    public class ReportStatisticIncomeModel
    {
        public DateTime OnPeriodMonth { get; set; }
        public DateTime OnMonth { get; set; }
        public double TotalIncomePeriod { get; set; }
        public double TotalIncome { get; set; }
    }
}