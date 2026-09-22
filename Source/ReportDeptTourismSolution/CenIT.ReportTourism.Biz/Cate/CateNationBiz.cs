using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate
{
    public class CateNationBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _cateNationalDelete = "Cate_National_Delete";
        private readonly string _cateNationalGet = "Cate_National_Get";
        private readonly string _cateNationalGetByID = "Cate_National_GetByID";
        private readonly string _cateNationalSave = "Cate_National_Save";


        public List<CateNationalModel> Get(out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var dataNational = AppProcessor.ProcedureProvider.ExecuteTypedList<CateNationalModel>(_cateNationalGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);
            total = 0;
            if (dataNational != null && dataNational.Count > 0)
                total = int.Parse(dataNational.First().TotalRow.ToString());
            return dataNational;
        }

        public int Save(CateNationalModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateNationalSave, DATA_PROVIDER_NAME,
                model.NationalId,
                model.NationalCode,
                model.NationalName,
                model.ContinentId,
                model.ContinentName,
                model.CreatedBy,
                model.LastModifiedBy
            );
            return result.GetValueOrDefault(0);
        }

        private CateNationalModel LoadDetail(int? nationalID)
        {
            var dataNational =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateNationalModel>(_cateNationalGetByID,
                    DATA_PROVIDER_NAME, nationalID);
            return dataNational;
        }

        public CateNationalModel GetById(int? nationalID)
        {
            var nations = LoadDetail(nationalID);
            return nations;
        }

        public bool Delete(CateNationalModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateNationalDelete, DATA_PROVIDER_NAME,
                model.NationalId, model.LastModifiedBy);
            return result == model.NationalId;
        }
    }
}