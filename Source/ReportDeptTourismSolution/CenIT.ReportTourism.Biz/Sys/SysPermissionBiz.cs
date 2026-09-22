using System.Collections.Generic;
using System.Data;
using System.Linq;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Sys
{
    public class SysPermissionBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _sysPermissionGet = "Sys_Permission_Get";
        private readonly string _sysPermissionGetByRoleId = "Sys_Permission_GetByRoleId";
        private readonly string _sysPermissionGetViaUser = "Sys_Permission_GetViaUser";
        private readonly string _sysPermissionIsAllow = "Sys_Permission_IsAllow";
        private readonly string _sysRoleSavePermission = "Sys_Permission_Save";

        private List<SysPermissionModel> Get(out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listPermissions = AppProcessor.ProcedureProvider.ExecuteTypedList<SysPermissionModel>(_sysPermissionGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listPermissions != null && listPermissions.Count > 0)
                total = int.Parse(listPermissions.First()?.TotalRow.ToString() ?? "0");
            return listPermissions;
        }

        public List<SysPermissionModel> GetByRoleId(int groupId)
        {
            var permissions =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysPermissionModel>(_sysPermissionGetByRoleId,
                    DATA_PROVIDER_NAME, groupId);
            return permissions;
        }

        public bool Save(int groupId, DataTable permission, string separated)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysRoleSavePermission, DATA_PROVIDER_NAME,
                groupId,
                permission,
                separated);

            return result > 0;
        }

        public bool IsAllow(string userName, string areaName, string controllerName, string actionName)
        {
            var dataPermissions = AppProcessor.ProcedureProvider.ExecuteTypedList<SysPermissionModel>(
                _sysPermissionIsAllow, DATA_PROVIDER_NAME, userName,
                areaName, controllerName, actionName);

            return dataPermissions != null && dataPermissions.Count > 0;
        }

        public List<SysUserPermissionModel> GetViaUser(string userName)
        {
            var lstPermissions =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysUserPermissionModel>(_sysPermissionGetViaUser,
                    DATA_PROVIDER_NAME, userName);
            return lstPermissions;
        }
    }
}