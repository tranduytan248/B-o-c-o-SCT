using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using CenIT.ReportTourism.Biz.Cate;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Report;

namespace CenIT.ReportTourism.Biz.Report
{
    public class ReportDashboardBiz
    {
        private const string SnapshotProcedure = "Report_Dashboard_IndustrialSnapshot";
        private const string SummaryProcedure = "Report_Dashboard_OverviewSummary";
        private static readonly CultureInfo Vietnamese = CultureInfo.GetCultureInfo("vi-VN");
        private readonly ReportBiz _reports = new ReportBiz();

        private static DashboardMetric[] Metrics(int type)
        {
            if (type == 1) return new[]
            {
                new DashboardMetric { Key = "primary", Code = "1.1", Label = DashboardText.Get("Metric_IndustrialRevenue"), Unit = DashboardText.Get("Unit_BillionVnd") },
                new DashboardMetric { Key = "secondary", Code = "6", Label = DashboardText.Get("Metric_ExportValue"), Unit = DashboardText.Get("Unit_ThousandUsd") },
                new DashboardMetric { Key = "tertiary", Code = "7", Label = DashboardText.Get("Metric_ImportValue"), Unit = DashboardText.Get("Unit_ThousandUsd") }
            };
            if (type == 2) return new[]
            {
                new DashboardMetric { Key = "primary", Code = "1", Label = DashboardText.Get("Metric_WholesaleRetailRevenue"), Unit = DashboardText.Get("Unit_MillionVnd") },
                new DashboardMetric { Key = "secondary", Code = "2", Label = DashboardText.Get("Metric_VehicleRepairRevenue"), Unit = DashboardText.Get("Unit_MillionVnd") },
                new DashboardMetric { Key = "tertiary", Code = "1.1", Label = DashboardText.Get("Metric_RetailSubset"), Unit = DashboardText.Get("Unit_MillionVnd") }
            };
            return new[]
            {
                new DashboardMetric { Key = "primary", Code = "0", Label = DashboardText.Get("Metric_FobValue"), Unit = DashboardText.Get("Unit_Usd") },
                new DashboardMetric { Key = "secondary", Code = "1", Label = DashboardText.Get("Metric_DirectExport"), Unit = DashboardText.Get("Unit_Usd") },
                new DashboardMetric { Key = "tertiary", Code = "2", Label = DashboardText.Get("Metric_EntrustedExport"), Unit = DashboardText.Get("Unit_Usd") }
            };
        }

        public static string TypeName(int type)
        {
            return DashboardText.Get(type == 2 ? "Type_Trading" : type == 3 ? "Type_ExportImport" : "Type_Manufacturing");
        }

        public DashboardModel GetDashboard(DashboardFilters filters)
        {
            filters = Normalize(filters);
            var byType = Enumerable.Range(1, 3).ToDictionary(type => type, type => GetRows(filters, type));
            var cards = byType.Select(pair =>
            {
                var current = At(pair.Value, filters.Year, filters.Month).ToList();
                var metric = Metrics(pair.Key)[0];
                return new DashboardTypeCard
                {
                    ReportType = pair.Key, Name = TypeName(pair.Key), MetricLabel = metric.Label,
                    Unit = metric.Unit, Value = FormatOrDash(SumOrNull(current.Select(r => r.PrimaryValue))),
                    Assigned = current.Count, Received = current.Count(r => r.DataImported),
                    Conflicts = current.Count(r => r.MetricConflict)
                };
            }).ToList();
            var end = new DateTime(filters.Year, filters.Month, 1);
            var today = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            if (end > today) end = today;
            var trend = Enumerable.Range(0, 12).Select(offset => end.AddMonths(offset - 11)).Select(date =>
                new DashboardReceiptPoint
                {
                    Month = date.ToString("yyyy-MM"),
                    Type1 = At(byType[1], date.Year, date.Month).Count(r => r.DataImported),
                    Type2 = At(byType[2], date.Year, date.Month).Count(r => r.DataImported),
                    Type3 = At(byType[3], date.Year, date.Month).Count(r => r.DataImported)
                }).ToList();
            var movements = Comparable(byType[1], filters, "primary")
                .OrderByDescending(x => Math.Abs(x.Change)).Take(5).ToList();
            return new DashboardModel
            {
                Filters = filters, FilterOptions = BuildOptions(byType.Values.SelectMany(x => x).ToList(), filters, "Index", false),
                TypeCards = cards, Classification = GetSummary(filters), ReceiptTrend = trend,
                Movements = movements
            };
        }

