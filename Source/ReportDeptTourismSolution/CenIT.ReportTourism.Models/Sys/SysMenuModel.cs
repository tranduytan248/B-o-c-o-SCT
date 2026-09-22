using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Sys
{
    public class SysMenuModel
    {
        public string ModuleName { get; set; }

        public int MenuId { get; set; }

        [CustomRequired]
        [CustomDisplayName("Menu_Label_Name")]
        public string Name { get; set; }

        public int? Position { get; set; } = 1;
        public int? LevelMenu { get; set; } = 1;
        public string Depth { get; set; }
        public int? ParentId { get; set; }

        [CustomRequired]
        [CustomDisplayName("Menu_Label_Link")]
        public string Link { get; set; }

        public string Icon { get; set; }
        public int? FunctionActionId { get; set; }
        public bool IsShow { get; set; } = true;
        public string FunctionName { get; set; }
        public int? TotalRow { get; set; } = 0;

        public List<ListItem> FunctionActions { get; set; }

        public List<ListItem> ParentMenus { get; set; }
    }
}