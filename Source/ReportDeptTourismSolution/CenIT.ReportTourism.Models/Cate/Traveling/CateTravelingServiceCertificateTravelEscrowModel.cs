using System;
using System.Data;
using System.Web;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.Traveling
{
    public class CateTravelingServiceCertificateTravelEscrowModel
    {
        public int CertificateTravelEscrowId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("TravelingCertificateTravelEscrow_Label_CertificateFile")]
        [CustomRequired]
        public Guid CertificateId { get; set; }

        public string CertificateView { get; set; }
        public DataTable DataCertificate { get; set; }

        [CustomDisplayName("TravelingCertificateTravelEscrow_Label_CertificateFile")]
        public HttpPostedFileBase FileCertificate { get; set; }

        [CustomDisplayName("TravelingCertificateTravelEscrow_Label_BankName")]
        [CustomRequired]
        public string BankName { get; set; }

        [CustomDisplayName("TravelingCertificateTravelEscrow_Label_BankAccountNo")]
        [CustomRequired]
        public string BankAccountNo { get; set; }

        [CustomDisplayName("TravelingCertificateTravelEscrow_Label_Amount")]
        public double Amount { get; set; }

        [CustomDisplayName("TravelingCertificateTravelEscrow_Label_Amount")]
        public string AnnouncedView { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public string SavedBy { get; set; }
    }
}