        public DashboardAnalysisModel GetAnalysis(DashboardFilters filters)
        {
            filters = Normalize(filters);
            var rows = GetRows(filters, filters.ReportType);
            var current = At(rows, filters.Year, filters.Month).ToList();
            var metric = SelectedMetric(filters);
            var values = current.Select(r => Value(r, metric.Key));
            var monthly = Enumerable.Range(1, filters.Month).Select(month =>
                SumOrNull(At(rows, filters.Year, month).Select(r => Value(r, metric.Key)))).ToList();
            var ytdValues = monthly.Where(v => v.HasValue).ToList();
            var currentDate = new DateTime(filters.Year, filters.Month, 1);
            var trend = Enumerable.Range(0, 12).Select(offset => currentDate.AddMonths(offset - 11))
                .Where(date => date <= DateTime.Today)
                .Select(date => new DashboardMetricPoint
                {
                    Month = date.ToString("yyyy-MM"),
                    Value = SumOrNull(At(rows, date.Year, date.Month).Select(r => Value(r, metric.Key)))
                }).ToList();
            var sectorValues = current.GroupBy(r => String.IsNullOrWhiteSpace(r.EconomicSectorName) ? DashboardText.Get("UnknownSector") : r.EconomicSectorName)
                .Select(g => new DashboardBreakdown { Name = g.Key, Value = SumOrNull(g.Select(r => Value(r, metric.Key))) ?? 0 })
                .OrderByDescending(g => g.Value).ToList();
            var sectorTotal = sectorValues.Sum(g => g.Value);
            foreach (var sector in sectorValues) sector.Share = sectorTotal > 0 ? sector.Value * 100 / sectorTotal : 0;
            var mom = Comparable(rows, filters, metric.Key).ToList();
            var yoy = Comparable(rows, filters, metric.Key, -12).ToList();
            return new DashboardAnalysisModel
            {
                Filters = filters, FilterOptions = BuildOptions(rows, filters, "Analysis", true),
                TypeName = TypeName(filters.ReportType), SelectedMetric = metric, Metrics = Metrics(filters.ReportType),
                CurrentValue = FormatOrDash(SumOrNull(values)),
                YtdValue = ytdValues.Count == 0 ? "—" : Format(ytdValues.Sum(v => v.Value)),
                YtdMonths = ytdValues.Count,
                Mom = Rate(mom), Yoy = Rate(yoy), MomCompared = mom.Count, YoyCompared = yoy.Count,
                Assigned = current.Count,
                Trend = trend, Sectors = sectorValues,
                Movements = mom.OrderByDescending(x => Math.Abs(x.Change)).Take(8).ToList()
            };
        }

        public DashboardWarningsModel GetWarnings(DashboardFilters filters)
        {
            filters = Normalize(filters);
            var rows = GetRows(filters, filters.ReportType);
            var movements = Comparable(rows, filters, "primary").ToList();
            var current = At(rows, filters.Year, filters.Month).ToList();
            var end = new DateTime(filters.Year, filters.Month, 1);
            var trend = Enumerable.Range(0, 12).Select(offset => end.AddMonths(offset - 11))
                .Where(date => date <= DateTime.Today)
                .Select(date =>
                {
                    var period = new DashboardFilters { Year = date.Year, Month = date.Month };
                    var pairs = Comparable(rows, period, "primary").ToList();
                    return new DashboardMetricPoint
                    {
                        Month = date.ToString("yyyy-MM"),
                        Value = pairs.Count == 0 ? (decimal?)null :
                            pairs.Count(x => x.Percent.HasValue && x.Percent.Value < (filters.ReportType == 1 ? -10 : 0))
                    };
                }).ToList();
            return new DashboardWarningsModel
            {
                Filters = filters, FilterOptions = BuildOptions(rows, filters, "Warnings", false),
                TypeName = TypeName(filters.ReportType), Metric = Metrics(filters.ReportType)[0],
                IsIndustrial = filters.ReportType == 1, ComparableCount = movements.Count,
                Assigned = current.Count,
                DeclineOver10 = movements.Count(x => x.Percent < -10),
                DeclineOver20 = movements.Count(x => x.Percent < -20),
                DeclineOver30 = movements.Count(x => x.Percent < -30),
                ConflictCount = current.Count(r => r.MetricConflict), Trend = trend,
                Movements = (filters.ReportType == 1
                    ? movements.Where(x => x.Percent < -10).OrderBy(x => x.Change)
                    : movements.OrderByDescending(x => Math.Abs(x.Change))).Take(8).ToList()
            };
        }

