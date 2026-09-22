using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using CaptchaMvc.HtmlHelpers;
using CenIT.Libs.VNPTSmartCA.Models.Responses;
using CenIT.Libs.VNPTSmartCA.Providers;
using CenIT.Libs.VNPTSmartCA.Services;
using CenIT.ReportTourism.Caches.Cate;
using CenIT.ReportTourism.Caches.Sys;
using CenIT.ReportTourism.Core.Apps;
using CenIT.ReportTourism.Core.Conts;
using CenIT.ReportTourism.Core.Helpers;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Models;
using CenIT.ReportTourism.WebApp.Models;
using FastMember;
using TSFramework.App.Attributes;
using TSFramework.App.Processors;
using TSFramework.Core.Enums;
using TSFramework.Core.Helpers;
using TSFramework.Core.Members.Mail;
using TSFramework.Core.Providers;
using TSFramework.Core.Utils;
using VnptHashSignatures.Interface;
using ResetPasswordModel = CenIT.ReportTourism.WebApp.Models.ResetPasswordModel;
using reCAPTCHA.MVC;

namespace CenIT.ReportTourism.WebApp.Controllers
{
    [AllowAnonymous]
    public class AccountController : AppController
    {
        #region Properties

        private const string SESSION_VARIABLE_NAME = "SessionNumber";
        private readonly SysConfigsCache _configsCache = new SysConfigsCache();
        private readonly CateEnterpriseCache _enterpriseCache = new CateEnterpriseCache();

        private readonly string _enterpriseFolder = "Enterprise";
        private readonly CateEnterpriseRegisterNotifyCache _enterpriseRegisterNotifyCache = new CateEnterpriseRegisterNotifyCache();
        private readonly string _enterpriseTitle = AppProcessor.Messagor.GetMessage("Enterprise_Label");

        private readonly string _moduleRefDocsPathFolder =
            ConfigurationManager.AppSettings["AttachmentFolderPath"] ?? @"/Contents/Modules/Cate/Attachments/";
        //private readonly SysConfigsCache _configsCache;

        private readonly CateProvinceCache _provinceCache = new CateProvinceCache();
        private readonly SysUserCache _userCache = new SysUserCache();
        private readonly CateWardCache _wardCache = new CateWardCache();

        #endregion

