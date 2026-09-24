using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate
{
    public class CateBusinessProductModel : BaseModel
    {
        public int ProductId { get; set; }

        [CustomDisplayName("Code_Title")]
        [CustomRequired]
        public string ProductCode { get; set; }

        [CustomDisplayName("Name_Title")]
        [CustomRequired]
        public string ProductName { get; set; }

        [CustomDisplayName("BusinessIndustry_Title")]
        [CustomRequired]
        public int? IndustryId { get; set; }

        public string IndustryName { get; set; }

        public List<ListItem> ListIndustries { get; set; } = new List<ListItem>();

        [CustomDisplayName("Product_Parent")]
        public int? ParentId { get; set; }

        [CustomDisplayName("Unit_Title")]
        public string Unit { get; set; }

        [CustomDisplayName("DisplayOrder_Title")]
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }
    }
}