        public DashboardProgressModel GetProgress(DashboardFilters filters)
        {
            filters = Normalize(filters);
            var rows = GetRows(filters, filters.ReportType);
            var current = At(rows, filters.Year, filters.Month).ToList();
            var metrics = Metrics(filters.ReportType);
            var end = new DateTime(filters.Year, filters.Month, 1);
            var trend = Enumerable.Range(0, 12).Select(offset => end.AddMonths(offset - 11))
                .Where(date => date <= DateTime.Today)
                .Select(date => new DashboardMetricPoint
                {
                    Month = date.ToString("yyyy-MM"),
                    Value = At(rows, date.Year, date.Month).Count(r => r.DataImported)
                }).ToList();
            return new DashboardProgressModel
            {
                Filters = filters, FilterOptions = BuildOptions(rows, filters, "Progress", false),
                TypeName = TypeName(filters.ReportType), Assigned = current.Count,
                Received = current.Count(r => r.DataImported),
                FileAnyType = current.Count(r => r.FileAnyType),
                MetricValues = current.Count(r => r.PrimaryValue.HasValue),
                MetricPresence = metrics.Select(metric => new DashboardBreakdown
                {
                    Name = metric.Label + " (" + metric.Code + ")",
                    Assigned = current.Count,
                    Received = current.Count(r => Value(r, metric.Key).HasValue)
                }).ToList(),
                Conflicts = current.Count(r => r.MetricConflict),
                CoveragePercent = current.Count == 0 ? 0 : 100m * current.Count(r => r.DataImported) / current.Count,
                ReceiptTrend = trend,
                Areas = Breakdown(current, r => r.WardName, DashboardText.Get("UnknownArea")),
                Sectors = Breakdown(current, r => r.EconomicSectorName, DashboardText.Get("UnknownSector")),
                Enterprises = current.OrderByDescending(r => r.MetricConflict).ThenBy(r => r.DataImported)
                    .ThenBy(r => r.PrimaryValue.HasValue)
                    .ThenBy(r => r.BusinessName).Select(r => new DashboardEnterpriseRow
                    {
                        EnterpriseId = r.EnterpriseId, Name = r.BusinessName, WardName = r.WardName,
                        DataImported = r.DataImported, FileAnyType = r.FileAnyType,
                        HasMetricValue = r.PrimaryValue.HasValue, MetricConflict = r.MetricConflict,
                        MissingMetricCodes = String.Join(", ", metrics
                            .Where(metric => !Value(r, metric.Key).HasValue).Select(metric => metric.Code))
                    }).ToList()
            };
        }

        private DashboardOverviewSummary GetSummary(DashboardFilters filters)
        {
            var table = _reports.GetDataReport(SummaryProcedure, new DateTime(filters.Year, filters.Month, 1));
            if (table.Rows.Count == 0) return new DashboardOverviewSummary();
            var row = table.Rows[0];
            return new DashboardOverviewSummary
            {
                ActiveEnterprises = Convert.ToInt32(row["ActiveEnterprises"]),
                ConfiguredEnterprises = Convert.ToInt32(row["ConfiguredEnterprises"]),
                MissingBusinessRow = Convert.ToInt32(row["MissingBusinessRow"]),
                MissingIndustry = Convert.ToInt32(row["MissingIndustry"]),
                MissingReportType = Convert.ToInt32(row["MissingReportType"]),
                TypeZeroRows = Convert.ToInt32(row["TypeZeroRows"]),
                FutureDatedRows = Convert.ToInt32(row["FutureDatedRows"])
            };
        }

