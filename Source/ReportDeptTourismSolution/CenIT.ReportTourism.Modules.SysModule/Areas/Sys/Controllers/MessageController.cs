using System;
using System.Web.Mvc;
using CenIT.ReportTourism.Caches.Sys;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Attributes;
using TSFramework.App.BaseApps;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Controllers
{
    public class MessageController : AppController
    {
        private readonly string _funcName = AppProcessor.Messagor.GetMessage("Message_Title");
        private readonly SysMessageCache _messageCache;

        public MessageController()
        {
            _messageCache = new SysMessageCache();
        }

        // GET: Message
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
            var data = _messageCache.Get(out total, dataSearch);

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
            var model = new SysMessageModel();
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult Add(SysMessageModel model)
        {
            if (!ModelState.IsValid) return PartialView("_Message", model);
            string response;

            var idMessage = _messageCache.Save(new SysMessageModel
            {
                LangCode = model.LangCode,
                LabelKey = model.LabelKey,
                Message = model.Message
            });

            if (idMessage > 0)
            {
                AppProcessor.Messagor.Refresh(BaseAppContext.Current.CurrentLanguageCode);

                response = CreateMessage(
                    $"{_funcName} [{model.LangCode} - {model.LabelKey}]",
                    EnumProcessType.Add, EnumMsgIcon.Success);
            }
            else
            {
                response = CreateMessage(
                    $"{_funcName} [{model.LangCode} - {model.LabelKey}]",
                    EnumProcessType.Add, EnumMsgIcon.Error);
            }

            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(string id = "")
        {
            var model = _messageCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(SysMessageModel model)
        {
            if (!ModelState.IsValid) return PartialView("_Message", model);
            string response;
            var idMessage = _messageCache.Save(new SysMessageModel
            {
                LangCode = model.LangCode,
                LabelKey = model.LabelKey,
                Message = model.Message
            });

            if (idMessage > 0)
            {
                AppProcessor.Messagor.Refresh(BaseAppContext.Current.CurrentLanguageCode);
                response = CreateMessage(
                    $"{_funcName} [{model.LangCode} - {model.LabelKey}]",
                    EnumProcessType.Edit, EnumMsgIcon.Success);
            }
            else
            {
                response = CreateMessage(
                    $"{_funcName} [{model.LangCode} - {model.LabelKey}]",
                    EnumProcessType.Edit, EnumMsgIcon.Error);
            }

            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(string id = "")
        {
            var model = _messageCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"),
                $"<b>{_funcName} [{model.LangCode} - {model.LabelKey}]</b>");

            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(SysMessageModel model)
        {
            var deleted = _messageCache.Delete(model);
            string response;

            if (deleted)
            {
                AppProcessor.Messagor.Refresh(BaseAppContext.Current.CurrentLanguageCode);
                response = CreateMessage(
                    $"{_funcName} [{model.LangCode} - {model.LabelKey}]",
                    EnumProcessType.Delete, EnumMsgIcon.Success);
            }
            else
            {
                response = CreateMessage(
                    $"{_funcName} [{model.LangCode} - {model.LabelKey}]",
                    EnumProcessType.Delete, EnumMsgIcon.Error);
            }

            return Json(new { status = true, message = response });
        }
    }
}