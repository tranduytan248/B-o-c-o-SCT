using System;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate
{
    public class CateEnterpriseTypeModel : BaseModel
    {
        public int EnterpriseTypeId { get; set; }

        [CustomDisplayName("EnterpriseType_Code")]
        [CustomRequired]
        public string Code { get; set; }

        [CustomDisplayName("EnterpriseType_Name")]
        [CustomRequired]
        public string Name { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }
    }
}