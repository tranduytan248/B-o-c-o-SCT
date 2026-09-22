using System.Collections.Generic;
using System.ComponentModel;
using CenIT.ReportTourism.Biz.Cate.Accommodation;
using CenIT.ReportTourism.Models.Cate.Accommodation;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;
using TSFramework.Core.Utils;

namespace CenIT.ReportTourism.Caches.Cate.Accommodation
{
    [DataObject]
    public class CateAccommodationServiceTypeServiceCache : CacheLayer
    {
        private CateAccommodationServiceTypeServiceBiz _cateAccommodationServiceTypeServiceApi;

        private CateAccommodationServiceTypeServiceBiz Api => _cateAccommodationServiceTypeServiceApi ??
                                                              (_cateAccommodationServiceTypeServiceApi =
                                                                  new CateAccommodationServiceTypeServiceBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "CateAccommodationServiceTypeServicesCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateAccommodationServiceTypeServiceModel> GetAll(int? enterpriseId = null)
        {
            var rawKey = $"AllCateAccommodationServiceTypeServices-{enterpriseId}";
            // See if the item is in the cache
            var cateAccommodationServiceTypeServices =
                GetCacheItem(rawKey) as List<CateAccommodationServiceTypeServiceModel>;
            if (cateAccommodationServiceTypeServices != null) return cateAccommodationServiceTypeServices;
            // Item not found in cache - retrieve it and insert it into the cache
            cateAccommodationServiceTypeServices = Api.GetAll(enterpriseId);
            AddCacheItem(rawKey, cateAccommodationServiceTypeServices);
            return cateAccommodationServiceTypeServices;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateAccommodationServiceTypeServiceModel> Get(int? enterpriseId, out int total,
            SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = string.Concat($"ListCateAccommodationServiceTypeServices-{enterpriseId}-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var cateAccommodationServiceTypeServices =
                GetCacheItem(rawKey) as List<CateAccommodationServiceTypeServiceModel>;
            if (cateAccommodationServiceTypeServices != null) return cateAccommodationServiceTypeServices;
            // Item not found in cache - retrieve it and insert it into the cache
            cateAccommodationServiceTypeServices = Api.Get(enterpriseId, out total, search);
            AddCacheItem(rawKey, cateAccommodationServiceTypeServices);
            AddCacheItem(rawKeyTotal, total);
            return cateAccommodationServiceTypeServices;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateAccommodationServiceTypeServiceModel GetById(int? accommodationTypeServiceId)
        {
            if (accommodationTypeServiceId < 0) return null;
            var rawKey = string.Concat("CateAccommodationServiceTypeServiceByID-", accommodationTypeServiceId);
            // See if the item is in the cache
            var cateAccommodationServiceTypeService = GetCacheItem(rawKey) as CateAccommodationServiceTypeServiceModel;
            if (cateAccommodationServiceTypeService != null) return cateAccommodationServiceTypeService;
            // Item not found in cache - retrieve it and insert it into the cache
            cateAccommodationServiceTypeService = Api.GetById(accommodationTypeServiceId) ??
                                                  new CateAccommodationServiceTypeServiceModel();
            AddCacheItem(rawKey, cateAccommodationServiceTypeService);
            return cateAccommodationServiceTypeService;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int Save(CateAccommodationServiceTypeServiceModel model)
        {
            var accommodationTypeServiceId = Api.Save(model);
            if (accommodationTypeServiceId > 0)
                // Invalidate the cache
                InvalidateCache();
            return accommodationTypeServiceId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CateAccommodationServiceTypeServiceModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }
    }
}