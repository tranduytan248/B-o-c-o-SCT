using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using CenIT.ReportTourism.Models.Report;

namespace CenIT.ReportTourism.Biz.Report
{
    public class ReportDashboardBiz
    {
        // One source of truth for industrial-revenue warning thresholds.
        private const decimal Warning10 = 10m;
        private const decimal Warning20 = 20m;
        private const decimal Warning30 = 30m;
        private const string SnapshotProcedure = "Report_Dashboard_IndustrialSnapshot";
        private static readonly CultureInfo Vietnamese = CultureInfo.GetCultureInfo("vi-VN");
        private readonly ReportBiz _reports = new ReportBiz();

        public DashboardModel GetDashboard(DashboardFilters filters)
        {
            filters = Normalize(filters);
            var rows = GetRows(filters);
            var current = At(rows, filters.Year, filters.Month).ToList();
            var submission = Submission(current);
            var trend = IndicatorTrend(rows, filters);
            var filterOptions = BuildFilterOptions(rows, filters, "Index", false);
            var industrial = Kpi(rows, filters, "industrial", "Doanh thu công nghiệp", "Tỷ đồng", "blue");
            var exports = Kpi(rows, filters, "export", "Xuất khẩu", "1.000 USD", "green");
            var imports = Kpi(rows, filters, "import", "Nhập khẩu", "1.000 USD", "amber");
            var impacts = Comparable(rows, filters, "industrial").ToList();

            return new DashboardModel
            {
                Filters = filters,
                FilterOptions = filterOptions,
                Years = filterOptions.Years, Months = filterOptions.Months,
                Areas = filterOptions.Areas, EconomicSectors = filterOptions.EconomicSectors,
                Industries = filterOptions.Industries, Enterprises = filterOptions.Enterprises,
                Kpis = new List<DashboardKpi>
                {
                    industrial, exports, imports,
                    new DashboardKpi { Label = "Tỷ lệ báo cáo", Value = submission.CompletionPercent.ToString("0.0", Vietnamese),
                        Unit = "%", Tone = "blue", Note = submission.Submitted + " / " + submission.TotalEnterprises + " đơn vị trong nhóm theo dõi đã nộp" }
                },
                MonthlyTrend = trend,
                SectorContributions = SectorShares(current),
                IndustryContributions = ImpactsByIndustry(impacts),
                IncreasingEnterprises = EnterpriseImpacts(impacts.Where(x => x.Change > 0).OrderByDescending(x => x.Change).Take(3), impacts.Sum(x => x.Change)),
                DecreasingEnterprises = EnterpriseImpacts(impacts.Where(x => x.Change < 0).OrderBy(x => x.Change).Take(3), impacts.Sum(x => x.Change)),
                Submission = submission,
                Alerts = Alerts(impacts),
                HasComparisonData = impacts.Any(x => x.HasRate),
                ReportedReasons = Reasons(current),
                FilterSummary = "Tháng " + filters.Month.ToString("00") + "/" + filters.Year + " · " + current.Count + " doanh nghiệp thuộc nhóm theo dõi đã phân loại trong phạm vi lọc"
            };
        }

        public DashboardAnalysisModel GetAnalysis(DashboardFilters filters)
        {
            filters = Normalize(filters);
            var rows = GetRows(filters);
            var current = At(rows, filters.Year, filters.Month).ToList();
            var metric = filters.Metric;
            var impacts = Comparable(rows, filters, metric).ToList();
            var label = metric == "industrial" ? "Doanh thu công nghiệp" : metric == "import" ? "Nhập khẩu" : "Xuất khẩu";
            var unit = metric == "industrial" ? "Tỷ đồng" : "1.000 USD";
            var values = current.Select(r => Value(r, metric)).Where(v => v.HasValue).ToList();
            var ytdMonths = Enumerable.Range(1, filters.Month).Select(month => At(rows, filters.Year, month)
                .Select(r => Value(r, metric)).Where(v => v.HasValue).ToList()).ToList();
            var ytd = ytdMonths.All(month => month.Count > 0)
                ? (decimal?)ytdMonths.Sum(month => month.Sum(v => v.Value)) : null;
            var filterOptions = BuildFilterOptions(rows, filters, "Analysis", true);
            return new DashboardAnalysisModel
            {
                Filters = filters,
                FilterOptions = filterOptions,
                Metrics = filterOptions.Metrics,
                MetricLabel = label,
                CurrentValue = values.Count == 0 ? "Chưa có dữ liệu" : Format(values.Sum(v => v.Value)) + " " + unit,
                YtdValue = ytd.HasValue ? Format(ytd.Value) + " " + unit : "Chưa đủ kỳ báo cáo",
                Mom = Rate(rows, filters, metric, -1),
                Yoy = Rate(rows, filters, metric, -12),
                SectorContributions = SectorShares(current).Where(x =>
                    (metric == "industrial" ? x.GtsXcnShare : metric == "import" ? x.ImportShare : x.ExportShare) > 0).ToList(),
                IndustryContributions = ImpactsByIndustry(impacts),
                EnterpriseImpact = EnterpriseImpacts(impacts.OrderByDescending(x => Math.Abs(x.Change)).Take(8), impacts.Sum(x => x.Change)),
                Reasons = Reasons(current)
            };
        }

