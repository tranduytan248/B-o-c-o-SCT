using System;
using System.Collections.Generic;

namespace CenIT.ReportTourism.Models.Report
{
    public class DashboardModel
    {
        public DashboardFilters Filters { get; set; }
        public DashboardFilterOptions FilterOptions { get; set; }
        public IList<DashboardOption> Years { get; set; }
        public IList<DashboardOption> Months { get; set; }
        public IList<DashboardOption> Areas { get; set; }
        public IList<DashboardOption> EconomicSectors { get; set; }
        public IList<DashboardOption> Industries { get; set; }
        public IList<DashboardOption> Enterprises { get; set; }
        public IList<DashboardKpi> Kpis { get; set; }
        public IList<DashboardTrendPoint> MonthlyTrend { get; set; }
        public IList<DashboardSectorContribution> SectorContributions { get; set; }
        public IList<DashboardIndustryImpact> IndustryContributions { get; set; }
        public IList<DashboardEnterpriseImpact> IncreasingEnterprises { get; set; }
        public IList<DashboardEnterpriseImpact> DecreasingEnterprises { get; set; }
        public DashboardSubmissionStatus Submission { get; set; }
        public DashboardAlertSummary Alerts { get; set; }
        public bool HasComparisonData { get; set; }
        public IList<string> ReportedReasons { get; set; }
        public string FilterSummary { get; set; }
    }

    public class DashboardFilters
    {
        public int Year { get; set; }
        public int Month { get; set; }
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

    public class DashboardKpi
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public decimal? Mom { get; set; }
        public decimal? Yoy { get; set; }
        public string Note { get; set; }
        public string Tone { get; set; }
    }

    public class DashboardTrendPoint
    {
        public string Month { get; set; }
        public decimal? GtsXcnIndex { get; set; }
        public decimal? ExportIndex { get; set; }
        public decimal? ImportIndex { get; set; }
        public decimal Completion { get; set; }
        public int Over10 { get; set; }
        public int Over20 { get; set; }
        public int Over30 { get; set; }
    }

    public class DashboardSectorContribution
    {
        public string Name { get; set; }
        public decimal GtsXcnShare { get; set; }
        public decimal ExportShare { get; set; }
        public decimal ImportShare { get; set; }
    }

    public class DashboardIndustryImpact
    {
        public string Name { get; set; }
        public decimal ChangePercent { get; set; }
        public decimal Contribution { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal PreviousValue { get; set; }
        public decimal? ContributionShare { get; set; }
        public bool HasGrowthRate { get; set; }
    }

    public class DashboardEnterpriseImpact
    {
        public string Name { get; set; }
        public string Industry { get; set; }
        public decimal ChangePercent { get; set; }
        public decimal Contribution { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal PreviousValue { get; set; }
        public decimal? ContributionShare { get; set; }
        public bool HasGrowthRate { get; set; }
    }

    public class DashboardSubmissionStatus
    {
        public int TotalEnterprises { get; set; }
        public int Submitted { get; set; }
        public int CompleteIndicators { get; set; }
        public int NotSubmitted { get; set; }
        public int Late { get; set; }
        public int MissingIndicators { get; set; }
        public decimal CompletionPercent { get; set; }
    }

    public class DashboardAlertSummary
    {
        public int DeclineOver10 { get; set; }
        public int DeclineOver20 { get; set; }
        public int DeclineOver30 { get; set; }
        public int UrgentEnterprises { get; set; }
    }

    public class DashboardProgressBreakdown
    {
        public string Name { get; set; }
        public int Submitted { get; set; }
        public int NotSubmitted { get; set; }
        public decimal CompletionPercent { get; set; }
    }

    public class DashboardAnalysisModel
    {
        public DashboardFilters Filters { get; set; }
        public DashboardFilterOptions FilterOptions { get; set; }
        public IList<DashboardOption> Metrics { get; set; }
        public string MetricLabel { get; set; }
        public string CurrentValue { get; set; }
        public string YtdValue { get; set; }
        public decimal? Mom { get; set; }
        public decimal? Yoy { get; set; }
        public IList<DashboardSectorContribution> SectorContributions { get; set; }
        public IList<DashboardIndustryImpact> IndustryContributions { get; set; }
        public IList<DashboardEnterpriseImpact> EnterpriseImpact { get; set; }
        public IList<string> Reasons { get; set; }
    }

    public class DashboardWarningsModel
    {
        public DashboardFilters Filters { get; set; }
        public DashboardFilterOptions FilterOptions { get; set; }
        public DashboardAlertSummary Summary { get; set; }
        public bool HasComparisonData { get; set; }
        public IList<DashboardTrendPoint> Trend { get; set; }
        public IList<DashboardIndustryImpact> ByIndustry { get; set; }
        public IList<DashboardEnterpriseImpact> UrgentEnterprises { get; set; }
    }

    public class DashboardProgressModel
    {
        public DashboardFilters Filters { get; set; }
        public DashboardFilterOptions FilterOptions { get; set; }
        public DashboardSubmissionStatus Submission { get; set; }
        public IList<DashboardTrendPoint> Trend { get; set; }
        public IList<DashboardProgressBreakdown> Areas { get; set; }
        public IList<DashboardProgressBreakdown> Industries { get; set; }
        public IList<DashboardProgressBreakdown> OutstandingAreas { get; set; }
    }

    public class DashboardSubmissionDetailsModel
    {
        public DashboardSubmissionStatus Submission { get; set; }
        public DashboardFilters Filters { get; set; }
        public bool ShowDetailsLink { get; set; }
    }

    public class DashboardQualityModel
    {
        public DashboardFilters Filters { get; set; }
        public DashboardFilterOptions FilterOptions { get; set; }
        public DashboardSubmissionStatus Submission { get; set; }
        public int IndustrialRevenueCoverage { get; set; }
        public int ExportCoverage { get; set; }
        public int ImportCoverage { get; set; }
        public int IndustrialRevenueHistoryMonths { get; set; }
        public int ExportHistoryMonths { get; set; }
        public int ImportHistoryMonths { get; set; }
        public IList<DashboardQualityEnterprise> MissingIndicatorEnterprises { get; set; }
    }

    public class DashboardQualityEnterprise
    {
        public string Name { get; set; }
        public string MissingCodes { get; set; }
        public bool IsLate { get; set; }
    }
}
