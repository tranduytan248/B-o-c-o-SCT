using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CenIT.ReportTourism.Caches.Cate;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Models;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Controllers
{
    public class WardsController : AppController
    {
        private readonly CateProvinceCache _provinceCache = new CateProvinceCache();
        private readonly CateWardCache _wardCache = new CateWardCache();

        private readonly string _wardTitle = AppProcessor.Messagor.GetMessage("Ward_Title");

        // GET: Cate/Wards

        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult Index()
        {
            var searchModel = new MyWardsSearchModel
            {
                ListProvinces = _provinceCache.GetAll()
                    .OrderBy(d => d.ProvinceName)
                    .Select(d => new ListItem(d.ProvinceName, d.ProvinceId.ToString())).ToList(),
            };
            return View(searchModel);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get(int? provinceId, int? districtId, MyWardsSearchModel searchModel)
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
            searchModel.ProvinceIds = string.IsNullOrEmpty(searchModel.ProvinceIds) ? null : searchModel.ProvinceIds;

            var data = _wardCache.Get(searchModel.ProvinceIds, out int total, dataSearch);
            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult Add(int? districtId, int? provinceId)
        {
            var provinceModel = _provinceCache.GetById(provinceId.GetValueOrDefault(0));
            var districtModel = _districtCache.GetById(districtId.GetValueOrDefault(0));
            var model = new CateWardModel
            {
                DistrictId = districtId.GetValueOrDefault(0),
                DistrictCode = districtModel?.DistrictCode,
                Districts = _districtCache.GetAll()
                    .Select(d => new ListItem($"{d.ProvinceName} - {d.DistrictName}", d.DistrictId.ToString()))
                    .ToList(),
                ProvinceId = districtModel?.ProvinceId ?? 0,
                ProvinceCode = provinceModel?.ProvinceCode,
                Provinces = _provinceCache.GetAll()
                    .Select(d => new ListItem($"{d.ProvinceName} ", d.ProvinceId.ToString())).ToList()
            };
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Add)]
        [ValidateInput(false)]
        public ActionResult Add(CateWardModel model)
        {
            if (!ModelState.IsValid)
            {
                var provinceModel = _provinceCache.GetAll().FirstOrDefault();
                model.ProvinceId = provinceModel?.ProvinceId;
                model.ProvinceName = provinceModel?.ProvinceName;
                var districtModel = _districtCache.GetAll().FirstOrDefault();
                model.DistrictId = districtModel?.DistrictId;
                model.DistrictName = districtModel?.DistrictName;
                model.Districts = _districtCache.GetAll()
                    .OrderBy(d => d.DistrictName)
                    .Select(d => new ListItem(d.DistrictName, d.DistrictId.ToString())).ToList();
                model.Provinces = _provinceCache.GetAll().OrderBy(d => d.ProvinceName)
                    .Select(d => new ListItem(d.ProvinceName, d.ProvinceId.ToString())).ToList();
                return PartialView("_Wards", model);
            }

            string response;
            var idNatinal = _wardCache.Save(new CateWardModel
            {
                DistrictId = model.DistrictId,
                ProvinceId = model.ProvinceId,
                WardId = model.WardId,
                WardCode = model.WardCode,
                WardName = model.WardName,
                UserCreated = User.UserName,
                DateCreated = DateTime.Now
            });

            if (idNatinal == 0)
                response = CreateMessage($"{_wardTitle} [{model.WardName}]", EnumProcessType.Add,
                    EnumMsgIcon.Success);
            else if (idNatinal == -9)
                response = CreateMessage($"{_wardTitle} [ {model.WardName}]", EnumProcessType.DataExisted,
                    EnumMsgIcon.Error
                );
            else
                response = CreateMessage($"{_wardTitle} [ {model.WardName}]", EnumProcessType.Add, EnumMsgIcon.Success
                );
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id = 0)
        {
            var model = _wardCache.GetById(id);

            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_wardTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            model.Provinces = _provinceCache.GetAll().OrderBy(d => d.ProvinceName)
                .Select(d => new ListItem(d.ProvinceName, d.ProvinceId.ToString())).ToList();
            model.Districts = _districtCache.GetAll()
                .OrderBy(d => d.DistrictName)
                .Select(d => new ListItem(d.DistrictName, d.DistrictId.ToString())).ToList();
            if (model.ProvinceId == null) return PartialView("_Edit", model);
            {
                int totalDistrict;
                model.Districts = _districtCache.GetByDistricts(model.ProvinceId, out totalDistrict)
                    .OrderBy(w => w.DistrictName)
                    .Select(d => new ListItem(d.DistrictName, d.DistrictId.ToString())).ToList();
            }

            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult Edit(CateWardModel model)
        {
            if (!ModelState.IsValid)
            {
                var provinceModel = _provinceCache.GetAll().FirstOrDefault();
                model.ProvinceId = provinceModel?.ProvinceId;
                model.ProvinceName = provinceModel?.ProvinceName;
                model.Provinces = _provinceCache.GetAll().OrderBy(d => d.ProvinceName)
                    .Select(d => new ListItem(d.ProvinceName, d.ProvinceId.ToString())).ToList();
                var districtModel = _districtCache.GetAll().FirstOrDefault();
                model.DistrictId = districtModel?.DistrictId;
                model.DistrictName = districtModel?.DistrictName;
                model.Districts = _districtCache.GetAll()
                    .OrderBy(d => d.DistrictName)
                    .Select(d => new ListItem(d.DistrictName, d.DistrictId.ToString())).ToList();
                model.Provinces = _provinceCache.GetAll().OrderBy(d => d.ProvinceName)
                    .Select(d => new ListItem(d.ProvinceName, d.ProvinceId.ToString())).ToList();
                return PartialView("_Wards", model);
            }

            string response;
            var nationalId = _wardCache.Save(new CateWardModel
            {
                DistrictId = model.DistrictId,
                ProvinceId = model.ProvinceId,
                WardId = model.WardId,
                WardCode = model.WardCode,
                WardName = model.WardName,
                UserCreated = User.UserName,
                DateCreated = DateTime.Now
            });

            if (nationalId == 0)
                response = CreateMessage($"{_wardTitle} [{model.WardName}]", EnumProcessType.Edit,
                    EnumMsgIcon.Success);
            else if (nationalId == -9)
                response = CreateMessage($"{_wardTitle} [ {model.WardName}]", EnumProcessType.DataExisted,
                    EnumMsgIcon.Error
                );
            else
                response = CreateMessage($"{_wardTitle} [ {model.WardName}]", EnumProcessType.Edit, EnumMsgIcon.Success
                );
            return Json(new
            {
                status = true, message = response
            }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id = 0)
        {
            var model = _wardCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_wardTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"),
                $"<b>{_wardTitle} [{model.WardName}]</b>");
            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(CateWardModel model)
        {
            model.UserCreated = User.UserName;
            var deleted = _wardCache.Delete(model);
            var response = CreateMessage($"{model.WardName}", EnumProcessType.Delete,
                deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult WardViaDistrict(int provinceId)
        {
            int totalDistrict;
            var lstWardViaDistricts = _districtCache.GetByDistricts(provinceId, out totalDistrict)
                .OrderBy(d => d.ProvinceName).ToList();
            return Json(new { Wards = lstWardViaDistricts });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult WardViaListDistrict(string provinceId)
        {
            if (string.IsNullOrWhiteSpace(provinceId)) return Json(null);
            var provinceIds = provinceId.Split('|');
            if (provinceId.Length <= 0) return Json(null);
            var lstWardViaDistricts = _districtCache.GetAll()
                .Where(x => provinceIds.Contains(x.ProvinceId.ToString())).ToList();
            return Json(new { Wards = lstWardViaDistricts });
        }

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult WardByDistrict(int id)
        {
            var districtModel = _districtCache.GetById(id);
            if (districtModel != null)
                return PartialView("_WardByDistrict",
                    new CateDistrictModel
                        { DistrictId = districtModel.DistrictId, DistrictName = districtModel.DistrictName });
            var districtName = AppProcessor.Messagor.GetMessage("District_Title");
            return Json(new
            {
                status = true,
                message = CreateMessage($"{districtName}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
            }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpPost]
        public ActionResult GetWardByDistricts(int? provinceId, int? districtId, MyWardsSearchModel searchModel)
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
            var data = _wardCache.Get(provinceId, districtId, searchModel.ProvinceIds, searchModel.DistrictIds,
                out total, dataSearch);
            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
            return result;
        }
    }
}