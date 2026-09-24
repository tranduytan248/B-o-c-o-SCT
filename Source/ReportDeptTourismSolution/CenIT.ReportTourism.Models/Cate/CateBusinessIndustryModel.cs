using System;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate
{
    public class CateBusinessIndustryModel : BaseModel
    {
        public int IndustryId { get; set; }

        [CustomRequired]
        [CustomDisplayName("Industry_Code")]
        public string IndustryCode { get; set; }

        [CustomDisplayName("Industry_Name")]
        public string IndustryName { get; set; }

        [CustomDisplayName("Industry_Parent")]
        public int? ParentId { get; set; }

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