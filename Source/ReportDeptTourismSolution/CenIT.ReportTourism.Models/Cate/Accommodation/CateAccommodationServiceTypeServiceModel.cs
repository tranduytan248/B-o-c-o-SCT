using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.Accommodation
{
    public class CateAccommodationServiceTypeServiceModel
    {
        public int AccommodationTypeServiceId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("AccommodationTypeService_Label_Name")]
        [CustomRequired]
        public int ServiceId { get; set; }

        [CustomDisplayName("AccommodationTypeService_Label_Name")]
        public string ServiceName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [CustomDisplayName("AccommodationTypeService_Label_Name")]
        public List<ListItem> ListTypeServices { get; set; }

        public int? TotalRow { get; set; } = 0;

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public string SavedBy { get; set; }
    }
}