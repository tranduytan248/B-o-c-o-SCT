using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.WebApp.Models
{
    public class ResetPasswordModel
    {
        [CustomDisplayName("User_Label_UserName")]
        public string UserName { get; set; }

        // Mật khẩu chỉ dùng để băm, không hiển thị lại => cho phép ký tự như '<' mà không bị request validation chặn
        [CustomRequired]
        [CustomDisplayName("Authorize_New_Password")]
        [System.Web.Mvc.AllowHtml]
        public string NewPassword { get; set; } = null;

        [CustomRequired]
        [CustomDisplayName("Authorize_Confirm_Password")]
        [CustomCompare("NewPassword", ErrorMessage = "Common_MessageCompareNotMatch")]
        [System.Web.Mvc.AllowHtml]
        public string ConfirmPassword { get; set; }

        public string Salt { get; set; }

        public string ErrMessage { get; set; }

        public bool IsPermit { get; set; } = true;
    }
}