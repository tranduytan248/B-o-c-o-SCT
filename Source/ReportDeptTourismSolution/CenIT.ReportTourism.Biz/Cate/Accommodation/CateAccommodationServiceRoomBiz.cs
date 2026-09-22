using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Cate.Accommodation;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate.Accommodation
{
    public class CateAccommodationServiceRoomBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";

        private readonly string _cateAccommodationServicesInfrastructuresRoomDelete =
            "Cate_AccommodationServices_Infrastructures_Rooms_Delete";

        private readonly string _cateAccommodationServicesInfrastructuresRoomGet =
            "Cate_AccommodationServices_Infrastructures_Rooms_Get";

        private readonly string _cateAccommodationServicesInfrastructuresRoomGetById =
            "Cate_AccommodationServices_Infrastructures_Rooms_GetByID";

        private readonly string _cateAccommodationServicesInfrastructuresRoomSave =
            "Cate_AccommodationServices_Infrastructures_Rooms_Save";

        public List<CateAccommodationServiceInfrastructureRoomModel> Get(int? enterpriseId, out int total,
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
                AppProcessor.ProcedureProvider.ExecuteTypedList<CateAccommodationServiceInfrastructureRoomModel>(
                    _cateAccommodationServicesInfrastructuresRoomGet, DATA_PROVIDER_NAME,
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

        public List<CateAccommodationServiceInfrastructureRoomModel> GetAll(int? enterpriseId = null)
        {
            int total;
            var listpublicCates = Get(enterpriseId, out total, null);
            return listpublicCates;
        }

        public CateAccommodationServiceInfrastructureRoomModel GetById(long? publicCateId)
        {
            var publicCate =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateAccommodationServiceInfrastructureRoomModel>(
                    _cateAccommodationServicesInfrastructuresRoomGetById, DATA_PROVIDER_NAME, publicCateId);
            return publicCate;
        }

        public int Save(CateAccommodationServiceInfrastructureRoomModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateAccommodationServicesInfrastructuresRoomSave,
                DATA_PROVIDER_NAME,
                model.InfrastructureRoomId,
                model.EnterpriseId,
                model.RoomType,
                model.TotalRooms,
                model.AnnouncedPrice,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }

        public bool Delete(CateAccommodationServiceInfrastructureRoomModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateAccommodationServicesInfrastructuresRoomDelete,
                DATA_PROVIDER_NAME, model.InfrastructureRoomId, model.Reason, model.SavedBy);
            return result == model.InfrastructureRoomId;
        }
    }
}