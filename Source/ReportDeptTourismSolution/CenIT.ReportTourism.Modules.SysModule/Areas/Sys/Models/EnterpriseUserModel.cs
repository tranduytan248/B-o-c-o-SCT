using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Models
{
    public class EnterpriseUserModel
    {
        [CustomDisplayName("User_Label_FullName")]
        public string FullName { get; set; }

        [CustomDisplayName("User_Label_Email")]
        public string Email { get; set; }

        [CustomDisplayName("User_Label_UserName")]
        public string UserName { get; set; }

        [CustomDisplayName("Enterprises_Label_Name")]
        public string StrEnterprisesSelected { get; set; }

        public List<ListItem> Enterprises { get; set; }
    }
}