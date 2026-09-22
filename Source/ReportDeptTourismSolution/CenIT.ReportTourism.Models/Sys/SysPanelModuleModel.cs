using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Sys
{
    public class SysPanelModuleModel
    {
        [CustomRequired]
        [CustomDisplayName("PanelModule_Label_PanelName")]
        public string PanelName { get; set; }

        public int ContentPanelId { get; set; }

        [CustomRequired]
        [CustomDisplayName("PanelModule_Label_Module")]
        public int ModuleId { get; set; }

        public int OrderBy { get; set; } = 0;
        public string ModuleName { get; set; }
        public string AssemblyName { get; set; }
        public string MainController { get; set; }
        public string ModuleView { get; set; }
        public List<ListItem> ActiveModules { get; set; }
    }
}