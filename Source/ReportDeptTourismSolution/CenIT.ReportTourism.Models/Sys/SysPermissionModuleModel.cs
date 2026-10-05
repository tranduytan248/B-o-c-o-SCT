using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Sys
{
    public class SysPermissionModuleModel
    {
        [CustomRequired] public int ModuleId { get; set; }

        [CustomDisplayName("AppModule_Label_ModuleName")]
        public string ModuleName { get; set; }

        [CustomDisplayName("AppModule_Label_Description")]
        public string Description { get; set; }

        [CustomDisplayName("AppModule_Label_PermissionUsers")]
        [CustomRequired]
        public List<int> ListUserIds { get; set; }

        [CustomDisplayName("AppModule_Label_PermissionUsers")]
        public string PermissionUserIds { get; set; }

        public string SelectedUser { get; set; }


        [CustomDisplayName("AppModule_Label_PermissionUsers")]
        public List<ListItem> Users { get; set; }
    }
}