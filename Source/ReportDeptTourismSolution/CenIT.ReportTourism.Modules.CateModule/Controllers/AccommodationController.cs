using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CenIT.ReportTourism.Caches.Cate;
using CenIT.ReportTourism.Caches.Cate.Accommodation;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Core.Conts;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Cate.Accommodation;
using CenIT.ReportTourism.Models.Sys;
using FastMember;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Modules.CateModule.Controllers
{
    public class AccommodationController : AppController
    {
        private readonly string _accommodationCertificateFolder = "AccommodationCertificates";
        private readonly string _accommodationInfoFolder = "AccommodationInfos";
        private readonly CateAccommodationServiceCache _accommodationServiceCache;
        private readonly CateAccommodationServiceRoomCache _accommodationServiceRoomCache;
        private readonly CateAccommodationServiceTypeServiceCache _accommodationServiceTypeServiceCache;

        private readonly string _enterpriseAccommodationCertificateTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_Accommodation_Certificate_Title");

        private readonly string _enterpriseAccommodationHRInformationTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_Accommodation_HRInformation_Title");

        private readonly string _enterpriseAccommodationInfrastructureRoomTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_Accommodation_Infrastructure_Room_Title");

        private readonly string _enterpriseAccommodationInfrastructureTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_Accommodation_Infrastructure_Title");

        private readonly string _enterpriseAccommodationTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_Accommodation_Title");

        private readonly string _enterpriseAccommodationTypeServiceTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_Accommodation_TypeService_Title");

        private readonly CateEnterpriseCache _enterpriseCache;

        private readonly string _enterpriseTitle = AppProcessor.Messagor.GetMessage("Enterprise_Title");

        private readonly string _moduleRefDocsPathFolder =
            ConfigurationManager.AppSettings["AttachmentFolderPath"] ?? @"/Contents/Modules/Cate/Attachments/";

        private readonly CatePublicCateCache _publicCateCache;

        public AccommodationController()
        {
            _publicCateCache = new CatePublicCateCache();
            _enterpriseCache = new CateEnterpriseCache();
            _accommodationServiceCache = new CateAccommodationServiceCache();
            _accommodationServiceRoomCache = new CateAccommodationServiceRoomCache();
            _accommodationServiceTypeServiceCache = new CateAccommodationServiceTypeServiceCache();
        }

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult Info(int enterpriseId)
        {
            var enterpriseModel = _enterpriseCache.GetById(enterpriseId);
            if (enterpriseModel == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            var accommodationModel = new CateAccommodationServiceModel
            {
                EnterpriseId = enterpriseId
            };
            return PartialView("_Accommodation", accommodationModel);
        }

        #region Certificate

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult Certificate(int enterpriseId)
        {
            var certificateModel = _accommodationServiceCache.GetCertificateViaEnterprise(enterpriseId);
            certificateModel = certificateModel ?? new CateAccommodationServiceCertificateModel();
            return PartialView("_TabCertificate", certificateModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult SaveCertificate(CateAccommodationServiceCertificateModel model)
        {
            if (!ModelState.IsValid)
            {
                var certificateModel = _accommodationServiceCache.GetCertificateViaEnterprise(model.EnterpriseId);
                return PartialView("_TabCertificate", certificateModel);
            }

            var enterpriseId = SaveUploadFile(model);

            if (enterpriseId == -7)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            var response = CreateMessage(
                $"{_enterpriseAccommodationTitle} - {_enterpriseAccommodationCertificateTitle}", EnumProcessType.Edit,
                enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Infrastructure

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult Infrastructure(int enterpriseId)
        {
            var cul = CultureInfo.GetCultureInfo("vi-VN");
            var infrastructureModel = _accommodationServiceCache.GetInfrastructureViaEnterprise(enterpriseId);
            infrastructureModel = infrastructureModel ?? new CateAccommodationServiceInfrastructureModel();
            infrastructureModel.EnterpriseId = enterpriseId;
            infrastructureModel.InitialInvestmentCapitalView = infrastructureModel.InitialInvestmentCapital != null
                ? infrastructureModel.InitialInvestmentCapital.Value.ToString("#,### VND", cul.NumberFormat)
                : string.Empty;
            infrastructureModel.UpgradeInvestmentCapitalView = infrastructureModel.UpgradeInvestmentCapital != null
                ? infrastructureModel.UpgradeInvestmentCapital.Value.ToString("#,### VND", cul.NumberFormat)
                : string.Empty;
            return PartialView("_TabInfrastructure", infrastructureModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult SaveInfrastructure(CateAccommodationServiceInfrastructureModel model)
        {
            if (!ModelState.IsValid)
            {
                var cul = CultureInfo.GetCultureInfo("vi-VN");
                var infrastructureModel = _accommodationServiceCache.GetInfrastructureViaEnterprise(model.EnterpriseId);
                infrastructureModel = infrastructureModel ?? new CateAccommodationServiceInfrastructureModel();
                infrastructureModel.EnterpriseId = model.EnterpriseId;
                infrastructureModel.InitialInvestmentCapitalView = infrastructureModel.InitialInvestmentCapital != null
                    ? infrastructureModel.InitialInvestmentCapital.Value.ToString("#,### VND", cul.NumberFormat)
                    : string.Empty;
                infrastructureModel.UpgradeInvestmentCapitalView = infrastructureModel.UpgradeInvestmentCapital != null
                    ? infrastructureModel.UpgradeInvestmentCapital.Value.ToString("#,### VND", cul.NumberFormat)
                    : string.Empty;

                return PartialView("_TabInfrastructure", infrastructureModel);
            }

            var enterpriseId =
                _accommodationServiceCache.SaveInfrastructures(new CateAccommodationServiceInfrastructureModel
                {
                    EnterpriseId = model.EnterpriseId,
                    InitialInvestmentCapital = model.InitialInvestmentCapital,
                    UpgradeInvestmentCapital = model.UpgradeInvestmentCapital,
                    TotalArea = model.TotalArea,
                    TotalConstructionArea = model.TotalConstructionArea,
                    TotalRoom = model.TotalRoom,
                    Reason = model.Reason,
                    SavedBy = User.Email
                });

            if (enterpriseId == -7)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            var response = CreateMessage(
                $"{_enterpriseAccommodationTitle} - {_enterpriseAccommodationInfrastructureTitle}",
                EnumProcessType.Edit,
                enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        #region Room Type

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetRoomTypes(int? enterpriseId = null)
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
            var data = _accommodationServiceRoomCache.Get(enterpriseId, out total, dataSearch);
            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult AddRoomType(int enterpriseId)
        {
            var infrastructureRoomModel = new CateAccommodationServiceInfrastructureRoomModel
            {
                EnterpriseId = enterpriseId,
                ListRoomTypes = _publicCateCache.GetAll((int)EnumTypePublicCate.TypeRoomAccommodation)
                    .Select(c => new ListItem(c.CateName, c.CateId.ToString())).ToList()
            };
            return PartialView("_AddRoomType", infrastructureRoomModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult AddRoomType(CateAccommodationServiceInfrastructureRoomModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListRoomTypes = _publicCateCache.GetAll((int)EnumTypePublicCate.TypeRoomAccommodation)
                    .Select(c => new ListItem(c.CateName, c.CateId.ToString())).ToList();
                return PartialView("_RoomType", model);
            }

            var infrastructureRoomId = _accommodationServiceRoomCache.Save(
                new CateAccommodationServiceInfrastructureRoomModel
                {
                    EnterpriseId = model.EnterpriseId,
                    RoomType = model.RoomType,
                    TotalRooms = model.TotalRooms,
                    AnnouncedPrice = model.AnnouncedPrice,
                    Reason = model.Reason,
                    SavedBy = User.Email
                });

            if (infrastructureRoomId == -9)
            {
                var errMessage =
                    CreateMessage($"{_enterpriseAccommodationInfrastructureRoomTitle} [{model.RoomTypeName}]",
                        EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            if (infrastructureRoomId == -7)
            {
                var errMessage = CreateMessage($"{_enterpriseTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage($"{_enterpriseAccommodationInfrastructureRoomTitle} [{model.RoomTypeName}]",
                EnumProcessType.Add,
                infrastructureRoomId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditRoomType(int id = 0)
        {
            var model = _accommodationServiceRoomCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseAccommodationInfrastructureRoomTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            model.ListRoomTypes = _publicCateCache.GetAll((int)EnumTypePublicCate.TypeRoomAccommodation)
                .Select(c => new ListItem(c.CateName, c.CateId.ToString())).ToList();
            var cul = CultureInfo.GetCultureInfo("vi-VN");
            model.AnnouncedPriceView = model.AnnouncedPrice != 0
                ? model.AnnouncedPrice.ToString("#,### VND", cul.NumberFormat)
                : string.Empty;
            return PartialView("_EditRoomType", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditRoomType(CateAccommodationServiceInfrastructureRoomModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListRoomTypes = _publicCateCache.GetAll((int)EnumTypePublicCate.TypeRoomAccommodation)
                    .Select(c => new ListItem(c.CateName, c.CateId.ToString())).ToList();
                return PartialView("_RoomType", model);
            }

            var infrastructureRoomId = _accommodationServiceRoomCache.Save(
                new CateAccommodationServiceInfrastructureRoomModel
                {
                    InfrastructureRoomId = model.InfrastructureRoomId,
                    EnterpriseId = model.EnterpriseId,
                    RoomType = model.RoomType,
                    TotalRooms = model.TotalRooms,
                    AnnouncedPrice = model.AnnouncedPrice,
                    Reason = model.Reason,
                    SavedBy = User.Email
                });

            if (infrastructureRoomId == -9)
            {
                var errMessage =
                    CreateMessage($"{_enterpriseAccommodationInfrastructureRoomTitle} [{model.RoomTypeName}]",
                        EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            if (infrastructureRoomId == -7)
            {
                var errMessage = CreateMessage($"{_enterpriseTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage($"{_enterpriseAccommodationInfrastructureRoomTitle} [{model.RoomTypeName}]",
                EnumProcessType.Edit,
                infrastructureRoomId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteRoomType(int id = 0)
        {
            var model = _accommodationServiceRoomCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseAccommodationInfrastructureRoomTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"),
                $"<b>{_enterpriseAccommodationInfrastructureRoomTitle} [{model.RoomTypeName}]</b>");
            return PartialView("_DeleteRoomType", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteRoomType(CateAccommodationServiceInfrastructureRoomModel model)
        {
            model.SavedBy = User.Email;
            var deleted = _accommodationServiceRoomCache.Delete(model);

            var response = CreateMessage($"{_enterpriseAccommodationInfrastructureRoomTitle} [{model.RoomTypeName}]",
                EnumProcessType.Delete, deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        #endregion

        #endregion

        #region HRInformation

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult HRInformation(int enterpriseId)
        {
            var hrInformationModel = _accommodationServiceCache.GetHRInformationViaEnterprise(enterpriseId);
            hrInformationModel = hrInformationModel ?? new CateAccommodationServiceHRInformationModel();
            hrInformationModel.EnterpriseId = enterpriseId;
            return PartialView("_TabHRInformation", hrInformationModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult SaveHRInformation(CateAccommodationServiceHRInformationModel model)
        {
            if (!ModelState.IsValid)
            {
                var hrInformationModel = _accommodationServiceCache.GetHRInformationViaEnterprise(model.EnterpriseId);
                hrInformationModel = hrInformationModel ?? new CateAccommodationServiceHRInformationModel();
                hrInformationModel.EnterpriseId = model.EnterpriseId;
                return PartialView("_TabHRInformation", model);
            }

            var enterpriseId = _accommodationServiceCache.SaveHRInformations(
                new CateAccommodationServiceHRInformationModel
                {
                    EnterpriseId = model.EnterpriseId,
                    StaffOnLeader = model.StaffOnLeader,
                    StaffOnManager = model.StaffOnManager,

                    TotalStaff = model.TotalStaff,

                    StaffOnReceptionist = model.StaffOnReceptionist,
                    StaffOnRestaurant = model.StaffOnRestaurant,
                    StaffOnBar = model.StaffOnBar,
                    StaffOnKitchen = model.StaffOnKitchen,
                    StaffOnRoom = model.StaffOnRoom,
                    StaffOnLiteracyOnCollege = model.StaffOnLiteracyOnCollege,

                    StaffOnUniversity = model.StaffOnUniversity,
                    StaffOnCollege = model.StaffOnCollege,
                    StaffOnIntermediate = model.StaffOnIntermediate,
                    StaffOnHighSchool = model.StaffOnHighSchool,

                    HaveForeignLanguageCertificate = model.HaveForeignLanguageCertificate,
                    HaveProfessionalCertificate = model.HaveProfessionalCertificate,
                    Reason = model.Reason,
                    SavedBy = User.Email
                });

            if (enterpriseId == -7)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            var response = CreateMessage(
                $"{_enterpriseAccommodationTitle} - {_enterpriseAccommodationHRInformationTitle}", EnumProcessType.Edit,
                enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Type Service

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult TypeService(int enterpriseId)
        {
            return PartialView("_TabTypeService", enterpriseId);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetTypeServices(int? enterpriseId = null)
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
            var data = _accommodationServiceTypeServiceCache.Get(enterpriseId, out total, dataSearch);
            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult AddTypeService(int enterpriseId)
        {
            var infrastructureRoomModel = new CateAccommodationServiceTypeServiceModel
            {
                EnterpriseId = enterpriseId,
                ListTypeServices = _publicCateCache.GetAll((int)EnumTypePublicCate.TypeServiceAccommodation)
                    .Select(c => new ListItem(c.CateName, c.CateId.ToString())).ToList(),
                Reason = "Thêm mới"
            };
            return PartialView("_AddTypeService", infrastructureRoomModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult AddTypeService(CateAccommodationServiceTypeServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListTypeServices = _publicCateCache.GetAll((int)EnumTypePublicCate.TypeServiceAccommodation)
                    .Select(c => new ListItem(c.CateName, c.CateId.ToString())).ToList();
                return PartialView("_TypeService", model);
            }

            var infrastructureRoomId = _accommodationServiceTypeServiceCache.Save(
                new CateAccommodationServiceTypeServiceModel
                {
                    AccommodationTypeServiceId = 0,
                    EnterpriseId = model.EnterpriseId,
                    ServiceId = model.ServiceId,
                    ServiceName = model.ServiceName,
                    Reason = model.Reason,
                    SavedBy = User.Email
                });

            if (infrastructureRoomId == -9)
            {
                var errMessage = CreateMessage($"{_enterpriseAccommodationTypeServiceTitle} [{model.ServiceName}]",
                    EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            if (infrastructureRoomId == -7)
            {
                var errMessage = CreateMessage($"{_enterpriseTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage($"{_enterpriseAccommodationTypeServiceTitle} [{model.ServiceName}]",
                EnumProcessType.Add,
                infrastructureRoomId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditTypeService(int id = 0)
        {
            var model = _accommodationServiceTypeServiceCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseAccommodationTypeServiceTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            model.ListTypeServices = _publicCateCache.GetAll((int)EnumTypePublicCate.TypeServiceAccommodation)
                .Select(c => new ListItem(c.CateName, c.CateId.ToString())).ToList();
            model.Reason = "Cập nhật";

            return PartialView("_EditTypeService", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditTypeService(CateAccommodationServiceTypeServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListTypeServices = _publicCateCache.GetAll((int)EnumTypePublicCate.TypeServiceAccommodation)
                    .Select(c => new ListItem(c.CateName, c.CateId.ToString())).ToList();
                return PartialView("_TypeService", model);
            }

            var infrastructureRoomId = _accommodationServiceTypeServiceCache.Save(
                new CateAccommodationServiceTypeServiceModel
                {
                    AccommodationTypeServiceId = model.AccommodationTypeServiceId,
                    EnterpriseId = model.EnterpriseId,
                    ServiceId = model.ServiceId,
                    ServiceName = model.ServiceName,
                    Reason = model.Reason,
                    SavedBy = User.Email
                });

            if (infrastructureRoomId == -9)
            {
                var errMessage = CreateMessage($"{_enterpriseAccommodationTypeServiceTitle} [{model.ServiceName}]",
                    EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            if (infrastructureRoomId == -7)
            {
                var errMessage = CreateMessage($"{_enterpriseTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage($"{_enterpriseAccommodationTypeServiceTitle} [{model.ServiceName}]",
                EnumProcessType.Edit,
                infrastructureRoomId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteTypeService(int id = 0)
        {
            var model = _accommodationServiceTypeServiceCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseAccommodationTypeServiceTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"),
                $"<b>{_enterpriseAccommodationTypeServiceTitle} [{model.ServiceName}]</b>");
            return PartialView("_DeleteTypeService", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteTypeService(CateAccommodationServiceTypeServiceModel model)
        {
            model.SavedBy = User.Email;
            var deleted = _accommodationServiceTypeServiceCache.Delete(model);

            var response = CreateMessage($"{_enterpriseAccommodationTypeServiceTitle} [{model.ServiceName}]",
                EnumProcessType.Delete, deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        #endregion

        #region Accommodation Info

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult AccommodationInfo(int enterpriseId)
        {
            var accommodationInfoModel = _accommodationServiceCache.GetInfoViaEnterprise(enterpriseId);
            accommodationInfoModel = accommodationInfoModel ?? new CateAccommodationServiceModel();
            accommodationInfoModel.EnterpriseId = enterpriseId;
            accommodationInfoModel.ListAccommodationClass = _publicCateCache
                .GetAll((int)EnumTypePublicCate.TypeAccommodationClass)
                .Select(c => new ListItem(c.CateName, c.CateId.ToString())).ToList();
            return PartialView("_TabAccommodationInfo", accommodationInfoModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult SaveInfo(CateAccommodationServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                var accommodationInfoModel = _accommodationServiceCache.GetInfoViaEnterprise(model.EnterpriseId);
                accommodationInfoModel = accommodationInfoModel ?? new CateAccommodationServiceModel();
                accommodationInfoModel.EnterpriseId = model.EnterpriseId;
                accommodationInfoModel.ListAccommodationClass = _publicCateCache
                    .GetAll((int)EnumTypePublicCate.TypeAccommodationClass)
                    .Select(c => new ListItem(c.CateName, c.CateId.ToString())).ToList();

                return PartialView("_TabAccommodationInfo", accommodationInfoModel);
            }

            Guid fileId;

            var dataCertificate =
                CreateDataRefDocs(model.EnterpriseId, model.FileAccommodationClassCertificate, out fileId);

            model.AccommodationClassCertificate = fileId == Guid.Empty ? model.AccommodationClassCertificate : fileId;

            var enterpriseId =
                _accommodationServiceCache.SaveInfo(new CateAccommodationServiceModel
                {
                    AccommodationServiceId = model.AccommodationServiceId,
                    EnterpriseId = model.EnterpriseId,
                    AccommodationClass = model.AccommodationClass,
                    AccommodationClassCertificate = model.AccommodationClassCertificate,
                    DataAccommodationClassCertificate = dataCertificate,
                    Reason = model.Reason,
                    SavedBy = User.Email
                });

            if (enterpriseId == -7)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            if (enterpriseId > 0 && model.FileAccommodationClassCertificate != null)
            {
                var moduleRefDocsPathFolder = string.Concat(_moduleRefDocsPathFolder, _accommodationInfoFolder, "/",
                    model.EnterpriseId.ToString());
                var moduleRefDocsAbsolutePathFolder = Server.MapPath(moduleRefDocsPathFolder);
                if (!Directory.Exists(moduleRefDocsAbsolutePathFolder))
                    Directory.CreateDirectory(moduleRefDocsAbsolutePathFolder);
                var sFileSecurityCertificatePath = string.Concat(moduleRefDocsAbsolutePathFolder, "/",
                    fileId.ToString().ToUpper(), Path.GetExtension(model.FileAccommodationClassCertificate.FileName));

                model.FileAccommodationClassCertificate.SaveAs(sFileSecurityCertificatePath);
            }

            var response = CreateMessage(
                $"{_enterpriseAccommodationTitle} - {_enterpriseAccommodationInfrastructureTitle}",
                EnumProcessType.Edit,
                enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Extend Function

        private DataTable CreateDataRefDocs(int enterpriseId, HttpPostedFileBase refDoc, out Guid fileId)
        {
            fileId = Guid.Empty;
            var lstDocs = new List<CateDocModel>();
            if (refDoc != null)
            {
                fileId = Guid.NewGuid();
                var moduleRefDocsPathFolder = string.Concat(_moduleRefDocsPathFolder, _accommodationInfoFolder, "/",
                    enterpriseId.ToString());

                lstDocs.Add(new CateDocModel
                {
                    FileId = fileId,
                    FilePath = moduleRefDocsPathFolder,
                    FileName = Path.GetFileNameWithoutExtension(refDoc.FileName),
                    FileExt = Path.GetExtension(refDoc.FileName),
                    ContentType = refDoc.ContentType
                });
            }

            var dataRefDocs = new DataTable();
            using (var reader =
                   ObjectReader.Create(lstDocs, "FileId", "FilePath", "FileName", "FileExt", "ContentType"))
            {
                dataRefDocs.Load(reader);
            }

            return dataRefDocs;
        }

        private int SaveUploadFile(CateAccommodationServiceCertificateModel model)
        {
            if (model.EnterpriseId <= 0) return -7;

            var lstSecurityCertificates = new List<CateDocModel>();
            var lstFireProtectionCertificates = new List<CateDocModel>();
            var lstEnvironmentalProtectionCertificates = new List<CateDocModel>();
            var lstHygieneFoodSafetyCertificates = new List<CateDocModel>();
            var lstConstructionPermits = new List<CateDocModel>();

            var moduleRefDocsPathFolder = string.Concat(_moduleRefDocsPathFolder, _accommodationCertificateFolder, "/",
                model.EnterpriseId.ToString());
            var moduleRefDocsAbsolutePathFolder = Server.MapPath(moduleRefDocsPathFolder);

            if (!Directory.Exists(moduleRefDocsAbsolutePathFolder))
                Directory.CreateDirectory(moduleRefDocsAbsolutePathFolder);

            #region FileSecurityCertificate

            if (model.FileSecurityCertificate != null)
            {
                var securityCertificate = new CateDocModel
                {
                    FileId = Guid.NewGuid(),
                    FilePath = moduleRefDocsPathFolder,
                    FileName = Path.GetFileNameWithoutExtension(model.FileSecurityCertificate.FileName),
                    FileExt = Path.GetExtension(model.FileSecurityCertificate.FileName),
                    ContentType = model.FileSecurityCertificate.ContentType
                };
                model.SecurityCertificate = securityCertificate.FileId;
                lstSecurityCertificates.Add(securityCertificate);
            }

            #endregion

            #region FileFireProtectionCertificate

            if (model.FileFireProtectionCertificate != null)
            {
                var fireProtectionCertificate = new CateDocModel
                {
                    FileId = Guid.NewGuid(),
                    FilePath = moduleRefDocsPathFolder,
                    FileName = Path.GetFileNameWithoutExtension(model.FileFireProtectionCertificate.FileName),
                    FileExt = Path.GetExtension(model.FileFireProtectionCertificate.FileName),
                    ContentType = model.FileFireProtectionCertificate.ContentType
                };
                model.FireProtectionCertificate = fireProtectionCertificate.FileId;
                lstFireProtectionCertificates.Add(fireProtectionCertificate);
            }

            #endregion

            #region FileEnvironmentalProtectionCertificate

            if (model.FileEnvironmentalProtectionCertificate != null)
            {
                var environmentalProtectionCertificate = new CateDocModel
                {
                    FileId = Guid.NewGuid(),
                    FilePath = moduleRefDocsPathFolder,
                    FileName = Path.GetFileNameWithoutExtension(model.FileEnvironmentalProtectionCertificate.FileName),
                    FileExt = Path.GetExtension(model.FileEnvironmentalProtectionCertificate.FileName),
                    ContentType = model.FileEnvironmentalProtectionCertificate.ContentType
                };
                model.EnvironmentalProtectionCertificate = environmentalProtectionCertificate.FileId;
                lstEnvironmentalProtectionCertificates.Add(environmentalProtectionCertificate);
            }

            #endregion

            #region FileHygieneFoodSafetyCertificate

            if (model.FileHygieneFoodSafetyCertificate != null)
            {
                var hygieneFoodSafetyCertificate = new CateDocModel
                {
                    FileId = Guid.NewGuid(),
                    FilePath = moduleRefDocsPathFolder,
                    FileName = Path.GetFileNameWithoutExtension(model.FileHygieneFoodSafetyCertificate.FileName),
                    FileExt = Path.GetExtension(model.FileHygieneFoodSafetyCertificate.FileName),
                    ContentType = model.FileHygieneFoodSafetyCertificate.ContentType
                };
                model.HygieneFoodSafetyCertificate = hygieneFoodSafetyCertificate.FileId;
                lstHygieneFoodSafetyCertificates.Add(hygieneFoodSafetyCertificate);
            }

            #endregion

            #region FileConstructionPermit

            if (model.FileConstructionPermit != null)
            {
                var fileConstructionPermit = new CateDocModel
                {
                    FileId = Guid.NewGuid(),
                    FilePath = moduleRefDocsPathFolder,
                    FileName = Path.GetFileNameWithoutExtension(model.FileConstructionPermit.FileName),
                    FileExt = Path.GetExtension(model.FileConstructionPermit.FileName),
                    ContentType = model.FileConstructionPermit.ContentType
                };
                model.ConstructionPermit = fileConstructionPermit.FileId;
                lstConstructionPermits.Add(fileConstructionPermit);
            }

            #endregion

            var enterpriseId = _accommodationServiceCache.SaveCertificates(new CateAccommodationServiceCertificateModel
            {
                EnterpriseId = model.EnterpriseId,
                SecurityCertificate = model.SecurityCertificate,
                DataSecurityCertificate = CreateDataRefDocs(lstSecurityCertificates),
                FireProtectionCertificate = model.FireProtectionCertificate,
                DataFireProtectionCertificate = CreateDataRefDocs(lstFireProtectionCertificates),
                EnvironmentalProtectionCertificate = model.EnvironmentalProtectionCertificate,
                DataEnvironmentalProtectionCertificate = CreateDataRefDocs(lstEnvironmentalProtectionCertificates),
                HygieneFoodSafetyCertificate = model.HygieneFoodSafetyCertificate,
                DataHygieneFoodSafetyCertificate = CreateDataRefDocs(lstHygieneFoodSafetyCertificates),
                ConstructionPermit = model.ConstructionPermit,
                DataConstructionPermit = CreateDataRefDocs(lstConstructionPermits),
                Reason = model.Reason,
                SavedBy = User.Email
            });

            if (enterpriseId <= 0) return enterpriseId;
            if (model.FileSecurityCertificate != null)
            {
                var sFileSecurityCertificatePath = string.Concat(moduleRefDocsAbsolutePathFolder, "/",
                    model.SecurityCertificate.ToString().ToUpper(),
                    Path.GetExtension(model.FileSecurityCertificate.FileName));

                model.FileSecurityCertificate.SaveAs(sFileSecurityCertificatePath);
            }

            if (model.FileFireProtectionCertificate != null)
            {
                var sFileFireProtectionCertificatePath = string.Concat(moduleRefDocsAbsolutePathFolder, "/",
                    model.FireProtectionCertificate.ToString().ToUpper(),
                    Path.GetExtension(model.FileFireProtectionCertificate.FileName));

                model.FileFireProtectionCertificate.SaveAs(sFileFireProtectionCertificatePath);
            }

            if (model.FileEnvironmentalProtectionCertificate != null)
            {
                var sFileEnvironmentalProtectionCertificatePath = string.Concat(moduleRefDocsAbsolutePathFolder, "/",
                    model.EnvironmentalProtectionCertificate.ToString().ToUpper(),
                    Path.GetExtension(model.FileEnvironmentalProtectionCertificate.FileName));

                model.FileEnvironmentalProtectionCertificate.SaveAs(sFileEnvironmentalProtectionCertificatePath);
            }

            if (model.FileHygieneFoodSafetyCertificate != null)
            {
                var sFileHygieneFoodSafetyCertificatePath = string.Concat(moduleRefDocsAbsolutePathFolder, "/",
                    model.HygieneFoodSafetyCertificate.ToString().ToUpper(),
                    Path.GetExtension(model.FileHygieneFoodSafetyCertificate.FileName));

                model.FileHygieneFoodSafetyCertificate.SaveAs(sFileHygieneFoodSafetyCertificatePath);
            }

            if (model.FileConstructionPermit != null)
            {
                var sFileConstructionPermitPath = string.Concat(moduleRefDocsAbsolutePathFolder, "/",
                    model.ConstructionPermit.ToString().ToUpper(),
                    Path.GetExtension(model.FileConstructionPermit.FileName));

                model.FileConstructionPermit.SaveAs(sFileConstructionPermitPath);
            }

            return enterpriseId;
        }

        private DataTable CreateDataRefDocs(List<CateDocModel> lstDocs)
        {
            var dataRefDocs = new DataTable();
            using (var reader =
                   ObjectReader.Create(lstDocs, "FileId", "FilePath", "FileName", "FileExt", "ContentType"))
            {
                dataRefDocs.Load(reader);
            }

            return dataRefDocs;
        }

        #endregion
    }
}