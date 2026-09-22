using System.ComponentModel;
using System.Data;
using CenIT.ReportTourism.Biz.Report;
using TSFramework.Core.Members.Caching;

namespace CenIT.ReportTourism.Caches.Report
{
    [DataObject]
    public class ReportCache : CacheLayer
    {
        private ReportBiz _reportApi;

        private ReportBiz Api => _reportApi ?? (_reportApi = new ReportBiz());

        protected override string[] MasterCacheKeyArray => new[] { "ReportCache", "CENIT.ENV.Cache" };

        public DataTable GetDataReport(string procedureName, params object[] p)
        {
            return Api.GetDataReport(procedureName, p);
        }
    }
}