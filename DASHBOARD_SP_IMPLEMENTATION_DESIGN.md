# Industrial dashboard stored procedure implementation design

**Status:** Design only. No procedure, table, index, application code, or dashboard UI is changed by this document.

## 1. Evidence and independent validation

Primary database/context evidence is [the Antigravity architecture audit](DASHBOARD_DATA_ARCHITECTURE_AUDIT.md). I checked its proposed contracts against the repository's [current snapshot definition](Source/ReportDeptTourismSolution/SqlScripts/p_Report_Dashboard_IndustrialSnapshot.sql), `ReportDashboardBiz`, dashboard models/cache, both XML procedure maps, and the audit's table inventory. Prior read-only DataGrip inspection of the connected database additionally confirmed the relevant column lists and representative rows. The design does **not** treat the audit's business conclusions as approved rules.

| Evidence | Verified consequence for the contract |
|---|---|
| `Report_DataImports` contains `EnterpriseId`, `ForMonth`, `TypeReport`, `Code`, `PerformInPeriod`, `IsDeleted`; `DataReport_Imports` is empty in the inspected database | Filter imported indicators by one explicit `TypeReport`; use `Report_DataImports`, not the legacy/staging table. |
| `Report_ReportFiles` contains `EnterpriseId`, `ForMonth`, `IsLate`, `Reason`, `CreatedDate`, `IsDeleted`, but **no `TypeReport` or stable file ID in the inspected schema** | A file proves only an enterprise/month file exists. It cannot prove submission for the requested report type. The audit's suggested type-filtered latest-file join is impossible with this schema. |
| `ReportTargetConfig` has `Code`, `Targets`, `Unit`, presentation/order fields, but **no `TypeReport`, mandatory flag, or validity rule**; `ReportDynamicGroupRule` has grouping patterns | The catalog supports labels/units, not report-type membership, completeness, or validity. Do not derive mandatory indicators from it. |
| `Cate_BusinessEnterprise.IndustryIds` is a CSV list; `Cate_BusinessEnterpriseProduct.IsMainProduct` concerns products | The first industry token is not an authoritative primary industry. Industry membership filtering is possible; primary-industry allocation is unresolved. |
| `Cate_Enterprises` has current `IsActive`/`IsDeleted`; `Cate_BusinessEnterprise` has current `EnterpriseStatusId` | Current classification is not a dated obligation ledger. A current active cohort is only a candidate display population, not a monthly reporting denominator. |
| Observed `Report_DataImports.ForMonth` includes first-of-month and day-28 dates, and dates after the inspection date | Exact-date equality drops same-calendar-month rows. Normalize by calendar month for lookup, but detect duplicate business keys; do not decide whether a day-28 record supersedes day-1 data. Do not infer validity from future-dated rows. |
| Current snapshot groups `SUM` by enterprise and exact `ForMonth` without `TypeReport`, selects first CSV industry token, and collapses missing `IsLate` to false | These produce mixed-type values, false primary industry, and false certainty about lateness. |
| `ReportDashboardBiz` uses one snapshot for Index, Analysis, Warnings, Progress, Quality; MoM/YoY/YTD, warnings and breakdowns are computed in C# | One factual snapshot remains sufficient. No widget-specific aggregate SP is justified. Biz must stop converting unknown obligations/completeness into zero or false. |

The audit's numeric cohort and late-file counts are not contract constants. Its proposed status exclusion, `ReportTargetConfig.TypeReport`, and typed file submission are unverified or contradicted by the inspected schema. Historical coverage should be measured from the data during implementation, not assumed from audit prose.

## 2. Final architecture and decisions

```text
Cate_Enterprises + Cate_BusinessEnterprise + category names
                      │ current candidate population
Report_DataImports ───┼──> p_Report_Dashboard_IndustrialSnapshot
                      │     one enterprise × calendar month, for one TypeReport
Report_ReportFiles ────┘     raw facts + explicit unknowns
                                      │
                        ReportDashboardBiz / dashboard cache
                        Index · Analysis · Warnings · Progress · Quality
```