        public DashboardWarningsModel GetWarnings(DashboardFilters filters)
        {
            filters = Normalize(filters);
            var rows = GetRows(filters);
            var impacts = Comparable(rows, filters, "industrial").ToList();
            var warnings = impacts.Where(x => x.HasRate && x.Percent < -Warning10).ToList();
            var trend = new List<DashboardTrendPoint>();
            foreach (var month in Enumerable.Range(1, filters.Month))
            {
                var monthFilter = new DashboardFilters { Year = filters.Year, Month = month };
                var monthImpacts = Comparable(rows, monthFilter, "industrial").ToList();
                if (monthImpacts.Count == 0) continue;
                trend.Add(new DashboardTrendPoint
                {
                    Month = filters.Year + "-" + month.ToString("00"),
                    Over10 = monthImpacts.Count(x => x.HasRate && x.Percent < -Warning10),
                    Over20 = monthImpacts.Count(x => x.HasRate && x.Percent < -Warning20),
                    Over30 = monthImpacts.Count(x => x.HasRate && x.Percent < -Warning30)
                });
            }
            return new DashboardWarningsModel
            {
                Filters = filters,
                FilterOptions = BuildFilterOptions(rows, filters, "Warnings", false),
                Summary = Alerts(impacts),
                HasComparisonData = impacts.Any(x => x.HasRate),
                Trend = trend,
                ByIndustry = warnings.GroupBy(x => x.Current.IndustryName ?? "Chưa phân ngành")
                    .Select(g => new DashboardIndustryImpact { Name = g.Key, Contribution = g.Count() })
                    .OrderByDescending(x => x.Contribution).Take(8).ToList(),
                UrgentEnterprises = EnterpriseImpacts(warnings.OrderBy(x => x.Change).ThenBy(x => x.Percent).Take(8), impacts.Sum(x => x.Change))
            };
        }

        public DashboardProgressModel GetProgress(DashboardFilters filters)
        {
            filters = Normalize(filters);
            var rows = GetRows(filters);
            var current = At(rows, filters.Year, filters.Month).ToList();
            var area = Breakdown(current, r => r.WardName ?? "Chưa có địa bàn");
            var industry = Breakdown(current, r => r.IndustryName ?? "Chưa phân ngành");
            var trend = new List<DashboardTrendPoint>();
            foreach (var month in Enumerable.Range(1, filters.Month))
            {
                var monthRows = At(rows, filters.Year, month).ToList();
                if (!monthRows.Any(r => r.Submitted)) continue;
                trend.Add(new DashboardTrendPoint
                {
                    Month = filters.Year + "-" + month.ToString("00"),
                    Completion = Submission(monthRows).CompletionPercent
                });
            }
            return new DashboardProgressModel
            {
                Filters = filters,
                FilterOptions = BuildFilterOptions(rows, filters, "Progress", false),
                Submission = Submission(current),
                Trend = trend,
                Areas = area,
                Industries = industry,
                OutstandingAreas = area.Concat(industry).Where(x => x.NotSubmitted > 0)
                    .OrderByDescending(x => x.NotSubmitted).Take(4).ToList()
            };
        }

