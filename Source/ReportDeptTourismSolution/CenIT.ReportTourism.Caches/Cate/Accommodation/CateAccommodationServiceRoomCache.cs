using System.Collections.Generic;
using System.ComponentModel;
using CenIT.ReportTourism.Biz.Cate.Accommodation;
using CenIT.ReportTourism.Models.Cate.Accommodation;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;
using TSFramework.Core.Utils;

namespace CenIT.ReportTourism.Caches.Cate.Accommodation
{
    [DataObject]
    public class CateAccommodationServiceRoomCache : CacheLayer
    {
        private CateAccommodationServiceRoomBiz _cateAccommodationServiceRoomApi;

        private CateAccommodationServiceRoomBiz Api => _cateAccommodationServiceRoomApi ??
                                                       (_cateAccommodationServiceRoomApi =
                                                           new CateAccommodationServiceRoomBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "CateAccommodationServiceRoomsCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateAccommodationServiceInfrastructureRoomModel> GetAll(int? enterpriseId = null)
        {
            var rawKey = $"AllCateAccommodationServiceRooms-{enterpriseId}";
            // See if the item is in the cache
            var cateAccommodationServiceRooms =
                GetCacheItem(rawKey) as List<CateAccommodationServiceInfrastructureRoomModel>;
            if (cateAccommodationServiceRooms != null) return cateAccommodationServiceRooms;
            // Item not found in cache - retrieve it and insert it into the cache
            cateAccommodationServiceRooms = Api.GetAll(enterpriseId);
            AddCacheItem(rawKey, cateAccommodationServiceRooms);
            return cateAccommodationServiceRooms;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<CateAccommodationServiceInfrastructureRoomModel> Get(int? enterpriseId, out int total,
            SysSearchModel search = null)
        {
            var objectKey = EHashMD5.FromObject(search);
            var rawKey = string.Concat($"ListCateAccommodationServiceRooms-{enterpriseId}-", objectKey);
            var rawKeyTotal = string.Concat(rawKey, "-Total");
            total = 0;
            var cacheTotal = (int?)GetCacheItem(rawKeyTotal);
            total = cacheTotal ?? 0;
            // See if the item is in the cache
            var cateAccommodationServiceRooms =
                GetCacheItem(rawKey) as List<CateAccommodationServiceInfrastructureRoomModel>;
            if (cateAccommodationServiceRooms != null) return cateAccommodationServiceRooms;
            // Item not found in cache - retrieve it and insert it into the cache
            cateAccommodationServiceRooms = Api.Get(enterpriseId, out total, search);
            AddCacheItem(rawKey, cateAccommodationServiceRooms);
            AddCacheItem(rawKeyTotal, total);
            return cateAccommodationServiceRooms;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateAccommodationServiceInfrastructureRoomModel GetById(int? infrastructureRoomId)
        {
            if (infrastructureRoomId < 0) return null;
            var rawKey = string.Concat("CateAccommodationServiceRoomByID-", infrastructureRoomId);
            // See if the item is in the cache
            var cateAccommodationServiceRoom = GetCacheItem(rawKey) as CateAccommodationServiceInfrastructureRoomModel;
            if (cateAccommodationServiceRoom != null) return cateAccommodationServiceRoom;
            // Item not found in cache - retrieve it and insert it into the cache
            cateAccommodationServiceRoom = Api.GetById(infrastructureRoomId) ??
                                           new CateAccommodationServiceInfrastructureRoomModel();
            AddCacheItem(rawKey, cateAccommodationServiceRoom);
            return cateAccommodationServiceRoom;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int Save(CateAccommodationServiceInfrastructureRoomModel model)
        {
            var infrastructureRoomId = Api.Save(model);
            if (infrastructureRoomId > 0)
                // Invalidate the cache
                InvalidateCache();
            return infrastructureRoomId;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CateAccommodationServiceInfrastructureRoomModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }
    }
}