using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Web.Mvc;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.WebApp.Models;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.WebApp.Controllers
{
    [AllowAnonymous]
    public class SysLogController : AppController
    {
        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult Index()
        {
            var listErrFile = new List<FileInfo>();
            var dirErrLogs =
                Path.Combine(HostingEnvironment.MapPath("/" + ConfigurationManager.AppSettings["LogPath"]));
            var errFiles = Directory.GetFiles(dirErrLogs);

            foreach (var file in errFiles)
            {
                var fi = new FileInfo(file);
                if (fi.LastAccessTime.Month == DateTime.Now.Month) listErrFile.Add(fi);
            }

            listErrFile = listErrFile.OrderByDescending(fi => fi.LastAccessTime).Take(10).ToList();

            return View(new SysLogModel {ListErrFile = listErrFile});
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult ListErrFile(DateTime? fromMonth, DateTime? toMonth)
        {
            fromMonth = fromMonth ?? DateTime.Now;
            toMonth = toMonth ?? DateTime.Now;

            var listErrFile = new List<FileInfo>();
            var dirErrLogs =
                Path.Combine(HostingEnvironment.MapPath("/" + ConfigurationManager.AppSettings["LogPath"]));
            var errFiles = Directory.GetFiles(dirErrLogs);

            foreach (var file in errFiles)
            {
                var fi = new FileInfo(file);
                if (fi.CreationTime.Month >= fromMonth.Value.Month && fi.CreationTime.Month <= toMonth.Value.Month) listErrFile.Add(fi);
            }

            listErrFile = listErrFile.OrderByDescending(fi => fi.LastAccessTime).Take(30).ToList();

            return PartialView("_ListErrFile", new SysLogModel { ListErrFile = listErrFile });
        }

        [ActionType(Type = EnumActionType.View)]
        [AjaxOnly]
        [HttpGet]
        public ActionResult DeleteFile(string fileName, string type = "Err")
        {
            var fullPathFile = string.Empty;

            if (type == "Err")
                fullPathFile =
                    Path.Combine(HostingEnvironment.MapPath("/" + ConfigurationManager.AppSettings["LogPath"]),
                        fileName);

            if (!System.IO.File.Exists(fullPathFile))
                return Json(new
                {
                    status = true,
                    message = CreateMessage("File Log", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"),
                $"File Log: <b>{fileName}</b>");
            var fi = new FileInfoModel {Name = fileName, FullName = fullPathFile};

            return PartialView("_Delete", fi);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteFile(FileInfoModel fi)
        {
            var isDeleted = false;
            if (System.IO.File.Exists(fi.FullName))
            {
                System.IO.File.Delete(fi.FullName);
                isDeleted = true;
            }

            var response = CreateMessage($"File Log: <b>{fi.Name}</b>", EnumProcessType.Delete,
                isDeleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new
            {
                status = true, message = response,
                fileName = Regex.Replace(fi.Name, @"[^0-9a-zA-Z]+", "", RegexOptions.Compiled)
            });
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult DownloadFile(string fileName, string type = "Err")
        {
            var fullPathFile = string.Empty;

            if (type == "Err")
                fullPathFile =
                    Path.Combine(HostingEnvironment.MapPath("/" + ConfigurationManager.AppSettings["LogPath"]),
                        fileName);

            if (!System.IO.File.Exists(fullPathFile))
                return Json(new
                {
                    status = true,
                    message = CreateMessage("File Log", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            var bytes = System.IO.File.ReadAllBytes(fullPathFile);
            return File(bytes, "text/plain", fileName);
        }

        [ActionType(Type = EnumActionType.View)]
        [AjaxOnly]
        [HttpGet]
        public ActionResult ViewErrFile(string fileName)
        {
            var fullPathFile =
                Path.Combine(HostingEnvironment.MapPath("/" + ConfigurationManager.AppSettings["LogPath"]), fileName);
            if (!System.IO.File.Exists(fullPathFile))
                return Json(new
                {
                    status = true,
                    message = CreateMessage("File Log", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            var fileContents = System.IO.File.ReadAllLines(fullPathFile);
            return PartialView("_ViewFile", string.Join(Environment.NewLine, fileContents));
        }

        [ActionType(Type = EnumActionType.View)]
        [AjaxOnly]
        [HttpGet]
        public ActionResult DeleteOldFile()
        {
            return PartialView("_DeleteOldFile");
        }

        [ActionType(Type = EnumActionType.View)]
        [AjaxOnly]
        [HttpPost]
        public ActionResult DeleteOldFile(int monthAgo)
        {
            var dirErrLogs =
                Path.Combine(HostingEnvironment.MapPath("/" + ConfigurationManager.AppSettings["LogPath"]));
            var errFiles = Directory.GetFiles(dirErrLogs);

            foreach (var file in errFiles)
            {
                var fi = new FileInfo(file);
                if (fi.LastAccessTime < DateTime.Now.AddMonths(-1 * monthAgo)) fi.Delete();
            }

            var response = CreateMessage($"Đã Xoá file log của <b>{monthAgo}</b> tháng trước",
                EnumProcessType.Delete, EnumMsgIcon.Success);
            return Json(new {status = true, message = response});
        }

    }
}