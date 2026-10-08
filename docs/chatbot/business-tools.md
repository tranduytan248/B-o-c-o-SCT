# Chatbot business tools and verified SP mapping — 7 October 2026

The catalog contains **23 read-only tools**. `POST /Chatbot/Session` issues a viewer session only after login; `POST /Chatbot/Tools` validates its protected capability, current active user, bot/subject, schema and current permissions. Names, reasons and indicator labels are data, never instructions. There is no generic SQL, procedure-name, table-name, credential or write-operation input.

The latest connection was successfully audited using encrypted TDS 7.0. The audit inspected all **73 user tables and 281 deployed procedures** across BaseApp and ReportTourismDB, plus the application mappings. The initial validation ran eight proposed query contracts; the subsequent account-scope validation is documented in [security-scope.md](security-scope.md). **No database objects or data were changed.** See [database-audit.json](database-audit.json), [database-schema.csv](database-schema.csv), and [stored-procedure-inventory.csv](stored-procedure-inventory.csv).

Two current dashboard contracts are outdated and two are absent. **Deploy the SP changes in [sp-deployment.md](sp-deployment.md) before enabling the updated tools/application.** C# adapters do not fall back to incompatible procedures and present their results as valid data.

## Exactly what each tool does

SP names below are physical SQL names; `p_` is omitted in the application's logical XML names. R = ReportTourismDB / ReportTourismProvider; C = BaseApp / SysProvider. Dashboard tools use scoped FilterOptions(kind=latest) if no month/calendar timeframe is provided. Enterprise accounts without Dashboard/View default to the current calendar month. Every Chatbot SP receives server-owned @ChatbotUserId,@ProvinceRoleIds,@EnterpriseRoleIds before its business parameters. Dashboard filters resolve names/IDs with FilterOptions; these lookup calls are additional to the principal SPs listed.

