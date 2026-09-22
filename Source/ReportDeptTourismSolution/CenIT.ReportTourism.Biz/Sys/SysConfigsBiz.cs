using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Sys
{
    public class SysConfigsBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _sysConfigsDelete = "Sys_Configs_Delete";
        private readonly string _sysConfigsGet = "Sys_Configs_Get";
        private readonly string _sysConfigsGetByID = "Sys_Configs_GetByID";
        private readonly string _sysConfigsGetViaKey = "Sys_Configs_GetViaKey";
        private readonly string _sysConfigsSave = "Sys_Configs_Save";

        private List<SysConfigsModel> Get(out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listConfigs = AppProcessor.ProcedureProvider.ExecuteTypedList<SysConfigsModel>(_sysConfigsGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listConfigs != null && listConfigs.Count > 0)
                total = int.Parse(listConfigs.First()?.TotalRow.ToString() ?? "0");
            return listConfigs;
        }

        private SysConfigsModel LoadDetail(int configId)
        {
            var dataConfigs =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysConfigsModel>(_sysConfigsGetByID,
                    DATA_PROVIDER_NAME, configId);
            return dataConfigs;
        }

        public SysConfigsModel GetViaKey(string configKey)
        {
            var dataConfig =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysConfigsModel>(_sysConfigsGetViaKey,
                    DATA_PROVIDER_NAME, configKey);
            return dataConfig;
        }

        public bool Delete(SysConfigsModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysConfigsDelete, DATA_PROVIDER_NAME, model.ConfigId,
                model.DeletedBy);
            return result == model.ConfigId;
        }

        public List<SysConfigsModel> GetAll()
        {
            int total;
            var listConfigs = Get(out total, null);
            return listConfigs;
        }

        public SysConfigsModel GetById(int configId)
        {
            var configs = LoadDetail(configId);
            return configs;
        }


        public List<SysConfigsModel> GetList(out int total, SysSearchModel search = null)
        {
            var listConfigs = Get(out total, search);
            return listConfigs;
        }

        public int Save(SysConfigsModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysConfigsSave, DATA_PROVIDER_NAME,
                model.ConfigId,
                model.ConfigKey,
                model.ConfigValue,
                model.ConfigDesc,
                model.CreatedBy,
                model.LastModifiedBy);

            return result.GetValueOrDefault(0);
        }
    }
}