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
        public string PermissionUserIDs { get; set; }

        [CustomDisplayName("AppModule_Label_PermissionUsers")]
        public List<ListItem> Users { get; set; }
    }
}