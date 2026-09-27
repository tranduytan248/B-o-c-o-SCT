using System;
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
            "ReportDashboardCache", "DataImportCache", "ReportExtendInfosCache", "EnterprisesCache", "CENIT.APP.Cache"
        };

        public DashboardModel GetDashboard(DashboardFilters filters)
        {
            var key = CacheKey("Overview", filters);
            var cached = GetCacheItem(key) as DashboardModel;
            if (cached != null) return cached;
            cached = Api.GetDashboard(filters);
            AddCacheItem(key, cached);
            return cached;
        }

        public DashboardAnalysisModel GetAnalysis(DashboardFilters filters)
        {
            var key = CacheKey("Analysis", filters);
            var cached = GetCacheItem(key) as DashboardAnalysisModel;
            if (cached != null) return cached;
            cached = Api.GetAnalysis(filters);
            AddCacheItem(key, cached);
            return cached;
        }

        public DashboardWarningsModel GetWarnings(DashboardFilters filters)
        {
            var key = CacheKey("Warnings", filters);
            var cached = GetCacheItem(key) as DashboardWarningsModel;
            if (cached != null) return cached;
            cached = Api.GetWarnings(filters);
            AddCacheItem(key, cached);
            return cached;
        }

        public DashboardProgressModel GetProgress(DashboardFilters filters)
        {
            var key = CacheKey("Progress", filters);
            var cached = GetCacheItem(key) as DashboardProgressModel;
            if (cached != null) return cached;
            cached = Api.GetProgress(filters);
            AddCacheItem(key, cached);
            return cached;
        }

        public DashboardQualityModel GetQuality(DashboardFilters filters)
        {
            var key = CacheKey("Quality", filters);
            var cached = GetCacheItem(key) as DashboardQualityModel;
            if (cached != null) return cached;
            cached = Api.GetQuality(filters);
            AddCacheItem(key, cached);
            return cached;
        }

        private static string CacheKey(string page, DashboardFilters filters)
        {
            filters = filters ?? new DashboardFilters();
            return string.Join("-", "ManagementDashboard", page, filters.Year, filters.Month,
                filters.AreaId ?? "all", filters.EconomicSectorId ?? "all", filters.IndustryId ?? "all",
                filters.EnterpriseId ?? "all", filters.Metric ?? "industrial");
        }
    }
}
