using System;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate
{
    public class CateEnterpriseRegisterNotifyModel
    {
        [CustomDisplayName("Enterprise_Label")]
        [CustomRequired]
        public int EnterpriseID { get; set; }

        [CustomDisplayName("Enterprise_Label_Owner")]
        public string OwnerEnterpriseName { get; set; }

        [CustomDisplayName("Enterprise_Label_BusinessName")]
        public string BusinessName { get; set; }

        [CustomDisplayName("Enterprise_Notify_Label_HasProcessed")]
        public bool HasProcessed { get; set; }

        [CustomDisplayName("Enterprise_Notify_Label_CompletedBy")]
        public string CompletedBy { get; set; }

        public bool IsConfirm { get; set; }

        public string SaveBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? TotalRow { get; set; } = 0;
    }
}