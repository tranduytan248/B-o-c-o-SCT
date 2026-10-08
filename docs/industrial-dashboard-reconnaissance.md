# Industrial dashboard: database reconnaissance and design (2026-09-28)

> Historical audit of the previous database. For the current connection, SQL contracts, and implementation, see [dashboard-implementation.md](dashboard-implementation.md).

## A. Database reconnaissance summary

The configured `ReportTourismDB` connects to `baocao.sct.cenit.vn` on `APPDEMO\MSSQL2012`; `BaseApp` connects to `baocao.sct.cenit.vn.cate`. Both use schema `dbo`. No relevant synonyms or summary views exist in the report database. Its only view, `Get_RandValue`, and functions `fnSplit` and `fGenerate_RandomNumber` are utilities, not analytical sources.

The report database has six user tables: `Report_DataImports`, `DataReport_Imports`, `Report_ReportFiles`, `Report_ExtendInfos`, `ReportTargetConfig`, and `ReportDynamicGroupRule`. `Report_DataImports` has 15 active indicator rows for September 2026, covering two enterprises; `Report_ReportFiles` has eight active September submissions. There are no earlier report months. The category database has 18,783 active `Cate_Enterprises`, but only 64 active enterprises have a `Cate_BusinessEnterprise` classification. These 64 form the **current monitored/classified reporting cohort**, not the total enterprise population of Khánh Hòa. They are the denominator for this dashboard's reporting progress. Of the eight submitted files, five belong to this cohort; only one of those five has all three required dashboard indicators. The discrepancy must be shown rather than silently counted as complete indicator data.

The currently populated codes are `01` total revenue, `0101` industrial revenue, `03A` workers, `03B` mean income, `05` budget payment, `06` export, `07` import, and one product code `3512200`. `0101` is **industrial revenue**, not GTSXCN. No GTSXCN code or definition was found in `ReportTargetConfig` or active data. Export/import use `1.000 USD`; the two enterprises' `06` and `07` values are zero or one. No 12-month trend or YoY comparison can presently be inferred. The `ComparedSamePeriodLastYear` field is null in all 15 rows. Existing stored prior-period and accumulated fields are reported inputs, not independently validated history.

## B. Relevant stored procedures

All named procedures below exist in the live report database; their SQL definitions, not just XML names, were inspected. None of the five legacy dashboard procedures was called by `ReportDashboardBiz` before this change; that class previously fabricated every value. Source references to the mapped report procedures are indirect XML/report configuration rather than typed C# calls.

- **Currently used by direct source calls:** `p_Report_DataImports_Get`, `GetDataImport`, `GetForUserOnMonth`, `GetViaEnterpriseOnMonth`, `ImportDatas`, `CheckDataImport`, and `Delete` through `ReportDataImportBiz`; other tourism report procedures may be invoked through the report configuration and generic `ReportCache.GetDataReport`.
- **Exists but unused by the mock dashboard:** all five `p_Report_Dashboard_Statistic*` procedures, the three `usp_Report_KTXH*` procedures, and the industrially relevant but unreferenced `p_Report_Reports_04_BaoCaoHoatDongDoanhNghiep` mapping.
- **Mapped but missing/unverified:** none among the requested `Report_ThongKe_*`, `Report_Dashboard_*`, `Report_DataImports_*`, and `Report_Reports_*` mappings. The duplicate `Report_DataImports_Get` XML entry exists in both copies and was left unchanged.

