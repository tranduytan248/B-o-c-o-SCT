using System;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate.Traveling
{
    public class CateTravelingServiceBranchOfficeModel
    {
        public int BranchOfficeId { get; set; }

        [CustomDisplayName("Enterprise_Title")]
        [CustomRequired]
        public int EnterpriseId { get; set; }

        [CustomDisplayName("TravelingBranchOffice_Label_BranchName")]
        [CustomRequired]
        public string BranchName { get; set; }

        [CustomDisplayName("TravelingBranchOffice_Label_BranchAddress")]
        public string BranchAddress { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? TotalRow { get; set; } = 0;

        [CustomDisplayName("Reason_Title")] public string Reason { get; set; }

        public string SavedBy { get; set; }
    }
}