# Reuse, create and modify SPs — verified 7 October 2026

The application uses two connections: **BaseApp** for categories/security (SysProvider), and **ReportTourismDB** for reporting (ReportTourismProvider). The configured database names are `baocaosct.khanhhoa.gov.vn.cate` and `baocaosct.khanhhoa.gov.vn`. No live objects or source rows were changed during this task. This is a concrete deployment list for review and manual execution.

## Reuse unchanged

| Physical SP / connection | Exact parameters passed by the tool adapter | Reused purpose |
| --- | --- | --- |
| p_Cate_BusinessIndustry_Get / BaseApp | @Search=NULL,@Order='1',@OrderDir='ASC',@PageIndex=0,@PageSize=-1 through GetAll cache | Labels for growth industry-assignment groups; live pagination also checked with PageSize=50 |
| p_Report_DataImports_Get / ReportTourismDB | @ForEmp=user,@EnterpriseIds=assigned IDs CSV,@FromMonth=range start,@ToMonth=range end,@TypeBusinessIds=NULL,@Search=keyword,@Order='1',@OrderDir='DESC',@PageIndex=0,@PageSize=limit | Assigned report submission groups. Output has no ReportId, so the adapter no longer emits default/false IDs |
| p_Report_DataImports_GetDataImport / ReportTourismDB | @EnterpriseId=authorized assigned ID,@ForMonth=first day of month | Raw indicator detail; C# then applies optional type/label filter and pagination |
| p_Report_DataImports_GetForUserOnMonth / ReportTourismDB | @ForUser=user,@OnMonth=first day of month | Imported enterprise evidence for assigned submitted/missing status |
| p_Sys_User_GetByUserName / BaseApp | @UserName from validated FormsAuthentication ticket | Session active-user lookup; never return the raw user model |
| p_Sys_User_GetById / BaseApp | @UserId from protected capability | Executor active-user lookup; no user ID override from caller |

Fresh SQL permission checks are mandatory, and the application’s existing `AppProcessor.Author.IsAllow` is also required for Dashboard/View, Import/View and Cate/Enterprise/View. The live database contains p_Sys_Permission_IsAllow and p_Sys_Permission_GetViaUser; the integration combines these function permissions with the centralized SQL enterprise scope; cached framework grants alone cannot grant access. The user-data lookup SPs are security dependencies, not registered business tools.

Six business read SPs above were executed on the live DB with an assigned enterprise that has reports; all result contracts matched. None of the save/import/delete procedures is executed by chatbot tools.

## Create scoped Chatbot objects — required before enabling

Every Chatbot procedure takes **three server-owned parameters first**: `@ChatbotUserId int,@ProvinceRoleIds varchar(500),@EnterpriseRoleIds varchar(500)`. They come from the protected capability and server configuration; the tool JSON cannot override them. The following 2 functions and 9 SPs were absent at audit time. CREATE is for first deployment; inspect any subsequently installed object and ALTER it to this scoped contract instead.

