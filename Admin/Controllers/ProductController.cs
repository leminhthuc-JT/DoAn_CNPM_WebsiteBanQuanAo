using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class ProductController : Controller
    {
        G6_QLBQAEntities db = new G6_QLBQAEntities();
        // GET: Product
        public ActionResult ChiTietSP(int masp)
        {
            var sp = db.SanPham.FirstOrDefault(s => s.masp == masp);
            //sản phẩm liên quan
            List<SanPham> lienquan = db.SanPham.Where(s => s.madm == sp.madm || s.maloai == sp.maloai || s.math == sp.math && s.masp != masp).ToList();
            ViewBag.LQ = lienquan;
            var thuongHieu = db.ThuongHieu.FirstOrDefault(t => t.math == sp.math);
            ViewBag.TH = thuongHieu.tenth;
<<<<<<< HEAD
=======
            List<CTSanPham> color = db.CTSanPham.Where(r => r.masp == masp).GroupBy(r => r.mam).Select(g => g.FirstOrDefault()).ToList();
            List<CTSanPham> size = db.CTSanPham.Where(r => r.masp == masp).GroupBy(r => r.mas).Select(g => g.FirstOrDefault()).ToList();
            ViewBag.Image = color;
            ViewBag.Size = size;

            var reviews = db.BinhLuan.Where(bl => bl.masp == masp).OrderByDescending(bl => bl.ngay).ToList();

            ViewBag.Reviews = reviews;
>>>>>>> 1c51ef331213275be17e3d2aebf5834e4bc02749
            return View(sp);
        }
    }
}