using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TH_Project.Data;

namespace TH_Project.ViewModel
{
    public class DetailProductVM
    {
        public XEGANMAY Xe { get; set; } // Sửa tên SACH thành Xe nếu là xe gắn máy
        public IEnumerable<LOAIXE> LoaiXes { get; set; } // Sửa CHUDE thành LoaiXe hoặc bảng phù hợp
        public IEnumerable<HANGSANXUAT> HangSanXuats { get; set; } // Sửa NHAXUATBAN thành NhaXuatBan hoặc bảng phù hợp
    }
}