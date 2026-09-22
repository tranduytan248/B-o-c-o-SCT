using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CenIT.ReportTourism.Caches.Cate;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Core.Conts;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Models;
using ExcelDataReader;
using FastMember;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;
using TSFramework.Core.Helpers;

namespace CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Controllers
{
    public class EnterpriseController : AppController
    {
        private readonly CateEnterpriseCache _enterpriseCache = new CateEnterpriseCache();
        private readonly CateProvinceCache _provinceCache = new CateProvinceCache();
        private readonly CateWardCache _wardCache = new CateWardCache();

        private readonly string _enterpriseFolder = "Enterprise";
        private readonly CateEnterpriseRegisterNotifyCache _enterpriseRegisterNotifyCache = new CateEnterpriseRegisterNotifyCache();
        private readonly string _enterpriseTitle = AppProcessor.Messagor.GetMessage("Enterprise_Label");

        private readonly string _moduleRefDocsPathFolder =
            ConfigurationManager.AppSettings["AttachmentFolderPath"] ?? "/Contents/Modules/Cate/Attachments/";

        private readonly string _pathTemplateFolder = "~/Contents/Modules/Cate/Templates/";

        // GET: Modules
        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult Index()
        {
            var searchModel = new EnterpriseSearchModel
            {
                ListWards = _wardCache.GetAll()
                    .OrderBy(d => d.WardName)
                    .Select(d => new ListItem(d.WardName, d.WardId.ToString())).ToList(),
                ListTypeBusiness = Enum.GetValues(typeof(EnumTypeBusiness))
                    .Cast<EnumTypeBusiness>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList()
            };
            return View(searchModel);
        }

        #region Enterprise

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get(EnterpriseSearchModel searchModel)
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
            searchModel.TypeBusinessIds =
                string.IsNullOrEmpty(searchModel.TypeBusinessIds) ? null : searchModel.TypeBusinessIds;
            searchModel.WardIds = string.IsNullOrEmpty(searchModel.WardIds) ? null : searchModel.WardIds;
            var data = _enterpriseCache.Get(User.UserName, searchModel.TypeBusinessIds, searchModel.WardIds,
                out var total, dataSearch);

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
            //var provinceModel = _provinceCache.GetAll().FirstOrDefault();
            var enterpriseModel = new CateEnterpriseModel
            {
                ListProvinces = _provinceCache.GetAll()
                    .OrderBy(d => d.ProvinceName)
                    .Select(d => new ListItem(d.ProvinceName, d.ProvinceId.ToString())).ToList(),
                ListWards = new List<ListItem>(),
                ListTypeBusiness = Enum.GetValues(typeof(EnumTypeBusiness))
                    .Cast<EnumTypeBusiness>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList(),
                Reason = "Thêm mới"
            };
            return PartialView("_Add", enterpriseModel);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult Add(CateEnterpriseModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListProvinces = _provinceCache.GetAll()
                    .OrderBy(d => d.ProvinceName)
                    .Select(d => new ListItem(d.ProvinceName, d.ProvinceId.ToString())).ToList();
                model.ListWards = new List<ListItem>();
                model.ListTypeBusiness = Enum.GetValues(typeof(EnumTypeBusiness))
                    .Cast<EnumTypeBusiness>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList();

                return PartialView("_Enterprise", model);
            }

            var enterpriseId = _enterpriseCache.Save(new CateEnterpriseModel
            {
                EnterpriseId = 0,
                OwnerEnterpriseName = model.OwnerEnterpriseName,
                BusinessName = model.BusinessName,
                TaxCode = model.TaxCode,
                BusinessAddress = model.BusinessAddress,
                StreetName = model.StreetName,
                WardId = model.WardId,
                WardName = model.WardName,
                TypeBusiness = model.TypeBusiness,
                TypeBusinessName = model.TypeBusinessName,
                LegalRepresentationName = model.LegalRepresentationName,
                LegalRepresentationPhone = model.LegalRepresentationPhone,
                LegalRepresentationEmail = model.LegalRepresentationEmail,
                Website = model.Website,
                Phone = model.Phone,
                Email = model.Email,
                Reason = model.Reason,
                SavedBy = User.Email
            });

