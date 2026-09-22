using CenIT.ReportTourism.Models.Cate.ServicesForTourist;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate.ServicesForTourist
{
    public class CateServicesForTouristBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _cateServicesForTouristsGetCertificates = "Cate_ServicesForTourists_GetCertificates";

        private readonly string _cateServicesForTouristsGetHRInformations =
            "Cate_ServicesForTourists_GetHRInformations";

        private readonly string _cateServicesForTouristsGetInfo = "Cate_ServicesForTourists_GetInfo";

        private readonly string _cateServicesForTouristsGetInfrastructures =
            "Cate_ServicesForTourists_GetInfrastructures";

        private readonly string _cateServicesForTouristsSaveCertificates = "Cate_ServicesForTourists_SaveCertificates";

        private readonly string _cateServicesForTouristsSaveHRInformations =
            "Cate_ServicesForTourists_SaveHRInformations";

        private readonly string _cateServicesForTouristsSaveInfo = "Cate_ServicesForTourists_SaveInfo";

        private readonly string _cateServicesForTouristsSaveInfrastructures =
            "Cate_ServicesForTourists_SaveInfrastructures";

        public int SaveCertificates(CateServicesForTouristCertificateModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateServicesForTouristsSaveCertificates,
                DATA_PROVIDER_NAME,
                model.EnterpriseId,
                model.LicenseFile,
                model.DataLicenseFile,
                model.LicenseNumber,
                model.ReleaseDate,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }

        public CateServicesForTouristCertificateModel GetCertificateViaEnterprise(int? enterpriseId = null)
        {
            var transportTouristsService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateServicesForTouristCertificateModel>(
                    _cateServicesForTouristsGetCertificates, DATA_PROVIDER_NAME, enterpriseId);
            return transportTouristsService;
        }

        public CateServicesForTouristHRInformationModel GetHRInformationViaEnterprise(int? enterpriseId = null)
        {
            var transportTouristsService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateServicesForTouristHRInformationModel>(
                    _cateServicesForTouristsGetHRInformations, DATA_PROVIDER_NAME, enterpriseId);
            return transportTouristsService;
        }

        public int SaveHRInformations(CateServicesForTouristHRInformationModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateServicesForTouristsSaveHRInformations,
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

        public CateServicesForTouristInfrastructureModel GetInfrastructureViaEnterprise(int? enterpriseId = null)
        {
            var transportTouristsService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateServicesForTouristInfrastructureModel>(
                    _cateServicesForTouristsGetInfrastructures, DATA_PROVIDER_NAME, enterpriseId);
            return transportTouristsService;
        }

        public int SaveInfrastructures(CateServicesForTouristInfrastructureModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateServicesForTouristsSaveInfrastructures,
                DATA_PROVIDER_NAME,
                model.EnterpriseId,
                model.InitialInvestmentCapital,
                model.UpgradeInvestmentCapital,
                model.TotalArea,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }

        public CateServicesForTouristModel GetInfoViaEnterprise(int? enterpriseId = null)
        {
            var touristAttraction =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateServicesForTouristModel>(
                    _cateServicesForTouristsGetInfo, DATA_PROVIDER_NAME, enterpriseId);
            return touristAttraction;
        }

        public int SaveInfo(CateServicesForTouristModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateServicesForTouristsSaveInfo, DATA_PROVIDER_NAME,
                model.ServicesForTouristId,
                model.EnterpriseId,
                model.TypeServicesForTourists,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }
    }
}