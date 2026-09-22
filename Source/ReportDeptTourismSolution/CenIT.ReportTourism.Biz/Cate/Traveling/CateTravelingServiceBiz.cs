using CenIT.ReportTourism.Models.Cate.Traveling;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate.Traveling
{
    public class CateTravelingServiceBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _cateTravelingServicesGetCertificates = "Cate_TravelingServices_GetCertificates";

        private readonly string _cateTravelingServicesGetCertificateTravelEscrows =
            "Cate_TravelingServices_GetCertificateTravelEscrows";

        private readonly string _cateTravelingServicesGetHRInformations = "Cate_TravelingServices_GetHRInformations";

        private readonly string _cateTravelingServicesSaveCertificates = "Cate_TravelingServices_SaveCertificates";

        private readonly string _cateTravelingServicesSaveCertificateTravelEscrows =
            "Cate_TravelingServices_SaveCertificateTravelEscrows";

        private readonly string _cateTravelingServicesSaveHRInformations = "Cate_TravelingServices_SaveHRInformations";

        public int SaveCertificates(CateTravelingServiceCertificateModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateTravelingServicesSaveCertificates,
                DATA_PROVIDER_NAME,
                model.EnterpriseId,
                model.LicenseFile,
                model.DataLicenseFile,
                model.LicenseNumber,
                model.ReleaseDate,
                model.TimeRelease,
                model.Inbound,
                model.Outbound,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }

        public CateTravelingServiceCertificateModel GetCertificateViaEnterprise(int? enterpriseId = null)
        {
            var travelingService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateTravelingServiceCertificateModel>(
                    _cateTravelingServicesGetCertificates, DATA_PROVIDER_NAME, enterpriseId);
            return travelingService;
        }

        public int SaveCertificateTravelEscrows(CateTravelingServiceCertificateTravelEscrowModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateTravelingServicesSaveCertificateTravelEscrows,
                DATA_PROVIDER_NAME,
                model.EnterpriseId,
                model.CertificateId,
                model.DataCertificate,
                model.BankName,
                model.BankAccountNo,
                model.Amount,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }

        public CateTravelingServiceCertificateTravelEscrowModel GetCertificateViaEnterpriseTravelEscrow(
            int? enterpriseId = null)
        {
            var travelingService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateTravelingServiceCertificateTravelEscrowModel>(
                    _cateTravelingServicesGetCertificateTravelEscrows, DATA_PROVIDER_NAME, enterpriseId);
            return travelingService;
        }

        public CateTravelingServiceHRInformationModel GetHRInformationViaEnterprise(int? enterpriseId = null)
        {
            var accommodationService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateTravelingServiceHRInformationModel>(
                    _cateTravelingServicesGetHRInformations, DATA_PROVIDER_NAME, enterpriseId);
            return accommodationService;
        }

        public int SaveHRInformations(CateTravelingServiceHRInformationModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateTravelingServicesSaveHRInformations,
                DATA_PROVIDER_NAME,
                model.EnterpriseId,
                model.TotalStaff,
                model.StaffOnLeader,
                model.StaffOnManager,
                model.TourGuide,
                model.StaffOnLiteracyOnCollege,
                model.StaffOnUniversity,
                model.StaffOnCollege,
                model.StaffOnIntermediate,
                model.StaffOnHighSchool,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }
    }
}