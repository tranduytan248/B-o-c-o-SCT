namespace CenIT.ReportTourism.Models.Sys
{
    public class SysContentPanelModel
    {
        public int ContentPanelId { get; set; }
        public string ContentPanelName { get; set; }
        public string Note { get; set; }
        public int LayoutId { get; set; }
        public bool Deleted { get; set; }
        public int WidthCol { get; set; } = 1;
        public int? TotalRow { get; set; } = 0;
    }
}