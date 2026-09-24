using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
            }
        };

        #endregion

        // GET: Cate/ReportDataImport
        public ActionResult Index()
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
            return View(searchModel);
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
                    TypeReport = enterpriseModel.TypeBusiness,
                    TypeReportName =
                        AppProcessor.Messagor.GetMessage(
                            EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)),
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

                logActions.AppendLine(
                    $" - [{User.UserName}] thực hiện gửi báo cáo [{EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)}] cho doanh nghiệp [{enterpriseModel.BusinessName}]");
                logActions.AppendLine(" - Thực hiện kiểm tra nội dung báo cáo");

                if (!CheckCorrectTemplate(model.FileImport,
                        EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)))
                {
                    logActions.AppendLine(" + Nội dung báo cáo không đúng định dạng");
                    AppProcessor.Logger.Message(logActions.ToString());

                    string sTypeBiz = _mappingReportTypeBiz[enterpriseModel.TypeBusiness];
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
                    TypeReport = enterpriseModel.TypeBusiness,
                    TypeReportName =
                        AppProcessor.Messagor.GetMessage(
                            EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)),
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
                TypeReport = enterpriseModel.TypeBusiness,
                TypeReportName =
                    AppProcessor.Messagor.GetMessage(
                        EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)),
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

            if (!CheckCorrectTemplate(model.FileImport,
                    EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)))
            {
                string sTypeBiz = _mappingReportTypeBiz[enterpriseModel.TypeBusiness];
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
        public ActionResult View(int? enterpriseId, DateTime? onMonth)
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
            var lstReportsViaUsers = _importCache.GetForUserOnMonth(User.UserName, DateTime.Now);
            var lstEnterpriseOther = lstEnterpisePermits
                .Where(e => !lstReportsViaUsers.Exists(r => r.EnterpriseId == e.EnterpriseId)).ToList();

            var reportModel = new TourismReportModel
            {
                ListEnterprises = lstEnterpriseOther.OrderBy(e => e.BusinessName)
                    .Select(e => new ListItem(e.BusinessName, e.EnterpriseId.ToString())).ToList(),
                DayDeadlineSendReport =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Report")?.ConfigValue ?? "0"),
                DayDeadlineSendReportLate =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Late_Report")?.ConfigValue ?? "0"),
                AccessToken = (string)Session[$"VNPT-SmartCA-{User?.UserName}-AccessToken"],
                EnableSignDigitalDoc =
                    (_configCache.GetViaKey("Enable_SignDigital_Doc")?.ConfigValue ?? "0") != "0"
            };
            return PartialView("_Add", reportModel);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Add(TourismReportModel model)
        {
            StringBuilder logActions = new StringBuilder();
            logActions.AppendLine("=========================================");

            if (!ModelState.IsValid)
            {
                var lstEnterpisePermits = _enterpriseCache.GetViaUser(User.UserName);
                var lstReportsViaUsers = _importCache.GetForUserOnMonth(User.UserName, DateTime.Now);
                var lstEnterpriseOther = lstEnterpisePermits
                    .Where(e => !lstReportsViaUsers.Exists(r => r.EnterpriseId == e.EnterpriseId)).ToList();

                var reportModel = new TourismReportModel
                {
                    ListEnterprises = lstEnterpriseOther.OrderBy(e => e.BusinessName)
                        .Select(e => new ListItem(e.BusinessName, e.EnterpriseId.ToString())).ToList(),
                    DayDeadlineSendReport =
                        int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Report")?.ConfigValue ?? "0"),
                    DayDeadlineSendReportLate =
                        int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Late_Report")?.ConfigValue ?? "0"),
                    EnableSignDigitalDoc =
                        (_configCache.GetViaKey("Enable_SignDigital_Doc")?.ConfigValue ?? "0") != "0"
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

            logActions.AppendLine($" - [{User.UserName}] thực hiện gửi báo cáo [{EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)}] cho doanh nghiệp [{enterpriseModel.BusinessName}]");
            logActions.AppendLine(" - Đọc nội dung báo cáo");

            var dataReport = ReadFormData(Request.Form);

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
            var enterpriseId = _importCache.Import(new ReportDataImportModel
            {
                EnterpriseId = model.EnterpriseId,
                ForMonth = model.ForMonth,
                TypeReport = enterpriseModel.TypeBusiness,
                TypeReportName =
                    AppProcessor.Messagor.GetMessage(
                        EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)),
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
        public ActionResult Edit(int? enterpriseId, DateTime? onMonth)
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

            ViewBag.Title =
                $"{_importTile} <b>[{enterpriseModel.BusinessName}]</b> tháng <b class='text-yellow'>[{onMonth?.ToString("MM/yyyy")}]</b>";
            var dataImports = _importCache.GetViaEnterpriseOnMonth(enterpriseId, onMonth);
            if (dataImports == null || dataImports.Count <= 0)
                return Json(new
                {
                    status = false,
                    errorCode = 1,
                    message = CreateMessage($"{_importTile}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            // Báo cáo chỉ tiêu công nghiệp không lưu các dòng tiêu đề nhóm. Khi sửa,
            // dựng lại toàn bộ khung từ danh mục rồi gắn số liệu đã lưu theo chỉ tiêu.
            if (enterpriseModel.TypeBusiness == 0)
            {
                var catalogResult = LoadViewTypeReport(enterpriseId) as PartialViewResult;
                var catalog = catalogResult?.Model as List<ReportDataImportModel>;
                if (catalog != null && catalog.Any())
                    dataImports = MergeBusinessProductData(catalog, dataImports);
            }

            var reportModel = new TourismReportModel
            {
                //ListEnterprises = lstEnterpriseOther.OrderBy(e => e.BusinessName)
                //    .Select(e => new ListItem(e.BusinessName, e.EnterpriseId.ToString())).ToList(),
                DayDeadlineSendReport =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Report")?.ConfigValue ?? "0"),
                DayDeadlineSendReportLate =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Late_Report")?.ConfigValue ?? "0"),
                EnterpriseId = enterpriseId,
                ForMonth = onMonth ?? DateTime.Now,
                ListDataImports = dataImports,
                EnterpriseName = enterpriseModel.BusinessName,
                TypeReport = (EnumTypeBusiness)enterpriseModel.TypeBusiness,
                TypeReportName = AppProcessor.Messagor.GetMessage(
                    EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)),
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
                    TypeReport = (EnumTypeBusiness)enterpriseModel.TypeBusiness,
                    TypeReportName = AppProcessor.Messagor.GetMessage(
                        EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)),
                    IsEdit = true,
                    EnableSignDigitalDoc =
                        (_configCache.GetViaKey("Enable_SignDigital_Doc")?.ConfigValue ?? "0") != "0"
                };
                return PartialView("_Add", reportModel);
            }

            #endregion

            logActions.AppendLine($" - [{User.UserName}] thực hiện gửi báo cáo [{EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)}] cho doanh nghiệp [{enterpriseModel.BusinessName}]");
            logActions.AppendLine(" - Đọc nội dung báo cáo");

            var dataReport = ReadFormData(Request.Form);

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

            var enterpriseId = _importCache.Import(new ReportDataImportModel
            {
                EnterpriseId = model.EnterpriseId,
                ForMonth = model.ForMonth,
                TypeReport = enterpriseModel.TypeBusiness,
                TypeReportName =
                    AppProcessor.Messagor.GetMessage(
                        EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)),
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
        [ActionType(Type = EnumActionType.View)]
        public ActionResult LoadViewTypeReport(int? enterpriseId)
        {
            var products = new List<ReportDataImportModel>();
            if (!enterpriseId.HasValue)
            {
                return PartialView("_BusinessProductReport", products);
            }

            var enterprises = _enterpriseCache.GetViaUser(User.UserName) ?? new List<CateEnterpriseModel>();
            if (enterprises.All(x => x.EnterpriseId != enterpriseId.Value))
            {
                return PartialView("_BusinessProductReport", products);
            }

            const string sql = @"
SELECT CategoryOrder, CategoryName, ProductName, Unit, ProductCode, DisplayOrder, ProductId
FROM
(
    SELECT 1 AS CategoryOrder, N'Sản phẩm công nghiệp chủ yếu' AS CategoryName,
           bp.ProductName, bp.Unit, bp.ProductCode, bp.DisplayOrder, bp.ProductId
    FROM dbo.Cate_BusinessEnterprise AS be
    INNER JOIN dbo.Cate_BusinessIndustry AS bi ON bi.IndustryId = be.MainIndustryId
    INNER JOIN dbo.Cate_BusinessProduct AS bp ON bp.IndustryId = bi.IndustryId
    WHERE be.EnterpriseId = @EnterpriseId
    UNION ALL
    SELECT 2, N'Kim ngạch xuất khẩu', iv.ImportValueName, iv.Unit, iv.ImportValueCode,
           iv.DisplayOrder, iv.ImportValueId
    FROM dbo.Cate_BusinessEnterprise AS be
    INNER JOIN dbo.Cate_BusinessProductImportValue AS iv ON iv.IndustryId = be.MainIndustryId
    WHERE be.EnterpriseId = @EnterpriseId
    UNION ALL
    SELECT 3, N'Kim ngạch nhập khẩu', ev.ExportValueName, ev.Unit, ev.ExportValueCode,
           ev.DisplayOrder, ev.ExportValueId
    FROM dbo.Cate_BusinessEnterprise AS be
    INNER JOIN dbo.Cate_BusinessProductExportValue AS ev ON ev.IndustryId = be.MainIndustryId
    WHERE be.EnterpriseId = @EnterpriseId
) AS source
ORDER BY CategoryOrder, ISNULL(DisplayOrder, 0), ProductId";

            var connectionString = ConfigurationManager.ConnectionStrings["BaseApp"]?.ConnectionString;
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@EnterpriseId", SqlDbType.BigInt).Value = enterpriseId.Value;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        var currentCategory = -1;
                        while (reader.Read())
                        {
                            var category = Convert.ToInt32(reader["CategoryOrder"]);
                            if (category != currentCategory)
                            {
                                products.Add(new ReportDataImportModel { Targets = Convert.ToString(reader["CategoryName"]) });
                                currentCategory = category;
                            }

                            products.Add(new ReportDataImportModel
                            {
                                Targets = Convert.ToString(reader["ProductName"]),
                                Unit = Convert.ToString(reader["Unit"]),
                                Code = Convert.ToString(reader["ProductCode"]).Trim()
                            });
                        }
                    }
                }
            }

            return PartialView("_BusinessProductReport", products);
        }

        private static List<ReportDataImportModel> MergeBusinessProductData(
            List<ReportDataImportModel> catalog, List<ReportDataImportModel> savedData)
        {
            foreach (var item in catalog.Where(x => !string.IsNullOrWhiteSpace(x.Code)))
            {
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
                item.AccumulatedBeginingOfYear = saved.AccumulatedBeginingOfYear;
                item.ComparedSamePeriodLastYear = saved.ComparedSamePeriodLastYear;
            }

            return catalog;
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
            dartaFormReport.Columns.Add("PerformPreviousPeriod", typeof(double));
            dartaFormReport.Columns.Add("PerformInPeriod", typeof(double));
            dartaFormReport.Columns.Add("AccumulatedBeginingOfYear", typeof(double));
            dartaFormReport.Columns.Add("ComparedSamePeriodLastYear", typeof(double));

            #endregion

            var numKeys = formData.AllKeys.Count(k => k.Contains("Target_"));
            var isBusinessProductReport = string.Equals(formData["BusinessProductReport"], "true",
                StringComparison.OrdinalIgnoreCase);
            for (var idx = 1; idx <= numKeys; idx++)
            {
                var code = formData[$"Code_{idx}"];
                if (isBusinessProductReport && string.IsNullOrWhiteSpace(code)) continue;

                dartaFormReport.Rows.Add(
                    formData[$"Target_{idx}"],
                    formData[$"Unit_{idx}"],
                    code,
                    string.IsNullOrEmpty(formData[$"PerformPreviousPeriod_{idx}"])
                        ? (double?)null
                        : double.Parse(formData[$"PerformPreviousPeriod_{idx}"]),
                    string.IsNullOrEmpty(formData[$"PerformInPeriod_{idx}"])
                        ? (double?)null
                        : double.Parse(formData[$"PerformInPeriod_{idx}"]),
                    string.IsNullOrEmpty(formData[$"AccumulatedBeginingOfYear_{idx}"])
                        ? (double?)null
                        : double.Parse(formData[$"AccumulatedBeginingOfYear_{idx}"]),
                    string.IsNullOrEmpty(formData[$"CompareSamePeriodLastYear_{idx}"])
                        ? (double?)null
                        : double.Parse(formData[$"CompareSamePeriodLastYear_{idx}"]));
            }

            return dartaFormReport;
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

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult CheckDataFileSignToken(DataFileSignTokenModel model)
        {
            if (model.FileDataBase64 == null)
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

            var arraBytes = Convert.FromBase64String(model.FileDataBase64);
            var streamDatas = new MemoryStream(arraBytes);

            if (!CheckCorrectTemplate(streamDatas, EnumHelper.GetDescription((EnumTypeBusiness)enterpriseModel.TypeBusiness)))
            {
                string sTypeBiz = _mappingReportTypeBiz[enterpriseModel.TypeBusiness];
                return Json(new
                {
                    status = false,
                    message = CreateMessage($"Tệp dữ liệu import không đúng loại báo cáo thuộc [{sTypeBiz}]",
                        EnumProcessType.NonFormat, EnumMsgIcon.Error)
                });
            }

            #region Check Data Import

            bool isSuccessImport;
            var dataImports = ReadDataImports(streamDatas, model.FileExt, out isSuccessImport);
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
    }
}