| Object / SQL file | Database | Remaining parameters → returned elements / purpose |
| --- | --- | --- |
| [f_Chatbot_UserScope.sql](../../Source/ReportDeptTourismSolution/SqlScripts/f_Chatbot_UserScope.sql) | BaseApp | @UserId,@ProvinceRoleIds,@EnterpriseRoleIds → current active/unlocked account flags and fresh Dashboard/Import/Registry view permissions |
| [f_Chatbot_AuthorizedEnterprises.sql](../../Source/ReportDeptTourismSolution/SqlScripts/f_Chatbot_AuthorizedEnterprises.sql) | BaseApp | @UserId,@ProvinceRoleIds,@EnterpriseRoleIds,@AllowProvince → authorized active EnterpriseIds; explicit assignments take precedence; tax-code fallback only for enterprise roles |
| [p_Cate_Chatbot_EnterpriseScope.sql](../../Source/ReportDeptTourismSolution/SqlScripts/p_Cate_Chatbot_EnterpriseScope.sql) | BaseApp | @Mode access/assigned/detail,@EnterpriseId nullable → internal access flags or safe scoped enterprise profiles/classifications; replaces unsafe legacy name/email/creator matching |
| [p_Cate_Chatbot_EnterpriseProducts.sql](../../Source/ReportDeptTourismSolution/SqlScripts/p_Cate_Chatbot_EnterpriseProducts.sql) | BaseApp | @EnterpriseId,@Offset,@Limit≤50 → total + actual active assigned products, industry labels, IsMainProduct and DisplayOrder; foreign IDs denied |
| [p_Report_Chatbot_Snapshot.sql](../../Source/ReportDeptTourismSolution/SqlScripts/p_Report_Chatbot_Snapshot.sql) | ReportTourismDB | @ForMonth,@TypeReport,@WardId,@EconomicSectorId,@IndustryId,@EnterpriseId → scoped observed enterprise/month KPI values, conflicts, file evidence and reported reasons; retained metric normalization of dashboard snapshot |
| [p_Report_Chatbot_Summary.sql](../../Source/ReportDeptTourismSolution/SqlScripts/p_Report_Chatbot_Summary.sql) | ReportTourismDB | @ForMonth,@WardId,@EconomicSectorId,@IndustryId,@EnterpriseId,@GroupBy nullable/ward/sector → scoped registry/coverage counts and grouped denominators; diagnostics cannot reveal other accounts |
| [p_Report_Chatbot_FilterOptions.sql](../../Source/ReportDeptTourismSolution/SqlScripts/p_Report_Chatbot_FilterOptions.sql) | ReportTourismDB | @OptionKind year/latest/ward/sector/industry/enterprise,@TypeReport,@Search,@Page,@PageSize≤100 → TotalRow,Value,Text from authorized population only, including latest/year selectors |
| [p_Report_Chatbot_ProgressDetails.sql](../../Source/ReportDeptTourismSolution/SqlScripts/p_Report_Chatbot_ProgressDetails.sql) | ReportTourismDB | @ForMonth,@TypeReport,@WardId,@EconomicSectorId,@IndustryId,@EnterpriseId,@Status,@Page,@PageSize≤100 → scoped expected worklist/total with snapshot flags including missing observations |
| [p_Report_Chatbot_IndicatorData.sql](../../Source/ReportDeptTourismSolution/SqlScripts/p_Report_Chatbot_IndicatorData.sql) | ReportTourismDB | @FromMonth,@ToMonth (≤12 months),@TypeReport,@EnterpriseId,@WardId,@EconomicSectorId,@IndustryId,@Code,@CodePrefix,@Search,@Offset,@Limit≤50 → scoped total and raw indicator fields, source/normalized month, timestamps, exact-code duplicate counts |
| [p_Report_Chatbot_Metadata.sql](../../Source/ReportDeptTourismSolution/SqlScripts/p_Report_Chatbot_Metadata.sql) | ReportTourismDB | @Kind catalog/policy/capabilities,@TypeReport,@Search,@Offset,@Limit≤50 → shared static indicator definitions and two deadline settings; observed counts/period bounds scoped to account; orphan diagnostics hidden from restricted accounts |
| [p_Report_Chatbot_ReportFiles.sql](../../Source/ReportDeptTourismSolution/SqlScripts/p_Report_Chatbot_ReportFiles.sql) | ReportTourismDB | @EnterpriseId,@FromMonth,@ToMonth (≤12 months),@Offset,@Limit≤50 → scoped current/soft-deleted file evidence, dates/lateness/reasons; no file paths, content or user identities |

Dates are first-of-month `date` values; dimension IDs are nullable `int`. Scope is applied before totals, paging or aggregation. Review the explicit category database name in cross-database SQL before deployment. Matching mappings are included in WebApp and module XML files. Chatbot `ChatbotAccess.Report` maps the dashboard service's four logical names to the scoped `Report_Chatbot_*` counterparts and bypasses shared report/dashboard caches. It rejects any unlisted procedure name.

## Existing dashboard changes — separate UI deployment

The original dashboard screen still uses `p_Report_Dashboard_IndustrialSnapshot` and `p_Report_Dashboard_OverviewSummary` (existing, workspace ALTER proposals), plus `p_Report_Dashboard_FilterOptions` and `p_Report_Dashboard_ProgressDetails` (new CREATE proposals). Their economic calculation improvements are documented in the dashboard implementation. **Chatbot does not execute those unscoped dashboard procedures.** Deploying them cannot replace the scoped objects above. This change does not certify the object authorization of every legacy application controller or write SP.

No existing live SP needs modification specifically for the new Chatbot scope, because the scoped counterparts are separate contracts. If the four previously proposed `Report_Chatbot_IndicatorData/Metadata/ReportFiles` and `Cate_Chatbot_EnterpriseProducts` have since been installed, update them to the actor-prefixed, scoped versions before enabling the matching application.

## Existing SPs intentionally not used for the new tools

| Existing procedure(s) | Audited limitation / action |
| --- | --- |
| p_Cate_BusinessProducts_GetViaEnterprise | Industry-compatible candidate products, not actual assigned products; omits IsActive/IsDeleted/IndustryName in result. Old chatbot filter mistakenly interpreted absent IsActive as false. Replaced with actual-assignment SP; do not repurpose this SP because import forms need industry candidates |
| p_Cate_BusinessEnterpriseProduct_Get | References dbo.BusinessProduct and dbo.BusinessIndustry, absent from audited category tables; dedicated chatbot SP uses Cate_* tables. Repair the existing product-assignment screen procedure separately by correcting table references; no unrelated screen rewrite here |
| p_Cate_BusinessEnterpriseProduct_GetMainProduct | Returns TOP(1); would hide multiple stored main flags. Not sufficient for a full assigned-product list |
| p_Cate_BusinessProductImpExp_* | References Cate_BusinessProductImpExp, absent from audited schema. Current trade merchandise is stored as codes/labels in Report_DataImports; do not build tools over nonexistent table |
| p_Report_DataImports_GetViaEnterpriseOnMonth | Submission/status groups, not code/value rows; use GetDataImport for actual indicators |
| p_Report_Dashboard_StatisticVisitor, StatisticMapVisitor, StatisticIncome, StatisticTypeBusiness and tourism templates | Tourism-specific codes/populations. Not business sources for Sở Công Thương KPIs |
| p_Report_ThongKe_BaoCaoChuaNop | Checks EnterpriseId IS NULL on nonnullable source; not correct missing-population worklist. Use ProgressDetails |
| p_Report_Manufacturing*, p_Report_Trading*, usp_Report_KTXH* | Existing template/reporting outputs and wrappers; not the verified metric comparison contract. Some formulas use reported plan/prior fields, sparse YTD and ratios rather than growth (ratio×100 vs (ratio−1)×100). Reuse for existing template screens, not silently relabel as analytical growth. Changes need approval of template-column semantics |
| p_Report_DataImports_ImportDatas, CheckDataImport, Delete; p_Report_ReportFiles_Save; category Save/Import and p_SCT_Enterprise_Upsert | Existing writes remain for authorized application workflows; not called through read-only chatbot capability |

