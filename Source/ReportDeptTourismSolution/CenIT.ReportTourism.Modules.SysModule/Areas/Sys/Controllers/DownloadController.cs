using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using TSFramework.App.Attributes;
using TSFramework.App.BaseApps;
using TSFramework.App.Extends;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Controllers
{
    public class DownloadController : BaseController
    {
        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult DownloadSource()
        {
            var settingDownloadFolders = ConfigurationManager.AppSettings["AppDownloadFolders"];
            var lstDownloadFolders = settingDownloadFolders.Split(','); //new List<string>();

            //{
            //    //"App_Data",
            //    //"Areas",
            //    //"bin",
            //    //"Configs",
            //    //"Content",
            //    //"dlls",
            //    //"EmailTemplates",
            //    //"Imports",
            //    //"libs",
            //    //"Reports",
            //    //"Templates",
            //    //"TSFramework",
            //    //"Views",
            //    //"XmlTemplates"
            //};

            var publicHtmlFolder = HostingEnvironment.MapPath("/");
            var zipFileName = $"PublicHtml-{DateTime.Now:yyyyMMddHHmmss}.zip";

            byte[] bytes = null;

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var folder in Directory.GetDirectories(publicHtmlFolder, "*", SearchOption.AllDirectories))
                    {
                        var folderInfo = new DirectoryInfo(folder);
                        if (!lstDownloadFolders.Contains(folderInfo.Name)) continue;
                        archive.CreateEntryFromAny(folder);
                    }
                }

                bytes = memoryStream.ToArray();
            }

            return File(bytes, "application/zip", zipFileName);
        }
    }
}