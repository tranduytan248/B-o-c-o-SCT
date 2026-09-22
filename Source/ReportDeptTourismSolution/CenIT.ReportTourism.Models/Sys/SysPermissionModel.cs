namespace CenIT.ReportTourism.Models.Sys
{
    public class SysPermissionModel
    {
        public int PermissionId { get; set; }
        public int RoleId { get; set; }
        public int FunctionId { get; set; }
        public string Action { get; set; }
        public int? TotalRow { get; set; } = 0;
    }
}