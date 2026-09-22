using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using CenIT.ReportTourism.Caches.Sys;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.WebApp.Models;
using TSFramework.App.Attributes;
using TSFramework.App.BaseApps;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.WebApp.Controllers
{
    public class HomeController : AppController
    {
        private readonly SysLayoutCache _sysLayoutCache;
        private readonly SysModuleCache _sysModuleCache;

        public HomeController()
        {
            _sysLayoutCache = new SysLayoutCache();
            _sysModuleCache = new SysModuleCache();
        }

        [ActionType(Type = EnumActionType.View)]
        public ActionResult Index()
        {
            //var activatedLayout = _sysLayoutCache.GetActivatedLayout();
            //var skinName = activatedLayout == null ? "_Default" : activatedLayout.LayoutName;
            //var layout = _sysLayoutCache.GetByName(skinName);
            //layout = layout ?? new AppLayoutModel();
            //layout.ListContentPanels = _sysContentPanelCache.GetByLayoutId(layout.Layout_ID);
            //if (MappingModuleWidget != null && MappingModuleWidget.Count > 0) return View(layout);
            //InitModulePanel();
            //return View(layout);

            var lstModuleBelongUsers = _sysModuleCache.GetByUserName(User.UserName);
            return View(lstModuleBelongUsers);
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult RenderModule(string widgetId)
        {
            var listModulesHtml = new List<PanelModuleHtmlModel>();
            if (MappingModuleWidget.Count == 0) InitModulePanel();

            if (!MappingModuleWidget.ContainsKey(widgetId)) return Json(null, JsonRequestBehavior.AllowGet);

            string moduleName = null;
            var listModules = MappingModuleWidget[widgetId];
            foreach (var module in listModules)
            {
                Assembly asm;
                try
                {
                    moduleName = module?.ModuleName;
                    asm = module?.AssemblyName != null ? Assembly.Load(module.AssemblyName) : null;
                    //asm = Assembly.Load($"Modules.{moduleName}");
                }
                catch
                {
                    asm = null;
                }

                string dataModuleHtml;
                if (asm == null || string.IsNullOrEmpty(module.MainController))
                {
                    dataModuleHtml = string.Empty;
                }
                else
                {
                    var moduleController = ControllerBuilder.Current.GetControllerFactory()
                        .CreateController(ControllerContext.RequestContext, module.MainController) as BaseController;
                    if (moduleController == null)
                    {
                        dataModuleHtml = string.Empty;
                    }
                    else
                    {
                        moduleController.ControllerContext =
                            new ControllerContext(Request.RequestContext, moduleController);
                        dataModuleHtml = RenderPartialToString(moduleController, $@"~/Views/{module.ModuleView}",
                            null, ViewData, TempData);
                        //dataModuleHtml = RenderPartialToString(moduleController, $@"~/Views/{moduleName}/_View.cshtml",
                        //    null, ViewData, TempData);
                    }
                }

                listModulesHtml.Add(new PanelModuleHtmlModel
                {
                    ModuleHtml = dataModuleHtml, ModuleName = moduleName, ModuleId = module.ModuleId,
                    OrderBy = module.OrderBy
                });
            }

            return Json(listModulesHtml, JsonRequestBehavior.AllowGet);
        }

        //[ActionType(Type = EnumActionType.Edit)]
        //public ActionResult ClearCache(string returnUrl = "")
        //{
        //    HttpRuntime.UnloadAppDomain();
        //    if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
        //    return RedirectToAction("Index");
        //}
    }
}