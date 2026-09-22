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
    public class CateWardCache : CacheLayer
    {
        private CateWardBiz _wardApi;

        private CateWardBiz Api => _wardApi ?? (_wardApi = new CateWardBiz());

        protected override string[] MasterCacheKeyArray => new[] { "WardsCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateWardModel> Get(string provinceIds, out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey =
                $"ListWards-ViaProvince-{provinceIds}-ViaSearch-{objectKey}";
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateWardModel> wards) return wards;
            // Item not found in cache - retrieve it and insert it into the cache
            wards = Api.LoadList(provinceIds, out total, search);
            AddCacheItem(rawKey, wards);
            AddCacheItem(rawKeyTotal, total);

            return wards;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateWardModel GetById(int? wardId)
        {
            if (wardId < 0) return null;

            var rawKey = string.Concat("WardByID-", wardId);

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is CateWardModel ward) return ward;
            // Item not found in cache - retrieve it and insert it into the cache
            ward = Api.GetById(wardId);
            if (ward != null) AddCacheItem(rawKey, ward);

            return ward;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(CateWardModel model)
        {
            var wardId = Api.Save(model);
            if (wardId > 0)
                // Invalidate the cache
                InvalidateCache();
            return wardId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CateWardModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CateWardModel> GetAll(int? provinceId = null)
        {
            var rawKey = $"AllWard-{provinceId}";
            if (GetCacheItem(rawKey) is List<CateWardModel> listWards) return listWards;
            // Item not found in cache - retrieve it and insert it into the cache
            listWards = Api.GetAll($"{provinceId}");
            AddCacheItem(rawKey, listWards);

            return listWards;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CateWardModel> GetByProvinceId(int? provinceId, out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);

            var rawKey = string.Concat("AllWardsByProvinceId-", provinceId, objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;

            if (GetCacheItem(rawKey) is List<CateWardModel> listWards) return listWards;
            // Item not found in cache - retrieve it and insert it into the cache
            listWards = Api.GetByProvinceId(provinceId, out total, search);
            if (listWards == null) return null;
            AddCacheItem(rawKey, listWards);
            AddCacheItem(rawKeyTotal, total);

            return listWards;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CateWardModel> GetByProvinceId(int? provinceId)
        {

            var rawKey = string.Concat("AllWardsByProvinceId-", provinceId);

            if (GetCacheItem(rawKey) is List<CateWardModel> listWards) return listWards;
            // Item not found in cache - retrieve it and insert it into the cache
            listWards = Api.GetByProvinceId(provinceId, out int total, null);
            if (listWards == null) return null;
            AddCacheItem(rawKey, listWards);

            return listWards;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CateWardModel> GetByStreetId(int? streetId, out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);

            var rawKey = string.Concat("AllWardsByStreetId-", streetId, objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;

            if (GetCacheItem(rawKey) is List<CateWardModel> listWards) return listWards;
            // Item not found in cache - retrieve it and insert it into the cache
            listWards = Api.GetByStreetId(streetId, out total, search);
            if (listWards == null) return null;
            AddCacheItem(rawKey, listWards);
            AddCacheItem(rawKeyTotal, total);

            return listWards;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CateWardModel> GetByStreetId(int? streetId)
        {
            var rawKey = string.Concat("AllWardsByStreetId-", streetId);

            if (GetCacheItem(rawKey) is List<CateWardModel> listWards) return listWards;
            // Item not found in cache - retrieve it and insert it into the cache
            listWards = Api.GetByStreetId(streetId);
            if (listWards == null) return null;
            AddCacheItem(rawKey, listWards);

            return listWards;
        }
    }
}