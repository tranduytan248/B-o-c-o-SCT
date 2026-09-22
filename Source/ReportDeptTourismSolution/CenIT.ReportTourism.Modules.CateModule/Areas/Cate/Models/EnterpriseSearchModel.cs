using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Models
{
    public class EnterpriseSearchModel
    {
        public EnterpriseSearchModel()
        {
            ListTypeBusiness = ListWards = new List<ListItem>();
        }

        [CustomDisplayName("Enterprise_Label_TypeBusiness")]
        public int? TypeBusiness { get; set; }

        public string TypeBusinessIds { get; set; }

        [CustomDisplayName("Enterprise_Label_TypeBusiness")]
        public List<int> ListTypeBusinessId { get; set; }

        [CustomDisplayName("Enterprise_Label_TypeBusiness")]
        public List<ListItem> ListTypeBusiness { get; set; }

        public string WardIds { get; set; }

        [CustomDisplayName("Enterprise_Label_Ward")]
        public List<int> ListWardId { get; set; }

        [CustomDisplayName("Enterprise_Label_Ward")]
        public List<ListItem> ListWards { get; set; }
    }
}