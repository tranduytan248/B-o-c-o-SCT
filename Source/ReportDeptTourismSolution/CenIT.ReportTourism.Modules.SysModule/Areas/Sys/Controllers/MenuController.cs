using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CenIT.ReportTourism.Caches.Sys;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Controllers
{
    public class MenuController : AppController
    {
        private readonly SysFunctionActionCache _apiFunctionAction;
        private readonly SysMenuCache _apiMenu;
        private readonly string _funcName = AppProcessor.Messagor.GetMessage("Menu_Title");

        public MenuController()
        {
            _apiFunctionAction = new SysFunctionActionCache();
            _apiMenu = new SysMenuCache();
        }

        // GET: Menu
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Index()
        {
            return View();
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get()
        {
            var search = Request.Form.GetValues("search[value]")?[0];
            var draw = Request.Form.GetValues("draw")?[0];
            var order = Request.Form.GetValues("order[0][column]")?[0];
            var orderDir = Request.Form.GetValues("order[0][dir]")?[0];
            var startRec = Convert.ToInt32(Request.Form.GetValues("start")?[0]);
            var pageSize = Convert.ToInt32(Request.Form.GetValues("length")?[0]);

            var dataSearch = new SysSearchModel
            {
                Search = string.IsNullOrEmpty(search) ? null : search,
                Order = order,
                OrderDir = orderDir,
                StartIndex = startRec,
                PageSize = pageSize
            };
            int total;
            var data = _apiMenu.Get(out total, dataSearch);

            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult Add()
        {
            var model = new SysMenuModel { FunctionActions = CreateListFunctionActions() };
            model.ParentMenus = CreateListParentMenus(model.MenuId);
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult Add(SysMenuModel model)
        {
            if (ModelState.IsValid)
            {
                var idMenu = _apiMenu.Save(new SysMenuModel
                {
                    MenuId = model.MenuId,
                    Name = model.Name,
                    FunctionActionId = model.FunctionActionId,
                    Depth = model.Depth,
                    Icon = model.Icon,
                    Position = model.Position,
                    Link = model.Link,
                    LevelMenu = model.LevelMenu,
                    IsShow = model.IsShow,
                    ParentId = model.ParentId
                });

                var response = CreateMessage($"{_funcName} [{model.Name}]", EnumProcessType.Add,
                    idMenu > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
                return Json(new { status = true, message = response });
            }

            model.FunctionActions = CreateListFunctionActions();
            model.ParentMenus = CreateListParentMenus(model.MenuId);

            return PartialView("_MenuView", model);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id = 0)
        {
            var model = _apiMenu.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            model.FunctionActions = CreateListFunctionActions();
            model.ParentMenus = CreateListParentMenus(model.MenuId);

            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(SysMenuModel model)
        {
            if (ModelState.IsValid)
            {
                var idMenu = _apiMenu.Save(new SysMenuModel
                {
                    MenuId = model.MenuId,
                    Name = model.Name,
                    FunctionActionId = model.FunctionActionId,
                    Depth = model.Depth,
                    Icon = model.Icon,
                    Position = model.Position,
                    Link = model.Link,
                    LevelMenu = model.LevelMenu,
                    IsShow = model.IsShow,
                    ParentId = model.ParentId
                });

                var response = CreateMessage($"{_funcName} [{model.Name}]", EnumProcessType.Edit,
                    idMenu > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
                return Json(new { status = true, message = response });
            }

            model.FunctionActions = CreateListFunctionActions();
            model.ParentMenus = CreateListParentMenus(model.MenuId);

            return PartialView("_MenuView", model);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id = 0)
        {
            var model = _apiMenu.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"),
                $"<b>{_funcName} [{model.Name}]</b>");

            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(SysMenuModel model)
        {
            var deleted = _apiMenu.Delete(model);

            var response = CreateMessage($"{_funcName} [{model.Name}]", EnumProcessType.Delete,
                deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [NonAction]
        private List<ListItem> CreateListFunctionActions()
        {
            return _apiFunctionAction.GetAll()
                .Select(fa => new ListItem
                    { Value = fa.FunctionActionId.ToString(), Text = $"{fa.Area} - {fa.Function} - {fa.Action}" })
                .OrderBy(item => item.Text).Distinct().ToList();
        }

        [NonAction]
        private List<ListItem> CreateListParentMenus(int idMenu)
        {
            return _apiMenu.GetAll().Where(mn => mn.MenuId != idMenu)
                .Select(mn => new ListItem { Value = mn.MenuId.ToString(), Text = mn.Name }).ToList();
        }
    }
}