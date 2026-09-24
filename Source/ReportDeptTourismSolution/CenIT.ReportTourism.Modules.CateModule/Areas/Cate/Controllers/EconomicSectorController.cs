using System;
using System.Web.Mvc;
using CenIT.ReportTourism.Caches.Cate;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Controllers
{
    public class EconomicSectorController : AppController
    {
        private readonly CateEconomicSectorCache _cache = new CateEconomicSectorCache();
        private readonly string _title = AppProcessor.Messagor.GetMessage("EconomicSector_Title");

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
            var data = _cache.Get(out total, dataSearch);
            return Json(new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult Add()
        {
            return PartialView("_Add");
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Add)]
        [ValidateInput(false)]
        public ActionResult Add(CateEconomicSectorModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_EconomicSector", model);

            var id = _cache.Save(new CateEconomicSectorModel
            {
                EconomicSectorId = model.EconomicSectorId,
                Code = model.Code,
                Name = model.Name,
                DisplayOrder = model.DisplayOrder,
                IsActive = model.IsActive,
                UpdatedBy = User.UserName
            });
            if (id == -2)
            {
                var errMessage = CreateMessage($"{_title} [{model.Code} - {model.Name}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage($"{_title} ", EnumProcessType.Add, id > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id = 0)
        {
            var model = _cache.GetById(id);
            if (model == null)
                return Json(new { status = true, message = CreateMessage($"{_title}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) });
            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(CateEconomicSectorModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_EconomicSector", model);

            var id = _cache.Save(new CateEconomicSectorModel
            {
                EconomicSectorId = model.EconomicSectorId,
                Code = model.Code,
                Name = model.Name,
                DisplayOrder = model.DisplayOrder,
                IsActive = model.IsActive,
                UpdatedBy = User.UserName
            });

            if (id == -2)
            {
                var errMessage = CreateMessage($"{_title} [{model.Code} - {model.Name}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage($"{_title} [{model.Name}]", EnumProcessType.Edit, id > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id = 0)
        {
            var model = _cache.GetById(id);
            if (model == null)
                return Json(new { status = true, message = CreateMessage($"{_title}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"), $"<b>{_title} [{model.Name}]</b>");
            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(CateEconomicSectorModel model)
        {
            var delete = _cache.GetById(model.EconomicSectorId);
            delete.LastModifiedBy = User.UserName;
            var deleted = _cache.Delete(delete);

            var response = CreateMessage($"{model.Name}", EnumProcessType.Delete, deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }
    }
}
