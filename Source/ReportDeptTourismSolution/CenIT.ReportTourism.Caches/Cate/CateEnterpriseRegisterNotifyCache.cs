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
    public class CateEnterpriseRegisterNotifyCache : CacheLayer
    {
        private CateEnterpriseRegisterNotifyBiz _enterpriseApi;

        private CateEnterpriseRegisterNotifyBiz Api =>
            _enterpriseApi ?? (_enterpriseApi = new CateEnterpriseRegisterNotifyBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "EnterpriseRegisterNotifysCache", "EnterpriseCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateEnterpriseRegisterNotifyModel> GetAll()
        {
            var rawKey = "AllEnterpriseRegisterNotifys";
            // See if the item is in the cache
            var enterprises = GetCacheItem(rawKey) as List<CateEnterpriseRegisterNotifyModel>;
            if (enterprises != null) return enterprises;
            // Item not found in cache - retrieve it and insert it into the cache
            enterprises = Api.GetAll();
            AddCacheItem(rawKey, enterprises);
            return enterprises;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateEnterpriseRegisterNotifyModel> Get(out int total,
            SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = string.Concat("ListEnterpriseRegisterNotifysForEmp-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var enterprises = GetCacheItem(rawKey) as List<CateEnterpriseRegisterNotifyModel>;
            if (enterprises != null) return enterprises;
            // Item not found in cache - retrieve it and insert it into the cache
            enterprises = Api.Get(out total, search);
            AddCacheItem(rawKey, enterprises);
            AddCacheItem(rawKeyTotal, total);
            return enterprises;
        }


        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateEnterpriseRegisterNotifyModel GetById(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("EnterpriseRegisterNotifyByID-", enterpriseId);
            // See if the item is in the cache
            var enterprise = GetCacheItem(rawKey) as CateEnterpriseRegisterNotifyModel;
            if (enterprise != null) return enterprise;
            // Item not found in cache - retrieve it and insert it into the cache
            enterprise = Api.GetById(enterpriseId) ?? new CateEnterpriseRegisterNotifyModel();
            AddCacheItem(rawKey, enterprise);
            return enterprise;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int Save(CateEnterpriseRegisterNotifyModel model)
        {
            var enterpriseId = Api.Save(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }
    }
}