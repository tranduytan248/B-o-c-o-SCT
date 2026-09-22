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
using CenIT.ReportTourism.Caches.Cate.TouristAttraction;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Core.Conts;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Cate.TouristAttraction;
using CenIT.ReportTourism.Models.Sys;
using FastMember;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Controllers.TypeBusiness
{
    public class TypeTouristAttractionController : AppController
    {
        private readonly CateEnterpriseCache _enterpriseCache;

        private readonly string _enterpriseTitle = AppProcessor.Messagor.GetMessage("Enterprise_Title");

        private readonly string _enterpriseTouristAttractionCertificateTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_TouristAttraction_Certificate_Title");

        private readonly string _enterpriseTouristAttractionHRInformationTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_TouristAttraction_HRInformation_Title");

        private readonly string _enterpriseTouristAttractionInfrastructureTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_TouristAttraction_Infrastructure_Title");

        private readonly string _enterpriseTouristAttractionTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_TouristAttraction_Title");

        private readonly string _enterpriseTouristAttractionTypeServiceTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_TouristAttraction_Infrastructure_TypeService_Title");

        private readonly string _moduleRefDocsPathFolder =
            ConfigurationManager.AppSettings["AttachmentFolderPath"] ?? @"/Contents/Modules/Cate/Attachments/";

        private readonly CatePublicCateCache _publicCateCache;
        private readonly CateTouristAttractionCache _touristAttractionCache;

        private readonly string _touristAttractionCertificateFolder = "TouristAttractionCertificates";
        private readonly CateTouristAttractionTypeServiceCache _touristAttractionTypeServiceCache;

        public TypeTouristAttractionController()
        {
            _publicCateCache = new CatePublicCateCache();
            _enterpriseCache = new CateEnterpriseCache();
            _touristAttractionCache = new CateTouristAttractionCache();
            _touristAttractionTypeServiceCache = new CateTouristAttractionTypeServiceCache();
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
            var touristAttractionModel = new CateTouristAttractionModel
            {
                EnterpriseId = enterpriseId
            };
            return PartialView("_Info", touristAttractionModel);
        }

        #region TouristAttraction Info

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult TouristAttractionInfo(int enterpriseId)
        {
            var touristAttractionInfoModel = _touristAttractionCache.GetInfoViaEnterprise(enterpriseId);
            touristAttractionInfoModel = touristAttractionInfoModel ?? new CateTouristAttractionModel();
            touristAttractionInfoModel.EnterpriseId = enterpriseId;
            touristAttractionInfoModel.ListTypeTourismActivitieIds =
                string.IsNullOrEmpty(touristAttractionInfoModel.TypeTourismActivities)
                    ? new List<int>()
                    : touristAttractionInfoModel.TypeTourismActivities.Split(',').Select(int.Parse).ToList();
            touristAttractionInfoModel.ListTypeTourismActivities = _publicCateCache
                .GetAll((int)EnumTypePublicCate.TypeTourismActivity)
                .Select(c => new ListItem(c.CateName, c.CateId.ToString())).ToList();
            return PartialView("_TabTouristAttractionInfo", touristAttractionInfoModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult SaveInfo(CateTouristAttractionModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListTypeTourismActivities = _publicCateCache
                    .GetAll((int)EnumTypePublicCate.TypeTourismActivity)
                    .Select(c => new ListItem(c.CateName, c.CateId.ToString())).ToList();
                return PartialView("_TabTouristAttractionInfo", model);
            }

            model.TypeTourismActivities = string.Join(",", model.ListTypeTourismActivitieIds);
            var enterpriseId =
                _touristAttractionCache.SaveInfo(new CateTouristAttractionModel
                {
                    TouristAttractionId = model.TouristAttractionId,
                    EnterpriseId = model.EnterpriseId,
                    TypeTourismActivities = model.TypeTourismActivities,
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
                $"{_enterpriseTouristAttractionTitle} - {_enterpriseTouristAttractionInfrastructureTitle}",
                EnumProcessType.Edit,
                enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Certificate

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult Certificate(int enterpriseId)
        {
            var certificateModel = _touristAttractionCache.GetCertificateViaEnterprise(enterpriseId);
            certificateModel = certificateModel ?? new CateTouristAttractionCertificateModel();
            certificateModel.EnterpriseId = enterpriseId;
            return PartialView("_TabCertificate", certificateModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult SaveCertificate(CateTouristAttractionCertificateModel model)
        {
            if (!ModelState.IsValid)
            {
                var certificateModel = _touristAttractionCache.GetCertificateViaEnterprise(model.EnterpriseId);
                certificateModel = certificateModel ?? new CateTouristAttractionCertificateModel();
                certificateModel.EnterpriseId = model.EnterpriseId;
                return PartialView("_TabCertificate", model);
            }

            Guid fileId;

            var dataLicenseCertificate =
                CreateDataRefDocs(model.EnterpriseId, model.FileLicenseFile, _touristAttractionCertificateFolder,
                    out fileId);

            model.LicenseFile = fileId == Guid.Empty ? model.LicenseFile : fileId;

            var enterpriseId =
                _touristAttractionCache.SaveCertificates(new CateTouristAttractionCertificateModel
                {
                    CertificateId = model.CertificateId,
                    EnterpriseId = model.EnterpriseId,
                    LicenseFile = model.LicenseFile,
                    DataLicenseFile = dataLicenseCertificate,
                    LicenseNumber = model.LicenseNumber,
                    ReleaseDate = model.ReleaseDate,
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

            if (enterpriseId > 0 && model.FileLicenseFile != null)
            {
                var moduleRefDocsPathFolder = string.Concat(_moduleRefDocsPathFolder,
                    _touristAttractionCertificateFolder, "/", model.EnterpriseId.ToString());
                var moduleRefDocsAbsolutePathFolder = Server.MapPath(moduleRefDocsPathFolder);
                if (!Directory.Exists(moduleRefDocsAbsolutePathFolder))
                    Directory.CreateDirectory(moduleRefDocsAbsolutePathFolder);
                var sFileSecurityCertificatePath = string.Concat(moduleRefDocsAbsolutePathFolder, "/",
                    model.LicenseFile.ToString().ToUpper(), Path.GetExtension(model.FileLicenseFile.FileName));

                model.FileLicenseFile.SaveAs(sFileSecurityCertificatePath);
            }

            var response = CreateMessage(
                $"{_enterpriseTouristAttractionTitle} - {_enterpriseTouristAttractionCertificateTitle}",
                EnumProcessType.Edit,
                enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region HRInformation

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult HRInformation(int enterpriseId)
        {
            var hrInformationModel = _touristAttractionCache.GetHRInformationViaEnterprise(enterpriseId);
            hrInformationModel = hrInformationModel ?? new CateTouristAttractionHRInformationModel();
            hrInformationModel.EnterpriseId = enterpriseId;
            return PartialView("_TabHRInformation", hrInformationModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult SaveHRInformation(CateTouristAttractionHRInformationModel model)
        {
            if (!ModelState.IsValid) return PartialView("_TabHRInformation", model);

            var enterpriseId = _touristAttractionCache.SaveHRInformations(new CateTouristAttractionHRInformationModel
            {
                EnterpriseId = model.EnterpriseId,
                TotalStaff = model.TotalStaff,
                StaffOnLiteracyOnCollege = model.StaffOnLiteracyOnCollege,
                StaffOnUniversity = model.StaffOnUniversity,
                StaffOnCollege = model.StaffOnCollege,
                StaffOnIntermediate = model.StaffOnIntermediate,
                StaffOnHighSchool = model.StaffOnHighSchool,
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
                $"{_enterpriseTouristAttractionTitle} - {_enterpriseTouristAttractionHRInformationTitle}",
                EnumProcessType.Edit,
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
            var infrastructureModel = _touristAttractionCache.GetInfrastructureViaEnterprise(enterpriseId);
            infrastructureModel = infrastructureModel ?? new CateTouristAttractionInfrastructureModel();
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
        public ActionResult SaveInfrastructure(CateTouristAttractionInfrastructureModel model)
        {
            if (!ModelState.IsValid) return PartialView("_TabInfrastructure", model);

            var enterpriseId =
                _touristAttractionCache.SaveInfrastructures(new CateTouristAttractionInfrastructureModel
                {
                    EnterpriseId = model.EnterpriseId,
                    InitialInvestmentCapital = model.InitialInvestmentCapital,
                    UpgradeInvestmentCapital = model.UpgradeInvestmentCapital,
                    TotalArea = model.TotalArea,
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
                $"{_enterpriseTouristAttractionTitle} - {_enterpriseTouristAttractionInfrastructureTitle}",
                EnumProcessType.Edit,
                enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        #region TypeService

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
            var data = _touristAttractionTypeServiceCache.Get(enterpriseId, out total, dataSearch);
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
            var infrastructureTypeServiceModel = new CateTouristAttractionTypeServiceModel
            {
                EnterpriseId = enterpriseId,
                Reason = "Thêm mới"
            };
            return PartialView("_AddTypeService", infrastructureTypeServiceModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult AddTypeService(CateTouristAttractionTypeServiceModel model)
        {
            if (!ModelState.IsValid) return PartialView("_TypeService", model);

            var infrastructureTypeServiceId = _touristAttractionTypeServiceCache.Save(
                new CateTouristAttractionTypeServiceModel
                {
                    TouristAttractionTypeServiceId = 0,
                    EnterpriseId = model.EnterpriseId,
                    ServiceName = model.ServiceName,
                    Reason = model.Reason,
                    SavedBy = User.Email
                });

            if (infrastructureTypeServiceId == -9)
            {
                var errMessage = CreateMessage($"{_enterpriseTouristAttractionTypeServiceTitle} [{model.ServiceName}]",
                    EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            if (infrastructureTypeServiceId == -7)
            {
                var errMessage = CreateMessage($"{_enterpriseTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage($"{_enterpriseTouristAttractionTypeServiceTitle} [{model.ServiceName}]",
                EnumProcessType.Add,
                infrastructureTypeServiceId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditTypeService(int id = 0)
        {
            var model = _touristAttractionTypeServiceCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTouristAttractionTypeServiceTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            return PartialView("_EditTypeService", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditTypeService(CateTouristAttractionTypeServiceModel model)
        {
            if (!ModelState.IsValid) return PartialView("_TypeService", model);

            var infrastructureTypeServiceId = _touristAttractionTypeServiceCache.Save(
                new CateTouristAttractionTypeServiceModel
                {
                    TouristAttractionTypeServiceId = model.TouristAttractionTypeServiceId,
                    EnterpriseId = model.EnterpriseId,
                    ServiceName = model.ServiceName,
                    Reason = model.Reason,
                    SavedBy = User.Email
                });

            if (infrastructureTypeServiceId == -9)
            {
                var errMessage = CreateMessage($"{_enterpriseTouristAttractionTypeServiceTitle} [{model.ServiceName}]",
                    EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            if (infrastructureTypeServiceId == -7)
            {
                var errMessage = CreateMessage($"{_enterpriseTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage($"{_enterpriseTouristAttractionTypeServiceTitle} [{model.ServiceName}]",
                EnumProcessType.Edit,
                infrastructureTypeServiceId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteTypeService(int id = 0)
        {
            var model = _touristAttractionTypeServiceCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTouristAttractionTypeServiceTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"),
                $"<b>{_enterpriseTouristAttractionTypeServiceTitle} [{model.ServiceName}]</b>");
            return PartialView("_DeleteTypeService", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteTypeService(CateTouristAttractionTypeServiceModel model)
        {
            model.SavedBy = User.Email;
            var deleted = _touristAttractionTypeServiceCache.Delete(model);

            var response = CreateMessage($"{_enterpriseTouristAttractionTypeServiceTitle} [{model.ServiceName}]",
                EnumProcessType.Delete, deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        #endregion

        #endregion

        #region Extend Function

        private DataTable CreateDataRefDocs(int enterpriseId, HttpPostedFileBase refDoc, string saveFolder,
            out Guid fileId)
        {
            fileId = Guid.Empty;
            var lstDocs = new List<CateDocModel>();
            if (refDoc != null)
            {
                fileId = Guid.NewGuid();
                var moduleRefDocsPathFolder =
                    string.Concat(_moduleRefDocsPathFolder, saveFolder, "/", enterpriseId.ToString());

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