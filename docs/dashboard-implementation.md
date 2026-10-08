# Report dashboard implementation — 6 October 2026

## Database and deployment boundary

Audited server: `10.57.30.156`; report DB: `baocaosct.khanhhoa.gov.vn`; category DB: `baocaosct.khanhhoa.gov.vn.cate`. The audit and query validation used TDS 7.0. No live database objects or rows were changed. SQL files are deployment proposals, not an automatic startup migration. Existing import/save/delete procedures remain unchanged.

Deploy the four scripts in `Source/ReportDeptTourismSolution/SqlScripts` manually to the report database before releasing the updated application:

1. `p_Report_Dashboard_IndustrialSnapshot.sql` — ALTER existing procedure; canonical/alternate codes, unit validation, sparse enterprise/month observations, any-type file evidence and reason.
2. `p_Report_Dashboard_OverviewSummary.sql` — ALTER existing procedure; filtered expected populations, ward/sector denominators, province-wide classification and out-of-cohort diagnostics.
3. `p_Report_Dashboard_FilterOptions.sql` — CREATE new read procedure; stable dimensions, latest reporting month, paginated enterprise lookup.
4. `p_Report_Dashboard_ProgressDetails.sql` — CREATE new read procedure; full assigned population with paginated status worklist.

The CREATE scripts are for first deployment; subsequent edits require ALTER. Review the target database and cross-database category references before running them. They do not update reporting data, but deploying them changes procedure definitions. Preserve current procedure definitions for rollback; roll back SQL and application together because this release changes the dashboard result contract. Do not run application with these SQL contracts missing. Apply no migrations, indexes or changes to historic reports. Restart/recycle the application after deployment to discard old dashboard models. Use a Windows .NET Framework/classic ASP.NET build host for the actual solution.

## Display-to-procedure mapping

| Page / elements | Data contract |
| --- | --- |
| Overview: three primary-metric cards, MoM/YoY, metric/import coverage, decline/incomplete links, three-type count/rate chart, selected-type movement/reason table | IndustrialSnapshot (once per report type) + OverviewSummary |
| Overview: province-wide classification panel | OverviewSummary's province-wide fields, independent of dimension-filtered expected counts |
| Analysis: metric selector, current/partial YTD, matched-enterprise MoM/YoY, monthly line with coverage, sector / industry-assignment / enterprise composition and signed movements | IndustrialSnapshot + OverviewSummary; code/label mapping in ReportDashboardBiz |
| Analysis: sector to assigned-industry groups; expand a group's contributing enterprises | Snapshot dimensions; multi-industry group counted once, not allocated to an arbitrary main industry |
| Warnings: cumulative >10/20/30% declines for every type, eligible-pair coverage, null history states, sorted enterprise/reason list | IndustrialSnapshot + OverviewSummary |
| Warnings: short incomplete-data list and full-worklist link | ProgressDetails (incomplete), plus province-wide out-of-cohort diagnostic from OverviewSummary |
| Progress: imported/expected, complete required metrics, incomplete/conflicts, any-type file evidence and recorded late files, count/rate/file trend | IndustrialSnapshot + OverviewSummary |
| Progress: ward/sector completion bars | OverviewSummary grouped denominators + snapshot imported counts |
| Progress: missing/complete/incomplete/conflict/late-file filtered worklist and paging | ProgressDetails |
| All pages: year/ward/sector/industry choices; enterprise Select2 search | FilterOptions; EnterpriseOptions controller action uses the same dashboard View authorization |
| Enterprise inspection modal | Reused p_Report_DataImports_GetDataImport, restricted to requested type in controller; normal Razor encoding |

## Metric rules

| Type | Primary | Secondary | Tertiary | Units |
| --- | --- | --- | --- | --- |
| 1 | 0101 / 1.1 | 06 / 6 | 07 / 7 | Tỷ đồng; Nghìn USD; Nghìn USD |
| 2 | 01 / 1 | 40 / 2 | 02 / 1.1 | Triệu đồng |
| 3 | FOB / 0 | XK_TT / 1 | UT_XK / 2 | USD |

