using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using CenIT.ReportTourism.Biz.Report;
using CenIT.ReportTourism.Models.Report;
using TSFramework.Core.Members.Caching;

namespace CenIT.ReportTourism.Caches.Report
{
    public class ReportDashboardCache : CacheLayer
    {
        private ReportDashboardBiz _dataDashboardApi;
        private ReportDashboardBiz Api => _dataDashboardApi ?? (_dataDashboardApi = new ReportDashboardBiz());

        protected override string[] MasterCacheKeyArray => new[]
        {
            "ReportDashboardCache", "DataImportCache", "ReportExtendInfosCache", "EnterprisesCache", "SysMessagesCache", "SysConfigsCache", "BusinessIndustryCache", "EconomicSectorCache", "BusinessProductsCache", "CENIT.APP.Cache"
        };

        public DashboardModel GetDashboard(DashboardFilters filters)
        {
            if (ReportIndustryScope.Current != null) return Api.GetDashboard(Normalize(filters));
            filters = Normalize(filters);
            var key = CacheKey("Overview", filters);
            var cached = GetCacheItem(key) as DashboardModel;
            if (cached != null) return cached;
            cached = Api.GetDashboard(filters);
            AddDashboardItem(key, cached);
            return cached;
        }

        public DashboardAnalysisModel GetAnalysis(DashboardFilters filters)
        {
            if (ReportIndustryScope.Current != null) return Api.GetAnalysis(Normalize(filters));
            filters = Normalize(filters);
            var key = CacheKey("Analysis", filters);
            var cached = GetCacheItem(key) as DashboardAnalysisModel;
            if (cached != null) return cached;
            cached = Api.GetAnalysis(filters);
            AddDashboardItem(key, cached);
            return cached;
        }

        public DashboardWarningsModel GetWarnings(DashboardFilters filters)
        {
            if (ReportIndustryScope.Current != null) return Api.GetWarnings(Normalize(filters));
            filters = Normalize(filters);
            var key = CacheKey("Warnings", filters);
            var cached = GetCacheItem(key) as DashboardWarningsModel;
            if (cached != null) return cached;
            cached = Api.GetWarnings(filters);
            AddDashboardItem(key, cached);
            return cached;
        }

        public DashboardProgressModel GetProgress(DashboardFilters filters)
        {
            if (ReportIndustryScope.Current != null) return Api.GetProgress(Normalize(filters));
            filters = Normalize(filters);
            var key = CacheKey("Progress", filters);
            var cached = GetCacheItem(key) as DashboardProgressModel;
            if (cached != null) return cached;
            cached = Api.GetProgress(filters);
            AddDashboardItem(key, cached);
            return cached;
        }

        public DashboardFilters Normalize(DashboardFilters filters) { return Api.Normalize(filters); }

        public IList<DashboardOption> GetEnterpriseOptions(int? type,string search,int page)
        {
            if (ReportIndustryScope.Current != null) return Api.GetFilterOptions("enterprise",type,search,page);
            var key = "EnterpriseOptions-" + type + "-" + page + "-" + search;
            var cached = GetCacheItem(key) as IList<DashboardOption>;
            if (cached != null) return cached;
            cached = Api.GetFilterOptions("enterprise",type,search,page);
            AddDashboardItem(key,cached);
            return cached;
        }

        private void AddDashboardItem(string key,object value)
        {
            var cache = HttpRuntime.Cache;
            foreach (var master in MasterCacheKeyArray)
                if (cache[master] == null) cache[master] = DateTime.Now;
            var dependency = new CacheDependency(null,MasterCacheKeyArray);
            cache.Insert(MasterCacheKeyArray[0] + "-" + key,value,dependency,DateTime.Now.AddMinutes(60),Cache.NoSlidingExpiration);
        }

        private static string CacheKey(string page, DashboardFilters filters)
        {
            filters = filters ?? new DashboardFilters();
            return string.Join("-", "ManagementDashboard", page, filters.Year, filters.Month, filters.ReportType,
                filters.AreaId ?? "all", filters.EconomicSectorId ?? "all", filters.IndustryId ?? "all",
                filters.EnterpriseId ?? "all", filters.Metric ?? "primary", filters.Status, filters.Page, filters.Breakdown);
        }
    }
}
