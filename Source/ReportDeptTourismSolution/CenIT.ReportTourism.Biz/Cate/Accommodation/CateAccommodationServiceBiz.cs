using CenIT.ReportTourism.Models.Cate.Accommodation;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate.Accommodation
{
    public class CateAccommodationServiceBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";

        private readonly string _cateAccommodationServicesGetCertificates =
            "Cate_AccommodationServices_GetCertificates";

        private readonly string _cateAccommodationServicesGetCertificatesViaId =
            "Cate_AccommodationServices_GetCertificatesViaId";

        private readonly string _cateAccommodationServicesGetHRInformations =
            "Cate_AccommodationServices_GetHRInformations";

        private readonly string _cateAccommodationServicesGetInfo = "Cate_AccommodationServices_GetInfo";

        private readonly string _cateAccommodationServicesGetInfrastructures =
            "Cate_AccommodationServices_GetInfrastructures";

        private readonly string _cateAccommodationServicesGetViaEnterprise =
            "Cate_AccommodationServices_GetViaEnterprise";

        private readonly string _cateAccommodationServicesSaveCertificates =
            "Cate_AccommodationServices_SaveCertificates";

        private readonly string _cateAccommodationServicesSaveHRInformations =
            "Cate_AccommodationServices_SaveHRInformations";

        private readonly string _cateAccommodationServicesSaveInfo = "Cate_AccommodationServices_SaveInfo";

        private readonly string _cateAccommodationServicesSaveInfrastructures =
            "Cate_AccommodationServices_SaveInfrastructures";

        public CateAccommodationServiceModel GetViaEnterprise(int? enterpriseId = null)
        {
            var accommodationService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateAccommodationServiceModel>(
                    _cateAccommodationServicesGetViaEnterprise, DATA_PROVIDER_NAME, enterpriseId);
            return accommodationService;
        }

        public CateAccommodationServiceCertificateModel GetCertificatesViaId(int? accommodation)
        {
            var accommodationService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateAccommodationServiceCertificateModel>(
                    _cateAccommodationServicesGetCertificatesViaId, DATA_PROVIDER_NAME, accommodation);
            return accommodationService;
        }

        public int SaveCertificates(CateAccommodationServiceCertificateModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateAccommodationServicesSaveCertificates,
                DATA_PROVIDER_NAME,
                model.EnterpriseId,
                model.SecurityCertificate,
                model.DataSecurityCertificate,
                model.FireProtectionCertificate,
                model.DataFireProtectionCertificate,
                model.EnvironmentalProtectionCertificate,
                model.DataEnvironmentalProtectionCertificate,
                model.HygieneFoodSafetyCertificate,
                model.DataHygieneFoodSafetyCertificate,
                model.ConstructionPermit,
                model.DataConstructionPermit,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }

        public CateAccommodationServiceCertificateModel GetCertificateViaEnterprise(int? enterpriseId = null)
        {
            var accommodationService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateAccommodationServiceCertificateModel>(
                    _cateAccommodationServicesGetCertificates, DATA_PROVIDER_NAME, enterpriseId);
            return accommodationService;
        }

        public CateAccommodationServiceInfrastructureModel GetInfrastructureViaEnterprise(int? enterpriseId = null)
        {
            var accommodationService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateAccommodationServiceInfrastructureModel>(
                    _cateAccommodationServicesGetInfrastructures, DATA_PROVIDER_NAME, enterpriseId);
            return accommodationService;
        }

        public int SaveInfrastructures(CateAccommodationServiceInfrastructureModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateAccommodationServicesSaveInfrastructures,
                DATA_PROVIDER_NAME,
                model.EnterpriseId,
                model.InitialInvestmentCapital,
                model.UpgradeInvestmentCapital,
                model.TotalArea,
                model.TotalConstructionArea,
                model.TotalRoom,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }

        public CateAccommodationServiceHRInformationModel GetHRInformationViaEnterprise(int? enterpriseId = null)
        {
            var accommodationService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateAccommodationServiceHRInformationModel>(
                    _cateAccommodationServicesGetHRInformations, DATA_PROVIDER_NAME, enterpriseId);
            return accommodationService;
        }

        public int SaveHRInformations(CateAccommodationServiceHRInformationModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateAccommodationServicesSaveHRInformations,
                DATA_PROVIDER_NAME,
                model.EnterpriseId,
                model.TotalStaff,
                model.StaffOnLeader,
                model.StaffOnManager,
                model.StaffOnReceptionist,
                model.StaffOnRestaurant,
                model.StaffOnBar,
                model.StaffOnKitchen,
                model.StaffOnRoom,
                model.StaffOnLiteracyOnCollege,
                model.StaffOnUniversity,
                model.StaffOnCollege,
                model.StaffOnIntermediate,
                model.StaffOnHighSchool,
                model.HaveForeignLanguageCertificate,
                model.HaveProfessionalCertificate,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }

        public CateAccommodationServiceModel GetInfoViaEnterprise(int? enterpriseId = null)
        {
            var accommodationService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateAccommodationServiceModel>(
                    _cateAccommodationServicesGetInfo, DATA_PROVIDER_NAME, enterpriseId);
            return accommodationService;
        }

        public int SaveInfo(CateAccommodationServiceModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateAccommodationServicesSaveInfo, DATA_PROVIDER_NAME,
                model.AccommodationServiceId,
                model.EnterpriseId,
                model.AccommodationClass,
                model.AccommodationClassCertificate,
                model.DataAccommodationClassCertificate,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }
    }
}