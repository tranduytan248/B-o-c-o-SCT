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
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Core.Conts;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Modules.CateModule.Models;
using FastMember;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;
using TSFramework.Core.Helpers;

namespace CenIT.ReportTourism.Modules.CateModule.Controllers
{
    public class MyEnterpriseController : AppController
    {
        // GET: MyEnterprise
        public ActionResult Index()
        {
            var searchModel = new MyEnterpiseSearchModel
            {
                ListEnterprises = _enterpriseCache.GetViaUser(User.UserName)
                    .Select(e => new ListItem(e.BusinessName, e.EnterpriseId.ToString())).ToList()
            };
            return View(searchModel);
        }

        #region Enterprise


        private readonly CateEnterpriseCache _enterpriseCache = new CateEnterpriseCache();
        private readonly CateWardCache _wardCache = new CateWardCache();
        private readonly CateProvinceCache _provinceCache = new CateProvinceCache();
        private readonly CateBusinessIndustryCache _industryCache = new CateBusinessIndustryCache();
        private readonly CateEnterpriseTypeCache _enterpriseTypeCache = new CateEnterpriseTypeCache();
        private readonly CateEconomicSectorCache _economicSectorCache = new CateEconomicSectorCache();
        private readonly CateEnterpriseStatusCache _enterpriseStatusCache = new CateEnterpriseStatusCache();

        private readonly string _enterpriseFolder = "Enterprise";
        private readonly string _enterpriseTitle = AppProcessor.Messagor.GetMessage("Enterprise_Title");

        private readonly string _moduleRefDocsPathFolder =
            ConfigurationManager.AppSettings["AttachmentFolderPath"] ?? @"/Contents/Modules/Cate/Attachments/";

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Info(int id = 0)
        {
            var model = _enterpriseCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            var provinceModel = _provinceCache.GetViaWard(model.WardId);
            model.ListProvinces = _provinceCache.GetAll()
                .OrderBy(d => d.ProvinceName)
                .Select(d => new ListItem(d.ProvinceName, d.ProvinceId.ToString()))
                .Distinct().ToList();

            model.ListWards = _wardCache.GetAll(provinceModel?.ProvinceId)
                .OrderBy(d => d.WardName)
                .Select(d => new ListItem(d.WardName, d.WardId.ToString()))
                .Distinct().ToList();

            model.ListBusinessIndustry = _industryCache.GetAll()
                .OrderBy(d => d.IndustryName)
                .Select(d => new ListItem(d.IndustryName, d.IndustryId.ToString())).ToList();

            model.ListEnterpriseType = _enterpriseTypeCache.GetAll()
                .OrderBy(d => d.Name)
                .Select(d => new ListItem(d.Name, d.EnterpriseTypeId.ToString())).ToList();

            model.ListEconomicSector = _economicSectorCache.GetAll()
                .OrderBy(d => d.Name)
                .Select(d => new ListItem(d.Name, d.EconomicSectorId.ToString())).ToList();

            model.ListEnterpriseStatus = _enterpriseStatusCache.GetAll()
                .OrderBy(d => d.Name)
                .Select(d => new ListItem(d.Name, d.EnterpriseStatusId.ToString())).ToList();

            model.ListTypeBusiness = Enum.GetValues(typeof(EnumTypeBusiness))
                .Cast<EnumTypeBusiness>()
                .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                    ((int)x).ToString()))
                .ToList();

