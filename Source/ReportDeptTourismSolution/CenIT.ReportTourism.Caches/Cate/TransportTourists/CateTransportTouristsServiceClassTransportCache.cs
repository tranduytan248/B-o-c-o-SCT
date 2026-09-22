using System.Collections.Generic;
using System.ComponentModel;
using CenIT.ReportTourism.Biz.Cate.TransportTourists;
using CenIT.ReportTourism.Models.Cate.TransportTourists;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;
using TSFramework.Core.Utils;

namespace CenIT.ReportTourism.Caches.Cate.TransportTourists
{
    [DataObject]
    public class CateTransportTouristsServiceClassTransportCache : CacheLayer
    {
        private CateTransportTouristsServiceClassTransportBiz _cateTransportTouristsServiceClassTransportApi;

        private CateTransportTouristsServiceClassTransportBiz Api => _cateTransportTouristsServiceClassTransportApi ??
                                                                     (_cateTransportTouristsServiceClassTransportApi =
                                                                         new
                                                                             CateTransportTouristsServiceClassTransportBiz()
                                                                     );

        protected override string[] MasterCacheKeyArray => new[]
            { "CateTransportTouristsServiceClassTransportsCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateTransportTouristsServiceInfrastructureClassTransportModel> GetAll(int? enterpriseId = null)
        {
            var rawKey = $"AllCateTransportTouristsServiceClassTransports-{enterpriseId}";
            // See if the item is in the cache
            var cateTransportTouristsServiceClassTransports =
                GetCacheItem(rawKey) as List<CateTransportTouristsServiceInfrastructureClassTransportModel>;
            if (cateTransportTouristsServiceClassTransports != null) return cateTransportTouristsServiceClassTransports;
            // Item not found in cache - retrieve it and insert it into the cache
            cateTransportTouristsServiceClassTransports = Api.GetAll(enterpriseId);
            AddCacheItem(rawKey, cateTransportTouristsServiceClassTransports);
            return cateTransportTouristsServiceClassTransports;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateTransportTouristsServiceInfrastructureClassTransportModel> Get(int? enterpriseId, out int total,
            SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = string.Concat($"ListCateTransportTouristsServiceClassTransports-{enterpriseId}-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var cateTransportTouristsServiceClassTransports =
                GetCacheItem(rawKey) as List<CateTransportTouristsServiceInfrastructureClassTransportModel>;
            if (cateTransportTouristsServiceClassTransports != null) return cateTransportTouristsServiceClassTransports;
            // Item not found in cache - retrieve it and insert it into the cache
            cateTransportTouristsServiceClassTransports = Api.Get(enterpriseId, out total, search);
            AddCacheItem(rawKey, cateTransportTouristsServiceClassTransports);
            AddCacheItem(rawKeyTotal, total);
            return cateTransportTouristsServiceClassTransports;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateTransportTouristsServiceInfrastructureClassTransportModel GetById(
            int? infrastructureClassTransportId)
        {
            if (infrastructureClassTransportId < 0) return null;
            var rawKey = string.Concat("CateTransportTouristsServiceClassTransportByID-",
                infrastructureClassTransportId);
            // See if the item is in the cache
            var cateTransportTouristsServiceClassTransport =
                GetCacheItem(rawKey) as CateTransportTouristsServiceInfrastructureClassTransportModel;
            if (cateTransportTouristsServiceClassTransport != null) return cateTransportTouristsServiceClassTransport;
            // Item not found in cache - retrieve it and insert it into the cache
            cateTransportTouristsServiceClassTransport = Api.GetById(infrastructureClassTransportId) ??
                                                         new
                                                             CateTransportTouristsServiceInfrastructureClassTransportModel();
            AddCacheItem(rawKey, cateTransportTouristsServiceClassTransport);
            return cateTransportTouristsServiceClassTransport;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int Save(CateTransportTouristsServiceInfrastructureClassTransportModel model)
        {
            var infrastructureClassTransportId = Api.Save(model);
            if (infrastructureClassTransportId > 0)
                // Invalidate the cache
                InvalidateCache();
            return infrastructureClassTransportId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CateTransportTouristsServiceInfrastructureClassTransportModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }
    }
}