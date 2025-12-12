using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class HomeController : Controller
    {
        G6_QLBQAEntities db = new G6_QLBQAEntities();
        // GET: Home
        public ActionResult Index()
        {
            List<SanPham> lstSP = db.SanPham.ToList();
            return View(lstSP);
        }
    }
}