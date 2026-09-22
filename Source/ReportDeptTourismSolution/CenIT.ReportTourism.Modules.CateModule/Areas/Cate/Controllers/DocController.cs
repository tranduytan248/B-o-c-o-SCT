using System.Web.Mvc;
using CenIT.ReportTourism.Caches.Cate;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Models.Cate;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;

namespace CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Controllers
{
    public class DocController : AppController
    {
        private readonly CateDocCache _docCache;
        private readonly string _docTitle = AppProcessor.Messagor.GetMessage("CateDoc_Title");

        public DocController()
        {
            _docCache = new CateDocCache();
        }

        [HttpGet]
        [AjaxOnly]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult RefDoc(string fileId)
        {
            var cateDoc = _docCache.GetById(fileId);
            if (cateDoc == null)
                return Json(new
                {
                    status = false,
                    message = CreateMessage($"{_docTitle}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            var fullPathFile = Server.MapPath($@"{cateDoc.FilePath}\{cateDoc.FileId}{cateDoc.FileExt}");
            if (!System.IO.File.Exists(fullPathFile))
                return Json(new
                {
                    status = false,
                    message = CreateMessage($"{_docTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            return Json(new
            {
                status = true,
                downloadPath = Url.Action("DownloadDoc", new { fileId })
            });
        }

        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult DownloadDoc(string fileId)
        {
            var cateDoc = _docCache.GetById(fileId);
            if (cateDoc == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_docTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            var fullPathFile = Server.MapPath($@"{cateDoc.FilePath}\{cateDoc.FileId}{cateDoc.FileExt}");
            if (!System.IO.File.Exists(fullPathFile))
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_docTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            var arrBytes = System.IO.File.ReadAllBytes(fullPathFile);

            return File(arrBytes, cateDoc.ContentType, cateDoc.FileName + cateDoc.FileExt);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteDoc(string fileId)
        {
            var cateDoc = _docCache.GetById(fileId);
            if (cateDoc == null)
                return Json(new
                {
                    status = false,
                    message = CreateMessage($"{_docTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            var fullPathFile =
                Server.MapPath($@"{cateDoc.FilePath}\{cateDoc.FileId.ToString().ToUpper()}{cateDoc.FileExt}");
            if (!System.IO.File.Exists(fullPathFile))
                return Json(new
                {
                    status = false,
                    message = CreateMessage($"{_docTitle}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Common_ConfirmMessage"),
                $"<b>{_docTitle} [{cateDoc.FileName}{cateDoc.FileExt}]</b>");
            return PartialView("_DeleteDoc", cateDoc);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteDoc(CateDocModel model)
        {
            model.Reason = $"Xoá {_docTitle}";
            model.SavedBy = User.Email;

            var deleted = _docCache.Delete(model);
            if (deleted)
            {
                var fullPathFile =
                    Server.MapPath($@"{model.FilePath}\{model.FileId.ToString().ToUpper()}{model.FileExt}");
                if (System.IO.File.Exists(fullPathFile)) System.IO.File.Delete(fullPathFile);
            }

            var response = CreateMessage($"{_docTitle} [{model.FileName}{model.FileExt}]",
                EnumProcessType.Delete, deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response, fileId = model.FileId });
        }
    }
}