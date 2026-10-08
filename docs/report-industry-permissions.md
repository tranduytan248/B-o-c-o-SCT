# Reports industry permissions

Implemented scope: `/Report/Dashboard/*`, `/Report/Import/*`, `/Report/Report/*`, and `/Report/ExtendInfo/*`. No Sys/User management or chatbot feature was changed. Dashboard layout and existing action/role/enterprise permissions remain in place.

The live database was used for SELECT-only verification. **No table, function, grant, or SP has been deployed.** Deploy the SQL below before releasing the application changes.

## Permission rules

- Category table: `dbo.Report_UserBusinessIndustryPermissions`, one row per `(UserId, IndustryId)` with foreign keys and audit fields. Grants are normalized rows; the existing enterprise `Cate_BusinessEnterprise.IndustryIds` stays comma-separated.
- Active users with zero grant rows retain the previous industry scope, including administrators. Removing all grants restores that scope.
- Once any grant exists, an enterprise is visible if any exact numeric token in any of its business-industry rows matches a grant. Duplicate tokens/grants do not multiply report rows. Matching `2845` does not match `28450`. No implied descendants or special administrator bypass.
- Users with grants cannot see enterprises without a matching industry. Inactive/unknown users fail closed.
- Existing role, action, and enterprise authorization still applies. Industry grants only narrow that scope.
- Industry identity comes from the authenticated principal, never from request parameters. Direct enterprise details, downloads, bound import models, and edit/delete requests are checked before their action executes.
- Aggregation, time series, filter options, totals, and pagination operate on scoped SQL sources. Shared report caches are bypassed within Reports requests; permissions are re-read on the next request, including after removing grants. Other module readers retain their existing procedure/cache behavior.
- Province supplementary data in `Report_ExtendInfos` has no enterprise industry owner. Restricted users get HTTP 403 on its controller; supplementary rows are omitted from scoped exports. Full-access users keep the existing data.

## Deployment order

Run these scripts manually in a SQL client using an account authorized for schema deployment. `GO` batch separators are required; the scripts contain explicit `USE` statements. They can be rerun without deleting grants.

1. `Source/ReportDeptTourismSolution/SqlScripts/ReportIndustryPermissions/01_CategoryPermissions.sql`: creates the table, category scope/enterprise functions, and administrative grant-save SP in `baocaosct.khanhhoa.gov.vn.cate`.
2. `02_ReportScope.sql`: creates report source functions and two permission-read SPs in `baocaosct.khanhhoa.gov.vn`.
3. `03_ScopedReportProcedures.sql`: creates 33 scoped report procedures, including the selector and the complete helper closure for legacy exports. Existing original procedures are retained for callers outside Reports.
4. Deploy the Biz, Caches, ReportModule assemblies and the mirrored `Report_StoredProcedures.xml` mappings, with a normal application restart.

Report SPs receive a required final `@IndustryUserName varchar(255)` parameter. They are additive `_IndustryScope` versions of existing SPs, except the new selector. No existing SP signature or write SP is altered. Category database access must be available to the report DB connection for the cross-database functions; the existing reports already use this category DB connection pattern.

The administrative save SP accepts comma-separated grants and replaces the user's entire grant set atomically. It validates active users/industries and deduplicates IDs. Only administrators/operators with SQL execution rights should receive permission to execute it; report viewers do not need this SP. `@SavedBy` is an audit label, not an authorization check. No permission-editing screen is included in this Reports-only change.

```sql
-- Category DB only; run manually after reviewing the target user and industry IDs.
DECLARE @TargetUserId int = 123; -- replace with an existing active user's ID
EXEC dbo.p_Report_IndustryPermissions_Save
 @UserId=@TargetUserId, @IndustryIds='2845,2846', @SavedBy='your-admin-username';
-- Passing @IndustryIds='' removes all grants and restores the previous full industry scope.
```

## Dashboard and other entry points

| Screen/data | Scoped procedure | Behavior |
|---|---|---|
| All four dashboards: metrics, month/year trends, enterprise movement and warnings | `p_Report_Dashboard_IndustrialSnapshot_IndustryScope` | Applies industry permission before metric grouping and observed-month joins. |
| Registry quality, expected report cohorts, area/sector breakdowns | `p_Report_Dashboard_OverviewSummary_IndustryScope` | Both expected cohorts and diagnostic counts respect permissions. |
| Dashboard years, latest period, wards, sectors, industries, enterprise search | `p_Report_Dashboard_FilterOptions_IndustryScope` | Options derive from permitted enterprises and reports. |
| Progress grid and statuses | `p_Report_Dashboard_ProgressDetails_IndustryScope` | Counts and pages derive from the permitted cohort. |
| Dashboard enterprise detail / import raw data | `p_Report_DataImports_GetDataImport_IndustryScope` | Enterprise data is scoped in SQL and direct requests are checked in the controller. |
| Import search/history | `p_Report_DataImports_Get_IndustryScope` | Retains existing enterprise/role predicates; adds industry scope before pagination. |
| User's monthly imports | `p_Report_DataImports_GetForUserOnMonth_IndustryScope` | Intersects existing user scope with grants. |
| Import summary/details | `p_Report_DataImports_GetViaEnterpriseOnMonth_IndustryScope` | Restricts enterprise data. |
| Reports enterprise selector | `p_Report_Industry_Enterprises_SearchSelect2` | SQL filtering precedes pagination; assigned enterprise lists are intersected without broadening existing assignments. |
| Legacy exports 01–04 | `p_Report_Reports_01_*_IndustryScope` through `p_Report_Reports_04_*_IndustryScope` | Uses scoped sources and passes the authenticated username to all nested helper SPs. |