| Decision | Object | Reason |
|---|---|---|
| **MODIFY** | `dbo.p_Report_Dashboard_IndustrialSnapshot` | Preserve the existing Enterprise × Month result grain and 24-calendar-month window; correct type, period, duplicate, and unknown-state semantics. |
| **KEEP** | `p_Report_DataImports_ImportDatas` and other import/write SPs | They own ingestion. Their validation rules are not established by this design. |
| **KEEP** | `usp_Report_KTXH` and report-printing SPs | They serve other reports and do not expose the dashboard grain. |
| **CREATE: none now** | — | Current pages already consume a cached snapshot. A paged enterprise-detail SP is deferred until real server paging or scale requires it; no unused contract is introduced. |
| **REMOVE from industrial call graph** | Legacy `p_Report_Dashboard_Statistic*` and `p_Report_ThongKe_*` | Their tourism/general-report semantics cannot define this dashboard. This is a call-site decision, **not** a request to drop database objects used elsewhere. |

The snapshot is a *fact feed*, not an official submission or compliance report. `@TypeReport` is required because the imported indicator table has that dimension. No default type and no all-types aggregation are safe. Which type the industrial dashboard should select remains an explicit product/business configuration decision; the observed numeric values `0` and `1` are data, not an approval to hardcode either.

### Unresolved decisions with explicit boundaries

1. **Reporting obligation:** identify the source and effective dates of enterprise × month × report-type obligation, including entrants, exits, suspensions, and exemptions. Until then `ExpectedToReport` is `NULL`, and due/not-due and official submission percentages cannot be calculated. The candidate cohort count must not be labeled a denominator.
2. **Primary industry:** identify the authoritative primary-industry field/rule and its effective date. Until then `PrimaryIndustryId`/`PrimaryIndustryName` are `NULL`; industry filter means *membership in any listed industry*, while exclusive industry shares, contributions, and industry progress are unavailable.
3. **Mandatory indicators / valid report:** approve required codes per type and period, whether zero is valid, whether optional export/import codes apply, treatment of duplicate/revised imports, and whether a file is required. Until then `CompleteIndicators` and `ValidReport` are `NULL`; presence of `0101`, `06`, and `07` is diagnostic coverage only.

## 3. Exact contract: `dbo.p_Report_Dashboard_IndustrialSnapshot`

