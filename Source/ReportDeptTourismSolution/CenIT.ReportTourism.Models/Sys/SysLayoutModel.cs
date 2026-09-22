using System;
using System.Collections.Generic;
using System.Web;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Sys
{
    public class SysLayoutModel
    {
        public int LayoutId { get; set; }

        [CustomRequired]
        [CustomDisplayName("AppLayout_Label_LayoutName")]
        public string LayoutName { get; set; }

        [CustomRequired]
        [CustomDisplayName("AppLayout_Label_LayoutView")]
        public string LayoutView { get; set; }

        [CustomDisplayName("AppLayout_Label_Note")]
        public string Note { get; set; }

        [CustomDisplayName("AppLayout_Label_NumberContentPanel")]
        public int NumberContentPanel { get; set; } = 1;

        [CustomDisplayName("AppLayout_Label_NumberCol")]
        public int NumberCol { get; set; } = 1;

        public string Creator { get; set; } = null;
        public DateTime? CreateDated { get; set; } = null;
        public string Updater { get; set; } = null;
        public DateTime? UpdateDated { get; set; } = null;
        public bool Deleted { get; set; } = false;
        public bool Activated { get; set; } = false;

        [CustomDisplayName("AppLayout_Label_ZipFile")]
        public HttpPostedFileBase ZipFile { get; set; }

        public List<SysContentPanelModel> ContentPanels { get; set; }
        public int? TotalRow { get; set; } = 0;
    }
}