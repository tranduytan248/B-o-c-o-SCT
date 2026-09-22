using System.Collections.Generic;
using System.Data;
using System.Linq;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;
using TSFramework.Core.Utils;

namespace CenIT.ReportTourism.Biz.Sys
{
    public class SysModuleBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _sysModuleContentPanelDeleteModule = "Sys_ModuleContentPanel_DeleteModule";
        private readonly string _sysModuleContentPanelGetByLayout = "Sys_ModuleContentPanel_GetByLayout";
        private readonly string _sysModuleContentPanelGetByUser = "Sys_ModuleContentPanel_GetByUser";
        private readonly string _sysModuleContentPanelSave = "Sys_ModuleContentPanel_Save";
        private readonly string _sysModuleContentPanelSaveList = "Sys_ModuleContentPanel_SaveList";
        private readonly string _sysModuleDelete = "Sys_Module_Delete";
        private readonly string _sysModuleGet = "Sys_Module_Get";
        private readonly string _sysModuleGetById = "Sys_Module_GetById";
        private readonly string _sysModuleGetByUser = "Sys_Module_GetByUser";
        private readonly string _sysModuleGetNotInContentPanel = "Sys_Module_GetNotInContentPanel";
        private readonly string _sysModulePermissionGetUsers = "Sys_Module_Permission_GetUsers";
        private readonly string _sysModulePermissionSave = "Sys_Module_Permission_Save";
        private readonly string _sysModuleSave = "Sys_Module_Save";

        public List<SysModuleContentPanelModel> GetByLayoutName(string layoutName)
        {
            var dataContentPanel =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysModuleContentPanelModel>(
                    _sysModuleContentPanelGetByLayout, DATA_PROVIDER_NAME, layoutName);
            return dataContentPanel;
        }

        public List<SysModuleModel> GetNotInContentPanel(string contentPanelName)
        {
            var dataModules =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysModuleModel>(_sysModuleGetNotInContentPanel,
                    DATA_PROVIDER_NAME, contentPanelName);
            return dataModules;
        }

        public List<SysModuleContentPanelModel> GetByUser(string userName)
        {
            var dataContentPanel =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysModuleContentPanelModel>(
                    _sysModuleContentPanelGetByUser, DATA_PROVIDER_NAME, userName);
            return dataContentPanel;
        }

        private List<SysModuleModel> Get(out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listModules = AppProcessor.ProcedureProvider.ExecuteTypedList<SysModuleModel>(_sysModuleGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listModules != null && listModules.Count > 0)
                total = int.Parse(listModules.First()?.TotalRow.ToString() ?? "0");
            return listModules;
        }

        private SysModuleModel LoadDetail(int layoutId)
        {
            var dataModule =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysModuleModel>(_sysModuleGetById,
                    DATA_PROVIDER_NAME, layoutId);
            return dataModule;
        }

        public bool Delete(SysModuleModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysModuleDelete, DATA_PROVIDER_NAME, model.ModuleId);
            return result == model.ModuleId;
        }

        public bool DeleteContentPanelModule(int moduleId, string contentPanelName, string updater)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysModuleContentPanelDeleteModule, DATA_PROVIDER_NAME,
                moduleId, contentPanelName, updater);
            return result > 0;
        }

        public List<SysModuleModel> GetAll()
        {
            int total;
            var listModules = Get(out total, null);
            return listModules;
        }

        public SysModuleModel GetById(int layoutId)
        {
            var layout = LoadDetail(layoutId);
            return layout;
        }

        public List<SysModuleModel> GetList(out int total, SysSearchModel search = null)
        {
            var listModules = Get(out total, search);
            return listModules;
        }

        public int Save(SysModuleModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysModuleSave, DATA_PROVIDER_NAME,
                model.ModuleId,
                model.ModuleName,
                model.AssemblyName,
                model.MainController,
                model.ModuleView,
                model.Description,
                model.Icon,
                model.Creator,
                model.Updater
            );

            return result.GetValueOrDefault(0);
        }

        public int SaveContentPanelModule(string contentPanelName, SysModuleModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysModuleContentPanelSave, DATA_PROVIDER_NAME,
                contentPanelName,
                model.ModuleId,
                model.Updater
            );

            return result.GetValueOrDefault(0);
        }

        public int SaveListContentPanelModule(DataTable dataContentPanelModules, string creator)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysModuleContentPanelSaveList, DATA_PROVIDER_NAME,
                dataContentPanelModules,
                creator
            );

            return result.GetValueOrDefault(0);
        }

        public int SavePermissionModule(SysPermissionModuleModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysModulePermissionSave, DATA_PROVIDER_NAME,
                model.ModuleId,
                EString.SplitToTable(model.PermissionUserIDs, new[] { ',' })
            );

            return result.GetValueOrDefault(0);
        }

        public List<SysUserModel> GetPermissionUsers(int moduleId)
        {
            var lstUsers =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysUserModel>(_sysModulePermissionGetUsers,
                    DATA_PROVIDER_NAME, moduleId);
            return lstUsers;
        }

        public List<SysModuleModel> GetByUserName(string userName)
        {
            var dataModules =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysModuleModel>(_sysModuleGetByUser, DATA_PROVIDER_NAME,
                    userName);
            return dataModules;
        }
    }
}