| # | Contract item | Decision |
|---|---|---|
| 1 | **Name** | `dbo.p_Report_Dashboard_IndustrialSnapshot` (modify in place when implementation is authorized). |
| 2 | **Purpose** | Return auditable raw enterprise/month facts for the new industrial dashboards, with unknown business classifications represented explicitly. |
| 3 | **Grain** | Exactly one row per `(EnterpriseId, ForMonth)` for the requested *single* `TypeReport`; `ForMonth` is always the first day of a calendar month. `TypeReport` is constant across the result and is included as a provenance column. No row multiplication by industry, file, or indicator. |
| 4 | **Parameters** | `@ForMonth date` required; `@WardId int = NULL`; `@EconomicSectorId int = NULL`; `@IndustryId int = NULL` (any listed industry); `@EnterpriseId bigint = NULL` to match the inspected enterprise key; `@TypeReport int` required, no default. The deployment must verify exact DB column types and the generic `GetDataReport` parameter ordering. `0` is not rejected as an ID merely because it is zero. |
| 5 | **Source tables** | Runtime data: `dbo.Report_DataImports`, `dbo.Report_ReportFiles`; current candidate directory: `[baocao.sct.cenit.vn.cate].dbo.Cate_Enterprises`, `Cate_BusinessEnterprise`; optional category names: `Cate_Wards`, `Cate_EconomicSector`. `Cate_BusinessIndustry` may supply a filter label outside this fact SP, but does not assign primary industry. `ReportTargetConfig` is an implementation-time metadata cross-check for code labels/units, not a runtime join or completeness source. No `DataReport_Imports` or tourism SP. |
| 6 | **Join rules** | Inner join enterprise directory to business classification on `EnterpriseId`; cross join 24 generated months; left join pre-aggregated indicator and file facts on `(EnterpriseId, normalized month)`. Apply the `TypeReport` predicate *inside* the indicator read before aggregation. Aggregate file existence separately; never join each raw file to each raw indicator. Do not join `Report_ReportFiles` by type, since it has no such column. |
| 7 | **Cohort/population rules** | Candidate cohort keeps existing `e.IsActive = 1 AND e.IsDeleted = 0` plus a `Cate_BusinessEnterprise` row, with optional ward, sector, enterprise, and any-listed-industry filters. Do not additionally exclude an `EnterpriseStatusId` without an approved status policy. Return current status/directory fields as provenance. This is *current* scope repeated over historical months, not a historical obligation cohort. No implicit `IsApproved` rule. |
| 8 | **Period rules** | Keep the current 24 calendar months: Jan 1 of `YEAR(@ForMonth)-1` through Dec 1 of `YEAR(@ForMonth)`. Normalize `@ForMonth` to its calendar month; `ForMonth` output is month-start. Filter source rows with `>= @Start AND < @End`, then bucket with `DATEFROMPARTS(YEAR(source.ForMonth), MONTH(source.ForMonth), 1)`. Do not use exact-date equality or assume `ForMonthInt` encoding. Rows after today's date stay raw data and should be flagged/handled by Biz display policy, not silently rewritten. |
| 9 | **Indicator selection rules** | For this dashboard's **defined metrics**, read active rows for codes `0101` (industrial revenue, `Tỷ đồng`), `06` (export, `1.000 USD`), `07` (import, `1.000 USD`) for the requested type. A value is reported only if exactly one nondeleted row exists for that `(EnterpriseId, month, TypeReport, Code)` **and** its `PerformInPeriod` is non-NULL. If more than one row exists, return `NULL` for that metric and a conflict flag/count; do not sum or pick a revision without a business rule. This fixed metric mapping is not a mandatory-indicator list. Never rename revenue as production value (`GTSXCN`). |
| 10 | **TypeReport handling** | Required scalar filter for `Report_DataImports`; echo as output. No `NULL = all types` behavior. File existence and late/reason fields are explicitly `AnyType` observations. Typed `Submitted` remains unknown (`NULL`) until a reliable type-to-file linkage is available. If a later approved rule maps a file to a type, change this contract deliberately rather than inferring it from coincident indicator rows. |
| 11 | **NULL/zero semantics** | `IndustrialRevenue`, `ExportValue`, `ImportValue`: `NULL` means no single observed non-NULL value (missing, null source, or duplicate conflict); numeric `0` is observed zero. `DataImported = 1` means at least one active imported row for the selected type/month, even if all its values are NULL; `0` means none. `FileSubmittedAnyType = 1/0` reflects active file existence. `Submitted`, `ExpectedToReport`, `CompleteIndicators`, `ValidReport` are nullable `bit` and `NULL` means unknown, never false. `LatestFileIsLateAnyType` is NULL with no file or an ambiguous latest timestamp; do not coerce it to 0. |
| 12 | **Output columns** | See the typed list immediately below; output names form the proposed versioned application contract. |
| 13 | **Required indexes** | No new index is *proven required* by the current small data volume. Verify existing PK/index definitions and actual plan first. Candidate filtered covering indexes if scans become material: `Report_DataImports (TypeReport, ForMonth, EnterpriseId, Code) INCLUDE (PerformInPeriod) WHERE IsDeleted = 0`; `Report_ReportFiles (ForMonth, EnterpriseId, CreatedDate DESC) INCLUDE (IsLate, Reason) WHERE IsDeleted = 0`. Confirm filtered-index compatibility, column types, write cost, and absence of an equivalent index before creating. Category tables need verified keys on `EnterpriseId`; the CSV industry filter cannot use a normal `IndustryId` index. |
| 14 | **Dashboard consumers** | `ReportDashboardBiz.GetRows` feeds Index, Analysis, Warnings, Progress, Quality via `ReportDashboardCache`. Biz retains MoM, YoY, YTD, trends, shares, and warning thresholds. It must carry `TypeReport` in filter/cache keys and honor nullable statuses; no additional dashboard SP consumer is justified. |
| 15 | **Compatibility impact** | Breaking application contract: add one required argument; change `EnterpriseId` handling to `long`; replace legacy `Submitted`/`IsLate` booleans with nullable typed/untyped facts; industry fields become unresolved; add duplicate and coverage diagnostics. Both XML mappings retain the same procedure name, but the Biz data mapping, dashboard models, cache key, and view wording must be updated in the same release. Do not deploy the SQL alone against the old `Convert.ToBoolean(row["Submitted"])` consumer. |
| 16 | **Risks / unresolved rules** | Obligation denominator, type-specific file submission, primary industry, mandatory codes, validity, duplicate/revision precedence, status-effective history, `IsLate` semantics, future-dated rows, and period coverage remain open. Each can change interpretation without changing raw imported values. |

