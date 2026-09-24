using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate
{
    public class CateEnterpriseStatusBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _cateEnterpriseStatusDelete = "Cate_EnterpriseStatus_Delete";
        private readonly string _cateEnterpriseStatusGet = "Cate_EnterpriseStatus_Get";
        private readonly string _cateEnterpriseStatusGetByID = "Cate_EnterpriseStatus_GetByID";
        private readonly string _cateEnterpriseStatusSave = "Cate_EnterpriseStatus_Save";


        public List<CateEnterpriseStatusModel> Get(out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var dataEnterpriseStatus = AppProcessor.ProcedureProvider.ExecuteTypedList<CateEnterpriseStatusModel>(_cateEnterpriseStatusGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);
            total = 0;
            if (dataEnterpriseStatus != null && dataEnterpriseStatus.Count > 0)
                total = int.Parse(dataEnterpriseStatus.First().TotalRow.ToString());
            return dataEnterpriseStatus;
        }

        public int Save(CateEnterpriseStatusModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateEnterpriseStatusSave, DATA_PROVIDER_NAME,
                model.EnterpriseStatusId,
                model.Code,
                model.Name,
                model.DisplayOrder,
                model.IsActive,
                model.UpdatedBy
            );
            return result.GetValueOrDefault(0);
        }

        private CateEnterpriseStatusModel LoadDetail(int? statusId)
        {
            var dataEnterpriseStatus =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateEnterpriseStatusModel>(_cateEnterpriseStatusGetByID,
                    DATA_PROVIDER_NAME, statusId);
            return dataEnterpriseStatus;
        }

        public CateEnterpriseStatusModel GetById(int? statusId)
        {
            var enterpriseStatus = LoadDetail(statusId);
            return enterpriseStatus;
        }

        public bool Delete(CateEnterpriseStatusModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateEnterpriseStatusDelete, DATA_PROVIDER_NAME,
                model.EnterpriseStatusId, model.LastModifiedBy);
            return result == model.EnterpriseStatusId;
        }
    }
}