The original SPs are reused as source logic; the application calls their new scoped equivalents within Reports. Existing report writes (`ImportDatas`, `Delete`, file save) retain their contracts, with the controller's industry check after existing authorization and before the write flow. Full exact procedure mappings are in `procedure-map.json` beside the scripts.

## Validation performed on 2026-10-08

- All SQL scripts parsed with Microsoft ScriptDom `TSql110Parser` (SQL Server 2012): no errors.
- Four dashboard SELECT bodies executed against the live DB using inline equivalents of the source functions and simulated grants. No-grant rows matched the existing dashboard query bodies exactly. This verifies query bodies, not deployment of the new objects.
- Current active cohort: 18,854 enterprises. Industry `2845`: 68 enterprises, including 53 with imported report data across available months. Duplicate simulated grants still give 68 enterprises.
- Latest type-1 snapshot: 78 observed enterprise/month rows without grants, 58 for industry `2845`. Restricted overview, selector, and progress all agree on a 68-enterprise cohort.
- Isolated semantic C# compilation passed for the permission helper/base controller, report Biz/Caches, dashboard controller and models, against .NET Framework 4.5.2 references and MVC, using stubs for unchanged application infrastructure. Modified controllers also received syntax checks.
- Full project build cannot run on this Mac: legacy `Microsoft.WebApplication.targets` is absent. Deployment, browser authorization checks, and complete legacy export execution still require the Windows/IIS environment after deploying these scripts. No automated test cases were added.

## Manual deployment verification

Use an authorized Reports user with no grants first, then with one industry and multiple industries. Confirm the four dashboards, filters, imports/history, exports, and direct enterprise/download URLs agree on the permitted enterprise scope. Confirm an enterprise with several comma-separated industries is visible when any one matches; verify an unrelated industry is denied. Verify grant removal takes effect on the next request and preserves existing role/enterprise restrictions. Deploy and validate in a nonproduction DB first because the new SQL objects have intentionally not been installed on the live DB.

## Complete scoped procedure list

- `p_Report_Dashboard_FilterOptions_IndustryScope`
- `p_Report_Dashboard_IndustrialSnapshot_IndustryScope`
- `p_Report_Dashboard_OverviewSummary_IndustryScope`
- `p_Report_Dashboard_ProgressDetails_IndustryScope`
- `p_Report_Dashboard_StatisticEnterprise_IndustryScope`
- `p_Report_Dashboard_StatisticIncome_IndustryScope`
- `p_Report_Dashboard_StatisticMapVisitor_IndustryScope`
- `p_Report_Dashboard_StatisticTypeBusiness_IndustryScope`
- `p_Report_Dashboard_StatisticVisitor_IndustryScope`
- `p_Report_DataImports_GenerateSaleOfTourism_IndustryScope`
- `p_Report_DataImports_Get_IndustryScope`
- `p_Report_DataImports_GetDataImport_IndustryScope`
- `p_Report_DataImports_GetForUserOnMonth_IndustryScope`
- `p_Report_DataImports_GetViaEnterpriseOnMonth_IndustryScope`
- `p_Report_GenerateSaleOfTourism_ByCode_IndustryScope`
- `p_Report_GenerateSaleOfTourism_Sum_IndustryScope`
- `p_Report_GenerateSaleOfTourism_SumAmount_IndustryScope`
- `p_Report_GenerateSaleOfTourism_SumCustomer_IndustryScope`
- `p_Report_GenerateSaleOfTourism_SumPercentForAVG_IndustryScope`
- `p_Report_GenerateSaleOfTourism_SumPercentForOther_IndustryScope`
- `p_Report_GenerateSaleOfTourism_SumPercentForStart_IndustryScope`
- `p_Report_GenerateSaleOfTourism_SumRevenue_IndustryScope`
- `p_Report_GenerateSaleOfTourism_SumViaShip_IndustryScope`
- `p_Report_Industry_Enterprises_SearchSelect2`
- `p_Report_Reports_01_UocKetQuaHoatDongKinhDoanh_IndustryScope`
- `p_Report_Reports_02_ThongKeQuocTichKhachDuLich_IndustryScope`
- `p_Report_Reports_03_DoanhNghiepChuaGuiBaoCao_IndustryScope`
- `p_Report_Reports_04_BaoCaoHoatDongDoanhNghiep_IndustryScope`
- `p_Report_ThongKe_CongSuatPhong_ByStar_IndustryScope`
- `p_Report_ThongKe_CongSuatPhong_TrungBinh_IndustryScope`
- `p_Report_ThongKe_TongDoanhThu_IndustryScope`
- `p_Report_ThongKe_TongKhachDuLich_IndustryScope`
- `p_Report_ThongKe_TongKhachDuLich_ByCode_IndustryScope`
