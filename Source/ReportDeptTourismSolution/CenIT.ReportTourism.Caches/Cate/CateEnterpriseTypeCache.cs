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
    public class CateEnterpriseTypeCache : CacheLayer
    {
        private CateEnterpriseTypeBiz _enterpriseTypeApi;
        private CateEnterpriseTypeBiz Api => _enterpriseTypeApi ?? (_enterpriseTypeApi = new CateEnterpriseTypeBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "EnterpriseTypeCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateEnterpriseTypeModel> Get(out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = string.Concat("ListEnterpriseType-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateEnterpriseTypeModel> enterpriseType) return enterpriseType;
            // Item not found in cache - retrieve it and insert it into the cache
            enterpriseType = Api.Get(out total, search);
            AddCacheItem(rawKey, enterpriseType);
            AddCacheItem(rawKeyTotal, total);
            return enterpriseType;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateEnterpriseTypeModel> GetAll()
        {
            int total;
            var rawKey = "AllEnterpriseType";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateEnterpriseTypeModel> enterpriseType) return enterpriseType;
            // Item not found in cache - retrieve it and insert it into the cache
            enterpriseType = Api.Get(out total, null);
            AddCacheItem(rawKey, enterpriseType);
            return enterpriseType;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(CateEnterpriseTypeModel model)
        {
            var enterpriseTypeId = Api.Save(model);
            if (enterpriseTypeId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseTypeId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateEnterpriseTypeModel GetById(int? enterpriseTypeid)
        {
            if (enterpriseTypeid < 0) return null;
            var rawKey = string.Concat("EnterpriseTypeByID-", enterpriseTypeid);
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is CateEnterpriseTypeModel enterpriseType) return enterpriseType;
            // Item not found in cache - retrieve it and insert it into the cache
            enterpriseType = Api.GetById(enterpriseTypeid) ?? new CateEnterpriseTypeModel();
            AddCacheItem(rawKey, enterpriseType);
            return enterpriseType;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CateEnterpriseTypeModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }
    }
}