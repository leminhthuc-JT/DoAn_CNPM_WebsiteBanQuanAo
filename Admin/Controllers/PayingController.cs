using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class PayingController : Controller
    {
        // GET: Paying
        private G6_QLBQAEntities db = new G6_QLBQAEntities();
        public ActionResult Index(int matk, int masp, int mam, int mas, int quantity)
        {
            SanPham sp = db.SanPham.Where(x => x.masp == masp).FirstOrDefault();
            TaiKhoan tk = db.TaiKhoan.Where(x => x.matk == matk).FirstOrDefault();
            string anh = db.CTSanPham.Where(x => x.masp == masp && x.mam == mam && x.mas == mas).Select(x => x.link).FirstOrDefault();
            ViewBag.SL = quantity;
            ViewBag.Mau = mam;
            ViewBag.Size = mas;
            ViewBag.Gia = sp.giaban * quantity;
            ViewBag.Anh = anh;
            return View();
        }
    }
}