| Tool | Inputs and exact returned elements | SP / service | Authorization and limits |
| --- | --- | --- | --- |
| `search_enterprises` | Name/tax/address keyword, manufacturing/trading type; assigned enterprise IDs, names, tax, address, ward/province, type, industry IDs, economic sector, status; total and hasMore | C `p_Cate_Chatbot_EnterpriseScope(mode='assigned')`; filtering in C# | Authenticated user's assigned scope; up to 50 |
| `get_enterprise_detail` | Assigned enterprise ID or query; safe profile plus first 50 **assigned** products | C EnterpriseScope(assigned) + `p_Cate_Chatbot_EnterpriseProducts` | Assigned scope; excludes contacts and attachment HTML |
| `get_enterprise_products` | enterpriseId/query, scope=assigned (default) or registry, offset/limit; product ID/code/name/unit, industry ID/name, IsMainProduct, display order | C EnterpriseScope(assigned), or EnterpriseScope(detail) after authorized registry selection; `p_Cate_Chatbot_EnterpriseProducts` | Assigned scope; registry requires Dashboard/View + Cate/Enterprise/View; paginated |
| `search_reports` | Keyword, period, optional assigned enterprise ID, offset/limit; enterprise, period, type and creation timestamp; total, nextOffset, hasMore | R `p_Report_DataImports_Get(@ForEmp,@EnterpriseIds,@FromMonth,@ToMonth,@TypeBusinessIds,@Search,@Order,@OrderDir,@PageIndex,@PageSize)` | Report/Import/View + assigned IDs; SP returns submission groups, **not ReportId** |
| `get_report_detail` | Assigned enterprise, month, optional type/indicator text, offset/limit; Code, Targets, Unit, four reported numeric fields, type/name, creation date | R `p_Report_DataImports_GetDataImport(@EnterpriseId,@ForMonth)` via GetViaEnterpriseOnMonth **C# method** | Import/View + assigned scope; SQL `GetViaEnterpriseOnMonth` is a different status SP and is **not** used for indicator detail |
| `get_my_reporting_status` | Month and all/submitted/missing; assigned/submitted/missing counts, enterprise flags | C EnterpriseScope(assigned) + R `p_Report_DataImports_GetForUserOnMonth(@ForUser,@OnMonth)` | Import/View; imported indicator evidence, not proof of correct-type file or overdue status |
| `search_enterprise_registry` | Name/tax keyword, offset/limit; active enterprise ID and display name, total, nextOffset | R `p_Report_Chatbot_FilterOptions('enterprise',NULL,@Search,@Page,100)` | Dashboard/View + authorized enterprise population; includes enterprises without classification/report configuration |
| `get_enterprise_registry_detail` | Enterprise ID/name/tax; safe profile and assigned product page | R FilterOptions + C `p_Cate_Chatbot_EnterpriseScope(mode='detail',@EnterpriseId)` + C EnterpriseProducts | Dashboard/View + Cate/Enterprise/View; verifies master remains active |
| `get_reporting_dimensions` | Dimension year/latest/ward/sector/industry/enterprise, optional type/search, offset/limit; actual IDs/names | R `p_Report_Chatbot_FilterOptions` | Dashboard/View + authorized enterprise population; choices reflect active reporting registry; sector choices are populated categories, not every dictionary entry |
| `search_indicator_catalog` | Optional type/name/code search, offset/limit; code, label, unit, parent, sourceKind, supplied/source counts, first/last observation; dynamic code patterns | R `p_Report_Chatbot_Metadata('catalog',...)` | Dashboard/View + authorized enterprise population; distinguishes configured vs observed vs dynamic_rule; config alone is not data |
| `query_report_indicators` | Month or fromMonth/toMonth (≤12 months), optional type/enterprise/ward/sector/industry, exact code, literal prefix, label keyword; individual raw indicator rows with source date, normalized month, index/level, four reported numbers, late flag, creation/latest-modification timestamps, exact-code duplicate count | R `p_Report_Chatbot_IndicatorData` | Dashboard/View + authorized enterprise population; registered active enterprises only, including unconfigured enterprises; **not an aggregation API** |
| `get_commerce_statistics` | Month, dimensions, optional type; existing dashboard overview cards, coverage and reporting trend | R `p_Report_Chatbot_Snapshot` (types 1/2/3) + `p_Report_Chatbot_Summary` via GetDashboard | Dashboard/View + authorized enterprise population; authorized registry counts are separated from selected-type economic coverage; overview movements omitted, use explicit analysis |
| `analyze_commerce_indicator` | Month, type and primary/secondary/tertiary; existing dashboard current values/trends/composition/movements | R Snapshot + Summary via GetAnalysis | Dashboard/View + authorized enterprise population; existing dashboard semantics; use comparison tool for strict full-month YTD comparison |
| `get_reporting_warnings` | Month, type, dimensions; existing primary-KPI MoM dashboard thresholds, top 8 warnings and top 8 incomplete-data cases | R Snapshot + Summary + `p_Report_Chatbot_ProgressDetails('incomplete',...)` via GetWarnings | Dashboard/View + authorized enterprise population; calculated read-only warnings, no alert delivery |
| `get_reporting_progress` | Month/type/dimensions and all/missing/incomplete/complete/conflict/file-late, offset/limit; counts and paged full expected-population worklist with tax, ward/sector/industry, import/file/late/conflict flags, values and Reason | R Snapshot + Summary + ProgressDetails; worklist explicitly queries requested status/page | Dashboard/View + authorized enterprise population; includes enterprises with **no snapshot observation**; file evidence is any type |
| `get_indicator_comparison` | Business metric, month/dimensions; observed values, matched MoM, YoY and YTD-vs-prior-YTD, absolute change, growth%, comparable/excluded counts, coverage/status | R Snapshot + Summary; calculations in ChatbotAnalytics | Dashboard/View + authorized enterprise population; every required month must be valid in both periods; positive baseline for rates |
| `analyze_growth_drivers` | Metric, comparison, groupBy sector/industry/area/enterprise, direction, dimensions, offset/limit; each group's before/after/change/growth/contribution percentage points, nextTool/nextInput | R Snapshot + Summary; C `p_Cate_BusinessIndustry_Get` for names | Dashboard/View + authorized enterprise population; each enterprise attributed once to its complete industry assignment group, not allocated arbitrarily to one industry |
| `get_indicator_alerts` | Metric, comparison and strict declineThreshold=10/20/30, dimensions, offset/limit; matched enterprise changes/decline%, severity and cumulative severity counts | R Snapshot + Summary | Dashboard/View + authorized enterprise population; only positive enterprise baselines; no persisted alert/job/email |
| `get_reporting_data_quality` | Metric/month/dimensions, offset/limit; expected/observed/supplied/conflict coverage, systemwide registry gaps, paged **observed** issue rows | R Snapshot + Summary | Dashboard/View + authorized enterprise population; use reporting_progress for wholly missing enterprises |
| `get_reported_reasons` | Enterprise/month/type selection; latest monthly file Reason, availability and file-created timestamp | R `p_Report_Chatbot_ProgressDetails('all',...,enterprise,...)` | Dashboard/View + authorized enterprise population; Reason is reported text, not verified business causation; files have no type |
| `get_report_submission_history` | One enterprise and month/range ≤12 months, offset/limit; current and soft-deleted file records, source/normalized month, IsDeleted, IsLate, Reason, creation/latest-modification dates | R `p_Report_Chatbot_ReportFiles` | Dashboard/View + authorized enterprise population; no path/content/user identities; **not complete before/after revision history** |
| `get_reporting_policy` | No input; current values/descriptions of Day_Deadline_Send_Report and Day_Deadline_Send_Late_Report | R `p_Report_Chatbot_Metadata('policy',...)` → C Sys_Configs whitelist | Dashboard/View + authorized enterprise population; config days do not prove an enforced period lock/reopen approval |
| `get_data_capabilities` | No input; active row/enterprise/month counts, first/last source period, orphan count, supported data and explicit gaps | R `p_Report_Chatbot_Metadata('capabilities',...)` | Dashboard/View + authorized enterprise population; no security logs, credentials or arbitrary configuration |