        public DashboardQualityModel GetQuality(DashboardFilters filters)
        {
            filters = Normalize(filters);
            var rows = GetRows(filters);
            var current = At(rows, filters.Year, filters.Month).ToList();
            var start = new DateTime(filters.Year, filters.Month, 1).AddMonths(-11);
            var end = new DateTime(filters.Year, filters.Month, 1);
            var history = rows.Where(r => r.ForMonth >= start && r.ForMonth <= end).ToList();
            return new DashboardQualityModel
            {
                Filters = filters,
                FilterOptions = BuildFilterOptions(rows, filters, "Quality", false),
                Submission = Submission(current),
                IndustrialRevenueCoverage = current.Count(r => r.Submitted && r.IndustrialRevenue.HasValue),
                ExportCoverage = current.Count(r => r.Submitted && r.ExportValue.HasValue),
                ImportCoverage = current.Count(r => r.Submitted && r.ImportValue.HasValue),
                IndustrialRevenueHistoryMonths = history.Where(r => r.IndustrialRevenue.HasValue).Select(r => r.ForMonth).Distinct().Count(),
                ExportHistoryMonths = history.Where(r => r.ExportValue.HasValue).Select(r => r.ForMonth).Distinct().Count(),
                ImportHistoryMonths = history.Where(r => r.ImportValue.HasValue).Select(r => r.ForMonth).Distinct().Count(),
                MissingIndicatorEnterprises = current.Where(r => r.Submitted && !HasRequiredIndicators(r))
                    .Select(r => new DashboardQualityEnterprise
                    {
                        Name = r.BusinessName,
                        IsLate = r.IsLate,
                        MissingCodes = String.Join(", ", new[]
                        {
                            r.IndustrialRevenue.HasValue ? null : "0101",
                            r.ExportValue.HasValue ? null : "06",
                            r.ImportValue.HasValue ? null : "07"
                        }.Where(code => code != null))
                    }).OrderBy(r => r.Name).Take(12).ToList()
            };
        }

        private List<SnapshotRow> GetRows(DashboardFilters filters)
        {
            var table = _reports.GetDataReport(SnapshotProcedure,
                new DateTime(filters.Year, filters.Month, 1), DbFilter(filters.AreaId),
                DbFilter(filters.EconomicSectorId), DbFilter(filters.IndustryId), DbFilter(filters.EnterpriseId));
            return table.Rows.Cast<DataRow>().Select(row => new SnapshotRow
            {
                ForMonth = Convert.ToDateTime(row["ForMonth"]),
                EnterpriseId = Convert.ToInt32(row["EnterpriseId"]),
                BusinessName = Convert.ToString(row["BusinessName"]),
                WardId = NullableInt(row["WardId"]), WardName = Convert.ToString(row["WardName"]),
                EconomicSectorId = NullableInt(row["EconomicSectorId"]),
                EconomicSectorName = Convert.ToString(row["EconomicSectorName"]),
                IndustryId = NullableInt(row["IndustryId"]), IndustryName = Convert.ToString(row["IndustryName"]),
                Submitted = Convert.ToBoolean(row["Submitted"]), IsLate = Convert.ToBoolean(row["IsLate"]),
                Reason = Convert.ToString(row["Reason"]),
                IndustrialRevenue = NullableDecimal(row["IndustrialRevenue"]),
                ExportValue = NullableDecimal(row["ExportValue"]), ImportValue = NullableDecimal(row["ImportValue"])
            }).ToList();
        }

        private static object DbFilter(string value)
        {
            int parsed;
            return Int32.TryParse(value, out parsed) && parsed > 0 ? (object)parsed : DBNull.Value;
        }

        private static int? NullableInt(object value) { return value == DBNull.Value ? (int?)null : Convert.ToInt32(value); }
        private static decimal? NullableDecimal(object value) { return value == DBNull.Value ? (decimal?)null : Convert.ToDecimal(value); }

        private static DashboardFilters Normalize(DashboardFilters filters)
        {
            filters = filters ?? new DashboardFilters();
            if (filters.Year < 2000 || filters.Year > 2100) filters.Year = DateTime.Today.Year;
            if (filters.Month < 1 || filters.Month > 12) filters.Month = DateTime.Today.Month;
            filters.AreaId = filters.AreaId ?? "all";
            filters.EconomicSectorId = filters.EconomicSectorId ?? "all";
            filters.IndustryId = filters.IndustryId ?? "all";
            filters.EnterpriseId = filters.EnterpriseId ?? "all";
            filters.Metric = filters.Metric == "export" || filters.Metric == "import" ? filters.Metric : "industrial";
            return filters;
        }

