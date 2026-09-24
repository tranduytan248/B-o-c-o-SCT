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
    public class CateBusinessProductCache : CacheLayer
    {
        private CateBusinessProductBiz _businessProductApi;
        private CateBusinessProductBiz Api => _businessProductApi ?? (_businessProductApi = new CateBusinessProductBiz());
        protected override string[] MasterCacheKeyArray => new[] { "BusinessProductsCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CateBusinessProductModel> GetViaEnterprise(int enterpriseId)
        {
            var rawKey = "BusinessProductsForEnterprise-" + enterpriseId;
            if (GetCacheItem(rawKey) is List<CateBusinessProductModel> products) return products;
            products = Api.GetViaEnterprise(enterpriseId) ?? new List<CateBusinessProductModel>();
            AddCacheItem(rawKey, products);
            return products;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CateBusinessProductModel> GetByPrefix(int enterpriseId, string productCodePrefix)
        {
            var rawKey = "BusinessProductsByPrefix-" + enterpriseId + "-" + productCodePrefix;
            if (GetCacheItem(rawKey) is List<CateBusinessProductModel> products) return products;
            products = Api.GetByPrefix(enterpriseId, productCodePrefix) ?? new List<CateBusinessProductModel>();
            AddCacheItem(rawKey, products);
            return products;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CateBusinessProductModel> GetUnconfiguredByPrefix(int enterpriseId, string productCodePrefix)
        {
            var rawKey = "UnconfiguredBusinessProductsByPrefix-" + enterpriseId + "-" + productCodePrefix;
            if (GetCacheItem(rawKey) is List<CateBusinessProductModel> products) return products;
            products = Api.GetUnconfiguredByPrefix(enterpriseId, productCodePrefix)
                       ?? new List<CateBusinessProductModel>();
            AddCacheItem(rawKey, products);
            return products;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateBusinessProductModel> Get(out int total, int? industryId = null, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = $"ListBusinessProduct--{industryId}-{objectKey}";
            var rawKeyTotal = $"{rawKey}-Total";
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateBusinessProductModel> businessProduct) return businessProduct;
            // Item not found in cache - retrieve it and insert it into the cache
            businessProduct = Api.Get(out total, industryId, search);
            AddCacheItem(rawKey, businessProduct);
            AddCacheItem(rawKeyTotal, total);
            return businessProduct;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateBusinessProductModel> GetAll(int? industryId = null)
        {
            int total;
            var rawKey = $"AllBusinessProduct-{industryId}";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateBusinessProductModel> businessProduct) return businessProduct;
            // Item not found in cache - retrieve it and insert it into the cache
            businessProduct = Api.Get(out total, industryId, null);
            AddCacheItem(rawKey, businessProduct);
            return businessProduct;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(CateBusinessProductModel model)
        {
            var productId = Api.Save(model);
            if (productId > 0)
                // Invalidate the cache
                InvalidateCache();
            return productId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateBusinessProductModel GetById(int? businessProductid)
        {
            if (businessProductid < 0) return null;
            var rawKey = string.Concat("BusinessProductByID-", businessProductid);
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is CateBusinessProductModel businessProduct) return businessProduct;
            // Item not found in cache - retrieve it and insert it into the cache
            businessProduct = Api.GetById(businessProductid) ?? new CateBusinessProductModel();
            AddCacheItem(rawKey, businessProduct);
            return businessProduct;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CateBusinessProductModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }
    }
}