Assigned enterprise/report/status lists also accept offset. search_reports requests an explicit enterprise if its assigned-ID CSV would exceed the existing SP varchar(500) limit; it never truncates the authorized list.

All inputs are optional; a missing/ambiguous enterprise returns needsClarification rather than selecting arbitrarily. Pages default to 10, maximum 50; use nextOffset/hasMore. Registry/dimension/status queries can cross an internal 100-row page boundary. Exact source lists and recorded timestamps are preserved; no unknown data becomes zero.

## Business elements → tables → SP outputs

| Element | Source columns | Returned through |
| --- | --- | --- |
| Unique enterprise identity/profile/address | Cate_Enterprises.EnterpriseId,TaxCode,BusinessName,BusinessAddress,WardId/WardName,ProvinceId/ProvinceName,TypeBusiness | EnterpriseScope(assigned/detail); FilterOptions for ID/name selection. Active tax codes currently have no duplicates; this observation is not proof of a unique DB constraint |
| Economic sector, industry assignments, status | Cate_BusinessEnterprise.EconomicSectorId,IndustryIds,EnterpriseTypeId,EnterpriseStatusId; Cate_EconomicSector, Cate_BusinessIndustry, Cate_EnterpriseType, Cate_EnterpriseStatus | Enterprise detail, Snapshot/ProgressDetails dimensions, FilterOptions; profile uses scoped EnterpriseScope(detail) joins |
| Actual assigned/main products | Cate_BusinessEnterpriseProduct.ProductId,IsMainProduct,DisplayOrder → Cate_BusinessProduct.ProductCode,ProductName,Unit,IndustryId | New EnterpriseProducts. Industry-compatible candidates are **not** enterprise assignments |
| Raw monthly economic observations | Report_DataImports.TypeReport,Code,Targets,Unit,PerformInPeriod | IndicatorData / GetDataImport; validated monetary values through Snapshot |
| Reported prior/plan/cumulative/comparison fields | PerformPreviousPeriod,AccumulatedBeginingOfYear,ComparedSamePeriodLastYear | Raw detail/IndicatorData. Manufacturing templates label AccumulatedBeginingOfYear as **annual plan**; never automatically treat that column as recomputed YTD |
| Header/subset/product hierarchy | ReportTargetConfig.ParentCode; ReportDynamicGroupRule.CodePattern; reported Index/Level | Metadata catalog and IndicatorData. Type 2 templates use raw Index/Level, not only config. Never sum parent with children/subsets |
| Product quantities | Observed numeric/product codes, including 351*, 2.351*, 102*; reported unit (kWh, tons, kg…) | Catalog → IndicatorData exact code/prefix. Do not add unlike products/units; these are not GTSXCN |
| Export/import merchandise and destinations | Observed XKB*/NKB* and type 3 XK_MH_BRA_22, XK_MH_SAU_21, XK_MH_USA_23… labels/codes | Catalog → IndicatorData. Destinations are currently embedded in labels/codes, no normalized market table; distinguish amount from physical quantity |
| Import presence/completeness/conflicts | Active Report_DataImports rows, canonical aliases and valid units/numbers | Snapshot and ProgressDetails; missing/invalid/duplicate metric stays null |
| File evidence, reported lateness/reasons | Report_ReportFiles.ForMonth,IsDeleted,IsLate,Reason,CreatedDate,LastModifiedDate | ProgressDetails latest monthly record; ReportFiles current/soft-deleted records. No TypeReport in this table |
| Deadlines | Sys_Configs.ConfigKey/ConfigValue, two explicit report deadline keys | Metadata policy; does not expose any other settings |
| Editing evidence | Source creation/latest-modification timestamps, soft-delete flags | IndicatorData and ReportFiles. Sys_UserLogs/Sys_ProcedureLogs are technical logs and are **not** a domain before/after report revision ledger |

