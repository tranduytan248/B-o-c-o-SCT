using System;
using System.Collections.Generic;

namespace CenIT.ReportTourism.Models.Report
{
    public class DashboardModel
    {
        public DashboardModel()
        {
            ListBaocaoCacThang = new List<Thongkebaocaothang>();
            ListBaocaoTre = new List<ThongkebaocaoTre>();
            ListBaocaoChuaNop = new List<ThongkebaocaoChuaNop>();
            ListThongkedoanhnghiep = new List<Thongkedoanhnghiep>();
        }

        public DateTime? ForMonth { get; set; }
        public List<Thongkebaocaothang> ListBaocaoCacThang { get; set; }
        public List<ThongkebaocaoTre> ListBaocaoTre { get; set; }
        public List<ThongkebaocaoChuaNop> ListBaocaoChuaNop { get; set; }
        public List<Thongkedoanhnghiep> ListThongkedoanhnghiep { get; set; }
    }
}