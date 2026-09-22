using System.Collections.Generic;
using System.ComponentModel;
using CenIT.ReportTourism.Biz.Report;
using CenIT.ReportTourism.Models.Report;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;
using TSFramework.Core.Utils;

namespace CenIT.ReportTourism.Caches.Report
{
    [DataObject]
    public class ReportExtendInfoCache : CacheLayer
    {
        private ReportExtendInfoBiz _reportExtendInfoApi;

        private ReportExtendInfoBiz Api => _reportExtendInfoApi ?? (_reportExtendInfoApi = new ReportExtendInfoBiz());

        protected override string[] MasterCacheKeyArray => new[] { "ReportExtendInfosCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<ReportExtendInfoModel> GetAll()
        {
            const string rawKey = "AllReportExtendInfos";
            // See if the item is in the cache
            var reportExtendInfos = GetCacheItem(rawKey) as List<ReportExtendInfoModel>;
            if (reportExtendInfos != null) return reportExtendInfos;
            // Item not found in cache - retrieve it and insert it into the cache
            reportExtendInfos = Api.GetAll();
            AddCacheItem(rawKey, reportExtendInfos);

            return reportExtendInfos;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<ReportExtendInfoModel> Get(out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = string.Concat("ListReportExtendInfos-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var reportExtendInfos = GetCacheItem(rawKey) as List<ReportExtendInfoModel>;
            if (reportExtendInfos != null) return reportExtendInfos;
            // Item not found in cache - retrieve it and insert it into the cache
            reportExtendInfos = Api.GetList(out total, search);
            AddCacheItem(rawKey, reportExtendInfos);
            AddCacheItem(rawKeyTotal, total);

            return reportExtendInfos;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public ReportExtendInfoModel GetById(int? reportExtendInfoId)
        {
            if (reportExtendInfoId < 0) return null;

            var rawKey = string.Concat("ReportExtendInfoByID-", reportExtendInfoId);

            // See if the item is in the cache
            var reportExtendInfo = GetCacheItem(rawKey) as ReportExtendInfoModel;
            if (reportExtendInfo != null) return reportExtendInfo;
            // Item not found in cache - retrieve it and insert it into the cache
            reportExtendInfo = Api.GetById(reportExtendInfoId);
            if (reportExtendInfo != null) AddCacheItem(rawKey, reportExtendInfo);

            return reportExtendInfo;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(ReportExtendInfoModel model)
        {
            var reportExtendInfoId = Api.Save(model);
            if (reportExtendInfoId > 0)
                // Invalidate the cache
                InvalidateCache();
            return reportExtendInfoId;
        }
    }
}