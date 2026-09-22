using System;
using System.Collections.Generic;
using System.ComponentModel;
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


        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public ReportStatisticEnterpriseModel GetStatisticEnterprise(DateTime? forMonth)
        {
            var rawKey = $"StatisticEnterpriseOnMonth--{forMonth}";
            // See if the item is in the cache
            var statisticEnterprise = GetCacheItem(rawKey) as ReportStatisticEnterpriseModel;
            if (statisticEnterprise != null) return statisticEnterprise;
            // Item not found in cache - retrieve it and insert it into the cache
            statisticEnterprise = Api.GetStatisticEnterprise(forMonth);
            AddCacheItem(rawKey, statisticEnterprise);
            return statisticEnterprise;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<ReportStatisticTypeBusinessModel> GetStatisticTypeBusiness(DateTime? forMonth)
        {
            var rawKey = $"DataStatisticTypeBusinessOnMonth--{forMonth}";
            // See if the item is in the cache
            var dataStatisticTypeBusiness = GetCacheItem(rawKey) as List<ReportStatisticTypeBusinessModel>;
            if (dataStatisticTypeBusiness != null) return dataStatisticTypeBusiness;
            // Item not found in cache - retrieve it and insert it into the cache
            dataStatisticTypeBusiness = Api.GetStatisticTypeBusiness(forMonth);
            AddCacheItem(rawKey, dataStatisticTypeBusiness);
            return dataStatisticTypeBusiness;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<ReportStatisticVisitorModel> GetStatisticVisitor(DateTime? forMonth)
        {
            var rawKey = $"DataStatisticVisitorOnMonth--{forMonth}";
            // See if the item is in the cache
            var dataStatisticVisitors = GetCacheItem(rawKey) as List<ReportStatisticVisitorModel>;
            if (dataStatisticVisitors != null) return dataStatisticVisitors;
            // Item not found in cache - retrieve it and insert it into the cache
            dataStatisticVisitors = Api.GetStatisticVisitor(forMonth);
            AddCacheItem(rawKey, dataStatisticVisitors);
            return dataStatisticVisitors;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<ReportStatisticMapVisitorModel> GetStatisticMapVisitor(DateTime? forMonth)
        {
            var rawKey = $"DataStatisticMapVisitorOnMonth--{forMonth}";
            // See if the item is in the cache
            var dataStatisticMapVisitors = GetCacheItem(rawKey) as List<ReportStatisticMapVisitorModel>;
            if (dataStatisticMapVisitors != null) return dataStatisticMapVisitors;
            // Item not found in cache - retrieve it and insert it into the cache
            dataStatisticMapVisitors = Api.GetStatisticMapVisitor(forMonth);
            AddCacheItem(rawKey, dataStatisticMapVisitors);
            return dataStatisticMapVisitors;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<ReportStatisticIncomeModel> GetStatisticIncome(DateTime? forMonth, int? typeStatistic)
        {
            var rawKey = $"DataStatisticIncomeOnMonth--{forMonth}-{typeStatistic}";
            // See if the item is in the cache
            var dataStatisticIncomes = GetCacheItem(rawKey) as List<ReportStatisticIncomeModel>;
            if (dataStatisticIncomes != null) return dataStatisticIncomes;
            // Item not found in cache - retrieve it and insert it into the cache
            dataStatisticIncomes = Api.GetStatisticIncome(forMonth, typeStatistic);
            AddCacheItem(rawKey, dataStatisticIncomes);
            return dataStatisticIncomes;
        }
    }
}