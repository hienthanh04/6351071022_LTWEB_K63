using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TH_Project.Data;

namespace TH_Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly QLBANSACHEntities _db;
        public AdminController(QLBANSACHEntities db)
        {
            _db = db;
        }
        public AdminController() : this(new QLBANSACHEntities())
        {
        }
        // GET: Admin
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Sach(int? page)
        {
            int pageSize = 9; // Số lượng sách trên mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là trang 1

            var books = await _db.XEGANMAYs.ToListAsync(); // Lấy tất cả sách từ cơ sở dữ liệu
            var pagedBooks = books.ToPagedList(pageNumber, pageSize); // Phân trang danh sách sách

            return View(pagedBooks); // Trả về view với danh sách đã phân trang
        }






        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            // Lấy dữ liệu từ FormCollection
            var tendn = collection["username"];
            var matkhau = collection["password"];

            // Kiểm tra dữ liệu nhập vào
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }

            // Kiểm tra nếu không có lỗi
            if (string.IsNullOrEmpty(ViewData["Loi1"]?.ToString()) && string.IsNullOrEmpty(ViewData["Loi2"]?.ToString()))
            {
                // Kiểm tra tên đăng nhập và mật khẩu
                Admin ad = _db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    // Lưu thông tin vào session và chuyển hướng
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewData["Thongbao"] = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }

            // Trả về view nếu có lỗi
            return View();
        }


        [HttpGet]
        public async Task<ActionResult> Create()
        {

            ViewBag.MaCD = new SelectList(_db.LOAIXEs.ToList().OrderBy(n => n.TenLoaiXe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(_db.HANGSANXUATs.ToList().OrderBy(n => n.TenHSX), "MaNXB", "TenNXB");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]

        public async Task<ActionResult> Create(XEGANMAY sach, HttpPostedFileBase fileUpload)
        {
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                var fileName = Path.GetFileName(fileUpload.FileName); // Lưu đường dẫn của file
                var path = Path.Combine(Server.MapPath("~/Content/images"), fileName); // Kiểm tra hình ảnh tồn tại chưa?

                if (System.IO.File.Exists(path))
                {
                    ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                }
                else
                {
                    // Lưu hình ảnh vào đường dẫn
                    fileUpload.SaveAs(path);
                    sach.Anhbia = "/Content/images/" + fileName; // Lưu đường dẫn hình ảnh vào đối tượng sach
                }
            }
            else
            {
                ViewBag.Thongbao = "Vui lòng chọn hình ảnh để tải lên."; // Thông báo nếu không có tệp nào được tải lên
            }

            // Gửi dữ liệu cho View
            ViewBag.MaCD = new SelectList(_db.LOAIXEs.ToList().OrderBy(n => n.TenLoaiXe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(_db.HANGSANXUATs.ToList().OrderBy(n => n.TenHSX), "MaNXB", "TenNXB");

            // Lưu thông tin sách vào cơ sở dữ liệu
            _db.XEGANMAYs.Add(sach);
            await _db.SaveChangesAsync();

            // Trả về View
            return RedirectToAction("Sach");
        }

        public async Task UploadImageAsync(XEGANMAY product, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                // Tạo tên file duy nhất để tránh trùng lặp
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(Server.MapPath("~/Content/images/"), uniqueFileName);

                // Kiểm tra xem thư mục đã tồn tại chưa, nếu chưa thì tạo mới
                var directoryPath = Server.MapPath("~/Content/images/");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath); // Tạo thư mục nếu chưa có
                }

                // Lưu tệp vào thư mục
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.InputStream.CopyToAsync(fileStream);
                }

                // Cập nhật đường dẫn hình ảnh vào đối tượng product
                product.Anhbia = "/Content/images/" + uniqueFileName;
            }
            else
            {
                throw new InvalidOperationException("Không có tệp nào được tải lên.");
            }
        }



        public async Task<ActionResult> Details(int? id)
        {
            // Kiểm tra nếu id không có giá trị
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // Trả về mã lỗi 400 (Bad Request)
            }

            // Tìm xe trong cơ sở dữ liệu
            XEGANMAY sach = await _db.XEGANMAYs.FindAsync(id);
            if (sach == null)
            {
                return HttpNotFound("Không tìm thấy xe."); // Trả về mã lỗi 404 nếu không tìm thấy
            }

            // Truyền dữ liệu qua ViewBag (nếu cần thiết)
            ViewBag.Masach = sach.MaXe;

            // Trả về view với model là đối tượng sách
            return View(sach);
        }


        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        { // lấy ra đối tượng sách cần xóa theo mã
            XEGANMAY sach = await _db.XEGANMAYs.SingleOrDefaultAsync(n => n.MaXe == id);
            ViewBag.Masach = sach.MaXe;
            if (sach == null) { Response.StatusCode = 404; return null; }
            return View(sach);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirm(int id)
        {
            var sach = await _db.XEGANMAYs.FindAsync(id);

            if (sach == null)
            {
                return HttpNotFound();
            }

            _db.XEGANMAYs.Remove(sach);
            await _db.SaveChangesAsync();

            return RedirectToAction("Sach");
        }

        // Chỉnh sửa sản phẩm
        [HttpGet]
        public ActionResult Suasach(int id)
        {

            {
                Response.StatusCode = 400;
                return HttpNotFound("ID không được cung cấp.");
            }
            // Lấy đối tượng sách từ database
            XEGANMAY sach = _db.XEGANMAYs.SingleOrDefault(n => n.MaXe == id);

            // Kiểm tra nếu sách không tồn tại
            if (sach == null)
            {
                Response.StatusCode = 404;
                return HttpNotFound("Không tìm thấy xe.");
            }

            // Tạo SelectList cho dropdown
            ViewBag.MaCD = new SelectList(_db.LOAIXEs.OrderBy(n => n.TenLoaiXe), "MaCD", "TenChude", sach.MaLX);
            ViewBag.MaNXB = new SelectList(_db.HANGSANXUATs.OrderBy(n => n.TenHSX), "MaNXB", "TenNXB", sach.MaNPP);

            return View(sach); // Trả về View với model sách
        }



        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Suasach(XEGANMAY sach, HttpPostedFileBase fileUpload)
        {
            // Kiểm tra xem model có hợp lệ không
            if (sach == null)
            {
                ViewBag.Thongbao = "Thông tin xe không hợp lệ.";
                return View(sach);
            }

            // Xử lý upload file hình ảnh
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                // Tạo tên file duy nhất
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileUpload.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/images/"), uniqueFileName);

                // Kiểm tra nếu thư mục chứa ảnh không tồn tại, thì tạo mới
                var directoryPath = Server.MapPath("~/Content/images/");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Lưu file vào server
                fileUpload.SaveAs(path);
                sach.Anhbia = "/Content/images/" + uniqueFileName; // Cập nhật đường dẫn ảnh
            }

            try
            {
                // Tìm sách trong database
                var model = await _db.XEGANMAYs.FirstOrDefaultAsync(n => n.MaXe == sach.MaXe);

                if (model != null)
                {
                    // Cập nhật thông tin sách
                    model.TenXe = sach.TenXe;
                    model.Mota = sach.Mota;
                    model.MaLX = sach.MaLX;
                    model.MaNPP = sach.MaNPP;
                    model.Giaban = sach.Giaban;
                    model.Soluongton = sach.Soluongton;

                    // Nếu người dùng không upload ảnh mới, giữ nguyên ảnh cũ
                    if (!string.IsNullOrEmpty(sach.Anhbia))
                    {
                        model.Anhbia = sach.Anhbia;
                    }

                    model.Ngaycapnhat = DateTime.Now;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _db.SaveChangesAsync();
                    TempData["Message"] = "Cập nhật xe thành công!";
                    return RedirectToAction("Sach");
                }
                else
                {
                    ViewBag.Thongbao = "Không tìm thấy xe cần sửa.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Thongbao = "Đã xảy ra lỗi: " + ex.Message;
            }

            // Tạo lại SelectList cho dropdown
            ViewBag.MaCD = new SelectList(_db.LOAIXEs.OrderBy(n => n.TenLoaiXe), "MaCD", "TenChude", sach.MaLX);
            ViewBag.MaNXB = new SelectList(_db.HANGSANXUATs.OrderBy(n => n.TenHSX), "MaNXB", "TenNXB", sach.MaNPP);

            // Trả về view với dữ liệu model
            return View(sach);
        }



        public ActionResult ThongKeSach()
        {
            var chartData = _db.XEGANMAYs
                               .GroupBy(s => s.MaLX)
                               .Select(g => new
                               {
                                   TenChuDe = g.FirstOrDefault().LOAIXE.TenLoaiXe,
                                   SoLuongSach = g.Count() 
                               })
                               .ToList();

            return View(chartData);
        }





    }
}