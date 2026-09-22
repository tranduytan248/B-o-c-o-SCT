namespace CenIT.ReportTourism.Modules.ReportModule.Areas.Report.Models
{
    public class DataFileSignTokenModel
    {
        public int? EnterpriseId { get; set; }
        public int? TypeSignature { get; set; } = 0;
        public string FileName { get; set; }
        public string FileExt { get; set; }
        public string FileDataBase64 { get; set; }
    }
}