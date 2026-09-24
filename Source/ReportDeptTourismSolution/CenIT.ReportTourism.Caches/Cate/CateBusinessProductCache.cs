using System.Collections.Generic;
using System.ComponentModel;
using CenIT.ReportTourism.Biz.Cate;
using CenIT.ReportTourism.Models.Cate;
using TSFramework.Core.Members.Caching;

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
    }
}
