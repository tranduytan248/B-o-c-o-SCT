using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Cate.Traveling;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate.Traveling
{
    public class CateTravelingServiceBranchOfficeBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";

        private readonly string _cateTravelingServicesBranchOfficeDelete =
            "Cate_TravelingServices_BranchOffices_Delete";

        private readonly string _cateTravelingServicesBranchOfficeGet = "Cate_TravelingServices_BranchOffices_Get";

        private readonly string _cateTravelingServicesBranchOfficeGetById =
            "Cate_TravelingServices_BranchOffices_GetByID";

        private readonly string _cateTravelingServicesBranchOfficeSave = "Cate_TravelingServices_BranchOffices_Save";

        public List<CateTravelingServiceBranchOfficeModel> Get(int? enterpriseId, out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var dataTravelingServicesRoom =
                AppProcessor.ProcedureProvider.ExecuteTypedList<CateTravelingServiceBranchOfficeModel>(
                    _cateTravelingServicesBranchOfficeGet, DATA_PROVIDER_NAME,
                    enterpriseId,
                    search.Search,
                    search.Order,
                    search.OrderDir,
                    search.StartIndex,
                    search.PageSize);
            total = 0;
            if (dataTravelingServicesRoom != null && dataTravelingServicesRoom.Count > 0)
                total = int.Parse(dataTravelingServicesRoom.First().TotalRow.ToString());
            return dataTravelingServicesRoom;
        }

        public List<CateTravelingServiceBranchOfficeModel> GetAll(int? enterpriseId = null)
        {
            int total;
            var listpublicCates = Get(enterpriseId, out total, null);
            return listpublicCates;
        }

        public CateTravelingServiceBranchOfficeModel GetById(long? publicCateId)
        {
            var publicCate =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateTravelingServiceBranchOfficeModel>(
                    _cateTravelingServicesBranchOfficeGetById, DATA_PROVIDER_NAME, publicCateId);
            return publicCate;
        }

        public int Save(CateTravelingServiceBranchOfficeModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateTravelingServicesBranchOfficeSave,
                DATA_PROVIDER_NAME,
                model.BranchOfficeId,
                model.EnterpriseId,
                model.BranchName,
                model.BranchAddress,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }

        public bool Delete(CateTravelingServiceBranchOfficeModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateTravelingServicesBranchOfficeDelete,
                DATA_PROVIDER_NAME, model.BranchOfficeId, model.Reason, model.SavedBy);
            return result == model.BranchOfficeId;
        }
    }
}