## Validated KPI mapping

The same alias/number/unit validation as the updated dashboard SP applies. More than one source row for a normalized enterprise/month/metric invalidates that metric even if amounts match.

| Metric input | Type / Snapshot output | Codes / unit | Important meaning |
| --- | --- | --- | --- |
| industrial_revenue | 1 / PrimaryValue | 0101 or 1.1 / tỷ đồng | Industrial revenue, **not GTSXCN** |
| export_value | 1 / SecondaryValue | 06 or 6 / nghìn USD | Export reported by manufacturing enterprises |
| import_value | 1 / TertiaryValue | 07 or 7 / nghìn USD | Import reported by manufacturing enterprises |
| wholesale_retail_revenue | 2 / PrimaryValue | 01 or 1 / triệu đồng | Wholesale/retail total |
| retail_revenue | 2 / TertiaryValue | 02 or 1.1 / triệu đồng | Retail subset; do not add to total |
| repair_revenue | 2 / SecondaryValue | 40 or 2 / triệu đồng | Repair-service revenue |
| export_fob | 3 / PrimaryValue | FOB or 0 / USD | FOB total, already includes direct + entrusted export |
| direct_export_value | 3 / SecondaryValue | XK_TT or 1 / USD | Direct export |
| entrusted_export_value | 3 / TertiaryValue | UT_XK or 2 / USD | Entrusted export |
| industrial_production_value | Unsupported | No verified code/unit/price basis | GTSXCN = Giá trị sản xuất công nghiệp. Never substitute revenue |

Do not combine manufacturing export and FOB: different population/unit and possible overlap. Codes are type-specific. A genuine zero is supplied; a missing value is unknown. Rates require a positive baseline. Growth = (current−baseline)/baseline×100. Contribution percentage points = group change / **matched selected-scope** baseline×100, not average enterprise growth.

Industry filtering tests membership. Contribution grouping keeps the whole sorted IndustryIds assignment as one group so enterprises are not counted twice. nextInput uses industryGroup to drill into that exact group. Summary has no exact-assignment-group denominator; expected/excluded counts are null for that filter rather than borrowing the parent denominator. Classification is today's classification, not historical effective-date data.

