using System;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.TouristAttraction
{
    public class CateTouristAttractionHRInformationModel
    {
        public int HRInfoId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("TouristAttractionHRInformation_Label_TotalStaff")]
        public int? TotalStaff { get; set; }

        [CustomDisplayName("TouristAttractionHRInformation_Label_StaffOnLiteracyOnCollege")]
        public int? StaffOnLiteracyOnCollege { get; set; }

        [CustomDisplayName("TouristAttractionHRInformation_Label_StaffOnUniversity")]
        public int? StaffOnUniversity { get; set; }

        [CustomDisplayName("TouristAttractionHRInformation_Label_StaffOnCollege")]
        public int? StaffOnCollege { get; set; }

        [CustomDisplayName("TouristAttractionHRInformation_Label_StaffOnIntermediate")]
        public int? StaffOnIntermediate { get; set; }

        [CustomDisplayName("TouristAttractionHRInformation_Label_StaffOnHighSchool")]
        public int? StaffOnHighSchool { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public string SavedBy { get; set; }
    }
}