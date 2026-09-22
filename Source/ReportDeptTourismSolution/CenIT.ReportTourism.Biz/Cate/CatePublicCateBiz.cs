using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate
{
    public class CatePublicCateBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _catePublicCateDelete = "Cate_PublicCates_Delete";
        private readonly string _catePublicCateGet = "Cate_PublicCates_Get";
        private readonly string _catePublicCateGetById = "Cate_PublicCates_GetByID";
        private readonly string _catePublicCateSave = "Cate_PublicCates_Save";

        public List<CatePublicCateModel> Get(string cateTypeIds, out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var dataPublicCate = AppProcessor.ProcedureProvider.ExecuteTypedList<CatePublicCateModel>(
                _catePublicCateGet, DATA_PROVIDER_NAME,
                cateTypeIds,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);
            total = 0;
            if (dataPublicCate != null && dataPublicCate.Count > 0)
                total = int.Parse(dataPublicCate.First().TotalRow.ToString());
            return dataPublicCate;
        }

        public List<CatePublicCateModel> GetAll(int? cateTypeId = null)
        {
            var listpublicCates = Get(cateTypeId?.ToString(), out _, null);
            return listpublicCates;
        }

        public CatePublicCateModel GetById(long? publicCateId)
        {
            var publicCate =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CatePublicCateModel>(_catePublicCateGetById,
                    DATA_PROVIDER_NAME, publicCateId);
            return publicCate;
        }

        public int Save(CatePublicCateModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_catePublicCateSave, DATA_PROVIDER_NAME,
                model.CateId,
                model.CateName,
                model.CateTypeId,
                model.CateTypeName,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }

        public bool Delete(CatePublicCateModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_catePublicCateDelete, DATA_PROVIDER_NAME, model.CateId,
                model.SavedBy);
            return result == model.CateId;
        }
    }
}