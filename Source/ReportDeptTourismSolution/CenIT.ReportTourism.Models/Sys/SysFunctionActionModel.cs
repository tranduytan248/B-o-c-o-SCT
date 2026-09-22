namespace CenIT.ReportTourism.Models.Sys
{
    public class SysFunctionActionModel
    {
        public string Area { get; set; }
        public int FunctionActionId { get; set; }
        public int FunctionId { get; set; }
        public string Function { get; set; }
        public string Action { get; set; }
        public int? TotalRow { get; set; } = 0;
    }
}