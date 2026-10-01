using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CenIT.ReportTourism.Caches.Sys;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Attributes;
using TSFramework.App.BaseApps;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Core.Apps
{
    public class AppController : BaseController
    {
        private const string SESSION_KEY_MODULE_WIDGET = "ModuleWidgetForUser{0}";
        private const string SESSION_KEY_REQUIRE_CHANGE_PASSWORD = "RequireChangePassword{0}";

        // Tài khoản không phải đổi mật khẩu: sau khoảng thời gian này đọc lại DB (quản trị có thể vừa cấp lại mật khẩu)
        private static readonly TimeSpan RequireChangePasswordRecheck = TimeSpan.FromMinutes(5);

        // Các action của Account (area gốc) mà tài khoản phải đổi mật khẩu vẫn được dùng
        private static readonly string[] RequireChangePasswordAllowedAccountActions =
            { "RequireChangePassword", "Login", "Logout", "ForgotPassword", "ResetPassword" };

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

        /// <summary>
        ///     Tài khoản phải đổi mật khẩu (đăng nhập lần đầu / được quản trị cấp lại mật khẩu) thì chỉ được vào
        ///     trang đổi mật khẩu (Account/RequireChangePassword), đăng nhập/đăng xuất, quên mật khẩu và trang lỗi.
        /// </summary>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result != null) return;
            if (User == null || !User.Identity.IsAuthenticated || string.IsNullOrEmpty(User.UserName)) return;

            // Child action trong layout (menu, thông báo...) không hiển thị khi chưa đổi mật khẩu
            if (filterContext.IsChildAction)
            {
                if (IsRequireChangePassword(User.UserName)) filterContext.Result = new EmptyResult();
                return;
            }

            var areaName = filterContext.RouteData.DataTokens["area"] as string;
            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            if (string.IsNullOrEmpty(areaName))
            {
                if (string.Equals(controllerName, "Error", StringComparison.OrdinalIgnoreCase)) return;
                if (string.Equals(controllerName, "Account", StringComparison.OrdinalIgnoreCase) &&
                    RequireChangePasswordAllowedAccountActions.Contains(filterContext.ActionDescriptor.ActionName,
                        StringComparer.OrdinalIgnoreCase)) return;
            }

            if (!IsRequireChangePassword(User.UserName)) return;

            var changePasswordUrl = Url.Action("RequireChangePassword", "Account", new { area = "" });
            if (Request.IsAjaxRequest())
            {
                filterContext.Result = Json(new
                {
                    status = false,
                    returnUrl = changePasswordUrl,
                    message = CreateMessage("Vui lòng đổi mật khẩu trước khi sử dụng hệ thống.",
                                  EnumProcessType.NonFormat, EnumMsgIcon.Warning) +
                              $"window.location.href = '{changePasswordUrl}';"
                }, JsonRequestBehavior.AllowGet);
                return;
            }

            var returnUrl = string.Equals(Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase)
                ? Request.RawUrl
                : null;
            filterContext.Result = new RedirectResult(
                Url.Action("RequireChangePassword", "Account", new { area = "", returnUrl }));
        }

        /// <summary>
        ///     Tài khoản có phải đổi mật khẩu không (cache theo Session, reload = true thì đọc lại từ DB)
        /// </summary>
        protected bool IsRequireChangePassword(string userName, bool reload = false)
        {
            if (string.IsNullOrEmpty(userName)) return false;

            var sessionKey = GetRequireChangePasswordKey(userName);
            if (!reload && Session != null)
            {
                var cached = Session[sessionKey] as bool?;
                var checkedOn = Session[sessionKey + "CheckedOn"] as DateTime?;
                if (cached == true) return true;
                if (cached == false && checkedOn.HasValue &&
                    DateTime.Now - checkedOn.Value < RequireChangePasswordRecheck) return false;
            }

            bool isRequire;
            try
            {
                isRequire = new SysUserCache().IsRequireChangePassword(userName);
            }
            catch (KeyNotFoundException ex)
            {
                // Chưa khai báo procedure (chưa chạy script SQL / chưa restart app pool) thì không chặn người dùng
                AppProcessor.Logger.Error(ex);
                isRequire = false;
            }
            catch (Exception ex)
            {
                // Lỗi DB tạm thời: không chặn ở request này và không lưu Session để request sau đọc lại
                AppProcessor.Logger.Error(ex);
                return false;
            }

            SetRequireChangePasswordSession(userName, isRequire);
            return isRequire;
        }

        /// <summary>
        ///     Đánh dấu tài khoản phải đổi mật khẩu (isRequire = true) hoặc đã tự đổi mật khẩu (isRequire = false)
        /// </summary>
        protected void SetRequireChangePassword(string userName, bool isRequire, string reason)
        {
            if (string.IsNullOrEmpty(userName)) return;

            try
            {
                new SysUserCache().SetRequireChangePassword(userName, isRequire, reason, User?.UserName ?? userName);
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
            }

            SetRequireChangePasswordSession(userName, isRequire);
        }

        protected void SetRequireChangePasswordSession(string userName, bool isRequire)
        {
            if (Session == null || string.IsNullOrEmpty(userName)) return;
            var sessionKey = GetRequireChangePasswordKey(userName);
            Session[sessionKey] = isRequire;
            Session[sessionKey + "CheckedOn"] = DateTime.Now;
        }

        private static string GetRequireChangePasswordKey(string userName)
        {
            return string.Format(SESSION_KEY_REQUIRE_CHANGE_PASSWORD, userName.ToLowerInvariant());
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