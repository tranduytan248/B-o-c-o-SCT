using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CenIT.ReportTourism.Caches.Cate;
using CenIT.ReportTourism.Caches.Cate.TransportTourists;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Core.Conts;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Cate.TransportTourists;
using CenIT.ReportTourism.Models.Sys;
using FastMember;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;
using TSFramework.Core.Helpers;

namespace CenIT.ReportTourism.Modules.CateModule.Controllers
{
    public class TransportTouristsController : AppController
    {
        private readonly CateEnterpriseCache _enterpriseCache;

        private readonly string _enterpriseTitle = AppProcessor.Messagor.GetMessage("Enterprise_Title");

        private readonly string _enterpriseTransportTouristsCertificateTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_TransportTourists_Certificate_Title");

        private readonly string _enterpriseTransportTouristsHRInformationTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_TransportTourists_HRInformation_Title");

        private readonly string _enterpriseTransportTouristsInfrastructureClassTransportTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_TransportTourists_Infrastructure_ClassTransport_Title");

        private readonly string _enterpriseTransportTouristsInfrastructureTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_TransportTourists_Infrastructure_Title");

        private readonly string _enterpriseTransportTouristsTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_TransportTourists_Title");

        private readonly string _moduleRefDocsPathFolder =
            ConfigurationManager.AppSettings["AttachmentFolderPath"] ?? @"/Contents/Modules/Cate/Attachments/";

        private readonly string _transportTouristsCertificateFolder = "TransportTouristsCertificates";
        private readonly CateTransportTouristsServiceCache _transportTouristsServiceCache;
        private readonly CateTransportTouristsServiceClassTransportCache _transportTouristsServiceClassTransportCache;

        public TransportTouristsController()
        {
            _enterpriseCache = new CateEnterpriseCache();
            _transportTouristsServiceCache = new CateTransportTouristsServiceCache();
            _transportTouristsServiceClassTransportCache = new CateTransportTouristsServiceClassTransportCache();
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
            var transportTouristsServiceModel = new CateTransportTouristsServiceModel
            {
                EnterpriseId = enterpriseId
            };
            return PartialView("_TransportTourists", transportTouristsServiceModel);
        }