        #region Login & Logout

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = "")
        {
            var model = new LoginModel();
            if (Session[SESSION_VARIABLE_NAME] == null) Session[SESSION_VARIABLE_NAME] = 0;

            //var typeMembershipProvider = ConfigurationManager.AppSettings["AppMembershipType"];
            //var typeMembershipProviderNotRedirect = ConfigurationManager.AppSettings["MembershipProviderNotRedirect"];

            //if (string.IsNullOrEmpty(typeMembershipProviderNotRedirect) ||
            //    !typeMembershipProviderNotRedirect.Split(',').Contains(typeMembershipProvider))
            //{
            //    var isAuthen = Membership.ValidateUser("", "");
            //    if (isAuthen) return RedirectToAction("Index", "Home");
            //    var sSsoUrlService = ConfigurationManager.AppSettings["SSO_URLService"];
            //    var sSsoAppCode = ConfigurationManager.AppSettings["SSO_AppCode"];
            //    var sSsoUrlLogin = $"{sSsoUrlService}Login.aspx?appcode={sSsoAppCode}";
            //    return Redirect(sSsoUrlLogin);
            //}

            ViewBag.ReturnUrl = returnUrl;
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            if (Request.IsAjaxRequest())
                Response.StatusCode = 401;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl = "")
        {
            if (model.NeedCaptcha)
                if (!this.IsCaptchaValid("Captcha không chính xác"))
                    return View(model);

            var iRequestCount = 0;
            if (Session[SESSION_VARIABLE_NAME] == null) // should not happen!
            {
                Session[SESSION_VARIABLE_NAME] = 0;
            }
            else
            {
                var n = (int)Session[SESSION_VARIABLE_NAME];
                n++;
                Session[SESSION_VARIABLE_NAME] = n;
                iRequestCount = n;
            }

            if (iRequestCount >= 3) model.NeedCaptcha = true;

            if (!ModelState.IsValidField("UserName") && !ModelState.IsValidField("Email") ||
                !ModelState.IsValidField("Password")) return PartialView("_Login", model);

            model.SenderIP = Request.UserHostAddress;
            model.SenderHeader = string.Join(",", Request.Headers);

            var isAuthen = Membership.ValidateUser(model.Email, model.Password);
            if (!isAuthen)
            {
                var msgAuthIncorrect = AppProcessor.Messagor.GetMessage("Authorize_LoginIncorrect");
                SendResponseNotify("MsgLoginFail", msgAuthIncorrect, EnumProcessType.NonFormat, EnumMsgIcon.Error);
                _userCache.SaveLogin(model.UserName, false, model.SenderIP, model.SenderHeader);
                model.NeedCaptcha = iRequestCount >= 3;

                return PartialView("_Login", model);
                //return View(model);
            }

            _userCache.SaveLogin(model.UserName, true, model.SenderIP, model.SenderHeader);

            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.Action("Index", "MyEnterprise"); // RedirectToAction("Index", "MyEnterprise");

            return Json(new
            {
                status = true,
                returnUrl,
                message = CreateMessage("Đăng nhập thành công.",
                    EnumProcessType.NonFormat, EnumMsgIcon.Success)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            var typeMembershipProvider = ConfigurationManager.AppSettings["AppMembershipType"];
            var typeMembershipProviderNotRedirect = ConfigurationManager.AppSettings["MembershipProviderNotRedirect"];

            var isSuccess = Membership.DeleteUser(User.Identity.Name, true);
            if (!isSuccess) return RedirectToAction("Index", "Home");
            if (!string.IsNullOrEmpty(typeMembershipProviderNotRedirect) &&
                typeMembershipProviderNotRedirect.Split(',').Contains(typeMembershipProvider))
                return RedirectToAction("Login", "Account");
            var sSsoUrlService = ConfigurationManager.AppSettings["SSO_URLService"];
            var sSsoAppCode = ConfigurationManager.AppSettings["SSO_AppCode"];
            return Redirect($"{sSsoUrlService}?do=logout&appcode={sSsoAppCode}"); // redirects to external url
        }

        #endregion

        #region Change Password

        [HttpGet]
        [AllowAnyPermission]
        public ActionResult ChangePassword(string userName)
        {
            var currentUser = _userCache.GetByUserName(userName);
            if (currentUser == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage("Tài khoản", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                }, JsonRequestBehavior.AllowGet);
            return PartialView("_ChangePassword", new ChangePasswordModel
            {
                UserName = currentUser.UserName
            });
        }

        [HttpPost]
        [AllowAnyPermission]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid) return PartialView("_PasswordModel", model);

            if (!Regex.IsMatch(model.NewPassword, @"^(?=(.*\d){2})(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z\d]).{8,}$"))
            {
                ModelState.AddModelError("NewPassword",
                    "Mật khẩu phải dài ít nhất 8 ký tự và bao gồm ký tự thường, ký tự hoa, chữ số và ký tự đặc biệt");
                return PartialView("_PasswordModel", model);
            }

            var salt = UPasswordHash.GenerateSalt(model.NewPassword);
            var passwordHash = UPasswordHash.GenerateCryptoPassword(model.NewPassword, salt);

            var idUser = _userCache.ResetPassword(
                model.UserName,
                passwordHash,
                salt,
                "Đổi mật khẩu",
                User.UserName
            );
            switch (idUser)
            {
                case -1:
                    return Json(new
                    {
                        status = false,
                        message = CreateMessage("Tài khoản không tồn tại hoặc đã khoá.",
                            EnumProcessType.NonFormat, EnumMsgIcon.Error)
                    }, JsonRequestBehavior.AllowGet);
                default:
                    AppProcessor.Notifider.ForceLogout(model.UserName);
                    return Json(new
                    {
                        status = true,
                        message = CreateMessage("Mật khẩu",
                            EnumProcessType.Edit, EnumMsgIcon.Success)
                    }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Reset Password

        [HttpGet]
        [AllowAnyPermission]
        public ActionResult ResetPassword(string userName, string token)
        {
            var currentUser = _userCache.GetByUserName(userName);
            if (currentUser == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"Tài khoản {userName}", EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                }, JsonRequestBehavior.AllowGet);

            if (!currentUser.IsActive)
                return Json(new
                {
                    status = true,
                    message = CreateMessage(
                        $"Tài khoản <b>[{currentUser.FullName} - {currentUser.UserName}]</b> đã ngưng hoạt động.",
                        EnumProcessType.NonFormat, EnumMsgIcon.Error)
                });

            var resetPasswordModel = new ResetPasswordModel { UserName = currentUser.UserName };

            bool isCorrect;
            var decryptToken = EStringCipher.Decrypt(token, currentUser.Password, out isCorrect);
            if (isCorrect && !string.IsNullOrEmpty(decryptToken))
            {
                var arrTokens = decryptToken.Split('-');
                var hashPassword = arrTokens.Length > 0 ? arrTokens[0] : "";
                if (currentUser.Password == hashPassword)
                {
                    var dataTicks = long.Parse(arrTokens.Length > 0 ? arrTokens[1] : "0");
                    var timeExpier = new DateTime(dataTicks);
                    if (timeExpier >= DateTime.Now) return PartialView("ResetPassword", resetPasswordModel);
                    resetPasswordModel.ErrMessage =
                        AppProcessor.Messagor.GetMessage("ResetPassword_Token_Expired_Message");
                    resetPasswordModel.IsPermit = false;
                    return PartialView("ResetPassword", resetPasswordModel);
                }
            }

            resetPasswordModel.ErrMessage = AppProcessor.Messagor.GetMessage("ResetPassword_Token_Incorrect_Message");
            resetPasswordModel.IsPermit = false;

            return PartialView("ResetPassword", resetPasswordModel);
        }

        [HttpPost]
        [AllowAnyPermission]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid) return PartialView("_ResetPassword", model);

            if (!Regex.IsMatch(model.NewPassword, @"^(?=(.*\d){2})(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z\d]).{8,}$"))
            {
                ModelState.AddModelError("NewPassword",
                    "Mật khẩu phải dài ít nhất 8 ký tự và bao gồm ký tự thường, ký tự hoa, chữ số và ký tự đặc biệt");
                return PartialView("_ResetPassword", model);
            }

            var salt = UPasswordHash.GenerateSalt(model.NewPassword);
            var passwordHash = UPasswordHash.GenerateCryptoPassword(model.NewPassword, salt);

            var idUser = _userCache.ResetPassword(
                model.UserName,
                passwordHash,
                salt,
                "Đặt lại mật khẩu",
                model.UserName
            );
            switch (idUser)
            {
                case -1:
                    return Json(new
                    {
                        status = false,
                        message = CreateMessage("Tài khoản không tồn tại hoặc đã khoá.",
                            EnumProcessType.NonFormat, EnumMsgIcon.Error)
                    }, JsonRequestBehavior.AllowGet);
                default:
                    AppProcessor.Notifider.ForceLogout(model.UserName);
                    return Json(new
                    {
                        status = true,
                        message = CreateMessage("Mật khẩu",
                            EnumProcessType.Edit, EnumMsgIcon.Success)
                    }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Forgot Password

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            //return PartialView("_ForgotPassword", new SysUserModel());
            return PartialView("_ForgotPassword");
        }

        [HttpPost]
        [AllowAnyPermission]
        public ActionResult ForgotPassword(LoginModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return Json(new
                {
                    status = false,
                    message = CreateMessage("Bạn chưa nhập Email.", EnumProcessType.NonFormat, EnumMsgIcon.Error)
                }, JsonRequestBehavior.AllowGet);

            if (string.IsNullOrEmpty(model.UserName))
                return Json(new
                {
                    status = false,
                    message = CreateMessage("Bạn chưa nhập Tài khoản.", EnumProcessType.NonFormat, EnumMsgIcon.Error)
                }, JsonRequestBehavior.AllowGet);

            if (!EString.IsValidEmail(model.Email))
                return Json(new
                {
                    status = false,
                    message = CreateMessage("Email không đúng định dạng.", EnumProcessType.NonFormat, EnumMsgIcon.Error)
                }, JsonRequestBehavior.AllowGet);

            var userModel = _userCache.GetByUserName(model.UserName);
            if (userModel == null || userModel.Email != model.Email)
                return Json(new
                {
                    status = false,
                    message = CreateMessage("Thông tin tài khoản hoặc email không tồn tại", EnumProcessType.NonFormat,
                        EnumMsgIcon.Error)
                }, JsonRequestBehavior.AllowGet);

            userModel.HostUrl = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "");
            var baseToken = $"{userModel.Password}-{DateTime.Now.AddHours(24).Ticks}";
            var passPharse = userModel.Password;
            var tokenResetPassword = EStringCipher.Encrypt(baseToken, passPharse);
            userModel.DetailUrl = Url.Action("ResetPassword", "Account",
                new { area = "", userName = userModel.UserName, token = tokenResetPassword });

            var mailBodyHtml = RenderTemplateHtmlProvider.RenderStringHtml(
                HostingEnvironment.MapPath(
                    @"~/Contents/Modules/Sys/EmailTemplates/_TemplateResetPassword.cshtml"), userModel);

            AppProcessor.Mailer.PushEmail(new List<MailModel>
            {
                new MailModel
                {
                    DicImgs = new Dictionary<string, byte[]>
                    {
                        { "LogoVNPT", System.IO.File.ReadAllBytes($"{Server.MapPath(ConfigurationManager.AppSettings["Mail_VNPTLogoPath"])}")},
                        {
                            "LogoTourism",
                            System.IO.File.ReadAllBytes($"{Server.MapPath(ConfigurationManager.AppSettings["Mail_TourismLogoPath"])}")
                            //new WebClient().DownloadData(ConfigurationManager.AppSettings["Mail_TourismLogoPath"])
                        }
                    },
                    Subject =
                        $"[{AppProcessor.Messagor.GetMessage("App_Title")}] {AppProcessor.Messagor.GetMessage("MailSubject_ResetPassword_Message")}",
                    To = new List<string> {model.Email},
                    Body = mailBodyHtml,
                    IsBodyHtml = true,
                    DisplayNameFrom = AppProcessor.Messagor.GetMessage("App_Owner_DisplayName")
                }
            });

            return Json(new
            {
                status = true,
                message = CreateMessage($"Đã gửi yêu cầu đặt lại mật khẩu đến địa chỉ Email: <b>[{model.Email}]</b> ",
                    EnumProcessType.NonFormat, EnumMsgIcon.Success)
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Register

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            var enterpriseModel = new CateEnterpriseModel
            {
                ListProvinces = _provinceCache.GetAll()
                    .OrderBy(d => d.ProvinceName)
                    .Select(d => new ListItem(d.ProvinceName, d.ProvinceId.ToString())).ToList(),
                ListTypeBusiness = Enum.GetValues(typeof(EnumTypeBusiness))
                    .Cast<EnumTypeBusiness>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList(),
                Reason = "Đăng ký Doanh nghiệp"
            };
            return PartialView("_Register", enterpriseModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [CaptchaValidator(PrivateKey = "6LfqRGkjAAAAAJpe5A3tTukD3EkEoqVgb2aAz_SR", ErrorMessage = "Thao tác xác nhận sai !", RequiredMessage = "Bạn cần phải hoàn thành việc xác nhận.")]
        public ActionResult Register(CateEnterpriseModel model)
        {
            //var response = CreateMessage($"{_enterpriseTitle} [Tiên Đồng - Ngọc Nữ]", EnumProcessType.Add, EnumMsgIcon.Error);
            //return Json(new { status = false, message = response, email = "tiendongngocnu@gmail.com" });

            if (!ModelState.IsValid)
            {
                model.ListProvinces = _provinceCache.GetAll()
                    .OrderBy(d => d.ProvinceName)
                    .Select(d => new ListItem(d.ProvinceName, d.ProvinceId.ToString())).ToList();
                model.ListTypeBusiness = Enum.GetValues(typeof(EnumTypeBusiness))
                    .Cast<EnumTypeBusiness>()
                    .Select(x => new ListItem(AppProcessor.Messagor.GetMessage(EnumHelper.GetDescription(x)),
                        ((int)x).ToString()))
                    .ToList();

                return PartialView("_RegisterBox", model);
            }
            var sPassword = EString.GenerateStrongPassword(8);
            var salt = UPasswordHash.GenerateSalt(sPassword);
            var passwordHash = UPasswordHash.GenerateCryptoPassword(sPassword, salt);

            var enterpriseId = _enterpriseCache.Register(new CateEnterpriseModel
            {
                EnterpriseId = 0,
                OwnerEnterpriseName = model.OwnerEnterpriseName,
                BusinessName = model.BusinessName,
                TaxCode = model.TaxCode,
                BusinessAddress = model.BusinessAddress,
                StreetName = model.StreetName,
                WardId = model.WardId,
                WardName = model.WardName,
                TypeBusiness = model.TypeBusiness,
                TypeBusinessName = model.TypeBusinessName,
                LegalRepresentationName = model.LegalRepresentationName,
                LegalRepresentationPhone = model.LegalRepresentationPhone,
                LegalRepresentationEmail = model.LegalRepresentationEmail,
                Website = model.Website,
                Phone = model.Phone,
                Email = model.Email,
                Password = passwordHash,
                Salt = salt,
                Reason = model.Reason,
                SavedBy = "Anonymous"
            });

            if (enterpriseId == -9)
            {
                var errMessage =
                    CreateMessage($"{AppProcessor.Messagor.GetMessage("Enterprise_Label_TaxCode")} [{model.TaxCode}]",
                        EnumProcessType.DataExisted, EnumMsgIcon.Error);
                return Json(new { status = false, message = errMessage });
            }

            if (enterpriseId > 0)
            {
                _enterpriseRegisterNotifyCache.Save(new CateEnterpriseRegisterNotifyModel
                {
                    BusinessName = model.BusinessName,
                    CompletedBy = null,
                    EnterpriseID = enterpriseId,
                    HasProcessed = false,
                    OwnerEnterpriseName = model.OwnerEnterpriseName,
                    SaveBy = model.Email
                });

                //var sRoleIDs = ConfigurationManager.AppSettings["Enterprise_Role_Default"];
                //var sRoleIDs = string.Empty;
                //var sPassword = EString.GenerateStrongPassword(8);

                var mailNewUser = new SysMailUserModel
                {
                    FullName = model.OwnerEnterpriseName,
                    UserName = model.TaxCode,
                    Email = model.Email,
                    Password = sPassword,
                    HostUrl = Request.Url?.Host,
                    SupportEmail = _configsCache.GetViaKey("Email_Support")?.ConfigValue
                };

                //var salt = UPasswordHash.GenerateSalt(sPassword);
                //var passwordHash = UPasswordHash.GenerateCryptoPassword(sPassword, salt);

                //var idUser = _userCache.Save(new SysUserModel
                //{
                //    UserId = 0,
                //    FullName = model.OwnerEnterpriseName,
                //    UserName = model.TaxCode,
                //    Email = model.Email,
                //    Password = passwordHash,
                //    Salt = salt,
                //    RoleIDs = sRoleIDs,
                //    IsActive = false,
                //    Reason = model.Reason
                //}, "Anonymous");
                //if (idUser > 0)
                {
                    var dataHtml = RenderTemplateHtmlProvider.RenderStringHtml(
                        HostingEnvironment.MapPath(@"~/Contents/Modules/Sys/EmailTemplates/_TemplateNewUser.cshtml"),
                        mailNewUser);

                    AppProcessor.Mailer.PushEmail(new List<MailModel>
                    {
                        new MailModel
                        {
                            DicImgs = new Dictionary<string, byte[]>
                            {
                                { "LogoVNPT", System.IO.File.ReadAllBytes($"{Server.MapPath(ConfigurationManager.AppSettings["Mail_VNPTLogoPath"])}")},
                                {
                                    "LogoTourism",
                                    System.IO.File.ReadAllBytes($"{Server.MapPath(ConfigurationManager.AppSettings["Mail_TourismLogoPath"])}")
                                    //new WebClient().DownloadData(ConfigurationManager.AppSettings["Mail_TourismLogoPath"])
                                }
                            },
                            From = null,
                            DisplayNameFrom = null,
                            Subject =
                                $"[{AppProcessor.Messagor.GetMessage("App_Title")}] Thông tin tài khoản {model.BusinessName}",
                            To = new List<string> {model.Email},
                            IsBodyHtml = true,
                            Body = dataHtml
                        }
                    });
                }

                _enterpriseCache.SaveEnterprisePermissions(new CateEnterprisePermissionsModel
                {
                    EnterpriseIds = $"{enterpriseId}",
                    ForUser = model.TaxCode
                });

                model.EnterpriseId = enterpriseId;
                var businessCetificatesId = SaveUploadFile(model);
                if (businessCetificatesId == -7)
                    return Json(new
                    {
                        status = false,
                        message = CreateMessage($"{_enterpriseTitle}",
                            EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                    });
            }

            var response = CreateMessage($"{_enterpriseTitle} [{model.OwnerEnterpriseName} - {model.BusinessName}]",
                EnumProcessType.Add, enterpriseId > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response, email = model.Email });
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult RegisterSuccess(string email)
        {
            return PartialView("_RegisterSuccess", email);
        }

        #endregion

        #region Search

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Search()
        {
            var searchModel = new SearchEnterpriseModel();

            return PartialView("_Search", searchModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Search(SearchEnterpriseModel model)
        {
            var dataEnterprises = _enterpriseCache.Search(model.TaxCode, model.Email, model.EnterpriseName);
            return PartialView("_SearchResult", dataEnterprises);
        }

        #endregion

        #region Smart CA

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

        [HttpGet]
        [AllowAnonymous]
        public ActionResult SmartCA()
        {
            var model = new DigitalSignModel
            {
                AccessToken = (string)Session[$"VNPT-SmartCA-{HttpContext.Request.AnonymousID}-AccessToken"]
            };

            return PartialView("_SmartCA", model);
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpGet]
        public ActionResult LoginSmartCA()
        {
            return PartialView("_LoginCA", new LoginSmartCAModel());
        }

        [AllowAnonymous]
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
                accessToken,
                message = CreateMessage($"{AppProcessor.Messagor.GetMessage("Login_SmartCA_Label")} thành công",
                    EnumProcessType.NonFormat, EnumMsgIcon.Success)
            });
        }

        [HttpGet]
        [AjaxOnly]
        [AllowAnonymous]
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
            var userInfo = (UserInfoModel)Session[$"VNPT-SmartCA-{User?.UserName}-UserInfo"];

            return PartialView("_UserInfo", userInfo ?? new UserInfoModel());
        }

        [AllowAnonymous]
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

            var accessToken = (string)Session[$"VNPT-SmartCA-{User?.UserName}-AccessToken"];
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
            var isSuccess = 0;
            try
            {
                //isSuccess = SignHash(accessToken, model.FileImports, fileSignedFullPath);
                isSuccess = Sign(accessToken, model.FileImports, fileSignedFullPath);
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
        private int Sign(string accessToken, HttpPostedFileBase fileImport, string fullSignedPath)
        {
            var lstCredentials = VNPTSmartCAProvider.GetListCredentials(accessToken);
            var credentialId = lstCredentials?[0];
            var fileData = StreamHelper.ReadFully(fileImport.InputStream);

            var tranId = VNPTSmartCAProvider.Sign(accessToken, credentialId, "",
                fileImport.FileName, fileData);
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
                        datasigned = tranInfo.Documents[0].DataSigned;
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

            // ------------------------------------------------------------------------------------------
            // 3. Package external signature to signed file

            System.IO.File.WriteAllBytes(fullSignedPath, Convert.FromBase64String(datasigned));

            return 0;
        }

        #endregion

        #region Extend Function

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult WardViaProvince(int provinceId = 0)
        {
                int totalWards;
            var lstWardViaProvinces =
                _wardCache.GetByProvinceId(provinceId, out totalWards).OrderBy(d => d.WardName).ToList();
            return Json(new { Wards = lstWardViaProvinces });
        }

        private int SaveUploadFile(CateEnterpriseModel model)
        {
            if (model.EnterpriseId <= 0) return -7;
            if (model.ListCertificateFiles == null || model.ListCertificateFiles.Count == 0) return 0;

            var lstDocs = new List<CateDocModel>();

            var moduleRefDocsPathFolder = string.Concat(_moduleRefDocsPathFolder, _enterpriseFolder, "/",
                model.EnterpriseId.ToString());
            var moduleRefDocsAbsolutePathFolder = Server.MapPath(moduleRefDocsPathFolder);

            if (!Directory.Exists(moduleRefDocsAbsolutePathFolder))
                Directory.CreateDirectory(moduleRefDocsAbsolutePathFolder);
            model.ListCertificateFiles.ForEach(f =>
            {
                if (f == null) return;
                var unionResolutionDoc = new CateDocModel
                {
                    FileId = Guid.NewGuid(),
                    FilePath = moduleRefDocsPathFolder,
                    FileName = Path.GetFileNameWithoutExtension(f.FileName),
                    FileExt = Path.GetExtension(f.FileName),
                    ContentType = f.ContentType
                };

                f.SaveAs(string.Concat(moduleRefDocsAbsolutePathFolder, "/",
                    unionResolutionDoc.FileId.ToString().ToUpper(), unionResolutionDoc.FileExt));
                lstDocs.Add(unionResolutionDoc);
            });
            if (lstDocs.Count <= 0) return 0;
            var regulationId = _enterpriseCache.SaveDocs(new CateEnterpriseModel
            {
                EnterpriseId = model.EnterpriseId,
                CertificateFiles = CreateDataRefDocs(lstDocs),
                Reason = model.Reason,
                SavedBy = User.Email
            });

            return regulationId;
        }

        private DataTable CreateDataRefDocs(List<CateDocModel> lstDocs)
        {
            var dataRefDocs = new DataTable();
            using (var reader =
                ObjectReader.Create(lstDocs, "FileId", "FilePath", "FileName", "FileExt", "ContentType"))
            {
                dataRefDocs.Load(reader);
            }

            return dataRefDocs;
        }

        #endregion
    }
}