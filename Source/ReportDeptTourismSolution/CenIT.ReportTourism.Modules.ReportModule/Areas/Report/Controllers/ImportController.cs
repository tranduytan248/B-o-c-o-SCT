using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CenIT.Libs.VNPTSmartCA.Models.Responses;
using CenIT.Libs.VNPTSmartCA.Providers;
using CenIT.Libs.VNPTSmartCA.Services;
using CenIT.ReportTourism.Caches.Cate;
using CenIT.ReportTourism.Caches.Report;
using CenIT.ReportTourism.Caches.Sys;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Core.Conts;
using CenIT.ReportTourism.Core.Helpers;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Report;
using CenIT.ReportTourism.Models.Sys;
using CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Models;
using CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Models;
using DocumentFormat.OpenXml.Packaging;
using ExcelDataReader;
using Microsoft.Reporting.WebForms;
using Spire.Xls;
using Spire.Xls.Collections;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;
using TSFramework.Core.Helpers;
using TSFramework.Core.Providers;
using VnptHashSignatures.Interface;

namespace CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Controllers
{
    public class ImportController : AppController
    {
        private readonly SysConfigsCache _configCache = new SysConfigsCache();
        private readonly CateEnterpriseCache _enterpriseCache = new CateEnterpriseCache();

        private readonly string _enterpriseTitle = AppProcessor.Messagor.GetMessage("Enterprise_Title");
        private readonly ReportDataImportCache _importCache = new ReportDataImportCache();
        private readonly string _importTile = AppProcessor.Messagor.GetMessage("ReportDataImport_Title");
        private readonly CateBusinessProductCache _businessProductCache;
        private readonly CateNationalCache _nationalCache = new CateNationalCache();

        private readonly string _templateImportPathFolder =
            ConfigurationManager.AppSettings["Modules_Report_TemplateImportFolderPath"] ??
            "/Contents/Modules/Report/Templates/";

        #region Mapping