        private List<SnapshotRow> GetRows(DashboardFilters filters, int reportType)
        {
            var table = _reports.GetDataReport(SnapshotProcedure,
                new DateTime(filters.Year, filters.Month, 1), reportType, DbFilter(filters.AreaId),
                DbFilter(filters.EconomicSectorId), DbFilter(filters.IndustryId),
                DbFilter(filters.EnterpriseId));
            return table.Rows.Cast<DataRow>().Select(row => new SnapshotRow
            {
                ForMonth = Convert.ToDateTime(row["ForMonth"]), EnterpriseId = Convert.ToInt32(row["EnterpriseId"]),
                BusinessName = Convert.ToString(row["BusinessName"]),
                WardId = NullableInt(row["WardId"]), WardName = Convert.ToString(row["WardName"]),
                EconomicSectorId = NullableInt(row["EconomicSectorId"]),
                EconomicSectorName = Convert.ToString(row["EconomicSectorName"]),
                IndustryIds = Convert.ToString(row["IndustryIds"]),
                DataImported = Convert.ToBoolean(row["DataImported"]),
                FileAnyType = Convert.ToBoolean(row["FileSubmittedAnyType"]),
                MetricConflict = Convert.ToBoolean(row["MetricConflict"]),
                PrimaryValue = NullableDecimal(row["PrimaryValue"]),
                SecondaryValue = NullableDecimal(row["SecondaryValue"]),
                TertiaryValue = NullableDecimal(row["TertiaryValue"])
            }).ToList();
        }

        private static object DbFilter(string value)
        {
            int parsed;
            return Int32.TryParse(value, out parsed) && parsed >= 0 ? (object)parsed : DBNull.Value;
        }

        private static int? NullableInt(object value) { return value == DBNull.Value ? (int?)null : Convert.ToInt32(value); }
        private static decimal? NullableDecimal(object value) { return value == DBNull.Value ? (decimal?)null : Convert.ToDecimal(value); }
        private static string Format(decimal value) { return value.ToString(decimal.Truncate(value) == value ? "#,##0" : "#,##0.##", Vietnamese); }
        private static string FormatOrDash(decimal? value) { return value.HasValue ? Format(value.Value) : "—"; }

        private static DashboardFilters Normalize(DashboardFilters filters)
        {
            filters = filters ?? new DashboardFilters();
            var previous = DateTime.Today.AddMonths(-1);
            if (filters.Year < 2000 || filters.Year > 2100 || filters.Month < 1 || filters.Month > 12 ||
                new DateTime(filters.Year, filters.Month, 1) > new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1))
            {
                filters.Year = previous.Year; filters.Month = previous.Month;
            }
            if (filters.ReportType < 1 || filters.ReportType > 3) filters.ReportType = 1;
            filters.AreaId = filters.AreaId ?? "all";
            filters.EconomicSectorId = filters.EconomicSectorId ?? "all";
            filters.IndustryId = filters.IndustryId ?? "all";
            filters.EnterpriseId = filters.EnterpriseId ?? "all";
            if (filters.Metric != "secondary" && filters.Metric != "tertiary") filters.Metric = "primary";
            return filters;
        }

        private static DashboardMetric SelectedMetric(DashboardFilters filters)
        {
            return Metrics(filters.ReportType).First(m => m.Key == filters.Metric);
        }

        private static IEnumerable<SnapshotRow> At(IEnumerable<SnapshotRow> rows, int year, int month)
        {
            return rows.Where(r => r.ForMonth.Year == year && r.ForMonth.Month == month);
        }

        private static decimal? Value(SnapshotRow row, string metric)
        {
            return metric == "secondary" ? row.SecondaryValue : metric == "tertiary" ? row.TertiaryValue : row.PrimaryValue;
        }

        private static decimal? SumOrNull(IEnumerable<decimal?> values)
        {
            var known = values.Where(v => v.HasValue).ToList();
            return known.Count == 0 ? (decimal?)null : known.Sum(v => v.Value);
        }

        private static List<DashboardMovement> Comparable(List<SnapshotRow> rows, DashboardFilters filters,
            string metric, int offset = -1)
        {
            var currentDate = new DateTime(filters.Year, filters.Month, 1);
            var previousDate = currentDate.AddMonths(offset);
            var previous = At(rows, previousDate.Year, previousDate.Month).ToDictionary(r => r.EnterpriseId);
            var result = new List<DashboardMovement>();
            foreach (var row in At(rows, filters.Year, filters.Month))
            {
                SnapshotRow old;
                if (!previous.TryGetValue(row.EnterpriseId, out old)) continue;
                var value = Value(row, metric);
                var baseline = Value(old, metric);
                if (!value.HasValue || !baseline.HasValue) continue;
                result.Add(new DashboardMovement
                {
                    Name = row.BusinessName, WardName = row.WardName,
                    CurrentValue = value.Value, PreviousValue = baseline.Value,
                    Change = value.Value - baseline.Value,
                    Percent = baseline.Value > 0 ? (value.Value - baseline.Value) * 100m / baseline.Value : (decimal?)null
                });
            }
            return result;
        }

