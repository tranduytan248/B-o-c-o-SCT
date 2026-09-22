using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using TSFramework.App.Attributes;
using TSFramework.App.Extends;
using TSFramework.App.Principals;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;

namespace TSFramework.App.BaseApps
{
    [CustomAuthenticate]
    public class BaseController : Controller
    {
        private const string SCRIPT_MAIN_KEY = "ScriptKey";
        protected new AppPrincipal User => HttpContext.User as AppPrincipal;

        public string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(model);

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public string RenderPartialToString(Controller controller, string partialViewName, object model,
            ViewDataDictionary viewData, TempDataDictionary tempData)
        {
            var result = ViewEngines.Engines.FindPartialView(controller.ControllerContext, partialViewName);

            if (result.View == null) return string.Empty;
            controller.ViewData.Model = model;
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var output = new HtmlTextWriter(sw))
                {
                    var viewContext = new ViewContext(controller.ControllerContext, result.View, viewData, tempData,
                        output);
                    result.View.Render(viewContext, output);
                }
            }

            return sb.ToString();
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            if (ViewData.Keys.Count == 0) return;
            foreach (var m in ViewData.Keys)
                if (m.Contains(SCRIPT_MAIN_KEY))
                    filterContext.RequestContext.HttpContext.Response.Write(ViewData[m]);
        }

        protected new static JsonResult Json(
            object data,
            string contentType = null,
            Encoding contentEncoding = null,
            JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        {
            return new JsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult ActionIsAllow(string controllerName, string actionName, string areaName = null)
        {
            var isAllow = AuthorityExtensions.IsAllow(Request.RequestContext, User.UserName, controllerName, actionName,
                areaName);
            return Json(new {status = isAllow}, JsonRequestBehavior.AllowGet);
        }

        #region Create Message

        public string CreateMessage(string message, EnumProcessType typeProc, EnumMsgIcon icon, string sUrl = "",
            string sTarget = "")
        {
            return AppProcessor.Notifider.CreateMessage(message, typeProc, icon, sUrl, sTarget);
        }

        public string CreateNotify(string message, EnumProcessType typeProc, EnumMsgIcon icon)
        {
            return AppProcessor.Notifider.CreateNotify(message, typeProc, icon);
        }

        public void SendScriptResponse(string sKey, string scriptAction)
        {
            var scriptMesResponse = $@"<script type='text/javascript' language='javascript'>{scriptAction}</script>";
            ViewData[$"{SCRIPT_MAIN_KEY}_{sKey}"] = scriptMesResponse;
        }

        public void SendResponseNotify(string sKey, string message, EnumProcessType typeProc, EnumMsgIcon icon)
        {
            var mesNotify = AppProcessor.Notifider.CreateNotify(message, typeProc, icon);
            var scriptMesNotify = $@"<script type='text/javascript' language='javascript'>{mesNotify}</script>";

            ViewData[$"{SCRIPT_MAIN_KEY}_{sKey}"] = scriptMesNotify;
        }

        #endregion
    }
}