| Procedure(s) | Inputs, source and behavior | Dashboard decision |
|---|---|---|
| `p_Report_Dashboard_StatisticEnterprise` | `@OnMonth datetime`; counts distinct late/submitted IDs from `Report_DataImports` and all active category enterprises. Does not filter deleted indicator rows or industrial cohort. | Do not reuse for industrial completion. |
| `p_Report_Dashboard_StatisticTypeBusiness` | `@OnMonth datetime`; hard-coded five tourism business types and active enterprise counts. | Do not reuse. |
| `p_Report_Dashboard_StatisticVisitor`, `...StatisticMapVisitor`, `...StatisticIncome` | Month/type inputs; aggregate tourism indicator codes, countries, visitors or tourism revenue. `StatisticIncome` joins calendar months and prior year. | Do not repurpose. |
| `p_Report_ThongKe_BaoCaoTrongThang` | `@Thang nvarchar(6)` (`MMyyyy`); counts distinct reporting IDs using a formatted date predicate; no deleted-row filter. | Useful historical reference, not directly reusable for filtered industrial progress. |
| `p_Report_ThongKe_BaoCaoTre` | Same input and distinct-count pattern, adds `IsLate=1`; no deleted-row filter. | Same limitation. |
| `p_Report_ThongKe_BaoCaoChuaNop` | Same input; requires `EnterpriseId IS NULL` on a nonnullable column, so always returns zero. | Cannot reuse. |
| `p_Report_ThongKe_BaoCaoDoanhNghiep` | `@ForMonth nvarchar(6)`; hard-coded tourism `TypeBusiness` counts and report stats. | Cannot reuse. |
| `p_Report_Reports_03_DoanhNghiepChuaGuiBaoCao` | `@ForMonth datetime`; anti-joins active category enterprises to nondeleted `Report_DataImports`; returns enterprise details but no industrial cohort/filter. | Reuse for legacy detail report only. |
| `p_Report_Reports_04_BaoCaoHoatDongDoanhNghiep` | `@ForMonth datetime`, optional enterprise; groups current and prior-year rows by `TypeReport,Code,Targets,Unit`, sums reported monthly/prior/cumulative values, calculates prior-year ratios with null zero baseline. It returns separate rows for different `TypeReport`, even for the same code. | Reuse for existing report; not a filtered dashboard total. |
| `p_Report_DataImports_Get`, `...GetDataImport`, `...GetForUserOnMonth`, `...GetViaEnterpriseOnMonth` | Read indicator rows or per-enterprise submission, joining category enterprise/user and applying caller permissions where relevant. | Keep for existing import screens and detail drill-down. |
| `p_Report_DataImports_ImportDatas`, `...CheckDataImport`, `...Delete`, `...GenerateSaleOfTourism` | Import/check/delete workflow or tourism derived rows; import writes indicator rows and `Report_ReportFiles`, with `IsLate` based on category `Sys_Configs` deadline keys. | Do not change. |
| `p_Report_Reports_01_UocKetQuaHoatDongKinhDoanh`, `...02_ThongKeQuocTichKhachDuLich`, tourism `p_Report_GenerateSaleOfTourism_*` and `p_Report_ThongKe_*` revenue/visitor/occupancy procedures | Tourism code lists, tourism-specific joins and aggregations. | Not suitable for industrial analytics. |
| `usp_Report_KTXH`, `usp_Report_KTXH_TheoEnterprise`, `usp_Report_KTXH_Tong` | `@ForMonth`, `@TypeReport`, optional/required enterprise. The shared procedure aggregates generic `Code` rows from `Report_DataImports`, joins `ReportTargetConfig` and `ReportDynamicGroupRule`, and computes report-template columns across selected/prior periods; the other two are wrappers. It has no ward/sector/industry or classified-cohort filter and is designed for a report sheet rather than an enterprise/month dashboard result. | Useful reference for code semantics; cannot directly supply the filtered dashboard without changing existing report behavior. |
| `p_Report_ThongKe_BaoCaoTongHop` | Date input and legacy summary output, without a filtered industrial cohort. | No direct reuse. |

Existing output contracts and callers were checked before rejecting modifications. Changing any of the tourism procedures would alter existing reports or preserve their incorrect denominator, so no existing procedure should be modified.

## C. Views and functions

| Object | Type / inputs | Source and output | Code use / dashboard use |
|---|---|---|---|
| `dbo.Get_RandValue` | View | Random value helper | No dashboard use. |
| `dbo.fnSplit` | Table-valued function, delimited string | Split values (used by legacy filter procedures) | Industry IDs are comma-delimited, but a dashboard query should match tokens explicitly. |
| `dbo.fGenerate_RandomNumber` | Scalar function | Random number | No dashboard use. |

The category database has the master tables `Cate_Enterprises`, `Cate_BusinessEnterprise`, `Cate_EconomicSector`, `Cate_BusinessIndustry`, `Cate_EnterpriseType`, and `Cate_Wards`. Existing category Get procedures serve individual pickers but provide no reporting aggregation. No relevant summary view was found.

Its only functions are `fnSplit`, `fc_ConvertString`, `fChuyenCoDauThanhKhongDau`, and `fnConvertToUnsignCharacter`; they are string utilities. It has no user view or relevant synonym.

## D. Generic reporting data model

