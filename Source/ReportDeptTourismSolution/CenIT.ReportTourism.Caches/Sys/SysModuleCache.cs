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
    public class SysModuleCache : CacheLayer
    {
        private SysModuleBiz _contentPanelApi;

        private SysModuleBiz Api => _contentPanelApi ?? (_contentPanelApi = new SysModuleBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "SysContentPanelsCache", "SysLayoutCache", "SysModuleCache", "CenIT.Application.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SysModuleContentPanelModel> GetByLayoutName(string moduleName)
        {
            var rawKey = string.Concat("ModuleContentPanelsByLayoutName-", moduleName);
            // See if the item is in the cache
            var contentPanels = GetCacheItem(rawKey) as List<SysModuleContentPanelModel>;
            if (contentPanels != null) return contentPanels;
            // Item not found in cache - retrieve it and insert it into the cache
            contentPanels = Api.GetByLayoutName(moduleName);
            if (contentPanels != null)
                AddCacheItem(rawKey, contentPanels);

            return contentPanels;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SysModuleContentPanelModel> GetByUser(string userName)
        {
            var rawKey = string.Concat("ModuleContentPanelsByUserName-", userName);
            // See if the item is in the cache
            var contentPanels = GetCacheItem(rawKey) as List<SysModuleContentPanelModel>;
            if (contentPanels != null) return contentPanels;
            // Item not found in cache - retrieve it and insert it into the cache
            contentPanels = Api.GetByUser(userName);
            if (contentPanels != null)
                AddCacheItem(rawKey, contentPanels);

            return contentPanels;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysModuleModel> GetAll()
        {
            const string rawKey = "AllModules";
            // See if the item is in the cache
            var modules = GetCacheItem(rawKey) as List<SysModuleModel>;
            if (modules != null) return modules;
            // Item not found in cache - retrieve it and insert it into the cache
            modules = Api.GetAll();
            AddCacheItem(rawKey, modules);

            return modules;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysModuleModel> Get(out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);

            var rawKey = string.Concat("ListModules-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var modules = GetCacheItem(rawKey) as List<SysModuleModel>;
            if (modules != null) return modules;
            // Item not found in cache - retrieve it and insert it into the cache
            modules = Api.GetList(out total, search);
            if (modules == null) return null;
            AddCacheItem(rawKey, modules);
            AddCacheItem(rawKeyTotal, total);

            return modules;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysModuleModel GetById(int moduleId)
        {
            if (moduleId < 0) return null;

            var rawKey = string.Concat("ModuleByID-", moduleId);

            // See if the item is in the cache
            var module = GetCacheItem(rawKey) as SysModuleModel;
            if (module != null) return module;
            // Item not found in cache - retrieve it and insert it into the cache
            module = Api.GetById(moduleId);
            if (module != null)
                AddCacheItem(rawKey, module);

            return module;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(SysModuleModel model)
        {
            var moduleId = Api.Save(model);
            if (moduleId > 0)
                // Invalidate the cache
                InvalidateCache();
            return moduleId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(SysModuleModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? SaveContentPanelModule(string contentPanelName, SysModuleModel model)
        {
            var moduleId = Api.SaveContentPanelModule(contentPanelName, model);
            if (moduleId > 0)
                // Invalidate the cache
                InvalidateCache();
            return moduleId;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? SaveListContentPanelModule(DataTable dataContentPanelModules, string creator)
        {
            var moduleId = Api.SaveListContentPanelModule(dataContentPanelModules, creator);
            if (moduleId > 0)
                // Invalidate the cache
                InvalidateCache();
            return moduleId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool DeleteContentPanelModule(int moduleId, string contentPanelName, string updater)
        {
            var isDeleted = Api.DeleteContentPanelModule(moduleId, contentPanelName, updater);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SysModuleModel> GetNotInContentPanel(string contentPanelName)
        {
            var rawKey = string.Concat("ModuleNotInContentPanelByContentPanelName-", contentPanelName);
            // See if the item is in the cache
            var modules = GetCacheItem(rawKey) as List<SysModuleModel>;
            if (modules != null) return modules;
            // Item not found in cache - retrieve it and insert it into the cache
            modules = Api.GetNotInContentPanel(contentPanelName);
            if (modules != null) AddCacheItem(rawKey, modules);

            return modules;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? SavePermissionModule(SysPermissionModuleModel model)
        {
            var retId = Api.SavePermissionModule(model);
            if (retId > 0)
                // Invalidate the cache
                InvalidateCache();
            return retId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysUserModel> GetPermissionUsers(int moduleId)
        {
            var rawKey = string.Concat("PermissionUserByModuleId-", moduleId);
            // See if the item is in the cache
            var users = GetCacheItem(rawKey) as List<SysUserModel>;
            if (users != null) return users;
            // Item not found in cache - retrieve it and insert it into the cache
            users = Api.GetPermissionUsers(moduleId);
            if (users != null)
                AddCacheItem(rawKey, users);

            return users;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SysModuleModel> GetByUserName(string userName)
        {
            var rawKey = string.Concat("AllModuleByUserName-", userName);
            // See if the item is in the cache
            var modules = GetCacheItem(rawKey) as List<SysModuleModel>;
            if (modules != null) return modules;
            // Item not found in cache - retrieve it and insert it into the cache
            modules = Api.GetByUserName(userName);
            if (modules != null) AddCacheItem(rawKey, modules);

            return modules;
        }
    }
}