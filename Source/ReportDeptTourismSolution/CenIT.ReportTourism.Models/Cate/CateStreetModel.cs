using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate
{
    public class CateStreetModel
    {
        public int? StreetId { get; set; } = 0;

        [CustomDisplayName("Street_Label_Parent")]
        public int? ParentId { get; set; } = 0;

        [CustomDisplayName("Street_Label_Code")]
        [CustomRequired]
        public string StreetCode { get; set; }

        [CustomRequired]
        [CustomDisplayName("Street_Label_Name")]
        public string StreetName { get; set; }

        [CustomDisplayName("Street_Label_Parent")]
        public string ParentName { get; set; }

        [CustomDisplayName("Street_Label_Parent")]
        public string ParentCode { get; set; }

        public string UserCreated { get; set; }
        public DateTime DateCreated { get; set; }

        public int? TotalRow { get; set; } = 0;

        public List<ListItem> Provinces { get; set; }
        public List<ListItem> Wards { get; set; }
        public List<ListItem> Streets { get; set; }

        [CustomRequired]
        [CustomDisplayName("Street_Label_Ward")]
        public string WardIds { get; set; }

        [CustomDisplayName("Street_Label_Ward")]
        public int? WardId { get; set; }
    }
}