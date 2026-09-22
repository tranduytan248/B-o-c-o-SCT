using System.ComponentModel;
using CenIT.ReportTourism.Biz.Cate.Traveling;
using CenIT.ReportTourism.Models.Cate.Traveling;
using TSFramework.Core.Members.Caching;

namespace CenIT.ReportTourism.Caches.Cate.Traveling
{
    [DataObject]
    public class CateTravelingServiceCache : CacheLayer
    {
        private CateTravelingServiceBiz _travelingServiceApi;

        private CateTravelingServiceBiz Api =>
            _travelingServiceApi ?? (_travelingServiceApi = new CateTravelingServiceBiz());

        protected override string[] MasterCacheKeyArray => new[]
            { "TravelingServicesCache", "DocsCache", "CENIT.APP.Cache" };

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateTravelingServiceCertificateModel GetCertificateViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("TravelingServiceCertificateByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var travelingServiceCertificateTravelEscrow = GetCacheItem(rawKey) as CateTravelingServiceCertificateModel;
            if (travelingServiceCertificateTravelEscrow != null) return travelingServiceCertificateTravelEscrow;
            // Item not found in cache - retrieve it and insert it into the cache
            travelingServiceCertificateTravelEscrow = Api.GetCertificateViaEnterprise(enterpriseId) ??
                                                      new CateTravelingServiceCertificateModel();
            AddCacheItem(rawKey, travelingServiceCertificateTravelEscrow);
            return travelingServiceCertificateTravelEscrow;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveCertificates(CateTravelingServiceCertificateModel model)
        {
            var enterpriseId = Api.SaveCertificates(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateTravelingServiceCertificateTravelEscrowModel GetCertificateViaEnterpriseTravelEscrow(
            int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("TravelingServiceCertificateTravelEscrowByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var travelingServiceCertificateTravelEscrow =
                GetCacheItem(rawKey) as CateTravelingServiceCertificateTravelEscrowModel;
            if (travelingServiceCertificateTravelEscrow != null) return travelingServiceCertificateTravelEscrow;
            // Item not found in cache - retrieve it and insert it into the cache
            travelingServiceCertificateTravelEscrow = Api.GetCertificateViaEnterpriseTravelEscrow(enterpriseId) ??
                                                      new CateTravelingServiceCertificateTravelEscrowModel();
            AddCacheItem(rawKey, travelingServiceCertificateTravelEscrow);
            return travelingServiceCertificateTravelEscrow;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveCertificateTravelEscrows(CateTravelingServiceCertificateTravelEscrowModel model)
        {
            var enterpriseId = Api.SaveCertificateTravelEscrows(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CateTravelingServiceHRInformationModel GetHRInformationViaEnterprise(int? enterpriseId)
        {
            if (enterpriseId < 0) return null;
            var rawKey = string.Concat("TravelingServiceHRInformationByEnterpriseID-", enterpriseId);
            // See if the item is in the cache
            var accommodationServiceHRInformation = GetCacheItem(rawKey) as CateTravelingServiceHRInformationModel;
            if (accommodationServiceHRInformation != null) return accommodationServiceHRInformation;
            // Item not found in cache - retrieve it and insert it into the cache
            accommodationServiceHRInformation = Api.GetHRInformationViaEnterprise(enterpriseId) ??
                                                new CateTravelingServiceHRInformationModel();
            AddCacheItem(rawKey, accommodationServiceHRInformation);
            return accommodationServiceHRInformation;
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public int SaveHRInformations(CateTravelingServiceHRInformationModel model)
        {
            var enterpriseId = Api.SaveHRInformations(model);
            if (enterpriseId > 0)
                // Invalidate the cache
                InvalidateCache();
            return enterpriseId;
        }
    }
}