### Proposed output schema

| Column(s) | SQL type / meaning |
|---|---|
| `ForMonth` | `date NOT NULL`, month-start. |
| `TypeReport` | `int NOT NULL`, requested type. |
| `EnterpriseId` | `bigint NOT NULL`; directory fields `BusinessName`, `TaxCode` nullable strings as source permits. |
| `WardId`, `WardName`, `EconomicSectorId`, `EconomicSectorName`, `EnterpriseStatusId`, `IndustryIds` | Current category attributes; IDs and labels nullable. `IndustryIds` remains raw membership text. |
| `PrimaryIndustryId`, `PrimaryIndustryName` | `int NULL` and nullable text, both `NULL` until authoritative rule exists. The old `IndustryId`/`IndustryName` output must not silently keep first-token semantics. |
| `ExpectedToReport` | `bit NULL`, currently always `NULL`. |
| `FileSubmittedAnyType` | `bit NOT NULL`, existence of ≥1 active file for enterprise/calendar month. |
| `Submitted` | `bit NULL`, type-specific submission status, currently always `NULL`. |
| `LatestFileIsLateAnyType`, `LatestFileReasonAnyType` | `bit NULL`, nullable text; from a uniquely selected latest active file *only*. `FileSelectionAmbiguous bit NOT NULL` flags tied maximum `CreatedDate` values. Never call these typed lateness/reason. |
| `DataImported` | `bit NOT NULL`, any active imported row for selected type/month (any code). |
| `IndustrialRevenue`, `ExportValue`, `ImportValue` | `decimal(28,4) NULL`; selected `PerformInPeriod` values, with no cross-unit addition. Verify source precision and overflow before deployment. |
| `Has0101Value`, `Has06Value`, `Has07Value` | `bit NOT NULL`, exactly one row and non-NULL observed value, including zero. Diagnostic coverage only. |
| `CoreCodeConflict` | `bit NOT NULL`, more than one active row for at least one of the three core codes in this enterprise/month/type. |
| `CompleteIndicators`, `ValidReport` | `bit NULL`, currently always `NULL` pending approved rules. |

`FileSelectionAmbiguous` detects ties at the latest timestamp, not a type match. `LatestFileIsLateAnyType` and reason are observations about an untyped file and must not drive typed compliance decisions. If output width becomes a measured concern, category labels can later be moved to a catalog lookup without adding another dashboard aggregation SP.

## 4. SQL logic skeleton (illustrative; not a deployable procedure)