Template/configuration (`ReportTargetConfig.Code`, `Targets`, `Unit`, and dynamic product rows from category product tables) → import form values → `ReportDataImportBiz.Import` → `p_Report_DataImports_ImportDatas` → `Report_DataImports` indicator rows. The import also writes one `Report_ReportFiles` row per enterprise/month with `Reason`, `IsLate`, `CreatedDate`; later edits update the file row. `TypeReport` distinguishes templates but the same core code can appear under multiple types. The report output procedure groups generic code rows, not dedicated physical export/import columns.

`Report_DataImports.EnterpriseId` (`int`) joins `Cate_Enterprises.EnterpriseId` (`int`), then `Cate_BusinessEnterprise.EnterpriseId` (`bigint`). The latter carries `EconomicSectorId`, `EnterpriseTypeId`, and comma-delimited `IndustryIds`; `Cate_Enterprises.WardId` provides the area key. `Cate_Enterprises` also supplies `TaxCode`, `BusinessName`, status flags, and place names. `Report_DataImports.ForMonth` (`date`), `Code`, `PerformPreviousPeriod`, `PerformInPeriod`, `AccumulatedBeginingOfYear`, and `ComparedSamePeriodLastYear` supply generic metrics. `Report_ReportFiles` supplies submission status/date, lateness, and reason. No `IsLocked` exists in either active report table. Historical period comparisons must use actual prior-month/prior-year rows when present; a zero or missing baseline yields no rate.

## E. Dashboard-to-database capability matrix

| Widget / business question | Metric and dimensions | Existing Cache/Biz/SP/View | Direct reuse / new query |
|---|---|---|---|
| Export, import now | Sum `PerformInPeriod` for `Code 06/07`, month, ward, sector, industry, enterprise | `ReportCache`/`ReportBiz`/`p_Report_Reports_04_BaoCaoHoatDongDoanhNghiep` (unfiltered) | New filtered aggregate needed. |
| GTSXCN now | No validated code; `0101` means industrial revenue | None | Unsupported. Replace label with industrial revenue if shown. |
| MoM, YoY, YTD, 12-month trend | Actual monthly values by code and period | Existing report 04 only gives raw prior-year result for one month | New filtered history query; blank until history exists. |
| Sector, industry, enterprise contribution; reasons | Current and historical enterprise code amounts; sector/industry keys; `Report_ReportFiles.Reason` | Category Get procedures and import detail methods | New filtered query; positive/negative contribution only with comparable baseline. |
| Declines >10/20/30%, trend, urgent enterprises | Enterprise monthly value versus actual prior month; material amount | No alert rule table or matching SP | One central threshold in Biz; no alerts without a valid baseline. |
| Expected/submitted/not submitted/late/completion | Active classified enterprise cohort; `Report_ReportFiles` | Legacy `p_Report_Dashboard_StatisticEnterprise` and `p_Report_Reports_03...` have wrong cohort/source | New filtered progress query; details from existing import screen. |
| Submission by ward/sector/industry and trend | Same cohort/status by dimensions and month | Category master and report files | New query; no view. |

## F. SP reuse / modification / creation plan

Reuse the existing import and report procedures for their current screens. Modify none. Add **one** `p_Report_Dashboard_IndustrialSnapshot` to return a filtered industrial enterprise/month snapshot for the selected year and preceding comparison year. It will join the active classified cohort to category dimensions, report files, and coded indicator aggregates (`0101`, `06`, `07`). Inputs: selected year/month and nullable `WardId`, `EconomicSectorId`, `IndustryId`, `EnterpriseId` using real `int` keys. Output: one enterprise/month row with status, reason, dimension names, and monthly indicator values. The expected maximum unfiltered result is 64 enterprises × 24 months = 1,536 rows at current cohort size. It uses database-side code aggregation and one application round trip per cached page. No report values are written. Source tables are `Cate_Enterprises`, `Cate_BusinessEnterprise`, `Cate_EconomicSector`, `Cate_BusinessIndustry`, `Report_ReportFiles`, and `Report_DataImports`; no date function is applied to indexed `ForMonth` columns. Test all-null filters, single enterprise/ward/industry, empty period, and year boundary before deployment. Add the provider mapping in both ReportModule and WebApp copies.

## G. Dashboard redesign proposal

### Executive overview