            if (enterpriseId == -9)
            {
                var errMessage =
                    CreateMessage($"{AppProcessor.Messagor.GetMessage("Enterprise_Label_TaxCode")} [{model.TaxCode}]",
                        EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            if (enterpriseId > 0)
            {
                model.EnterpriseId = enterpriseId;
                var businessCetificatesId = SaveUploadFile(model);
                if (businessCetificatesId == -7)
                    return Json(new
                    {
                        status = true,
                        message = CreateMessage($"{_enterpriseTitle}",
                            EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                    });
            }

            var response = CreateMessage($"{_enterpriseTitle} [{model.OwnerEnterpriseName} - {model.BusinessName}]",
                EnumProcessType.Add,
                enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id = 0)
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
            model.ProvinceId = provinceModel?.ProvinceId;
            model.ProvinceName = provinceModel?.ProvinceName;
            model.ListProvinces = _provinceCache.GetAll()
                .OrderBy(d => d.ProvinceName)
                .Select(d => new ListItem(d.ProvinceName, d.ProvinceId.ToString())).ToList();
            model.ListWards = _wardCache.GetByProvinceId(model.ProvinceId).OrderBy(d => d.WardName)
                .Select(d => new ListItem(d.WardName, d.WardId.ToString())).ToList();

            if (model.ProvinceId != null)
            {
                model.ListWards = _wardCache.GetByProvinceId(model.ProvinceId, out int _)
                    .OrderBy(w => w.WardName)
                    .Select(d => new ListItem(d.WardName, d.WardId.ToString())).ToList();
            }

            model.ListTypeBusiness = Enum.GetValues(typeof(EnumTypeBusiness))
                .Cast<EnumTypeBusiness>()
                .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                    ((int)x).ToString()))
                .ToList();
            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(CateEnterpriseModel model)
        {
            if (!ModelState.IsValid)
            {
                var provinceModel = _provinceCache.GetAll().FirstOrDefault();
                model.ProvinceId = provinceModel?.ProvinceId;
                model.ProvinceName = provinceModel?.ProvinceName;

                model.ListProvinces = _provinceCache.GetAll()
                    .OrderBy(d => d.ProvinceName)
                    .Select(d => new ListItem(d.ProvinceName, d.ProvinceId.ToString())).ToList();
                model.ListWards = _wardCache.GetByProvinceId(model.ProvinceId).OrderBy(d => d.WardName)
                    .Select(d => new ListItem(d.WardName, d.WardId.ToString())).ToList();

                model.ListTypeBusiness = Enum.GetValues(typeof(EnumTypeBusiness))
                    .Cast<EnumTypeBusiness>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList();
                return PartialView("_Enterprise", model);
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
                TypeBusiness = model.TypeBusiness,
                TypeBusinessName = model.TypeBusinessName,
                LegalRepresentationName = model.LegalRepresentationName,
                LegalRepresentationPhone = model.LegalRepresentationPhone,
                LegalRepresentationEmail = model.LegalRepresentationEmail,
                Website = model.Website,
                Phone = model.Phone,
                Email = model.Email,
                Reason = model.Reason,
                SavedBy = User.Email
            });

            if (enterpriseId == -9)
            {
                var errMessage =
                    CreateMessage($"{AppProcessor.Messagor.GetMessage("Enterprise_Label_TaxCode")} [{model.TaxCode}]",
                        EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = true, message = errMessage });
            }

