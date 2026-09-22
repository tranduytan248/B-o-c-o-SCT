using System.Collections.Generic;
using System.ComponentModel;
using CenIT.ReportTourism.Biz.Sys;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;
using TSFramework.Core.Utils;

namespace CenIT.ReportTourism.Caches.Sys
{
    [DataObject]
    public class SysLayoutCache : CacheLayer
    {
        private SysLayoutBiz _layoutApi;

        private SysLayoutBiz Api => _layoutApi ?? (_layoutApi = new SysLayoutBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "SysLayoutsCache", "SysContentPanelsCache", "CenIT.Application.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysLayoutModel> GetAll()
        {
            const string rawKey = "AllLayouts";
            // See if the item is in the cache
            var layouts = GetCacheItem(rawKey) as List<SysLayoutModel>;
            if (layouts != null) return layouts;
            // Item not found in cache - retrieve it and insert it into the cache
            layouts = Api.GetAll();
            AddCacheItem(rawKey, layouts);

            return layouts;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysLayoutModel> Get(out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);

            var rawKey = string.Concat("ListLayouts-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var layouts = GetCacheItem(rawKey) as List<SysLayoutModel>;
            if (layouts != null) return layouts;
            // Item not found in cache - retrieve it and insert it into the cache
            layouts = Api.GetList(out total, search);
            AddCacheItem(rawKey, layouts);
            AddCacheItem(rawKeyTotal, total);

            return layouts;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysLayoutModel GetById(int layoutId)
        {
            if (layoutId < 0) return null;

            var rawKey = string.Concat("LayoutByID-", layoutId);

            // See if the item is in the cache
            var layout = GetCacheItem(rawKey) as SysLayoutModel;
            if (layout != null) return layout;
            // Item not found in cache - retrieve it and insert it into the cache
            layout = Api.GetById(layoutId) ?? new SysLayoutModel();
            AddCacheItem(rawKey, layout);

            return layout;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysLayoutModel GetByName(string layoutName)
        {
            if (string.IsNullOrEmpty(layoutName)) return null;

            var rawKey = string.Concat("LayoutByName-", layoutName);

            // See if the item is in the cache
            var layout = GetCacheItem(rawKey) as SysLayoutModel;
            if (layout != null) return layout;
            // Item not found in cache - retrieve it and insert it into the cache
            layout = Api.GetByName(layoutName) ?? new SysLayoutModel();
            AddCacheItem(rawKey, layout);

            return layout;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(SysLayoutModel model)
        {
            var layoutId = Api.Save(model);
            if (layoutId > 0)
                // Invalidate the cache
                InvalidateCache();
            return layoutId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(SysLayoutModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public bool Activated(SysLayoutModel model)
        {
            var isSuccess = Api.Activated(model);
            if (isSuccess)
                // Invalidate the cache
                InvalidateCache();
            return isSuccess;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysLayoutModel GetActivatedLayout()
        {
            const string rawKey = "ActivatedLayout";

            // See if the item is in the cache
            var layout = GetCacheItem(rawKey) as SysLayoutModel;
            if (layout != null) return layout;
            // Item not found in cache - retrieve it and insert it into the cache
            layout = Api.GetActivatedLayout() ?? new SysLayoutModel();
            AddCacheItem(rawKey, layout);

            return layout;
        }
    }
}