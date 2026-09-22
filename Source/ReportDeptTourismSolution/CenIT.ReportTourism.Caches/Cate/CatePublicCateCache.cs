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
    public class CatePublicCateCache : CacheLayer
    {
        private CatePublicCateBiz _publicCateApi;
        private CatePublicCateBiz Api => _publicCateApi ?? (_publicCateApi = new CatePublicCateBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "PublicCatesCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CatePublicCateModel> GetAll(int? cateTypeId = null)
        {
            var rawKey = $"AllPublicCates-{cateTypeId}";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CatePublicCateModel> publicCates) return publicCates;
            // Item not found in cache - retrieve it and insert it into the cache
            publicCates = Api.GetAll(cateTypeId);
            AddCacheItem(rawKey, publicCates);
            return publicCates;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CatePublicCateModel> Get(string cateTypeIds, out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = string.Concat($"ListPublicCates-{cateTypeIds}-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CatePublicCateModel> publicCates) return publicCates;
            // Item not found in cache - retrieve it and insert it into the cache
            publicCates = Api.Get(cateTypeIds, out total, search);
            AddCacheItem(rawKey, publicCates);
            AddCacheItem(rawKeyTotal, total);
            return publicCates;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CatePublicCateModel GetById(long? publicCateId)
        {
            if (publicCateId < 0) return null;
            var rawKey = string.Concat("PublicCateByID-", publicCateId);
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is CatePublicCateModel publicCate) return publicCate;
            // Item not found in cache - retrieve it and insert it into the cache
            publicCate = Api.GetById(publicCateId) ?? new CatePublicCateModel();
            AddCacheItem(rawKey, publicCate);
            return publicCate;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int Save(CatePublicCateModel model)
        {
            var publicCateId = Api.Save(model);
            if (publicCateId > 0)
                // Invalidate the cache
                InvalidateCache();
            return publicCateId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CatePublicCateModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CatePublicCateModel> GetChildsViaId(string cateTypeId = null)
        {
            int iTotal;
            var rawKey = $"AllPublicCatesViaId-{cateTypeId}";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CatePublicCateModel> publicCates) return publicCates;
            // Item not found in cache - retrieve it and insert it into the cache
            publicCates = Api.Get(cateTypeId, out iTotal, null);
            AddCacheItem(rawKey, publicCates);
            return publicCates;
        }
    }
}