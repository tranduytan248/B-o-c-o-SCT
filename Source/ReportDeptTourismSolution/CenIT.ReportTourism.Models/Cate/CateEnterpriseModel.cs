using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate
{
    public class CateEnterpriseModel
    {
        public CateEnterpriseModel()
        {
            ListWards = ListProvinces = ListTypeBusiness = new List<ListItem>();
        }

        public int EnterpriseId { get; set; }

        [CustomDisplayName("Enterprise_Label_ReCaptcha")]
        public string Captcha { get; set; }

        [CustomDisplayName("Enterprise_Label_Owner")]
        [CustomRequired]
        public string OwnerEnterpriseName { get; set; }

        [CustomDisplayName("Enterprise_Label_BusinessName")]
        [CustomRequired]
        public string BusinessName { get; set; }

        [CustomDisplayName("Enterprise_Label_TaxCode")]
        [CustomRequired]
        public string TaxCode { get; set; }

        [CustomDisplayName("Enterprise_Label_Address")]
        public string BusinessAddress { get; set; }

        [CustomDisplayName("Enterprise_Label_Street")]
        public string StreetName { get; set; }

        [CustomDisplayName("Enterprise_Label_Ward")]
        [CustomRequired]
        public int? WardId { get; set; }

        [CustomDisplayName("Enterprise_Label_Ward")]
        public string WardName { get; set; }

        [CustomDisplayName("Enterprise_Label_Province")]
        [CustomRequired]
        public int? ProvinceId { get; set; }

        [CustomDisplayName("Enterprise_Label_Province")]
        public string ProvinceName { get; set; }

        [CustomDisplayName("Enterprise_Label_TypeBusiness")]
        [CustomRequired]
        public int TypeBusiness { get; set; }

        [CustomDisplayName("Enterprise_Label_TypeBusiness")]
        public string TypeBusinessName { get; set; }

        [CustomDisplayName("Enterprise_Label_LegalRepresentationName")]
        public string LegalRepresentationName { get; set; }

        [CustomDisplayName("Enterprise_Label_LegalRepresentationPhone")]
        [CustomRequired]
        public string LegalRepresentationPhone { get; set; }

        [CustomDisplayName("Enterprise_Label_LegalRepresentationEmail")]
        [CustomRequired]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        public string LegalRepresentationEmail { get; set; }

        [CustomDisplayName("Enterprise_Label_Website")]
        public string Website { get; set; }

        [CustomDisplayName("Enterprise_Label_Phone")]
        public string Phone { get; set; }

        [CustomDisplayName("Enterprise_Label_Email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [CustomRequired]
        public string Email { get; set; }

        [CustomDisplayName("Enterprise_Label_BusinessCertificateFiles")]
        public DataTable CertificateFiles { get; set; }

        [CustomDisplayName("Enterprise_Label_BusinessCertificateFiles")]
        public string ListPathCertificateFiles { get; set; }

        [CustomDisplayName("Enterprise_Label_BusinessCertificateFiles")]
        public List<HttpPostedFileBase> ListCertificateFiles { get; set; }

        public bool IsDeleted { get; set; }

        [CustomDisplayName("Enterprise_Label_IsActive")]
        public bool IsActive { get; set; } = true;

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [CustomDisplayName("Enterprise_Label_TypeBusiness")]
        public List<ListItem> ListTypeBusiness { get; set; }

        [CustomDisplayName("Enterprise_Label_Ward")]
        public List<ListItem> ListWards { get; set; }

        [CustomDisplayName("Enterprise_Label_Province")]
        public List<ListItem> ListProvinces { get; set; }

        public int? TotalRow { get; set; } = 0;

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public bool IsSelected { get; set; } = false;

        public bool IsConfirm { get; set; } = false;

        public string SavedBy { get; set; }

        public string Password { get; set; } = null;
        public string Salt { get; set; }
    }

    public class TemplateNotifyEnterpise
    {
        public int EnterpriseId { get; set; }

        public string EnterpriseName { get; set; }

        public int DayDeadlineSendReport { get; set; } = 0;

        public int DayDeadlineSendReportLate { get; set; } = 0;

        public string UrlSubmitReport { get; set; }

    }
}