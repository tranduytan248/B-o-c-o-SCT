using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate
{
    public class CateBusinessProductBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _cateBusinessProductDelete = "Cate_BusinessProduct_Delete";
        private readonly string _cateBusinessProductGet = "Cate_BusinessProduct_Get";
        private readonly string _cateBusinessProductGetByID = "Cate_BusinessProduct_GetByID";
        private readonly string _cateBusinessProductSave = "Cate_BusinessProduct_Save";


        public List<CateBusinessProductModel> Get(out int total, int? industryId, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var dataBusinessProduct = AppProcessor.ProcedureProvider.ExecuteTypedList<CateBusinessProductModel>(_cateBusinessProductGet,
                DATA_PROVIDER_NAME, industryId,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);
            total = 0;
            if (dataBusinessProduct != null && dataBusinessProduct.Count > 0)
                total = int.Parse(dataBusinessProduct.First().TotalRow.ToString());
            return dataBusinessProduct;
        }

        public int Save(CateBusinessProductModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateBusinessProductSave, DATA_PROVIDER_NAME,
                model.ProductId,
                model.ProductCode,
                model.ProductName,
                model.IndustryId,
                model.ParentId,
                model.Unit,
                model.DisplayOrder,
                model.IsActive,
                model.UpdatedBy
            );
            return result.GetValueOrDefault(0);
        }

        private CateBusinessProductModel LoadDetail(int? productId)
        {
            var dataBusinessProduct =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateBusinessProductModel>(_cateBusinessProductGetByID,
                    DATA_PROVIDER_NAME, productId);
            return dataBusinessProduct;
        }

        public CateBusinessProductModel GetById(int? productId)
        {
            var product = LoadDetail(productId);
            return product;
        }

        public bool Delete(CateBusinessProductModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateBusinessProductDelete, DATA_PROVIDER_NAME,
                model.ProductId, model.UpdatedBy);
            return result == model.ProductId;
        }
    }
}