using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Models
{
    public class PublicCateSearchModel
    {
        public PublicCateSearchModel()
        {
            ListCateTypes = new List<ListItem>();
        }

        [CustomDisplayName("PublicCate_Label_CateType")]
        public int? CateTypeId { get; set; }

        [CustomDisplayName("PublicCate_Label_CateType")]
        public List<int> ListCateTypeId { get; set; }

        [CustomDisplayName("PublicCate_Label_CateType")]
        public List<ListItem> ListCateTypes { get; set; }
    }
}