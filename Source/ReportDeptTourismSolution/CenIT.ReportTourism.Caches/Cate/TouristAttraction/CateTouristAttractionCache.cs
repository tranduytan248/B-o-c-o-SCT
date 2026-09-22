using System.ComponentModel;
using CenIT.ReportTourism.Biz.Cate.TouristAttraction;
using CenIT.ReportTourism.Models.Cate.TouristAttraction;
using TSFramework.Core.Members.Caching;

namespace CenIT.ReportTourism.Caches.Cate.TouristAttraction
{
    [DataObject]
    public class CateTouristAttractionCache : CacheLayer
    {
        private CateTouristAttractionBiz _transportTouristsServiceApi;

        private CateTouristAttractionBiz Api => _transportTouristsServiceApi ??
                                                (_transportTouristsServiceApi = new CateTouristAttractionBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "TouristAttractionsCache", "DocsCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateTouristAttractionCertificateModel GetCertificateViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("TouristAttractionCertificateByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var transportTouristsServiceCertificateTravelEscrow =
                GetCacheItem(rawKey) as CateTouristAttractionCertificateModel;
            if (transportTouristsServiceCertificateTravelEscrow != null)
                return transportTouristsServiceCertificateTravelEscrow;
            // Item not found in cache - retrieve it and insert it into the cache
            transportTouristsServiceCertificateTravelEscrow = Api.GetCertificateViaEnterprise(enterpriseId) ??
                                                              new CateTouristAttractionCertificateModel();
            AddCacheItem(rawKey, transportTouristsServiceCertificateTravelEscrow);
            return transportTouristsServiceCertificateTravelEscrow;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveCertificates(CateTouristAttractionCertificateModel model)
        {
            var enterpriseId = Api.SaveCertificates(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateTouristAttractionHRInformationModel GetHRInformationViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("TouristAttractionHRInformationByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var touristAttractionHRInformation = GetCacheItem(rawKey) as CateTouristAttractionHRInformationModel;
            if (touristAttractionHRInformation != null) return touristAttractionHRInformation;
            // Item not found in cache - retrieve it and insert it into the cache
            touristAttractionHRInformation = Api.GetHRInformationViaEnterprise(enterpriseId) ??
                                             new CateTouristAttractionHRInformationModel();
            AddCacheItem(rawKey, touristAttractionHRInformation);
            return touristAttractionHRInformation;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveHRInformations(CateTouristAttractionHRInformationModel model)
        {
            var enterpriseId = Api.SaveHRInformations(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateTouristAttractionInfrastructureModel GetInfrastructureViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("TouristAttractionInfrastructureByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var touristAttractionInfrastructure = GetCacheItem(rawKey) as CateTouristAttractionInfrastructureModel;
            if (touristAttractionInfrastructure != null) return touristAttractionInfrastructure;
            // Item not found in cache - retrieve it and insert it into the cache
            touristAttractionInfrastructure = Api.GetInfrastructureViaEnterprise(enterpriseId) ??
                                              new CateTouristAttractionInfrastructureModel();
            AddCacheItem(rawKey, touristAttractionInfrastructure);
            return touristAttractionInfrastructure;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveInfrastructures(CateTouristAttractionInfrastructureModel model)
        {
            var enterpriseId = Api.SaveInfrastructures(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateTouristAttractionModel GetInfoViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("TouristAttractionInfoByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var touristAttraction = GetCacheItem(rawKey) as CateTouristAttractionModel;
            if (touristAttraction != null) return touristAttraction;
            // Item not found in cache - retrieve it and insert it into the cache
            touristAttraction = Api.GetInfoViaEnterprise(enterpriseId) ?? new CateTouristAttractionModel();
            AddCacheItem(rawKey, touristAttraction);
            return touristAttraction;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveInfo(CateTouristAttractionModel model)
        {
            var enterpriseId = Api.SaveInfo(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }
    }
}