        private static decimal? Rate(IList<DashboardMovement> movements)
        {
            if (movements.Count == 0) return null;
            var baseline = movements.Sum(m => m.PreviousValue);
            return baseline == 0 ? (decimal?)null : movements.Sum(m => m.Change) * 100m / baseline;
        }

        private static IList<DashboardBreakdown> Breakdown(List<SnapshotRow> rows,
            Func<SnapshotRow, string> label, string unknown)
        {
            return rows.GroupBy(r => String.IsNullOrWhiteSpace(label(r)) ? unknown : label(r))
                .Select(g => new DashboardBreakdown
                {
                    Name = g.Key, Assigned = g.Count(), Received = g.Count(r => r.DataImported)
                }).OrderByDescending(g => g.Received).ThenBy(g => g.Name).ToList();
        }

        private static DashboardFilterOptions BuildOptions(List<SnapshotRow> rows, DashboardFilters filters,
            string action, bool includeMetric)
        {
            var industryIds = rows.SelectMany(r => (r.IndustryIds ?? "").Split(','))
                .Select(id => id.Trim()).Where(id => id.Length > 0).Distinct().ToList();
            int total;
            var industryNames = (new CateBusinessIndustryBiz().Get(out total, null) ?? new List<CateBusinessIndustryModel>())
                .Where(i => i.IsActive && !i.IsDeleted)
                .GroupBy(i => i.IndustryId).ToDictionary(g => g.Key.ToString(), g => g.First().IndustryName);
            var industries = new List<DashboardOption> { new DashboardOption { Value = "all", Text = DashboardText.Get("Filter_AllIndustries") } };
            industries.AddRange(industryIds.Select(id => new DashboardOption
            {
                Value = id, Text = industryNames.ContainsKey(id) ? industryNames[id] : DashboardText.Get("Filter_IndustryCode") + " " + id
            }).OrderBy(x => x.Text));
            var years = rows.Where(r => r.DataImported || r.PrimaryValue.HasValue || r.SecondaryValue.HasValue || r.TertiaryValue.HasValue)
                .Select(r => r.ForMonth.Year).Concat(new[] { filters.Year, DateTime.Today.Year })
                .Distinct().OrderByDescending(y => y).Select(y => new DashboardOption { Value = y.ToString(), Text = y.ToString() }).ToList();
            return new DashboardFilterOptions
            {
                Filters = filters, Action = action, Years = years,
                Months = Enumerable.Range(1, 12).Select(m => new DashboardOption
                { Value = m.ToString(), Text = Vietnamese.DateTimeFormat.GetMonthName(m) }).ToList(),
                Areas = Options(rows, r => r.WardId, r => r.WardName, DashboardText.Get("Filter_AllAreas")),
                EconomicSectors = Options(rows, r => r.EconomicSectorId, r => r.EconomicSectorName, DashboardText.Get("Filter_AllSectors")),
                Industries = industries,
                Enterprises = Options(rows, r => (int?)r.EnterpriseId, r => r.BusinessName, DashboardText.Get("Filter_AllEnterprises")),
                Metrics = includeMetric ? Metrics(filters.ReportType).Select(m => new DashboardOption
                { Value = m.Key, Text = m.Label }).ToList() : new List<DashboardOption>()
            };
        }

        private static IList<DashboardOption> Options(List<SnapshotRow> rows, Func<SnapshotRow, int?> id,
            Func<SnapshotRow, string> label, string allLabel)
        {
            var options = new List<DashboardOption> { new DashboardOption { Value = "all", Text = allLabel } };
            options.AddRange(rows.Where(r => id(r).HasValue).GroupBy(r => id(r).Value)
                .Select(g => new DashboardOption { Value = g.Key.ToString(), Text = label(g.First()) })
                .OrderBy(x => x.Text));
            return options;
        }

        private sealed class SnapshotRow
        {
            public DateTime ForMonth;
            public int EnterpriseId;
            public string BusinessName;
            public int? WardId;
            public string WardName;
            public int? EconomicSectorId;
            public string EconomicSectorName;
            public string IndustryIds;
            public bool DataImported;
            public bool FileAnyType;
            public bool MetricConflict;
            public decimal? PrimaryValue;
            public decimal? SecondaryValue;
            public decimal? TertiaryValue;
        }
    }
}
