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
using TSFramework.Core.Helpers;

namespace CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Controllers
{
    public class FunctionController : AppController
    {
        private readonly string _funcName = AppProcessor.Messagor.GetMessage("Function_Title");
        private readonly SysFunctionCache _functionCache;
        private readonly SysModuleCache _moduleCache;

        public FunctionController()
        {
            _functionCache = new SysFunctionCache();
            _moduleCache = new SysModuleCache();
        }

        // GET: Function
        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
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
            var data = _functionCache.Get(out total, dataSearch);

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
            var model = new SysFunctionModel
            {
                Actions = CreateListItemAction(),
                CodeTypes = CreateListCodeType(),
                Modules = _moduleCache.GetAll().Select(m => new ListItem(m.ModuleName, m.ModuleId.ToString())).ToList()
            };
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult Add(SysFunctionModel model)
        {
            model.Actions = CreateListItemAction();
            model.CodeTypes = CreateListCodeType();
            model.Modules = _moduleCache.GetAll().Select(m => new ListItem(m.ModuleName, m.ModuleId.ToString()))
                .ToList();

            if (!ModelState.IsValid) return PartialView("_Function", model);

            var idFunction = _functionCache.Save(new SysFunctionModel
            {
                FunctionId = 0,
                ModuleId = model.ModuleId,
                Area = model.Area,
                Name = model.Name,
                Description = model.Description,
                SelectedActions = model.SelectedActions.Trim(','),
                IsDeleted = false
            });

            var response = CreateMessage($"{_funcName} [{model.Name}]", EnumProcessType.Add,
                idFunction > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id = 0)
        {
            var model = _functionCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            model.Actions = CreateListItemAction();
            model.CodeTypes = CreateListCodeType();
            model.Modules = _moduleCache.GetAll().Select(m => new ListItem(m.ModuleName, m.ModuleId.ToString()))
                .ToList();
            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(SysFunctionModel model)
        {
            if (ModelState.IsValid)
            {
                var idFunction = _functionCache.Save(new SysFunctionModel
                {
                    FunctionId = model.FunctionId,
                    ModuleId = model.ModuleId,
                    Area = model.Area,
                    Name = model.Name,
                    Description = model.Description,
                    SelectedActions = model.SelectedActions.Trim(','),
                    IsDeleted = false
                });

                var response = CreateMessage($"{_funcName} [{model.Name}]", EnumProcessType.Edit,
                    idFunction > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
                return Json(new { status = true, message = response });
            }

            model.Actions = CreateListItemAction();
            model.CodeTypes = CreateListCodeType();
            model.Modules = _moduleCache.GetAll().Select(m => new ListItem(m.ModuleName, m.ModuleId.ToString()))
                .ToList();

            return PartialView("_Function", model);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id = 0)
        {
            var model = _functionCache.GetById(id);
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
        public ActionResult Delete(SysFunctionModel model)
        {
            var deleted = _functionCache.Delete(model);

            var response = CreateMessage($"{_funcName} [{model.Name}]", EnumProcessType.Delete,
                deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [NonAction]
        private List<ListItem> CreateListItemAction()
        {
            return new List<ListItem>
            {
                new ListItem
                {
                    Text = AppProcessor.Messagor.GetMessage(
                        $"ActionType{EnumHelper.GetDescription(EnumActionType.Add)}"),
                    Value = EnumHelper.GetDescription(EnumActionType.Add)
                },
                new ListItem
                {
                    Text = AppProcessor.Messagor.GetMessage(
                        $"ActionType{EnumHelper.GetDescription(EnumActionType.Delete)}"),
                    Value = EnumHelper.GetDescription(EnumActionType.Delete)
                },
                new ListItem
                {
                    Text = AppProcessor.Messagor.GetMessage(
                        $"ActionType{EnumHelper.GetDescription(EnumActionType.Edit)}"),
                    Value = EnumHelper.GetDescription(EnumActionType.Edit)
                },
                new ListItem
                {
                    Text = AppProcessor.Messagor.GetMessage(
                        $"ActionType{EnumHelper.GetDescription(EnumActionType.View)}"),
                    Value = EnumHelper.GetDescription(EnumActionType.View)
                }
            };
        }

        [NonAction]
        private List<ListItem> CreateListCodeType()
        {
            return new List<ListItem>
            {
                new ListItem
                {
                    Text = EnumHelper.GetDescription(EnumAppCode.WebApp),
                    Value = EnumHelper.GetDescription(EnumAppCode.WebApp)
                },
                new ListItem
                {
                    Text = EnumHelper.GetDescription(EnumAppCode.WebApi),
                    Value = EnumHelper.GetDescription(EnumAppCode.WebApi)
                }
            };
        }
    }
}