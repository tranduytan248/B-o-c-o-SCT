using System.Web;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Models
{
    public class DigitalSignModel
    {
        [CustomDisplayName("DataImport_Label_FileImport")]
        public HttpPostedFileBase FileImports { get; set; }

        public string AccessToken { get; set; }
        public string FileGuids { get; set; }
    }
}