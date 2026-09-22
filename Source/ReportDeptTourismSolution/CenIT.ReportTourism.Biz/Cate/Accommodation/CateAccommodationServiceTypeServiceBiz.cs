using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Cate.Accommodation;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate.Accommodation
{
    public class CateAccommodationServiceTypeServiceBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";

        private readonly string _cateAccommodationServicesTypeServiceDelete =
            "Cate_AccommodationServices_TypeServices_Delete";

        private readonly string _cateAccommodationServicesTypeServiceGet =
            "Cate_AccommodationServices_TypeServices_Get";

        private readonly string _cateAccommodationServicesTypeServiceGetById =
            "Cate_AccommodationServices_TypeServices_GetByID";

        private readonly string _cateAccommodationServicesTypeServiceSave =
            "Cate_AccommodationServices_TypeServices_Save";

        public List<CateAccommodationServiceTypeServiceModel> Get(int? enterpriseId, out int total,
            SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var dataAccommodationServicesRoom =
                AppProcessor.ProcedureProvider.ExecuteTypedList<CateAccommodationServiceTypeServiceModel>(
                    _cateAccommodationServicesTypeServiceGet, DATA_PROVIDER_NAME,
                    enterpriseId,
                    search.Search,
                    search.Order,
                    search.OrderDir,
                    search.StartIndex,
                    search.PageSize);
            total = 0;
            if (dataAccommodationServicesRoom != null && dataAccommodationServicesRoom.Count > 0)
                total = int.Parse(dataAccommodationServicesRoom.First().TotalRow.ToString());
            return dataAccommodationServicesRoom;
        }

        public List<CateAccommodationServiceTypeServiceModel> GetAll(int? enterpriseId = null)
        {
            int total;
            var listpublicCates = Get(enterpriseId, out total, null);
            return listpublicCates;
        }

        public CateAccommodationServiceTypeServiceModel GetById(long? publicCateId)
        {
            var publicCate =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateAccommodationServiceTypeServiceModel>(
                    _cateAccommodationServicesTypeServiceGetById, DATA_PROVIDER_NAME, publicCateId);
            return publicCate;
        }

        public int Save(CateAccommodationServiceTypeServiceModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateAccommodationServicesTypeServiceSave,
                DATA_PROVIDER_NAME,
                model.AccommodationTypeServiceId,
                model.EnterpriseId,
                model.ServiceId,
                model.ServiceName,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }

        public bool Delete(CateAccommodationServiceTypeServiceModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateAccommodationServicesTypeServiceDelete,
                DATA_PROVIDER_NAME, model.AccommodationTypeServiceId, model.Reason, model.SavedBy);
            return result == model.AccommodationTypeServiceId;
        }
    }
}