using System;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.TransportTourists
{
    public class CateTransportTouristsServiceInfrastructureModel
    {
        public int InfrastructureId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("TransportTouristsInfrastructure_Label_TotalTransport")]
        public int? TotalTransports { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public string SavedBy { get; set; }
    }
}