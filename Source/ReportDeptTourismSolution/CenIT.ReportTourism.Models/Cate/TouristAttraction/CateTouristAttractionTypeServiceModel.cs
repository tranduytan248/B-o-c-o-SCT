using System;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.TouristAttraction
{
    public class CateTouristAttractionTypeServiceModel
    {
        public int TouristAttractionTypeServiceId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("TouristAttractionTypeService_Label_Name")]
        [CustomRequired]
        public string ServiceName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? TotalRow { get; set; } = 0;

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public string SavedBy { get; set; }
    }
}