## Other app/database audit findings

Four pre-existing XML mappings are absent: `Cate_Team_GetByWardId`→`Cate_Team_GetByWardId`, `Sys_Function_Register`→p_Sys_Function_Register, `Sys_Role_GetAction`→p_Sys_Role_GetAction, `Sys_Permission_Get`→p_Sys_Permission_Get. These are separate legacy mappings, not Chatbot scope dependencies. An existing p_Cate_Team_GetByWardCode takes a ward **code**, not an ID; do not simply repoint the mapping. No equivalent contract was verified for the other three. Their application paths need recovery from the correct deployment SQL or an explicit contract review. The chatbot does not call these missing names.

The complete deployed/mapped/unmapped inventory and parameter types are in [stored-procedure-inventory.csv](stored-procedure-inventory.csv). “Deployed” confirms existence, not that every legacy admin/tourism path has been executed successfully.

## Additional business capabilities needing a data-model change first

These are not executable CREATE scripts; assigning an SP name alone cannot supply absent source data.

| Capability | Needed data / prospective SP contract |
| --- | --- |
| GTSXCN | Confirm indicator code, unit and current/constant-price basis; extend import validation/template/config first, then Snapshot outputs/metric descriptor. Do not modify 0101 revenue to pretend it is GTSXCN |
| Full before/after edit history | Append-only report revision records with actor, time, period/type, original/new values and reason; a future Report_RevisionHistory_Get read SP plus atomic revision writing in every authorized change path |
| Period locks and reopening | Period/type lock state and scoped reopen approvals; future Report_PeriodLock_Get/Set and ReopenRequest_Get/Save contracts, enforced transactionally in import/edit/delete paths |
| Verified causes and normalized markets | Typed monthly business-reason entries; trade rows linked to product/country/flow/value/quantity/unit. Then read SPs for reason and market breakdown; current labels remain reported statements |
| Persisted automated alerts | A valid metric/history source and alert event/acknowledgment state; scheduled evaluation and idempotent alert saving separate from read queries |
| Provincial data reuse | Trusted service upsert with tax-code identity, provenance, source timestamp/version and audit; review p_SCT_Enterprise_Upsert before reuse. Do not expose ingestion through chatbot bearer capability |
| Excel/PDF generation | Dedicated export controller/service with report scope and authorization; SPs can supply the same read data, but a chatbot read response is not an exported file |

## Deployment and verification order

1. Review role IDs and cross-database names; preserve any existing definitions. In BaseApp install `f_Chatbot_UserScope`, then `f_Chatbot_AuthorizedEnterprises`, then the two `p_Cate_Chatbot_*` SPs.
2. Install the seven `p_Report_Chatbot_*` SPs in ReportTourismDB. Apply matching WebApp/module XML and application code atomically; old SP contracts must not remain enabled with new code.
3. Provision explicit enterprise assignments for restricted managers. Review `Chatbot:ProvinceRoleIds=1,6` and `Chatbot:EnterpriseRoleIds=5`; missing/invalid/overlapping configuration fails closed. No role name or role ID is accepted from callers.
4. Grant the runtime database identity the necessary procedure execution and cross-database read/function access, with no new schema-administration privileges. Do not expose direct SQL execution to end users. Recycle IIS and enable Chatbot only after deployment and upstream configuration.
5. On Windows/IIS verify viewer login/CSRF, altered subject/token/enterprise IDs, manager and enterprise denial, permission revocation, counts/selectors/metadata and drill-down pagination. Keep the feature disabled until those checks pass.

The initial 37 economic query checks plus six reused-SP checks passed. The account-scope review additionally executed the proposed functions and read query bodies against live data **by inlining them, without creating or altering objects**; see [security-scope.md](security-scope.md) and [security-validation.json](security-validation.json). Native Windows/.NET Framework compilation, deployed SP execution and live IIS/Chatbot/browser execution remain unverified on this Mac. No automatic test cases were added and no live SQL was deployed.
