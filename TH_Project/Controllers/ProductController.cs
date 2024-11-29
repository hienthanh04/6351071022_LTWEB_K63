using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TH_Project.Data;
using System.Threading.Tasks;
using System.Data.Entity;
using TH_Project.ViewModel;


namespace TH_Project.Controllers
{
    public class ProductController : Controller
    {
        private readonly QLBANSACHEntities _db;
        public ProductController(QLBANSACHEntities db)
        {
            _db = db;
        }
        public ProductController() : this(new QLBANSACHEntities())
        {
        }
        // GET: Product
        [HttpGet]
        public async Task<ActionResult> Index(int? idNXB, int? idCD)
        {
            var products = new ProductVM() { 
                LOAIXEs = await _db.LOAIXEs.ToListAsync(),
                HSXs = await _db.HANGSANXUATs.ToListAsync()
            };

            if (idNXB.HasValue)
            {
                products.XEes = await _db.XEGANMAYs.Where(s => s.MaXe == idNXB.Value).ToListAsync();
            }
            else if (idCD.HasValue)
            {
                products.XEes = await _db.XEGANMAYs.Where(s => s.MaXe == idCD.Value).ToListAsync();
            }
            else
            {
                products.XEes = await _db.XEGANMAYs.ToListAsync(); 
            }
            ViewData["Chude"] = products.LOAIXEs;
            ViewData["NXB"] = products.HSXs;

            return View(products); 
        }

        [HttpGet]
        public async Task<ActionResult> ProductDetail(int id)
        {

            var product = await _db.XEGANMAYs.FindAsync(id);
            var productVM = new DetailProductVM()
            {
                LoaiXes = await _db.LOAIXEs.ToListAsync(),
                HangSanXuats = await _db.HANGSANXUATs.ToListAsync(),
                Xe = product
            };

            if (product == null)
            {
                return HttpNotFound(); 
            }

            ViewData["Chude"] = productVM.LoaiXes;
            ViewData["NXB"] = productVM.HangSanXuats;
            return View(productVM);
        }


    }
}