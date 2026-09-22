using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using CenIT.ReportTourism.Biz.Sys;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;
using TSFramework.Core.Utils;

namespace CenIT.ReportTourism.Caches.Sys
{
    [DataObject]
    public class SysFunctionCache : CacheLayer
    {
        private SysFunctionBiz _functionApi;

        private SysFunctionBiz Api => _functionApi ?? (_functionApi = new SysFunctionBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "SysFunctionsCache", "SysPermissionsCache", "SysMenusCache", "SysRolesCache", "CenIT.Application.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysFunctionModel> GetAll()
        {
            const string rawKey = "AllFunctions";
            // See if the item is in the cache
            var functions = GetCacheItem(rawKey) as List<SysFunctionModel>;
            if (functions != null) return functions;
            // Item not found in cache - retrieve it and insert it into the cache
            functions = Api.GetAll();
            AddCacheItem(rawKey, functions);

            return functions;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysFunctionModel> Get(out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);

            var rawKey = string.Concat("ListFunctions-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var functions = GetCacheItem(rawKey) as List<SysFunctionModel>;
            if (functions != null) return functions;
            // Item not found in cache - retrieve it and insert it into the cache
            functions = Api.GetList(out total, search);
            AddCacheItem(rawKey, functions);
            AddCacheItem(rawKeyTotal, total);

            return functions;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysFunctionModel GetById(int functionId)
        {
            if (functionId < 0) return null;

            var rawKey = string.Concat("FunctionByID-", functionId);

            // See if the item is in the cache
            var function = GetCacheItem(rawKey) as SysFunctionModel;
            if (function != null) return function;
            // Item not found in cache - retrieve it and insert it into the cache
            function = Api.GetById(functionId) ?? new SysFunctionModel();
            function.SelectedActions = GetActions(functionId);
            AddCacheItem(rawKey, function);

            return function;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(SysFunctionModel model)
        {
            var functionId = Api.Save(model);
            if (functionId > 0)
                // Invalidate the cache
                InvalidateCache();
            return functionId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(SysFunctionModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public string GetActions(int functionId)
        {
            var rawKey = string.Concat("ActionsByFunction-", functionId);

            var sActions = GetCacheItem(rawKey) as string;
            if (!string.IsNullOrEmpty(sActions)) return sActions;
            sActions = Api.GetActions(functionId);
            AddCacheItem(rawKey, sActions);

            return sActions;
        }

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public int? Register(DataTable data)
        {
            var functionId = Api.Register(data);
            if (functionId > 0)
                // Invalidate the cache
                InvalidateCache();
            return functionId;
        }
    }
}