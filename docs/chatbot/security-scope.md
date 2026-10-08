# Chatbot account scope and IDOR controls

The authenticated account determines the maximum accessible enterprise population. Tool IDs and filters narrow that population; they never grant access. This policy applies to the Chatbot Session/Tools integration and its 23 tools. Existing application screens and write workflows retain their own authorization and need separate object-level review before claiming the entire legacy application is IDOR-safe.

## Account matrix

| Account | Enterprise profile/products and assigned import tools | Dashboard, registry, raw indicators and analytics |
| --- | --- | --- |
| Enterprise role 5 | Explicit approved enterprise assignments; if no assignment exists, approved enterprise with TaxCode exactly matching account username. Import tools additionally require Import/View | Denied even if Dashboard/View is accidentally granted, including mixed enterprise/admin roles |
| Province administrator roles 1 or 6 | Assigned tools remain assignment-scoped; they do not silently search the whole province. Role 6 currently lacks Import/View | Province-wide only with fresh Dashboard/View, no enterprise role and no explicit assignments. Registry details additionally require Cate/Enterprise/View |
| Administrator with explicit enterprise assignments | Assigned approved enterprise population | Restricted to those assignments, even with a province allowlisted role |
| Management/officer outside province allowlist | Explicit approved enterprise assignments; applicable function rights required | Dashboard/View plus assignments; no assignment means empty population. Registry detail also requires Cate/Enterprise/View |
| Inactive, deleted, locked or unknown account | Denied / no authorized objects | Denied |

Role IDs are based on the audited database, not role-name inference. The live database has no dedicated management role. Provision management accounts with explicit `Cate_EnterprisePermissions` and appropriate view rights. Add a role to the server's province allowlist only when its whole-province authority is intentional. Environment overrides are `CHATBOT_PROVINCEROLEIDS` and `CHATBOT_ENTERPRISEROLEIDS`. Empty allowlists grant no corresponding broad rights; missing, invalid or overlapping lists fail closed.

## Enforcement and source mapping

- `/Chatbot/Session` checks Forms Authentication, CSRF and current DB account state before issuing a viewer capability. `/Chatbot/Tools` checks protected token, expiry, bot, subject and current identity on every request. Caller-provided user IDs, role lists, SQL names or unexpected fields are rejected by the API/catalog.
- `ChatbotAccess` owns the authenticated user ID and role configuration. `f_Chatbot_UserScope` reads current account, role and exact function/action grants; `f_Chatbot_AuthorizedEnterprises` calculates the row population. Deleted roles/functions never confer rights.
- `p_Cate_Chatbot_EnterpriseScope` replaces the legacy GetViaUser/GetByID chatbot paths. Email, full-name and CreatedBy matching cannot authorize an enterprise. Explicit assignments take precedence over tax-code fallback and province roles; removing assignments from a manager does not turn them into a province user unless separately allowlisted.
- Searches, safe profiles, actual product assignments and clarification candidates use this population. A guessed foreign enterprise ID is denied or absent without exposing its name. Numeric dimension IDs must match an authorized option; they cannot fall through to a similarly named object.
- The four dashboard service reads are remapped to `p_Report_Chatbot_Snapshot/Summary/FilterOptions/ProgressDetails`. Scope applies before counts, group denominators, aggregation and pagination. The same restriction covers latest/year selectors, missing-report lists, reasons and drill-downs.
- `p_Report_Chatbot_IndicatorData/ReportFiles/Metadata` apply the same SQL row scope. Static indicator definitions and the two deadline settings are shared reference data; observed values, counts and dates remain scoped. Restricted metadata does not expose orphan enterprise counts.
- Assigned import queries reuse GetDataImport only after exact object membership; Get receives a nonempty authorized enterprise CSV with an SQL AND restriction; GetForUserOnMonth evidence is intersected with the current authorized list before computing status. CSV overflow requires selecting an enterprise, never truncating or omitting the restriction. None of their raw user/contact fields is returned.
- Chatbot reads bypass shared dashboard/report caches. Only public industry reference labels use the existing dictionary cache. HTTP responses are no-store. No tool can write, save, delete, unlock or change permissions.

Rights are checked on every tool request and the new SQL contracts recheck actor state. This is not a promise of atomic revocation midway through all legacy reused reads. A stolen capability remains usable until its short expiry (at most 15 minutes) unless account/role/object rights are removed; browser logout destroys the widget but does not invalidate already-issued capabilities on the server. Strict immediate logout revocation requires a server-side session/revocation store integrated with login/logout.

## Read-only verification

Validation used actual active enterprise accounts with distinct enterprises, an administrator with five explicit assignments, and administrators without assignments. All queries ran SELECT bodies with the proposed functions inlined; no live functions/SPs were installed and no permissions/data were changed. Results contain counts and flags only, not account identities or business values: [security-validation.json](security-validation.json).

Foreign-enterprise membership counts were zero for both enterprise accounts, foreign product lookup was denied, enterprise/unknown dashboard queries were denied, and assigned/province admin sources were distinct (five versus 18,854 registry enterprises at validation). Scoped latest/year/ward/sector/industry selectors and catalog/policy queries executed successfully. There was no dedicated management account to verify end to end; management follows the explicit-assignment branch and still requires Windows/IIS validation with a provisioned account.

C# 7.3 syntax and catalog/XML/project consistency are checked locally. The Mac cannot build the legacy WebApplication target; Windows compilation and real IIS token, permission-revocation and IDOR checks remain required. See [sp-deployment.md](sp-deployment.md) for the 2-function/9-SP deployment order. Keep `Chatbot:Enabled=false` until these scoped contracts are deployed and validated.
