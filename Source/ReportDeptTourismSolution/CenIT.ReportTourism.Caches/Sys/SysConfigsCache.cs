using System.Collections.Generic;
using System.ComponentModel;
using CenIT.ReportTourism.Biz.Sys;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;
using TSFramework.Core.Utils;

namespace CenIT.ReportTourism.Caches.Sys
{
    [DataObject]
    public class SysConfigsCache : CacheLayer
    {
        private SysConfigsBiz _configsApi;

        private SysConfigsBiz Api => _configsApi ?? (_configsApi = new SysConfigsBiz());

        protected override string[] MasterCacheKeyArray =>
            new[] { "SysConfigsCache", "DataImportCache", "CenIT.Application.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysConfigsModel> GetAll()
        {
            const string rawKey = "AllConfigs";
            // See if the item is in the cache
            var configs = GetCacheItem(rawKey) as List<SysConfigsModel>;
            if (configs != null) return configs;
            // Item not found in cache - retrieve it and insert it into the cache
            configs = Api.GetAll();
            AddCacheItem(rawKey, configs);

            return configs;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysConfigsModel> Get(out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = string.Concat("ListMessages-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            // See if the item is in the cache
            var configs = GetCacheItem(rawKey) as List<SysConfigsModel>;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;

            if (configs != null) return configs;
            // Item not found in cache - retrieve it and insert it into the cache
            configs = Api.GetList(out total, search);
            AddCacheItem(rawKey, configs);
            AddCacheItem(rawKeyTotal, total);

            return configs;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysConfigsModel GetById(int configId)
        {
            if (configId < 0) return null;

            var rawKey = string.Concat("SysConfigByID-", configId);

            // See if the item is in the cache
            var config = GetCacheItem(rawKey) as SysConfigsModel;
            if (config != null) return config;
            // Item not found in cache - retrieve it and insert it into the cache
            config = Api.GetById(configId);
            if (config != null) AddCacheItem(rawKey, config);

            return config;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysConfigsModel GetViaKey(string configKey)
        {
            if (string.IsNullOrEmpty(configKey)) return null;

            var rawKey = string.Concat("SysConfigByID-", configKey);

            // See if the item is in the cache
            var config = GetCacheItem(rawKey) as SysConfigsModel;
            if (config != null) return config;
            // Item not found in cache - retrieve it and insert it into the cache
            config = Api.GetViaKey(configKey);
            if (config != null) AddCacheItem(rawKey, config);

            return config;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(SysConfigsModel model)
        {
            var configId = Api.Save(model);
            if (configId > 0)
                // Invalidate the cache
                InvalidateCache();
            return configId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(SysConfigsModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }
    }
}