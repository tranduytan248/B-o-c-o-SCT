using System.Collections.Generic;
using System.ComponentModel;
using CenIT.ReportTourism.Biz.Cate.TouristAttraction;
using CenIT.ReportTourism.Models.Cate.TouristAttraction;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;
using TSFramework.Core.Utils;

namespace CenIT.ReportTourism.Caches.Cate.TouristAttraction
{
    [DataObject]
    public class CateTouristAttractionTypeServiceCache : CacheLayer
    {
        private CateTouristAttractionTypeServiceBiz _cateTouristAttractionTypeServiceApi;

        private CateTouristAttractionTypeServiceBiz Api => _cateTouristAttractionTypeServiceApi ??
                                                           (_cateTouristAttractionTypeServiceApi =
                                                               new CateTouristAttractionTypeServiceBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "CateTouristAttractionTypeServicesCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateTouristAttractionTypeServiceModel> GetAll(int? enterpriseId = null)
        {
            var rawKey = $"AllCateTouristAttractionTypeServices-{enterpriseId}";
            // See if the item is in the cache
            var cateTouristAttractionTypeServices = GetCacheItem(rawKey) as List<CateTouristAttractionTypeServiceModel>;
            if (cateTouristAttractionTypeServices != null) return cateTouristAttractionTypeServices;
            // Item not found in cache - retrieve it and insert it into the cache
            cateTouristAttractionTypeServices = Api.GetAll(enterpriseId);
            AddCacheItem(rawKey, cateTouristAttractionTypeServices);
            return cateTouristAttractionTypeServices;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateTouristAttractionTypeServiceModel> Get(int? enterpriseId, out int total,
            SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = string.Concat($"ListCateTouristAttractionTypeServices-{enterpriseId}-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var cateTouristAttractionTypeServices = GetCacheItem(rawKey) as List<CateTouristAttractionTypeServiceModel>;
            if (cateTouristAttractionTypeServices != null) return cateTouristAttractionTypeServices;
            // Item not found in cache - retrieve it and insert it into the cache
            cateTouristAttractionTypeServices = Api.Get(enterpriseId, out total, search);
            AddCacheItem(rawKey, cateTouristAttractionTypeServices);
            AddCacheItem(rawKeyTotal, total);
            return cateTouristAttractionTypeServices;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateTouristAttractionTypeServiceModel GetById(int? touristAttractionTypeServiceId)
        {
            if (touristAttractionTypeServiceId < 0) return null;
            var rawKey = string.Concat("CateTouristAttractionTypeServiceByID-", touristAttractionTypeServiceId);
            // See if the item is in the cache
            var cateTouristAttractionTypeService = GetCacheItem(rawKey) as CateTouristAttractionTypeServiceModel;
            if (cateTouristAttractionTypeService != null) return cateTouristAttractionTypeService;
            // Item not found in cache - retrieve it and insert it into the cache
            cateTouristAttractionTypeService = Api.GetById(touristAttractionTypeServiceId) ??
                                               new CateTouristAttractionTypeServiceModel();
            AddCacheItem(rawKey, cateTouristAttractionTypeService);
            return cateTouristAttractionTypeService;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int Save(CateTouristAttractionTypeServiceModel model)
        {
            var touristAttractionTypeServiceId = Api.Save(model);
            if (touristAttractionTypeServiceId > 0)
                // Invalidate the cache
                InvalidateCache();
            return touristAttractionTypeServiceId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CateTouristAttractionTypeServiceModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }
    }
}