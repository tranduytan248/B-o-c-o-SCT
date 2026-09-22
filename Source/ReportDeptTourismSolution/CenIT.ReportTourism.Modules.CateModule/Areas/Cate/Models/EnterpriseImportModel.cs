using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Models
{
    public class EnterpriseImportModel
    {
        public EnterpriseImportModel()
        {
            ListTypeBusiness = new List<ListItem>();
        }

        [CustomDisplayName("Enterprise_Label_TypeBusiness")]
        public int? TypeBusiness { get; set; }

        [CustomDisplayName("Enterprise_Label_TypeBusiness")]
        public string TypeBusinessName { get; set; }

        [CustomDisplayName("Enterprise_Label_DataImport")]
        [CustomRequired]
        public HttpPostedFileBase FileImportData { get; set; }

        [CustomDisplayName("Enterprise_Label_TypeBusiness")]
        public List<ListItem> ListTypeBusiness { get; set; }
    }
}