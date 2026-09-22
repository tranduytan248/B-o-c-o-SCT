using System.ComponentModel.DataAnnotations;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Modules.SysModule.Areas.Sys.Models
{
    public class LoginSmartCAModel
    {
        [CustomDisplayName("Authorize_UserName")]
        [CustomRequired]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [CustomRequired]
        [CustomDisplayName("Authorize_Password")]
        public string Password { get; set; }
    }
}