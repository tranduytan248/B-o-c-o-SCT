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
    public class CateEconomicSectorCache : CacheLayer
    {
        private CateEconomicSectorBiz _economicSectorApi;
        private CateEconomicSectorBiz Api => _economicSectorApi ?? (_economicSectorApi = new CateEconomicSectorBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "EconomicSectorCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateEconomicSectorModel> Get(out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = string.Concat("ListEconomicSector-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateEconomicSectorModel> economicSector) return economicSector;
            // Item not found in cache - retrieve it and insert it into the cache
            economicSector = Api.Get(out total, search);
            AddCacheItem(rawKey, economicSector);
            AddCacheItem(rawKeyTotal, total);
            return economicSector;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateEconomicSectorModel> GetAll()
        {
            int total;
            var rawKey = "AllEconomicSector";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateEconomicSectorModel> economicSector) return economicSector;
            // Item not found in cache - retrieve it and insert it into the cache
            economicSector = Api.Get(out total, null);
            AddCacheItem(rawKey, economicSector);
            return economicSector;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(CateEconomicSectorModel model)
        {
            var economicSectorid = Api.Save(model);
            if (economicSectorid > 0)
                // Invalidate the cache
                InvalidateCache();
            return economicSectorid;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateEconomicSectorModel GetById(int? economicSectorid)
        {
            if (economicSectorid < 0) return null;
            var rawKey = string.Concat("EconomicSectorByID-", economicSectorid);
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is CateEconomicSectorModel economicSector) return economicSector;
            // Item not found in cache - retrieve it and insert it into the cache
            economicSector = Api.GetById(economicSectorid) ?? new CateEconomicSectorModel();
            AddCacheItem(rawKey, economicSector);
            return economicSector;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CateEconomicSectorModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }
    }
}