using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CenIT.ReportTourism.Caches.Cate;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Core.Conts;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Models;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;
using TSFramework.Core.Helpers;

namespace CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Controllers
{
    public class NationalController : AppController
    {
        private readonly CateNationalCache _nationalCache = new CateNationalCache();
        private readonly string _nationalTile = AppProcessor.Messagor.GetMessage("National_Title");

        // GET: Cate/National
        public ActionResult Index()
        {
            var searchModel = new NationSearchModel
            {
                ListContinent = Enum.GetValues(typeof(EnumContinents))
                    .Cast<EnumContinents>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList()
            };
            return View(searchModel);
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
            var data = _nationalCache.Get(out total, dataSearch);
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
            var model = new CateNationalModel
            {
                ListContinent = Enum.GetValues(typeof(EnumContinents))
                    .Cast<EnumContinents>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList()
            };
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Add)]
        [ValidateInput(false)]
        public ActionResult Add(CateNationalModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListContinent = Enum.GetValues(typeof(EnumContinents))
                    .Cast<EnumContinents>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList();
                return PartialView("_National", model);
            }

            var idNatinal = _nationalCache.Save(new CateNationalModel
            {
                NationalId = model.NationalId,
                NationalCode = model.NationalCode,
                NationalName = model.NationalName,
                ContinentId = model.ContinentId,
                ContinentName = model.ContinentName,
                CreatedBy = User.UserName,
                CreatedDate = DateTime.Now
            });
            if (idNatinal == -2)
            {
                var errMessage = CreateMessage($"{_nationalTile} [{model.NationalCode} - {model.NationalName}]",
                    EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage($"{_nationalTile} ", EnumProcessType.Add,
                idNatinal > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id = 0)
        {
            var model = _nationalCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_nationalTile}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            model.ListContinent = Enum.GetValues(typeof(EnumContinents))
                .Cast<EnumContinents>()
                .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                    ((int)x).ToString()))
                .ToList();
            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(CateNationalModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListContinent = Enum.GetValues(typeof(EnumContinents))
                    .Cast<EnumContinents>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList();
                return PartialView("_National", model);
            }

            var nationalId = _nationalCache.Save(new CateNationalModel
            {
                NationalId = model.NationalId,
                NationalCode = model.NationalCode,
                NationalName = model.NationalName,
                ContinentId = model.ContinentId,
                ContinentName = model.ContinentName,
                LastModifiedBy = User.UserName,
                LastModifiedDate = DateTime.Now
            });

            if (nationalId == -2)
            {
                var errMessage = CreateMessage($"{_nationalTile} [{model.NationalCode} - {model.NationalName}]",
                    EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage($"{_nationalTile} [{model.NationalName} - {model.ContinentName}]",
                EnumProcessType.Edit,
                nationalId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id = 0)
        {
            var model = _nationalCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_nationalTile}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"),
                $"<b>{_nationalTile} [{model.NationalName} - {model.ContinentName}]</b>");
            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(CateNationalModel model)
        {
            var delete = _nationalCache.GetById(model.NationalId);
            delete.LastModifiedBy = User.UserName;
            var deleted = _nationalCache.Delete(delete);

            var response = CreateMessage($"{model.NationalName}", EnumProcessType.Delete,
                deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }
    }
}