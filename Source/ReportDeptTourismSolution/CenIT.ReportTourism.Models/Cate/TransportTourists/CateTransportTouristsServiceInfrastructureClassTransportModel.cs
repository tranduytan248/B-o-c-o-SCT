using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.TransportTourists
{
    public class CateTransportTouristsServiceInfrastructureClassTransportModel
    {
        public CateTransportTouristsServiceInfrastructureClassTransportModel()
        {
            ListTypeTransports = new List<ListItem>();
        }

        public int InfrastructureClassTransportId { get; set; }
        public int InfrastructureId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("TransportTouristsInfrastructure_Label_TypeTransport")]
        [CustomRequired]
        public int TypeTransport { get; set; }

        [CustomDisplayName("TransportTouristsInfrastructure_Label_TypeTransport")]
        public string TypeTransportName { get; set; }

        [CustomDisplayName("TransportTouristsInfrastructure_Label_TypeTransport")]
        public List<ListItem> ListTypeTransports { get; set; }

        [CustomDisplayName("TransportTouristsInfrastructure_Label_ClassTransport")]
        public string ClassTransportName { get; set; }

        [CustomDisplayName("TransportTouristsInfrastructure_Label_TotalTransport")]
        [CustomRequired]
        public int TotalTransport { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? TotalRow { get; set; } = 0;

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public string SavedBy { get; set; }
    }
}