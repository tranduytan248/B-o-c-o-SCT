using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate
{
    public class CateBusinessIndustryBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _cateBusinessIndustryDelete = "Cate_BusinessIndustry_Delete";
        private readonly string _cateBusinessIndustryGet = "Cate_BusinessIndustry_Get";
        private readonly string _cateBusinessIndustryGetByID = "Cate_BusinessIndustry_GetByID";
        private readonly string _cateBusinessIndustrySave = "Cate_BusinessIndustry_Save";


        public List<CateBusinessIndustryModel> Get(out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var dataBusinessIndustry = AppProcessor.ProcedureProvider.ExecuteTypedList<CateBusinessIndustryModel>(_cateBusinessIndustryGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);
            total = 0;
            if (dataBusinessIndustry != null && dataBusinessIndustry.Count > 0)
                total = int.Parse(dataBusinessIndustry.First().TotalRow.ToString());
            return dataBusinessIndustry;
        }

        public int Save(CateBusinessIndustryModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateBusinessIndustrySave, DATA_PROVIDER_NAME,
                model.IndustryId,
                model.IndustryCode,
                model.IndustryName,
                model.ParentId,
                model.DisplayOrder,
                model.IsActive,
                model.UpdatedBy
            );
            return result.GetValueOrDefault(0);
        }

        private CateBusinessIndustryModel LoadDetail(int? industryId)
        {
            var dataBusinessIndustry =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateBusinessIndustryModel>(_cateBusinessIndustryGetByID,
                    DATA_PROVIDER_NAME, industryId);
            return dataBusinessIndustry;
        }

        public CateBusinessIndustryModel GetById(int? industryId)
        {
            var industry = LoadDetail(industryId);
            return industry;
        }

        public bool Delete(CateBusinessIndustryModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateBusinessIndustryDelete, DATA_PROVIDER_NAME,
                model.IndustryId, model.UpdatedBy);
            return result == model.IndustryId;
        }
    }
}