using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Modules.CateModule.Models
{
    public class MyEnterpiseSearchModel
    {
        [CustomDisplayName("Enterprise_Title")]
        public int? EnterpriseId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        public List<ListItem> ListEnterprises { get; set; } = new List<ListItem>();
    }
}