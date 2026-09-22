using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Sys
{
    public class SysMessageModel
    {
        [CustomRequired]
        [CustomDisplayName("Message_Label_LangCode")]
        public string LangCode { get; set; }

        [CustomRequired]
        [CustomDisplayName("Message_Label_LabelKey")]
        public string LabelKey { get; set; }

        [CustomRequired]
        [CustomDisplayName("Message_Label_Message")]
        public string Message { get; set; }

        public int? TotalRow { get; set; } = 0;
    }
}