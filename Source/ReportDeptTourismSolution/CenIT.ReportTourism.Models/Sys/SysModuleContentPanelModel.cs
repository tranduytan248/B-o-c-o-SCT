namespace CenIT.ReportTourism.Models.Sys
{
    public class SysModuleContentPanelModel
    {
        public int ContentPanelId { get; set; }
        public int ModuleId { get; set; }
        public int OrderBy { get; set; } = 0;
        public string ContentPanelName { get; set; }
        public string ModuleName { get; set; }
        public string MainController { get; set; }
        public string AssemblyName { get; set; }
        public string ModuleView { get; set; }
        public int? TotalRow { get; set; } = 0;
    }
}