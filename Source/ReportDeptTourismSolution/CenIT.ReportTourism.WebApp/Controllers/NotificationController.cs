using System.Linq;
using System.Web.Mvc;
using CenIT.ReportTourism.Caches.Cate;
using CenIT.ReportTourism.Core.Apps;
using TSFramework.App.Attributes;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.WebApp.Controllers
{
    public class NotificationController : AppController
    {
        private readonly CateEnterpriseRegisterNotifyCache _enterpriseRegisterNotifyCache;

        public NotificationController()
        {
            _enterpriseRegisterNotifyCache = new CateEnterpriseRegisterNotifyCache();
        }

        // GET: Notification
        //[ChildActionOnly]
        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult Index()
        {
            var lstNotifies = _enterpriseRegisterNotifyCache.GetAll().Where(n => !n.HasProcessed).ToList();
            return PartialView("Index", lstNotifies);
        }
    }
}