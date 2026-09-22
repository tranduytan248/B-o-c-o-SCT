using System.Collections.Generic;
using System.ComponentModel;
using CenIT.ReportTourism.Biz.Sys;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;
using TSFramework.Core.Utils;

namespace CenIT.ReportTourism.Caches.Sys
{
    [DataObject]
    public class SysContentPanelCache : CacheLayer
    {
        private SysContentPanelBiz _contentPanelApi;

        private SysContentPanelBiz Api => _contentPanelApi ?? (_contentPanelApi = new SysContentPanelBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "SysContentPanelsCache", "SysLayoutCache", "CenIT.Application.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysContentPanelModel> GetAll()
        {
            const string rawKey = "AllContentPanels";
            // See if the item is in the cache
            var contentPanels = GetCacheItem(rawKey) as List<SysContentPanelModel>;
            if (contentPanels != null) return contentPanels;
            // Item not found in cache - retrieve it and insert it into the cache
            contentPanels = Api.GetAll();
            AddCacheItem(rawKey, contentPanels);

            return contentPanels;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysContentPanelModel> Get(out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);

            var rawKey = string.Concat("ListContentPanels-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var contentPanels = GetCacheItem(rawKey) as List<SysContentPanelModel>;
            if (contentPanels != null) return contentPanels;
            // Item not found in cache - retrieve it and insert it into the cache
            contentPanels = Api.GetList(out total, search);
            AddCacheItem(rawKey, contentPanels);
            AddCacheItem(rawKeyTotal, total);

            return contentPanels;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysContentPanelModel GetById(int contentPanelId)
        {
            if (contentPanelId < 0) return null;

            var rawKey = string.Concat("ContentPanelByID-", contentPanelId);

            // See if the item is in the cache
            var contentPanel = GetCacheItem(rawKey) as SysContentPanelModel;
            if (contentPanel != null) return contentPanel;
            // Item not found in cache - retrieve it and insert it into the cache
            contentPanel = Api.GetById(contentPanelId) ?? new SysContentPanelModel();
            AddCacheItem(rawKey, contentPanel);

            return contentPanel;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(SysContentPanelModel model)
        {
            var contentPanelId = Api.Save(model);
            if (contentPanelId > 0)
                // Invalidate the cache
                InvalidateCache();
            return contentPanelId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(SysContentPanelModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SysContentPanelModel> GetByLayoutName(string layoutName)
        {
            var rawKey = string.Concat("AllContentPanelsByLayoutName-", layoutName);
            // See if the item is in the cache
            var contentPanels = GetCacheItem(rawKey) as List<SysContentPanelModel>;
            if (contentPanels != null) return contentPanels;
            // Item not found in cache - retrieve it and insert it into the cache
            contentPanels = Api.GetByLayoutName(layoutName);
            AddCacheItem(rawKey, contentPanels);

            return contentPanels;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SysContentPanelModel> GetByLayoutId(int layoutId)
        {
            var rawKey = string.Concat("AllContentPanelsByLayoutId-", layoutId);
            // See if the item is in the cache
            var contentPanels = GetCacheItem(rawKey) as List<SysContentPanelModel>;
            if (contentPanels != null) return contentPanels;
            // Item not found in cache - retrieve it and insert it into the cache
            contentPanels = Api.GetByLayoutId(layoutId);
            AddCacheItem(rawKey, contentPanels);

            return contentPanels;
        }
    }
}