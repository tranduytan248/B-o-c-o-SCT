using System.Collections.Generic;

namespace CenIT.ReportTourism.Models.Sys
{
    public class SysMenuViewModel
    {
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public string Name { get; set; }
        public int Position { get; set; } = 1;
        public int LevelMenu { get; set; } = 1;
        public string Depth { get; set; }
        public string Link { get; set; }
        public string Icon { get; set; }
        public int FunctionActionId { get; set; }

        public List<SysMenuViewModel> Childs { get; set; }
    }
}