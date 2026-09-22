using System;

namespace CenIT.ReportTourism.Models.Sys
{
    public class SysIPRequestModel
    {
        public string IP { get; set; }
        public bool IsLock { get; set; }
        public DateTime LastestRequest { get; set; }
    }
}