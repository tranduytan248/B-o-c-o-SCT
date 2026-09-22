using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Cate.TouristAttraction;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate.TouristAttraction
{
    public class CateTouristAttractionTypeServiceBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";

        private readonly string _cateTouristAttractionsTypeServiceDelete =
            "Cate_TouristAttractions_TypeServices_Delete";

        private readonly string _cateTouristAttractionsTypeServiceGet = "Cate_TouristAttractions_TypeServices_Get";

        private readonly string _cateTouristAttractionsTypeServiceGetById =
            "Cate_TouristAttractions_TypeServices_GetByID";

        private readonly string _cateTouristAttractionsTypeServiceSave = "Cate_TouristAttractions_TypeServices_Save";

        public List<CateTouristAttractionTypeServiceModel> Get(int? enterpriseId, out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var dataTouristAttractionsTypeService =
                AppProcessor.ProcedureProvider.ExecuteTypedList<CateTouristAttractionTypeServiceModel>(
                    _cateTouristAttractionsTypeServiceGet, DATA_PROVIDER_NAME,
                    enterpriseId,
                    search.Search,
                    search.Order,
                    search.OrderDir,
                    search.StartIndex,
                    search.PageSize);
            total = 0;
            if (dataTouristAttractionsTypeService != null && dataTouristAttractionsTypeService.Count > 0)
                total = int.Parse(dataTouristAttractionsTypeService.First().TotalRow.ToString());
            return dataTouristAttractionsTypeService;
        }

        public List<CateTouristAttractionTypeServiceModel> GetAll(int? enterpriseId = null)
        {
            int total;
            var listTypeServices = Get(enterpriseId, out total, null);
            return listTypeServices;
        }

        public CateTouristAttractionTypeServiceModel GetById(long? typeServiceId)
        {
            var typeService =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateTouristAttractionTypeServiceModel>(
                    _cateTouristAttractionsTypeServiceGetById, DATA_PROVIDER_NAME, typeServiceId);
            return typeService;
        }

        public int Save(CateTouristAttractionTypeServiceModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateTouristAttractionsTypeServiceSave,
                DATA_PROVIDER_NAME,
                model.TouristAttractionTypeServiceId,
                model.EnterpriseId,
                model.ServiceName,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }

        public bool Delete(CateTouristAttractionTypeServiceModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateTouristAttractionsTypeServiceDelete,
                DATA_PROVIDER_NAME, model.TouristAttractionTypeServiceId, model.Reason, model.SavedBy);
            return result == model.TouristAttractionTypeServiceId;
        }
    }
}