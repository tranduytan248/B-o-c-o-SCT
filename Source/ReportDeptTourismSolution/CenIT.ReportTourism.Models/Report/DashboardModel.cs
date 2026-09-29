using System;
using System.Collections.Generic;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Models.Report
{
    public static class DashboardText
    {
        public static string Get(string key)
        {
            return AppProcessor.Messagor.GetMessage("Dashboard_" + key);
        }
    }

    public class DashboardFilters
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int ReportType { get; set; }
        public string AreaId { get; set; }
        public string EconomicSectorId { get; set; }
        public string IndustryId { get; set; }
        public string EnterpriseId { get; set; }
        public string Metric { get; set; }
    }

    public class DashboardOption
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public class DashboardFilterOptions
    {
        public DashboardFilters Filters { get; set; }
        public string Action { get; set; }
        public IList<DashboardOption> Years { get; set; }
        public IList<DashboardOption> Months { get; set; }
        public IList<DashboardOption> Areas { get; set; }
        public IList<DashboardOption> EconomicSectors { get; set; }
        public IList<DashboardOption> Industries { get; set; }
        public IList<DashboardOption> Enterprises { get; set; }
        public IList<DashboardOption> Metrics { get; set; }
    }

    public class DashboardNavigationModel
    {
        public string Active { get; set; }
        public DashboardFilters Filters { get; set; }
    }

    public class DashboardMetric
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public string Unit { get; set; }
        public string Code { get; set; }
    }

    public class DashboardTypeCard
    {
        public int ReportType { get; set; }
        public string Name { get; set; }
        public string MetricLabel { get; set; }
        public string Unit { get; set; }
        public string Value { get; set; }
        public int Assigned { get; set; }
        public int Received { get; set; }
        public int Conflicts { get; set; }
    }

    public class DashboardOverviewSummary
    {
        public int ActiveEnterprises { get; set; }
        public int ConfiguredEnterprises { get; set; }
        public int MissingBusinessRow { get; set; }
        public int MissingIndustry { get; set; }
        public int MissingReportType { get; set; }
        public int TypeZeroRows { get; set; }
        public int FutureDatedRows { get; set; }
        public int NeedsClassification { get { return ActiveEnterprises - ConfiguredEnterprises; } }
    }

    public class DashboardReceiptPoint
    {
        public string Month { get; set; }
        public int Type1 { get; set; }
        public int Type2 { get; set; }
        public int Type3 { get; set; }
    }

    public class DashboardMetricPoint
    {
        public string Month { get; set; }
        public decimal? Value { get; set; }
    }

    public class DashboardBreakdown
    {
        public string Name { get; set; }
        public int Assigned { get; set; }
        public int Received { get; set; }
        public decimal Value { get; set; }
        public decimal Share { get; set; }
    }

    public class DashboardMovement
    {
        public string Name { get; set; }
        public string WardName { get; set; }
        public decimal PreviousValue { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal Change { get; set; }
        public decimal? Percent { get; set; }
    }

    public class DashboardEnterpriseRow
    {
        public int EnterpriseId { get; set; }
        public string Name { get; set; }
        public string WardName { get; set; }
        public bool DataImported { get; set; }
        public bool FileAnyType { get; set; }
        public bool HasMetricValue { get; set; }
        public bool MetricConflict { get; set; }
        public string MissingMetricCodes { get; set; }
    }

    public class DashboardModel
    {
        public DashboardFilters Filters { get; set; }
        public DashboardFilterOptions FilterOptions { get; set; }
        public IList<DashboardTypeCard> TypeCards { get; set; }
        public DashboardOverviewSummary Classification { get; set; }
        public IList<DashboardReceiptPoint> ReceiptTrend { get; set; }
        public IList<DashboardMovement> Movements { get; set; }
    }

    public class DashboardAnalysisModel
    {
        public DashboardFilters Filters { get; set; }
        public DashboardFilterOptions FilterOptions { get; set; }
        public string TypeName { get; set; }
        public DashboardMetric SelectedMetric { get; set; }
        public IList<DashboardMetric> Metrics { get; set; }
        public string CurrentValue { get; set; }
        public string YtdValue { get; set; }
        public int YtdMonths { get; set; }
        public decimal? Mom { get; set; }
        public decimal? Yoy { get; set; }
        public int MomCompared { get; set; }
        public int YoyCompared { get; set; }
        public int Assigned { get; set; }
        public IList<DashboardMetricPoint> Trend { get; set; }
        public IList<DashboardBreakdown> Sectors { get; set; }
        public IList<DashboardMovement> Movements { get; set; }
    }

    public class DashboardWarningsModel
    {
        public DashboardFilters Filters { get; set; }
        public DashboardFilterOptions FilterOptions { get; set; }
        public string TypeName { get; set; }
        public DashboardMetric Metric { get; set; }
        public bool IsIndustrial { get; set; }
        public int ComparableCount { get; set; }
        public int Assigned { get; set; }
        public int DeclineOver10 { get; set; }
        public int DeclineOver20 { get; set; }
        public int DeclineOver30 { get; set; }
        public int ConflictCount { get; set; }
        public IList<DashboardMetricPoint> Trend { get; set; }
        public IList<DashboardMovement> Movements { get; set; }
    }

    public class DashboardProgressModel
    {
        public DashboardFilters Filters { get; set; }
        public DashboardFilterOptions FilterOptions { get; set; }
        public string TypeName { get; set; }
        public int Assigned { get; set; }
        public int Received { get; set; }
        public int FileAnyType { get; set; }
        public int MetricValues { get; set; }
        public IList<DashboardBreakdown> MetricPresence { get; set; }
        public int Conflicts { get; set; }
        public decimal CoveragePercent { get; set; }
        public IList<DashboardMetricPoint> ReceiptTrend { get; set; }
        public IList<DashboardBreakdown> Areas { get; set; }
        public IList<DashboardBreakdown> Sectors { get; set; }
        public IList<DashboardEnterpriseRow> Enterprises { get; set; }
    }
}