## What today's database can and cannot answer

On 7 October 2026 there are 461 active indicator rows: type 1=390 (62 source enterprise IDs), type 2=37 (1), type 3=34 (3). All economic observations are October 2–7; only **one month** exists. Six rows belong to an enterprise absent from the master. Raw active-registry query returns 455 rows. September has files, not economic observations.

| Reporting type | Current assigned population | Imported | Missing | Complete required KPIs | Incomplete |
| --- | ---: | ---: | ---: | ---: | ---: |
| Manufacturing | 18,854 | 61 | 18,793 | 22 | 39 |
| Trading | 2 | 1 | 1 | 0 | 1 |
| Export/import | 33 | 3 | 30 | 2 | 1 |

Type populations overlap; do not add them into a unique enterprise total. One export/import enterprise has a conflict. Registry has 18,854 active enterprises, 250 classified, 18,602 missing a business classification row and 18,604 missing industry. The chatbot can report current data and coverage; actual MoM/YoY/YTD comparative growth is unavailable without history. Historical expected populations use current assignments.

| Original business requirement | Available answers now | Remaining data/workflow requirement |
| --- | --- | --- |
| 1. Enterprise master | Profile, tax, geography, classifications and assigned products | Data quality/constraints and external reconciliation remain separate |
| 2. Monthly entry | Read all actual monthly fields and submission evidence | 8–10-field online form and replacing Excel import are not chatbot writes |
| 3. Computed comparisons | Validated KPI comparisons once both periods have complete observations | GTSXCN source and prior/monthly history are absent; reported plan fields are not independent historical observations |
| 4. Alerts | Query >10/20/30% from valid pairs | No current historical pairs; notification/persistence job not implemented |
| 5. Leadership dashboard | Filtered KPI/coverage, known hierarchy and drill-down | Complete province economic coverage has not been demonstrated |
| 6. Governance | Worklists, configured deadlines, current edit timestamps/soft-deleted file evidence, permission checks | Period lock/reopen, full revision ledger, export generation require their own domain contracts; chatbot cannot claim these operations happened |
| 7. Management explanations | Contribution tree and exact recorded reasons | Dedicated reason categories and verified causation are not present |
| 8. Data connection | Fixed allowlisted APIs/SP adapters and reusable tax/enterprise IDs | Provincial ingestion/upsert needs separate trusted service authentication and provenance; no arbitrary write capability |

## Natural-language routing examples

- “Tháng mới nhất đã báo cáo bao nhiêu?” → get_reporting_dimensions(latest) → get_reporting_progress(month, reportType).
- “Sản lượng điện gió tháng 10?” → search_indicator_catalog(keyword=Điện gió, reportType=manufacturing) → query_report_indicators(month=2026-10, selected code). Preserve kWh and pagination; do not claim a province total from a partial page.
- “Xuất khẩu sang Hoa Kỳ?” → search_indicator_catalog(reportType=export_import, keyword=Hoa Kỳ) → query_report_indicators(code=XK_MH_USA_23, reportType=export_import, month=2026-10).
- “Xuất khẩu giảm do doanh nghiệp nào?” → get_indicator_comparison(metric=export_fob) → analyze_growth_drivers(groupBy=economic_sector) → returned nextInput → get_reported_reasons. With today's data, state insufficient history instead of inventing the decrease.
- “Ai sửa báo cáo và đã sửa từ số nào sang số nào?” → get_report_submission_history can provide recorded timestamps/soft-delete evidence, but must say a complete before/after ledger is unavailable. User identities and security logs are not returned.
- “GTSXCN tăng bao nhiêu?” → metric=industrial_production_value → unsupported/missing_verified_indicator_source; request the verified indicator definition, not substitute doanh thu.

## Verification status

The audit and SELECT-body validations are real live-data checks, not mocked tests. No automatic test cases were added. SQL deployment, the native Windows/.NET Framework build, IIS execution, authenticated Chatbot upstream call and browser rendering still require the deployment environment. The audit inventory is a dated snapshot; run validation again after deployment or source edits.
