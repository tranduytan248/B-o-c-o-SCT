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
using CenIT.ReportTourism.Caches.Cate.ServicesForTourist;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Core.Conts;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Cate.ServicesForTourist;
using FastMember;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Controllers.TypeBusiness
{
    public class TypeServicesForTouristController : AppController
    {
        private readonly CateEnterpriseCache _enterpriseCache;

        private readonly string _enterpriseServicesForTouristCertificateTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_ServicesForTourists_Certificate_Title");

        private readonly string _enterpriseServicesForTouristHRInformationTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_ServicesForTourists_HRInformation_Title");

        private readonly string _enterpriseServicesForTouristInfrastructureTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_ServicesForTourists_Infrastructure_Title");

        private readonly string _enterpriseServicesForTouristTitle =
            AppProcessor.Messagor.GetMessage("Enterprise_ServicesForTourists_Title");

        private readonly string _enterpriseTitle = AppProcessor.Messagor.GetMessage("Enterprise_Title");

        private readonly string _moduleRefDocsPathFolder =
            ConfigurationManager.AppSettings["AttachmentFolderPath"] ?? @"/Contents/Modules/Cate/Attachments/";

        private readonly CatePublicCateCache _publicCateCache;
        private readonly CateServicesForTouristCache _touristAttractionCache;

        private readonly string _touristAttractionCertificateFolder = "ServicesForTouristCertificates";

        public TypeServicesForTouristController()
        {
            _publicCateCache = new CatePublicCateCache();
            _enterpriseCache = new CateEnterpriseCache();
            _touristAttractionCache = new CateServicesForTouristCache();
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
            var touristAttractionModel = new CateServicesForTouristModel
            {
                EnterpriseId = enterpriseId
            };
            return PartialView("_Info", touristAttractionModel);
        }

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

        #endregion

        #region ServicesForTourist Info

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult ServicesForTouristInfo(int enterpriseId)
        {
            var touristAttractionInfoModel = _touristAttractionCache.GetInfoViaEnterprise(enterpriseId);
            touristAttractionInfoModel = touristAttractionInfoModel ?? new CateServicesForTouristModel();
            touristAttractionInfoModel.EnterpriseId = enterpriseId;
            touristAttractionInfoModel.ListTypeServicesForTouristIds =
                string.IsNullOrEmpty(touristAttractionInfoModel.TypeServicesForTourists)
                    ? new List<int>()
                    : touristAttractionInfoModel.TypeServicesForTourists.Split(',').Select(int.Parse).ToList();
            touristAttractionInfoModel.ListTypeServicesForTourists = _publicCateCache
                .GetAll((int)EnumTypePublicCate.TypeServicesForTourist)
                .Select(c => new ListItem(c.CateName, c.CateId.ToString())).ToList();
            return PartialView("_TabServicesForTouristInfo", touristAttractionInfoModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult SaveInfo(CateServicesForTouristModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListTypeServicesForTourists = _publicCateCache
                    .GetAll((int)EnumTypePublicCate.TypeServicesForTourist)
                    .Select(c => new ListItem(c.CateName, c.CateId.ToString())).ToList();
                return PartialView("_TabServicesForTouristInfo", model);
            }

            model.TypeServicesForTourists = string.Join(",", model.ListTypeServicesForTouristIds);
            var enterpriseId =
                _touristAttractionCache.SaveInfo(new CateServicesForTouristModel
                {
                    ServicesForTouristId = model.ServicesForTouristId,
                    EnterpriseId = model.EnterpriseId,
                    TypeServicesForTourists = model.TypeServicesForTourists,
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
                $"{_enterpriseServicesForTouristTitle} - {_enterpriseServicesForTouristInfrastructureTitle}",
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
            certificateModel = certificateModel ?? new CateServicesForTouristCertificateModel();
            certificateModel.EnterpriseId = enterpriseId;
            return PartialView("_TabCertificate", certificateModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult SaveCertificate(CateServicesForTouristCertificateModel model)
        {
            if (!ModelState.IsValid)
            {
                var certificateModel = _touristAttractionCache.GetCertificateViaEnterprise(model.EnterpriseId);
                certificateModel = certificateModel ?? new CateServicesForTouristCertificateModel();
                certificateModel.EnterpriseId = model.EnterpriseId;
                return PartialView("_TabCertificate", certificateModel);
            }

            Guid fileId;

            var dataLicenseCertificate =
                CreateDataRefDocs(model.EnterpriseId, model.FileLicenseFile, _touristAttractionCertificateFolder,
                    out fileId);

            model.LicenseFile = fileId == Guid.Empty ? model.LicenseFile : fileId;

            var enterpriseId =
                _touristAttractionCache.SaveCertificates(new CateServicesForTouristCertificateModel
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
                $"{_enterpriseServicesForTouristTitle} - {_enterpriseServicesForTouristCertificateTitle}",
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
            hrInformationModel = hrInformationModel ?? new CateServicesForTouristHRInformationModel();
            hrInformationModel.EnterpriseId = enterpriseId;
            return PartialView("_TabHRInformation", hrInformationModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult SaveHRInformation(CateServicesForTouristHRInformationModel model)
        {
            if (!ModelState.IsValid) return PartialView("_TabHRInformation", model);

            var enterpriseId = _touristAttractionCache.SaveHRInformations(new CateServicesForTouristHRInformationModel
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
                $"{_enterpriseServicesForTouristTitle} - {_enterpriseServicesForTouristHRInformationTitle}",
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
            infrastructureModel = infrastructureModel ?? new CateServicesForTouristInfrastructureModel();
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
        public ActionResult SaveInfrastructure(CateServicesForTouristInfrastructureModel model)
        {
            if (!ModelState.IsValid) return PartialView("_TabInfrastructure", model);

            var enterpriseId =
                _touristAttractionCache.SaveInfrastructures(new CateServicesForTouristInfrastructureModel
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
                $"{_enterpriseServicesForTouristTitle} - {_enterpriseServicesForTouristInfrastructureTitle}",
                EnumProcessType.Edit,
                enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}