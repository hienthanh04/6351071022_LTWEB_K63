using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TH_Project.Data;
using TH_Project.Models;

namespace TH_Project.Controllers
{
    public class GioHangController : Controller
    {
        private readonly QLBANSACHEntities _db;
        public GioHangController(QLBANSACHEntities db)
        {
            _db = db;
        }
        public GioHangController() : this(new QLBANSACHEntities())
        {
        }

        public List<GioHang> LayGioHang()
        {
            var list = Session["GioHang"] as List<GioHang>;
            if (list == null)
            {
                list = new List<GioHang>();
                Session["GioHang"] = list;

            }
            return list;
        }
        // GET: GioHang
        public ActionResult Index()
        {
            var list = LayGioHang();
            if(list.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();

            ViewData["Chude"] =  _db.LOAIXEs.ToList();
            ViewData["NXB"] =  _db.HANGSANXUATs.ToList();
            return View(list);
        }

        public ActionResult ThemGioHang(int iMaSach, string strURL)
        {
           
            var list = LayGioHang();
            var sanPham = list.Find(n => n.iMaSach == iMaSach);

            if (sanPham == null)
            {
                sanPham = new GioHang(iMaSach);
                list.Add(sanPham);
                return Redirect(strURL);
            }
            else
            {
                sanPham.iSoLuong++;
                return Redirect(strURL);
            }
        }
        public ActionResult XoaGioHang(int iMaSP)
        {
            List<GioHang> lstGiohang = LayGioHang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.iMaSach == iMaSP);
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iMaSach == iMaSP);
                return RedirectToAction("Index");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["UserName"] == null || string.IsNullOrEmpty(Session["UserName"].ToString()) ||
     Session["TaiKhoan"] == null || string.IsNullOrEmpty(Session["TaiKhoan"].ToString()))
            {
                return RedirectToAction("Index", "User");
            }

            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<GioHang> lstGiohang = LayGioHang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewData["Chude"] =  _db.LOAIXEs.ToList();
            ViewData["NXB"] =  _db.HANGSANXUATs.ToList();
            return View(lstGiohang);
        }

        public ActionResult DatHang(FormCollection collection)
        {
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("DangNhap", "User");
            }

            List<GioHang> gh = LayGioHang();
            if (gh == null || gh.Count == 0)
            {
                return RedirectToAction("Index", "Home");  
            }

            DONDATHANG ddh = new DONDATHANG(); 
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;

            var ngaygiao = collection["Ngaygiao"];
            if (DateTime.TryParse(ngaygiao, out DateTime ngayGiaoParsed))
            {
                ddh.Ngaygiao = ngayGiaoParsed;
            }
            else
            {
               
                ddh.Ngaygiao = DateTime.Now.AddDays(1);  
            }

            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;

            _db.DONDATHANGs.Add(ddh);  
            _db.SaveChanges(); 

            foreach (var item in gh)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaXe = item.iMaSach;
                ctdh.Soluong = item.iSoLuong;
                ctdh.Dongia = (decimal)item.dDonGia; 

                _db.CHITIETDONTHANGs.Add(ctdh);  
            }

            _db.SaveChanges();  

            Session["GioHang"] = null;

            return RedirectToAction("XacNhanDonHang", "GioHang");
        }

        public ActionResult XacNhanDonHang()
        {
            ViewData["Chude"] = _db.LOAIXEs.ToList();
            ViewData["NXB"] = _db.HANGSANXUATs.ToList();
            return View();
        }

        public ActionResult CapnhatGiohang(int iMaSP, FormCollection f)
        {
            List<GioHang> lstGiohang = LayGioHang();

            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.iMaSach == iMaSP);

            if (sanpham != null)
            {
                sanpham.iSoLuong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Index");
        }
        public ActionResult XoaTatcaGiohang()
        {
            // Lấy giỏ hàng từ Session
            List<GioHang> lstGiohang = LayGioHang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "Home");
        }


        public int TongSoLuong()
        {
            int count = 0;
            var list = Session["GioHang"] as List<GioHang>;
            if(list != null)
            {
                count  = list.Sum(n => n.iSoLuong);
            }

            return count;
        }

        public Double TongTien()
        {
            double sum = 0;
            var list = Session["GioHang"] as List<GioHang>;
            if (list != null) { 
                sum = list.Sum(n => n.dThanhTien);
            }   
            return sum;
        }
    }
}