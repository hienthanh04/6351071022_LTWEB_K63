    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TH_Project.Data;

namespace TH_Project.Models
{
    public class GioHang
    {
        QLBANSACHEntities db = new QLBANSACHEntities();
        public int iMaSach {  get; set; }
        public string sTenSach { get; set; }
        public string sAnhBia { get; set; }
        public Double dDonGia { get; set; }
        public int iSoLuong { get; set; }
        public Double dThanhTien
        {
            get {return dDonGia*iSoLuong;}
        }

        public GioHang(int MaSach)
        {
            iMaSach = MaSach;
            XEGANMAY sach = db.XEGANMAYs.Single(s => s.MaXe == iMaSach);
            sTenSach = sach.TenXe;
            sAnhBia = sach.Anhbia;
            dDonGia = double.Parse(sach.Giaban.ToString());
            iSoLuong = 1;
        }
    }
}