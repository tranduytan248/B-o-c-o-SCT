using System.Collections.Generic;
using System.ComponentModel;
using CenIT.ReportTourism.Biz.Cate;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;
using TSFramework.Core.Utils;

namespace CenIT.ReportTourism.Caches.Cate
{
    [DataObject]
    public class CateEnterpriseStatusCache : CacheLayer
    {
        private CateEnterpriseStatusBiz _enterpriseStatusApi;
        private CateEnterpriseStatusBiz Api => _enterpriseStatusApi ?? (_enterpriseStatusApi = new CateEnterpriseStatusBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "EnterpriseStatusCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateEnterpriseStatusModel> Get(out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = string.Concat("ListEnterpriseStatus-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateEnterpriseStatusModel> enterpriseStatus) return enterpriseStatus;
            // Item not found in cache - retrieve it and insert it into the cache
            enterpriseStatus = Api.Get(out total, search);
            AddCacheItem(rawKey, enterpriseStatus);
            AddCacheItem(rawKeyTotal, total);
            return enterpriseStatus;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateEnterpriseStatusModel> GetAll()
        {
            int total;
            var rawKey = "AllEnterpriseStatus";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateEnterpriseStatusModel> enterpriseStatus) return enterpriseStatus;
            // Item not found in cache - retrieve it and insert it into the cache
            enterpriseStatus = Api.Get(out total, null);
            AddCacheItem(rawKey, enterpriseStatus);
            return enterpriseStatus;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(CateEnterpriseStatusModel model)
        {
            var enterpriseStatusId = Api.Save(model);
            if (enterpriseStatusId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseStatusId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateEnterpriseStatusModel GetById(int? enterpriseStatusid)
        {
            if (enterpriseStatusid < 0) return null;
            var rawKey = string.Concat("EnterpriseStatusByID-", enterpriseStatusid);
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is CateEnterpriseStatusModel enterpriseStatus) return enterpriseStatus;
            // Item not found in cache - retrieve it and insert it into the cache
            enterpriseStatus = Api.GetById(enterpriseStatusid) ?? new CateEnterpriseStatusModel();
            AddCacheItem(rawKey, enterpriseStatus);
            return enterpriseStatus;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CateEnterpriseStatusModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }
    }
}