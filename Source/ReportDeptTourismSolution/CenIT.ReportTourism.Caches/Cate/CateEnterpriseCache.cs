using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using CenIT.ReportTourism.Biz.Cate;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Search;
using TSFramework.Core.Members.Caching;
using TSFramework.Core.Utils;

namespace CenIT.ReportTourism.Caches.Cate
{
    [DataObject]
    public class CateEnterpriseCache : CacheLayer
    {
        private CateEnterpriseBiz _enterpriseApi;
        private CateEnterpriseBiz Api => _enterpriseApi ?? (_enterpriseApi = new CateEnterpriseBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "EnterprisesCache", "DataImportCache", "DocsCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateEnterpriseModel> GetAll(SearchEnterpriseModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = $"AllEnterprises-{objectKey}";
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateEnterpriseModel> enterprises) return enterprises;
            // Item not found in cache - retrieve it and insert it into the cache
            enterprises = Api.GetAll(search);
            AddCacheItem(rawKey, enterprises);
            return enterprises;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateEnterpriseModel> Get(out int total, SearchEnterpriseModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = $"ListEnterprisesForEmp-{objectKey}";
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateEnterpriseModel> enterprises) return enterprises;
            // Item not found in cache - retrieve it and insert it into the cache
            enterprises = Api.Get(out total, search);
            AddCacheItem(rawKey, enterprises);
            AddCacheItem(rawKeyTotal, total);
            return enterprises;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CateEnterpriseModel> GetViaUser(string forUser)
        {
            if (string.IsNullOrEmpty(forUser)) return null;
            var rawKey = string.Concat("EnterpriseForUser-", forUser);
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateEnterpriseModel> enterprises) return enterprises;
            // Item not found in cache - retrieve it and insert it into the cache
            enterprises = Api.GetViaUser(forUser) ?? new List<CateEnterpriseModel>();
            AddCacheItem(rawKey, enterprises);
            return enterprises;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CateEnterpriseModel> GetNotSumitReportYet(DateTime? forMonth)
        {
            if (forMonth == null) return null;
            var rawKey = string.Concat("EnterpriseNotSumitReportYet-", forMonth);
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateEnterpriseModel> enterprises) return enterprises;
            // Item not found in cache - retrieve it and insert it into the cache
            enterprises = Api.GetNotSumitReportYet(forMonth) ?? new List<CateEnterpriseModel>();
            AddCacheItem(rawKey, enterprises);
            return enterprises;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateEnterpriseModel GetById(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("EnterpriseByID-", enterpriseId);
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is CateEnterpriseModel enterprise) return enterprise;
            // Item not found in cache - retrieve it and insert it into the cache
            enterprise = Api.GetById(enterpriseId) ?? new CateEnterpriseModel();
            AddCacheItem(rawKey, enterprise);
            return enterprise;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int Save(CateEnterpriseModel model)
        {
            var enterpriseId = Api.Save(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public int Register(CateEnterpriseModel model)
        {
            var enterpriseId = Api.Register(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public int Import(int? typeBusiness, string typeBusinessName, DataTable dataImport, string importBy)
        {
            var importResult = Api.Import(typeBusiness, typeBusinessName, dataImport, importBy);
            if (importResult > 0)
                // Invalidate the cache
                InvalidateCache();
            return importResult;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveDocs(CateEnterpriseModel model)
        {
            var enterpriseId = Api.SaveDocs(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CateEnterpriseModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool ChangeStatus(CateEnterpriseModel model)
        {
            var isSucess = Api.ChangeStatus(model);
            if (isSucess)
                // Invalidate the cache
                InvalidateCache();
            return isSucess;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateEnterprisePermissionsModel> GetCateEnterprisePermissions(string userId)
        {
            var rawKey = string.Concat("GroupsByUserID-", userId);
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is List<CateEnterprisePermissionsModel> groups) return groups;
            // Item not found in cache - retrieve it and insert it into the cache
            groups = Api.GetCateEnterprisePermissions(userId);
            if (groups != null) AddCacheItem(rawKey, groups);

            return groups;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveEnterprisePermissions(CateEnterprisePermissionsModel model)
        {
            var enterpriseId = Api.SaveEnterprisePermissions(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }


        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public void DeleteCateEnterprisePermissions(CateEnterprisePermissionsModel model)
        {
            Api.DeleteCateEnterprisePermissions(model);
            //if (isDeleted)
            //    // Invalidate the cache
            //    InvalidateCache();
            //return isDeleted;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CateEnterpriseModel> Search(string taxCode, string email, string businessName)
        {
            //var rawKey = $"SearchEnterprise-{taxCode}-{email}-{businessName}";
            // See if the item is in the cache
            //var enterprises = GetCacheItem(rawKey) as List<CateEnterpriseModel>;
            //if (enterprises != null) return enterprises;
            // Item not found in cache - retrieve it and insert it into the cache
            var enterprises = Api.Search(taxCode, email, businessName) ?? new List<CateEnterpriseModel>();
            //AddCacheItem(rawKey, enterprises);
            return enterprises;
        }
    }
}