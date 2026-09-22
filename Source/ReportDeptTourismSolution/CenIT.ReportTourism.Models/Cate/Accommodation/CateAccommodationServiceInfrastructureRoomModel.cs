using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.Accommodation
{
    public class CateAccommodationServiceInfrastructureRoomModel
    {
        public int InfrastructureRoomId { get; set; }
        public int InfrastructureId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("AccommodationInfrastructure_Label_TypeRoom")]
        [CustomRequired]
        public int RoomType { get; set; }

        [CustomDisplayName("AccommodationInfrastructure_Label_TypeRoom")]
        public string RoomTypeName { get; set; }

        [CustomDisplayName("AccommodationInfrastructure_Label_TotalRoom")]
        [CustomRequired]
        public int TotalRooms { get; set; }

        [CustomDisplayName("AccommodationInfrastructure_Label_AnnouncedPrice")]
        [CustomRequired]
        public double AnnouncedPrice { get; set; }

        public string AnnouncedPriceView { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [CustomDisplayName("AccommodationInfrastructure_Label_RoomType")]
        public List<ListItem> ListRoomTypes { get; set; }

        public int? TotalRow { get; set; } = 0;

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public string SavedBy { get; set; }
    }
}