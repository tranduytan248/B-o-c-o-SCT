using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.Traveling
{
    public class CateTravelingServiceModel
    {
        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }
    }
}