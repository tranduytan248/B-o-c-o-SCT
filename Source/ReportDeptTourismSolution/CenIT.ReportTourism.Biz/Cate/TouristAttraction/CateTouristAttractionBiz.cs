using CenIT.ReportTourism.Models.Cate.TouristAttraction;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate.TouristAttraction
{
    public class CateTouristAttractionBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _cateTouristAttractionsGetCertificates = "Cate_TouristAttractions_GetCertificates";

        private readonly string _cateTouristAttractionsGetHRInformations = "Cate_TouristAttractions_GetHRInformations";

        private readonly string _cateTouristAttractionsGetInfo = "Cate_TouristAttractions_GetInfo";

        private readonly string _cateTouristAttractionsGetInfrastructures =
            "Cate_TouristAttractions_GetInfrastructures";

        private readonly string _cateTouristAttractionsSaveCertificates = "Cate_TouristAttractions_SaveCertificates";

        private readonly string _cateTouristAttractionsSaveHRInformations =
            "Cate_TouristAttractions_SaveHRInformations";

        private readonly string _cateTouristAttractionsSaveInfo = "Cate_TouristAttractions_SaveInfo";

        private readonly string _cateTouristAttractionsSaveInfrastructures =
            "Cate_TouristAttractions_SaveInfrastructures";

        public int SaveCertificates(CateTouristAttractionCertificateModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateTouristAttractionsSaveCertificates,
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

        public CateTouristAttractionCertificateModel GetCertificateViaEnterprise(int? enterpriseId = null)
        {
            var transportTouristsService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateTouristAttractionCertificateModel>(
                    _cateTouristAttractionsGetCertificates, DATA_PROVIDER_NAME, enterpriseId);
            return transportTouristsService;
        }

        public CateTouristAttractionHRInformationModel GetHRInformationViaEnterprise(int? enterpriseId = null)
        {
            var transportTouristsService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateTouristAttractionHRInformationModel>(
                    _cateTouristAttractionsGetHRInformations, DATA_PROVIDER_NAME, enterpriseId);
            return transportTouristsService;
        }

        public int SaveHRInformations(CateTouristAttractionHRInformationModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateTouristAttractionsSaveHRInformations,
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

        public CateTouristAttractionInfrastructureModel GetInfrastructureViaEnterprise(int? enterpriseId = null)
        {
            var transportTouristsService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateTouristAttractionInfrastructureModel>(
                    _cateTouristAttractionsGetInfrastructures, DATA_PROVIDER_NAME, enterpriseId);
            return transportTouristsService;
        }

        public int SaveInfrastructures(CateTouristAttractionInfrastructureModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateTouristAttractionsSaveInfrastructures,
                DATA_PROVIDER_NAME,
                model.EnterpriseId,
                model.InitialInvestmentCapital,
                model.UpgradeInvestmentCapital,
                model.TotalArea,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }

        public CateTouristAttractionModel GetInfoViaEnterprise(int? enterpriseId = null)
        {
            var touristAttraction =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateTouristAttractionModel>(
                    _cateTouristAttractionsGetInfo, DATA_PROVIDER_NAME, enterpriseId);
            return touristAttraction;
        }

        public int SaveInfo(CateTouristAttractionModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateTouristAttractionsSaveInfo, DATA_PROVIDER_NAME,
                model.TouristAttractionId,
                model.EnterpriseId,
                model.TypeTourismActivities,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }
    }
}