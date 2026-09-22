using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.ServicesForTourist
{
    public class CateServicesForTouristModel
    {
        public CateServicesForTouristModel()
        {
            ListTypeServicesForTourists = new List<ListItem>();
            ListTypeServicesForTouristIds = new List<int>();
        }

        public int ServicesForTouristId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("ServicesForTourists_Label_TypeServicesForTourist")]
        public List<int> ListTypeServicesForTouristIds { get; set; }

        [CustomDisplayName("ServicesForTourists_Label_TypeServicesForTourist")]
        public string TypeServicesForTourists { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public List<ListItem> ListTypeServicesForTourists { get; set; }

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public string SavedBy { get; set; }
    }
}