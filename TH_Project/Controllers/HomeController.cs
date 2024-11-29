using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TH_Project.Data;
using System.Threading.Tasks;
using System.Data.Entity;
using TH_Project.ViewModel;

using PagedList;
using PagedList.Mvc;
using PagedList.EntityFramework;


namespace TH_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly QLBANSACHEntities _db;
        public HomeController(QLBANSACHEntities db)
        {
            _db = db;
        }
        public HomeController() : this(new QLBANSACHEntities())
        {
        }
        [HttpGet]
        public async Task<ActionResult> Index(int? page)
        {
            int pageSize = 15;
            int pageNumber = (page ?? 1);

            HomeVM homeVM = new HomeVM()
            {
                XEes = await _db.XEGANMAYs.OrderBy(s => s.MaXe).ToPagedListAsync(pageNumber, pageSize),
                LOAIXEs = await _db.LOAIXEs.ToListAsync(),
                HSXs = await _db.HANGSANXUATs.ToListAsync()
            };

            ViewData["Chude"] = homeVM.LOAIXEs;
            ViewData["NXB"] = homeVM.HSXs;

            return View(homeVM);
        }


    }
}