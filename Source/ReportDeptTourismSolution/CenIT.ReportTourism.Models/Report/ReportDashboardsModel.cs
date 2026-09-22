using System;

namespace CenIT.ReportTourism.Models.Report
{
    public class Thongkebaocaothang
    {
        public Thongkebaocaothang()
        {
            ForMonth = DateTime.UtcNow;
            SoLuong = 0;
        }

        public DateTime ForMonth { get; set; }
        public int SoLuong { get; set; }
    }

    public class ThongkebaocaoTre
    {
        public ThongkebaocaoTre()
        {
            ForMonth = DateTime.UtcNow;
            SoLuongBaoCaoTre = 0;
        }

        public DateTime ForMonth { get; set; }

        public int SoLuongBaoCaoTre { get; set; }
    }

    public class ThongkebaocaoChuaNop
    {
        public ThongkebaocaoChuaNop()
        {
            ForMonth = DateTime.UtcNow;
            SoLuongBaoCaoChuaNop = 0;
        }

        public DateTime ForMonth { get; set; }
        public int SoLuongBaoCaoChuaNop { get; set; }
    }

    public class Thongkedoanhnghiep
    {
        public Thongkedoanhnghiep()
        {
            ForMonth = DateTime.UtcNow;
            DN_Luutru = 0;
            DN_PhucVuLuHanh = 0;
            DN_DiemDuLich = 0;
            DN_KhuDiemDuLich = 0;
            DN_VanChuyen = 0;
        }

        public DateTime ForMonth { get; set; }
        public int DN_Luutru { get; set; }
        public int DN_PhucVuLuHanh { get; set; }
        public int DN_DiemDuLich { get; set; }
        public int DN_KhuDiemDuLich { get; set; }
        public int DN_VanChuyen { get; set; }
    }
}