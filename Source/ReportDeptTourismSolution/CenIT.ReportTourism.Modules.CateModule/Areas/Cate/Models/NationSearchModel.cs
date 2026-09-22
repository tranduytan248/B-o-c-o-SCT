using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Models
{
    public class NationSearchModel
    {
        public NationSearchModel()
        {
            ListContinent = new List<ListItem>();
        }

        [CustomDisplayName("National_Label_ContinentName")]
        public int? Continent { get; set; }

        [CustomDisplayName("National_Label_ContinentName")]
        public List<ListItem> ListContinent { get; set; }
    }
}