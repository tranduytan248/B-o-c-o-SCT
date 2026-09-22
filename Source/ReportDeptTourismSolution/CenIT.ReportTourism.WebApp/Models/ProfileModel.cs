using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.WebApp.Models
{
    public class ProfileModel
    {
        public List<ListItem> ListLanguages = new List<ListItem>
        {
            new ListItem {Text = "Asp.Net", Value = "1"},
            new ListItem {Text = "SQL Server", Value = "2"},
            new ListItem {Text = "Html", Value = "3"},
            new ListItem {Text = "JQuery", Value = "4"},
            new ListItem {Text = "CSS3", Value = "5"}
        };

        public List<ListItem> ListWards = new List<ListItem>
        {
            new ListItem {Text = "Tp. Nha Trang", Value = "1"},
            new ListItem {Text = "Tp. Cam Ranh", Value = "2"},
            new ListItem {Text = "Tx. Ninh Hoà", Value = "3"},
            new ListItem {Text = "Cam Lâm", Value = "4"},
            new ListItem {Text = "Vạn Ninh", Value = "5"}
        };

        [CustomRequired]
        [CustomDisplayName("User_Label_FullName")]
        public string Name { get; set; }

        public int Gender { get; set; } = 1;
        public int? Age { get; set; } = 18;

        [CustomRequired] public DateTime? Birthday { get; set; }

        public string Description { get; set; }
        public int? WardID { get; set; }
        public string Languages { get; set; }
        public bool IsActive { get; set; }
        public string Skills { get; set; }

        public string UrlAvatar { get; set; } = "/Contents/imgs/avatar-default.png";
        public HttpPostedFileBase Avatar { get; set; }
        public HttpPostedFileBase CVFile { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}