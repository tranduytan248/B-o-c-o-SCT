using System;
using System.Collections.Generic;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Sys
{
    public class SysUserModel
    {
        public int? UserId { get; set; }


        [CustomDisplayName("User_Label_OfficeName")]
        public string OfficeName { get; set; }

        [CustomDisplayName("User_Label_FullName")]
        public string FullName { get; set; }

        [CustomRequired]
        [CustomDisplayName("User_Label_UserName")]
        public string UserName { get; set; }

        [CustomDisplayName("User_Label_Email")]
        //[EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [CustomRequired]
        public string Email { get; set; } = null;

        public string DetailUrl { get; set; }
        public string Password { get; set; } = null;
        public string Salt { get; set; }
        public bool IsActive { get; set; }
        public bool IsOnline { get; set; }
        public string RoleIDs { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string HostUrl { get; set; }


        [CustomRequired]
        [CustomDisplayName("Reason_Title")]
        public string Reason { get; set; }

        public List<SysRoleModel> Roles { get; set; }

        public List<SysUserPermissionModel> Permissions { get; set; }

        public int? TotalRow { get; set; } = 0;
    }
}