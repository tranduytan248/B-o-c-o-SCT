using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate
{
    public class CateWardBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _cateWardDelete = "Cate_Ward_Delete";
        private readonly string _cateWardGet = "Cate_Ward_Get";
        private readonly string _cateWardGetByProvinceId = "Cate_Ward_GetByProvinceId";
        private readonly string _cateWardGetById = "Cate_Ward_GetById";
        private readonly string _cateWardGetByStreetId = "Cate_Ward_GetByStreetId";
        private readonly string _cateWardSave = "Cate_Ward_Save";

        public List<CateWardModel> LoadList(string provinceIds, out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listWards = AppProcessor.ProcedureProvider.ExecuteTypedList<CateWardModel>(_cateWardGet,
                DATA_PROVIDER_NAME,
                provinceIds,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listWards != null && listWards.Count > 0)
                total = int.Parse(listWards.First()?.TotalRow.ToString() ?? "0");
            return listWards;
        }

        private CateWardModel LoadDetail(int? wardId)
        {
            var lstWardModels =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateWardModel>(_cateWardGetById, DATA_PROVIDER_NAME,
                    wardId);
            return lstWardModels;
        }

        public List<CateWardModel> GetAll(string provinceIds = null)
        {
            return LoadList(provinceIds, out _, null);
        }

        public int? Save(CateWardModel model)
        {
            var id = AppProcessor.ProcedureProvider.Execute(_cateWardSave, DATA_PROVIDER_NAME,
                model.WardId,
                model.ProvinceId,
                model.WardCode,
                model.WardName,
                model.UserCreated
            );

            return id;
        }

        public bool Delete(CateWardModel model)
        {
            var result =
                AppProcessor.ProcedureProvider.Execute(_cateWardDelete, DATA_PROVIDER_NAME, model.WardId,
                    model.UserCreated);
            return result == model.WardId;
        }

        public CateWardModel GetById(int? wardId)
        {
            var ward = LoadDetail(wardId);
            return ward;
        }

        public List<CateWardModel> GetByProvinceId(int? districtId, out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var listWards = AppProcessor.ProcedureProvider.ExecuteTypedList<CateWardModel>(_cateWardGetByProvinceId,
                DATA_PROVIDER_NAME, districtId,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);
            total = 0;
            if (listWards != null && listWards.Count > 0)
                total = int.Parse(listWards.First()?.TotalRow.ToString() ?? "0");
            return listWards;
        }

        public List<CateWardModel> GetByStreetId(int? streetId, out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var listWards = AppProcessor.ProcedureProvider.ExecuteTypedList<CateWardModel>(_cateWardGetByStreetId,
                DATA_PROVIDER_NAME, streetId,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);
            total = 0;
            if (listWards != null && listWards.Count > 0)
                total = int.Parse(listWards.First()?.TotalRow.ToString() ?? "0");
            return listWards;
        }

        public List<CateWardModel> GetByStreetId(int? streetId)
        {
            var search = new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var listWards = AppProcessor.ProcedureProvider.ExecuteTypedList<CateWardModel>(_cateWardGetByStreetId,
                DATA_PROVIDER_NAME, streetId,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);
            return listWards;
        }
    }
}