        private readonly Dictionary<int, string> _mappingReportTypeBiz = new Dictionary<int, string>{
            //{
            //    (int)EnumTypeBusiness.Accommodation,
            //    AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(EnumTypeBusiness.Accommodation))
            //},
            //{
            //    (int)EnumTypeBusiness.Traveling,
            //    AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(EnumTypeBusiness.Traveling))
            //},
            //{
            //    (int)EnumTypeBusiness.ServicesForTourists,
            //    AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(EnumTypeBusiness.TouristAttraction))
            //},
            //{
            //    (int)EnumTypeBusiness.TransportTourists,
            //    AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(EnumTypeBusiness.Traveling))
            //},
            {
                (int)EnumTypeBusiness.Manufacturing,
                AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(EnumTypeBusiness.Manufacturing))
            },
            {
                (int)EnumTypeBusiness.Trading,
                AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(EnumTypeBusiness.Trading))
            },
            {
                (int)EnumTypeBusiness.ImportExport,
                AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(EnumTypeBusiness.ImportExport))
            }
        };

        #endregion

        public ImportController()
        {
            _importCache = new ReportDataImportCache();
            _enterpriseCache = new CateEnterpriseCache();
            _configCache = new SysConfigsCache();
            _nationalCache = new CateNationalCache();
            _businessProductCache = new CateBusinessProductCache();
        }
        // GET: Cate/ReportDataImport
        public ActionResult Index()
        {
            var lstEnterpisePermits = _enterpriseCache.GetViaUser(User.UserName);
            var lstReportsViaUsers = _importCache.GetForUserOnMonth(User.UserName, DateTime.Now);
            var lstEnterpriseOther = (lstEnterpisePermits ?? new List<CateEnterpriseModel>())
                .Where(e => lstReportsViaUsers == null || !lstReportsViaUsers.Exists(r => r.EnterpriseId == e.EnterpriseId)).ToList();

            var searchModel = new ReportDataImportSearchModel
            {
                ListEnterprises = (lstEnterpisePermits ?? new List<CateEnterpriseModel>())
                    .Select(e => new ListItem(e.BusinessName, e.EnterpriseId.ToString())).ToList(),
                DayDeadlineSendReport =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Report")?.ConfigValue ?? "0"),
                DayDeadlineSendReportLate =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Late_Report")?.ConfigValue ?? "0"),
                ExistEnterpriseSubmitReportYet = (lstEnterpisePermits != null && lstEnterpisePermits.Count > 0 ? lstEnterpriseOther.Count > 0 : true),
                ListTypeBusiness = Enum.GetValues(typeof(EnumTypeBusiness))
                    .Cast<EnumTypeBusiness>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList(),
                EnableSignDigitalDoc =
                    (_configCache.GetViaKey("Enable_SignDigital_Doc")?.ConfigValue ?? "0") != "0"
            };
            return View(searchModel);
        }

        [HttpGet]
        [AllowAnyPermission]
        public ActionResult SearchEnterprisesSelect2(string q = null, string typeBusiness = null, int page = 1)
        {
            try
            {
                var lstEnterpisePermits = _enterpriseCache.GetViaUser(User.UserName);
                if (lstEnterpisePermits != null && lstEnterpisePermits.Count > 0)
                {
                    var filtered = lstEnterpisePermits.Where(e =>
                        (string.IsNullOrEmpty(typeBusiness) || e.TypeBusiness == typeBusiness) &&
                        (string.IsNullOrEmpty(q) ||
                         (!string.IsNullOrEmpty(e.BusinessName) && e.BusinessName.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) ||
                         (!string.IsNullOrEmpty(e.TaxCode) && e.TaxCode.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0))
                    ).ToList();

                    var results = filtered.Select(e => new
                    {
                        id = e.EnterpriseId,
                        text = string.IsNullOrEmpty(e.TaxCode) ? e.BusinessName : string.Format("{0} - {1}", e.BusinessName, e.TaxCode),
                        typeBusiness = e.TypeBusiness
                    }).ToList();

                    return Json(new
                    {
                        results = results,
                        pagination = new { more = false }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int pageSize = 20;
                    int pageIndex = page > 0 ? page - 1 : 0;
                    var keyword = string.IsNullOrWhiteSpace(q) ? null : q.Trim();
                    var typeBiz = string.IsNullOrWhiteSpace(typeBusiness) ? null : typeBusiness.Trim();

                    List<CateEnterpriseModel> list = null;
                    try
                    {
                        list = AppProcessor.ProcedureProvider.ExecuteTypedList<CateEnterpriseModel>(
                            "Cate_Enterprises_SearchSelect2", "SysProvider",
                            keyword, typeBiz, pageIndex, pageSize);
                    }
                    catch (Exception exSp)
                    {
                        AppProcessor.Logger.Message("ExecuteTypedList error in SearchEnterprisesSelect2: " + exSp.Message);
                    }


                    int total = 0;
                    if (list != null && list.Count > 0)
                    {
                        total = list.First().TotalRow.GetValueOrDefault(0);
                    }

                    var results = (list ?? new List<CateEnterpriseModel>()).Select(e => new
                    {
                        id = e.EnterpriseId,
                        text = string.IsNullOrEmpty(e.TaxCode) ? e.BusinessName : string.Format("{0} - {1}", e.BusinessName, e.TaxCode),
                        typeBusiness = e.TypeBusiness
                    }).ToList();

                    bool more = (pageIndex + 1) * pageSize < total;
                    return Json(new
                    {
                        results = results,
                        pagination = new { more = more }
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Message("General error in SearchEnterprisesSelect2: " + ex.Message);
                return Json(new
                {
                    results = new object[0],
                    pagination = new { more = false }
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get(ReportDataImportSearchModel searchModel)
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
            var data = _importCache.Get(User.UserName, searchModel.EnterpriseIds, searchModel.FromMonth,
                searchModel.ToMonth, searchModel.TypeBusinessIds, out total, dataSearch);
            if (data != null)
            {
                foreach (var report in data)
                    report.CanDelete = report.CanDelete && !IsReportLocked(report.ForMonth);
            }
            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        #region Download

        [HttpGet]
        [AjaxOnly]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult DownloadSignedDoc(int enterpriseId, DateTime onMonth)
        {
            var enterpriseModel = _enterpriseCache.GetById(enterpriseId);
            if (enterpriseModel == null)
                return Json(new
                {
                    status = false,
                    errorCode = 1,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            var dataImports = _importCache.GetViaEnterpriseOnMonth(enterpriseId, onMonth);
            if (dataImports == null || dataImports.Count <= 0)
                return Json(new
                {
                    status = false,
                    errorCode = 1,
                    message = CreateMessage($"{_importTile}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            var fullSignedPath = HostingEnvironment.MapPath("/" + _signedFilesPathFolder);
            if (!Directory.Exists(fullSignedPath))
                return Json(new
                {
                    status = false,
                    errorCode = 1,
                    message = CreateMessage($"{_importTile}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            var subFolderSignedPath = Path.Combine(fullSignedPath, $"{onMonth.Year}", $"{onMonth.Month}",
                $"{enterpriseModel.EnterpriseId}");
            if (!Directory.Exists(subFolderSignedPath) || !Directory.GetFiles(subFolderSignedPath).ToList().Any())
                return Json(new
                {
                    status = false,
                    errorCode = 1,
                    message = CreateMessage($"{_importTile}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            return Json(new
            {
                status = true,
                downloadPath = Url.Action("DownloadDoc", new { year = onMonth.Year, month = onMonth.Month, enterpriseId })
            });
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public async Task<ActionResult> DownloadDoc(int year, int month, int enterpriseId)
        {
            var enterpriseModel = _enterpriseCache.GetById(enterpriseId);
            var fullSignedPath = HostingEnvironment.MapPath("/" + _signedFilesPathFolder);
            var subFolderSignedPath = Path.Combine(fullSignedPath, $"{year}", $"{month}", $"{enterpriseId}");
            var signedFiles = Directory.GetFiles(subFolderSignedPath);
            var signedFile = signedFiles[0];
            var fileInfo = new FileInfo(signedFile);

            var fileData = System.IO.File.ReadAllBytes(signedFile);
            return await Task.Run(() => File(fileData, MimeMapping.GetMimeMapping(fileInfo.Name),
                $"{enterpriseModel.BusinessName}-{fileInfo.Name}"));
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public async Task<ActionResult> DownloadTemplate(string templateName = "TemplateImport_LuHanh.xlsx")
        {
            var lstEnterpisePermits = _enterpriseCache.GetViaUser(User.UserName);
            var lstReportsViaUsers = _importCache.GetForUserOnMonth(User.UserName, DateTime.Now);
            var lstEnterpriseOther = lstEnterpisePermits
                .Where(e => !lstReportsViaUsers.Exists(r => r.EnterpriseId == e.EnterpriseId)).ToList();

            var searchModel = new ReportDataImportSearchModel
            {
                ListEnterprises = lstEnterpisePermits
                    .Select(e => new ListItem(e.BusinessName, e.EnterpriseId.ToString())).ToList(),
                DayDeadlineSendReport =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Report")?.ConfigValue ?? "0"),
                DayDeadlineSendReportLate =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Late_Report")?.ConfigValue ?? "0"),
                ExistEnterpriseSubmitReportYet = lstEnterpriseOther.Count > 0,
                ListTypeBusiness = Enum.GetValues(typeof(EnumTypeBusiness))
                    .Cast<EnumTypeBusiness>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList(),
                EnableSignDigitalDoc =
                    (_configCache.GetViaKey("Enable_SignDigital_Doc")?.ConfigValue ?? "0") != "0"
            };

            #region Download File

            var fullPathTemplateImportFolder = HostingEnvironment.MapPath("/" + _templateImportPathFolder);

            var attachDocPath = Path.Combine(fullPathTemplateImportFolder, templateName);
            if (!System.IO.File.Exists(attachDocPath)) return View("Index", searchModel);
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename= " + $"{templateName}");
            using (var fileStream = new FileStream(attachDocPath, FileMode.Open))
            {
                using (var myMemoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(myMemoryStream);
                    myMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }

            #endregion

            return View("Index", searchModel);
        }

        #endregion

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult ViewAction()
        {
            var lstEnterpisePermits = _enterpriseCache.GetViaUser(User.UserName);
            var lstReportsViaUsers = _importCache.GetForUserOnMonth(User.UserName, DateTime.Now);
            var lstEnterpriseOther = (lstEnterpisePermits ?? new List<CateEnterpriseModel>())
                .Where(e => lstReportsViaUsers == null || !lstReportsViaUsers.Exists(r => r.EnterpriseId == e.EnterpriseId)).ToList();

            var searchModel = new ReportDataImportSearchModel
            {
                ListEnterprises = (lstEnterpisePermits ?? new List<CateEnterpriseModel>())
                    .Select(e => new ListItem(e.BusinessName, e.EnterpriseId.ToString())).ToList(),
                DayDeadlineSendReport =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Report")?.ConfigValue ?? "0"),
                DayDeadlineSendReportLate =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Late_Report")?.ConfigValue ?? "0"),
                ExistEnterpriseSubmitReportYet = (lstEnterpisePermits != null && lstEnterpisePermits.Count > 0 ? lstEnterpriseOther.Count > 0 : true),
                EnableSignDigitalDoc =
                    (_configCache.GetViaKey("Enable_SignDigital_Doc")?.ConfigValue ?? "0") != "0"
            };
            return PartialView("_ActionView", searchModel);
        }

        #region Import

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult ImportData()
        {
            var lstEnterpisePermits = _enterpriseCache.GetViaUser(User.UserName);
            var lstReportsViaUsers = _importCache.GetForUserOnMonth(User.UserName, DateTime.Now);
            var lstEnterpriseOther = lstEnterpisePermits
                .Where(e => !lstReportsViaUsers.Exists(r => r.EnterpriseId == e.EnterpriseId)).ToList();

            var model = new ReportDataImportModel
            {
                ListEnterprises = lstEnterpriseOther.OrderBy(e => e.BusinessName)
                    .Select(e => new ListItem(e.BusinessName, e.EnterpriseId.ToString())).ToList(),
                EnterpriseId = lstEnterpriseOther.Count == 1 ? lstEnterpriseOther[0].EnterpriseId : (int?)null,
                EnterpriseName = lstEnterpriseOther.Count == 1 ? lstEnterpriseOther[0].BusinessName : null,
                DayDeadlineSendReport =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Report")?.ConfigValue ?? "0"),
                DayDeadlineSendReportLate =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Late_Report")?.ConfigValue ?? "0"),
                AccessToken = (string)Session[$"VNPT-SmartCA-{User?.UserName}-AccessToken"],
                EnableSignDigitalDoc =
                    (_configCache.GetViaKey("Enable_SignDigital_Doc")?.ConfigValue ?? "0") != "0"
            };
            return PartialView("_ImportData", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult UploadData(ReportDataImportModel model)
        {
            StringBuilder logActions = new StringBuilder();
            logActions.AppendLine("=========================================");
            if (IsReportLocked(model.ForMonth))
            {
                return Json(new
                {
                    status = false,
                    errorCode = 1,
                    message = CreateMessage(GetReportLockedMessage(model.ForMonth), EnumProcessType.NonFormat,
                        EnumMsgIcon.Error)
                });
            }
            logActions.AppendLine(
                $" - Loại ký số báo cáo: {model.TypeSignature} : {(model.TypeSignature == 0 ? "Tải file đã ký sẵn" : (model.TypeSignature == 1 ? "Ký số trực tiếp bằng Smart CA" : "Ký số Token CA"))}");

            if (model.TypeSignature == 2)
            {
                #region Check Valid Model And Data

                ModelState.Remove("FileImport");
                if (!ModelState.IsValid)
                {
                    var lstEnterpisePermits = _enterpriseCache.GetViaUser(User.UserName);
                    var lstReportsViaUsers = _importCache.GetForUserOnMonth(User.UserName, DateTime.Now);
                    var lstEnterpriseOther = lstEnterpisePermits
                        .Where(e => !lstReportsViaUsers.Exists(r => r.EnterpriseId == e.EnterpriseId)).ToList();

                    model.ListEnterprises = lstEnterpriseOther.OrderBy(e => e.BusinessName)
                        .Select(e => new ListItem(e.BusinessName, e.EnterpriseId.ToString())).ToList();
                    model.DayDeadlineSendReport =
                        int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Report")?.ConfigValue ?? "0");
                    model.DayDeadlineSendReportLate =
                        int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Late_Report")?.ConfigValue ?? "0");
                    model.EnableSignDigitalDoc =
                        (_configCache.GetViaKey("Enable_SignDigital_Doc")?.ConfigValue ?? "0") != "0";
                    model.AccessToken = (string)Session[$"VNPT-SmartCA-{User?.UserName}-AccessToken"];

                    return PartialView("_ImportView", model);
                }

                var enterpriseModel = _enterpriseCache.GetById(model.EnterpriseId);
                if (enterpriseModel == null)
                {
                    logActions.AppendLine(" - Lỗi: Doanh nghiệp không tồn tại");
                    AppProcessor.Logger.Message(logActions.ToString());
                    return Json(new
                    {
                        status = false,
                        message = CreateMessage($"{_enterpriseTitle}",
                            EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                    });
                }

                #endregion

                #region Create FilePame And Folder Path

                var dataFileImport = Convert.FromBase64String(model.FileDataBase64);
                var streamDatas = new MemoryStream(dataFileImport);

                var fullSignedPath = HostingEnvironment.MapPath("/" + _signedFilesPathFolder);
                var subFolderSignedPath = Path.Combine(fullSignedPath,
                    $"{model.ForMonth.GetValueOrDefault(DateTime.Now).Year}",
                    $"{model.ForMonth.GetValueOrDefault(DateTime.Now).Month}", $"{enterpriseModel.EnterpriseId}");

                if (!Directory.Exists(subFolderSignedPath) && !string.IsNullOrEmpty(subFolderSignedPath))
                    Directory.CreateDirectory(subFolderSignedPath);
                var fileExt = model.FileExt;
                var fileNameSigned =
                    $"Report_{enterpriseModel.EnterpriseId}_{(model.EnableSignDigitalDoc ? "signed" : "")}.{fileExt}";
                var fileSignedFullPath = Path.Combine(subFolderSignedPath, fileNameSigned);

                var sFileName = model.FileName;

                #endregion

                #region Check Data Import

                bool isSuccessImport;
                var dataImports = ReadDataImports(streamDatas, fileExt, out isSuccessImport);
                if (!isSuccessImport)
                {
                    logActions.AppendLine(" - Đọc nội dung báo cáo lỗi");
                    AppProcessor.Logger.Message(logActions.ToString());
                    return Json(new
                    {
                        status = false,
                        message = CreateMessage(AppProcessor.Messagor.GetMessage("ImportData_Message_Fail"),
                            EnumProcessType.NonFormat, EnumMsgIcon.Error)
                    });
                }

                var lstDataImports = ModelProvider.CreateListFromTable<ReportDataImportModel>(dataImports);
                var isWrongData = false;
                dataImports.Columns.Remove("RowIndex");

                var dataChecked = CheckDataImport(dataImports, model.ForMonth, model.EnterpriseId);
                if (dataChecked.Count > 0)
                {
                    logActions.AppendLine(" - Kiểm tra dung báo cáo: Nội dung báo cáo không đúng");
                    AppProcessor.Logger.Message(logActions.ToString());

                    //lstDataImports.ForEach(t =>
                    //{
                    //    if (dataChecked.FirstOrDefault(d => d.Code == t.Code) ==
                    //        null) return;
                    //    t.IsWrong = true;
                    //    isWrongData = true;
                    //});

                    var viewModel = new ReportDataImportViewModel
                    {
                        EnterpriseId = model.EnterpriseId,
                        ForMonth = model.ForMonth,
                        ReponseMessage =
                            CreateMessage($"{_importTile} - [{model.EnterpriseName} tháng {model.ForMonth:MM/yyyy}]",
                                EnumProcessType.Add, EnumMsgIcon.Error)
                    };
                    Session[$"DataImports-{User.UserName}-{model.EnterpriseId}-{model.ForMonth:MM/yyyy}"] =
                        lstDataImports;

                    ViewBag.Title = $"{_importTile} - <b>[{model.EnterpriseName} tháng {model.ForMonth:MM/yyyy}]</b>";
                    return PartialView("_ReviewData", viewModel);
                }

                #endregion

                logActions.AppendLine(" - Thực hiện lưu file báo cáo vào hệ thống");
                System.IO.File.WriteAllBytes(fileSignedFullPath, dataFileImport);

                #region Save data import

                var enterpriseId = _importCache.Import(new ReportDataImportModel
                {
                    EnterpriseId = model.EnterpriseId,
                    ForMonth = model.ForMonth,
                    TypeReport = model.TypeReport,
                    TypeReportName = model.TypeReportName,
                    //TypeReport = enterpriseModel.TypeBusiness,
                    //TypeReportName =
                    //    AppProcessor.Messagor.GetMessage(
                    //        EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)),
                    DataImport = dataImports,
                    ReportFile = fileNameSigned,
                    CreatedBy = User.UserName,
                    Reason = model.Reason
                });
                if (enterpriseId == -7)
                {
                    logActions.AppendLine(" - Lưu nội dung báo cáo vào hệ thống thất bại: Doanh nghiệp không tồn tại");
                    AppProcessor.Logger.Message(logActions.ToString());

                    return Json(new
                    {
                        status = false,
                        message = CreateMessage($"{_enterpriseTitle}",
                            EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                    });
                }

                if (enterpriseId == -99)
                {
                    logActions.AppendLine(
                        $" - Lưu nội dung báo cáo vào hệ thống thất bại: {AppProcessor.Messagor.GetMessage("ImportData_Message_Expire_Submit_Report")}");
                    AppProcessor.Logger.Message(logActions.ToString());

                    return Json(new
                    {
                        status = false,
                        message = CreateMessage(
                            AppProcessor.Messagor.GetMessage("ImportData_Message_Expire_Submit_Report"),
                            EnumProcessType.NonFormat, EnumMsgIcon.Error)
                    });
                }

                logActions.AppendLine(" - Lưu nội dung báo cáo vào hệ thống thành công");
                AppProcessor.Logger.Message(logActions.ToString());

                var response = CreateMessage($"{_importTile} - [{model.EnterpriseName} tháng {model.ForMonth:MM/yyyy}]",
                    EnumProcessType.Add,
                    enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
                return Json(new { status = true, message = response });

                #endregion
            }
            else
            {
                #region Check Valid Model & Data

                if (!ModelState.IsValid)
                {
                    var lstEnterpisePermits = _enterpriseCache.GetViaUser(User.UserName);
                    var lstReportsViaUsers = _importCache.GetForUserOnMonth(User.UserName, DateTime.Now);
                    var lstEnterpriseOther = lstEnterpisePermits
                        .Where(e => !lstReportsViaUsers.Exists(r => r.EnterpriseId == e.EnterpriseId)).ToList();

                    model.ListEnterprises = lstEnterpriseOther.OrderBy(e => e.BusinessName)
                        .Select(e => new ListItem(e.BusinessName, e.EnterpriseId.ToString())).ToList();
                    model.DayDeadlineSendReport =
                        int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Report")?.ConfigValue ?? "0");
                    model.DayDeadlineSendReportLate =
                        int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Late_Report")?.ConfigValue ?? "0");
                    model.EnableSignDigitalDoc =
                        (_configCache.GetViaKey("Enable_SignDigital_Doc")?.ConfigValue ?? "0") != "0";
                    model.AccessToken = (string)Session[$"VNPT-SmartCA-{User?.UserName}-AccessToken"];

                    return PartialView("_ImportView", model);
                }

                var enterpriseModel = _enterpriseCache.GetById(model.EnterpriseId);
                if (enterpriseModel == null)
                {
                    logActions.AppendLine(" - Lỗi: Doanh nghiệp không tồn tại");
                    AppProcessor.Logger.Message(logActions.ToString());
                    return Json(new
                    {
                        status = false,
                        message = CreateMessage($"{_enterpriseTitle}",
                            EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                    });
                }

                #endregion

                #region Check Correct Type Template

                //logActions.AppendLine(
                //    $" - [{User.UserName}] thực hiện gửi báo cáo [{EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)}] cho doanh nghiệp [{enterpriseModel.BusinessName}]");

                logActions.AppendLine(
                    $" - [{User.UserName}] thực hiện gửi báo cáo [{model.TypeReportName}] cho doanh nghiệp [{enterpriseModel.BusinessName}]");
                logActions.AppendLine(" - Thực hiện kiểm tra nội dung báo cáo");

                //if (!CheckCorrectTemplate(model.FileImport, EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)))
                if (!CheckCorrectTemplate(model.FileImport, model.TypeReportName))
                {
                    logActions.AppendLine(" + Nội dung báo cáo không đúng định dạng");
                    AppProcessor.Logger.Message(logActions.ToString());

                    //string sTypeBiz = _mappingReportTypeBiz[enterpriseModel.TypeBusiness];
                    string sTypeBiz = _mappingReportTypeBiz[model.TypeReport];
                    return Json(new
                    {
                        status = false,
                        message = CreateMessage($"Tệp dữ liệu import không đúng loại báo cáo thuộc [{sTypeBiz}]",
                            EnumProcessType.NonFormat, EnumMsgIcon.Error)
                    });
                }

                #endregion

                #region Create File Import

                var fullSignedPath = HostingEnvironment.MapPath("/" + _signedFilesPathFolder);
                var subFolderSignedPath = Path.Combine(fullSignedPath,
                    $"{model.ForMonth.GetValueOrDefault(DateTime.Now).Year}",
                    $"{model.ForMonth.GetValueOrDefault(DateTime.Now).Month}", $"{enterpriseModel.EnterpriseId}");

                if (!Directory.Exists(subFolderSignedPath) && !string.IsNullOrEmpty(subFolderSignedPath))
                    Directory.CreateDirectory(subFolderSignedPath);
                var fileExt = Path.GetExtension(model.FileImport.FileName);
                var fileNameSigned =
                    $"Report_{enterpriseModel.EnterpriseId}_{(model.EnableSignDigitalDoc ? "signed" : "")}{fileExt}";
                var fileSignedFullPath = Path.Combine(subFolderSignedPath, fileNameSigned);

                var dataFileImport = StreamHelper.ReadFully(model.FileImport.InputStream);
                var sFileName = model.FileImport.FileName;

                #endregion

                #region Check Data Import

                bool isSuccessImport;
                var dataImports = ReadDataImports(model.FileImport, out isSuccessImport);
                if (!isSuccessImport)
                {
                    logActions.AppendLine(" - Đọc nội dung báo cáo lỗi");
                    AppProcessor.Logger.Message(logActions.ToString());
                    return Json(new
                    {
                        status = false,
                        message = CreateMessage(AppProcessor.Messagor.GetMessage("ImportData_Message_Fail"),
                            EnumProcessType.NonFormat, EnumMsgIcon.Error)
                    });
                }

                var lstDataImports = ModelProvider.CreateListFromTable<ReportDataImportModel>(dataImports);
                var isWrongData = false;
                dataImports.Columns.Remove("RowIndex");

                var dataChecked = CheckDataImport(dataImports, model.ForMonth, model.EnterpriseId);
                if (dataChecked.Count > 0)
                {
                    logActions.AppendLine(" - Kiểm tra dung báo cáo: Nội dung báo cáo không đúng");
                    AppProcessor.Logger.Message(logActions.ToString());

                    //lstDataImports.ForEach(t =>
                    //{
                    //    if (dataChecked.FirstOrDefault(d => d.Code == t.Code) ==
                    //        null) return;
                    //    t.IsWrong = true;
                    //    isWrongData = true;
                    //});

                    var viewModel = new ReportDataImportViewModel
                    {
                        EnterpriseId = model.EnterpriseId,
                        ForMonth = model.ForMonth,
                        ReponseMessage =
                            CreateMessage($"{_importTile} - [{model.EnterpriseName} tháng {model.ForMonth:MM/yyyy}]",
                                EnumProcessType.Add, EnumMsgIcon.Error)
                    };
                    Session[$"DataImports-{User.UserName}-{model.EnterpriseId}-{model.ForMonth:MM/yyyy}"] =
                        lstDataImports;

                    ViewBag.Title = $"{_importTile} - <b>[{model.EnterpriseName} tháng {model.ForMonth:MM/yyyy}]</b>";
                    return PartialView("_ReviewData", viewModel);
                }

                #endregion

                if (model.EnableSignDigitalDoc && model.TypeSignature == 1)
                {
                    logActions.AppendLine(" - Thực hiện ký số báo cáo");

                    #region Sign Data

                    var accessToken = (string)Session[$"VNPT-SmartCA-{User?.UserName}-AccessToken"];
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        logActions.AppendLine(" - Chưa đăng nhập Smart CA");
                        AppProcessor.Logger.Message(logActions.ToString());

                        return Json(new
                        {
                            status = false,
                            errorCode = 1,
                            message = CreateMessage("Bạn chưa đăng nhập tài khoản Smart CA", EnumProcessType.NonFormat,
                                EnumMsgIcon.Error)
                        });
                    }

                    int isSuccess;
                    try
                    {
                        //isSuccess = SignHash(accessToken, model.FileImport, fileSignedFullPath);
                        string msgSignLogs;
                        isSuccess = Sign(accessToken, dataFileImport, sFileName, fileSignedFullPath, out msgSignLogs);
                        logActions.AppendLine(msgSignLogs);
                    }
                    catch (Exception e)
                    {
                        logActions.AppendLine(
                            $" - Thực hiện {_digitalSignTitle} dữ liệu báo cáo thất bại. Lỗi: [{e.Message}]");
                        AppProcessor.Logger.Message(logActions.ToString());

                        return Json(new
                        {
                            status = false,
                            errorCode = 1,
                            message = CreateMessage(
                                $"Thực hiện {_digitalSignTitle} dữ liệu báo cáo thất bại. Lỗi: [{e.Message}]",
                                EnumProcessType.NonFormat, EnumMsgIcon.Error)
                        });
                    }

                    if (isSuccess > 0)
                    {
                        var errMsg = "";
                        switch (isSuccess)
                        {
                            case 1:
                                errMsg = "Ký số thất bại";
                                break;
                            case 2:
                                errMsg = "Lỗi thông tin chữ ký số";
                                break;
                            case 3:
                                errMsg = "Người dùng không xác nhận ký số từ ứng dụng";
                                break;
                            case 4:
                                errMsg = "Lỗi chữ ký số";
                                break;
                            case 5:
                                errMsg = "Chữ ký số không khớp";
                                break;
                            case 6:
                                errMsg = "Từ chối ký số";
                                break;
                        }

                        logActions.AppendLine(
                            $" - Thực hiện {_digitalSignTitle} dữ liệu báo cáo thất bại. Lỗi: [{errMsg}]");
                        AppProcessor.Logger.Message(logActions.ToString());

                        return Json(new
                        {
                            status = false,
                            errorCode = 1,
                            message = CreateMessage(
                                $"Thực hiện {_digitalSignTitle} dữ liệu báo cáo thất bại. Lỗi: [{errMsg}]",
                                EnumProcessType.NonFormat, EnumMsgIcon.Error)
                        });
                    }

                    logActions.AppendLine($" - Thực hiện {_digitalSignTitle} dữ liệu báo cáo thành công");

                    #endregion
                }
                else
                {
                    logActions.AppendLine(" - Không cần ký số báo cáo và thực hiện lưu file báo cáo vào hệ thống");
                    model.FileImport.SaveAs(fileSignedFullPath);
                }

                //var lstTargets = lstDataImports.GroupBy(d => new { d.Targets, d.Unit })
                //    .Select(g => new { g.Key, Count = g.Count() });

                //lstDataImports.ForEach(t =>
                //{
                //    if (lstTargets.FirstOrDefault(d => d.Key.Targets == t.Targets && d.Key.Unit == t.Unit && d.Count > 1) ==
                //        null) return;
                //    t.IsWrong = true;
                //    isWrongData = true;
                //});

                //if (isWrongData)
                //{
                //    ReportDataImportViewModel viewModel = new ReportDataImportViewModel
                //    {
                //        EnterpriseId = model.EnterpriseId,
                //        ForMonth = model.ForMonth,
                //        ReponseMessage = CreateMessage($"{_importTile} - [{model.EnterpriseName} tháng {model.ForMonth:MM/yyyy}]", EnumProcessType.Add, EnumMsgIcon.Error)
                //    };
                //    Session[$"DataImports-{User.UserName}-{model.EnterpriseId}-{model.ForMonth:MM/yyyy}"] = lstDataImports;

                //    ViewBag.Title = $"{_importTile} - <b>[{model.EnterpriseName} tháng {model.ForMonth:MM/yyyy}]</b>";
                //    return PartialView("_ReviewData", viewModel);
                //}

                #region Save Data Import

                var enterpriseId = _importCache.Import(new ReportDataImportModel
                {
                    EnterpriseId = model.EnterpriseId,
                    ForMonth = model.ForMonth,
                    TypeReport = model.TypeReport,
                    TypeReportName = model.TypeReportName,
                    //TypeReport = enterpriseModel.TypeBusiness,
                    //TypeReportName =
                    //    AppProcessor.Messagor.GetMessage(
                    //        EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)),
                    DataImport = dataImports,
                    ReportFile = fileNameSigned,
                    CreatedBy = User.UserName,
                    Reason = model.Reason
                });
                if (enterpriseId == -7)
                {
                    logActions.AppendLine(" - Lưu nội dung báo cáo vào hệ thống thất bại: Doanh nghiệp không tồn tại");
                    AppProcessor.Logger.Message(logActions.ToString());

                    return Json(new
                    {
                        status = false,
                        message = CreateMessage($"{_enterpriseTitle}",
                            EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                    });
                }

                if (enterpriseId == -99)
                {
                    logActions.AppendLine(
                        $" - Lưu nội dung báo cáo vào hệ thống thất bại: {AppProcessor.Messagor.GetMessage("ImportData_Message_Expire_Submit_Report")}");
                    AppProcessor.Logger.Message(logActions.ToString());

                    return Json(new
                    {
                        status = false,
                        message = CreateMessage(
                            AppProcessor.Messagor.GetMessage("ImportData_Message_Expire_Submit_Report"),
                            EnumProcessType.NonFormat, EnumMsgIcon.Error)
                    });
                }

                logActions.AppendLine(" - Lưu nội dung báo cáo vào hệ thống thành công");
                AppProcessor.Logger.Message(logActions.ToString());

                var response = CreateMessage($"{_importTile} - [{model.EnterpriseName} tháng {model.ForMonth:MM/yyyy}]",
                    EnumProcessType.Add,
                    enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
                return Json(new { status = true, message = response });

                #endregion

            }
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetReviewDataImport(ReportDataImportViewSearchModel searchModel)
        {
            var draw = Request.Form.GetValues("draw")?[0];
            var dataImports =
                Session[$"DataImports-{User.UserName}-{searchModel.EnterpriseId}-{searchModel.ForMonth:MM/yyyy}"] as
                    List<ReportDataImportModel>;
            var total = dataImports?.Count ?? 0;
            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data = dataImports },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult ImportData(ReportDataImportModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListEnterprises = _enterpriseCache.GetViaUser(User.UserName)
                    .Select(e => new ListItem(e.BusinessName, e.EnterpriseId.ToString())).ToList();
                return PartialView("_ImportView", model);
            }

            var enterpriseModel = _enterpriseCache.GetById(model.EnterpriseId);
            if (enterpriseModel == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            //string sTypeBusiness = "EnumTypeBusiness.Accommodation";
            //int eTypeBusiness = enterpriseModel.TypeBusiness;

            //switch ((EnumTypeBusiness)eTypeBusiness)
            //{
            //    case EnumTypeBusiness.Accommodation:
            //        {
            //            sTypeBusiness = "EnumTypeBusiness.Accommodation";
            //            break;
            //        }
            //    case EnumTypeBusiness.Traveling:
            //        {
            //            sTypeBusiness = "EnumTypeBusiness.Traveling";
            //            break;
            //        }
            //}

            //Workbook workbook = new Workbook();
            //workbook.LoadFromStream(model.FileImport.InputStream);
            //BuiltInDocumentProperties wbProps = workbook.DocumentProperties;
            //if (wbProps.Category != sTypeBusiness && eTypeBusiness.ToString() != wbProps.Keywords)
            //{
            //    return Json(new
            //    {
            //        status = true,
            //        message = CreateMessage(string.Format(AppProcessor.Messagor.GetMessage("ImportData_Message_Wrong_Template"), AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness))), EnumProcessType.NonFormat, EnumMsgIcon.Error)
            //    });
            //}

            bool isSuccessImport;
            var dataImport = ReadDataImports(model.FileImport, out isSuccessImport);

            if (!isSuccessImport)
                return Json(new
                {
                    status = true,
                    message = CreateMessage(AppProcessor.Messagor.GetMessage("ImportData_Message_Fail"),
                        EnumProcessType.NonFormat, EnumMsgIcon.Error)
                });

            var enterpriseId = _importCache.Import(new ReportDataImportModel
            {
                EnterpriseId = model.EnterpriseId,
                ForMonth = model.ForMonth,
                TypeReport = model.TypeReport,
                TypeReportName = model.TypeReportName,
                //TypeReport = enterpriseModel.TypeBusiness,
                //TypeReportName =
                //    AppProcessor.Messagor.GetMessage(
                //        EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)),
                DataImport = dataImport,
                CreatedBy = User.UserName,
                Reason = model.Reason
            });
            if (enterpriseId == -7)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            var response = CreateMessage($"{_importTile} - [{model.EnterpriseName} tháng {model.ForMonth:MM/yyyy}]",
                EnumProcessType.Add,
                enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = enterpriseId > 0, message = response });
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult UploadFile(ReportDataImportModel model)
        {
            if (model.FileImport == null)
            {
                return Json(new
                {
                    status = false,
                    message = string.Empty
                });
            }

            var enterpriseModel = _enterpriseCache.GetById(model.EnterpriseId);
            if (enterpriseModel == null)
                return Json(new
                {
                    status = false,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            if (model.EnableSignDigitalDoc && model.TypeSignature != 1 && !HasSignature(model.FileImport))
            {
                return Json(new
                {
                    status = false,
                    message = CreateMessage("Tệp dữ liệu import chưa được ký số. Vui lòng kiểm tra lại.",
                        EnumProcessType.NonFormat, EnumMsgIcon.Error)
                });
            }

            //if (!CheckCorrectTemplate(model.FileImport, EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)))
            if (!CheckCorrectTemplate(model.FileImport, model.TypeReportName))
            {
                //string sTypeBiz = _mappingReportTypeBiz[enterpriseModel.TypeBusiness];
                string sTypeBiz = _mappingReportTypeBiz[model.TypeReport];
                return Json(new
                {
                    status = false,
                    message = CreateMessage($"Tệp dữ liệu import không đúng loại báo cáo thuộc [{sTypeBiz}]",
                        EnumProcessType.NonFormat, EnumMsgIcon.Error)
                });
            }

            #region Check Data Import

            bool isSuccessImport;
            var dataImports = ReadDataImports(model.FileImport, out isSuccessImport);
            if (!isSuccessImport)
                return Json(new
                {
                    status = false,
                    message = CreateMessage(AppProcessor.Messagor.GetMessage("ImportData_Message_Fail"),
                        EnumProcessType.NonFormat, EnumMsgIcon.Error)
                });
            var lstDataImports = ModelProvider.CreateListFromTable<ReportDataImportModel>(dataImports);

            #endregion

            return Json(new
            {
                status = true,
                dataImport = lstDataImports.OrderBy(d => d.RowIndex),
                typeReport = enterpriseModel.TypeBusiness,
                message = ""
            });
        }

        #endregion

        #region View Data Report

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Add)]
        [HttpGet]
        public ActionResult View(int? enterpriseId, DateTime? onMonth, int? typeReport = null)
        {
            var enterpriseModel = _enterpriseCache.GetById(enterpriseId);
            if (enterpriseModel == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            ViewBag.Title =
                $"{_importTile} <b>[{enterpriseModel.BusinessName} tháng {onMonth?.ToString("MM/yyyy")}]</b>";
            var dataImports = _importCache.GetViaEnterpriseOnMonth(enterpriseId, onMonth);
            if (typeReport.HasValue)
                dataImports = (dataImports ?? new List<ReportDataImportModel>()).Where(x => x.TypeReport == typeReport.Value).ToList();
            if (dataImports == null || dataImports.Count <= 0)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_importTile}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            var viewModel = new ReportDataImportViewModel
            {
                EnterpriseId = enterpriseId,
                ForMonth = onMonth,
                TypeReport = typeReport,
                ListDataImports = dataImports
            };
            return PartialView("_View", viewModel);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult GetDataImport(ReportDataImportViewSearchModel searchModel)
        {
            var draw = Request.Form.GetValues("draw")?[0];
            var data = _importCache.GetViaEnterpriseOnMonth(searchModel.EnterpriseId, searchModel.ForMonth);
            if (searchModel.TypeReport.HasValue)
                data = (data ?? new List<ReportDataImportModel>()).Where(x => x.TypeReport == searchModel.TypeReport.Value).ToList();
            if (!searchModel.TypeReport.HasValue || searchModel.TypeReport.Value == (int)EnumTypeBusiness.Manufacturing)
            {
                var catalogResult = LoadViewTypeReport(searchModel.EnterpriseId) as PartialViewResult;
                var catalog = catalogResult?.Model as List<ReportDataImportModel>;
                if (catalog != null && catalog.Any())
                    data = MergeBusinessProductData(catalog, data);
            }
            var total = data.Count;
            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        #endregion

        #region Delete

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int? enterpriseId, DateTime? onMonth)
        {
            if (IsReportLocked(onMonth))
                return Json(new
                {
                    status = false,
                    errorCode = 1,
                    message = CreateMessage(GetReportLockedMessage(onMonth), EnumProcessType.NonFormat,
                        EnumMsgIcon.Error)
                });

            var enterpriseModel = _enterpriseCache.GetById(enterpriseId);
            if (enterpriseModel == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            var model = _importCache.GetDataImportViaEnterpriseOnMonth(enterpriseId, onMonth);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_importTile}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            model.EnterpriseName = enterpriseModel.BusinessName;
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"),
                $"<b>{_importTile} [{enterpriseModel.BusinessName} tháng {onMonth?.ToString("MM/yyyy")}]</b>");
            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(ReportDataImportModel model)
        {
            if (IsReportLocked(model.ForMonth))
                return Json(new
                {
                    status = false,
                    errorCode = 1,
                    message = CreateMessage(GetReportLockedMessage(model.ForMonth), EnumProcessType.NonFormat,
                        EnumMsgIcon.Error)
                });

            if (!ModelState.IsValidField("Reason") ||
                !ModelState.IsValidField("EnterpriseId"))
                return PartialView("_DeleteView", model);
            model.SavedBy = User.Email;
            var isSuccess = _importCache.Delete(model);

            if (isSuccess)
            {
                var enterpriseModel = _enterpriseCache.GetById(model.EnterpriseId);
                if (enterpriseModel != null)
                {
                    var fullSignedPath = HostingEnvironment.MapPath("/" + _signedFilesPathFolder);
                    if (Directory.Exists(fullSignedPath))
                    {
                        var subFolderSignedPath = Path.Combine(fullSignedPath, $"{model.ForMonth.Value.Year}",
                            $"{model.ForMonth.Value.Month}",
                            $"{enterpriseModel.EnterpriseId}");
                        if (Directory.Exists(subFolderSignedPath) &&
                            Directory.GetFiles(subFolderSignedPath).ToList().Any())
                        {
                            var signedFiles = Directory.GetFiles(subFolderSignedPath);
                            foreach (var signedFilePath in signedFiles)
                            {
                                System.IO.File.Delete(signedFilePath);
                            }
                        }

                    }
                }
            }

            var response = CreateMessage(
                $"<b>{_importTile} [{model.EnterpriseName} tháng {model.ForMonth:MM/yyyy}]</b>",
                EnumProcessType.Delete, isSuccess ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = isSuccess, message = response });
        }

        #endregion

        #region Add Data Report

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult Add()
        {
            var lstEnterpisePermits = _enterpriseCache.GetViaUser(User.UserName);
            var lstEnterpriseOther = (lstEnterpisePermits ?? new List<CateEnterpriseModel>())
                .Where(e => !string.IsNullOrWhiteSpace(e.TypeBusiness))
                .OrderBy(e => e.BusinessName)
                .ToList();

            EnumTypeBusiness defaultType = EnumTypeBusiness.Manufacturing;
            if (lstEnterpriseOther.Count > 0)
            {
                int tbVal;
                if (int.TryParse(lstEnterpriseOther[0].TypeBusiness.Split(',')[0], out tbVal) && Enum.IsDefined(typeof(EnumTypeBusiness), tbVal))
                {
                    defaultType = (EnumTypeBusiness)tbVal;
                }
            }

            var reportModel = new TourismReportModel
            {
                ListEnterprises = lstEnterpriseOther.OrderBy(e => e.BusinessName)
                    .Select(e => new ListItem(e.BusinessName, e.EnterpriseId.ToString())).ToList(),
                EnterpriseId = lstEnterpriseOther.Count > 0 ? lstEnterpriseOther[0].EnterpriseId : (int?)null,
                EnterpriseName = lstEnterpriseOther.Count > 0 ? lstEnterpriseOther[0].BusinessName : null,
                EnterpriseTypeBusiness = lstEnterpriseOther.Count > 0 ? lstEnterpriseOther[0].TypeBusiness : null,
                DayDeadlineSendReport =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Report")?.ConfigValue ?? "0"),
                DayDeadlineSendReportLate =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Late_Report")?.ConfigValue ?? "0"),
                AccessToken = (string)Session[$"VNPT-SmartCA-{User?.UserName}-AccessToken"],
                EnableSignDigitalDoc =
                    (_configCache.GetViaKey("Enable_SignDigital_Doc")?.ConfigValue ?? "0") != "0",
                ForMonth = DateTime.Now,
                TypeReport = defaultType,
                TypeReportName = GetTypeBusinessDisplayName(defaultType),
                ListTypeBusiness = GetListTypeBusinessItems()
            };
            return PartialView("_Add", reportModel);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult Add(TourismReportModel model)
        {
            StringBuilder logActions = new StringBuilder();
            logActions.AppendLine("=========================================");

            if (IsReportLocked(model.ForMonth))
            {
                return Json(new
                {
                    status = false,
                    errorCode = 1,
                    message = CreateMessage(GetReportLockedMessage(model.ForMonth), EnumProcessType.NonFormat,
                        EnumMsgIcon.Error)
                });
            }

            if (!ModelState.IsValid)
            {
                var lstEnterpisePermits = _enterpriseCache.GetViaUser(User.UserName);
                var lstEnterpriseOther = lstEnterpisePermits
                    .Where(e => e.TypeBusiness == "1").ToList();

                var reportModel = new TourismReportModel
                {
                    ListEnterprises = lstEnterpriseOther.OrderBy(e => e.BusinessName)
                        .Select(e => new ListItem(e.BusinessName, e.EnterpriseId.ToString())).ToList(),
                    DayDeadlineSendReport =
                        int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Report")?.ConfigValue ?? "0"),
                    DayDeadlineSendReportLate =
                        int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Late_Report")?.ConfigValue ?? "0"),
                    EnableSignDigitalDoc =
                        (_configCache.GetViaKey("Enable_SignDigital_Doc")?.ConfigValue ?? "0") != "0",
                    ForMonth = model.ForMonth,
                    TypeReport = model.TypeReport,
                    TypeReportName = string.IsNullOrWhiteSpace(model.TypeReportName) || model.TypeReportName.StartsWith("TypeBusiness_") ? GetTypeBusinessDisplayName(model.TypeReport) : model.TypeReportName,
                    ListTypeBusiness = GetListTypeBusinessItems()
                };
                return PartialView("_Add", reportModel);
            }

            var enterpriseModel = _enterpriseCache.GetById(model.EnterpriseId);
            if (enterpriseModel == null)
            {
                logActions.AppendLine(" - Lỗi: Doanh nghiệp không tồn tại");
                AppProcessor.Logger.Message(logActions.ToString());
                return Json(new
                {
                    status = false,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            }

            //logActions.AppendLine($" - [{User.UserName}] thực hiện gửi báo cáo [{EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)}] cho doanh nghiệp [{enterpriseModel.BusinessName}]");

            logActions.AppendLine($" - [{User.UserName}] thực hiện gửi báo cáo [{model.TypeReportName}] cho doanh nghiệp [{enterpriseModel.BusinessName}]");
            logActions.AppendLine(" - Đọc nội dung báo cáo");

            var dataReport = ReadFormData(Request.Form);
            EnsureTradingDynamicCodes(dataReport, model.EnterpriseId ?? 0);

            if (string.Equals(Request.Form["BusinessProductReport"], "true", StringComparison.OrdinalIgnoreCase))
            {
                string revenueError;
                if (!ValidateRevenueConstraint(dataReport, out revenueError))
                {
                    logActions.AppendLine(" - Lỗi: " + revenueError);
                    AppProcessor.Logger.Message(logActions.ToString());
                    return Json(new
                    {
                        status = false,
                        errorCode = 1,
                        message = CreateMessage(revenueError, EnumProcessType.NonFormat, EnumMsgIcon.Error)
                    });
                }
            }

            #region Create File Report And Path

            var pathReportTemplate = Server.MapPath("~/Contents/Modules/Report/Templates/_TemplateImportReport.rdlc");
            string mimeType;
            string fileExt;

            var fullSignedPath = HostingEnvironment.MapPath("/" + _signedFilesPathFolder);
            var fileNameReport = $"Report-{enterpriseModel.BusinessName}-{User?.UserName}";

            var dataImports = CreateReport(dataReport, fileNameReport, pathReportTemplate, out mimeType, out fileExt);

            var subFolderSignedPath = Path.Combine(fullSignedPath, $"{model.ForMonth.Year}", $"{model.ForMonth.Month}",
                $"{enterpriseModel.EnterpriseId}");
            if (!Directory.Exists(subFolderSignedPath) && !string.IsNullOrEmpty(subFolderSignedPath))
                Directory.CreateDirectory(subFolderSignedPath);
            var fileNameWithExt = $"{fileNameReport}.{fileExt}";
            var fileNameSigned = $"Report_{enterpriseModel.EnterpriseId}_signed.{fileExt}";
            var fileSignedFullPath = Path.Combine(subFolderSignedPath, fileNameSigned);

            #endregion

            if (model.EnableSignDigitalDoc)
            {
                logActions.AppendLine(" - Thực hiện ký số báo cáo");

                #region Sign Data

                var accessToken = (string)Session[$"VNPT-SmartCA-{User?.UserName}-AccessToken"];
                if (string.IsNullOrEmpty(accessToken))
                {
                    logActions.AppendLine(" - Chưa đăng nhập Smart CA");
                    AppProcessor.Logger.Message(logActions.ToString());

                    return Json(new
                    {
                        status = false,
                        errorCode = 1,
                        message = CreateMessage("Bạn chưa đăng nhập tài khoản Smart CA", EnumProcessType.NonFormat,
                            EnumMsgIcon.Error)
                    });
                }

                int isSuccess;
                try
                {
                    //isSuccess = SignHash(accessToken, dataImports, fileNameWithExt, fileExt, fileSignedFullPath);
                    string msgSignLogs;
                    isSuccess = Sign(accessToken, dataImports, fileNameWithExt, fileSignedFullPath, out msgSignLogs);
                    logActions.AppendLine(msgSignLogs);
                }
                catch (Exception e)
                {
                    logActions.AppendLine($" - Thực hiện {_digitalSignTitle} dữ liệu báo cáo thất bại. Lỗi: [{e.Message}]");
                    AppProcessor.Logger.Message(logActions.ToString());
                    return Json(new
                    {
                        status = false,
                        errorCode = 1,
                        message = CreateMessage(
                            $"Thực hiện {_digitalSignTitle} dữ liệu báo cáo thất bại. Lỗi: [{e.Message}]",
                            EnumProcessType.NonFormat, EnumMsgIcon.Error)
                    });
                }

                if (isSuccess > 0)
                {
                    var errMsg = "";
                    switch (isSuccess)
                    {
                        case 1:
                            errMsg = "Ký số thất bại";
                            break;
                        case 2:
                            errMsg = "Lỗi thông tin chữ ký số";
                            break;
                        case 3:
                            errMsg = "Người dùng không xác nhận ký số từ ứng dụng";
                            break;
                        case 4:
                            errMsg = "Lỗi chữ ký số";
                            break;
                        case 5:
                            errMsg = "Chữ ký số không khớp";
                            break;
                        case 6:
                            errMsg = "Từ chối ký số";
                            break;
                    }

                    logActions.AppendLine($" - Thực hiện {_digitalSignTitle} dữ liệu báo cáo thất bại. Lỗi: [{errMsg}]");
                    AppProcessor.Logger.Message(logActions.ToString());

                    return Json(new
                    {
                        status = false,
                        errorCode = 1,
                        message = CreateMessage(
                            $"Thực hiện {_digitalSignTitle} dữ liệu báo cáo thất bại. Lỗi: [{errMsg}]",
                            EnumProcessType.NonFormat, EnumMsgIcon.Error)
                    });
                }

                logActions.AppendLine($" - Thực hiện {_digitalSignTitle} dữ liệu báo cáo thành công");

                #endregion
            }
            else
            {
                logActions.AppendLine($" - Thực hiện {_digitalSignTitle} dữ liệu báo cáo thành công");
                System.IO.File.WriteAllBytes(fileSignedFullPath, dataImports);
            }

            if (string.IsNullOrWhiteSpace(model.TypeReportName) || model.TypeReportName.StartsWith("TypeBusiness_"))
            {
                model.TypeReportName = GetTypeBusinessDisplayName(model.TypeReport);
            }

            var enterpriseId = _importCache.Import(new ReportDataImportModel
            {
                EnterpriseId = model.EnterpriseId,
                ForMonth = model.ForMonth,
                TypeReport = (int)model.TypeReport,
                TypeReportName = model.TypeReportName,
                //TypeReport = enterpriseModel.TypeBusiness,
                //TypeReportName =
                //    AppProcessor.Messagor.GetMessage(
                //        EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)),
                DataImport = dataReport,
                ReportFile = fileNameSigned,
                CreatedBy = User.UserName,
                Reason = "Thêm mới báo cáo"
            });

            if (enterpriseId == -7)
            {
                logActions.AppendLine(" - Lưu nội dung báo cáo vào hệ thống thất bại: Doanh nghiệp không tồn tại");
                AppProcessor.Logger.Message(logActions.ToString());
                return Json(new
                {
                    status = false,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            }

            if (enterpriseId == -99)
            {
                logActions.AppendLine($" - Lưu nội dung báo cáo vào hệ thống thất bại: {AppProcessor.Messagor.GetMessage("ImportData_Message_Expire_Submit_Report")}");
                AppProcessor.Logger.Message(logActions.ToString());

                return Json(new
                {
                    status = false,
                    message = CreateMessage(AppProcessor.Messagor.GetMessage("ImportData_Message_Expire_Submit_Report"),
                        EnumProcessType.NonFormat, EnumMsgIcon.Error)
                });
            }

            logActions.AppendLine(" - Lưu nội dung báo cáo vào hệ thống thành công");
            AppProcessor.Logger.Message(logActions.ToString());

            var response = CreateMessage($"{_importTile} - [{model.EnterpriseName} tháng {model.ForMonth:MM/yyyy}]",
                EnumProcessType.Add,
                enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        #endregion

        #region Edit Data Report

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int? enterpriseId, DateTime? onMonth, int? typeReport = null)
        {
            if (IsReportLocked(onMonth))
                return Json(new
                {
                    status = false,
                    errorCode = 1,
                    message = CreateMessage(GetReportLockedMessage(onMonth), EnumProcessType.NonFormat,
                        EnumMsgIcon.Error)
                });

            var enterpriseModel = _enterpriseCache.GetById(enterpriseId);
            if (enterpriseModel == null)
                return Json(new
                {
                    status = false,
                    errorCode = 1,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            ViewBag.Title =
                $"{_importTile} <b>[{enterpriseModel.BusinessName}]</b> tháng <b class='text-yellow'>[{onMonth?.ToString("MM/yyyy")}]</b>";
            var dataImports = _importCache.GetViaEnterpriseOnMonth(enterpriseId, onMonth);
            if (typeReport.HasValue)
                dataImports = (dataImports ?? new List<ReportDataImportModel>()).Where(x => x.TypeReport == typeReport.Value).ToList();
            if (dataImports == null || dataImports.Count <= 0)
                return Json(new
                {
                    status = false,
                    errorCode = 1,
                    message = CreateMessage($"{_importTile}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            var isTradingReport = dataImports.Any(x => x.TypeReport == (int)EnumTypeBusiness.Trading);
            var isImportExportReport = dataImports.Any(x => x.TypeReport == (int)EnumTypeBusiness.ImportExport);
            if (isTradingReport)
            {
                var tradingResult = LoadTradingReport(enterpriseId, onMonth, null, dataImports);
                dataImports = tradingResult.Model as List<ReportDataImportModel> ?? dataImports;
                ViewBag.IsTradingReport = true;
                ViewBag.TradingProductOptions = tradingResult.ViewData["TradingProductOptions"];
                ViewBag.ForMonth = tradingResult.ViewData["ForMonth"] ?? (onMonth ?? DateTime.Now);
            }
            else if (isImportExportReport)
            {
                var ieResult = LoadImportExportReport(enterpriseId, onMonth, null, dataImports);
                dataImports = ieResult.Model as List<ReportDataImportModel> ?? dataImports;
                ViewBag.IsImportExportReport = true;
                ViewBag.ListNationals = ieResult.ViewData["ListNationals"];
                ViewBag.ForMonth = ieResult.ViewData["ForMonth"] ?? (onMonth ?? DateTime.Now);
            }
            else
            {
                // Báo cáo chỉ tiêu công nghiệp không lưu các dòng tiêu đề nhóm. Khi sửa,
                // dựng lại toàn bộ khung từ danh mục rồi gắn số liệu đã lưu theo chỉ tiêu.
                var catalogResult = LoadViewTypeReport(enterpriseId, onMonth) as PartialViewResult;
                var catalog = catalogResult?.Model as List<ReportDataImportModel>;
                if (catalog != null && catalog.Any())
                {
                    dataImports = MergeBusinessProductData(catalog, dataImports);
                    ViewBag.IsBusinessProductReport = true;
                }
                if (catalogResult != null)
                {
                    ViewBag.ExportBusinessProductOptions = catalogResult.ViewData["ExportBusinessProductOptions"];
                    ViewBag.ImportBusinessProductOptions = catalogResult.ViewData["ImportBusinessProductOptions"];
                    ViewBag.BusinessProductEnterpriseId = catalogResult.ViewData["BusinessProductEnterpriseId"];
                    ViewBag.ForMonth = catalogResult.ViewData["ForMonth"] ?? (onMonth ?? DateTime.Now);
                }
            }
            EnumTypeBusiness selectedType = EnumTypeBusiness.Manufacturing;
            var savedTypeVal = dataImports?.FirstOrDefault(d => d.TypeReport > 0)?.TypeReport;
            if (savedTypeVal.HasValue && Enum.IsDefined(typeof(EnumTypeBusiness), savedTypeVal.Value))
            {
                selectedType = (EnumTypeBusiness)savedTypeVal.Value;
            }
            else if (!string.IsNullOrEmpty(enterpriseModel.TypeBusiness))
            {
                int entType;
                if (int.TryParse(enterpriseModel.TypeBusiness.Split(',')[0], out entType) && Enum.IsDefined(typeof(EnumTypeBusiness), entType))
                {
                    selectedType = (EnumTypeBusiness)entType;
                }
            }

            var typeReportName = GetTypeBusinessDisplayName(selectedType);

            var reportModel = new TourismReportModel
            {
                DayDeadlineSendReport =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Report")?.ConfigValue ?? "0"),
                DayDeadlineSendReportLate =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Late_Report")?.ConfigValue ?? "0"),
                EnterpriseId = enterpriseId,
                ForMonth = onMonth ?? DateTime.Now,
                ListDataImports = dataImports,
                EnterpriseName = enterpriseModel.BusinessName,
                TypeReport = selectedType,
                TypeReportName = typeReportName,
                ListTypeBusiness = GetListTypeBusinessItems(),
                IsEdit = true,
                AccessToken = (string)Session[$"VNPT-SmartCA-{User?.UserName}-AccessToken"],
                EnableSignDigitalDoc =
                    (_configCache.GetViaKey("Enable_SignDigital_Doc")?.ConfigValue ?? "0") != "0"
            };

            return PartialView("_Edit", reportModel);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(TourismReportModel model)
        {
            StringBuilder logActions = new StringBuilder();
            logActions.AppendLine("=========================================");

            if (IsReportLocked(model.ForMonth))
            {
                return Json(new
                {
                    status = false,
                    errorCode = 1,
                    message = CreateMessage(GetReportLockedMessage(model.ForMonth), EnumProcessType.NonFormat,
                        EnumMsgIcon.Error)
                });
            }

            #region Check Valid Form

            var enterpriseModel = _enterpriseCache.GetById(model.EnterpriseId);
            if (enterpriseModel == null)
            {
                logActions.AppendLine(" - Lỗi: Doanh nghiệp không tồn tại");
                AppProcessor.Logger.Message(logActions.ToString());

                return Json(new
                {
                    status = false,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Title =
                    $"{_importTile} <b>[{enterpriseModel.BusinessName}]</b> tháng <b class='text-yellow'>[{model.ForMonth:MM/yyyy}]</b>";
                var dataImports = _importCache.GetViaEnterpriseOnMonth(model.EnterpriseId, model.ForMonth);
                if (dataImports == null || dataImports.Count <= 0)
                    return Json(new
                    {
                        status = false,
                        message = CreateMessage($"{_importTile}",
                            EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                    });

                var reportModel = new TourismReportModel
                {
                    //ListEnterprises = lstEnterpriseOther.OrderBy(e => e.BusinessName)
                    //    .Select(e => new ListItem(e.BusinessName, e.EnterpriseId.ToString())).ToList(),
                    DayDeadlineSendReport =
                        int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Report")?.ConfigValue ?? "0"),
                    DayDeadlineSendReportLate =
                        int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Late_Report")?.ConfigValue ?? "0"),
                    EnterpriseId = model.EnterpriseId,
                    ForMonth = model.ForMonth,
                    ListDataImports = dataImports,
                    EnterpriseName = enterpriseModel.BusinessName,
                    TypeReport = model.TypeReport,
                    TypeReportName = string.IsNullOrWhiteSpace(model.TypeReportName) || model.TypeReportName.StartsWith("TypeBusiness_") ? GetTypeBusinessDisplayName(model.TypeReport) : model.TypeReportName,
                    ListTypeBusiness = GetListTypeBusinessItems(),
                    IsEdit = true,
                    EnableSignDigitalDoc =
                        (_configCache.GetViaKey("Enable_SignDigital_Doc")?.ConfigValue ?? "0") != "0"
                };
                return PartialView("_Edit", reportModel);
            }

            #endregion

            //logActions.AppendLine($" - [{User.UserName}] thực hiện gửi báo cáo [{EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)}] cho doanh nghiệp [{enterpriseModel.BusinessName}]");
            logActions.AppendLine($" - [{User.UserName}] thực hiện gửi báo cáo [{model.TypeReportName}] cho doanh nghiệp [{enterpriseModel.BusinessName}]");
            logActions.AppendLine(" - Đọc nội dung báo cáo");

            var dataReport = ReadFormData(Request.Form);
            EnsureTradingDynamicCodes(dataReport, model.EnterpriseId ?? 0);

            if (string.Equals(Request.Form["BusinessProductReport"], "true", StringComparison.OrdinalIgnoreCase))
            {
                string revenueError;
                if (!ValidateRevenueConstraint(dataReport, out revenueError))
                {
                    logActions.AppendLine(" - Lỗi: " + revenueError);
                    AppProcessor.Logger.Message(logActions.ToString());
                    return Json(new
                    {
                        status = false,
                        errorCode = 1,
                        message = CreateMessage(revenueError, EnumProcessType.NonFormat, EnumMsgIcon.Error)
                    });
                }
            }

            #region Create Report And File Path

            var pathReportTemplate = Server.MapPath("~/Contents/Modules/Report/Templates/_TemplateImportReport.rdlc");
            string mimeType;
            string fileExt;

            var fullSignedPath = HostingEnvironment.MapPath("/" + _signedFilesPathFolder);
            var fileNameReport = $"Report-{enterpriseModel.BusinessName}-{User?.UserName}";

            var dataImportViaTemplates =
                CreateReport(dataReport, fileNameReport, pathReportTemplate, out mimeType, out fileExt);

            var subFolderSignedPath = Path.Combine(fullSignedPath, $"{model.ForMonth.Year}", $"{model.ForMonth.Month}",
                $"{enterpriseModel.EnterpriseId}");
            if (!Directory.Exists(subFolderSignedPath) && !string.IsNullOrEmpty(subFolderSignedPath))
                Directory.CreateDirectory(subFolderSignedPath);
            var fileNameWithExt = $"{fileNameReport}.{fileExt}";
            var fileNameSigned = $"Report_{enterpriseModel.EnterpriseId}_signed.{fileExt}";
            var fileSignedFullPath = Path.Combine(subFolderSignedPath, fileNameSigned);

            #endregion

            if (model.EnableSignDigitalDoc)
            {
                logActions.AppendLine(" - Thực hiện ký số báo cáo");

                #region Sign Data

                var accessToken = (string)Session[$"VNPT-SmartCA-{User?.UserName}-AccessToken"];
                if (string.IsNullOrEmpty(accessToken))
                {
                    logActions.AppendLine(" - Chưa đăng nhập Smart CA");
                    AppProcessor.Logger.Message(logActions.ToString());

                    return Json(new
                    {
                        status = false,
                        errorCode = 1,
                        message = CreateMessage("Bạn chưa đăng nhập tài khoản Smart CA", EnumProcessType.NonFormat,
                            EnumMsgIcon.Error)
                    });
                }

                int isSuccess;
                try
                {
                    //isSuccess = SignHash(accessToken, dataImportViaTemplates, fileNameWithExt, fileExt, fileSignedFullPath);
                    string msgSignLogs;
                    isSuccess = Sign(accessToken, dataImportViaTemplates, fileNameWithExt, fileSignedFullPath, out msgSignLogs);
                    logActions.AppendLine(msgSignLogs);
                }
                catch (Exception e)
                {
                    logActions.AppendLine($" - Thực hiện {_digitalSignTitle} dữ liệu báo cáo thất bại. Lỗi: [{e.Message}]");
                    AppProcessor.Logger.Message(logActions.ToString());

                    return Json(new
                    {
                        status = false,
                        errorCode = 1,
                        message = CreateMessage(
                            $"Thực hiện {_digitalSignTitle} dữ liệu báo cáo thất bại. Lỗi: [{e.Message}]",
                            EnumProcessType.NonFormat, EnumMsgIcon.Error)
                    });
                }

                if (isSuccess > 0)
                {
                    var errMsg = "";
                    switch (isSuccess)
                    {
                        case 1:
                            errMsg = "Ký số thất bại";
                            break;
                        case 2:
                            errMsg = "Lỗi thông tin chữ ký số";
                            break;
                        case 3:
                            errMsg = "Người dùng không xác nhận ký số từ ứng dụng";
                            break;
                        case 4:
                            errMsg = "Lỗi chữ ký số";
                            break;
                        case 5:
                            errMsg = "Chữ ký số không khớp";
                            break;
                        case 6:
                            errMsg = "Từ chối ký số";
                            break;
                    }

                    logActions.AppendLine($" - Thực hiện {_digitalSignTitle} dữ liệu báo cáo thất bại. Lỗi: [{errMsg}]");
                    AppProcessor.Logger.Message(logActions.ToString());

                    return Json(new
                    {
                        status = false,
                        errorCode = 1,
                        message = CreateMessage(
                            $"Thực hiện {_digitalSignTitle} dữ liệu báo cáo thất bại. Lỗi: [{errMsg}]",
                            EnumProcessType.NonFormat, EnumMsgIcon.Error)
                    });
                }

                logActions.AppendLine($" - Thực hiện {_digitalSignTitle} dữ liệu báo cáo thành công");

                #endregion
            }
            else
            {
                logActions.AppendLine($" - Thực hiện {_digitalSignTitle} dữ liệu báo cáo thành công");

                System.IO.File.WriteAllBytes(fileSignedFullPath, dataImportViaTemplates);
            }

            if (string.IsNullOrWhiteSpace(model.TypeReportName) || model.TypeReportName.StartsWith("TypeBusiness_"))
            {
                model.TypeReportName = GetTypeBusinessDisplayName(model.TypeReport);
            }

            var enterpriseId = _importCache.Import(new ReportDataImportModel
            {
                EnterpriseId = model.EnterpriseId,
                ForMonth = model.ForMonth,
                TypeReport = (int)model.TypeReport,
                TypeReportName = model.TypeReportName,
                //TypeReport = enterpriseModel.TypeBusiness,
                //TypeReportName =
                //    AppProcessor.Messagor.GetMessage(
                //        EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)),
                DataImport = dataReport,
                CreatedBy = User.UserName,
                Reason = "Cập nhật báo cáo"
            });

            if (enterpriseId == -7)
            {
                logActions.AppendLine(" - Lưu nội dung báo cáo vào hệ thống thất bại: Doanh nghiệp không tồn tại");
                AppProcessor.Logger.Message(logActions.ToString());

                return Json(new
                {
                    status = false,
                    message = CreateMessage($"{_enterpriseTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            }

            if (enterpriseId == -99)
            {
                logActions.AppendLine($" - Lưu nội dung báo cáo vào hệ thống thất bại: {AppProcessor.Messagor.GetMessage("ImportData_Message_Expire_Submit_Report")}");
                AppProcessor.Logger.Message(logActions.ToString());

                return Json(new
                {
                    status = false,
                    message = CreateMessage(AppProcessor.Messagor.GetMessage("ImportData_Message_Expire_Submit_Report"),
                        EnumProcessType.NonFormat, EnumMsgIcon.Error)
                });
            }

            logActions.AppendLine(" - Lưu nội dung báo cáo vào hệ thống thành công");
            AppProcessor.Logger.Message(logActions.ToString());

            var response = CreateMessage($"{_importTile} - [{model.EnterpriseName} tháng {model.ForMonth:MM/yyyy}]",
                EnumProcessType.Edit,
                enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        #endregion

        #region Other Function

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult AddTradeCatalogProduct(string prefix)
        {
            var codePrefix = GetTradeCatalogCodePrefix(prefix);
            if (codePrefix == null)
                return Json(new { status = false, message = "Nhóm mặt hàng không hợp lệ." },
                    JsonRequestBehavior.AllowGet);

            ViewBag.TradePrefix = prefix.ToUpperInvariant();
            return PartialView("_AddTradeCatalogProduct");
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Add)]
        [ValidateInput(false)]
        public ActionResult AddTradeCatalogProduct(string prefix, string productName, string unit)
        {
            var codePrefix = GetTradeCatalogCodePrefix(prefix);
            productName = (productName ?? string.Empty).Trim();
            unit = (unit ?? string.Empty).Trim();
            if (codePrefix == null || string.IsNullOrWhiteSpace(productName))
                return Json(new { status = false, message = "Vui lòng nhập tên mặt hàng." });

            var products = _businessProductCache.GetAll() ?? new List<CateBusinessProductModel>();
            var nextNumber = products.Where(product => !string.IsNullOrWhiteSpace(product.ProductCode) &&
                                                       product.ProductCode.StartsWith(codePrefix,
                                                           StringComparison.OrdinalIgnoreCase))
                .Select(product => product.ProductCode.Substring(codePrefix.Length))
                .Select(suffix =>
                {
                    int number;
                    return int.TryParse(suffix, out number) ? number : 0;
                })
                .DefaultIfEmpty(0)
                .Max() + 1;

            int? productId = null;
            string productCode = null;
            // Retry to handle a concurrent insertion which happens to use the same next code.
            for (var attempt = 0; attempt < 10; attempt++)
            {
                productCode = codePrefix + (nextNumber + attempt).ToString("D3");
                productId = _businessProductCache.Save(new CateBusinessProductModel
                {
                    ProductCode = productCode,
                    ProductName = productName,
                    IndustryId = null,
                    Unit = unit,
                    DisplayOrder = 0,
                    IsActive = true,
                    UpdatedBy = User.UserName
                });
                if (productId != -2) break;
            }

            if (!productId.HasValue || productId.Value <= 0)
                return Json(new { status = false, message = "Không thể thêm mặt hàng. Vui lòng thử lại." });

            return Json(new
            {
                status = true,
                product = new { code = productCode, name = productName, unit }
            });
        }

        [AjaxOnly]
        [HttpGet]
        [AllowAnyPermission]
        public ActionResult SearchTradingProducts(string q = null, int page = 1)
        {
            const int pageSize = 20;
            page = Math.Max(1, page);
            int total;
            var search = new SysSearchModel
            {
                Search = string.IsNullOrWhiteSpace(q) ? null : q.Trim(),
                Order = "1",
                OrderDir = "ASC",
                StartIndex = (page - 1) * pageSize,
                PageSize = pageSize
            };
            var products = (_businessProductCache.Get(out total, null, search) ?? new List<CateBusinessProductModel>())
                .Where(x => x.IsActive && !string.IsNullOrWhiteSpace(x.ProductCode))
                .Select(x => new { id = x.ProductCode, text = x.ProductName + " (" + x.ProductCode + ")", name = x.ProductName })
                .ToList();
            return Json(new { results = products, pagination = new { more = page * pageSize < total } }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [AllowAnyPermission]
        public ActionResult SearchUnconfiguredBusinessProducts(int enterpriseId, string prefix, string q = null, int page = 1)
        {
            const int pageSize = 20;
            page = Math.Max(1, page);
            prefix = string.Equals(prefix, "NK", StringComparison.OrdinalIgnoreCase) ? "NK" : "XK";
            var all = _businessProductCache.GetUnconfiguredByPrefix(enterpriseId, prefix) ?? new List<CateBusinessProductModel>();
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                all = all.Where(x => (x.ProductName ?? string.Empty).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 || (x.ProductCode ?? string.Empty).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }
            var total = all.Count;
            var results = all.OrderBy(x => x.DisplayOrder).ThenBy(x => x.ProductName).Skip((page - 1) * pageSize).Take(pageSize)
                .Select(x => new { id = x.ProductCode, text = x.ProductName + " (" + x.ProductCode + ")", name = x.ProductName, unit = x.Unit }).ToList();
            return Json(new { results, pagination = new { more = page * pageSize < total } }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [AllowAnyPermission]
        public ActionResult SearchNationals(string q = null, int page = 1)
        {
            const int pageSize = 20;
            page = Math.Max(1, page);
            int total;
            var search = new SysSearchModel { Search = string.IsNullOrWhiteSpace(q) ? null : q.Trim(), Order = "1", OrderDir = "ASC", StartIndex = (page - 1) * pageSize, PageSize = pageSize };
            var nationals = _nationalCache.Get(out total, search) ?? new List<CateNationalModel>();
            var results = nationals.Select(x => new { id = x.NationalCode, text = x.NationalName, name = x.NationalName }).ToList();
            return Json(new { results, pagination = new { more = page * pageSize < total } }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [AllowAnyPermission]
        public ActionResult SearchExportProducts(int? enterpriseId, string q = null, int page = 1)
        {
            const int pageSize = 20;
            page = Math.Max(1, page);
            var all = new List<CateBusinessProductModel>();
            if (enterpriseId.GetValueOrDefault() > 0)
            {
                all.AddRange(_businessProductCache.GetUnconfiguredByPrefix(enterpriseId.Value, "XK") ?? new List<CateBusinessProductModel>());
                all.AddRange(_businessProductCache.GetByPrefix(enterpriseId.Value, "XK") ?? new List<CateBusinessProductModel>());
                all = all.GroupBy(x => x.ProductCode ?? string.Empty, StringComparer.OrdinalIgnoreCase).Select(x => x.First()).ToList();
            }
            all = all.Where(x => !string.IsNullOrWhiteSpace(x.ProductCode) && x.ProductCode.StartsWith("XKB", StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                all = all.Where(x => (x.ProductName ?? string.Empty).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 || (x.ProductCode ?? string.Empty).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }
            var total = all.Count;
            var products = all.OrderBy(x => x.DisplayOrder).ThenBy(x => x.ProductName).Skip((page - 1) * pageSize).Take(pageSize)
                .Select(x => new { id = x.ProductCode, text = x.ProductName, name = x.ProductName }).ToList();
            return Json(new { results = products, pagination = new { more = page * pageSize < total } }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [AllowAnyPermission]
        public ActionResult LoadViewTypeReport(int? enterpriseId, DateTime? onMonth = null, string forMonth = null, int? typeReport = null)
        {
            if (typeReport.HasValue && typeReport.Value == (int)EnumTypeBusiness.Trading)
            {
                return LoadTradingReport(enterpriseId, onMonth, forMonth);
            }
            if (typeReport.HasValue && typeReport.Value == (int)EnumTypeBusiness.ImportExport)
            {
                return LoadImportExportReport(enterpriseId, onMonth, forMonth);
            }

            if (!typeReport.HasValue && enterpriseId.HasValue && enterpriseId.Value > 0)
            {
                var ent = _enterpriseCache.GetById(enterpriseId.Value);
                if (ent != null && !string.IsNullOrEmpty(ent.TypeBusiness))
                {
                    int biz;
                    if (int.TryParse(ent.TypeBusiness.Split(',')[0], out biz))
                    {
                        if (biz == (int)EnumTypeBusiness.Trading)
                            return LoadTradingReport(enterpriseId, onMonth, forMonth);
                        if (biz == (int)EnumTypeBusiness.ImportExport)
                            return LoadImportExportReport(enterpriseId, onMonth, forMonth);
                    }
                }
            }

            var products = new List<ReportDataImportModel>();
            var mainProducts = new List<ReportDataImportModel>();
            var exportProducts = new List<ReportDataImportModel>();
            var importProducts = new List<ReportDataImportModel>();
            if (!enterpriseId.HasValue || enterpriseId.Value <= 0)
            {
                return PartialView("_BusinessProductReport", products);
            }

            var targetDate = onMonth;
            if (!targetDate.HasValue && !string.IsNullOrWhiteSpace(forMonth))
            {
                DateTime dt;
                if (DateTime.TryParseExact(forMonth.Trim(), new[] { "MM/yyyy", "M/yyyy", "yyyy-MM-dd", "yyyy/MM", "dd/MM/yyyy" },
                    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt))
                {
                    targetDate = dt;
                }
                else if (DateTime.TryParse(forMonth.Trim(), out dt))
                {
                    targetDate = dt;
                }
            }
            var currentForMonth = targetDate ?? DateTime.Now;

            try
            {
                mainProducts = (_businessProductCache.GetViaEnterprise(enterpriseId.Value) ?? new List<CateBusinessProductModel>())
                    .Select(product => new ReportDataImportModel
                    {
                        Targets = product.ProductName,
                        Unit = product.Unit,
                        Code = !string.IsNullOrWhiteSpace(product.ProductCode)
                            ? product.ProductCode.Trim()
                            : string.Format("02{0:D4}", product.ProductId)
                    }).ToList();
            }
            catch (Exception exMain)
            {
                AppProcessor.Logger.Message("GetViaEnterprise error: " + exMain.Message);
            }

            try
            {
                exportProducts = (_businessProductCache.GetByPrefix(enterpriseId.Value, "XK") ?? new List<CateBusinessProductModel>())
                    .Select(product => new ReportDataImportModel
                    {
                        Targets = product.ProductName,
                        Unit = product.Unit,
                        Code = product.ProductCode
                    }).ToList();
            }
            catch (Exception exXk)
            {
                AppProcessor.Logger.Message("GetByPrefix XK error: " + exXk.Message);
            }

            try
            {
                importProducts = (_businessProductCache.GetByPrefix(enterpriseId.Value, "NK") ?? new List<CateBusinessProductModel>())
                    .Select(product => new ReportDataImportModel
                    {
                        Targets = product.ProductName,
                        Unit = product.Unit,
                        Code = product.ProductCode
                    }).ToList();
            }
            catch (Exception exNk)
            {
                AppProcessor.Logger.Message("GetByPrefix NK error: " + exNk.Message);
            }

            // Khung chỉ tiêu theo mẫu báo cáo doanh nghiệp hằng tháng.
            products.Add(CreateBusinessProductLine("Tổng doanh thu", "Tỷ đồng", "01"));
            products.Add(CreateBusinessProductLine("Trong đó doanh thu công nghiệp", "Tỷ đồng", "0101"));
            products.Add(CreateBusinessProductHeader("Sản phẩm công nghiệp chủ yếu"));
            products.AddRange(mainProducts);
            products.Add(CreateBusinessProductLine("Kim ngạch xuất khẩu", "1.000 USD", "06"));
            products.Add(CreateBusinessProductHeader("Nhóm/mặt hàng xuất khẩu chủ yếu"));
            products.AddRange(exportProducts);
            products.Add(CreateBusinessProductLine("Kim ngạch nhập khẩu", "1.000 USD", "07"));
            products.Add(CreateBusinessProductHeader("Nhóm/mặt hàng nhập khẩu chủ yếu"));
            products.AddRange(importProducts);
            ApplyBusinessProductHierarchy(products);

            // Tự động nạp kế hoạch năm nếu đã từng nhập trong năm báo cáo hiện tại
            try
            {
                var reportYear = currentForMonth.Year;
                var allEnterpriseReports = _importCache.GetViaEnterpriseOnMonth(enterpriseId.Value, null);
                if (allEnterpriseReports != null && allEnterpriseReports.Any())
                {
                    var yearlyPlans = allEnterpriseReports
                        .Where(r => r.ForMonth.HasValue && r.ForMonth.Value.Year == reportYear && r.AccumulatedBeginingOfYear.HasValue && r.AccumulatedBeginingOfYear.Value > 0)
                        .ToList();

                    if (yearlyPlans.Any())
                    {
                        var plansByCode = yearlyPlans
                            .Where(r => !string.IsNullOrWhiteSpace(r.Code))
                            .GroupBy(r => r.Code.Trim(), StringComparer.OrdinalIgnoreCase)
                            .ToDictionary(g => g.Key, g => g.OrderByDescending(r => r.ForMonth ?? DateTime.MinValue).First().AccumulatedBeginingOfYear, StringComparer.OrdinalIgnoreCase);

                        var plansByTarget = yearlyPlans
                            .Where(r => !string.IsNullOrWhiteSpace(r.Targets))
                            .GroupBy(r => r.Targets.Trim(), StringComparer.OrdinalIgnoreCase)
                            .ToDictionary(g => g.Key, g => g.OrderByDescending(r => r.ForMonth ?? DateTime.MinValue).First().AccumulatedBeginingOfYear, StringComparer.OrdinalIgnoreCase);

                        foreach (var p in products)
                        {
                            // Kim ngạch xuất khẩu (06) và nhập khẩu (07) không nạp trực tiếp, chỉ tính từ các mặt hàng bên trong
                            if (p.Code == "06" || p.Code == "07" || p.Targets == "Kim ngạch xuất khẩu" || p.Targets == "Kim ngạch nhập khẩu")
                            {
                                continue;
                            }

                            if (!p.AccumulatedBeginingOfYear.HasValue || p.AccumulatedBeginingOfYear.Value <= 0)
                            {
                                double? planVal;
                                if (!string.IsNullOrWhiteSpace(p.Code) && plansByCode.TryGetValue(p.Code.Trim(), out planVal))
                                {
                                    p.AccumulatedBeginingOfYear = planVal;
                                }
                                else if (!string.IsNullOrWhiteSpace(p.Targets) && plansByTarget.TryGetValue(p.Targets.Trim(), out planVal))
                                {
                                    p.AccumulatedBeginingOfYear = planVal;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exPlan)
            {
                AppProcessor.Logger.Message("YearlyPlan loading error: " + exPlan.Message);
            }

            // Kim ngạch xuất khẩu và nhập khẩu chỉ tính từ tổng các mặt hàng bên trong
            RecalculateTradeSummaryInModel(products);

            ViewBag.BusinessProductEnterpriseId = enterpriseId.Value;
            ViewBag.ForMonth = currentForMonth;

            return PartialView("_BusinessProductReport", products);
        }

        private PartialViewResult LoadTradingReport(int? enterpriseId, DateTime? onMonth, string forMonth,
            List<ReportDataImportModel> savedData = null)
        {
            DateTime reportMonth;
            if (!onMonth.HasValue && !string.IsNullOrWhiteSpace(forMonth) &&
                DateTime.TryParse(forMonth, out reportMonth))
                onMonth = reportMonth;
            reportMonth = onMonth ?? DateTime.Now;

            var rows = CreateTradingReportLines();
            if (enterpriseId.HasValue && enterpriseId.Value > 0)
            {
                var previous = (_importCache.GetViaEnterpriseOnMonth(enterpriseId.Value, null) ?? new List<ReportDataImportModel>())
                    .Where(x => x.ForMonth.HasValue && x.ForMonth.Value.Year == reportMonth.Year &&
                                x.ForMonth.Value.Month < reportMonth.Month && x.TypeReport == (int)EnumTypeBusiness.Trading)
                    .GroupBy(x => x.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(x => x.Key, x => x.Sum(y => y.PerformInPeriod ?? 0), StringComparer.OrdinalIgnoreCase);
                foreach (var row in rows)
                {
                    double value;
                    row.ComparedSamePeriodLastYear = previous.TryGetValue(row.Code ?? string.Empty, out value) ? value : 0;
                }
            }
            if (savedData != null)
            {
                foreach (var row in rows)
                {
                    var saved = savedData.FirstOrDefault(x => string.Equals(x.Code, row.Code, StringComparison.OrdinalIgnoreCase));
                    if (saved == null) continue;
                    row.PerformInPeriod = saved.PerformInPeriod;
                    row.PerformPreviousPeriod = saved.PerformPreviousPeriod;
                    row.AccumulatedBeginingOfYear = saved.AccumulatedBeginingOfYear;
                }

                var dynamicRows = savedData.Where(x => !string.IsNullOrWhiteSpace(x.Code) &&
                                                       x.Code.StartsWith("TM_", StringComparison.OrdinalIgnoreCase))
                    .Select(x => new ReportDataImportModel
                    {
                        Targets = x.Targets,
                        Unit = x.Unit,
                        Code = x.Code,
                        PerformInPeriod = x.PerformInPeriod,
                        PerformPreviousPeriod = x.PerformPreviousPeriod,
                        AccumulatedBeginingOfYear = x.AccumulatedBeginingOfYear,
                        ComparedSamePeriodLastYear = (_importCache.GetViaEnterpriseOnMonth(enterpriseId, null) ?? new List<ReportDataImportModel>())
                            .Where(p => p.ForMonth.HasValue && p.ForMonth.Value.Year == reportMonth.Year && p.ForMonth.Value.Month < reportMonth.Month &&
                                        string.Equals(p.Code, x.Code, StringComparison.OrdinalIgnoreCase))
                            .Sum(p => p.PerformInPeriod ?? 0)
                    }).ToList();
                // Mặt hàng tự nhập thuộc nhóm 11, hiển thị ngay dưới "Hàng hóa khác"
                // và trước hai dòng Bán lẻ/Nền tảng trực tuyến của nhóm này.
                var otherRetailIndex = rows.FindIndex(x => x.Code == "38");
                if (otherRetailIndex < 0) rows.AddRange(dynamicRows);
                else rows.InsertRange(otherRetailIndex, dynamicRows);
            }

            ApplyTradingHierarchy(rows);
            RecalculateTradingSummary(rows);
            ViewBag.ForMonth = reportMonth;
            return PartialView("_TradingReport", rows);
        }

        private static List<ReportDataImportModel> CreateTradingReportLines()
        {
            return new List<ReportDataImportModel>
            {
                CreateBusinessProductLine("I. Doanh thu thuần hoạt động bán buôn, bán lẻ", "Triệu đồng", "01"),
                CreateBusinessProductLine("Trong đó: Bán lẻ", "Triệu đồng", "02"),
                CreateBusinessProductLine("Trong đó: Nền tảng trực tuyến", "Triệu đồng", "03"),
                CreateBusinessProductLine("1. Lương thực, thực phẩm", "Triệu đồng", "04"),
                CreateBusinessProductLine("Trong đó: Bán lẻ", "Triệu đồng", "05"),
                CreateBusinessProductLine("Trong đó: Nền tảng trực tuyến", "Triệu đồng", "06"),
                CreateBusinessProductLine("2. Hàng may mặc", "Triệu đồng", "07"),
                CreateBusinessProductLine("Trong đó: Bán lẻ", "Triệu đồng", "08"),
                CreateBusinessProductLine("Trong đó: Nền tảng trực tuyến", "Triệu đồng", "09"),
                CreateBusinessProductLine("3. Đồ dùng, dụng cụ, trang thiết bị gia đình", "Triệu đồng", "10"),
                CreateBusinessProductLine("Trong đó: Bán lẻ", "Triệu đồng", "11"),
                CreateBusinessProductLine("Trong đó: Nền tảng trực tuyến", "Triệu đồng", "12"),
                CreateBusinessProductLine("4. Văn phòng phẩm, văn hóa, giáo dục", "Triệu đồng", "13"),
                CreateBusinessProductLine("Trong đó: Bán lẻ", "Triệu đồng", "14"),
                CreateBusinessProductLine("Trong đó: Nền tảng trực tuyến", "Triệu đồng", "15"),
                CreateBusinessProductLine("5. Gỗ và vật liệu xây dựng", "Triệu đồng", "16"),
                CreateBusinessProductLine("Trong đó: Bán lẻ", "Triệu đồng", "17"),
                CreateBusinessProductLine("Trong đó: Nền tảng trực tuyến", "Triệu đồng", "18"),
                CreateBusinessProductLine("6. Phân bón, thuốc trừ sâu", "Triệu đồng", "19"),
                CreateBusinessProductLine("Trong đó: Bán lẻ", "Triệu đồng", "20"),
                CreateBusinessProductLine("Trong đó: Nền tảng trực tuyến", "Triệu đồng", "21"),
                CreateBusinessProductLine("7. Ô tô các loại", "Triệu đồng", "22"),
                CreateBusinessProductLine("Trong đó: Bán lẻ", "Triệu đồng", "23"),
                CreateBusinessProductLine("Trong đó: Nền tảng trực tuyến", "Triệu đồng", "24"),
                CreateBusinessProductLine("8. Phương tiện đi lại (trừ ô tô)", "Triệu đồng", "25"),
                CreateBusinessProductLine("Trong đó: Bán lẻ", "Triệu đồng", "26"),
                CreateBusinessProductLine("Trong đó: Nền tảng trực tuyến", "Triệu đồng", "27"),
                CreateBusinessProductLine("9. Xăng, dầu các loại", "Triệu đồng", "28"),
                CreateBusinessProductLine("Trong đó: Bán lẻ", "Triệu đồng", "29"),
                CreateBusinessProductLine("Trong đó: Nền tảng trực tuyến", "Triệu đồng", "30"),
                CreateBusinessProductLine("10. Nhiên liệu khác (trừ xăng dầu)", "Triệu đồng", "31"),
                CreateBusinessProductLine("Trong đó: Bán lẻ", "Triệu đồng", "32"),
                CreateBusinessProductLine("Trong đó: Nền tảng trực tuyến", "Triệu đồng", "33"),
                CreateBusinessProductLine("11. Hàng hóa khác", "Triệu đồng", "34"),
                CreateBusinessProductLine("Trong đó: Bán lẻ", "Triệu đồng", "38"),
                CreateBusinessProductLine("Trong đó: Nền tảng trực tuyến", "Triệu đồng", "39"),
                CreateBusinessProductLine("II. Doanh thu thuần hoạt động sửa chữa ô tô, mô tô, xe máy và xe có động cơ khác", "Triệu đồng", "40")
            };
        }

        private static void ApplyTradingHierarchy(List<ReportDataImportModel> rows)
        {
            var index = 0;
            var otherNumber = 0;
            foreach (var row in rows)
            {
                if (row.Code == "01") row.Level = "1";
                else if (row.Code == "02") row.Level = "1.1";
                else if (row.Code == "03") row.Level = "1.2";
                else if (new[] { "04", "07", "10", "13", "16", "19", "22", "25", "28", "31", "34" }.Contains(row.Code)) row.Level = "1." + ((Convert.ToInt32(row.Code) - 1) / 3 + 2);
                else if (new[] { "05", "08", "11", "14", "17", "20", "23", "26", "29", "32", "38" }.Contains(row.Code)) row.Level = row.Code == "38" ? "1.13.1" : "1." + ((Convert.ToInt32(row.Code) - 2) / 3 + 2) + ".1";
                else if (new[] { "06", "09", "12", "15", "18", "21", "24", "27", "30", "33", "39" }.Contains(row.Code)) row.Level = row.Code == "39" ? "1.13.2" : "1." + ((Convert.ToInt32(row.Code) - 3) / 3 + 2) + ".2";
                else if (row.Code == "40") row.Level = "2";
                else if (!string.IsNullOrWhiteSpace(row.Code) && row.Code.StartsWith("TM_", StringComparison.OrdinalIgnoreCase)) row.Level = "1.13." + (++otherNumber + 2);
                row.Index = index++;
            }
        }

        private static void RecalculateTradingSummary(List<ReportDataImportModel> rows)
        {
            var sums = new[] { new[] { "04", "05", "06" }, new[] { "07", "08", "09" }, new[] { "10", "11", "12" }, new[] { "13", "14", "15" }, new[] { "16", "17", "18" }, new[] { "19", "20", "21" }, new[] { "22", "23", "24" }, new[] { "25", "26", "27" }, new[] { "28", "29", "30" }, new[] { "31", "32", "33" }, new[] { "34", "38", "39" } };
            foreach (var sum in sums)
            {
                var parent = rows.FirstOrDefault(x => x.Code == sum[0]);
                var retail = rows.FirstOrDefault(x => x.Code == sum[1]);
                var online = rows.FirstOrDefault(x => x.Code == sum[2]);
                if (parent == null || retail == null || online == null) continue;
                parent.PerformInPeriod = (retail.PerformInPeriod ?? 0) + (online.PerformInPeriod ?? 0);
                parent.PerformPreviousPeriod = (retail.PerformPreviousPeriod ?? 0) + (online.PerformPreviousPeriod ?? 0);
                parent.AccumulatedBeginingOfYear = (retail.AccumulatedBeginingOfYear ?? 0) + (online.AccumulatedBeginingOfYear ?? 0);
            }
            var otherGoods = rows.FirstOrDefault(x => x.Code == "34");
            var otherDetails = rows.Where(x => !string.IsNullOrWhiteSpace(x.Code) && x.Code.StartsWith("TM_", StringComparison.OrdinalIgnoreCase)).ToList();
            if (otherGoods != null)
            {
                otherGoods.PerformInPeriod = otherDetails.Sum(x => x.PerformInPeriod ?? 0);
                otherGoods.PerformPreviousPeriod = otherDetails.Sum(x => x.PerformPreviousPeriod ?? 0);
                otherGoods.AccumulatedBeginingOfYear = otherDetails.Sum(x => x.AccumulatedBeginingOfYear ?? 0);
            }
            var totalRetail = rows.FirstOrDefault(x => x.Code == "02");
            var totalOnline = rows.FirstOrDefault(x => x.Code == "03");
            var retailRows = rows.Where(x => new[] { "05", "08", "11", "14", "17", "20", "23", "26", "29", "32", "38" }.Contains(x.Code)).ToList();
            var onlineRows = rows.Where(x => new[] { "06", "09", "12", "15", "18", "21", "24", "27", "30", "33", "39" }.Contains(x.Code)).ToList();
            if (totalRetail != null) { totalRetail.PerformInPeriod = retailRows.Sum(x => x.PerformInPeriod ?? 0); totalRetail.PerformPreviousPeriod = retailRows.Sum(x => x.PerformPreviousPeriod ?? 0); totalRetail.AccumulatedBeginingOfYear = retailRows.Sum(x => x.AccumulatedBeginingOfYear ?? 0); }
            if (totalOnline != null) { totalOnline.PerformInPeriod = onlineRows.Sum(x => x.PerformInPeriod ?? 0); totalOnline.PerformPreviousPeriod = onlineRows.Sum(x => x.PerformPreviousPeriod ?? 0); totalOnline.AccumulatedBeginingOfYear = onlineRows.Sum(x => x.AccumulatedBeginingOfYear ?? 0); }
            var total = rows.FirstOrDefault(x => x.Code == "01");
            if (total != null && totalRetail != null && totalOnline != null) { total.PerformInPeriod = (totalRetail.PerformInPeriod ?? 0) + (totalOnline.PerformInPeriod ?? 0); total.PerformPreviousPeriod = (totalRetail.PerformPreviousPeriod ?? 0) + (totalOnline.PerformPreviousPeriod ?? 0); total.AccumulatedBeginingOfYear = (totalRetail.AccumulatedBeginingOfYear ?? 0) + (totalOnline.AccumulatedBeginingOfYear ?? 0); }
        }

        private PartialViewResult LoadImportExportReport(int? enterpriseId, DateTime? onMonth, string forMonth = null,
            List<ReportDataImportModel> savedData = null)
        {
            DateTime reportMonth;
            if (!onMonth.HasValue && !string.IsNullOrWhiteSpace(forMonth) &&
                DateTime.TryParse(forMonth, out reportMonth))
                onMonth = reportMonth;
            reportMonth = onMonth ?? DateTime.Now;

            var rows = CreateImportExportReportLines(enterpriseId);

            if (savedData != null && savedData.Any())
            {
                foreach (var row in rows)
                {
                    var saved = savedData.FirstOrDefault(x => string.Equals(x.Code, row.Code, StringComparison.OrdinalIgnoreCase));
                    if (saved == null) continue;
                    row.PerformPreviousPeriod = saved.PerformPreviousPeriod;
                    row.ComparedSamePeriodLastYear = saved.ComparedSamePeriodLastYear;
                    row.PerformInPeriod = saved.PerformInPeriod;
                    row.AccumulatedBeginingOfYear = saved.AccumulatedBeginingOfYear;
                }

                var dynamicCountries = savedData.Where(x => !string.IsNullOrWhiteSpace(x.Code) &&
                                                            x.Code.StartsWith("XK_QG_", StringComparison.OrdinalIgnoreCase) &&
                                                            !string.Equals(x.Code, "XK_QG_HEADER", StringComparison.OrdinalIgnoreCase))
                    .Select(x => new ReportDataImportModel
                    {
                        Targets = x.Targets,
                        Unit = x.Unit ?? "USD",
                        Code = x.Code,
                        Level = x.Level,
                        Index = x.Index,
                        PerformPreviousPeriod = x.PerformPreviousPeriod,
                        ComparedSamePeriodLastYear = x.ComparedSamePeriodLastYear,
                        PerformInPeriod = x.PerformInPeriod,
                        AccumulatedBeginingOfYear = x.AccumulatedBeginingOfYear
                    }).ToList();

                var xkMhHeaderIndex = rows.FindIndex(x => string.Equals(x.Code, "XK_MH_HEADER", StringComparison.OrdinalIgnoreCase));
                if (xkMhHeaderIndex >= 0)
                {
                    rows.InsertRange(xkMhHeaderIndex, dynamicCountries);
                }
                else
                {
                    rows.AddRange(dynamicCountries);
                }

                var dynamicProducts = savedData.Where(x => !string.IsNullOrWhiteSpace(x.Code) &&
                                                           x.Code.StartsWith("XK_MH_", StringComparison.OrdinalIgnoreCase) &&
                                                           !string.Equals(x.Code, "XK_MH_HEADER", StringComparison.OrdinalIgnoreCase))
                    .Select(x => new ReportDataImportModel
                    {
                        Targets = x.Targets,
                        Unit = x.Unit ?? "USD",
                        Code = x.Code,
                        Level = x.Level,
                        Index = x.Index,
                        PerformPreviousPeriod = x.PerformPreviousPeriod,
                        ComparedSamePeriodLastYear = x.ComparedSamePeriodLastYear,
                        PerformInPeriod = x.PerformInPeriod,
                        AccumulatedBeginingOfYear = x.AccumulatedBeginingOfYear
                    }).ToList();

                var utXkIndex = rows.FindIndex(x => string.Equals(x.Code, "UT_XK", StringComparison.OrdinalIgnoreCase));
                if (utXkIndex >= 0)
                {
                    rows.InsertRange(utXkIndex, dynamicProducts);
                }
                else
                {
                    rows.AddRange(dynamicProducts);
                }
            }

            ApplyImportExportHierarchy(rows);
            RecalculateImportExportSummary(rows);

            ViewBag.ForMonth = reportMonth;
            ViewBag.ImportExportEnterpriseId = enterpriseId ?? 0;
            ViewBag.ListNationals = new List<CateNationalModel>();
            return PartialView("_ImportExportReport", rows);
        }

        private List<ReportDataImportModel> CreateImportExportReportLines(int? enterpriseId)
        {
            var lines = new List<ReportDataImportModel>
            {
                CreateBusinessProductLine("TỔNG GIÁ TRỊ (FOB) = I + II", "USD", "FOB"),
                CreateBusinessProductLine("I.Tổng trị giá xuất khẩu trực tiếp", "USD", "XK_TT"),
                new ReportDataImportModel { Targets = "Chia theo nước cuối cùng hàng đến", Code = "XK_QG_HEADER" },
                new ReportDataImportModel { Targets = "Mặt hàng xuất khẩu trực tiếp chia theo nước cuối cùng hàng đến", Code = "XK_MH_HEADER" },
                CreateBusinessProductLine("II. Trị giá ủy thác xuất khẩu", "USD", "UT_XK"),
                new ReportDataImportModel { Targets = "Mặt hàng ủy thác xuất khẩu", Code = "UT_MH_HEADER" }
            };

            if (enterpriseId.HasValue && enterpriseId.Value > 0)
            {
                try
                {
                    var mainProducts = (_businessProductCache.GetViaEnterprise(enterpriseId.Value) ?? new List<CateBusinessProductModel>())
                        .Select(product => new ReportDataImportModel
                        {
                            Targets = product.ProductName,
                            Unit = product.Unit,
                            Code = !string.IsNullOrWhiteSpace(product.ProductCode)
                                ? product.ProductCode.Trim()
                                : string.Format("02{0:D4}", product.ProductId)
                        }).ToList();

                    lines.AddRange(mainProducts);
                }
                catch (Exception ex)
                {
                    AppProcessor.Logger.Message("CreateImportExportReportLines GetViaEnterprise error: " + ex.Message);
                }
            }

            return lines;
        }

        private static void ApplyImportExportHierarchy(List<ReportDataImportModel> rows)
        {
            var index = 0;
            var countryNumber = 0;
            var productNumber = 0;
            var trustNumber = 0;
            var inTrustSection = false;

            foreach (var row in rows)
            {
                if (string.Equals(row.Code, "FOB", StringComparison.OrdinalIgnoreCase))
                {
                    row.Level = "0";
                }
                else if (string.Equals(row.Code, "XK_TT", StringComparison.OrdinalIgnoreCase))
                {
                    row.Level = "1";
                }
                else if (string.Equals(row.Code, "XK_QG_HEADER", StringComparison.OrdinalIgnoreCase))
                {
                    row.Level = "1.1";
                }
                else if (!string.IsNullOrWhiteSpace(row.Code) && row.Code.StartsWith("XK_QG_", StringComparison.OrdinalIgnoreCase))
                {
                    row.Level = "1.1." + (++countryNumber);
                }
                else if (string.Equals(row.Code, "XK_MH_HEADER", StringComparison.OrdinalIgnoreCase))
                {
                    row.Level = "1.2";
                }
                else if (!string.IsNullOrWhiteSpace(row.Code) && row.Code.StartsWith("XK_MH_", StringComparison.OrdinalIgnoreCase))
                {
                    row.Level = "1.2." + (++productNumber);
                }
                else if (string.Equals(row.Code, "UT_XK", StringComparison.OrdinalIgnoreCase))
                {
                    row.Level = "2";
                }
                else if (string.Equals(row.Code, "UT_MH_HEADER", StringComparison.OrdinalIgnoreCase))
                {
                    row.Level = "2.1";
                    inTrustSection = true;
                }
                else if (inTrustSection && !string.IsNullOrWhiteSpace(row.Code))
                {
                    row.Level = "2.1." + (++trustNumber);
                }

                row.Index = index++;
            }
        }

        private static void RecalculateImportExportSummary(List<ReportDataImportModel> rows)
        {
            var countryRows = rows.Where(x => !string.IsNullOrWhiteSpace(x.Code) &&
                                              x.Code.StartsWith("XK_QG_", StringComparison.OrdinalIgnoreCase) &&
                                              !string.Equals(x.Code, "XK_QG_HEADER", StringComparison.OrdinalIgnoreCase)).ToList();

            var utHeaderIdx = rows.FindIndex(x => string.Equals(x.Code, "UT_MH_HEADER", StringComparison.OrdinalIgnoreCase));
            var trustRows = utHeaderIdx >= 0
                ? rows.Skip(utHeaderIdx + 1).Where(x => !string.IsNullOrWhiteSpace(x.Code)).ToList()
                : new List<ReportDataImportModel>();

            var xkTtRow = rows.FirstOrDefault(x => string.Equals(x.Code, "XK_TT", StringComparison.OrdinalIgnoreCase));
            if (xkTtRow != null)
            {
                xkTtRow.PerformPreviousPeriod = countryRows.Sum(x => x.PerformPreviousPeriod ?? 0);
                xkTtRow.ComparedSamePeriodLastYear = countryRows.Sum(x => x.ComparedSamePeriodLastYear ?? 0);
                xkTtRow.PerformInPeriod = countryRows.Sum(x => x.PerformInPeriod ?? 0);
                xkTtRow.AccumulatedBeginingOfYear = countryRows.Sum(x => x.AccumulatedBeginingOfYear ?? 0);
            }

            var utXkRow = rows.FirstOrDefault(x => string.Equals(x.Code, "UT_XK", StringComparison.OrdinalIgnoreCase));
            if (utXkRow != null)
            {
                utXkRow.PerformPreviousPeriod = trustRows.Sum(x => x.PerformPreviousPeriod ?? 0);
                utXkRow.ComparedSamePeriodLastYear = trustRows.Sum(x => x.ComparedSamePeriodLastYear ?? 0);
                utXkRow.PerformInPeriod = trustRows.Sum(x => x.PerformInPeriod ?? 0);
                utXkRow.AccumulatedBeginingOfYear = trustRows.Sum(x => x.AccumulatedBeginingOfYear ?? 0);
            }

            var fobRow = rows.FirstOrDefault(x => string.Equals(x.Code, "FOB", StringComparison.OrdinalIgnoreCase));
            if (fobRow != null && xkTtRow != null && utXkRow != null)
            {
                fobRow.PerformPreviousPeriod = (xkTtRow.PerformPreviousPeriod ?? 0) + (utXkRow.PerformPreviousPeriod ?? 0);
                fobRow.ComparedSamePeriodLastYear = (xkTtRow.ComparedSamePeriodLastYear ?? 0) + (utXkRow.ComparedSamePeriodLastYear ?? 0);
                fobRow.PerformInPeriod = (xkTtRow.PerformInPeriod ?? 0) + (utXkRow.PerformInPeriod ?? 0);
                fobRow.AccumulatedBeginingOfYear = (xkTtRow.AccumulatedBeginingOfYear ?? 0) + (utXkRow.AccumulatedBeginingOfYear ?? 0);
            }
        }

        private static ReportDataImportModel CreateBusinessProductHeader(string targets)
        {
            return new ReportDataImportModel { Targets = targets };
        }

        private static ReportDataImportModel CreateBusinessProductLine(string targets, string unit, string code)
        {
            return new ReportDataImportModel { Targets = targets, Unit = unit, Code = code };
        }

        private static void ApplyBusinessProductHierarchy(List<ReportDataImportModel> products)
        {
            var storageIndex = 0;
            var section = string.Empty;
            var mainProductNumber = 0;
            var exportProductNumber = 0;
            var importProductNumber = 0;

            foreach (var product in products)
            {
                product.Index = null;
                product.Level = null;

                switch (product.Targets)
                {
                    case "Tổng doanh thu":
                        product.Level = "1";
                        break;
                    case "Trong đó doanh thu công nghiệp":
                        product.Level = "1.1";
                        break;
                    case "Sản phẩm công nghiệp chủ yếu":
                        section = "MainProduct";
                        product.Level = "2";
                        continue;
                    case "Lao động - Thu nhập":
                        section = "Labor";
                        product.Level = "3";
                        continue;
                    case "Tổng số lao động":
                        product.Level = "3.1";
                        break;
                    case "Thu nhập bình quân/người/tháng":
                        product.Level = "3.2";
                        break;
                    case "Nộp ngân sách":
                        section = string.Empty;
                        product.Level = "5";
                        break;
                    case "Kim ngạch xuất khẩu":
                        section = "Export";
                        product.Level = "6";
                        break;
                    case "Nhóm/mặt hàng xuất khẩu chủ yếu":
                        section = "Export";
                        continue;
                    case "Kim ngạch nhập khẩu":
                        section = "Import";
                        product.Level = "7";
                        break;
                    case "Nhóm/mặt hàng nhập khẩu chủ yếu":
                        section = "Import";
                        continue;
                }

                if (string.IsNullOrWhiteSpace(product.Code))
                    continue;

                if (section == "MainProduct" && string.IsNullOrWhiteSpace(product.Level))
                    product.Level = "2." + (++mainProductNumber);
                else if (section == "Export" && product.Code.StartsWith("XK", StringComparison.OrdinalIgnoreCase))
                    product.Level = "6." + (++exportProductNumber);
                else if (section == "Import" && product.Code.StartsWith("NK", StringComparison.OrdinalIgnoreCase))
                    product.Level = "7." + (++importProductNumber);

                product.Index = storageIndex++;
            }
        }

        private static List<ReportDataImportModel> MergeBusinessProductData(
            List<ReportDataImportModel> catalog, List<ReportDataImportModel> savedData)
        {
            savedData = savedData ?? new List<ReportDataImportModel>();
            foreach (var item in catalog.Where(x => !string.IsNullOrWhiteSpace(x.Code)))
            {
                // Kim ngạch XK và NK không nạp trực tiếp, chỉ tính từ tổng các mặt hàng bên trong
                if (item.Code == "06" || item.Code == "07" || item.Targets == "Kim ngạch xuất khẩu" || item.Targets == "Kim ngạch nhập khẩu")
                    continue;

                // Mã sản phẩm có thể trùng giữa ba nhóm danh mục, nên ưu tiên tên và đơn vị.
                var saved = savedData.FirstOrDefault(x =>
                                string.Equals(x.Targets, item.Targets, StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(x.Unit, item.Unit, StringComparison.OrdinalIgnoreCase))
                            ?? savedData.FirstOrDefault(x =>
                                !string.IsNullOrWhiteSpace(x.Code) &&
                                string.Equals(x.Code.Trim(), item.Code.Trim(), StringComparison.OrdinalIgnoreCase));

                if (saved == null)
                    continue;

                item.PerformPreviousPeriod = saved.PerformPreviousPeriod;
                item.PerformInPeriod = saved.PerformInPeriod;
                item.AccumulatedBeginingOfYear = saved.AccumulatedBeginingOfYear ?? item.AccumulatedBeginingOfYear;
                item.ComparedSamePeriodLastYear = saved.ComparedSamePeriodLastYear;
            }

            InsertTradeIndicators(catalog, savedData, "Nhóm/mặt hàng xuất khẩu chủ yếu", "XK");
            InsertTradeIndicators(catalog, savedData, "Nhóm/mặt hàng nhập khẩu chủ yếu", "NK");
            ApplyBusinessProductHierarchy(catalog);
            RecalculateTradeSummaryInModel(catalog);

            return catalog;
        }

        private static void RecalculateTradeSummaryInModel(List<ReportDataImportModel> products)
        {
            if (products == null) return;
            var exportSummary = products.FirstOrDefault(p => string.Equals(p.Code, "06", StringComparison.OrdinalIgnoreCase) || string.Equals(p.Targets, "Kim ngạch xuất khẩu", StringComparison.OrdinalIgnoreCase));
            if (exportSummary != null)
            {
                var xkItems = products.Where(p => !string.IsNullOrWhiteSpace(p.Code) && p.Code.StartsWith("XK", StringComparison.OrdinalIgnoreCase)).ToList();
                exportSummary.AccumulatedBeginingOfYear = xkItems.Any() ? xkItems.Sum(x => x.AccumulatedBeginingOfYear ?? 0) : (double?)0;
                exportSummary.PerformPreviousPeriod = xkItems.Any() ? xkItems.Sum(x => x.PerformPreviousPeriod ?? 0) : (double?)0;
                exportSummary.PerformInPeriod = xkItems.Any() ? xkItems.Sum(x => x.PerformInPeriod ?? 0) : (double?)0;
            }

            var importSummary = products.FirstOrDefault(p => string.Equals(p.Code, "07", StringComparison.OrdinalIgnoreCase) || string.Equals(p.Targets, "Kim ngạch nhập khẩu", StringComparison.OrdinalIgnoreCase));
            if (importSummary != null)
            {
                var nkItems = products.Where(p => !string.IsNullOrWhiteSpace(p.Code) && p.Code.StartsWith("NK", StringComparison.OrdinalIgnoreCase)).ToList();
                importSummary.AccumulatedBeginingOfYear = nkItems.Any() ? nkItems.Sum(x => x.AccumulatedBeginingOfYear ?? 0) : (double?)0;
                importSummary.PerformPreviousPeriod = nkItems.Any() ? nkItems.Sum(x => x.PerformPreviousPeriod ?? 0) : (double?)0;
                importSummary.PerformInPeriod = nkItems.Any() ? nkItems.Sum(x => x.PerformInPeriod ?? 0) : (double?)0;
            }
        }

        private static void InsertTradeIndicators(List<ReportDataImportModel> catalog,
            List<ReportDataImportModel> savedData, string header, string codePrefix)
        {
            var configuredCodes = new HashSet<string>(catalog
                .Where(x => !string.IsNullOrWhiteSpace(x.Code) &&
                            x.Code.StartsWith(codePrefix, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Code.Trim()), StringComparer.OrdinalIgnoreCase);
            var rows = savedData.Where(x => !string.IsNullOrWhiteSpace(x.Code) &&
                                            x.Code.StartsWith(codePrefix, StringComparison.OrdinalIgnoreCase) &&
                                            !configuredCodes.Contains(x.Code.Trim())).ToList();
            if (!rows.Any()) return;
            var headerIndex = catalog.FindIndex(x => x.Targets == header && string.IsNullOrWhiteSpace(x.Code));
            if (headerIndex >= 0) catalog.InsertRange(headerIndex + 1, rows);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult AddNational()
        {
            var lstNational = _nationalCache.GetAll();
            return PartialView("_AddNational", lstNational);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult UploadSignedFile()
        {
            return PartialView("_UploadSignedFile", new ReportDataImportModel());
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult UploadBaseFile()
        {
            var serialKeyVNPTToken = ConfigurationManager.AppSettings["VNPT_Token_Serial"];
            return PartialView("_UploadBaseFile", new ReportDataImportModel { VNPTTokenSerialKey = serialKeyVNPTToken });
        }

        #endregion

        #region Extend Function

        private DataTable ReadDataImports(HttpPostedFileBase fileImport, out bool isSucess)
        {
            isSucess = true;

            #region Init Datatable

            var dataImport = new DataTable();
            dataImport.Columns.Add("RowIndex", typeof(long));
            dataImport.Columns.Add("Targets");
            dataImport.Columns.Add("Unit");
            dataImport.Columns.Add("Code");
            dataImport.Columns.Add("PerformPreviousPeriod", typeof(double));
            dataImport.Columns.Add("PerformInPeriod", typeof(double));
            dataImport.Columns.Add("AccumulatedBeginingOfYear", typeof(double));
            dataImport.Columns.Add("ComparedSamePeriodLastYear", typeof(double));

            #endregion

            try
            {
                var fileExtension = Path.GetExtension(fileImport.FileName);

                if (fileExtension != ".xls" && fileExtension != ".xlsx") return null;
                var excelReader = fileExtension == ".xls"
                    ? ExcelReaderFactory.CreateBinaryReader(fileImport.InputStream)
                    : ExcelReaderFactory.CreateOpenXmlReader(fileImport.InputStream);

                var ds = excelReader.AsDataSet();

                var dt = ds.Tables[0];
                dt.Rows.RemoveAt(0); // Title Column
                dt.Rows.RemoveAt(0); // Code Column
                int rowIdx = 0;

                foreach (DataRow row in dt.Rows)
                {
                    var dataCells = (row.ItemArray.Length > 5
                        ? row.ItemArray.ToList().Where((item, idx) => idx < 5).Select((c, idx) =>
                            idx >= 3 ? string.IsNullOrEmpty(c.ToString()) ? null : c : c).ToArray()
                        : row.ItemArray.Select((c, idx) => idx >= 3 ? string.IsNullOrEmpty(c.ToString()) ? null : c : c)
                            .ToArray()).ToList();
                    //dataCells.Insert(3, null);
                    dataCells.Insert(0, rowIdx);
                    dataCells.Insert(6, null);
                    dataImport.Rows.Add(dataCells.ToArray());
                    rowIdx += 1;
                }

                return dataImport;
            }
            catch (Exception e)
            {
                AppProcessor.Logger.Error(e);
                isSucess = false;
                return dataImport;
            }
        }

        private DataTable ReadDataImports(Stream fileStream, string fileExtension, out bool isSucess)
        {
            isSucess = true;

            #region Init Datatable

            var dataImport = new DataTable();
            dataImport.Columns.Add("RowIndex", typeof(long));
            dataImport.Columns.Add("Targets");
            dataImport.Columns.Add("Unit");
            dataImport.Columns.Add("Code");
            dataImport.Columns.Add("PerformPreviousPeriod", typeof(double));
            dataImport.Columns.Add("PerformInPeriod", typeof(double));
            dataImport.Columns.Add("AccumulatedBeginingOfYear", typeof(double));
            dataImport.Columns.Add("ComparedSamePeriodLastYear", typeof(double));

            #endregion

            try
            {
                if (fileExtension != "xls" && fileExtension != "xlsx")
                {
                    isSucess = false;
                    return null;
                }
                var excelReader = fileExtension == "xls"
                    ? ExcelReaderFactory.CreateBinaryReader(fileStream)
                    : ExcelReaderFactory.CreateOpenXmlReader(fileStream);

                var ds = excelReader.AsDataSet();

                var dt = ds.Tables[0];
                dt.Rows.RemoveAt(0); // Title Column
                dt.Rows.RemoveAt(0); // Code Column
                int rowIdx = 0;

                foreach (DataRow row in dt.Rows)
                {
                    var dataCells = (row.ItemArray.Length > 5
                        ? row.ItemArray.ToList().Where((item, idx) => idx < 5).Select((c, idx) =>
                            idx >= 3 ? string.IsNullOrEmpty(c.ToString()) ? null : c : c).ToArray()
                        : row.ItemArray.Select((c, idx) => idx >= 3 ? string.IsNullOrEmpty(c.ToString()) ? null : c : c)
                            .ToArray()).ToList();
                    //dataCells.Insert(3, null);
                    dataCells.Insert(0, rowIdx);
                    dataCells.Insert(6, null);
                    dataImport.Rows.Add(dataCells.ToArray());
                    rowIdx += 1;
                }

                return dataImport;
            }
            catch (Exception e)
            {
                AppProcessor.Logger.Error(e);
                isSucess = false;
                return dataImport;
            }
        }

        private List<ReportDataImportModel> CheckDataImport(DataTable dataImportModels, DateTime? onMonth,
            int? enterpriseId)
        {
            var dataImportChecked = _importCache.CheckDataImport(new ReportDataImportModel
            {
                EnterpriseId = enterpriseId,
                ForMonth = onMonth,
                DataImport = dataImportModels
            });

            return dataImportChecked;
        }

        private DataTable ReadFormData(NameValueCollection formData)
        {
            #region Init Datatable

            var dartaFormReport = new DataTable();
            dartaFormReport.Columns.Add("Targets");
            dartaFormReport.Columns.Add("Unit");
            dartaFormReport.Columns.Add("Code");
            dartaFormReport.Columns.Add("Index", typeof(int));
            dartaFormReport.Columns.Add("Level");
            dartaFormReport.Columns.Add("PerformPreviousPeriod", typeof(double));
            dartaFormReport.Columns.Add("PerformInPeriod", typeof(double));
            dartaFormReport.Columns.Add("AccumulatedBeginingOfYear", typeof(double));
            dartaFormReport.Columns.Add("ComparedSamePeriodLastYear", typeof(double));

            #endregion

            var targetKeys = formData.AllKeys
                .Where(k => k != null && k.StartsWith("Target_"))
                .Select(k =>
                {
                    int id;
                    return int.TryParse(k.Substring("Target_".Length), out id) ? id : -1;
                })
                .Where(id => id > 0)
                .OrderBy(id => id)
                .ToList();

            var isBusinessProductReport = string.Equals(formData["BusinessProductReport"], "true",
                StringComparison.OrdinalIgnoreCase);
            var isTradingReport = string.Equals(formData["TradingReport"], "true", StringComparison.OrdinalIgnoreCase);
            var isImportExportReport = string.Equals(formData["ImportExportReport"], "true", StringComparison.OrdinalIgnoreCase);
            var dataIndex = 0;
            foreach (var idx in targetKeys)
            {
                var code = formData[$"Code_{idx}"];
                var target = formData[$"Target_{idx}"];
                if ((isBusinessProductReport || isTradingReport || isImportExportReport) && string.IsNullOrWhiteSpace(code)) continue;
                if (isImportExportReport && string.IsNullOrWhiteSpace(target)) continue;

                dartaFormReport.Rows.Add(
                    target,
                    formData[$"Unit_{idx}"],
                    code,
                    dataIndex++,
                    formData[$"Level_{idx}"],
                    ParseFormattedNumber(formData[$"PerformPreviousPeriod_{idx}"]),
                    ParseFormattedNumber(formData[$"PerformInPeriod_{idx}"]),
                    ParseFormattedNumber(formData[$"AccumulatedBeginingOfYear_{idx}"]),
                    ParseFormattedNumber(formData[$"CompareSamePeriodLastYear_{idx}"]));
            }

            // Dòng mặt hàng được thêm động có thể nằm giữa bảng nhưng có tên field ở cuối form.
            // Sắp lại Index theo Level để thứ tự lưu luôn khớp với số thứ tự hiển thị (1, 1.1, 2.1...).
            if (isBusinessProductReport)
            {
                RecalculateTradeSummaryInDataTable(dartaFormReport);

                var orderedRows = dartaFormReport.AsEnumerable()
                    .OrderBy(row => GetBusinessProductSortKey(Convert.ToString(row["Level"])))
                    .ToList();
                for (var rowIndex = 0; rowIndex < orderedRows.Count; rowIndex++)
                    orderedRows[rowIndex]["Index"] = rowIndex;
            }
            else if (isTradingReport)
            {
                RecalculateTradingSummaryInDataTable(dartaFormReport);
                var rows = dartaFormReport.AsEnumerable().ToList();
                var index = 0;
                var child = 0;
                foreach (var row in rows)
                {
                    var code = Convert.ToString(row["Code"]);
                    if (code == "01") row["Level"] = "1";
                    else if (code == "02") row["Level"] = "1.1";
                    else if (code == "03") row["Level"] = "1.2";
                    else if (new[] { "04", "07", "10", "13", "16", "19", "22", "25", "28", "31", "34" }.Contains(code)) row["Level"] = "1." + ((Convert.ToInt32(code) - 1) / 3 + 2);
                    else if (new[] { "05", "08", "11", "14", "17", "20", "23", "26", "29", "32", "38" }.Contains(code)) row["Level"] = code == "38" ? "1.13.1" : "1." + ((Convert.ToInt32(code) - 2) / 3 + 2) + ".1";
                    else if (new[] { "06", "09", "12", "15", "18", "21", "24", "27", "30", "33", "39" }.Contains(code)) row["Level"] = code == "39" ? "1.13.2" : "1." + ((Convert.ToInt32(code) - 3) / 3 + 2) + ".2";
                    else if (code == "40") row["Level"] = "2";
                    else if ((code ?? "").StartsWith("TM_", StringComparison.OrdinalIgnoreCase)) row["Level"] = "1.13." + (++child + 2);
                    row["Index"] = index++;
                }
            }
            else if (isImportExportReport)
            {
                RecalculateImportExportSummaryInDataTable(dartaFormReport);
                var orderedRows = dartaFormReport.AsEnumerable()
                    .OrderBy(row => GetBusinessProductSortKey(Convert.ToString(row["Level"])))
                    .ToList();

                var countryNumber = 0;
                var productNumber = 0;
                var trustNumber = 0;
                var inTrustSection = false;

                for (var rowIndex = 0; rowIndex < orderedRows.Count; rowIndex++)
                {
                    var row = orderedRows[rowIndex];
                    var code = Convert.ToString(row["Code"]);
                    if (string.Equals(code, "FOB", StringComparison.OrdinalIgnoreCase))
                    {
                        row["Level"] = "0";
                    }
                    else if (string.Equals(code, "XK_TT", StringComparison.OrdinalIgnoreCase))
                    {
                        row["Level"] = "1";
                    }
                    else if (string.Equals(code, "XK_QG_HEADER", StringComparison.OrdinalIgnoreCase))
                    {
                        row["Level"] = "1.1";
                    }
                    else if (!string.IsNullOrWhiteSpace(code) && code.StartsWith("XK_QG_", StringComparison.OrdinalIgnoreCase))
                    {
                        row["Level"] = "1.1." + (++countryNumber);
                    }
                    else if (string.Equals(code, "XK_MH_HEADER", StringComparison.OrdinalIgnoreCase))
                    {
                        row["Level"] = "1.2";
                    }
                    else if (!string.IsNullOrWhiteSpace(code) && code.StartsWith("XK_MH_", StringComparison.OrdinalIgnoreCase))
                    {
                        row["Level"] = "1.2." + (++productNumber);
                    }
                    else if (string.Equals(code, "UT_XK", StringComparison.OrdinalIgnoreCase))
                    {
                        row["Level"] = "2";
                    }
                    else if (string.Equals(code, "UT_MH_HEADER", StringComparison.OrdinalIgnoreCase))
                    {
                        row["Level"] = "2.1";
                        inTrustSection = true;
                    }
                    else if (inTrustSection && !string.IsNullOrWhiteSpace(code))
                    {
                        row["Level"] = "2.1." + (++trustNumber);
                    }

                    row["Index"] = rowIndex;
                }

                var sortedTable = dartaFormReport.Clone();
                foreach (var r in orderedRows)
                {
                    sortedTable.ImportRow(r);
                }
                return sortedTable;
            }

            return dartaFormReport;
        }

        private static double? ParseFormattedNumber(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            raw = raw.Trim();
            if (raw.Contains(",") && raw.Contains("."))
            {
                raw = raw.Replace(".", "").Replace(",", ".");
            }
            else if (raw.Contains(","))
            {
                raw = raw.Replace(",", ".");
            }
            else if (raw.Contains("."))
            {
                var dotCount = raw.Count(c => c == '.');
                if (dotCount > 1)
                {
                    raw = raw.Replace(".", "");
                }
                else
                {
                    var parts = raw.Split('.');
                    if (parts.Length > 1 && parts[1].Length == 3)
                    {
                        raw = raw.Replace(".", "");
                    }
                }
            }

            double result;
            if (double.TryParse(raw, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return null;
        }

        private static string GetBusinessProductSortKey(string level)
        {
            if (string.IsNullOrWhiteSpace(level)) return "99999";
            return string.Join(".", level.Split('.')
                .Select(part =>
                {
                    int number;
                    return int.TryParse(part, out number) ? number.ToString("D5") : "99999";
                }));
        }

        private static string GetTradeCatalogCodePrefix(string prefix)
        {
            if (string.Equals(prefix, "XK", StringComparison.OrdinalIgnoreCase)) return "XKB";
            if (string.Equals(prefix, "NK", StringComparison.OrdinalIgnoreCase)) return "NKB";
            return null;
        }

        private bool IsReportLocked(DateTime? forMonth)
        {
            if (!forMonth.HasValue) return false;

            int lockDay;
            if (!int.TryParse(_configCache.GetViaKey("Day_Deadline_Send_Late_Report")?.ConfigValue,
                    out lockDay) || lockDay <= 0)
                return false;

            var lastDayOfMonth = DateTime.DaysInMonth(forMonth.Value.Year, forMonth.Value.Month);
            var lockDate = new DateTime(forMonth.Value.Year, forMonth.Value.Month,
                Math.Min(lockDay, lastDayOfMonth));
            return DateTime.Today > lockDate;
        }

        private static string GetReportLockedMessage(DateTime? forMonth)
        {
            return $"Báo cáo tháng {forMonth?.ToString("MM/yyyy")} đã bị khóa, không thể sửa hoặc xóa thông tin.";
        }

        private static void RecalculateTradeSummaryInDataTable(DataTable table)
        {
            if (table == null) return;
            var rows = table.AsEnumerable().ToList();
            var exportRow = rows.FirstOrDefault(r => string.Equals(Convert.ToString(r["Code"]), "06", StringComparison.OrdinalIgnoreCase) || string.Equals(Convert.ToString(r["Targets"]), "Kim ngạch xuất khẩu", StringComparison.OrdinalIgnoreCase));
            if (exportRow != null)
            {
                var xkRows = rows.Where(r => (Convert.ToString(r["Code"]) ?? "").StartsWith("XK", StringComparison.OrdinalIgnoreCase)).ToList();
                exportRow["AccumulatedBeginingOfYear"] = xkRows.Any() ? xkRows.Sum(r => r["AccumulatedBeginingOfYear"] != DBNull.Value ? Convert.ToDouble(r["AccumulatedBeginingOfYear"]) : 0.0) : 0.0;
                exportRow["PerformPreviousPeriod"] = xkRows.Any() ? xkRows.Sum(r => r["PerformPreviousPeriod"] != DBNull.Value ? Convert.ToDouble(r["PerformPreviousPeriod"]) : 0.0) : 0.0;
                exportRow["PerformInPeriod"] = xkRows.Any() ? xkRows.Sum(r => r["PerformInPeriod"] != DBNull.Value ? Convert.ToDouble(r["PerformInPeriod"]) : 0.0) : 0.0;
            }

            var importRow = rows.FirstOrDefault(r => string.Equals(Convert.ToString(r["Code"]), "07", StringComparison.OrdinalIgnoreCase) || string.Equals(Convert.ToString(r["Targets"]), "Kim ngạch nhập khẩu", StringComparison.OrdinalIgnoreCase));
            if (importRow != null)
            {
                var nkRows = rows.Where(r => (Convert.ToString(r["Code"]) ?? "").StartsWith("NK", StringComparison.OrdinalIgnoreCase)).ToList();
                importRow["AccumulatedBeginingOfYear"] = nkRows.Any() ? nkRows.Sum(r => r["AccumulatedBeginingOfYear"] != DBNull.Value ? Convert.ToDouble(r["AccumulatedBeginingOfYear"]) : 0.0) : 0.0;
                importRow["PerformPreviousPeriod"] = nkRows.Any() ? nkRows.Sum(r => r["PerformPreviousPeriod"] != DBNull.Value ? Convert.ToDouble(r["PerformPreviousPeriod"]) : 0.0) : 0.0;
                importRow["PerformInPeriod"] = nkRows.Any() ? nkRows.Sum(r => r["PerformInPeriod"] != DBNull.Value ? Convert.ToDouble(r["PerformInPeriod"]) : 0.0) : 0.0;
            }
        }

        private static void RecalculateTradingSummaryInDataTable(DataTable table)
        {
            if (table == null) return;
            var rows = table.AsEnumerable().ToList();
            Func<DataRow, string, double> value = (row, field) => row[field] == DBNull.Value ? 0 : Convert.ToDouble(row[field]);
            Action<DataRow, DataRow, DataRow> setSum = (parent, retail, online) => { if (parent == null || retail == null || online == null) return; foreach (var field in new[] { "PerformInPeriod", "PerformPreviousPeriod", "AccumulatedBeginingOfYear" }) parent[field] = value(retail, field) + value(online, field); };
            foreach (var codes in new[] { new[] { "04", "05", "06" }, new[] { "07", "08", "09" }, new[] { "10", "11", "12" }, new[] { "13", "14", "15" }, new[] { "16", "17", "18" }, new[] { "19", "20", "21" }, new[] { "22", "23", "24" }, new[] { "25", "26", "27" }, new[] { "28", "29", "30" }, new[] { "31", "32", "33" }, new[] { "34", "38", "39" } }) setSum(rows.FirstOrDefault(x => Convert.ToString(x["Code"]) == codes[0]), rows.FirstOrDefault(x => Convert.ToString(x["Code"]) == codes[1]), rows.FirstOrDefault(x => Convert.ToString(x["Code"]) == codes[2]));
            var otherGoods = rows.FirstOrDefault(x => Convert.ToString(x["Code"]) == "34");
            var otherDetails = rows.Where(x => (Convert.ToString(x["Code"]) ?? string.Empty).StartsWith("TM_", StringComparison.OrdinalIgnoreCase)).ToList();
            if (otherGoods != null) foreach (var field in new[] { "PerformInPeriod", "PerformPreviousPeriod", "AccumulatedBeginingOfYear" }) otherGoods[field] = otherDetails.Sum(x => value(x, field));
            var retailCodes = new[] { "05", "08", "11", "14", "17", "20", "23", "26", "29", "32", "38" };
            var onlineCodes = new[] { "06", "09", "12", "15", "18", "21", "24", "27", "30", "33", "39" };
            var retailTotal = rows.FirstOrDefault(x => Convert.ToString(x["Code"]) == "02");
            var onlineTotal = rows.FirstOrDefault(x => Convert.ToString(x["Code"]) == "03");
            foreach (var field in new[] { "PerformInPeriod", "PerformPreviousPeriod", "AccumulatedBeginingOfYear" }) { if (retailTotal != null) retailTotal[field] = rows.Where(x => retailCodes.Contains(Convert.ToString(x["Code"]))).Sum(x => value(x, field)); if (onlineTotal != null) onlineTotal[field] = rows.Where(x => onlineCodes.Contains(Convert.ToString(x["Code"]))).Sum(x => value(x, field)); }
            setSum(rows.FirstOrDefault(x => Convert.ToString(x["Code"]) == "01"), retailTotal, onlineTotal);
        }

        private static void RecalculateImportExportSummaryInDataTable(DataTable table)
        {
            if (table == null) return;
            var rows = table.AsEnumerable().ToList();
            Func<DataRow, string, double> value = (row, field) => row[field] == DBNull.Value ? 0 : Convert.ToDouble(row[field]);

            var countryRows = rows.Where(x => {
                var code = Convert.ToString(x["Code"]);
                return !string.IsNullOrWhiteSpace(code) && code.StartsWith("XK_QG_", StringComparison.OrdinalIgnoreCase) && !string.Equals(code, "XK_QG_HEADER", StringComparison.OrdinalIgnoreCase);
            }).ToList();

            var utHeaderIdx = rows.FindIndex(x => string.Equals(Convert.ToString(x["Code"]), "UT_MH_HEADER", StringComparison.OrdinalIgnoreCase));
            var trustRows = utHeaderIdx >= 0
                ? rows.Skip(utHeaderIdx + 1).Where(x => !string.IsNullOrWhiteSpace(Convert.ToString(x["Code"]))).ToList()
                : new List<DataRow>();

            var xkTtRow = rows.FirstOrDefault(x => string.Equals(Convert.ToString(x["Code"]), "XK_TT", StringComparison.OrdinalIgnoreCase));
            if (xkTtRow != null)
            {
                foreach (var field in new[] { "PerformPreviousPeriod", "ComparedSamePeriodLastYear", "PerformInPeriod", "AccumulatedBeginingOfYear" })
                {
                    xkTtRow[field] = countryRows.Sum(x => value(x, field));
                }
            }

            var utXkRow = rows.FirstOrDefault(x => string.Equals(Convert.ToString(x["Code"]), "UT_XK", StringComparison.OrdinalIgnoreCase));
            if (utXkRow != null)
            {
                foreach (var field in new[] { "PerformPreviousPeriod", "ComparedSamePeriodLastYear", "PerformInPeriod", "AccumulatedBeginingOfYear" })
                {
                    utXkRow[field] = trustRows.Sum(x => value(x, field));
                }
            }

            var fobRow = rows.FirstOrDefault(x => string.Equals(Convert.ToString(x["Code"]), "FOB", StringComparison.OrdinalIgnoreCase));
            if (fobRow != null && xkTtRow != null && utXkRow != null)
            {
                foreach (var field in new[] { "PerformPreviousPeriod", "ComparedSamePeriodLastYear", "PerformInPeriod", "AccumulatedBeginingOfYear" })
                {
                    fobRow[field] = value(xkTtRow, field) + value(utXkRow, field);
                }
            }
        }

        private static string GetTypeBusinessDisplayName(EnumTypeBusiness type)
        {
            string msgKey = EnumHelper.GetDescription(type);
            string msg = AppProcessor.Messagor.GetMessage(msgKey);
            if (string.IsNullOrWhiteSpace(msg) || msg == msgKey)
            {
                switch (type)
                {
                    case EnumTypeBusiness.Manufacturing:
                        return "Doanh nghiệp sản xuất, kinh doanh";
                    case EnumTypeBusiness.Trading:
                        return "Doanh nghiệp thương mại, dịch vụ";
                    case EnumTypeBusiness.ImportExport:
                        return "Doanh nghiệp xuất, nhập khẩu";
                    default:
                        return msgKey;
                }
            }
            return msg;
        }

        private static List<ListItem> GetListTypeBusinessItems()
        {
            return Enum.GetValues(typeof(EnumTypeBusiness))
                .Cast<EnumTypeBusiness>()
                .Select(x => new ListItem(GetTypeBusinessDisplayName(x), ((int)x).ToString()))
                .ToList();
        }

        private static bool ValidateRevenueConstraint(DataTable table, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (table == null) return true;

            var rows = table.AsEnumerable().ToList();
            var totalRevRow = rows.FirstOrDefault(r => string.Equals(Convert.ToString(r["Code"]), "01", StringComparison.OrdinalIgnoreCase) || string.Equals(Convert.ToString(r["Targets"]), "Tổng doanh thu", StringComparison.OrdinalIgnoreCase));
            var industryRevRow = rows.FirstOrDefault(r => string.Equals(Convert.ToString(r["Code"]), "0101", StringComparison.OrdinalIgnoreCase) || string.Equals(Convert.ToString(r["Targets"]), "Trong đó doanh thu công nghiệp", StringComparison.OrdinalIgnoreCase));

            if (totalRevRow == null || industryRevRow == null) return true;

            var fields = new[]
            {
                new { Col = "AccumulatedBeginingOfYear", Label = "Kế hoạch năm" },
                new { Col = "PerformPreviousPeriod", Label = "Thực hiện tháng trước" },
                new { Col = "PerformInPeriod", Label = "Ước thực hiện tháng báo cáo" }
            };

            foreach (var f in fields)
            {
                double totalVal = totalRevRow[f.Col] != DBNull.Value ? Convert.ToDouble(totalRevRow[f.Col]) : 0.0;
                double indVal = industryRevRow[f.Col] != DBNull.Value ? Convert.ToDouble(industryRevRow[f.Col]) : 0.0;

                if (indVal > totalVal)
                {
                    errorMessage = string.Format("Doanh thu công nghiệp ({0:N2}) không được lớn hơn Tổng doanh thu ({1:N2}) ở cột {2}.", indVal, totalVal, f.Label);
                    return false;
                }
            }

            return true;
        }

        // Mặt hàng "Hàng hóa khác" được người dùng nhập tự do. Giữ lại mã cũ nếu cùng tên
        // đã từng được khai báo cho doanh nghiệp, nếu chưa có thì sinh mã nội bộ mới.
        private void EnsureTradingDynamicCodes(DataTable table, int enterpriseId)
        {
            if (table == null || enterpriseId <= 0) return;
            var dynamicRows = table.AsEnumerable()
                .Where(x => (Convert.ToString(x["Code"]) ?? string.Empty).StartsWith("TM_NEW_", StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (!dynamicRows.Any()) return;

            var knownCodes = (_importCache.GetViaEnterpriseOnMonth(enterpriseId, null) ?? new List<ReportDataImportModel>())
                .Where(x => !string.IsNullOrWhiteSpace(x.Targets) && !string.IsNullOrWhiteSpace(x.Code) && x.Code.StartsWith("TM_", StringComparison.OrdinalIgnoreCase))
                .GroupBy(x => x.Targets.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(x => x.Key, x => x.First().Code, StringComparer.OrdinalIgnoreCase);
            foreach (var row in dynamicRows)
            {
                var target = (Convert.ToString(row["Targets"]) ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(target)) continue;
                string code;
                if (!knownCodes.TryGetValue(target, out code))
                {
                    code = "TM_" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpperInvariant();
                    knownCodes[target] = code;
                }
                row["Code"] = code;
            }
        }

        private static bool ValidateTradingRetailConstraint(DataTable table, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (table == null) return true;
            var pairs = new[]
            {
                new { Total = "01", Retail = "02" }, new { Total = "03", Retail = "04" },
                new { Total = "05", Retail = "06" }, new { Total = "07", Retail = "08" },
                new { Total = "09", Retail = "10" }, new { Total = "11", Retail = "12" },
                new { Total = "14", Retail = "15" }, new { Total = "16", Retail = "17" },
                new { Total = "18", Retail = "19" }, new { Total = "20", Retail = "21" }
            };
            var fields = new[] { "PerformInPeriod", "PerformPreviousPeriod", "AccumulatedBeginingOfYear" };
            foreach (var pair in pairs)
            {
                var total = table.AsEnumerable().FirstOrDefault(x => Convert.ToString(x["Code"]) == pair.Total);
                var retail = table.AsEnumerable().FirstOrDefault(x => Convert.ToString(x["Code"]) == pair.Retail);
                if (total == null || retail == null) continue;
                foreach (var field in fields)
                {
                    var totalValue = total[field] == DBNull.Value ? 0 : Convert.ToDouble(total[field]);
                    var retailValue = retail[field] == DBNull.Value ? 0 : Convert.ToDouble(retail[field]);
                    if (retailValue > totalValue)
                    {
                        errorMessage = $"Chỉ tiêu '{Convert.ToString(retail["Targets"])}' không được lớn hơn '{Convert.ToString(total["Targets"])}'.";
                        return false;
                    }
                }
            }
            return true;
        }

        private byte[] CreateReport(DataTable data, string reportFilename, string urlPathReport, out string mimeType,
            out string fileExt)
        {
            #region Create Report

            // Variables
            Warning[] warnings;
            string[] streamIds;
            //string mimeType;
            string encoding;
            //string extension;

            // Setup the report viewer object and get the array of bytes
            var reportExcel = new ReportViewer { ProcessingMode = ProcessingMode.Local };

            reportExcel.LocalReport.ReportPath = urlPathReport;
            reportExcel.LocalReport.DataSources.Clear();
            reportExcel.LocalReport.DataSources.Add(new ReportDataSource("DataImport", data));
            reportExcel.LocalReport.DisplayName = reportFilename;

            //Chuyển sang Excel
            var bytes = reportExcel.LocalReport.Render("EXCELOPENXML", null, out mimeType, out encoding, out fileExt,
                out streamIds, out warnings);

            #endregion

            return bytes;
        }

        private bool CheckCorrectTemplate(HttpPostedFileBase fileImport, string typeBiz)
        {
            var lstKey = new Dictionary<string, string>{
                { "Tags", "ReportTourism"},
                { "Company","KHA_CenIT"}
            };

            Workbook workbook = new Workbook();
            workbook.LoadFromStream(fileImport.InputStream);
            BuiltInDocumentProperties p = workbook.DocumentProperties;
            for (int i = 0; i < p.Count; i++)
            {
                string pName = p[i].Name;
                if (!lstKey.ContainsKey(pName) && pName != "Category") continue;
                string pValue = p[i].Text;
                var lstTypeBiz = string.IsNullOrEmpty(pValue) ? new List<string>() : pValue.Split(';').ToList();
                if (pName == "Category" && !lstTypeBiz.Contains(typeBiz))
                {
                    return false;
                }
                if (pName == "Category") continue;
                if (lstKey[pName] != pValue)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckCorrectTemplate(Stream streamFile, string typeBiz)
        {
            var lstKey = new Dictionary<string, string>{
                { "Tags", "ReportTourism"},
                { "Company","KHA_CenIT"}
            };

            Workbook workbook = new Workbook();
            workbook.LoadFromStream(streamFile);
            BuiltInDocumentProperties p = workbook.DocumentProperties;
            for (int i = 0; i < p.Count; i++)
            {
                string pName = p[i].Name;
                if (!lstKey.ContainsKey(pName) && pName != "Category") continue;
                string pValue = p[i].Text;
                var lstTypeBiz = string.IsNullOrEmpty(pValue) ? new List<string>() : pValue.Split(';').ToList();
                if (pName == "Category" && !lstTypeBiz.Contains(typeBiz))
                {
                    return false;
                }
                if (pName == "Category") continue;
                if (lstKey[pName] != pValue)
                {
                    return false;
                }
            }

            return true;
        }

        private bool HasSignature(HttpPostedFileBase fileImport)
        {
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fileImport.InputStream, false))
            {
                var signature = spreadsheetDocument.DigitalSignatureOriginPart;
                if (signature == null) return false;
            }
            return true;
        }

        #endregion

        #region Smart CA

        #region Properties

        private readonly string _digitalSignTitle = AppProcessor.Messagor.GetMessage("DigitalSign_Title");

        private readonly Dictionary<int, string> _transactionStatus = new Dictionary<int, string>
        {
            {1, "SUCCESS"},
            {4000, "WAITING_FOR_SIGNER_CONFIRM"},
            {4001, "EXPIRED"},
            {4002, "SIGNER_REJECTED"},
            {4003, "AUTHORIZE_KEY_FAILED"},
            {4004, "SIGN_FAILED"}
        };

        private readonly string _signedFilesPathFolder =
            ConfigurationManager.AppSettings["Modules_Sys_SignedDoc_FolderPath"] ??
            "/Contents/Modules/Report/ReportSignedDocs/";

        #endregion

        #region Login & Logout

        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult LoginSmartCA()
        {
            return PartialView("_Login", new LoginSmartCAModel());
        }

        [ActionType(Type = EnumActionType.Edit)]
        [HttpPost]
        public ActionResult LoginSmartCA(LoginSmartCAModel model)
        {
            if (!ModelState.IsValid) return PartialView("_LoginSmartCA", model);

            string sRefreshToken;
            UserInfoModel userInfo = null;

            var accessToken = VNPTSmartCAProvider.AuthToken(model.UserName, model.Password, out sRefreshToken);
            if (!string.IsNullOrEmpty(accessToken)) userInfo = VNPTSmartCAProvider.GetUserInfo(accessToken);

            if (string.IsNullOrEmpty(accessToken))
                return Json(new
                {
                    status = false,
                    message = CreateMessage(
                        $"{AppProcessor.Messagor.GetMessage("Login_SmartCA_Label")} thất bại. Tài khoản hoặc mật khẩu không đúng.",
                        EnumProcessType.NonFormat, EnumMsgIcon.Error)
                });

            if (userInfo != null)
                Session[$"VNPT-SmartCA-{User?.UserName}-UserInfo"] = userInfo;

            Session[$"VNPT-SmartCA-{User?.UserName}-AccessToken"] = accessToken;

            return Json(new
            {
                status = true,
                message = CreateMessage($"{AppProcessor.Messagor.GetMessage("Login_SmartCA_Label")} thành công",
                    EnumProcessType.NonFormat, EnumMsgIcon.Success)
            });
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        [AjaxOnly]
        public ActionResult SignOutSmartCA()
        {
            Session[$"VNPT-SmartCA-{User?.UserName}-AccessToken"] = null;
            Session[$"VNPT-SmartCA-{User?.UserName}-UserInfo"] = null;

            return Json(new
            {
                status = true,
                message = ""
            });
        }

        [ActionType(Type = EnumActionType.View)]
        //[HttpGet]
        public ActionResult UserInfo()
        {
            var userInfo = (UserInfoModel)Session[$"VNPT-SmartCA-{User?.UserName}-UserInfo"];

            return PartialView("_UserInfo", userInfo ?? new UserInfoModel());
        }

        #endregion

        #region Sign Doc

        /// <summary>
        ///     Ký văn bản
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="fileImport"></param>
        /// <param name="fullSignedPath"></param>
        /// <returns>
        ///     - 0: thành công
        ///     - 1: ký thất bại
        ///     - 2: lỗi thông tin chữ ký số
        ///     - 3: người dùng không xác nhận ký số từ App
        ///     - 4: lỗi chữ ký
        ///     - 5: chữ ký không khớp
        /// </returns>
        private int SignHash(string accessToken, HttpPostedFileBase fileImport, string fullSignedPath)
        {
            var lstCredentials = VNPTSmartCAProvider.GetListCredentials(accessToken);
            var credentialId = lstCredentials?[0];
            var credentialInfo =
                VNPTSmartCAProvider.GetCredentialInfo(accessToken, credentialId, CTSType.Chain, true, true);
            var certBase64 = credentialInfo?.Cert?.Certificates?[0]?.Replace("\r\n", "");
            var fileData = StreamHelper.ReadFully(fileImport.InputStream);

            var typeExt = HashSignerFactory.OFFICE;
            //var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileImport.FileName);
            var fileExt = Path.GetExtension(fileImport.FileName);

            if (fileExt?.ToUpper() == ".PDF")
                typeExt = HashSignerFactory.PDF;
            else if (fileExt?.ToUpper() == ".XML") typeExt = HashSignerFactory.XML;

            IHashSigner signer;
            var tranId = VNPTSmartCAProvider.SignHash(out signer, accessToken, credentialId, certBase64, "",
                fileImport.FileName, fileData, typeExt);
            if (string.IsNullOrEmpty(tranId)) return 1;

            AppProcessor.Notifider.PushNotifyToUser("Sys", User.UserName, "Vui lòng xác nhận ký số trên ứng dụng.",
                EnumProcessType.NonFormat, EnumMsgIcon.Warning);

            var iCount = 1;
            var isConfirm = false;
            var datasigned = "";
            var tranStatus = -1;

            while (iCount <= 24 && !isConfirm)
            {
                AppProcessor.Logger.Message($"Chờ ký số lần {iCount}: ");

                AppProcessor.Notifider.PushNotifyToUser("Sys", User.UserName,
                    $"Chờ xác nhận ký số trên ứng dụng lần {iCount}", EnumProcessType.NonFormat, EnumMsgIcon.Warning);

                var tranInfo = VNPTSmartCAProvider.GetTranInfo(accessToken, tranId);
                tranStatus = tranInfo?.TranStatus ?? -1;

                if (tranInfo != null)
                {
                    if (tranInfo.TranStatus == 4000)
                    {
                        iCount = iCount + 1;
                        Thread.Sleep(5000);
                    }
                    else if (tranInfo.TranStatus == 4002)
                    {
                        datasigned = null;
                        break;
                    }
                    else
                    {
                        isConfirm = true;
                        datasigned = tranInfo.Documents[0].Sig;
                    }
                }
                else
                {
                    AppProcessor.Logger.Message("Lỗi nội dung");
                    return 2;
                }
            }

            var msgStatus = _transactionStatus[tranStatus];
            if (!isConfirm && datasigned == null)
            {
                AppProcessor.Logger.Message($"Từ chối ký số - {msgStatus}");
                return 6;
            }

            if (!isConfirm)
            {
                AppProcessor.Logger.Message($"Không xác nhận từ App - {msgStatus}");
                return 3;
            }

            if (string.IsNullOrEmpty(datasigned))
            {
                AppProcessor.Logger.Message($"Lỗi ký số - {msgStatus}");
                return 4;
            }

            if (!signer.CheckHashSignature(datasigned))
            {
                AppProcessor.Logger.Message($"Chữ ký số không khớp - {msgStatus}");
                return 5;
            }
            // ------------------------------------------------------------------------------------------
            AppProcessor.Logger.Message("Ký số thành công");

            // 3. Package external signature to signed file
            var signed = signer.Sign(datasigned);
            System.IO.File.WriteAllBytes(fullSignedPath, signed);
            return 0;
        }

        private int SignHash(string accessToken, byte[] dataImport, string fileNameWithExt, string fileExt,
            string fullSignedPath)
        {
            var lstCredentials = VNPTSmartCAProvider.GetListCredentials(accessToken);
            var credentialId = lstCredentials?[0];
            var credentialInfo =
                VNPTSmartCAProvider.GetCredentialInfo(accessToken, credentialId, CTSType.Chain, true, true);
            var certBase64 = credentialInfo?.Cert?.Certificates?[0]?.Replace("\r\n", "");

            var typeExt = HashSignerFactory.OFFICE;

            if (fileExt?.ToUpper() == ".PDF")
                typeExt = HashSignerFactory.PDF;
            else if (fileExt?.ToUpper() == ".XML") typeExt = HashSignerFactory.XML;

            IHashSigner signer;
            var tranId = VNPTSmartCAProvider.SignHash(out signer, accessToken, credentialId, certBase64, "",
                fileNameWithExt, dataImport, typeExt);
            if (string.IsNullOrEmpty(tranId)) return 1;

            AppProcessor.Notifider.PushNotifyToUser("Sys", User.UserName, "Vui lòng xác nhận ký số trên ứng dụng.",
                EnumProcessType.NonFormat, EnumMsgIcon.Warning);

            var iCount = 0;
            var isConfirm = false;
            var datasigned = "";
            var tranStatus = -1;

            while (iCount < 24 && !isConfirm)
            {
                AppProcessor.Logger.Message($"Chờ ký số lần {iCount}: ");

                AppProcessor.Notifider.PushNotifyToUser("Sys", User.UserName,
                    $"Chờ xác nhận ký số trên ứng dụng lần {iCount + 1}", EnumProcessType.NonFormat, EnumMsgIcon.Warning);

                var tranInfo = VNPTSmartCAProvider.GetTranInfo(accessToken, tranId);
                tranStatus = tranInfo?.TranStatus ?? -1;

                if (tranInfo != null)
                {
                    if (tranInfo.TranStatus == 4000)
                    {
                        iCount = iCount + 1;
                        Thread.Sleep(5000);
                    }
                    else if (tranInfo.TranStatus == 4002)
                    {
                        datasigned = null;
                        break;
                    }
                    else
                    {
                        isConfirm = true;
                        datasigned = tranInfo.Documents[0].Sig;
                    }
                }
                else
                {
                    AppProcessor.Logger.Message("Lỗi nội dung");
                    return 2;
                }
            }

            var msgStatus = _transactionStatus[tranStatus];
            if (!isConfirm && datasigned == null)
            {
                AppProcessor.Logger.Message($"Từ chối ký số - {msgStatus}");
                return 6;
            }

            if (!isConfirm)
            {
                AppProcessor.Logger.Message($"Không xác nhận từ App - {msgStatus}");
                return 3;
            }

            if (string.IsNullOrEmpty(datasigned))
            {
                AppProcessor.Logger.Message($"Lỗi ký số - {msgStatus}");
                return 4;
            }

            if (!signer.CheckHashSignature(datasigned))
            {
                AppProcessor.Logger.Message($"Chữ ký số không khớp - {msgStatus}");
                return 5;
            }
            // ------------------------------------------------------------------------------------------

            AppProcessor.Logger.Message("Ký số thành công");

            // 3. Package external signature to signed file
            var signed = signer.Sign(datasigned);
            System.IO.File.WriteAllBytes(fullSignedPath, signed);
            return 0;
        }

        /// <summary>
        ///     Ký văn bản
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="fileImport"></param>
        /// <param name="fullSignedPath"></param>
        /// <returns>
        ///     - 0: thành công
        ///     - 1: ký thất bại
        ///     - 2: lỗi thông tin chữ ký số
        ///     - 3: người dùng không xác nhận ký số từ App
        ///     - 4: lỗi chữ ký
        ///     - 5: chữ ký không khớp
        /// </returns>
        private int Sign(string accessToken, HttpPostedFileBase fileImport, string fullSignedPath)
        {
            var lstCredentials = VNPTSmartCAProvider.GetListCredentials(accessToken);
            var credentialId = lstCredentials?[0];
            var fileData = StreamHelper.ReadFully(fileImport.InputStream);
            var tranId = VNPTSmartCAProvider.Sign(accessToken, credentialId, "", fileImport.FileName, fileData);
            if (string.IsNullOrEmpty(tranId)) return 1;

            AppProcessor.Notifider.PushNotifyToUser("Sys", User.UserName, "Vui lòng xác nhận ký số trên ứng dụng.",
                EnumProcessType.NonFormat, EnumMsgIcon.Warning);

            var iCount = 1;
            var isConfirm = false;
            var datasigned = "";
            var tranStatus = -1;

            while (iCount <= 24 && !isConfirm)
            {
                AppProcessor.Logger.Message($"Chờ ký số lần {iCount}: ");

                AppProcessor.Notifider.PushNotifyToUser("Sys", User.UserName,
                    $"Chờ xác nhận ký số trên ứng dụng lần {iCount}", EnumProcessType.NonFormat, EnumMsgIcon.Warning);

                var tranInfo = VNPTSmartCAProvider.GetTranInfo(accessToken, tranId);
                tranStatus = tranInfo?.TranStatus ?? -1;

                if (tranInfo != null)
                {
                    if (tranInfo.TranStatus == 4000)
                    {
                        iCount = iCount + 1;
                        Thread.Sleep(5000);
                    }
                    else if (tranInfo.TranStatus == 4002)
                    {
                        datasigned = null;
                        break;
                    }
                    else
                    {
                        isConfirm = true;
                        datasigned = tranInfo.Documents[0].DataSigned;
                    }
                }
                else
                {
                    AppProcessor.Logger.Message("Lỗi nội dung");
                    return 2;
                }
            }

            var msgStatus = _transactionStatus[tranStatus];
            if (!isConfirm && datasigned == null)
            {
                AppProcessor.Logger.Message($"Từ chối ký số - {msgStatus}");
                return 6;
            }

            if (!isConfirm)
            {
                AppProcessor.Logger.Message($"Không xác nhận từ App - {msgStatus}");
                return 3;
            }

            if (string.IsNullOrEmpty(datasigned))
            {
                AppProcessor.Logger.Message($"Lỗi ký số - {msgStatus}");
                return 4;
            }

            AppProcessor.Logger.Message("Ký số thành công");

            // 3. Package external signature to signed file
            System.IO.File.WriteAllBytes(fullSignedPath, Convert.FromBase64String(datasigned));
            return 0;
        }

        private int Sign(string accessToken, byte[] dataImport, string fileNameWithExt,
            string fullSignedPath, out string msgLogs)
        {
            msgLogs = string.Empty;
            StringBuilder logBuilder = new StringBuilder();

            var lstCredentials = VNPTSmartCAProvider.GetListCredentials(accessToken);
            var credentialId = lstCredentials?[0];
            var tranId = VNPTSmartCAProvider.Sign(accessToken, credentialId, "",
                fileNameWithExt, dataImport);
            if (string.IsNullOrEmpty(tranId)) return 1;

            AppProcessor.Notifider.PushNotifyToUser("Sys", User.UserName, "Vui lòng xác nhận ký số trên ứng dụng.",
                EnumProcessType.NonFormat, EnumMsgIcon.Warning);

            var iCount = 0;
            var isConfirm = false;
            var datasigned = "";
            var tranStatus = -1;

            while (iCount < 24 && !isConfirm)
            {
                logBuilder.AppendLine($"    * Chờ ký số lần: {iCount}");
                //AppProcessor.Logger.Message($"Chờ ký số lần {iCount}: ");

                AppProcessor.Notifider.PushNotifyToUser("Sys", User.UserName,
                    $"Chờ xác nhận ký số trên ứng dụng lần {iCount + 1}", EnumProcessType.NonFormat, EnumMsgIcon.Warning);

                var tranInfo = VNPTSmartCAProvider.GetTranInfo(accessToken, tranId);
                tranStatus = tranInfo?.TranStatus ?? -1;

                if (tranInfo != null)
                {
                    if (tranInfo.TranStatus == 4000)
                    {
                        iCount = iCount + 1;
                        Thread.Sleep(5000);
                    }
                    else if (tranInfo.TranStatus == 4002)
                    {
                        datasigned = null;
                        break;
                    }
                    else
                    {
                        isConfirm = true;
                        datasigned = tranInfo.Documents[0].DataSigned;
                    }
                }
                else
                {
                    //AppProcessor.Logger.Message("Lỗi nội dung");
                    logBuilder.AppendLine(" + Lỗi nội dung");
                    msgLogs = logBuilder.ToString();
                    return 2;
                }
            }

            var msgStatus = _transactionStatus[tranStatus];
            if (!isConfirm && datasigned == null)
            {
                //AppProcessor.Logger.Message($"Từ chối ký số - {msgStatus}");
                logBuilder.AppendLine($" + Từ chối ký số - {msgStatus}");
                msgLogs = logBuilder.ToString();
                return 6;
            }

            if (!isConfirm)
            {
                //AppProcessor.Logger.Message($"Không xác nhận từ App - {msgStatus}");
                logBuilder.AppendLine($" + Không xác nhận từ App - {msgStatus}");
                msgLogs = logBuilder.ToString();
                return 3;
            }

            if (string.IsNullOrEmpty(datasigned))
            {
                //AppProcessor.Logger.Message($"Lỗi ký số - {msgStatus}");
                logBuilder.AppendLine($" + Lỗi ký số - {msgStatus}");
                msgLogs = logBuilder.ToString();
                return 4;
            }

            //AppProcessor.Logger.Message("Ký số thành công");
            logBuilder.AppendLine(" + Ký số thành công");
            msgLogs = logBuilder.ToString();

            // 3. Package external signature to signed file
            System.IO.File.WriteAllBytes(fullSignedPath, Convert.FromBase64String(datasigned));
            return 0;
        }

        #endregion

        #endregion

        #region VNPT Token

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult SignToken()
        {
            var serialKeyVNPTToken = ConfigurationManager.AppSettings["VNPT_Token_Serial"];
            return PartialView("_SignToken", serialKeyVNPTToken);
        }

        //[AjaxOnly]
        //[HttpPost]
        //[ActionType(Type = EnumActionType.Add)]
        //public ActionResult CheckDataFileSignToken(DataFileSignTokenModel model)
        //{
        //    if (model.FileDataBase64 == null)
        //    {
        //        return Json(new
        //        {
        //            status = false,
        //            message = string.Empty
        //        });
        //    }

        //    var enterpriseModel = _enterpriseCache.GetById(model.EnterpriseId);
        //    if (enterpriseModel == null)
        //        return Json(new
        //        {
        //            status = false,
        //            message = CreateMessage($"{_enterpriseTitle}",
        //                EnumProcessType.DataNotExist, EnumMsgIcon.Error)
        //        });

        //    var arraBytes = Convert.FromBase64String(model.FileDataBase64);
        //    var streamDatas = new MemoryStream(arraBytes);

        //    if (!CheckCorrectTemplate(streamDatas, EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)))
        //    {
        //        string sTypeBiz = _mappingReportTypeBiz[enterpriseModel.TypeBusiness];
        //        return Json(new
        //        {
        //            status = false,
        //            message = CreateMessage($"Tệp dữ liệu import không đúng loại báo cáo thuộc [{sTypeBiz}]",
        //                EnumProcessType.NonFormat, EnumMsgIcon.Error)
        //        });
        //    }

        //    #region Check Data Import

        //    bool isSuccessImport;
        //    var dataImports = ReadDataImports(streamDatas, model.FileExt, out isSuccessImport);
        //    if (!isSuccessImport)
        //        return Json(new
        //        {
        //            status = false,
        //            message = CreateMessage(AppProcessor.Messagor.GetMessage("ImportData_Message_Fail"),
        //                EnumProcessType.NonFormat, EnumMsgIcon.Error)
        //        });
        //    var lstDataImports = ModelProvider.CreateListFromTable<ReportDataImportModel>(dataImports);

        //    #endregion

        //    return Json(new
        //    {
        //        status = true,
        //        dataImport = lstDataImports.OrderBy(d => d.RowIndex),
        //        typeReport = enterpriseModel.TypeBusiness,
        //        message = ""
        //    });
        //}

        #endregion
    }
}
