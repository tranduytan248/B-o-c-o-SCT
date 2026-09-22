using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using CenIT.Libs.VNPTSmartCA.Models.Responses;
using CenIT.Libs.VNPTSmartCA.Providers;
using CenIT.Libs.VNPTSmartCA.Services;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Core.Helpers;
using CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Models;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;
using VnptHashSignatures.Interface;

namespace CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Controllers
{
    public class DigitalSignController : AppController
    {
        private readonly string _digitalSignTitle = AppProcessor.Messagor.GetMessage("DigitalSign_Title");

        private readonly string _signedFilesPathFolder =
            ConfigurationManager.AppSettings["Modules_Sys_SignedDoc_FolderPath"] ?? @"/Contents/Modules/Sys/SignedDoc/";

        private readonly Dictionary<int, string> _transactionStatus = new Dictionary<int, string>
        {
            {1, "SUCCESS"},
            {4000, "WAITING_FOR_SIGNER_CONFIRM"},
            {4001, "EXPIRED"},
            {4002, "SIGNER_REJECTED"},
            {4003, "AUTHORIZE_KEY_FAILED"},
            {4004, "SIGN_FAILED"}
        };

        // GET: Sys/DigitalSign
        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult Index()
        {
            var model = new DigitalSignModel
            {
                AccessToken = (string) Session[$"VNPT-SmartCA-{User?.UserName}-AccessToken"]
            };

            return View(model);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult UploadSignedSata(SignedDataModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.SignedData))
                return Json(new
                {
                    status = true,
                    message = CreateMessage("Dữ liệu ký số", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            if (!Directory.Exists(_signedFilesPathFolder)) Directory.CreateDirectory(_signedFilesPathFolder);

            var bytes = Convert.FromBase64String(model.SignedData);
            System.IO.File.WriteAllBytes(
                $"{Path.Combine(_signedFilesPathFolder, $"{model.FileName}.{model.FileType}")}", bytes);

            return Json(new
            {
                status = true,
                message = CreateMessage("Upload Dữ liệu ký số", EnumProcessType.NonFormat, EnumMsgIcon.Success)
            });
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult DownloadSignedFile(string fileName)
        {
            var fullSignedPath = HostingEnvironment.MapPath("/" + _signedFilesPathFolder);
            var fullPathSignedFile = $"{Path.Combine(fullSignedPath, fileName)}";
            if (!System.IO.File.Exists(fullPathSignedFile))
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"Tệp tin Dữ liệu ký số [{fileName}]",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            var fileBytes = System.IO.File.ReadAllBytes(fullPathSignedFile);
            var contentType = MimeMapping.GetMimeMapping(fileName);

            System.IO.File.Delete(fullPathSignedFile);

            return File(fileBytes, contentType, fileName);
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult LoginSmartCA()
        {
            return PartialView("_Login", new LoginSmartCAModel());
        }

        [ActionType(Type = EnumActionType.Edit)]
        [HttpPost]
        public ActionResult LoginSmartCA(LoginSmartCAModel model)
        {
            if (!ModelState.IsValid) return PartialView("_LoginSmartCA", model);

            string sRefreshToken;
            UserInfoModel userInfo = null;

            var accessToken = VNPTSmartCAProvider.AuthToken(model.UserName, model.Password, out sRefreshToken);
            if (!string.IsNullOrEmpty(accessToken)) userInfo = VNPTSmartCAProvider.GetUserInfo(accessToken);

            if (string.IsNullOrEmpty(accessToken))
                return Json(new
                {
                    status = false,
                    message = CreateMessage(
                        $"{AppProcessor.Messagor.GetMessage("Login_SmartCA_Label")} thất bại. Tài khoản hoặc mật khẩu không đúng.",
                        EnumProcessType.NonFormat, EnumMsgIcon.Error)
                });

            if (userInfo != null)
                Session[$"VNPT-SmartCA-{User?.UserName}-UserInfo"] = userInfo;

            Session[$"VNPT-SmartCA-{User?.UserName}-AccessToken"] = accessToken;

            return Json(new
            {
                status = true,
                message = CreateMessage($"{AppProcessor.Messagor.GetMessage("Login_SmartCA_Label")} thành công",
                    EnumProcessType.NonFormat, EnumMsgIcon.Success)
            });
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        [AjaxOnly]
        public ActionResult SignoutSmartCA()
        {
            Session[$"VNPT-SmartCA-{User?.UserName}-AccessToken"] = null;
            Session[$"VNPT-SmartCA-{User?.UserName}-UserInfo"] = null;

            return Json(new
            {
                status = true,
                message = ""
            });
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult UserInfo()
        {
            var userInfo = (UserInfoModel) Session[$"VNPT-SmartCA-{User?.UserName}-UserInfo"];

            return PartialView("_UserInfo", userInfo ?? new UserInfoModel());
        }

        [ActionType(Type = EnumActionType.Edit)]
        [HttpPost]
        public ActionResult Sign(DigitalSignModel model)
        {
            if (model.FileImports == null)
                return Json(new
                {
                    status = false,
                    errorCode = 0,
                    message = CreateMessage("Không có tệp dữ liệu", EnumProcessType.NonFormat, EnumMsgIcon.Error)
                });

            var accessToken = (string) Session[$"VNPT-SmartCA-{User?.UserName}-AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
                return Json(new
                {
                    status = false,
                    errorCode = 1,
                    message = CreateMessage("Bạn chưa đăng nhập tài khoản Smart CA", EnumProcessType.NonFormat,
                        EnumMsgIcon.Error)
                });

            var fullSignedPath = HostingEnvironment.MapPath("/" + _signedFilesPathFolder);
            if (!Directory.Exists(fullSignedPath) && !string.IsNullOrEmpty(fullSignedPath))
                Directory.CreateDirectory(fullSignedPath);
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(model.FileImports.FileName);
            var fileExt = Path.GetExtension(model.FileImports.FileName);
            var fileNameSigned = $"{fileNameWithoutExt}_signed{fileExt}";
            var fileSignedFullPath = Path.Combine(fullSignedPath, fileNameSigned);
            int isSuccess = 0;
            try
            {
                isSuccess = SignHash(accessToken, model.FileImports, fileSignedFullPath);
            }
            catch (Exception e)
            {
                AppProcessor.Logger.Error(e);
                return Json(new
                {
                    status = false,
                    errorCode = 1,
                    message = CreateMessage(
                        $"Thực hiện {_digitalSignTitle} tệp tin {model.FileImports.FileName} thất bại. {e.Message}",
                        EnumProcessType.NonFormat, EnumMsgIcon.Error)
                });
            }
            if (isSuccess == 0)
                return Json(new
                {
                    status = true,
                    fileName = fileNameSigned,
                    message = CreateMessage(
                        $"Thực hiện {_digitalSignTitle} tệp tin {model.FileImports.FileName} thành công.",
                        EnumProcessType.NonFormat, EnumMsgIcon.Success)
                });
            var errMsg = "";
            switch (isSuccess)
            {
                case 1:
                    errMsg = "Ký số thất bại";
                    break;
                case 2:
                    errMsg = "Lỗi thông tin chữ ký số";
                    break;
                case 3:
                    errMsg = "Người dùng không xác nhận ký số từ ứng dụng";
                    break;
                case 4:
                    errMsg = "Lỗi chữ ký số";
                    break;
                case 5:
                    errMsg = "Chữ ký số không khớp";
                    break;
                case 6:
                    errMsg = "Từ chối ký số";
                    break;
            }

            return Json(new
            {
                status = false,
                errorCode = 1,
                message = CreateMessage(
                    $"Thực hiện {_digitalSignTitle} tệp tin {model.FileImports.FileName} thất bại. {errMsg}",
                    EnumProcessType.NonFormat, EnumMsgIcon.Error)
            });
        }

        /// <summary>
        ///     Ký văn bản
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="fileImport"></param>
        /// <param name="fullSignedPath"></param>
        /// <returns>
        ///     - 0: thành công
        ///     - 1: ký thất bại
        ///     - 2: lỗi thông tin chữ ký số
        ///     - 3: người dùng không xác nhận ký số từ App
        ///     - 4: lỗi chữ ký
        ///     - 5: chữ ký không khớp
        /// </returns>
        private int SignHash(string accessToken, HttpPostedFileBase fileImport, string fullSignedPath)
        {
            var lstCredentials = VNPTSmartCAProvider.GetListCredentials(accessToken);
            var credentialId = lstCredentials?[0];
            var credentialInfo =
                VNPTSmartCAProvider.GetCredentialInfo(accessToken, credentialId, CTSType.Chain, true, true);
            var certBase64 = credentialInfo?.Cert?.Certificates?[0]?.Replace("\r\n", "");
            var fileData = StreamHelper.ReadFully(fileImport.InputStream);

            var typeExt = HashSignerFactory.OFFICE;
            //var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileImport.FileName);
            var fileExt = Path.GetExtension(fileImport.FileName);

            if (fileExt?.ToUpper() == ".PDF")
                typeExt = HashSignerFactory.PDF;
            else if (fileExt?.ToUpper() == ".XML") typeExt = HashSignerFactory.XML;

            IHashSigner signer;
            var tranId = VNPTSmartCAProvider.SignHash(out signer, accessToken, credentialId, certBase64, "",
                fileImport.FileName, fileData, typeExt);
            if (string.IsNullOrEmpty(tranId)) return 1;

            var iCount = 0;
            var isConfirm = false;
            var datasigned = "";
            var tranStatus = -1;

            while (iCount < 24 && !isConfirm)
            {
                AppProcessor.Logger.Message("Lấy thông tin lần " + iCount + " : ");
                var tranInfo = VNPTSmartCAProvider.GetTranInfo(accessToken, tranId);
                tranStatus = tranInfo?.TranStatus ?? -1;

                if (tranInfo != null)
                {
                    if (tranInfo.TranStatus == 4000)
                    {
                        iCount = iCount + 1;
                        Thread.Sleep(5000);
                    }
                    else if (tranInfo.TranStatus == 4002)
                    {
                        datasigned = null;
                        break;
                    }
                    else
                    {
                        isConfirm = true;
                        datasigned = tranInfo.Documents[0].Sig;
                    }
                }
                else
                {
                    AppProcessor.Logger.Message("Error from content");
                    return 2;
                }
            }

            var msgStatus = _transactionStatus[tranStatus];
            if (!isConfirm && datasigned == null)
            {
                AppProcessor.Logger.Message($"Signer rejected - {msgStatus}");
                return 6;
            }

            if (!isConfirm)
            {
                AppProcessor.Logger.Message($"Signer not confirm from App - {msgStatus}");
                return 3;
            }

            if (string.IsNullOrEmpty(datasigned))
            {
                AppProcessor.Logger.Message($"Sign error - {msgStatus}");
                return 4;
            }

            if (!signer.CheckHashSignature(datasigned))
            {
                AppProcessor.Logger.Message($"Signature not match - {msgStatus}");
                return 5;
            }
            // ------------------------------------------------------------------------------------------

            // 3. Package external signature to signed file
            var signed = signer.Sign(datasigned);
            System.IO.File.WriteAllBytes(fullSignedPath, signed);
            return 0;
        }
    }
}