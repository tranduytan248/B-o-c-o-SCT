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
    public class CateProvinceCache : CacheLayer
    {
        private CateProvinceBiz _provinceApi;

        private CateProvinceBiz Api => _provinceApi ?? (_provinceApi = new CateProvinceBiz());

        protected override string[] MasterCacheKeyArray => new[] { "ProvincesCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateProvinceModel> GetAll()
        {
            const string rawKey = "AllProvinces";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateProvinceModel> provinces) return provinces;
            // Item not found in cache - retrieve it and insert it into the cache
            provinces = Api.GetAll();
            AddCacheItem(rawKey, provinces);

            return provinces;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateProvinceModel> Get(out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = string.Concat("ListProvinces-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateProvinceModel> provinces) return provinces;
            // Item not found in cache - retrieve it and insert it into the cache
            provinces = Api.GetList(out total, search);
            AddCacheItem(rawKey, provinces);
            AddCacheItem(rawKeyTotal, total);

            return provinces;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateProvinceModel GetById(int? provinceId)
        {
            if (provinceId < 0) return null;

            var rawKey = string.Concat("ProvinceByID-", provinceId);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is CateProvinceModel province) return province;
            // Item not found in cache - retrieve it and insert it into the cache
            province = Api.GetById(provinceId);
            if (province != null) AddCacheItem(rawKey, province);

            return province;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(CateProvinceModel model)
        {
            var provinceId = Api.Save(model);
            if (provinceId > 0)
                // Invalidate the cache
                InvalidateCache();
            return provinceId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CateProvinceModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateProvinceModel GetViaWard(int? wardId)
        {
            if (wardId < 0) return null;

            var rawKey = string.Concat("ProvinceByViaWard-", wardId);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is CateProvinceModel province) return province;
            // Item not found in cache - retrieve it and insert it into the cache
            province = Api.GetViaWard(wardId);
            if (province != null) AddCacheItem(rawKey, province);

            return province;
        }
    }
}