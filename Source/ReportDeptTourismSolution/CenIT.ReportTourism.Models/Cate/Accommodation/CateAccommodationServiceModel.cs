using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.Accommodation
{
    public class CateAccommodationServiceModel
    {
        public int AccommodationServiceId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("AccommodationTypeService_Label_Class")]
        public int? AccommodationClass { get; set; }

        [CustomDisplayName("AccommodationTypeService_Label_ClassCertificate")]
        public Guid AccommodationClassCertificate { get; set; }

        [CustomDisplayName("AccommodationTypeService_Label_ClassCertificate")]
        public DataTable DataAccommodationClassCertificate { get; set; }

        [CustomDisplayName("AccommodationTypeService_Label_ClassCertificate")]
        public string AccommodationClassCertificateView { get; set; }

        [CustomDisplayName("AccommodationTypeService_Label_ClassCertificate")]
        public HttpPostedFileBase FileAccommodationClassCertificate { get; set; }

        [CustomDisplayName("AccommodationTypeService_Label_Class")]
        public List<ListItem> ListAccommodationClass { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public string SavedBy { get; set; }
    }
}