            if (provinceModel == null) return PartialView("_Info", model);
            model.ProvinceId = provinceModel.ProvinceId;
            model.ProvinceName = provinceModel.ProvinceName;
            return PartialView("_Info", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Info(CateEnterpriseModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListProvinces = _provinceCache.GetAll()
                    .OrderBy(d => d.ProvinceName)
                    .Select(d => new ListItem(d.ProvinceName, d.ProvinceId.ToString()))
                    .Distinct().ToList();

                model.ListWards = _wardCache.GetAll(model.ProvinceId)
                    .OrderBy(d => d.WardName)
                    .Select(d => new ListItem(d.WardName, d.WardId.ToString()))
                    .Distinct().ToList();

                model.ListBusinessIndustry = _industryCache.GetAll()
                    .OrderBy(d => d.IndustryName)
                    .Select(d => new ListItem(d.IndustryName, d.IndustryId.ToString())).ToList();

                model.ListEnterpriseType = _enterpriseTypeCache.GetAll()
                    .OrderBy(d => d.Name)
                    .Select(d => new ListItem(d.Name, d.EnterpriseTypeId.ToString())).ToList();

                model.ListEconomicSector = _economicSectorCache.GetAll()
                    .OrderBy(d => d.Name)
                    .Select(d => new ListItem(d.Name, d.EconomicSectorId.ToString())).ToList();

                model.ListEnterpriseStatus = _enterpriseStatusCache.GetAll()
                    .OrderBy(d => d.Name)
                    .Select(d => new ListItem(d.Name, d.EnterpriseStatusId.ToString())).ToList();

                model.ListTypeBusiness = Enum.GetValues(typeof(EnumTypeBusiness))
                    .Cast<EnumTypeBusiness>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList();
                return PartialView("_Info", model);
            }

            var enterpriseId = _enterpriseCache.Save(new CateEnterpriseModel
            {
                EnterpriseId = model.EnterpriseId,
                OwnerEnterpriseName = model.OwnerEnterpriseName,
                BusinessName = model.BusinessName,
                TaxCode = model.TaxCode,
                BusinessAddress = model.BusinessAddress,
                StreetName = model.StreetName,
                WardId = model.WardId,
                WardName = model.WardName,

                //TypeBusiness = model.TypeBusiness,
                //TypeBusinessName = model.TypeBusinessName,
                
                LegalRepresentationName = model.LegalRepresentationName,
                LegalRepresentationPhone = model.LegalRepresentationPhone,
                LegalRepresentationEmail = model.LegalRepresentationEmail,
                Website = model.Website,
                Phone = model.Phone,
                Email = model.Email,
                Reason = model.Reason,

                TypeBusiness = model.ListTypeBusinessId != null && model.ListTypeBusinessId.Count > 0 ? string.Join(",", model.ListTypeBusinessId) : null,
                IndustryIds = model.ListIndustryId != null && model.ListIndustryId.Count > 0 ? string.Join(",", model.ListIndustryId) : null,
                EnterpriseTypeId = model.EnterpriseTypeId,
                EconomicSectorId = model.EconomicSectorId,
                EnterpriseStatusId = model.EnterpriseStatusId,

                SavedBy = User.Email
            });

            if (enterpriseId == -9)
            {
                var errMessage =
                    CreateMessage($"{_enterpriseTitle} [{model.TaxCode} - {model.BusinessName}]",
                        EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            if (enterpriseId > 0 && model.ListCertificateFiles != null && model.ListCertificateFiles.Count > 0)
            {
                var businessCetificatesId = SaveUploadFile(model.EnterpriseId, model.ListCertificateFiles,
                    _enterpriseFolder, model.Reason);
                if (businessCetificatesId == -7)
                    return Json(new
                    {
                        status = true,
                        message = CreateMessage($"{_enterpriseTitle}",
                            EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                    });
            }

            var response = CreateMessage($"{_enterpriseTitle} [{model.OwnerEnterpriseName} - {model.BusinessName}]",
                EnumProcessType.Edit,
                enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        //[AjaxOnly]
        //[HttpGet]
        //[ActionType(Type = EnumActionType.Edit)]
        //public ActionResult TypeBusinessInfo(int id = 0)
        //{
        //    var model = _enterpriseCache.GetById(id);
        //    if (model == null)
        //        return Json(new
        //        {
        //            status = true,
        //            message = CreateMessage($"{_enterpriseTitle}",
        //                EnumProcessType.DataNotExist, EnumMsgIcon.Error)
        //        });
        //    switch ((EnumTypeBusiness)model.TypeBusiness)
        //    {
        //        case EnumTypeBusiness.Accommodation:
        //            {
        //                return RedirectToAction("Info", "Accommodation",
        //                    new
        //                    {
        //                        enterpriseId = id
        //                    });
        //            }

        //        case EnumTypeBusiness.ServicesForTourists:
        //            {
        //                return RedirectToAction("Info", "ServicesForTourist",
        //                    new
        //                    {
        //                        enterpriseId = id
        //                    });
        //            }

        //        case EnumTypeBusiness.TouristAttraction:
        //            {
        //                return RedirectToAction("Info", "TouristAttraction",
        //                    new
        //                    {
        //                        enterpriseId = id
        //                    });
        //            }

        //        case EnumTypeBusiness.TransportTourists:
        //            {
        //                return RedirectToAction("Info", "TransportTourists",
        //                    new
        //                    {
        //                        enterpriseId = id
        //                    });
        //            }

        //        case EnumTypeBusiness.Traveling:
        //            {
        //                return RedirectToAction("Info", "Traveling",
        //                    new
        //                    {
        //                        enterpriseId = id
        //                    });
        //            }

        //        default:
        //            return RedirectToAction("Info", "Accommodation",
        //                new
        //                {
        //                    enterpriseId = id
        //                });
        //    }
        //}

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult WardViaProvince(int provinceId = 0)
        {
            int total;

            var lstWardViaProvinces = _wardCache.GetByProvinceId(provinceId, out total).OrderBy(d => d.ProvinceName).ToList();
            return Json(new { Wards = lstWardViaProvinces });
        }

        #endregion

        #region Extend Function

        //private int SaveUploadFile(CateEnterpriseModel model)
        private int SaveUploadFile(int enterpriseId, List<HttpPostedFileBase> lstFiles, string moduleFolderPath,
            string sReason)
        {
            if (lstFiles == null || lstFiles.Count == 0 || enterpriseId <= 0) return -7;

            var lstDocs = new List<CateDocModel>();

            var moduleRefDocsPathFolder =
                string.Concat(_moduleRefDocsPathFolder, moduleFolderPath, "/", enterpriseId.ToString());
            var moduleRefDocsAbsolutePathFolder = Server.MapPath(moduleRefDocsPathFolder);

            if (!Directory.Exists(moduleRefDocsAbsolutePathFolder))
                Directory.CreateDirectory(moduleRefDocsAbsolutePathFolder);
            lstFiles.ForEach(f =>
            {
                if (f == null) return;
                var unionResolutionDoc = new CateDocModel
                {
                    FileId = Guid.NewGuid(),
                    FilePath = moduleRefDocsPathFolder,
                    FileName = Path.GetFileNameWithoutExtension(f.FileName),
                    FileExt = Path.GetExtension(f.FileName),
                    ContentType = f.ContentType
                };

                f.SaveAs(string.Concat(moduleRefDocsAbsolutePathFolder, "/",
                    unionResolutionDoc.FileId.ToString().ToUpper(), unionResolutionDoc.FileExt));
                lstDocs.Add(unionResolutionDoc);
            });
            if (lstDocs.Count <= 0) return 0;
            var regulationId = _enterpriseCache.SaveDocs(new CateEnterpriseModel
            {
                EnterpriseId = enterpriseId,
                CertificateFiles = CreateDataRefDocs(lstDocs),
                Reason = sReason,
                SavedBy = User.Email
            });

            return regulationId;
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