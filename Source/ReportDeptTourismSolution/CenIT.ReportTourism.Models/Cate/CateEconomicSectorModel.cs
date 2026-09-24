using System;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate
{
    public class CateEconomicSectorModel : BaseModel
    {
        public int EconomicSectorId { get; set; }

        [CustomDisplayName("Code_Title")]
        [CustomRequired]
        public string Code { get; set; }

        [CustomDisplayName("Name_Title")]
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