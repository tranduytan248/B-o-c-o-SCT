using System.Linq;
using System.Web.Mvc;
using TSFramework.App.Extends;
using TSFramework.App.Principals;

namespace TSFramework.App.BaseApps
{
    public abstract class BaseViewPage : WebViewPage
    {
        protected new virtual AppPrincipal User => base.User as AppPrincipal;

        public string RenderButton(bool isModal, string modalId, string eleClass, string urlAction, string icon,
            string title)
        {
            var pActions = !string.IsNullOrEmpty(urlAction) ? urlAction.Split('/') : new string[] { };
            pActions = pActions.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            if (pActions.Length <= 2) return string.Empty;
            if (pActions.Length == 2)
                if (!AuthorityExtensions.IsAllow(Request.RequestContext,
                    Request.RequestContext.HttpContext.User.Identity.Name, pActions[0], pActions[1]))
                    return string.Empty;

            if (pActions.Length == 3)
                if (!AuthorityExtensions.IsAllow(Request.RequestContext,
                    Request.RequestContext.HttpContext.User.Identity.Name, pActions[1], pActions[2], pActions[0]))
                    return string.Empty;

            var sModal = isModal ? "data-modal=''" : "";
            var buttonTemplate =
                $"<a {sModal} data-modal-id='{modalId}' class='{eleClass}' href='{urlAction}'>{icon}&nbsp;{title}</a>";
            return buttonTemplate;
        }
    }

    public abstract class BaseViewPage<TModel> : WebViewPage<TModel>
    {
        protected new virtual AppPrincipal User => base.User as AppPrincipal;

        protected string RenderButton(bool isModal, string modalId, string eleClass, string urlAction, string icon,
            string title)
        {
            var pActions = !string.IsNullOrEmpty(urlAction) ? urlAction.Split('/') : new string[] { };
            pActions = pActions.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            if (pActions.Length <= 2) return string.Empty;
            if (pActions.Length == 2)
                if (!AuthorityExtensions.IsAllow(Request.RequestContext,
                    Request.RequestContext.HttpContext.User.Identity.Name, pActions[0], pActions[1]))
                    return string.Empty;

            if (pActions.Length == 3)
                if (!AuthorityExtensions.IsAllow(Request.RequestContext,
                    Request.RequestContext.HttpContext.User.Identity.Name, pActions[1], pActions[2], pActions[0]))
                    return string.Empty;

            var sModal = isModal ? "data-modal=''" : "";
            var buttonTemplate =
                $"<a {sModal} data-modal-id='{modalId}' class='{eleClass}' href='{urlAction}'>{icon}&nbsp;{title}</a>";
            return buttonTemplate;
        }
    }
}