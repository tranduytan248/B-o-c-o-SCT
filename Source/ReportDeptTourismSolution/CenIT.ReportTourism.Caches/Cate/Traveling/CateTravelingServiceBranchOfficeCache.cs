using System.Collections.Generic;
using System.ComponentModel;
using CenIT.ReportTourism.Biz.Cate.Traveling;
using CenIT.ReportTourism.Models.Cate.Traveling;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;
using TSFramework.Core.Utils;

namespace CenIT.ReportTourism.Caches.Cate.Traveling
{
    [DataObject]
    public class CateTravelingServiceBranchOfficeCache : CacheLayer
    {
        private CateTravelingServiceBranchOfficeBiz _cateTravelingServiceBranchOfficeApi;

        private CateTravelingServiceBranchOfficeBiz Api => _cateTravelingServiceBranchOfficeApi ??
                                                           (_cateTravelingServiceBranchOfficeApi =
                                                               new CateTravelingServiceBranchOfficeBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "CateTravelingServiceBranchOfficesCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateTravelingServiceBranchOfficeModel> GetAll(int? enterpriseId = null)
        {
            var rawKey = $"AllCateTravelingServiceBranchOffices-{enterpriseId}";
            // See if the item is in the cache
            var cateTravelingServiceBranchOffices = GetCacheItem(rawKey) as List<CateTravelingServiceBranchOfficeModel>;
            if (cateTravelingServiceBranchOffices != null) return cateTravelingServiceBranchOffices;
            // Item not found in cache - retrieve it and insert it into the cache
            cateTravelingServiceBranchOffices = Api.GetAll(enterpriseId);
            AddCacheItem(rawKey, cateTravelingServiceBranchOffices);
            return cateTravelingServiceBranchOffices;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateTravelingServiceBranchOfficeModel> Get(int? enterpriseId, out int total,
            SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = string.Concat($"ListCateTravelingServiceBranchOffices-{enterpriseId}-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var cateTravelingServiceBranchOffices = GetCacheItem(rawKey) as List<CateTravelingServiceBranchOfficeModel>;
            if (cateTravelingServiceBranchOffices != null) return cateTravelingServiceBranchOffices;
            // Item not found in cache - retrieve it and insert it into the cache
            cateTravelingServiceBranchOffices = Api.Get(enterpriseId, out total, search);
            AddCacheItem(rawKey, cateTravelingServiceBranchOffices);
            AddCacheItem(rawKeyTotal, total);
            return cateTravelingServiceBranchOffices;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateTravelingServiceBranchOfficeModel GetById(int? branchOfficeId)
        {
            if (branchOfficeId < 0) return null;
            var rawKey = string.Concat("CateTravelingServiceBranchOfficeByID-", branchOfficeId);
            // See if the item is in the cache
            var cateTravelingServiceBranchOffice = GetCacheItem(rawKey) as CateTravelingServiceBranchOfficeModel;
            if (cateTravelingServiceBranchOffice != null) return cateTravelingServiceBranchOffice;
            // Item not found in cache - retrieve it and insert it into the cache
            cateTravelingServiceBranchOffice =
                Api.GetById(branchOfficeId) ?? new CateTravelingServiceBranchOfficeModel();
            AddCacheItem(rawKey, cateTravelingServiceBranchOffice);
            return cateTravelingServiceBranchOffice;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int Save(CateTravelingServiceBranchOfficeModel model)
        {
            var branchOfficeId = Api.Save(model);
            if (branchOfficeId > 0)
                // Invalidate the cache
                InvalidateCache();
            return branchOfficeId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CateTravelingServiceBranchOfficeModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }
    }
}