using System.Collections.Generic;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Sys
{
    public class SysRoleModel
    {
        public int RoleId { get; set; }

        [CustomRequired]
        [CustomDisplayName("Role_Label_Name")]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }
        public List<SysFunctionModel> Functions { get; set; }
        public string Permissions { get; set; }
        public int? TotalRow { get; set; } = 0;
    }
}