using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Models.Sys;
using CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Models;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Controllers
{
    public class AppConfigController : AppController
    {
        private readonly string _funcName = AppProcessor.Messagor.GetMessage("AppConfig_Title");
        private readonly string _webConfigFile = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Web.config");

        // GET: Sys/AppConfig
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Index()
        {
            //var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //config.AppSettings.Settings.Add("OS", "Linux");
            //config.Save(ConfigurationSaveMode.Modified);

            //ConfigurationManager.RefreshSection("appSettings");

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
            var lstAppConfigs = ReadAllConfigs();
            var total = lstAppConfigs.Count;

            var data = lstAppConfigs
                .FindAll(c =>
                    string.IsNullOrEmpty(dataSearch.Search) || c.AppValue.Contains(dataSearch.Search) ||
                    c.AppKey.Contains(dataSearch.Search))
                .Skip(dataSearch.StartIndex)
                .Take(dataSearch.PageSize);

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
            var model = new AppConfigModel();
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Add)]
        public ActionResult Add(AppConfigModel model)
        {
            if (!ModelState.IsValid) return PartialView("_AppConfig", model);
            var configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = _webConfigFile };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            if (config.AppSettings.Settings.AllKeys.Contains(model.AppKey))
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName} [{model.AppKey}]",
                        EnumProcessType.DataExisted, EnumMsgIcon.Error)
                });

            config.AppSettings.Settings.Add(model.AppKey, model.AppValue);
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");

            var response = CreateMessage($"{_funcName} [{model.AppKey}]", EnumProcessType.Add, EnumMsgIcon.Success);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(string appKey)
        {
            var lstAppConfigs = ReadAllConfigs();
            var model = lstAppConfigs.First(ac => ac.AppKey == appKey);
            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(AppConfigModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_AppConfig", model);

            var configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = _webConfigFile };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            if (config.AppSettings.Settings.AllKeys.Contains(model.AppKey))
            {
                config.AppSettings.Settings[model.AppKey].Value = model.AppValue;
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            else
            {
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName} [{model.AppKey}]",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            }

            var response = CreateMessage($"{_funcName} [{model.AppKey}]", EnumProcessType.Edit, EnumMsgIcon.Success);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(string appKey)
        {
            var lstAppConfigs = ReadAllConfigs();

            var model = lstAppConfigs.First(ac => ac.AppKey == appKey);
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"),
                $"<b>{_funcName} [{model.AppKey}]</b>");

            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(AppConfigModel model)
        {
            var configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = _webConfigFile };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            if (config.AppSettings.Settings.AllKeys.Contains(model.AppKey))
            {
                config.AppSettings.Settings.Remove(model.AppKey);
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            else
            {
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName} [{model.AppKey}]",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            }

            var response = CreateMessage($"{_funcName} [{model.AppKey}]", EnumProcessType.Delete, EnumMsgIcon.Success);
            return Json(new { status = true, message = response });
        }

        private List<AppConfigModel> ReadAllConfigs()
        {
            var lstConfigs = new List<AppConfigModel>();
            foreach (var sKey in ConfigurationManager.AppSettings.AllKeys)
                lstConfigs.Add(new AppConfigModel
                {
                    AppKey = sKey,
                    AppValue = ConfigurationManager.AppSettings[sKey]
                });

            return lstConfigs;
        }
    }
}