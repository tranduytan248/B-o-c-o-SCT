using System;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.Accommodation
{
    public class CateAccommodationServiceHRInformationModel
    {
        public int HRInfoId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("TravelingHRInformation_Label_TotalStaff")]
        public int? TotalStaff { get; set; }

        [CustomDisplayName("AccommodationHRInformation_Label_StaffOnLeader")]
        public int? StaffOnLeader { get; set; }

        [CustomDisplayName("AccommodationHRInformation_Label_StaffOnManager")]
        public int? StaffOnManager { get; set; }

        [CustomDisplayName("AccommodationHRInformation_Label_StaffOnReceptionist")]
        public int? StaffOnReceptionist { get; set; }

        [CustomDisplayName("AccommodationHRInformation_Label_StaffOnRestaurant")]
        public int? StaffOnRestaurant { get; set; }

        [CustomDisplayName("AccommodationHRInformation_Label_StaffOnBar")]
        public int? StaffOnBar { get; set; }

        [CustomDisplayName("AccommodationHRInformation_Label_StaffOnKitchen")]
        public int? StaffOnKitchen { get; set; }

        [CustomDisplayName("AccommodationHRInformation_Label_StaffOnRoom")]
        public int? StaffOnRoom { get; set; }

        [CustomDisplayName("AccommodationHRInformation_Label_StaffOnLiteracyOnCollege")]
        public int? StaffOnLiteracyOnCollege { get; set; }

        [CustomDisplayName("AccommodationHRInformation_Label_StaffOnUniversity")]
        public int? StaffOnUniversity { get; set; }

        [CustomDisplayName("AccommodationHRInformation_Label_StaffOnCollege")]
        public int? StaffOnCollege { get; set; }

        [CustomDisplayName("AccommodationHRInformation_Label_StaffOnIntermediate")]
        public int? StaffOnIntermediate { get; set; }

        [CustomDisplayName("AccommodationHRInformation_Label_StaffOnHighSchool")]
        public int? StaffOnHighSchool { get; set; }

        [CustomDisplayName("AccommodationHRInformation_Label_HaveForeignLanguageCertificate")]
        public int? HaveForeignLanguageCertificate { get; set; }

        [CustomDisplayName("AccommodationHRInformation_Label_HaveProfessionalCertificate")]
        public int? HaveProfessionalCertificate { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public string SavedBy { get; set; }
    }
}