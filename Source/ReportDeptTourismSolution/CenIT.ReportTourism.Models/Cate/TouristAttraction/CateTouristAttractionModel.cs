using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.TouristAttraction
{
    public class CateTouristAttractionModel
    {
        public CateTouristAttractionModel()
        {
            ListTypeTourismActivities = new List<ListItem>();
            ListTypeTourismActivitieIds = new List<int>();
        }

        public int TouristAttractionId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("TouristAttraction_Label_TypeTourismActivity")]
        public List<int> ListTypeTourismActivitieIds { get; set; }

        [CustomDisplayName("TouristAttraction_Label_TypeTourismActivity")]
        public string TypeTourismActivities { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public List<ListItem> ListTypeTourismActivities { get; set; }

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public string SavedBy { get; set; }
    }
}