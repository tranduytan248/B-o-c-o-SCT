using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.TransportTourists
{
    public class CateTransportTouristsServiceModel
    {
        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }
    }
}