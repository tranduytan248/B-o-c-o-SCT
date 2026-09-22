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
    public class CateNationalCache : CacheLayer
    {
        private CateNationBiz _nationalApi;
        private CateNationBiz Api => _nationalApi ?? (_nationalApi = new CateNationBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "NationalCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateNationalModel> Get(out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = string.Concat("ListNational-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateNationalModel> national) return national;
            // Item not found in cache - retrieve it and insert it into the cache
            national = Api.Get(out total, search);
            AddCacheItem(rawKey, national);
            AddCacheItem(rawKeyTotal, total);
            return national;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateNationalModel> GetAll()
        {
            int total;
            var rawKey = "AllNational";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateNationalModel> national) return national;
            // Item not found in cache - retrieve it and insert it into the cache
            national = Api.Get(out total, null);
            AddCacheItem(rawKey, national);
            return national;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(CateNationalModel model)
        {
            var nationaID = Api.Save(model);
            if (nationaID > 0)
                // Invalidate the cache
                InvalidateCache();
            return nationaID;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateNationalModel GetById(int? nationalid)
        {
            if (nationalid < 0) return null;
            var rawKey = string.Concat("EnterpriseByID-", nationalid);
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is CateNationalModel national) return national;
            // Item not found in cache - retrieve it and insert it into the cache
            national = Api.GetById(nationalid) ?? new CateNationalModel();
            AddCacheItem(rawKey, national);
            return national;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CateNationalModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }
    }
}