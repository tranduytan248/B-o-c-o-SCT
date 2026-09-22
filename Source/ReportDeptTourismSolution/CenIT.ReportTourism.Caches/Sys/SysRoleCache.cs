using System.Collections.Generic;
using System.ComponentModel;
using CenIT.ReportTourism.Biz.Sys;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;
using TSFramework.Core.Utils;

namespace CenIT.ReportTourism.Caches.Sys
{
    [DataObject]
    public class SysRoleCache : CacheLayer
    {
        private SysRoleBiz _roleApi;

        private SysRoleBiz Api => _roleApi ?? (_roleApi = new SysRoleBiz());

        protected override string[] MasterCacheKeyArray =>
            new[]
            {
                "SysRolesCache", "SysPermissionsCache", "SysMenusCache", "SysFunctionsCache", "CenIT.Application.Cache"
            };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysRoleModel> GetAll()
        {
            const string rawKey = "AllRoles";
            // See if the item is in the cache
            var roles = GetCacheItem(rawKey) as List<SysRoleModel>;
            if (roles != null) return roles;
            // Item not found in cache - retrieve it and insert it into the cache
            roles = Api.GetAll();
            AddCacheItem(rawKey, roles);

            return roles;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysRoleModel> Get(out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = string.Concat("ListRoles-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");

            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var roles = GetCacheItem(rawKey) as List<SysRoleModel>;
            if (roles != null) return roles;
            // Item not found in cache - retrieve it and insert it into the cache
            roles = Api.GetList(out total, search);
            AddCacheItem(rawKey, roles);
            AddCacheItem(rawKeyTotal, total);

            return roles;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysRoleModel GetById(int roleId)
        {
            if (roleId < 0) return null;

            var rawKey = string.Concat("RoleByID-", roleId);

            // See if the item is in the cache
            var role = GetCacheItem(rawKey) as SysRoleModel;
            if (role != null) return role;
            // Item not found in cache - retrieve it and insert it into the cache
            role = Api.GetById(roleId);
            if (role != null) AddCacheItem(rawKey, role);

            return role;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int? Save(SysRoleModel model)
        {
            var roleId = Api.Save(model);
            if (roleId > 0)
                // Invalidate the cache
                InvalidateCache();
            return roleId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(SysRoleModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }
    }
}