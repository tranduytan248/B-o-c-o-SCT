using System.ComponentModel;
using CenIT.ReportTourism.Biz.Cate.TransportTourists;
using CenIT.ReportTourism.Models.Cate.TransportTourists;
using TSFramework.Core.Members.Caching;

namespace CenIT.ReportTourism.Caches.Cate.TransportTourists
{
    [DataObject]
    public class CateTransportTouristsServiceCache : CacheLayer
    {
        private CateTransportTouristsServiceBiz _transportTouristsServiceApi;

        private CateTransportTouristsServiceBiz Api => _transportTouristsServiceApi ??
                                                       (_transportTouristsServiceApi =
                                                           new CateTransportTouristsServiceBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "TransportTouristsServicesCache", "DocsCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateTransportTouristsServiceCertificateModel GetCertificateViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("TransportTouristsServiceCertificateByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var transportTouristsServiceCertificateTravelEscrow =
                GetCacheItem(rawKey) as CateTransportTouristsServiceCertificateModel;
            if (transportTouristsServiceCertificateTravelEscrow != null)
                return transportTouristsServiceCertificateTravelEscrow;
            // Item not found in cache - retrieve it and insert it into the cache
            transportTouristsServiceCertificateTravelEscrow = Api.GetCertificateViaEnterprise(enterpriseId) ??
                                                              new CateTransportTouristsServiceCertificateModel();
            AddCacheItem(rawKey, transportTouristsServiceCertificateTravelEscrow);
            return transportTouristsServiceCertificateTravelEscrow;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveCertificates(CateTransportTouristsServiceCertificateModel model)
        {
            var enterpriseId = Api.SaveCertificates(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateTransportTouristsServiceHRInformationModel GetHRInformationViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("TransportTouristsServiceHRInformationByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var accommodationServiceHRInformation =
                GetCacheItem(rawKey) as CateTransportTouristsServiceHRInformationModel;
            if (accommodationServiceHRInformation != null) return accommodationServiceHRInformation;
            // Item not found in cache - retrieve it and insert it into the cache
            accommodationServiceHRInformation = Api.GetHRInformationViaEnterprise(enterpriseId) ??
                                                new CateTransportTouristsServiceHRInformationModel();
            AddCacheItem(rawKey, accommodationServiceHRInformation);
            return accommodationServiceHRInformation;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveHRInformations(CateTransportTouristsServiceHRInformationModel model)
        {
            var enterpriseId = Api.SaveHRInformations(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateTransportTouristsServiceInfrastructureModel GetInfrastructureViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("TransportTouristsServiceInfrastructureByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var accommodationServiceInfrastructure =
                GetCacheItem(rawKey) as CateTransportTouristsServiceInfrastructureModel;
            if (accommodationServiceInfrastructure != null) return accommodationServiceInfrastructure;
            // Item not found in cache - retrieve it and insert it into the cache
            accommodationServiceInfrastructure = Api.GetInfrastructureViaEnterprise(enterpriseId) ??
                                                 new CateTransportTouristsServiceInfrastructureModel();
            AddCacheItem(rawKey, accommodationServiceInfrastructure);
            return accommodationServiceInfrastructure;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveInfrastructures(CateTransportTouristsServiceInfrastructureModel model)
        {
            var enterpriseId = Api.SaveInfrastructures(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }
    }
}