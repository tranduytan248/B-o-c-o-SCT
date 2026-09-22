using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using CenIT.ReportTourism.Biz.Cate;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;
using TSFramework.Core.Utils;

namespace CenIT.ReportTourism.Caches.Cate
{
    [DataObject]
    public class CateStreetCache : CacheLayer
    {
        private CateStreetBiz _streetApi;

        private CateStreetBiz Api => _streetApi ?? (_streetApi = new CateStreetBiz());

        protected override string[] MasterCacheKeyArray => new[] { "StreetsCache", "WardsCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateStreetModel> Get(out int total, int? provinceId, int? wardId, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = $"ListStreets-ByProvinceId-{provinceId}-ByWardId-{wardId}-ViaSearch-{objectKey}";
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateStreetModel> streets) return streets;
            // Item not found in cache - retrieve it and insert it into the cache
            streets = Api.LoadList(out total, provinceId, wardId, search);
            AddCacheItem(rawKey, streets);
            AddCacheItem(rawKeyTotal, total);

            return streets;
        }

        public List<CateStreetModel> GetByWard(int idWard, out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = $"ListStreetsByWard-{idWard}-ViaSearch-{objectKey}";
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateStreetModel> streets) return streets;
            // Item not found in cache - retrieve it and insert it into the cache
            streets = Api.GetByWard(idWard, out total, search);
            AddCacheItem(rawKey, streets ?? new List<CateStreetModel>());
            AddCacheItem(rawKeyTotal, total);

            return streets;
        }

        public List<CateStreetModel> GetByWard(int? idWard)
        {
            var rawKey = $"ListStreetsByWard-{idWard}";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateStreetModel> streets) return streets;
            // Item not found in cache - retrieve it and insert it into the cache
            streets = Api.GetByWard(idWard);
            AddCacheItem(rawKey, streets ?? new List<CateStreetModel>());

            return streets;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateStreetModel GetById(int? streetId)
        {
            if (streetId < 0) return null;

            var rawKey = $"StreetByID-{streetId}";

            // See if the item is in the cache
            if (GetCacheItem(rawKey) is CateStreetModel street) return street;
            // Item not found in cache - retrieve it and insert it into the cache
            street = Api.GetById(streetId);
            if (street != null) AddCacheItem(rawKey, street);

            return street;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(CateStreetModel model, DataTable table)
        {
            var streetId = Api.Save(model, table);
            if (streetId > 0)
                // Invalidate the cache
                InvalidateCache();
            return streetId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CateStreetModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CateStreetModel> GetAll(int? provinceId, int? wardId)
        {
            var rawKey = $"AllStreets-ByProvinceId-{provinceId}-ByWardId-{wardId}";
            if (GetCacheItem(rawKey) is List<CateStreetModel> streets) return streets;
            // Item not found in cache - retrieve it and insert it into the cache
            streets = Api.GetAll(provinceId, wardId);
            AddCacheItem(rawKey, streets);

            return streets;
        }
    }
}