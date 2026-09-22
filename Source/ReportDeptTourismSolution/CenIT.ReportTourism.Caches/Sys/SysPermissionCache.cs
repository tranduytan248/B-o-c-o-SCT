using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using CenIT.ReportTourism.Biz.Sys;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;

namespace CenIT.ReportTourism.Caches.Sys
{
    [DataObject]
    public class SysPermissionCache : CacheLayer
    {
        private SysPermissionBiz _permissionApi;

        private SysPermissionBiz Api => _permissionApi ?? (_permissionApi = new SysPermissionBiz());

        protected override string[] MasterCacheKeyArray =>
            new[]
            {
                "SysPermissionsCache", "SysMenusCache", "SysRolesCache", "SysFunctionsCache", "CenIT.Application.Cache"
            };

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SysPermissionModel> GetByRoleId(int groupId)
        {
            if (groupId < 0) return null;

            var rawKey = string.Concat("PermissionByRoleID-", groupId);

            // See if the item is in the cache
            var permissions = GetCacheItem(rawKey) as List<SysPermissionModel>;
            if (permissions != null) return permissions;
            // Item not found in cache - retrieve it and insert it into the cache
            permissions = Api.GetByRoleId(groupId);
            AddCacheItem(rawKey, permissions);

            return permissions;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public bool Save(int groupId, DataTable permission, string separated)
        {
            var success = Api.Save(groupId, permission, separated);
            if (success)
                // Invalidate the cache
                InvalidateCache();
            return success;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public bool IsAllow(string userName, string areaName, string controllerName, string actionName)
        {
            // See if the item is in the cache

            // Item not found in cache - retrieve it and insert it into the cache
            return Api.IsAllow(userName, areaName, controllerName, actionName);
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SysUserPermissionModel> GetViaUser(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var rawKey = string.Concat("ListPermissionViaUser-", userName);

            // See if the item is in the cache
            var lstPermissions = GetCacheItem(rawKey) as List<SysUserPermissionModel>;
            if (lstPermissions != null) return lstPermissions;
            // Item not found in cache - retrieve it and insert it into the cache
            lstPermissions = Api.GetViaUser(userName);
            AddCacheItem(rawKey, lstPermissions);

            return lstPermissions;
        }
    }
}