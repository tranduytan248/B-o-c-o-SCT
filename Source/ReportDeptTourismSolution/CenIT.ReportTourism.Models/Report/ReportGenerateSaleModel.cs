using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Report
{
    public class ReportGenerateSaleModel
    {
        [CustomDisplayName("DataImport_Label_Targets")]
        public string Targets { get; set; }

        [CustomDisplayName("DataImport_Label_Unit")]
        public string Unit { get; set; }

        [CustomDisplayName("DataImport_Label_PerformPreviousPeriod")]
        public double? PerformPreviousPeriod { get; set; }

        [CustomDisplayName("DataImport_Label_PerformInPeriod")]
        public double? PerformInPeriod { get; set; }

        [CustomDisplayName("DataImport_Label_SamePeriodRateOfPerform")]
        public double? SamePeriodRateOfPerform { get; set; }

        [CustomDisplayName("DataImport_Label_SamePeriodRateOfAccumulated")]
        public double? SamePeriodRateOfAccumulated { get; set; }

        [CustomDisplayName("DataImport_Label_AccumulatedBeginingOfYear")]
        public double? AccumulatedBeginingOfYear { get; set; }
    }
}