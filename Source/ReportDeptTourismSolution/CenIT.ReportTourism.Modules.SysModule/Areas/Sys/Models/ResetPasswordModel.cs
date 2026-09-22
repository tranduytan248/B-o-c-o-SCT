using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Models
{
    public class ResetPasswordModel
    {
        [CustomDisplayName("User_Label_FullName")]
        public string FullName { get; set; }

        [CustomDisplayName("User_Label_Email")]
        public string Email { get; set; }

        [CustomDisplayName("User_Label_UserName")]
        public string UserName { get; set; }

        [CustomRequired]
        [CustomDisplayName("Authorize_Current_Password")]
        public string CurrentPassword { get; set; } = null;

        [CustomRequired]
        [CustomDisplayName("Authorize_New_Password")]
        public string NewPassword { get; set; } = null;

        [CustomRequired]
        [CustomDisplayName("Authorize_Confirm_Password")]
        [CustomCompare("NewPassword", ErrorMessage = "Common_MessageCompareNotMatch")]
        public string ConfirmPassword { get; set; }

        public string Salt { get; set; }

        [CustomRequired]
        [CustomDisplayName("Reason_Title")]
        public string Reason { get; set; }
    }
}