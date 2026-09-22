using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Helpers;

namespace TSFramework.App.Extends
{
    public static class AuthorityExtensions
    {
        public static bool IsAllow(RequestContext requestContext, string userName, string controllerName,
            string actionName, string areaName = null)
        {
            try
            {
                var namespaces = RouteTable.Routes.OfType<Route>()
                    .Where(d => d.DataTokens != null && d.DataTokens.ContainsKey("area") &&
                                (areaName == null || d.DataTokens["area"].Equals(areaName)))
                    .Select(r => r.DataTokens).ToArray()[0]["Namespaces"];

                var lstNamespaces = new List<string>((string[]) namespaces);

                var tempRequestContext =
                    new RequestContext(requestContext.HttpContext.Request.RequestContext.HttpContext, new RouteData());

                tempRequestContext.RouteData.DataTokens["Area"] = areaName;
                tempRequestContext.RouteData.DataTokens["Namespaces"] = lstNamespaces;

                tempRequestContext.RouteData.Values["Area"] = areaName;
                tempRequestContext.RouteData.Values["Namespaces"] = lstNamespaces;

                var factory = ControllerBuilder.Current.GetControllerFactory();
                var controller = factory.CreateController(tempRequestContext, controllerName) as ControllerBase;

                var controllerContext = new ControllerContext(tempRequestContext, controller);
                var controllerDescriptor = new ReflectedControllerDescriptor(controller?.GetType());
                var actionDescriptor = controllerDescriptor.FindAction(controllerContext, actionName);
                //var areaName = controllerContext.RouteData.DataTokens["area"] as string;
                areaName = areaName ?? controllerContext.RouteData.DataTokens["area"] as string;
                var actionTypes = actionDescriptor
                    .GetCustomAttributes(typeof(ActionTypeAttribute), false);
                var currentActionType = actionTypes.Length > 0 ? actionTypes[0] as ActionTypeAttribute : null;
                if (currentActionType == null) return false;
                var actionTypeName = EnumHelper.GetDescription(currentActionType.Type);
                var isAllow = AppProcessor.Author.IsAllow(userName, areaName, controllerName, actionTypeName);
                return isAllow;
            }
            catch
            {
                return false;
            }
        }

        //public static bool IsAllow(RequestContext requestContext, string userName, string areaName, string controllerName, string actionName)
        //{
        //    try
        //    {
        //        IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
        //        ControllerBase controller = factory.CreateController(requestContext, controllerName) as ControllerBase;
        //        var controllerContext = new ControllerContext(requestContext, controller);
        //        var controllerDescriptor = new ReflectedControllerDescriptor(controller?.GetType());
        //        var actionDescriptor = controllerDescriptor.FindAction(controllerContext, actionName);
        //        var areaName = controllerContext.RouteData.DataTokens["area"] as string;
        //        var actionTypes = actionDescriptor
        //            .GetCustomAttributes(typeof(ActionTypeAttribute), false);
        //        var currentActionType = actionTypes.Length > 0 ? actionTypes[0] as ActionTypeAttribute : null;
        //        if (currentActionType == null) return false;
        //        var actionTypeName = EnumHelper.GetDescription(currentActionType.Type);
        //        var isAllow = AppProcessor.Author.IsAllow(userName, EnumHelper.GetDescription(EnumAppCode.WebApp),
        //            areaName, controllerName, actionTypeName);
        //        return isAllow;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
    }
}