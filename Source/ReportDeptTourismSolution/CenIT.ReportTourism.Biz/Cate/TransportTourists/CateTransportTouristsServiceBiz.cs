using CenIT.ReportTourism.Models.Cate.TransportTourists;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate.TransportTourists
{
    public class CateTransportTouristsServiceBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";

        private readonly string _cateTransportTouristsServicesGetCertificates =
            "Cate_TransportTouristsServices_GetCertificates";

        private readonly string _cateTransportTouristsServicesGetHRInformations =
            "Cate_TransportTouristsServices_GetHRInformations";

        private readonly string _cateTransportTouristsServicesGetInfrastructures =
            "Cate_TransportTouristsServices_GetInfrastructures";

        private readonly string _cateTransportTouristsServicesSaveCertificates =
            "Cate_TransportTouristsServices_SaveCertificates";

        private readonly string _cateTransportTouristsServicesSaveHRInformations =
            "Cate_TransportTouristsServices_SaveHRInformations";

        private readonly string _cateTransportTouristsServicesSaveInfrastructures =
            "Cate_TransportTouristsServices_SaveInfrastructures";

        public int SaveCertificates(CateTransportTouristsServiceCertificateModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateTransportTouristsServicesSaveCertificates,
                DATA_PROVIDER_NAME,
                model.EnterpriseId,
                model.LicenseFile,
                model.DataLicenseFile,
                model.LicenseNumber,
                model.ReleaseDate,
                model.TimeRelease,
                model.WaterWay,
                model.Road,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }

        public CateTransportTouristsServiceCertificateModel GetCertificateViaEnterprise(int? enterpriseId = null)
        {
            var transportTouristsService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateTransportTouristsServiceCertificateModel>(
                    _cateTransportTouristsServicesGetCertificates, DATA_PROVIDER_NAME, enterpriseId);
            return transportTouristsService;
        }

        public CateTransportTouristsServiceHRInformationModel GetHRInformationViaEnterprise(int? enterpriseId = null)
        {
            var transportTouristsService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateTransportTouristsServiceHRInformationModel>(
                    _cateTransportTouristsServicesGetHRInformations, DATA_PROVIDER_NAME, enterpriseId);
            return transportTouristsService;
        }

        public int SaveHRInformations(CateTransportTouristsServiceHRInformationModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateTransportTouristsServicesSaveHRInformations,
                DATA_PROVIDER_NAME,
                model.EnterpriseId,
                model.TotalStaff,
                model.StaffOnLiteracyOnCollege,
                model.StaffOnUniversity,
                model.StaffOnCollege,
                model.StaffOnIntermediate,
                model.StaffOnHighSchool,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }

        public CateTransportTouristsServiceInfrastructureModel GetInfrastructureViaEnterprise(int? enterpriseId = null)
        {
            var transportTouristsService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateTransportTouristsServiceInfrastructureModel>(
                    _cateTransportTouristsServicesGetInfrastructures, DATA_PROVIDER_NAME, enterpriseId);
            return transportTouristsService;
        }

        public int SaveInfrastructures(CateTransportTouristsServiceInfrastructureModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateTransportTouristsServicesSaveInfrastructures,
                DATA_PROVIDER_NAME,
                model.EnterpriseId,
                model.TotalTransports,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }
    }
}