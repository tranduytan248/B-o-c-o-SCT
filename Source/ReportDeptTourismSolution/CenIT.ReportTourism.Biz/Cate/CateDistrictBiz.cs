using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate
{
    public class CateDistrictBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _cateDistrictDelete = "Cate_District_Delete";
        private readonly string _cateDistrictGet = "Cate_District_Get";
        private readonly string _cateDistrictGetById = "Cate_District_GetById";
        private readonly string _cateDistrictGetByProvinceCode = "Cate_District_GetByProvinceCode";
        private readonly string _cateDistrictGetByProvinceID = "Cate_District_GetByProvinceID";
        private readonly string _cateDistrictSave = "Cate_District_Save";

        public List<CateDistrictModel> Get(int? provinceId, string ProvincesIds, out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var dataEnterprise = AppProcessor.ProcedureProvider.ExecuteTypedList<CateDistrictModel>(_cateDistrictGet,
                DATA_PROVIDER_NAME,
                provinceId,
                ProvincesIds,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);
            total = 0;
            if (dataEnterprise != null && dataEnterprise.Count > 0)
                total = int.Parse(dataEnterprise.First().TotalRow.ToString());
            return dataEnterprise;
        }

        private CateDistrictModel LoadDetail(int? districtId)
        {
            var lstDistricts =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateDistrictModel>(_cateDistrictGetById,
                    DATA_PROVIDER_NAME, districtId);
            return lstDistricts;
        }

        public bool Delete(CateDistrictModel model)
        {
            var result =
                AppProcessor.ProcedureProvider.Execute(_cateDistrictDelete, DATA_PROVIDER_NAME, model.DistrictId,
                    model.UserCreated);
            return result == model.DistrictId;
        }


        public List<CateDistrictModel> GetAll(int? provinceId, string ProvincesIds = null)
        {
            int total;
            var listDistricts = Get(provinceId, ProvincesIds, out total, null);
            return listDistricts;
        }


        public CateDistrictModel GetById(int? districtId)
        {
            var district = LoadDetail(districtId);
            return district;
        }


        public int? Save(CateDistrictModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateDistrictSave, DATA_PROVIDER_NAME,
                model.DistrictId,
                model.ProvinceId,
                model.DistrictCode,
                model.DistrictName,
                model.UserCreated
            );

            return result;
        }

        public List<CateDistrictModel> GetByProvinceCode(string provinceCode, SysSearchModel search = null)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var listDistricts =
                AppProcessor.ProcedureProvider.ExecuteTypedList<CateDistrictModel>(_cateDistrictGetByProvinceCode,
                    DATA_PROVIDER_NAME, provinceCode, search.Search, search.Order, search.OrderDir, search.StartIndex,
                    search.PageSize);

            var total = 0;
            if (listDistricts != null && listDistricts.Count > 0)
                total = int.Parse(listDistricts.First()?.TotalRow.ToString() ?? "0");
            return listDistricts;
        }

        public List<CateDistrictModel> GetByProvinceID(int? ProvinceId, out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var listDistricts = AppProcessor.ProcedureProvider.ExecuteTypedList<CateDistrictModel>(
                _cateDistrictGetByProvinceID,
                DATA_PROVIDER_NAME,
                ProvinceId,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listDistricts != null && listDistricts.Count > 0)
                total = int.Parse(listDistricts.First()?.TotalRow.ToString() ?? "0");
            return listDistricts;
        }

        public List<CateDistrictModel> GetByProvinceID(int? ProvinceId)
        {
            var search = new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listDistricts = AppProcessor.ProcedureProvider.ExecuteTypedList<CateDistrictModel>(
                _cateDistrictGetByProvinceID,
                DATA_PROVIDER_NAME,
                ProvinceId,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            return listDistricts;
        }
    }
}