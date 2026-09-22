using System;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.Accommodation
{
    public class CateAccommodationServiceInfrastructureModel
    {
        public int InfrastructureId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("AccommodationInfrastructure_Label_InitialInvestmentCapital")]
        public double? InitialInvestmentCapital { get; set; }

        public string InitialInvestmentCapitalView { get; set; }

        [CustomDisplayName("AccommodationInfrastructure_Label_UpgradeInvestmentCapital")]
        public double? UpgradeInvestmentCapital { get; set; }

        public string UpgradeInvestmentCapitalView { get; set; }

        [CustomDisplayName("AccommodationInfrastructure_Label_TotalArea")]
        public double? TotalArea { get; set; }

        [CustomDisplayName("AccommodationInfrastructure_Label_TotalConstructionArea")]
        public double? TotalConstructionArea { get; set; }

        [CustomDisplayName("AccommodationInfrastructure_Label_TotalRoom")]
        public int? TotalRoom { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public string SavedBy { get; set; }
    }
}