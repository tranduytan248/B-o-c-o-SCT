using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Report
{
    // Activated only by Reports controllers, after the existing authorization filters.
    public sealed class ReportIndustryScope
    {
        private const string ContextKey = "CenIT.Reports.IndustryScope";
        private HashSet<long> _enterpriseIds;
        public string UserName { get; private set; }
        public bool IsRestricted { get; private set; }
        public static ReportIndustryScope Current
        {
            get { return HttpContext.Current == null ? null : HttpContext.Current.Items[ContextKey] as ReportIndustryScope; }
        }

        public static ReportIndustryScope Begin(string userName)
        {
            var rows = AppProcessor.ProcedureProvider.ExecuteProcedure(
                "Report_IndustryPermissions_Scope", "ReportTourismProvider", userName);
            if (rows == null || rows.Rows.Count != 1) throw new HttpException(403, "Report account is inactive or unknown.");
            var scope = new ReportIndustryScope { UserName = userName, IsRestricted = Convert.ToBoolean(rows.Rows[0]["IsRestricted"]) };
            HttpContext.Current.Items[ContextKey] = scope;
            return scope;
        }

        public bool Allows(long enterpriseId)
        {
            if (!IsRestricted) return true;
            if (_enterpriseIds == null)
            {
                var rows = AppProcessor.ProcedureProvider.ExecuteProcedure(
                    "Report_IndustryPermissions_Enterprises", "ReportTourismProvider", UserName);
                _enterpriseIds = new HashSet<long>(rows.AsEnumerable().Select(r => Convert.ToInt64(r["EnterpriseId"])));
            }
            return _enterpriseIds.Contains(enterpriseId);
        }

        public IEnumerable<T> Filter<T>(IEnumerable<T> enterprises, Func<T, long> enterpriseId)
        {
            return (enterprises ?? Enumerable.Empty<T>()).Where(e => Allows(enterpriseId(e)));
        }

        public void Demand(long enterpriseId)
        {
            if (!Allows(enterpriseId)) throw new HttpException(403, "Enterprise industry is outside your report permissions.");
        }

        public static string Procedure(string name)
        {
            return Current == null ? name : name + "_IndustryScope";
        }

        public static object[] Parameters(params object[] values)
        {
            return Current == null ? values : values.Concat(new object[] { Current.UserName }).ToArray();
        }
    }
}
