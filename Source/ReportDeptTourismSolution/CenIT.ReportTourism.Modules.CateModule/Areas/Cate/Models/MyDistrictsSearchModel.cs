using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Models
{
    public class MyDistrictsSearchModel
    {
        public MyDistrictsSearchModel()
        {
            ListProvinces = new List<ListItem>();
        }

        [CustomDisplayName("Province_Title")] public List<int> ListProvinceId { get; set; }

        public string ProvincesIds { get; set; }

        [CustomDisplayName("Province_Title")] public List<ListItem> ListProvinces { get; set; }
    }
}