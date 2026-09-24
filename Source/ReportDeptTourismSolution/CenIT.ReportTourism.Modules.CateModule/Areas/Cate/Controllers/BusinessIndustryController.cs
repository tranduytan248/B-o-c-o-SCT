using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CenIT.ReportTourism.Caches.Cate;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Controllers
{
    public class BusinessIndustryController : AppController
    {
        private readonly CateBusinessIndustryCache _cache = new CateBusinessIndustryCache();
        private readonly string _title = AppProcessor.Messagor.GetMessage("BusinessIndustry_Title");

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

        #region Add

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
        public ActionResult Add(CateBusinessIndustryModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_BusinessIndustry", model);

            var id = _cache.Save(new CateBusinessIndustryModel
            {
                IndustryId = model.IndustryId,
                IndustryCode = model.IndustryCode,
                IndustryName = model.IndustryName,
                ParentId = model.ParentId,
                DisplayOrder = model.DisplayOrder,
                IsActive = true,
                UpdatedBy = User.UserName
            });
            if (id == -2)
            {
                var errMessage = CreateMessage($"{_title} [{model.IndustryCode} - {model.IndustryName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage($"{_title} ", EnumProcessType.Add, id > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }
        
        #endregion

        #region Edit

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
        public ActionResult Edit(CateBusinessIndustryModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_BusinessIndustry", model);

            var id = _cache.Save(new CateBusinessIndustryModel
            {
                IndustryId = model.IndustryId,
                IndustryCode = model.IndustryCode,
                IndustryName = model.IndustryName,
                ParentId = model.ParentId,
                DisplayOrder = model.DisplayOrder,
                IsActive = true,
                UpdatedBy = User.UserName
            });

            if (id == -2)
            {
                var errMessage = CreateMessage($"{_title} [{model.IndustryCode} - {model.IndustryName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage($"{_title} [{model.IndustryName}]", EnumProcessType.Edit, id > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Delete

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id = 0)
        {
            var model = _cache.GetById(id);
            if (model == null)
                return Json(new { status = true, message = CreateMessage($"{_title}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"), $"<b>{_title} [{model.IndustryName}]</b>");
            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(CateBusinessIndustryModel model)
        {
            var delete = _cache.GetById(model.IndustryId);
            delete.LastModifiedBy = User.UserName;
            var deleted = _cache.Delete(delete);

            var response = CreateMessage($"{model.IndustryName}", EnumProcessType.Delete, deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        #endregion

        #region Products

        private readonly CateBusinessProductCache _productCache = new CateBusinessProductCache();
        private readonly string _productTitle = AppProcessor.Messagor.GetMessage("BusinessProduct_Title");

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Products(int id = 0)
        {
            var industry = _cache.GetById(id);

            if (industry == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_title}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            return PartialView("Product/_Products", industry);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetProducts(int industryId)
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
            var data = _productCache.Get(out total, industryId, dataSearch);
            return Json(new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult AddProduct(int id)
        {
            var industry = _cache.GetById(id);
            if (industry == null)
                return Json(new { status = true, message = CreateMessage($"{_title}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) });

            var product = new CateBusinessProductModel
            {
                IndustryId = industry.IndustryId,
                IndustryName = industry.IndustryName,
                ListIndustries = _cache.GetAll()?.Select(i => new ListItem{Text = i.IndustryName, Value = $"{i.IndustryId}"}).ToList(),
                IsActive = true,
                DisplayOrder = 1
            };
            return PartialView("Product/_Add", product);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Add)]
        [ValidateInput(false)]
        public ActionResult AddProduct(CateBusinessProductModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListIndustries = _cache.GetAll()?.Select(i => new ListItem { Text = i.IndustryName, Value = $"{i.IndustryId}" }).ToList();
                return PartialView("Product/_Product", model);
            }

            var id = _productCache.Save(new CateBusinessProductModel
            {
                IndustryId = model.IndustryId,
                ProductCode = model.ProductCode,
                ProductName = model.ProductName,
                DisplayOrder = model.DisplayOrder,
                IsActive = model.IsActive,
                UpdatedBy = User.UserName
            });
            if (id == -2)
            {
                var errMessage = CreateMessage($"{_productTitle} [{model.ProductCode} - {model.ProductName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage($"{_productTitle} ", EnumProcessType.Add, id > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditProduct(int id = 0)
        {
            var model = _productCache.GetById(id);
            if (model == null)
                return Json(new { status = true, message = CreateMessage($"{_productTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) });

            model.ListIndustries = _cache.GetAll()?.Select(i => new ListItem { Text = i.IndustryName, Value = $"{i.IndustryId}" }).ToList();

            return PartialView("Product/_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult EditProduct(CateBusinessProductModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListIndustries = _cache.GetAll()?.Select(i => new ListItem { Text = i.IndustryName, Value = $"{i.IndustryId}" }).ToList();
                return PartialView("Product/_Product", model);
            }

            var id = _productCache.Save(new CateBusinessProductModel
            {
                ProductId = model.ProductId,
                IndustryId = model.IndustryId,
                ProductCode = model.ProductCode,
                ProductName = model.ProductName,
                DisplayOrder = model.DisplayOrder,
                IsActive = model.IsActive,
                UpdatedBy = User.UserName
            });

            if (id == -2)
            {
                var errMessage = CreateMessage($"{_productTitle} [{model.ProductCode} - {model.ProductName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage($"{_productTitle} [{model.ProductCode} - {model.ProductName}]", EnumProcessType.Edit, id > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteProduct(int id = 0)
        {
            var model = _productCache.GetById(id);
            if (model == null)
                return Json(new { status = true, message = CreateMessage($"{_productTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error) });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"), $"<b>{_productTitle} [{model.ProductCode} - {model.ProductName}]</b>");
            return PartialView("Product/_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteProduct(CateBusinessProductModel model)
        {
            model.UpdatedBy = User.UserName;
            var isSuccess = _productCache.Delete(model);

            var response = CreateMessage($"<b>{_productTitle} [{model.ProductCode} - {model.ProductName}]</b>", EnumProcessType.Delete, isSuccess ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        #endregion
    }
}
