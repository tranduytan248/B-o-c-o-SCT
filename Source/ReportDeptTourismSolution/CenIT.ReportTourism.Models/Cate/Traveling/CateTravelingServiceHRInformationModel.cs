using System;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.Traveling
{
    public class CateTravelingServiceHRInformationModel
    {
        public int HRInfoId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("TravelingHRInformation_Label_TotalStaff")]
        public int? TotalStaff { get; set; }

        [CustomDisplayName("TravelingHRInformation_Label_StaffOnLeader")]
        public int? StaffOnLeader { get; set; }

        [CustomDisplayName("TravelingHRInformation_Label_StaffOnManager")]
        public int? StaffOnManager { get; set; }

        [CustomDisplayName("TravelingHRInformation_Label_TourGuide")]
        public int? TourGuide { get; set; }

        [CustomDisplayName("TravelingHRInformation_Label_StaffOnLiteracyOnCollege")]
        public int? StaffOnLiteracyOnCollege { get; set; }

        [CustomDisplayName("TravelingHRInformation_Label_StaffOnUniversity")]
        public int? StaffOnUniversity { get; set; }

        [CustomDisplayName("TravelingHRInformation_Label_StaffOnCollege")]
        public int? StaffOnCollege { get; set; }

        [CustomDisplayName("TravelingHRInformation_Label_StaffOnIntermediate")]
        public int? StaffOnIntermediate { get; set; }

        [CustomDisplayName("TravelingHRInformation_Label_StaffOnHighSchool")]
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