```sql
-- Future ALTER of the existing procedure, AFTER business decisions and coordinated
-- Biz changes. This illustrates data flow only; do not execute as-is.
-- @ForMonth date, @WardId int = NULL, @EconomicSectorId int = NULL,
-- @IndustryId int = NULL, @EnterpriseId bigint = NULL, @TypeReport int REQUIRED

SET @SelectedMonth = DATEFROMPARTS(YEAR(@ForMonth), MONTH(@ForMonth), 1);
SET @Start = DATEFROMPARTS(YEAR(@SelectedMonth) - 1, 1, 1);
SET @End = DATEFROMPARTS(YEAR(@SelectedMonth) + 1, 1, 1);

Months := 24 first-of-month dates in [@Start, @End);
Cohort := current, active/nondeleted Cate_Enterprises INNER JOIN
          Cate_BusinessEnterprise on EnterpriseId; apply ward/sector/enterprise
          filters and exact CSV-token industry-membership filter;
          do not derive a primary industry from token order;

ImportedRows := SELECT EnterpriseId,
                       DATEFROMPARTS(YEAR(ForMonth), MONTH(ForMonth), 1) AS MonthStart,
                       Code, PerformInPeriod
                FROM dbo.Report_DataImports
                WHERE IsDeleted = 0 AND TypeReport = @TypeReport
                  AND ForMonth >= @Start AND ForMonth < @End;

AnyImport := GROUP ImportedRows BY EnterpriseId, MonthStart
             -> DataImported = 1;
CoreByCode := GROUP ImportedRows WHERE Code IN ('0101','06','07')
              BY EnterpriseId, MonthStart, Code
              -> SourceRows, NonNullValueCount, SingleValue;
              -- emit SingleValue only when SourceRows = 1 and value IS NOT NULL;
              -- SourceRows > 1 emits conflict and NULL metric, never SUM.
CorePivot := one row per EnterpriseId, MonthStart with three nullable
             values, three Has...Value flags and CoreCodeConflict;

ActiveFiles := SELECT EnterpriseId,
                      DATEFROMPARTS(YEAR(ForMonth), MONTH(ForMonth), 1) AS MonthStart,
                      CreatedDate, IsLate, Reason
               FROM dbo.Report_ReportFiles
               WHERE IsDeleted = 0 AND ForMonth >= @Start AND ForMonth < @End;
FileByMonth := count active files; find MAX(CreatedDate) and count rows tied
               at that timestamp; emit FileSubmittedAnyType, ambiguity,
               and late/reason only for one uniquely latest row;
               -- no TypeReport filter exists here.

SELECT month, @TypeReport, cohort identity/current attributes,
       CAST(NULL AS int) AS PrimaryIndustryId, ...,
       CAST(NULL AS bit) AS ExpectedToReport,
       FileSubmittedAnyType,
       CAST(NULL AS bit) AS Submitted,
       LatestFileIsLateAnyType, LatestFileReasonAnyType,
       FileSelectionAmbiguous, DataImported,
       IndustrialRevenue, ExportValue, ImportValue,
       Has0101Value, Has06Value, Has07Value, CoreCodeConflict,
       CAST(NULL AS bit) AS CompleteIndicators,
       CAST(NULL AS bit) AS ValidReport
FROM Cohort CROSS JOIN Months
LEFT JOIN AnyImport, CorePivot, FileByMonth
  ON EnterpriseId AND MonthStart;
-- Invariant: COUNT(*) = COUNT(DISTINCT EnterpriseId, ForMonth).
```

Implementation must use valid T-SQL CTEs/temp tables and a deterministic month generator; the pseudocode intentionally avoids executable `CREATE`/`ALTER`. Group imported values *after* applying `TypeReport`, and group files by calendar month without claiming a type. A duplicate core code may be a legitimate amendment; diagnostic `NULL` prevents an unsupported revision choice while making the anomaly visible.

## 5. Migration from the current snapshot

1. **Resolve contract inputs:** obtain the dashboard's report type selection, source of monthly obligation, primary-industry rule, and mandatory/valid-report rule from accountable owners. Record effective dates and examples. These are gates for any *official* progress, industry allocation, or completeness labels. The factual snapshot can still be implemented with nullable unknowns if those features are withheld.
2. **Baseline read-only data:** inspect live column types, constraints, indexes, code/type distribution, duplicate `(EnterpriseId, calendar month, TypeReport, Code)` groups, same-month day-1/day-28 collisions, file timestamp ties, and source precision. Reconcile representative records with the audit. No hardcoded cohort count.
3. **Prepare a coordinated release:** revise the SQL source definition and `ReportDashboardBiz.GetRows` together. Pass required `@TypeReport`, map `EnterpriseId` as `Int64`, nullable flags as nullable booleans, and distinguish file-any-type facts from typed submission. Extend `DashboardFilters` and cache key with the selected report type. Keep both XML mappings pointed to the existing procedure name.
4. **Correct consumers before displaying official metrics:** keep MoM/YoY/YTD in Biz with observed values and explicit comparison coverage; decide how to label YTD as sum of imported monthly values versus source-reported accumulated value. Show observed file presence/data import/indicator coverage separately. Withhold official progress rate, not-submitted/late-by-type lists, completeness/validity counts, and exclusive industry breakdowns until the corresponding approved rules/data exist. Do not use `NULL` as false or zero.
5. **Deploy and verify atomically:** deploy app and procedure as one controlled change; validate result schema and representative reconciliations before exposing dashboards. Capture prior procedure definition and app version for rollback. If coordinated deployment is unavailable, do not alter the live SP under the current five-argument consumer; avoid a silent compatibility default that mixes types.
6. **Add indexes only when justified:** compare actual execution plans and latency with the current filtered data, then add only necessary, nonduplicate indexes through the normal migration process.

