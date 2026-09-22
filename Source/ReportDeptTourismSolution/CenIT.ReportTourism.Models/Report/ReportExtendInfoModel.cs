using System;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Report
{
    public class ReportExtendInfoModel
    {
        public int Id { get; set; }

        [CustomDisplayName("ExtendInfo_Label_ForMonth")]
        [CustomRequired]
        public DateTime ForMonth { get; set; } = DateTime.Now;

        [CustomDisplayName("ExtendInfo_Label_TotalGuestViaShip")]
        [CustomRequired]
        public int TotalGuestViaShip { get; set; }

        public int? TotalRow { get; set; } = 0;

        [CustomDisplayName("Reason_Title")]
        [CustomRequired]
        public string Reason { get; set; }

        public bool IsSelected { get; set; } = false;
        public string SavedBy { get; set; }
    }
}