            if (enterpriseId > 0)
            {
                var businessCetificatesId = SaveUploadFile(model);
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

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id = 0)
        {
            var model = _enterpriseCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"),
                $"<b>{_enterpriseTitle} [{model.OwnerEnterpriseName} - {model.BusinessName}]</b>");
            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(CateEnterpriseModel model)
        {
            model.SavedBy = User.Email;
            var deleted = _enterpriseCache.Delete(model);

            var response = CreateMessage($"{_enterpriseTitle} [{model.OwnerEnterpriseName} - {model.BusinessName}]",
                EnumProcessType.Delete, deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult ChangeStatus(int id = 0)
        {
            var model = _enterpriseCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(
                AppProcessor.Messagor.GetMessage("Common_MessageConfirm_ChangeStatus"),
                $"<b>{_enterpriseTitle} [{model.OwnerEnterpriseName} - {model.BusinessName}]</b>");
            return PartialView("_ChangeStatus", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult ChangeStatus(CateEnterpriseModel model)
        {
            model.SavedBy = User.Email;
            var isSucess = _enterpriseCache.ChangeStatus(model);

            var response = CreateMessage($"{_enterpriseTitle} [{model.OwnerEnterpriseName} - {model.BusinessName}]",
                EnumProcessType.Edit, isSucess ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult WardViaProvince(int provinceId = 0)
        {
            var lstWardViaProvinces =
                _wardCache.GetByProvinceId(provinceId, out int _).OrderBy(d => d.WardName).ToList();
            return Json(new { Wards = lstWardViaProvinces });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Review(int id = 0)
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
            model.ProvinceId = provinceModel?.ProvinceId;
            model.ProvinceName = provinceModel?.ProvinceName;

            model.ListProvinces = _provinceCache.GetAll()
                .OrderBy(d => d.ProvinceName)
                .Select(d => new ListItem(d.ProvinceName, d.ProvinceId.ToString())).ToList();
            model.ListWards = _wardCache.GetByProvinceId(model.ProvinceId).OrderBy(d => d.WardName)
                .Select(d => new ListItem(d.WardName, d.WardId.ToString())).ToList();

            if (model.ProvinceId != null)
            {
                model.ListWards = _wardCache.GetByProvinceId(model.ProvinceId, out int _)
                    .OrderBy(w => w.WardName)
                    .Select(d => new ListItem(d.WardName, d.WardId.ToString())).ToList();
            }

            model.ListTypeBusiness = Enum.GetValues(typeof(EnumTypeBusiness))
                .Cast<EnumTypeBusiness>()
                .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                    ((int)x).ToString()))
                .ToList();

            //_enterpriseRegisterNotifyCache.Save(new CateEnterpriseRegisterNotifyModel
            //{
            //    BusinessName = model.BusinessName,
            //    CompletedBy = User.UserName,
            //    EnterpriseID = model.EnterpriseId,
            //    HasProcessed = true,
            //    OwnerEnterpriseName = model.OwnerEnterpriseName,
            //    SaveBy = User.UserName
            //});

            return PartialView("_Review", model);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Approve(int id = 0)
        {
            var model = _enterpriseCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            return PartialView("_Approve", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Approve(CateEnterpriseModel model)
        {
            var existEnterpriseModel = _enterpriseCache.GetById(model.EnterpriseId);
            if (existEnterpriseModel == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            var isSucess = _enterpriseRegisterNotifyCache.Save(new CateEnterpriseRegisterNotifyModel
            {
                BusinessName = model.BusinessName,
                CompletedBy = User.UserName,
                EnterpriseID = model.EnterpriseId,
                HasProcessed = true,
                IsConfirm = model.IsConfirm,
                OwnerEnterpriseName = model.OwnerEnterpriseName,
                SaveBy = User.UserName
            }) > 0;

            var response = CreateMessage($"{_enterpriseTitle} [{model.OwnerEnterpriseName} - {model.BusinessName}]",
                EnumProcessType.Edit, isSucess ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        #endregion

        #region Import Data

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult Import()
        {
            var model = new EnterpriseImportModel
            {
                ListTypeBusiness = Enum.GetValues(typeof(EnumTypeBusiness))
                    .Cast<EnumTypeBusiness>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList()
            };

            return PartialView("_Import", model);
        }

        [HttpPost]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Import(EnterpriseImportModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListTypeBusiness = Enum.GetValues(typeof(EnumTypeBusiness))
                    .Cast<EnumTypeBusiness>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList();
                return PartialView("_ImportView", model);
            }

            bool isSuccessImport;

            var dataImport = ProcessDataImport(model, out isSuccessImport);
            dataImport.Columns.RemoveAt(0);
            if (!isSuccessImport)
                return Json(new
                {
                    status = false,
                    message = CreateMessage(AppProcessor.Messagor.GetMessage("ImportData_Message_Failed"),
                        EnumProcessType.NonFormat, EnumMsgIcon.Error)
                });

            var importResult =
                _enterpriseCache.Import(model.TypeBusiness, model.TypeBusinessName, dataImport, User.UserName);

            if (importResult >= 1)
                return Json(new
                {
                    status = true,
                    message = CreateMessage(AppProcessor.Messagor.GetMessage("ImportData_Message_Successful"),
                        EnumProcessType.NonFormat, EnumMsgIcon.Success)
                });
            return Json(new
            {
                status = false,
                message = CreateMessage(AppProcessor.Messagor.GetMessage("ImportData_Message_Failed"),
                    EnumProcessType.NonFormat, EnumMsgIcon.Error)
            });
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult DownloadTemplate()
        {
            #region Download Template

            var templateName = "Mẫu import Doanh Nghiệp.xlsx";
            var fullPathAttachDocsFolder = Server.MapPath(_pathTemplateFolder);

            var attachDocPath = Path.Combine(fullPathAttachDocsFolder, templateName);
            var arrBytes = System.IO.File.ReadAllBytes(attachDocPath);

            #endregion

            return File(arrBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                templateName);
        }

        #endregion

        #region Extend Function

        private DataTable ProcessDataImport(EnterpriseImportModel model, out bool isSucess)
        {
            isSucess = true;

            #region Init Datatable

            var dataImport = new DataTable();

            dataImport.Columns.Add("Index");
            dataImport.Columns.Add("TaxCode");
            dataImport.Columns.Add("OwnerEnterpriseName");
            dataImport.Columns.Add("BusinessName");
            dataImport.Columns.Add("BusinessAddress");
            dataImport.Columns.Add("StreetName");
            dataImport.Columns.Add("WardName");
            dataImport.Columns.Add("DistrictName");
            //dataImport.Columns.Add("ProvinceName");
            dataImport.Columns.Add("Phone");
            dataImport.Columns.Add("Website");
            dataImport.Columns.Add("Email");
            dataImport.Columns.Add("LegalRepresentationName");
            dataImport.Columns.Add("LegalRepresentationPhone");
            dataImport.Columns.Add("LegalRepresentationEmail");

            #endregion

            try
            {
                var fileExtension = Path.GetExtension(model.FileImportData.FileName);

                if (fileExtension != ".xls" && fileExtension != ".xlsx") return null;
                var excelReader = fileExtension == ".xls"
                    ? ExcelReaderFactory.CreateBinaryReader(model.FileImportData.InputStream)
                    : ExcelReaderFactory.CreateOpenXmlReader(model.FileImportData.InputStream);

                var ds = excelReader.AsDataSet();

                var dt = ds.Tables[0];
                dt.Rows.RemoveAt(0); // Title Column
                dt.Rows.RemoveAt(0); // Title Column

                foreach (DataRow row in dt.Rows)
                    dataImport.Rows.Add(row.ItemArray.Length > dataImport.Columns.Count
                        ? row.ItemArray.ToList().Where((item, idx) => idx < dataImport.Columns.Count)
                            .Select(c => string.IsNullOrEmpty(c.ToString()) ? null : c).ToArray()
                        : row.ItemArray.Select(c => string.IsNullOrEmpty(c.ToString()) ? null : c).ToArray());

                return dataImport;
            }
            catch (Exception e)
            {
                AppProcessor.Logger.Error(e);
                isSucess = false;
                return dataImport;
            }
        }

        private int SaveUploadFile(CateEnterpriseModel model)
        {
            if (model.ListCertificateFiles == null || model.ListCertificateFiles.Count == 0 ||
                model.EnterpriseId <= 0) return -7;

            var lstDocs = new List<CateDocModel>();

            var moduleRefDocsPathFolder = string.Concat(_moduleRefDocsPathFolder, _enterpriseFolder, "/",
                model.EnterpriseId.ToString());
            var moduleRefDocsAbsolutePathFolder = Server.MapPath(moduleRefDocsPathFolder);

            if (!Directory.Exists(moduleRefDocsAbsolutePathFolder))
                Directory.CreateDirectory(moduleRefDocsAbsolutePathFolder);
            model.ListCertificateFiles.ForEach(f =>
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
                EnterpriseId = model.EnterpriseId,
                CertificateFiles = CreateDataRefDocs(lstDocs),
                Reason = model.Reason,
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