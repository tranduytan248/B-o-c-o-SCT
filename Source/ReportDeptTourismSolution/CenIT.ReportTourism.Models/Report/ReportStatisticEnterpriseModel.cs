using System;

namespace CenIT.ReportTourism.Models.Report
{
    public class ReportStatisticEnterpriseModel
    {
        public DateTime OnMonth { get; set; }
        public int TotalEnterpriseSubmitReportLate { get; set; }
        public int TotalEnterpriseNotSubmitReportYet { get; set; }
        public int TotalReportSubmited { get; set; }
        public int TotalEnterprise { get; set; }
    }
}