## 6. Test matrix for implementation review

This is a review/verification matrix, **not** a request to write or execute tests in this design phase.

| Scenario | Required result / assertion |
|---|---|
| 24-month boundaries; Jan and Dec selection | Same 24 calendar months from previous Jan through selected-year Dec, no extra boundary month; unique `(EnterpriseId, ForMonth)`. |
| One type has `0101=0`, another has `0101=25` | Requested type returns observed `0` or `25` respectively; never `25` or `SUM=25` by accidental mixing. |
| No row for a core code versus one row with `PerformInPeriod=0` versus one row with `NULL` | Metric is `NULL`, `0`, `NULL`; corresponding `Has...Value` is `0`, `1`, `0`. |
| Active import only for a noncore code | `DataImported=1` but all three core metrics may be `NULL`; no inference of completeness. |
| Multiple active rows for same enterprise/calendar-month/type/core code, including day-1 and day-28 | `CoreCodeConflict=1`; affected metric `NULL`; no sum or arbitrary latest pick. |
| Different `ForMonth` days within same calendar month, no duplicate code | Both map to one calendar month without row multiplication; source-date anomaly remains inspectable. |
| Active file present but no import for selected type; import present but no file | First: `FileSubmittedAnyType=1`, `DataImported=0`; second: `0`, `1`. `Submitted` remains `NULL` in both. |
| Multiple files and tied maximum `CreatedDate` | `FileSelectionAmbiguous=1`, typed submission unknown, late/reason nullable; deterministic regardless physical row order. |
| `IsLate=0`, `IsLate=1`, absent file | Untyped late value only for uniquely latest file; absent file is `NULL`, not false. |
| Enterprise current active/nondeleted, inactive, deleted, lacking business classification, or different `EnterpriseStatusId` | Candidate inclusion follows only the documented current cohort predicate; no unapproved status or obligation rule. |
| `IndustryIds='12,34'`, malformed/empty list, selected `34` | Membership filter includes exact valid token `34`; does not match substrings such as `134`; no inferred primary industry. |
| Zero revenue, no revenue, and negative/positive baseline in Biz | Zero remains observed; rate is undefined for zero/nonpositive baseline under an explicit Biz policy; no fabricated comparison. |
| 2025/2026 historical and future-dated imported rows | Period bucketing is correct; display/validity policy for future rows is separately approved and tested. |
| No matching candidate enterprises | Empty result; Biz does not report 0% official submission. |
| Cache queries for two different `TypeReport` values | Distinct keys/results; no cross-type cache reuse. |
| Each dashboard page and both XML procedure mappings | Schema maps cleanly; unsupported progress/completeness/industry claims are hidden or labeled unknown; no legacy tourism SP call. |
| Actual execution plan at representative cohort and full filters | No raw file×indicator explosion; acceptable reads/latency; add candidate indexes only if measured need. |

## 7. Approval record needed before business classifications are implemented

| Decision | Minimum answer needed | Consequence until answered |
|---|---|---|
| Enterprise × month × type obligation | Authoritative source, effective dates, exemptions, and status transitions | `ExpectedToReport=NULL`; no official denominator, overdue, or completion rate. |
| Primary industry | Authoritative field/priority and historical effective date | `PrimaryIndustry*=NULL`; no exclusive industry totals or industry progress. |
| Mandatory indicators and valid report | Required codes by type/period, zero/null rules, file requirement, revision precedence | `CompleteIndicators=NULL`, `ValidReport=NULL`; core code coverage is diagnostic only. |
| File-to-type linkage | Schema or reliable lineage from each file to `TypeReport` | `Submitted=NULL`; `FileSubmittedAnyType` remains only an untyped observation. |

**Stop point:** this document defines the proposed architecture and the exact boundary of what current data can prove. It does not authorize or execute `CREATE PROCEDURE`, `ALTER PROCEDURE`, index DDL, or application changes.