        private static IEnumerable<SnapshotRow> At(IEnumerable<SnapshotRow> rows, int year, int month)
        {
            return rows.Where(r => r.ForMonth.Year == year && r.ForMonth.Month == month);
        }

        private static decimal? Value(SnapshotRow row, string metric)
        {
            return metric == "industrial" ? row.IndustrialRevenue : metric == "import" ? row.ImportValue : row.ExportValue;
        }

        private static DashboardKpi Kpi(List<SnapshotRow> rows, DashboardFilters filters, string metric,
            string label, string unit, string tone)
        {
            var values = At(rows, filters.Year, filters.Month).Select(r => Value(r, metric)).Where(v => v.HasValue).ToList();
            return new DashboardKpi
            {
                Label = label, Value = values.Count == 0 ? "—" : Format(values.Sum(v => v.Value)),
                Unit = unit, Tone = tone, Mom = Rate(rows, filters, metric, -1),
                Yoy = Rate(rows, filters, metric, -12), Note = "Số liệu doanh nghiệp đã nhập"
            };
        }

        private static decimal? Rate(List<SnapshotRow> rows, DashboardFilters filters, string metric, int monthOffset)
        {
            var currentDate = new DateTime(filters.Year, filters.Month, 1);
            var priorDate = currentDate.AddMonths(monthOffset);
            var current = At(rows, currentDate.Year, currentDate.Month).Where(r => Value(r, metric).HasValue)
                .ToDictionary(r => r.EnterpriseId, r => Value(r, metric).Value);
            var prior = At(rows, priorDate.Year, priorDate.Month).Where(r => Value(r, metric).HasValue)
                .ToDictionary(r => r.EnterpriseId, r => Value(r, metric).Value);
            if (current.Count == 0 || prior.Count == 0 || !current.Keys.OrderBy(x => x).SequenceEqual(prior.Keys.OrderBy(x => x)))
                return null;
            var baseline = prior.Values.Sum();
            return baseline == 0 ? (decimal?)null : (current.Values.Sum() - baseline) * 100m / baseline;
        }

        private static List<DashboardTrendPoint> IndicatorTrend(List<SnapshotRow> rows, DashboardFilters filters)
        {
            var start = new DateTime(filters.Year, filters.Month, 1).AddMonths(-11);
            var end = new DateTime(filters.Year, filters.Month, 1);
            return rows.Where(r => r.ForMonth >= start && r.ForMonth <= end)
                .GroupBy(r => r.ForMonth).OrderBy(g => g.Key)
                .Where(g => g.Any(r => r.IndustrialRevenue.HasValue || r.ExportValue.HasValue || r.ImportValue.HasValue))
                .Select(g => new DashboardTrendPoint
                {
                    Month = g.Key.ToString("yyyy-MM"),
                    GtsXcnIndex = SumOrNull(g.Select(r => r.IndustrialRevenue)),
                    ExportIndex = SumOrNull(g.Select(r => r.ExportValue)),
                    ImportIndex = SumOrNull(g.Select(r => r.ImportValue))
                }).ToList();
        }

        private static decimal? SumOrNull(IEnumerable<decimal?> values)
        {
            var known = values.Where(v => v.HasValue).ToList();
            return known.Count == 0 ? (decimal?)null : known.Sum(v => v.Value);
        }

        private static DashboardSubmissionStatus Submission(List<SnapshotRow> rows)
        {
            var submitted = rows.Count(r => r.Submitted);
            var complete = rows.Count(r => r.Submitted && HasRequiredIndicators(r));
            return new DashboardSubmissionStatus
            {
                TotalEnterprises = rows.Count, Submitted = submitted, NotSubmitted = rows.Count - submitted,
                CompleteIndicators = complete,
                Late = rows.Count(r => r.Submitted && r.IsLate),
                MissingIndicators = submitted - complete,
                CompletionPercent = rows.Count == 0 ? 0 : submitted * 100m / rows.Count
            };
        }

        private static bool HasRequiredIndicators(SnapshotRow row)
        {
            return row.IndustrialRevenue.HasValue && row.ExportValue.HasValue && row.ImportValue.HasValue;
        }

