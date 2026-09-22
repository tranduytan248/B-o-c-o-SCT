using System.Collections.Generic;
using System.IO;

namespace CenIT.ReportTourism.WebApp.Models
{
    public class SysLogModel
    {
        public SysLogModel()
        {
            ListErrFile = new List<FileInfo>();
        }

        public List<FileInfo> ListErrFile { get; set; }
    }
}