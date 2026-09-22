using System;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.TransportTourists
{
    public class CateTransportTouristsServiceHRInformationModel
    {
        public int HRInfoId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("TransportTouristsHRInformation_Label_TotalStaff")]
        public int? TotalStaff { get; set; }

        [CustomDisplayName("TransportTouristsHRInformation_Label_StaffOnLiteracyOnCollege")]
        public int? StaffOnLiteracyOnCollege { get; set; }

        [CustomDisplayName("TransportTouristsHRInformation_Label_StaffOnUniversity")]
        public int? StaffOnUniversity { get; set; }

        [CustomDisplayName("TransportTouristsHRInformation_Label_StaffOnCollege")]
        public int? StaffOnCollege { get; set; }

        [CustomDisplayName("TransportTouristsHRInformation_Label_StaffOnIntermediate")]
        public int? StaffOnIntermediate { get; set; }

        [CustomDisplayName("TransportTouristsHRInformation_Label_StaffOnHighSchool")]
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