        private static List<DashboardSectorContribution> SectorShares(List<SnapshotRow> rows)
        {
            var totals = new[] { SumOrNull(rows.Select(r => r.IndustrialRevenue)) ?? 0,
                SumOrNull(rows.Select(r => r.ExportValue)) ?? 0, SumOrNull(rows.Select(r => r.ImportValue)) ?? 0 };
            if (totals.All(total => total <= 0)) return new List<DashboardSectorContribution>();
            return rows.GroupBy(r => r.EconomicSectorName ?? "Chưa phân khu vực")
                .Select(g => new DashboardSectorContribution
                {
                    Name = g.Key,
                    GtsXcnShare = totals[0] > 0 ? (SumOrNull(g.Select(r => r.IndustrialRevenue)) ?? 0) * 100m / totals[0] : 0,
                    ExportShare = totals[1] > 0 ? (SumOrNull(g.Select(r => r.ExportValue)) ?? 0) * 100m / totals[1] : 0,
                    ImportShare = totals[2] > 0 ? (SumOrNull(g.Select(r => r.ImportValue)) ?? 0) * 100m / totals[2] : 0
                }).Where(x => x.GtsXcnShare > 0 || x.ExportShare > 0 || x.ImportShare > 0)
                .OrderByDescending(x => x.GtsXcnShare).ToList();
        }

        private static IEnumerable<Impact> Comparable(List<SnapshotRow> rows, DashboardFilters filters, string metric)
        {
            var date = new DateTime(filters.Year, filters.Month, 1);
            var previous = date.AddMonths(-1);
            var prior = At(rows, previous.Year, previous.Month).ToDictionary(r => r.EnterpriseId);
            foreach (var current in At(rows, filters.Year, filters.Month))
            {
                SnapshotRow old;
                if (!prior.TryGetValue(current.EnterpriseId, out old)) continue;
                var value = Value(current, metric);
                var baseline = Value(old, metric);
                if (!value.HasValue || !baseline.HasValue) continue;
                yield return new Impact { Current = current, Change = value.Value - baseline.Value,
                    CurrentValue = value.Value, PreviousValue = baseline.Value,
                    HasRate = baseline.Value > 0,
                    Percent = baseline.Value > 0 ? (value.Value - baseline.Value) * 100m / baseline.Value : 0 };
            }
        }

        private static DashboardAlertSummary Alerts(List<Impact> impacts)
        {
            return new DashboardAlertSummary
            {
                DeclineOver10 = impacts.Count(x => x.HasRate && x.Percent < -Warning10),
                DeclineOver20 = impacts.Count(x => x.HasRate && x.Percent < -Warning20),
                DeclineOver30 = impacts.Count(x => x.HasRate && x.Percent < -Warning30),
                UrgentEnterprises = impacts.Count(x => x.HasRate && x.Percent < -Warning30)
            };
        }

        private static List<DashboardIndustryImpact> ImpactsByIndustry(IEnumerable<Impact> impacts)
        {
            var all = impacts.ToList();
            var totalChange = all.Sum(x => x.Change);
            return all.GroupBy(x => x.Current.IndustryName ?? "Chưa phân ngành")
                .Select(g => new DashboardIndustryImpact { Name = g.Key, Contribution = g.Sum(x => x.Change),
                    CurrentValue = g.Sum(x => x.CurrentValue), PreviousValue = g.Sum(x => x.PreviousValue),
                    HasGrowthRate = g.Sum(x => x.PreviousValue) > 0,
                    ChangePercent = g.Sum(x => x.PreviousValue) > 0 ? g.Sum(x => x.Change) * 100m / g.Sum(x => x.PreviousValue) : 0,
                    ContributionShare = totalChange == 0 ? (decimal?)null : g.Sum(x => x.Change) * 100m / totalChange })
                .OrderByDescending(x => Math.Abs(x.Contribution)).Take(8).ToList();
        }

        private static List<DashboardEnterpriseImpact> EnterpriseImpacts(IEnumerable<Impact> impacts, decimal totalChange)
        {
            return impacts.Select(x => new DashboardEnterpriseImpact
            {
                Name = x.Current.BusinessName, Industry = x.Current.IndustryName ?? "Chưa phân ngành",
                ChangePercent = x.Percent, Contribution = x.Change,
                CurrentValue = x.CurrentValue, PreviousValue = x.PreviousValue,
                HasGrowthRate = x.HasRate,
                ContributionShare = totalChange == 0 ? (decimal?)null : x.Change * 100m / totalChange
            }).ToList();
        }

