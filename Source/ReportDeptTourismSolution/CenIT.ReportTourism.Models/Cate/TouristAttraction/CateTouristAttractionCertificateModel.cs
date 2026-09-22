using System;
using System.Data;
using System.Web;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.TouristAttraction
{
    public class CateTouristAttractionCertificateModel
    {
        public int CertificateId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("TouristAttractionCertificate_Label_LicenseFile")]
        [CustomRequired]
        public Guid LicenseFile { get; set; }

        [CustomDisplayName("TouristAttractionCertificate_Label_LicenseFile")]
        public string LicenseFileView { get; set; }

        [CustomDisplayName("TouristAttractionCertificate_Label_LicenseFile")]
        public DataTable DataLicenseFile { get; set; }

        [CustomDisplayName("TouristAttractionCertificate_Label_LicenseFile")]
        public HttpPostedFileBase FileLicenseFile { get; set; }

        [CustomDisplayName("TouristAttractionCertificate_Label_LicenseNumber")]
        [CustomRequired]
        public string LicenseNumber { get; set; }

        [CustomDisplayName("TouristAttractionCertificate_Label_ReleaseDate")]
        [CustomRequired]
        public DateTime ReleaseDate { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public string SavedBy { get; set; }
    }
}