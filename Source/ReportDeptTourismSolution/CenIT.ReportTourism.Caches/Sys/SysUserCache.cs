using System.Collections.Generic;
using System.ComponentModel;
using CenIT.ReportTourism.Biz.Sys;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;
using TSFramework.Core.Utils;

namespace CenIT.ReportTourism.Caches.Sys
{
    [DataObject]
    public class SysUserCache : CacheLayer
    {
        private SysUserBiz _userApi;

        protected override string[] MasterCacheKeyArray => new[] { "SysUsersCache", "CenIT.Application.Cache" };

        private SysUserBiz Api => _userApi ?? (_userApi = new SysUserBiz());

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public SysUserModel Login(string userName, string password)
        {
            return Api.Login(userName, password);
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public SysUserModel LoginViaEmail(string email, string password)
        {
            return Api.LoginViaEmail(email, password);
        }

        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int? Save(SysUserModel model, string savedBy)
        {
            var idUser = Api.Save(model, savedBy);
            // Invalidate the cache
            InvalidateCache();
            return idUser;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysUserModel> Get(out int total, SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);

            var rawKey = string.Concat("ListUsers-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var users = GetCacheItem(rawKey) as List<SysUserModel>;
            if (users != null) return users;
            // Item not found in cache - retrieve it and insert it into the cache
            users = Api.GetList(out total, search);
            if (users == null) return null;
            AddCacheItem(rawKey, users);
            AddCacheItem(rawKeyTotal, total);

            return users;
        }

        public List<SysUserModel> GetAll()
        {
            var rawKey = "AllUsers";
            var allUsers = GetCacheItem(rawKey) as List<SysUserModel>;
            if (allUsers != null) return allUsers;
            // Item not found in cache - retrieve it and insert it into the cache
            allUsers = Api.GetAll();
            if (allUsers == null) return null;
            AddCacheItem(rawKey, allUsers);
            return allUsers;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysUserModel GetById(int userId)
        {
            if (userId < 0) return null;

            var rawKey = string.Concat("UserByID-", userId);

            // See if the item is in the cache
            var user = GetCacheItem(rawKey) as SysUserModel;
            if (user != null) return user;
            // Item not found in cache - retrieve it and insert it into the cache
            user = Api.GetById(userId);
            AddCacheItem(rawKey, user);

            return user;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysUserModel GetByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var rawKey = string.Concat("UserByUserName-", userName);

            // See if the item is in the cache
            var user = GetCacheItem(rawKey) as SysUserModel;
            if (user != null) return user;
            // Item not found in cache - retrieve it and insert it into the cache
            user = Api.GetByUserName(userName);
            AddCacheItem(rawKey, user);

            return user;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysUserModel GetByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            var rawKey = string.Concat("UserByEmail-", email);

            // See if the item is in the cache
            var user = GetCacheItem(rawKey) as SysUserModel;
            if (user != null) return user;
            // Item not found in cache - retrieve it and insert it into the cache
            user = Api.GetByEmail(email);
            if (user != null)
                AddCacheItem(rawKey, user);

            return user;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(SysUserModel model, string deletedBy)
        {
            var isDeleted = Api.Delete(model, deletedBy);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public bool DeActive(SysUserModel model, string deactiveBy)
        {
            var isSuccess = Api.DeActive(model, deactiveBy);
            if (isSuccess)
                // Invalidate the cache
                InvalidateCache();
            return isSuccess;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public bool Active(SysUserModel model, string activeBy)
        {
            var isSuccess = Api.Active(model, activeBy);
            if (isSuccess)
                // Invalidate the cache
                InvalidateCache();
            return isSuccess;
        }


        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<SysRoleModel> GetRoles(int? userId)
        {
            var rawKey = string.Concat("RolesByUserID-", userId);
            // See if the item is in the cache
            var users = GetCacheItem(rawKey) as List<SysRoleModel>;
            if (users != null) return users;
            // Item not found in cache - retrieve it and insert it into the cache
            users = Api.GetRoles(userId);
            AddCacheItem(rawKey, users);

            return users;
        }

        [DataObjectMethod(DataObjectMethodType.Update, true)]
        public int? ChangePassword(string userName, string oldPass, string newPass, string salt, string reason,
            string changeBy)
        {
            var valReturn = Api.ChangePassword(userName, oldPass, newPass, salt, reason, changeBy);
            return valReturn;
        }

        [DataObjectMethod(DataObjectMethodType.Update, true)]
        public int? ResetPassword(string userName, string newPass, string salt, string reason, string changeBy)
        {
            var valReturn = Api.ResetPassword(userName, newPass, salt, reason, changeBy);
            return valReturn;
        }

        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int? SaveLogin(string userName, bool isValid, string senderIp, string senderHeader)
        {
            var idUser = Api.SaveLogin(userName, isValid, senderIp, senderHeader);
            return idUser;
        }
    }
}