Use four cards: industrial revenue (`0101`, explicitly named), export (`06`), import (`07`), and classified-enterprise submission completion. Each value shows its source unit and MoM/YoY only when actual comparator rows exist. Show a 12-month line with gaps for missing data, submission donut/progress, and a sector or enterprise contribution chart only when comparable data exists. Filters use actual year/month/ward/sector/industry/enterprise keys. No large table.

### Drill-down

Province → economic sector → industry → enterprise → submitted reason. Let the user choose industrial revenue/export/import. Use composition bars for current amounts, diverging bars for absolute changes, and concise reasons. Because `IndustryIds` can contain multiple IDs, avoid allocating the same enterprise amount to multiple industries until an attribution rule is agreed; the initial dashboard uses the first/main assigned industry and labels that choice.

### Warning dashboard

Three severity counts at >10%, >20%, >30% decline, a history line, industry distribution, and a short ranked enterprise list. A valid prior-month value greater than zero is required. Rank by absolute negative change first, then decline percentage; a missing baseline does not become an alert. Thresholds live in one Biz constant.

### Reporting progress

Show six separate counts: expected active enterprises in the current monitored/classified cohort, submitted report files, submitted files with all required dashboard indicator codes (`0101`, `06`, `07`), submitted files missing at least one required code, not submitted, and late. A numeric zero for an indicator counts as present; null or absent does not. Submission completion percentage is submitted files divided by expected cohort, independent of indicator completeness. Add monthly trend and completion by ward and industry. Enterprise lists stay behind detail actions. Lateness is a subset of submitted files, and overlaps the complete/missing groups.

### Portfolio decision after capability review

| Page | Management question and available evidence | Decision |
|---|---|
| Executive Overview | Current industrial revenue, export, import, file completion, short coverage and alert summary | Keep as landing page, with missing comparisons labelled unavailable. |
| Indicator Analysis | Compare and decompose one selected code: industrial revenue `0101`, export `06`, or import `07` | Keep one page with a metric selector and shared dimensions. Cross-unit sums are prohibited. |
| Enterprise Warnings | Which enterprises have a validated industrial-revenue decline over 10/20/30 percent? | Keep, but show insufficient-history state until actual prior-month baselines exist. |
| Reporting Progress | Who was expected to file, filed, filed with/without indicators, did not file, or filed late? | Keep with distinct file and indicator states. |
| Data Quality / Coverage | How many filed enterprises supplied each required code, and how many months support comparisons? | Keep a contextual detail page linked from Reporting Progress; omit it from the main dashboard tabs. |

Ward, economic sector, main industry, and enterprise are reusable filters/drill-down dimensions across these pages. The discovered database does not justify separate dashboards for each dimension. The main industry is the first listed industry because no allocation rule exists for multi-industry enterprises.

### Scale boundary for the tactical snapshot

`p_Report_Dashboard_IndustrialSnapshot` is the **current tactical data source**, not a permanent aggregation architecture. At today's 64-enterprise cohort it returns up to 1,536 enterprise-month rows for a two-year window and permits consistent filters and comparison rules in Biz. Track cohort size, returned row count, query duration, payload size, and dashboard response time as reporting expands. If the cohort grows materially or the snapshot becomes a latency/payload bottleneck, move repeated/heavy totals, breakdowns, trends, and warning calculations into DB-side summary procedures with the same filter semantics, rather than returning the full enterprise-month snapshot to every page. Preserve the source indicator definitions and file-versus-indicator distinctions during that move.

### Coverage and unavailable states

The available live database has only September 2026 indicator data. Missing prior-month or prior-year observations produce an explicit unavailable/insufficient-history state for MoM, YoY, movements, warnings, and trend conclusions. Missing values are never converted into zero. A real submitted zero remains a valid current-period value. The Data Quality page shows current per-code coverage and the count of observed months within the latest 12-month window; this is coverage evidence, not a guarantee of trend validity for every enterprise.

### Current mock widgets

| Current widget | Decision |
|---|---|
| GTSXCN KPI | CHANGE to industrial revenue (`0101`); never label it GTSXCN. |
| Export/import KPIs | KEEP, replace fabricated values with code `06`/`07`, correct unit and unavailable comparison state. |
| Completion KPI / submission progress | CHANGE to classified cohort and report-file source. |
| 12-month three-series trend | CHANGE to real monthly data with gaps; show no-history state for current database. |
| Sector shares | CHANGE from hard-coded percentages to actual amounts; show no-history state for contribution. |
| Industry contribution and enterprise movement | MOVE to analysis; require real comparison data. |
| Warning summary/trend/rankings | CHANGE to rule-backed analytics; empty state until valid baselines exist. |
| Reported reasons | MOVE to enterprise drill-down; use `Report_ReportFiles.Reason` only. |
| Mock year/area/sector/industry/enterprise options | CHANGE to actual database keys and available years. |

