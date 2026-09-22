using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CenIT.ReportTourism.Caches.Sys;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Attributes;
using TSFramework.App.BaseApps;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Core.Apps
{
    public class AppController : BaseController
    {
        private const string SESSION_KEY_MODULE_WIDGET = "ModuleWidgetForUser{0}";
        private readonly SysModuleCache _sysModuleCache;
        private Dictionary<string, List<SysPanelModuleModel>> _mappingModuleWidget;

        public AppController()
        {
            var appLayoutCl = new SysLayoutCache();
            _sysModuleCache = new SysModuleCache();
            var activatedLayout = appLayoutCl.GetActivatedLayout();
            if (activatedLayout != null) ViewBag.ActivatedLayout = activatedLayout.LayoutName;
        }

        protected Dictionary<string, List<SysPanelModuleModel>> MappingModuleWidget
        {
            get
            {
                _mappingModuleWidget =
                    Session[string.Format(SESSION_KEY_MODULE_WIDGET, User.UserName)] as
                        Dictionary<string, List<SysPanelModuleModel>> ??
                    new Dictionary<string, List<SysPanelModuleModel>>();
                return _mappingModuleWidget;
            }
            set
            {
                Session[string.Format(SESSION_KEY_MODULE_WIDGET, User.UserName)] = value;
                _mappingModuleWidget = value;
            }
        }

        [ChildActionOnly]
        [ActionType(Type = EnumActionType.View)]
        [AllowAnonymous]
        public ActionResult Menu()
        {
            var sysMenuCache = new SysMenuCache();
            var appMenus = sysMenuCache.GetByUserName(User.UserName);
            var viewMenus = GetViewMenus(appMenus);
            return PartialView("_Menu", CreateViewMenu(viewMenus));
        }

        [NonAction]
        public List<SysMenuViewModel> GetViewMenus(List<SysMenuModel> data, int menuParentId = 0)
        {
            var viewMenus = new List<SysMenuViewModel>();

            if (data == null) return viewMenus;
            var appMenus = menuParentId == 0
                ? data.Where(mn => !mn.ParentId.HasValue).ToList()
                : data.Where(mn => mn.ParentId == menuParentId).ToList();

            foreach (var item in appMenus)
                viewMenus.Add(new SysMenuViewModel
                {
                    Depth = item.Depth,
                    ModuleName = item.ModuleName,
                    FunctionActionId = item.FunctionActionId.GetValueOrDefault(),
                    Icon = item.Icon,
                    Id = item.MenuId,
                    LevelMenu = item.LevelMenu.GetValueOrDefault(),
                    Link = item.Link,
                    Name = item.Name,
                    Position = item.Position.GetValueOrDefault(),
                    Childs = GetViewMenus(data, item.MenuId)
                });

            return viewMenus;
        }

        [NonAction]
        public string CreateViewMenu(List<SysMenuViewModel> data)
        {
            var dataHtml = new StringBuilder();
            if (data == null) return dataHtml.ToString();
            foreach (var menu in data)
                if (menu.Childs.Count > 0)
                    dataHtml.Append("<li class='treeview' id='" + menu.Id + "'>")
                        .Append("<a href='#'>")
                        .Append("<i class='fa " + menu.Icon + "'></i>&nbsp;")
                        .Append("<span > " + menu.Name + " </span>")
                        .Append("<span class='fa fa-angle-left pull-right'></span>")
                        .Append("</a>")
                        .Append("<ul class='treeview-menu'>")
                        .Append(CreateViewMenu(menu.Childs))
                        .Append("</ul>");
                else
                    dataHtml.Append("<li class='treeview' id='" + menu.Id + "'>")
                        .Append("<a href='" + menu.Link + "' name='" + menu.Depth + "'>")
                        .Append("<i class='fa " + menu.Icon + "'></i>&nbsp;")
                        .Append("<span >" + menu.Name + "</span>")
                        .Append("</a></li>");
            return dataHtml.ToString();
        }

        [NonAction]
        protected void InitModulePanel()
        {
            var listModuleContentPanels = _sysModuleCache.GetByUser(User.UserName);
            var mappingModuleWidget = new Dictionary<string, List<SysPanelModuleModel>>();
            listModuleContentPanels?.ForEach(m =>
            {
                var listModules = new List<SysPanelModuleModel>
                {
                    new SysPanelModuleModel
                    {
                        ModuleName = m.ModuleName,
                        ModuleView = m.ModuleView,
                        AssemblyName = m.AssemblyName,
                        MainController = m.MainController,
                        ModuleId = m.ModuleId,
                        OrderBy = m.OrderBy
                    }
                };
                if (mappingModuleWidget.ContainsKey(m.ContentPanelName))
                    mappingModuleWidget[m.ContentPanelName].AddRange(listModules);
                else
                    mappingModuleWidget.Add(m.ContentPanelName, listModules);
            });
            MappingModuleWidget = mappingModuleWidget;
        }
    }
}