Aliases are scoped by report type. More than one source row for one normalized enterprise/month/metric is a conflict, even when the values match; values are not silently summed or selected by MAX. Missing values remain null; zero counts as supplied. Units accept case/spacing/dot/newline variations of the declared units, never infer monetary magnitude or currency conversion. Invalid units or unrepresentable amounts invalidate only the affected metric. Industry product quantities and tourism codes do not feed these monetary KPIs.

MoM/YoY use actual matched enterprise observations; percentage rates require a positive aggregate baseline. Severity requires a positive enterprise baseline; thresholds are strict and cumulative. YTD sums observed current-year months and displays observed months/selected months. Annual-plan and reported comparison fields are visible in raw detail only. Large amounts retain reported units and can be inspected by enterprise; there is no invented plausibility cutoff.

Dates are matched by `[month start, next month)` regardless of day. Default period is the latest nonfuture active indicator month. Future and malformed period/dimension/status/page inputs are rejected. Filter choices stay independent of result filters and selected out-of-group keys stay visible. Enterprise options and worklists are paginated; input SQL is parameterized. Each enterprise's active type assignment defines the expected population; type populations overlap. Historical denominators use current assignments and are labelled accordingly.

Files have no TypeReport: `FileSubmittedAnyType`, reasons and recorded file lateness are explicitly cross-type evidence. Imported status and complete metrics are separate. Missing imports are not automatically labelled overdue. Province-wide orphan/out-of-cohort diagnostics stay distinct from filtered counts. Dashboard cache entries depend on imports, enterprises, configuration, categories and messages; no global cache implementation was changed.

## Verified source baseline

Active indicator rows: 442, only October 2–6, 2026. No earlier active metric history. September has 8 report files and October 61; file records are not economic observations.

| Type | Expected | Imported | Missing | Complete | Incomplete | Any-type files in assigned cohort |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | 18,854 | 60 | 18,794 | 22 | 38 | 60 |
| 2 | 2 | 1 | 1 | 0 | 1 | 1 |
| 3 | 33 | 2 | 31 | 2 | 0 | 2 |

Enterprise 496 has source indicators but no enterprise master record; one out-of-cohort diagnostic. Province: 18,854 active enterprise records, 250 classified, 18,602 missing business row, 18,604 missing industry. No canonical metric collisions found. Wholesale/retail and retail subset have a genuine submitted zero; trading repair value is missing. Large manufacturing amounts require business reconciliation. With this database, comparison/warning cards must show insufficient history, not fabricated zero growth.

## Validation and manual acceptance

Proposed SELECT-only query bodies were executed against the live source tables, without CREATE, ALTER, INSERT, UPDATE, DELETE, temporary tables, or executing write procedures. SQL shape/results, dimensions, totals, null metrics, population and pagination were checked. Native ReportModule /t:Compile is blocked on this Mac by missing Microsoft.WebApplication.targets. Isolated changed C# plus all seven generated Razor views compile against .NET Framework 4.5.2 references with a minimal unchanged-framework surface; this does not replace the native solution build or deployed browser QA. JavaScript syntax and provider XML/copy consistency are checked. No automated test cases were added.

After manual deployment, run `Verify_Dashboard_ReadOnly.sql`. Confirm September has no economic values, October counts match the table, aliases restore metric coverage, same-month mid-month dates match, enterprise/ward/sector/industry filters narrow correctly, empty/out-of-group filters remain visible, zero does not become missing, status counts/page totals agree, cached models refresh after an authorized source edit, and small-screen/keyboard/modal/Select2/chart behavior works. Use actual past observations before evaluating threshold boundaries or year-boundary comparisons. No browser runtime is available on the Mac for classic ASP.NET; deployed rendering remains to be checked.
