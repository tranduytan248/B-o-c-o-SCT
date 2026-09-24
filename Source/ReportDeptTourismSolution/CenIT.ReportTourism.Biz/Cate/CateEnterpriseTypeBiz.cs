using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate
{
    public class CateEnterpriseTypeBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _cateEnterpriseTypeDelete = "Cate_EnterpriseType_Delete";
        private readonly string _cateEnterpriseTypeGet = "Cate_EnterpriseType_Get";
        private readonly string _cateEnterpriseTypeGetByID = "Cate_EnterpriseType_GetByID";
        private readonly string _cateEnterpriseTypeSave = "Cate_EnterpriseType_Save";


        public List<CateEnterpriseTypeModel> Get(out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var dataEnterpriseType = AppProcessor.ProcedureProvider.ExecuteTypedList<CateEnterpriseTypeModel>(_cateEnterpriseTypeGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);
            total = 0;
            if (dataEnterpriseType != null && dataEnterpriseType.Count > 0)
                total = int.Parse(dataEnterpriseType.First().TotalRow.ToString());
            return dataEnterpriseType;
        }

        public int Save(CateEnterpriseTypeModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateEnterpriseTypeSave, DATA_PROVIDER_NAME,
                model.EnterpriseTypeId,
                model.Code,
                model.Name,
                model.DisplayOrder,
                model.IsActive,
                model.UpdatedBy
            );
            return result.GetValueOrDefault(0);
        }

        private CateEnterpriseTypeModel LoadDetail(int? enterpriseTypeId)
        {
            var dataEnterpriseType =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateEnterpriseTypeModel>(_cateEnterpriseTypeGetByID,
                    DATA_PROVIDER_NAME, enterpriseTypeId);
            return dataEnterpriseType;
        }

        public CateEnterpriseTypeModel GetById(int? enterpriseTypeId)
        {
            var enterpriseType = LoadDetail(enterpriseTypeId);
            return enterpriseType;
        }

        public bool Delete(CateEnterpriseTypeModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateEnterpriseTypeDelete, DATA_PROVIDER_NAME,
                model.EnterpriseTypeId, model.UpdatedBy);
            return result == model.EnterpriseTypeId;
        }
    }
}