Chart contracts: monthly `0101` values → line chart (time sequence); current values by sector → stacked share bar (composition); comparable absolute change → diverging horizontal bars (signed impact); file submitted/not submitted → progress/donut (two-part share); severity history → line (time sequence). Charts must state their metric, dimension, comparison, and data source in the UI or its accessible label.

## Implementation and validation

1. **Existing DB objects reused:** `Report_DataImports`, `Report_ReportFiles`, `Cate_Enterprises`, `Cate_BusinessEnterprise`, `Cate_EconomicSector`, `Cate_BusinessIndustry`, `Cate_Wards`, and the indicator code definitions in `ReportTargetConfig`. Existing import/report procedures remain in place for their original screens.
2. **Existing DB objects modified:** none.
3. **New DB object:** `dbo.p_Report_Dashboard_IndustrialSnapshot`, deployed to `baocao.sct.cenit.vn`; its source is in `SqlScripts`. It returns read-only enterprise/month results for the classified cohort, with source indicator values and submission status. It does not write report data.
4. **Cache/Biz:** `ReportDashboardCache` retains its existing API and cache keys. `ReportDashboardBiz` now calls `ReportBiz.GetDataReport` (and therefore the existing `ProcedureProvider`) for each cached page, maps real results, returns current/prior values, absolute changes and shares of net change, and centralizes the warning thresholds. Zero-baseline pairs can contribute an absolute change but have no growth rate or severity. The controller defaults to the current month instead of a hard-coded mock month.
5. **XML:** `Report_Dashboard_IndustrialSnapshot` is mapped once in each of the ReportModule and WebApp XML files; both parse successfully.
6. **Mock widgets removed/changed:** fabricated KPI amounts, rates, trend points, sector shares, enterprise names, warnings, reasons, progress figures, and filter options are gone. GTSXCN was replaced by the actual `0101` industrial revenue indicator. Unsupported comparisons render an explicit unavailable state.
7. **Final information architecture:** Four main tabs: Overview, Indicator Analysis, Enterprise Warnings, and Reporting Progress. Data Quality / Coverage remains a contextual detail page reached from Reporting Progress. The overview retains four cards, industrial revenue trend, sector composition, short enterprise movement, submission progress ring, and warning summary. Analysis has a three-indicator selector. The Progress page separates six states, and Data Quality displays per-indicator coverage and history. Every page shares ward, sector, main-industry, and enterprise filters. The overview trend uses only industrial revenue because trade and revenue have different units.
8. **Procedure tests:** The deployed procedure returned 16 typed columns. For September 2026 it returned 1,536 rows (64 enterprises × 24 months), with exactly 64 unique enterprise IDs in the selected month, 5 submitted files, 3 late, `15000` industrial revenue (Tỷ đồng), and zero export/import (1.000 USD) for the classified cohort. January and December boundaries returned 64 distinct selected-month enterprises; enterprise `511` returned 1, ward `2088` returned 3, industry `2845` returned 63, and April 2024 returned no indicator/submission data. Calls took 24–144 ms in these spot checks. Nullable decimal indicators preserve missing values; the Biz rate logic suppresses null, missing, and zero baselines. No index was added.
9. **Unsupported data:** No validated GTSXCN indicator, prior-month or prior-year observations, complete YTD sequence, province-wide indicator coverage, or historical alert trend currently exists. Four of five classified submitted files lack the three dashboard indicators. Industrial classification covers 64 active enterprises out of 18,783 active enterprise records. Industry allocation uses the first listed industry; multi-industry attribution remains unresolved. Historical completion uses today's active classified cohort, as the registry has no effective-date history.
10. **Runtime:** The Models, Biz, and ReportModule C# sources compiled with native .NET Framework MSBuild. A full solution build hits pre-existing post-build `Robocopy` paths requiring `SolutionDir`; the ReportModule `/t:Compile` target succeeded with two pre-existing COM type-library warnings. Razor rendering, browser behavior, and screenshots were skipped at the user's request.
