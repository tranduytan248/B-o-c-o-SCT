using System;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using CenIT.ReportTourism.Caches.Sys;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;
using TSFramework.Core.Members.Job;
using TSFramework.Core.Providers;

namespace CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Controllers
{
    public class JobController : AppController
    {
        private readonly string _funcName = AppProcessor.Messagor.GetMessage("SysJob_Title");
        private readonly SysJobCache _jobCache;

        private readonly string _jobFolder = "Jobs";

        private readonly string _jobLibrariesPathFolder =
            ConfigurationManager.AppSettings["AttachmentFolderPath"] ?? @"/Contents/Modules/Sys/Attachments/";

        public JobController()
        {
            _jobCache = new SysJobCache();
        }

        // GET: SysJob
        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult Index()
        {
            //try
            //{
            //    var userModel = new SysUserCache().GetByUserName("trunglc.kha");
            //    string baseToken = $"{userModel.Password}-{DateTime.Now.AddHours(24).Ticks}";
            //    string passPharse = userModel.Password;
            //    string tokenResetPassword = EStringCipher.Encrypt(baseToken, passPharse);
            //    userModel.DetailUrl = Url.Action("ResetPassword", "Account", new { area = "", userName = userModel.UserName, token = tokenResetPassword });
            //    userModel.HostlUrl = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "");

            //    var mailBodyHtml = RenderTemplateHtmlProvider.RenderStringHtml(
            //        HostingEnvironment.MapPath(@"~/Contents/Modules/Sys/EmailTemplates/_TemplateResetPassword.cshtml"), userModel);

            //    AppProcessor.Mailer.PushEmail(new List<MailModel>
            //    {
            //        new MailModel
            //        {
            //            Subject = $"[{AppProcessor.Messagor.GetMessage("App_Title")}] {AppProcessor.Messagor.GetMessage("MailSubject_ResetPassword_Message")}",
            //            To = new List<string> { userModel.Email },
            //            Body = mailBodyHtml,
            //            IsBodyHtml = true,
            //            DisplayNameFrom = AppProcessor.Messagor.GetMessage("App_Owner_DisplayName")
            //        }
            //    });
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    throw;
            //}

            return View();
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get()
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
            var data = _jobCache.Get(out total, dataSearch);

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
            var model = new SysJobModel();
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult Add(SysJobModel model)
        {
            if (!ModelState.IsValid) return PartialView("_SysJob", model);

            var sFileName = Path.GetFileName(model.FileLibrary.FileName);
            var jobId = Guid.NewGuid();

            var idSysJob = _jobCache.Save(new SysJobModel
            {
                JobId = jobId,
                JobName = model.JobName,
                JobDescription = model.JobDescription,
                CronExpression = model.CronExpression,
                JobLibrary = sFileName,
                IsActive = model.IsActive,
                IsDeleted = false,
                SavedBy = User.UserName
            });

            if (idSysJob == -9)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName} - [{model.JobLibrary}]", EnumProcessType.DataExisted,
                        EnumMsgIcon.Error)
                });

            if (idSysJob > 0 && model.FileLibrary != null)
            {
                var jobLibrariesPathFolder = string.Concat(_jobLibrariesPathFolder, _jobFolder);
                var jobLibrariesAbsolutePathFolder = Server.MapPath(jobLibrariesPathFolder);
                var jobLibrariesAbsoluteFilePath = string.Concat(jobLibrariesAbsolutePathFolder, "/", sFileName);

                if (model.FileLibrary != null)
                {
                    if (!Directory.Exists(jobLibrariesAbsolutePathFolder))
                        Directory.CreateDirectory(jobLibrariesAbsolutePathFolder);

                    model.FileLibrary.SaveAs(jobLibrariesAbsoluteFilePath);
                }

                //MemoryStream memStream = new MemoryStream();
                //System.IO.File.Open(jobLibrariesAbsoluteFilePath, FileMode.Open).CopyTo(memStream);

                var jobPlugable = JobPlugableProvider.GetJobPlugable(jobLibrariesAbsoluteFilePath);
                JobSchedulerProvider.UpdateCronExpression(jobId.ToString(), model.CronExpression);

                if (jobPlugable != null)
                    ExecuteJob(jobPlugable, model);
            }

            var response = CreateMessage($"{_funcName} [{model.JobName}]", EnumProcessType.Add,
                idSysJob > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(string id)
        {
            var model = _jobCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(SysJobModel model)
        {
            if (!ModelState.IsValid) return PartialView("_SysJob", model);

            var idSysJob = _jobCache.Save(new SysJobModel
            {
                JobId = model.JobId,
                JobName = model.JobName,
                JobDescription = model.JobDescription,
                CronExpression = model.CronExpression,
                JobLibrary = model.JobLibrary,
                IsActive = model.IsActive,
                IsDeleted = false,
                SavedBy = User.UserName
            });
            if (idSysJob == -9)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName} - [{model.JobLibrary}]", EnumProcessType.DataExisted,
                        EnumMsgIcon.Error)
                });
            if (idSysJob > 0)
            {
                var sFileName = model.FileLibrary != null
                    ? Path.GetFileName(model.FileLibrary.FileName)
                    : model.JobLibrary;
                var jobLibrariesPathFolder = string.Concat(_jobLibrariesPathFolder, _jobFolder);
                var jobLibrariesAbsolutePathFolder = Server.MapPath(jobLibrariesPathFolder);
                var jobLibrariesAbsoluteFilePath = string.Concat(jobLibrariesAbsolutePathFolder, "/", sFileName);

                if (model.FileLibrary != null)
                {
                    if (!Directory.Exists(jobLibrariesAbsolutePathFolder))
                        Directory.CreateDirectory(jobLibrariesAbsolutePathFolder);
                    model.FileLibrary.SaveAs(jobLibrariesAbsoluteFilePath);
                }

                JobSchedulerProvider.DeleteJob(model.JobId.ToString());

                //MemoryStream memStream = new MemoryStream();
                //System.IO.File.Open(jobLibrariesAbsoluteFilePath, FileMode.Open).CopyTo(memStream);

                var jobPlugable = JobPlugableProvider.GetJobPlugable(jobLibrariesAbsoluteFilePath);
                JobSchedulerProvider.UpdateCronExpression(model.JobId.ToString(), model.CronExpression);

                if (jobPlugable != null)
                    ExecuteJob(jobPlugable, model);
            }

            var response = CreateMessage($"{_funcName} [{model.JobName}]", EnumProcessType.Edit,
                idSysJob > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(string id)
        {
            var model = _jobCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"),
                $"<b>{_funcName} [{model.JobName}]</b>");
            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(SysJobModel model)
        {
            model.SavedBy = User.UserName;
            var deleted = _jobCache.Delete(model);
            if (deleted)
            {
                JobSchedulerProvider.DeleteJob(model.JobId.ToString());

                var jobLibrariesPathFolder = string.Concat(_jobLibrariesPathFolder, _jobFolder);
                var jobLibrariesAbsolutePathFolder = Server.MapPath(jobLibrariesPathFolder);
                var jobLibrariesAbsoluteFilePath =
                    string.Concat(jobLibrariesAbsolutePathFolder, "/", model.JobLibrary);
                if (System.IO.File.Exists(jobLibrariesAbsoluteFilePath))
                    System.IO.File.Delete(jobLibrariesAbsoluteFilePath);
            }

            var response = CreateMessage($"{_funcName} [{model.JobName}]", EnumProcessType.Delete,
                deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult ChangeStatus(string id)
        {
            var model = _jobCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(
                model.IsActive
                    ? AppProcessor.Messagor.GetMessage("Deactive_ConfirmMessage")
                    : AppProcessor.Messagor.GetMessage("ReActive_ConfirmMessage"),
                $"<b>{_funcName} [{model.JobName}]</b>");
            return PartialView("_ChangeStatus", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult ChangeStatus(SysJobModel model)
        {
            model.IsActive = !model.IsActive;
            model.SavedBy = User.UserName;
            var isSuccess = _jobCache.ChangeStatus(model);
            if (isSuccess)
            {
                if (model.IsActive)
                    JobSchedulerProvider.ResumeJob(model.JobId.ToString());
                else
                    JobSchedulerProvider.PauseJob(model.JobId.ToString());
            }

            var response = CreateMessage($"{_funcName} [{model.JobName}]", EnumProcessType.Edit,
                isSuccess ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult CronExpression()
        {
            return PartialView("_CronExpression");
        }

        private void ExecuteJob(IJobPlugable mailJobPlugable, SysJobModel sysJobModel)
        {
            var instanceMailJob = mailJobPlugable?.BuildJob(sysJobModel.JobId.ToString(), sysJobModel.JobDescription,
                sysJobModel.CronExpression);
            if (instanceMailJob == null) return;

            instanceMailJob.MainJob.IsActive = sysJobModel.IsActive;
            JobSchedulerProvider.RegisterJobScheduler(instanceMailJob.JobType, instanceMailJob.MainJob);
        }
    }
}