        #region Certificate

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult Certificate(int enterpriseId)
        {
            var certificateModel = _transportTouristsServiceCache.GetCertificateViaEnterprise(enterpriseId);
            certificateModel = certificateModel ?? new CateTransportTouristsServiceCertificateModel();
            certificateModel.EnterpriseId = enterpriseId;
            return PartialView("_TabCertificate", certificateModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult SaveCertificate(CateTransportTouristsServiceCertificateModel model)
        {
            if (!ModelState.IsValid) return PartialView("_TabCertificate", model);

            var fileId = Guid.NewGuid();

            var dataLicenseCertificate =
                CreateDataRefDocs(model.EnterpriseId, fileId, model.FileLicenseFile,
                    _transportTouristsCertificateFolder);

            var enterpriseId =
                _transportTouristsServiceCache.SaveCertificates(new CateTransportTouristsServiceCertificateModel
                {
                    CertificateId = model.CertificateId,
                    EnterpriseId = model.EnterpriseId,
                    LicenseFile = fileId,
                    DataLicenseFile = dataLicenseCertificate,
                    LicenseNumber = model.LicenseNumber,
                    ReleaseDate = model.ReleaseDate,
                    TimeRelease = model.TimeRelease,
                    WaterWay = model.WaterWay,
                    Road = model.Road,
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
                    _transportTouristsCertificateFolder, "/", model.EnterpriseId.ToString());
                var moduleRefDocsAbsolutePathFolder = Server.MapPath(moduleRefDocsPathFolder);
                if (!Directory.Exists(moduleRefDocsAbsolutePathFolder))
                    Directory.CreateDirectory(moduleRefDocsAbsolutePathFolder);
                var sFileSecurityCertificatePath = string.Concat(moduleRefDocsAbsolutePathFolder, "/",
                    fileId.ToString().ToUpper(), Path.GetExtension(model.FileLicenseFile.FileName));

                model.FileLicenseFile.SaveAs(sFileSecurityCertificatePath);
            }

            var response = CreateMessage(
                $"{_enterpriseTransportTouristsTitle} - {_enterpriseTransportTouristsCertificateTitle}",
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
            var hrInformationModel = _transportTouristsServiceCache.GetHRInformationViaEnterprise(enterpriseId);
            hrInformationModel = hrInformationModel ?? new CateTransportTouristsServiceHRInformationModel();
            hrInformationModel.EnterpriseId = enterpriseId;
            return PartialView("_TabHRInformation", hrInformationModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult SaveHRInformation(CateTransportTouristsServiceHRInformationModel model)
        {
            if (!ModelState.IsValid) return PartialView("_TabHRInformation", model);

            var enterpriseId = _transportTouristsServiceCache.SaveHRInformations(
                new CateTransportTouristsServiceHRInformationModel
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
                $"{_enterpriseTransportTouristsTitle} - {_enterpriseTransportTouristsHRInformationTitle}",
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
            var infrastructureModel = _transportTouristsServiceCache.GetInfrastructureViaEnterprise(enterpriseId);
            infrastructureModel = infrastructureModel ?? new CateTransportTouristsServiceInfrastructureModel();
            infrastructureModel.EnterpriseId = enterpriseId;
            return PartialView("_TabInfrastructure", infrastructureModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult SaveInfrastructure(CateTransportTouristsServiceInfrastructureModel model)
        {
            if (!ModelState.IsValid) return PartialView("_TabInfrastructure", model);

            var enterpriseId =
                _transportTouristsServiceCache.SaveInfrastructures(new CateTransportTouristsServiceInfrastructureModel
                {
                    EnterpriseId = model.EnterpriseId,
                    TotalTransports = model.TotalTransports,
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
                $"{_enterpriseTransportTouristsTitle} - {_enterpriseTransportTouristsInfrastructureTitle}",
                EnumProcessType.Edit,
                enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        #region ClassTransport

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetClassTransports(int? enterpriseId = null)
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
            var data = _transportTouristsServiceClassTransportCache.Get(enterpriseId, out total, dataSearch);
            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult AddClassTransport(int enterpriseId)
        {
            var infrastructureClassTransportModel = new CateTransportTouristsServiceInfrastructureClassTransportModel
            {
                EnterpriseId = enterpriseId,
                ListTypeTransports = Enum.GetValues(typeof(EnumClassTransport))
                    .Cast<EnumClassTransport>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList(),
                Reason = "Thêm mới"
            };
            return PartialView("_AddClassTransport", infrastructureClassTransportModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult AddClassTransport(CateTransportTouristsServiceInfrastructureClassTransportModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListTypeTransports = Enum.GetValues(typeof(EnumClassTransport))
                    .Cast<EnumClassTransport>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList();
                return PartialView("_ClassTransport", model);
            }

            var infrastructureClassTransportId = _transportTouristsServiceClassTransportCache.Save(
                new CateTransportTouristsServiceInfrastructureClassTransportModel
                {
                    InfrastructureClassTransportId = 0,
                    EnterpriseId = model.EnterpriseId,
                    TypeTransport = model.TypeTransport,
                    TypeTransportName = model.TypeTransportName,
                    ClassTransportName = model.ClassTransportName,
                    TotalTransport = model.TotalTransport,
                    Reason = model.Reason,
                    SavedBy = User.Email
                });

            if (infrastructureClassTransportId == -9)
            {
                var errMessage =
                    CreateMessage(
                        $"{_enterpriseTransportTouristsInfrastructureClassTransportTitle} [{model.TypeTransportName} - {model.ClassTransportName}]",
                        EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            if (infrastructureClassTransportId == -7)
            {
                var errMessage = CreateMessage($"{_enterpriseTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage(
                $"{_enterpriseTransportTouristsInfrastructureClassTransportTitle} [{model.TypeTransportName} - {model.ClassTransportName}]",
                EnumProcessType.Add,
                infrastructureClassTransportId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditClassTransport(int id = 0)
        {
            var model = _transportTouristsServiceClassTransportCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTransportTouristsInfrastructureClassTransportTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            model.ListTypeTransports = Enum.GetValues(typeof(EnumClassTransport))
                .Cast<EnumClassTransport>()
                .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                    ((int)x).ToString()))
                .ToList();
            return PartialView("_EditClassTransport", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditClassTransport(CateTransportTouristsServiceInfrastructureClassTransportModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListTypeTransports = Enum.GetValues(typeof(EnumClassTransport))
                    .Cast<EnumClassTransport>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList();
                return PartialView("_ClassTransport", model);
            }

            var infrastructureClassTransportId = _transportTouristsServiceClassTransportCache.Save(
                new CateTransportTouristsServiceInfrastructureClassTransportModel
                {
                    InfrastructureClassTransportId = model.InfrastructureClassTransportId,
                    EnterpriseId = model.EnterpriseId,
                    TypeTransport = model.TypeTransport,
                    TypeTransportName = model.TypeTransportName,
                    ClassTransportName = model.ClassTransportName,
                    TotalTransport = model.TotalTransport,
                    Reason = model.Reason,
                    SavedBy = User.Email
                });

            if (infrastructureClassTransportId == -9)
            {
                var errMessage =
                    CreateMessage(
                        $"{_enterpriseTransportTouristsInfrastructureClassTransportTitle} [{model.TypeTransportName} - {model.ClassTransportName}]",
                        EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            if (infrastructureClassTransportId == -7)
            {
                var errMessage = CreateMessage($"{_enterpriseTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            var response = CreateMessage(
                $"{_enterpriseTransportTouristsInfrastructureClassTransportTitle} [{model.TypeTransportName} - {model.ClassTransportName}]",
                EnumProcessType.Edit,
                infrastructureClassTransportId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteClassTransport(int id = 0)
        {
            var model = _transportTouristsServiceClassTransportCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTransportTouristsInfrastructureClassTransportTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"),
                $"<b>{_enterpriseTransportTouristsInfrastructureClassTransportTitle} [{model.TypeTransportName} - {model.ClassTransportName}]</b>");
            return PartialView("_DeleteClassTransport", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteClassTransport(CateTransportTouristsServiceInfrastructureClassTransportModel model)
        {
            model.SavedBy = User.Email;
            var deleted = _transportTouristsServiceClassTransportCache.Delete(model);

            var response = CreateMessage(
                $"{_enterpriseTransportTouristsInfrastructureClassTransportTitle} [{model.TypeTransportName} - {model.ClassTransportName}]",
                EnumProcessType.Delete, deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        #endregion

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