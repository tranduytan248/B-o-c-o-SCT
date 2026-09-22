using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Sys
{
    public class SysMenuBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _sysMenuDelete = "Sys_Menu_Delete";
        private readonly string _sysMenuGet = "Sys_Menu_Get";
        private readonly string _sysMenuGetById = "Sys_Menu_GetById";
        private readonly string _sysMenuGetByUserId = "Sys_Menu_GetByUserId";
        private readonly string _sysMenuGetByUserName = "Sys_Menu_GetByUserName";
        private readonly string _sysMenuGetMenuChilds = "Sys_Menu_GetMenuChilds";
        private readonly string _sysMenuSave = "Sys_Menu_Save";

        private List<SysMenuModel> Get(out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listMenus = AppProcessor.ProcedureProvider.ExecuteTypedList<SysMenuModel>(_sysMenuGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listMenus != null && listMenus.Count > 0)
                total = int.Parse(listMenus.First()?.TotalRow.ToString() ?? "0");
            return listMenus;
        }

        private SysMenuModel LoadDetail(int menuId)
        {
            var dataMenu =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysMenuModel>(_sysMenuGetById, DATA_PROVIDER_NAME,
                    menuId);
            return dataMenu;
        }

        private List<SysMenuModel> Get(int userId)
        {
            var listMenus =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysMenuModel>(_sysMenuGetByUserId, DATA_PROVIDER_NAME,
                    userId);
            return listMenus;
        }

        private List<SysMenuModel> Get(string userName)
        {
            var listMenus =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysMenuModel>(_sysMenuGetByUserName, DATA_PROVIDER_NAME,
                    userName);
            return listMenus;
        }

        private List<SysMenuModel> LoadListChilds(int menuId)
        {
            var listMenus =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysMenuModel>(_sysMenuGetMenuChilds, DATA_PROVIDER_NAME,
                    menuId);
            return listMenus;
        }

        public bool Delete(SysMenuModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysMenuDelete, DATA_PROVIDER_NAME, model.MenuId);
            return result == model.MenuId;
        }

        public List<SysMenuModel> GetAll()
        {
            int total;
            var listMenus = Get(out total, null);
            return listMenus;
        }

        public SysMenuModel GetById(int menuId)
        {
            var menu = LoadDetail(menuId);
            return menu;
        }

        public List<SysMenuModel> GetByUserName(string userName)
        {
            var menus = Get(userName);
            return menus;
        }

        public List<SysMenuModel> GetByUserId(int userId)
        {
            var menus = Get(userId);
            return menus;
        }

        public List<SysMenuModel> GetMenuChilds(int menuId)
        {
            var menus = LoadListChilds(menuId);
            return menus;
        }

        public List<SysMenuModel> GetList(out int total, SysSearchModel search = null)
        {
            var listMenus = Get(out total, search);
            return listMenus;
        }

        public int Save(SysMenuModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysMenuSave, DATA_PROVIDER_NAME,
                model.MenuId,
                model.ParentId,
                model.FunctionActionId,
                model.Name,
                model.Position,
                model.LevelMenu,
                model.Depth,
                model.Link,
                model.Icon,
                model.IsShow);

            return result.GetValueOrDefault(0);
        }
    }
}