using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.App.Attributes;

namespace CenIT.ReportTourism.Models.Cate
{
    public class CateTeamModel
    {
        public int TeamId { get; set; }

        [CustomDisplayName("Team_Label_Ward")]
        [CustomRequired]
        public int? WardId { get; set; }

        [CustomDisplayName("Team_Label_Ward")] public string WardCode { get; set; }

        [CustomDisplayName("Team_Label_Ward")] public string WardTeam { get; set; }

        [CustomDisplayName("Team_Label_Code")]
        [CustomRequired]
        public string TeamCode { get; set; }

        [CustomRequired]
        [CustomDisplayName("Team_Label_Name")]
        public string TeamName { get; set; }

        [CustomDisplayName("Team_Label_Ward")] public string WardName { get; set; }

        public bool IsDeleted { get; set; }
        public int? TotalRow { get; set; } = 0;
        public string UserCreated { get; set; }
        public DateTime DateCreated { get; set; }

        public List<ListItem> Provinces { get; set; } = new List<ListItem>();
        public List<ListItem> Wards { get; set; } = new List<ListItem>();
    }
}