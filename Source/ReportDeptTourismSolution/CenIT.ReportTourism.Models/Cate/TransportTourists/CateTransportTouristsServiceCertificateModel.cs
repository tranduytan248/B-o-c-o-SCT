using System;
using System.Data;
using System.Web;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.TransportTourists
{
    public class CateTransportTouristsServiceCertificateModel
    {
        public int CertificateId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("TransportTouristsCertificate_Label_LicenseFile")]
        [CustomRequired]
        public Guid LicenseFile { get; set; }

        public string LicenseFileView { get; set; }
        public DataTable DataLicenseFile { get; set; }

        [CustomDisplayName("TransportTouristsCertificate_Label_LicenseFile")]
        public HttpPostedFileBase FileLicenseFile { get; set; }

        [CustomDisplayName("TransportTouristsCertificate_Label_LicenseNumber")]
        [CustomRequired]
        public string LicenseNumber { get; set; }

        [CustomDisplayName("TransportTouristsCertificate_Label_ReleaseDate")]
        [CustomRequired]
        public DateTime ReleaseDate { get; set; } = DateTime.Now;

        [CustomDisplayName("TransportTouristsCertificate_Label_TimeRelease")]
        public int TimeRelease { get; set; } = 1;

        [CustomDisplayName("TransportTouristsCertificate_Label_WaterWay")]
        public bool WaterWay { get; set; }

        [CustomDisplayName("TransportTouristsCertificate_Label_Road")]
        public bool Road { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public string SavedBy { get; set; }
    }
}