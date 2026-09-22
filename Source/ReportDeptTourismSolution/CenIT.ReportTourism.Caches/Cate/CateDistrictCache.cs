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
    public class CateDistrictCache : CacheLayer
    {
        private CateDistrictBiz _districtApi;

        private CateDistrictBiz Api => _districtApi ?? (_districtApi = new CateDistrictBiz());

        protected override string[] MasterCacheKeyArray =>
            new[] { "DistrictsCache", "ProvincesCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateDistrictModel> GetAll(int? provinceId = null)
        {
            var rawKey = $"AllEnterprises-{provinceId}-";
            // See if the item is in the cache
            var districts = GetCacheItem(rawKey) as List<CateDistrictModel>;
            if (districts != null) return districts;
            // Item not found in cache - retrieve it and insert it into the cache
            districts = Api.GetAll(provinceId);
            AddCacheItem(rawKey, districts);

            return districts;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateDistrictModel> Get(int? provinceId, string provincesIds, out int total,
            SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = string.Concat($"ListDistrics-{provinceId}-{provincesIds}-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var districts = GetCacheItem(rawKey) as List<CateDistrictModel>;
            if (districts != null) return districts;
            // Item not found in cache - retrieve it and insert it into the cache
            districts = Api.Get(provinceId, provincesIds, out total, search);
            AddCacheItem(rawKey, districts);
            AddCacheItem(rawKeyTotal, total);
            return districts;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateDistrictModel GetById(int? districtId)
        {
            if (districtId < 0) return null;

            var rawKey = $"DistrictByID-{districtId}";

            // See if the item is in the cache
            var district = GetCacheItem(rawKey) as CateDistrictModel;
            if (district != null) return district;
            // Item not found in cache - retrieve it and insert it into the cache
            district = Api.GetById(districtId);
            if (district != null) AddCacheItem(rawKey, district);

            return district;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(CateDistrictModel model)
        {
            var districtId = Api.Save(model);
            if (districtId > 0)
                // Invalidate the cache
                InvalidateCache();
            return districtId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CateDistrictModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateDistrictModel> GetByProvinceCode(string provinceCode, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            if (string.IsNullOrEmpty(provinceCode)) return null;
            var rawKey = $"LisDistrictsByProvinceCode-{provinceCode}-{objectKey}";

            // See if the item is in the cache
            var districts = GetCacheItem(rawKey) as List<CateDistrictModel>;
            if (districts != null) return districts;
            // Item not found in cache - retrieve it and insert it into the cache
            districts = Api.GetByProvinceCode(provinceCode, search);
            AddCacheItem(rawKey, districts);

            return districts;
        }

        public List<CateDistrictModel> GetByDistricts(int? provinceId, out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = $"ListByDistrictID-{provinceId}-ViaSearch-{objectKey}";
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var districts = GetCacheItem(rawKey) as List<CateDistrictModel>;
            if (districts != null) return districts;
            // Item not found in cache - retrieve it and insert it into the cache
            districts = Api.GetByProvinceID(provinceId, out total, search);
            AddCacheItem(rawKey, districts ?? new List<CateDistrictModel>());
            AddCacheItem(rawKeyTotal, total);

            return districts;
        }

        public List<CateDistrictModel> GetByProvinceID(int? provinceId)
        {
            var rawKey = $"ListByDistrictID-{provinceId}";
            // See if the item is in the cache
            var districts = GetCacheItem(rawKey) as List<CateDistrictModel>;
            if (districts != null) return districts;
            // Item not found in cache - retrieve it and insert it into the cache
            districts = Api.GetByProvinceID(provinceId);
            AddCacheItem(rawKey, districts ?? new List<CateDistrictModel>());

            return districts;
        }
    }
}