using System.Collections.Generic;
using System.ComponentModel;
using CenIT.ReportTourism.Biz.Sys;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;

namespace CenIT.ReportTourism.Caches.Sys
{
    [DataObject]
    public class SysFunctionActionCache : CacheLayer
    {
        private SysFunctionActionBiz _functionActionApi;

        private SysFunctionActionBiz Api => _functionActionApi ?? (_functionActionApi = new SysFunctionActionBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "SysFunctionsCache", "SysPermissionsCache", "SysMenusCache", "CenIT.Application.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysFunctionActionModel> GetAll()
        {
            const string rawKey = "AllFunctionActions";
            // See if the item is in the cache
            var functionActions = GetCacheItem(rawKey) as List<SysFunctionActionModel>;
            if (functionActions != null) return functionActions;
            // Item not found in cache - retrieve it and insert it into the cache
            functionActions = Api.GetAll();
            AddCacheItem(rawKey, functionActions);

            return functionActions;
        }
    }
}