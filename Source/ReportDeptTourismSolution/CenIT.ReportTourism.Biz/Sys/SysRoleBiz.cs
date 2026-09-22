using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Sys
{
    public class SysRoleBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _sysRoleDelete = "Sys_Role_Delete";
        private readonly string _sysRoleGet = "Sys_Role_Get";
        private readonly string _sysRoleGetById = "Sys_Role_GetById";
        private readonly string _sysRoleSave = "Sys_Role_Save";

        private List<SysRoleModel> Get(out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listRoles = AppProcessor.ProcedureProvider.ExecuteTypedList<SysRoleModel>(_sysRoleGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listRoles != null && listRoles.Count > 0)
                total = int.Parse(listRoles.First()?.TotalRow.ToString() ?? "0");
            return listRoles;
        }

        private SysRoleModel LoadDetail(int roleId)
        {
            var dataRole =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysRoleModel>(_sysRoleGetById, DATA_PROVIDER_NAME,
                    roleId);
            return dataRole;
        }

        public bool Delete(SysRoleModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysRoleDelete, DATA_PROVIDER_NAME, model.RoleId);
            return result == model.RoleId;
        }

        public List<SysRoleModel> GetAll()
        {
            int total;
            var listRoles = Get(out total, null);
            return listRoles;
        }

        public SysRoleModel GetById(int roleId)
        {
            var role = LoadDetail(roleId);
            return role;
        }

        public List<SysRoleModel> GetList(out int total, SysSearchModel search = null)
        {
            var listRoles = Get(out total, search);
            return listRoles;
        }

        public int Save(SysRoleModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysRoleSave, DATA_PROVIDER_NAME,
                model.RoleId,
                model.Name);

            return result.GetValueOrDefault(0);
        }
    }
}