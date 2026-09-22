using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate
{
    public class CatePublicCateModel
    {
        public int CateId { get; set; }

        [CustomDisplayName("PublicCate_Label_Name")]
        [CustomRequired]
        public string CateName { get; set; }

        [CustomDisplayName("PublicCate_Label_CateType")]
        [CustomRequired]
        public int CateTypeId { get; set; }

        [CustomDisplayName("PublicCate_Label_CateType")]
        public string CateTypeName { get; set; }

        public bool IsDeleted { get; set; }

        [CustomDisplayName("PublicCate_Label_CateType")]
        public List<ListItem> ListCateTypes { get; set; }

        public int? TotalRow { get; set; } = 0;

        public string SavedBy { get; set; }
    }
}