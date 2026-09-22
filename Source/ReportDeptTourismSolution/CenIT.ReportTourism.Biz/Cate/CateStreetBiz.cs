using System.Collections.Generic;
using System.Data;
using System.Linq;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate
{
    public class CateStreetBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _cateStreetDelete = "Cate_Street_Delete";
        private readonly string _cateStreetGet = "Cate_Street_Get";
        private readonly string _cateStreetGetById = "Cate_Street_GetById";
        private readonly string _cateStreetGetByWard = "Cate_Street_GetByWard";
        private readonly string _cateStreetSave = "Cate_Street_Save";

        public List<CateStreetModel> LoadList(out int total, int? provinceId, int? wardId, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listStreets = AppProcessor.ProcedureProvider.ExecuteTypedList<CateStreetModel>(_cateStreetGet,
                DATA_PROVIDER_NAME,
                provinceId,
                wardId,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listStreets != null && listStreets.Count > 0)
                total = int.Parse(listStreets.First()?.TotalRow.ToString() ?? "0");
            return listStreets;
        }

        public List<CateStreetModel> GetByWard(int? wardId, out int total, SysSearchModel search)
        {
            var listStreets = AppProcessor.ProcedureProvider.ExecuteTypedList<CateStreetModel>(_cateStreetGetByWard,
                DATA_PROVIDER_NAME,
                wardId,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listStreets != null && listStreets.Count > 0)
                total = int.Parse(listStreets.First()?.TotalRow.ToString() ?? "0");
            return listStreets;
        }

        public List<CateStreetModel> GetByWard(int? wardId)
        {
            var search = new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listStreets = AppProcessor.ProcedureProvider.ExecuteTypedList<CateStreetModel>(_cateStreetGetByWard,
                DATA_PROVIDER_NAME,
                wardId,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            return listStreets;
        }

        private CateStreetModel LoadDetail(int? streetId)
        {
            var lstStreetModels =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateStreetModel>(_cateStreetGetById,
                    DATA_PROVIDER_NAME, streetId);
            return lstStreetModels;
        }

        public int? Save(CateStreetModel model, DataTable table)
        {
            var id = AppProcessor.ProcedureProvider.Execute(_cateStreetSave, DATA_PROVIDER_NAME,
                model.StreetId,
                model.ParentId,
                model.StreetCode,
                model.StreetName,
                table,
                model.UserCreated
            );

            return id;
        }

        public bool Delete(CateStreetModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateStreetDelete, DATA_PROVIDER_NAME, model.StreetId,
                model.UserCreated);
            return result == model.StreetId;
        }

        public CateStreetModel GetById(int? streetId)
        {
            var street = LoadDetail(streetId);
            return street;
        }

        public List<CateStreetModel> GetAll(int? provinceId, int? wardId)
        {
            return LoadList(out _, provinceId, wardId, null);
        }
    }
}