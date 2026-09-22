using System;
using System.Data;
using System.Web;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.Accommodation
{
    public class CateAccommodationServiceCertificateModel
    {
        public int CertificateId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("AccommodationCertificate_Label_Security")]
        public Guid SecurityCertificate { get; set; }

        public string SecurityCertificateView { get; set; }
        public DataTable DataSecurityCertificate { get; set; }
        public HttpPostedFileBase FileSecurityCertificate { get; set; }

        [CustomDisplayName("AccommodationCertificate_Label_FireProtection")]
        public Guid FireProtectionCertificate { get; set; }

        public string FireProtectionCertificateView { get; set; }
        public DataTable DataFireProtectionCertificate { get; set; }
        public HttpPostedFileBase FileFireProtectionCertificate { get; set; }

        [CustomDisplayName("AccommodationCertificate_Label_EnvProtection")]
        public Guid EnvironmentalProtectionCertificate { get; set; }

        public string EnvironmentalProtectionCertificateView { get; set; }
        public DataTable DataEnvironmentalProtectionCertificate { get; set; }
        public HttpPostedFileBase FileEnvironmentalProtectionCertificate { get; set; }

        [CustomDisplayName("AccommodationCertificate_Label_HygieneFoodSafety")]
        public Guid HygieneFoodSafetyCertificate { get; set; }

        public string HygieneFoodSafetyCertificateView { get; set; }
        public DataTable DataHygieneFoodSafetyCertificate { get; set; }
        public HttpPostedFileBase FileHygieneFoodSafetyCertificate { get; set; }

        [CustomDisplayName("AccommodationCertificate_Label_ConstructionPermit")]
        public Guid ConstructionPermit { get; set; }

        public string ConstructionPermitView { get; set; }
        public DataTable DataConstructionPermit { get; set; }
        public HttpPostedFileBase FileConstructionPermit { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public string SavedBy { get; set; }
    }
}