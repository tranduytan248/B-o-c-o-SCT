using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Mvc;
using CenIT.ReportTourism.Caches.Cate;
using CenIT.ReportTourism.Caches.Cate.Traveling;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Cate.Traveling;
using CenIT.ReportTourism.Models.Sys;
using FastMember;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Modules.CateModule.Controllers
{
    public class TravelingController : AppController
    {
        private readonly CateEnterpriseCache _enterpriseCache;

        private readonly string _enterpriseTitle = AppProcessor.Messagor.GetMessage("Enterprise_Title");

        private readonly string _enterpriseTravelingBranchOfficeTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_Traveling_BranchOffice_Title");

        private readonly string _enterpriseTravelingCertificateTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_Traveling_Certificate_Title");

        private readonly string _enterpriseTravelingCertificateTravelEscrowTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_Traveling_CertificateTravelEscrow_Title");

        private readonly string _enterpriseTravelingHRInformationTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_Traveling_HRInformation_Title");

        private readonly string _enterpriseTravelingTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_Traveling_Title");

        private readonly string _moduleRefDocsPathFolder =
            ConfigurationManager.AppSettings["AttachmentFolderPath"] ?? @"/Contents/Modules/Cate/Attachments/";

        private readonly string _travelingCertificateFolder = "TravelingCertificates";
        private readonly string _travelingCertificateTravelEscrowFolder = "TravelingCertificateTravelEscrows";
        private readonly CateTravelingServiceBranchOfficeCache _travelingServiceBranchOfficeCache;
        private readonly CateTravelingServiceCache _travelingServiceCache;

        public TravelingController()
        {
            _enterpriseCache = new CateEnterpriseCache();
            _travelingServiceCache = new CateTravelingServiceCache();
            _travelingServiceBranchOfficeCache = new CateTravelingServiceBranchOfficeCache();
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
            var travelingServiceModel = new CateTravelingServiceModel
            {
                EnterpriseId = enterpriseId
            };
            return PartialView("_Traveling", travelingServiceModel);
        }

        #region Certificate

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult Certificate(int enterpriseId)
        {
            var certificateModel = _travelingServiceCache.GetCertificateViaEnterprise(enterpriseId);
            certificateModel = certificateModel ?? new CateTravelingServiceCertificateModel();
            certificateModel.EnterpriseId = enterpriseId;
            return PartialView("_TabCertificate", certificateModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult SaveCertificate(CateTravelingServiceCertificateModel model)
        {
            if (!ModelState.IsValid) return PartialView("_TabCertificate", model);

            var fileId = Guid.NewGuid();

            var dataLicenseCertificate =
                CreateDataRefDocs(model.EnterpriseId, fileId, model.FileLicenseFile, _travelingCertificateFolder);

            var enterpriseId =
                _travelingServiceCache.SaveCertificates(new CateTravelingServiceCertificateModel
                {
                    CertificateId = model.CertificateId,
                    EnterpriseId = model.EnterpriseId,
                    LicenseFile = fileId,
                    DataLicenseFile = dataLicenseCertificate,
                    LicenseNumber = model.LicenseNumber,
                    ReleaseDate = model.ReleaseDate,
                    TimeRelease = model.TimeRelease,
                    Inbound = model.Inbound,
                    Outbound = model.Outbound,
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
                var moduleRefDocsPathFolder = string.Concat(_moduleRefDocsPathFolder, _travelingCertificateFolder, "/",
                    model.EnterpriseId.ToString());
                var moduleRefDocsAbsolutePathFolder = Server.MapPath(moduleRefDocsPathFolder);
                if (!Directory.Exists(moduleRefDocsAbsolutePathFolder))
                    Directory.CreateDirectory(moduleRefDocsAbsolutePathFolder);
                var sFileSecurityCertificatePath = string.Concat(moduleRefDocsAbsolutePathFolder, "/",
                    fileId.ToString().ToUpper(), Path.GetExtension(model.FileLicenseFile.FileName));

                model.FileLicenseFile.SaveAs(sFileSecurityCertificatePath);
            }

            var response = CreateMessage($"{_enterpriseTravelingTitle} - {_enterpriseTravelingCertificateTitle}",
                EnumProcessType.Edit,
                enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Certificate Travel Escrow

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult CertificateTravelEscrow(int enterpriseId)
        {
            var cul = CultureInfo.GetCultureInfo("vi-VN");
            var certificateTravelEscrowModel =
                _travelingServiceCache.GetCertificateViaEnterpriseTravelEscrow(enterpriseId);
            certificateTravelEscrowModel =
                certificateTravelEscrowModel ?? new CateTravelingServiceCertificateTravelEscrowModel();
            certificateTravelEscrowModel.EnterpriseId = enterpriseId;
            certificateTravelEscrowModel.AnnouncedView = certificateTravelEscrowModel.Amount > 0
                ? certificateTravelEscrowModel.Amount.ToString("#,### VND", cul.NumberFormat)
                : string.Empty;
            return PartialView("_TabCertificateTravelEscrow", certificateTravelEscrowModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult SaveCertificateTravelEscrow(CateTravelingServiceCertificateTravelEscrowModel model)
        {
            if (!ModelState.IsValid) return PartialView("_TabCertificateTravelEscrow", model);

            var fileId = Guid.NewGuid();

            var dataCertificate =
                CreateDataRefDocs(model.EnterpriseId, fileId, model.FileCertificate,
                    _travelingCertificateTravelEscrowFolder);

            var enterpriseId =
                _travelingServiceCache.SaveCertificateTravelEscrows(new CateTravelingServiceCertificateTravelEscrowModel
                {
                    CertificateTravelEscrowId = model.CertificateTravelEscrowId,
                    EnterpriseId = model.EnterpriseId,
                    CertificateId = fileId,
                    DataCertificate = dataCertificate,
                    BankName = model.BankName,
                    BankAccountNo = model.BankAccountNo,
                    Amount = model.Amount,
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

            if (enterpriseId > 0 && model.FileCertificate != null)
            {
                var moduleRefDocsPathFolder = string.Concat(_moduleRefDocsPathFolder,
                    _travelingCertificateTravelEscrowFolder, "/", model.EnterpriseId.ToString());
                var moduleRefDocsAbsolutePathFolder = Server.MapPath(moduleRefDocsPathFolder);
                if (!Directory.Exists(moduleRefDocsAbsolutePathFolder))
                    Directory.CreateDirectory(moduleRefDocsAbsolutePathFolder);
                var sFileSecurityCertificatePath = string.Concat(moduleRefDocsAbsolutePathFolder, "/",
                    fileId.ToString().ToUpper(), Path.GetExtension(model.FileCertificate.FileName));

                model.FileCertificate.SaveAs(sFileSecurityCertificatePath);
            }

            var response = CreateMessage(
                $"{_enterpriseTravelingCertificateTravelEscrowTitle} - {_enterpriseTravelingCertificateTitle}",
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
            var hrInformationModel = _travelingServiceCache.GetHRInformationViaEnterprise(enterpriseId);
            hrInformationModel = hrInformationModel ?? new CateTravelingServiceHRInformationModel();
            hrInformationModel.EnterpriseId = enterpriseId;
            return PartialView("_TabHRInformation", hrInformationModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult SaveHRInformation(CateTravelingServiceHRInformationModel model)
        {
            if (!ModelState.IsValid) return PartialView("_TabHRInformation", model);

            var enterpriseId = _travelingServiceCache.SaveHRInformations(new CateTravelingServiceHRInformationModel
            {
                EnterpriseId = model.EnterpriseId,
                StaffOnLeader = model.StaffOnLeader,
                StaffOnManager = model.StaffOnManager,
                TourGuide = model.TourGuide,
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

            var response = CreateMessage($"{_enterpriseTravelingTitle} - {_enterpriseTravelingHRInformationTitle}",
                EnumProcessType.Edit,
                enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Branch Office

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult BranchOffice(int enterpriseId)
        {
            return PartialView("_TabBranchOffice", enterpriseId);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetBranchOffices(int? enterpriseId = null)
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
            var data = _travelingServiceBranchOfficeCache.Get(enterpriseId, out total, dataSearch);
            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult AddBranchOffice(int enterpriseId)
        {
            var infrastructureRoomModel = new CateTravelingServiceBranchOfficeModel
            {
                EnterpriseId = enterpriseId,
                Reason = "Thêm mới"
            };
            return PartialView("_AddBranchOffice", infrastructureRoomModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult AddBranchOffice(CateTravelingServiceBranchOfficeModel model)
        {
            if (!ModelState.IsValid) return PartialView("_BranchOffice", model);

            var infrastructureRoomId = _travelingServiceBranchOfficeCache.Save(new CateTravelingServiceBranchOfficeModel
            {
                BranchOfficeId = model.BranchOfficeId,
                EnterpriseId = model.EnterpriseId,
                BranchName = model.BranchName,
                BranchAddress = model.BranchAddress,
                Reason = model.Reason,
                SavedBy = User.Email
            });

            if (infrastructureRoomId == -9)
            {
                var errMessage = CreateMessage($"{_enterpriseTravelingBranchOfficeTitle} [{model.BranchName}]",
                    EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            if (infrastructureRoomId == -7)
            {
                var errMessage = CreateMessage($"{_enterpriseTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage($"{_enterpriseTravelingBranchOfficeTitle} [{model.BranchName}]",
                EnumProcessType.Add,
                infrastructureRoomId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditBranchOffice(int id = 0)
        {
            var model = _travelingServiceBranchOfficeCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTravelingBranchOfficeTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            model.Reason = "Cập nhật";
            return PartialView("_EditBranchOffice", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditBranchOffice(CateTravelingServiceBranchOfficeModel model)
        {
            if (!ModelState.IsValid) return PartialView("_BranchOffice", model);

            var infrastructureRoomId = _travelingServiceBranchOfficeCache.Save(new CateTravelingServiceBranchOfficeModel
            {
                BranchOfficeId = model.BranchOfficeId,
                EnterpriseId = model.EnterpriseId,
                BranchName = model.BranchName,
                BranchAddress = model.BranchAddress,
                Reason = model.Reason,
                SavedBy = User.Email
            });

            if (infrastructureRoomId == -9)
            {
                var errMessage = CreateMessage($"{_enterpriseTravelingBranchOfficeTitle} [{model.BranchName}]",
                    EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            if (infrastructureRoomId == -7)
            {
                var errMessage = CreateMessage($"{_enterpriseTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage($"{_enterpriseTravelingBranchOfficeTitle} [{model.BranchName}]",
                EnumProcessType.Edit,
                infrastructureRoomId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteBranchOffice(int id = 0)
        {
            var model = _travelingServiceBranchOfficeCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTravelingBranchOfficeTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"),
                $"<b>{_enterpriseTravelingBranchOfficeTitle} [{model.BranchName}]</b>");
            return PartialView("_DeleteBranchOffice", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteBranchOffice(CateTravelingServiceBranchOfficeModel model)
        {
            model.SavedBy = User.Email;
            var deleted = _travelingServiceBranchOfficeCache.Delete(model);

            var response = CreateMessage($"{_enterpriseTravelingBranchOfficeTitle} [{model.BranchName}]",
                EnumProcessType.Delete, deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        #endregion

        #region Extend Function

        private DataTable CreateDataRefDocs(int enterpriseId, Guid fileId, HttpPostedFileBase refDoc, string saveFolder)
        {
            var lstDocs = new List<CateDocModel>();
            if (refDoc != null)
            {
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