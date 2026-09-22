using System.ComponentModel;
using CenIT.ReportTourism.Biz.Cate.Accommodation;
using CenIT.ReportTourism.Models.Cate.Accommodation;
using TSFramework.Core.Members.Caching;

namespace CenIT.ReportTourism.Caches.Cate.Accommodation
{
    [DataObject]
    public class CateAccommodationServiceCache : CacheLayer
    {
        private CateAccommodationServiceBiz _accommodationServiceApi;

        private CateAccommodationServiceBiz Api => _accommodationServiceApi ??
                                                   (_accommodationServiceApi = new CateAccommodationServiceBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "AccommodationServicesCache", "DocsCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateAccommodationServiceModel GetById(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("AccommodationServiceByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var accommodationService = GetCacheItem(rawKey) as CateAccommodationServiceModel;
            if (accommodationService != null) return accommodationService;
            // Item not found in cache - retrieve it and insert it into the cache
            accommodationService = Api.GetViaEnterprise(enterpriseId) ?? new CateAccommodationServiceModel();
            AddCacheItem(rawKey, accommodationService);
            return accommodationService;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateAccommodationServiceCertificateModel GetCertificatesViaId(int? accommodationId)
        {
            if (accommodationId < 0) return null;
            var rawKey = string.Concat("AccommodationServiceByAccommodationID-", accommodationId);
            // See if the item is in the cache
            var accommodationServiceCertificate = GetCacheItem(rawKey) as CateAccommodationServiceCertificateModel;
            if (accommodationServiceCertificate != null) return accommodationServiceCertificate;
            // Item not found in cache - retrieve it and insert it into the cache
            accommodationServiceCertificate = Api.GetCertificatesViaId(accommodationId) ??
                                              new CateAccommodationServiceCertificateModel();
            AddCacheItem(rawKey, accommodationServiceCertificate);
            return accommodationServiceCertificate;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateAccommodationServiceCertificateModel GetCertificateViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("AccommodationServiceCertificateByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var accommodationServiceCertificate = GetCacheItem(rawKey) as CateAccommodationServiceCertificateModel;
            if (accommodationServiceCertificate != null) return accommodationServiceCertificate;
            // Item not found in cache - retrieve it and insert it into the cache
            accommodationServiceCertificate = Api.GetCertificateViaEnterprise(enterpriseId) ??
                                              new CateAccommodationServiceCertificateModel();
            AddCacheItem(rawKey, accommodationServiceCertificate);
            return accommodationServiceCertificate;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveCertificates(CateAccommodationServiceCertificateModel model)
        {
            var enterpriseId = Api.SaveCertificates(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateAccommodationServiceInfrastructureModel GetInfrastructureViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("AccommodationServiceInfrastructureByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var accommodationServiceInfrastructure =
                GetCacheItem(rawKey) as CateAccommodationServiceInfrastructureModel;
            if (accommodationServiceInfrastructure != null) return accommodationServiceInfrastructure;
            // Item not found in cache - retrieve it and insert it into the cache
            accommodationServiceInfrastructure = Api.GetInfrastructureViaEnterprise(enterpriseId) ??
                                                 new CateAccommodationServiceInfrastructureModel();
            AddCacheItem(rawKey, accommodationServiceInfrastructure);
            return accommodationServiceInfrastructure;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveInfrastructures(CateAccommodationServiceInfrastructureModel model)
        {
            var enterpriseId = Api.SaveInfrastructures(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateAccommodationServiceHRInformationModel GetHRInformationViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("AccommodationServiceHRInformationByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var accommodationServiceHRInformation = GetCacheItem(rawKey) as CateAccommodationServiceHRInformationModel;
            if (accommodationServiceHRInformation != null) return accommodationServiceHRInformation;
            // Item not found in cache - retrieve it and insert it into the cache
            accommodationServiceHRInformation = Api.GetHRInformationViaEnterprise(enterpriseId) ??
                                                new CateAccommodationServiceHRInformationModel();
            AddCacheItem(rawKey, accommodationServiceHRInformation);
            return accommodationServiceHRInformation;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveHRInformations(CateAccommodationServiceHRInformationModel model)
        {
            var enterpriseId = Api.SaveHRInformations(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateAccommodationServiceModel GetInfoViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("AccommodationServiceInfoByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var accommodationService = GetCacheItem(rawKey) as CateAccommodationServiceModel;
            if (accommodationService != null) return accommodationService;
            // Item not found in cache - retrieve it and insert it into the cache
            accommodationService = Api.GetInfoViaEnterprise(enterpriseId) ?? new CateAccommodationServiceModel();
            AddCacheItem(rawKey, accommodationService);
            return accommodationService;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveInfo(CateAccommodationServiceModel model)
        {
            var enterpriseId = Api.SaveInfo(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }
    }
}