        private static List<DashboardProgressBreakdown> Breakdown(List<SnapshotRow> rows, Func<SnapshotRow, string> key)
        {
            return rows.GroupBy(key).Select(g => new DashboardProgressBreakdown
            {
                Name = g.Key, Submitted = g.Count(r => r.Submitted), NotSubmitted = g.Count(r => !r.Submitted),
                CompletionPercent = g.Count(r => r.Submitted) * 100m / g.Count()
            }).OrderByDescending(x => x.NotSubmitted).Take(10).ToList();
        }

        private static List<string> Reasons(List<SnapshotRow> rows)
        {
            return rows.Where(r => r.Submitted && !String.IsNullOrWhiteSpace(r.Reason))
                .Select(r => r.BusinessName + ": " + r.Reason.Trim()).Take(8).ToList();
        }

        private static List<DashboardOption> Years(List<SnapshotRow> rows, DashboardFilters filters)
        {
            return rows.Where(r => r.Submitted || r.IndustrialRevenue.HasValue || r.ExportValue.HasValue || r.ImportValue.HasValue)
                .Select(r => r.ForMonth.Year).Concat(new[] { filters.Year, DateTime.Today.Year }).Distinct().OrderByDescending(y => y)
                .Select(y => new DashboardOption { Value = y.ToString(), Text = y.ToString() }).ToList();
        }

        private static DashboardFilterOptions BuildFilterOptions(List<SnapshotRow> rows, DashboardFilters filters,
            string action, bool includeMetric)
        {
            return new DashboardFilterOptions
            {
                Filters = filters, Action = action,
                Years = Years(rows, filters),
                Months = Enumerable.Range(1, 12).Select(month => new DashboardOption
                {
                    Value = month.ToString(CultureInfo.InvariantCulture),
                    Text = Vietnamese.DateTimeFormat.GetMonthName(month)
                }).ToList(),
                Areas = Options(rows, r => r.WardId, r => r.WardName, "Tất cả địa bàn"),
                EconomicSectors = Options(rows, r => r.EconomicSectorId, r => r.EconomicSectorName, "Tất cả khu vực"),
                Industries = Options(rows, r => r.IndustryId, r => r.IndustryName, "Tất cả ngành chính"),
                Enterprises = Options(rows, r => r.EnterpriseId, r => r.BusinessName, "Tất cả doanh nghiệp"),
                Metrics = includeMetric ? new List<DashboardOption>
                {
                    new DashboardOption { Value = "industrial", Text = "Doanh thu công nghiệp" },
                    new DashboardOption { Value = "export", Text = "Xuất khẩu" },
                    new DashboardOption { Value = "import", Text = "Nhập khẩu" }
                } : new List<DashboardOption>()
            };
        }

        private static List<DashboardOption> Options(List<SnapshotRow> rows, Func<SnapshotRow, int?> id,
            Func<SnapshotRow, string> label, string allLabel)
        {
            var options = new List<DashboardOption> { new DashboardOption { Value = "all", Text = allLabel } };
            options.AddRange(rows.Where(r => id(r).HasValue).GroupBy(r => id(r).Value)
                .Select(g => new DashboardOption { Value = g.Key.ToString(), Text = label(g.First()) })
                .OrderBy(x => x.Text));
            return options;
        }

        private static string Format(decimal value)
        {
            return value.ToString(decimal.Truncate(value) == value ? "#,##0" : "#,##0.##", Vietnamese);
        }

        private sealed class SnapshotRow
        {
            public DateTime ForMonth; public int EnterpriseId; public string BusinessName;
            public int? WardId; public string WardName; public int? EconomicSectorId;
            public string EconomicSectorName; public int? IndustryId; public string IndustryName;
            public bool Submitted; public bool IsLate; public string Reason;
            public decimal? IndustrialRevenue; public decimal? ExportValue; public decimal? ImportValue;
        }

        private sealed class Impact
        {
            public SnapshotRow Current; public decimal Change; public decimal Percent;
            public decimal CurrentValue; public decimal PreviousValue; public bool HasRate;
        }
    }
}
