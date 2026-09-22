using System.ComponentModel;
using CenIT.ReportTourism.Biz.Cate.ServicesForTourist;
using CenIT.ReportTourism.Models.Cate.ServicesForTourist;
using TSFramework.Core.Members.Caching;

namespace CenIT.ReportTourism.Caches.Cate.ServicesForTourist
{
    [DataObject]
    public class CateServicesForTouristCache : CacheLayer
    {
        private CateServicesForTouristBiz _transportTouristsServiceApi;

        private CateServicesForTouristBiz Api => _transportTouristsServiceApi ??
                                                 (_transportTouristsServiceApi = new CateServicesForTouristBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "ServicesForTouristsCache", "DocsCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateServicesForTouristCertificateModel GetCertificateViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("ServicesForTouristCertificateByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var transportTouristsServiceCertificateTravelEscrow =
                GetCacheItem(rawKey) as CateServicesForTouristCertificateModel;
            if (transportTouristsServiceCertificateTravelEscrow != null)
                return transportTouristsServiceCertificateTravelEscrow;
            // Item not found in cache - retrieve it and insert it into the cache
            transportTouristsServiceCertificateTravelEscrow = Api.GetCertificateViaEnterprise(enterpriseId) ??
                                                              new CateServicesForTouristCertificateModel();
            AddCacheItem(rawKey, transportTouristsServiceCertificateTravelEscrow);
            return transportTouristsServiceCertificateTravelEscrow;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveCertificates(CateServicesForTouristCertificateModel model)
        {
            var enterpriseId = Api.SaveCertificates(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateServicesForTouristHRInformationModel GetHRInformationViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("ServicesForTouristHRInformationByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var touristAttractionHRInformation = GetCacheItem(rawKey) as CateServicesForTouristHRInformationModel;
            if (touristAttractionHRInformation != null) return touristAttractionHRInformation;
            // Item not found in cache - retrieve it and insert it into the cache
            touristAttractionHRInformation = Api.GetHRInformationViaEnterprise(enterpriseId) ??
                                             new CateServicesForTouristHRInformationModel();
            AddCacheItem(rawKey, touristAttractionHRInformation);
            return touristAttractionHRInformation;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveHRInformations(CateServicesForTouristHRInformationModel model)
        {
            var enterpriseId = Api.SaveHRInformations(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateServicesForTouristInfrastructureModel GetInfrastructureViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("ServicesForTouristInfrastructureByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var touristAttractionInfrastructure = GetCacheItem(rawKey) as CateServicesForTouristInfrastructureModel;
            if (touristAttractionInfrastructure != null) return touristAttractionInfrastructure;
            // Item not found in cache - retrieve it and insert it into the cache
            touristAttractionInfrastructure = Api.GetInfrastructureViaEnterprise(enterpriseId) ??
                                              new CateServicesForTouristInfrastructureModel();
            AddCacheItem(rawKey, touristAttractionInfrastructure);
            return touristAttractionInfrastructure;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveInfrastructures(CateServicesForTouristInfrastructureModel model)
        {
            var enterpriseId = Api.SaveInfrastructures(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateServicesForTouristModel GetInfoViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("ServicesForTouristInfoByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var touristAttraction = GetCacheItem(rawKey) as CateServicesForTouristModel;
            if (touristAttraction != null) return touristAttraction;
            // Item not found in cache - retrieve it and insert it into the cache
            touristAttraction = Api.GetInfoViaEnterprise(enterpriseId) ?? new CateServicesForTouristModel();
            AddCacheItem(rawKey, touristAttraction);
            return touristAttraction;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveInfo(CateServicesForTouristModel model)
        {
            var enterpriseId = Api.SaveInfo(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }
    }
}