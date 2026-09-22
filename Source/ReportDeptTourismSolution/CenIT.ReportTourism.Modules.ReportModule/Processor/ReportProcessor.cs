using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Core.Interfaces;
using CenIT.ReportTourism.Modules.ReportModule.Providers;

namespace CenIT.ReportTourism.Modules.ReportModule.Processor
{
    public class ReportProcessor
    {
        private static List<IPlugableReport> _reports;

        public static List<IPlugableReport> Reports
        {
            get
            {
                if (_reports == null || _reports.Count == 0) _reports = ReportPlugableProvider.LoadReports();

                return _reports;
            }
            set { _reports = value; }
        }

        public static IPlugableReport GetReportByKey(string reportKey)
        {
            var reportMatch = Reports?.Where(p => p.ReportKey == reportKey).ToList();
            return reportMatch?.Count > 0 ? reportMatch.First() : null;
        }
    }
}