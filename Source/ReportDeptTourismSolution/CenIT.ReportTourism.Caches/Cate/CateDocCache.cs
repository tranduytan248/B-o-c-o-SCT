using System.ComponentModel;
using CenIT.ReportTourism.Biz.Cate;
using CenIT.ReportTourism.Models.Cate;
using TSFramework.Core.Members.Caching;

namespace CenIT.ReportTourism.Caches.Cate
{
    [DataObject]
    public class CateDocCache : CacheLayer
    {
        private CateDocBiz _docApi;
        private CateDocBiz Api => _docApi ?? (_docApi = new CateDocBiz());

        protected override string[] MasterCacheKeyArray => new[]
        {
            "DocsCache", "EnterprisesCache", "AccommodationServicesCache", "TravelingServicesCache",
            "ServicesForTouristsCache", "TouristAttractionsCache", "TransportTouristsServicesCache", "CENIT.APP.Cache"
        };


        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateDocModel GetById(string docId)
        {
            if (string.IsNullOrEmpty(docId)) return null;
            var rawKey = string.Concat("DocByID-", docId);
            // See if the item is in the cache
            if (GetCacheItem(rawKey) is CateDocModel doc) return doc;
            // Item not found in cache - retrieve it and insert it into the cache
            doc = Api.GetById(docId) ?? new CateDocModel();
            AddCacheItem(rawKey, doc);
            return doc;
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Delete(CateDocModel model)
        {
            var isDeleted = Api.Delete(model);
            if (isDeleted)
                // Invalidate the cache
                InvalidateCache();
            return isDeleted;
        }
    }
}