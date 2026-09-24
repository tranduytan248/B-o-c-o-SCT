using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate
{
    public class CateEconomicSectorBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _cateEconomicSectorDelete = "Cate_EconomicSector_Delete";
        private readonly string _cateEconomicSectorGet = "Cate_EconomicSector_Get";
        private readonly string _cateEconomicSectorGetByID = "Cate_EconomicSector_GetByID";
        private readonly string _cateEconomicSectorSave = "Cate_EconomicSector_Save";


        public List<CateEconomicSectorModel> Get(out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var dataEconomicSector = AppProcessor.ProcedureProvider.ExecuteTypedList<CateEconomicSectorModel>(_cateEconomicSectorGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);
            total = 0;
            if (dataEconomicSector != null && dataEconomicSector.Count > 0)
                total = int.Parse(dataEconomicSector.First().TotalRow.ToString());
            return dataEconomicSector;
        }

        public int Save(CateEconomicSectorModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateEconomicSectorSave, DATA_PROVIDER_NAME,
                model.EconomicSectorId,
                model.Code,
                model.Name,
                model.DisplayOrder,
                model.IsActive,
                model.UpdatedBy
            );
            return result.GetValueOrDefault(0);
        }

        private CateEconomicSectorModel LoadDetail(int? sectorId)
        {
            var dataEconomicSector =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateEconomicSectorModel>(_cateEconomicSectorGetByID,
                    DATA_PROVIDER_NAME, sectorId);
            return dataEconomicSector;
        }

        public CateEconomicSectorModel GetById(int? sectorId)
        {
            var economicSector = LoadDetail(sectorId);
            return economicSector;
        }

        public bool Delete(CateEconomicSectorModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateEconomicSectorDelete, DATA_PROVIDER_NAME,
                model.EconomicSectorId, model.LastModifiedBy);
            return result == model.EconomicSectorId;
        }
    }
}