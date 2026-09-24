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
    public class PublicCateController : AppController
    {
        private readonly CatePublicCateCache _publicCateCl = new CatePublicCateCache();
        private readonly string _publicCateTitle = AppProcessor.Messagor.GetMessage("PublicCate_Title");

        // GET: Modules
        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult Index()
        {
            var searchModel = new PublicCateSearchModel
            {
                ListCateTypes = Enum.GetValues(typeof(EnumTypePublicCate))
                    .Cast<EnumTypePublicCate>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList()
            };
            return View(searchModel);
        }

        #region Public Child Cate

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetChildrens(int? cateId = 0)
        {
            var draw = Request.Form.GetValues("draw")?[0];
            var lstUnionChilds = _publicCateCl.GetAll(cateId);
            var total = lstUnionChilds.Count;
            var result = Json(
                new
                {
                    draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data = lstUnionChilds
                },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        #endregion

        #region Public Categories

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get(string cateTypeIds = null)
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
            cateTypeIds = string.IsNullOrEmpty(cateTypeIds) ? null : cateTypeIds;
            var data = _publicCateCl.Get(cateTypeIds, out total, dataSearch);
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
            var categoryModel = new CatePublicCateModel
            {
                ListCateTypes = Enum.GetValues(typeof(EnumTypePublicCate))
                    .Cast<EnumTypePublicCate>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList()
            };
            return PartialView("_Add", categoryModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult Add(CatePublicCateModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListCateTypes = Enum.GetValues(typeof(EnumTypePublicCate))
                    .Cast<EnumTypePublicCate>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList();
                return PartialView("_PublicCate", model);
            }

            string response;
            var unionId = _publicCateCl.Save(new CatePublicCateModel
            {
                CateId = 0,
                CateName = model.CateName,
                CateTypeId = model.CateTypeId,
                CateTypeName = model.CateTypeName,
                SavedBy = User.Email
            });

            if (unionId == 0)
                response = CreateMessage($"{_publicCateTitle} [{model.CateName}]", EnumProcessType.Add,
                    EnumMsgIcon.Success);
            else if (unionId == -2)
                response = CreateMessage($"{_publicCateTitle} [ {model.CateName}]", EnumProcessType.DataExisted,
                    EnumMsgIcon.Error
                );
            else
                response = CreateMessage($"{_publicCateTitle} [ {model.CateName}]", EnumProcessType.Add,
                    EnumMsgIcon.Success
                );
            return Json(new
            {
                status = true,
                message = response
            }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id = 0)
        {
            var model = _publicCateCl.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_publicCateTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            model.ListCateTypes = Enum.GetValues(typeof(EnumTypePublicCate))
                .Cast<EnumTypePublicCate>()
                .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                    ((int)x).ToString()))
                .ToList();
            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(CatePublicCateModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListCateTypes = Enum.GetValues(typeof(EnumTypePublicCate))
                    .Cast<EnumTypePublicCate>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList();
                return PartialView("_PublicCate", model);
            }

            string response;
            var unionId = _publicCateCl.Save(new CatePublicCateModel
            {
                CateId = model.CateId,
                CateName = model.CateName,
                CateTypeId = model.CateTypeId,
                CateTypeName = model.CateTypeName,
                SavedBy = User.Email
            });
            if (unionId == 0)
                response = CreateMessage($"{_publicCateTitle} [{model.CateName}]", EnumProcessType.Add,
                    EnumMsgIcon.Success);
            else if (unionId == -2)
                response = CreateMessage($"{_publicCateTitle} [ {model.CateName}]", EnumProcessType.DataExisted,
                    EnumMsgIcon.Error
                );
            else
                response = CreateMessage($"{_publicCateTitle} [ {model.CateName}]", EnumProcessType.Add,
                    EnumMsgIcon.Success
                );
            return Json(new
            {
                status = true,
                message = response
            }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id = 0)
        {
            var model = _publicCateCl.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_publicCateTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"),
                $"<b>{_publicCateTitle} [{model.CateName} - {model.CateTypeName}]</b>");
            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(CatePublicCateModel model)
        {
            model.SavedBy = User.Email;
            var deleted = _publicCateCl.Delete(model);

            var response = CreateMessage($"{_publicCateTitle} [{model.CateName} - {model.CateTypeName}]",
                EnumProcessType.Delete, deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        #endregion
    }
}