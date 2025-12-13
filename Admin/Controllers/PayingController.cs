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
        public ActionResult Index(int matk = 0, int masp = 0, int mam = 0, int mas = 0, int quantity = 0, int search = 0, decimal ship = 0, int gg = 0, string tt = "")
        {
            SanPham sp = db.SanPham.Where(x => x.masp == masp).FirstOrDefault();
            TaiKhoan tk = db.TaiKhoan.Where(x => x.matk == matk).FirstOrDefault();
            string anh = db.CTSanPham.Where(x => x.masp == masp && x.mam == mam && x.mas == mas).Select(x => x.link).FirstOrDefault();
            string mau = db.mau.Where(x => x.mam == mam).Select(x => x.tenm).FirstOrDefault();
            string size = db.size.Where(x => x.mas == mas).Select(x => x.tens).FirstOrDefault();
            ViewBag.SL = quantity;
            ViewBag.Mau = mau;
            ViewBag.Size = size;
            ViewBag.MaM = mam;
            ViewBag.MaS = mas;
            ViewBag.Ship = ship;
            ViewBag.MaSP = masp;
            ViewBag.MaTK = matk;
            GiamGia vou = new GiamGia();
            if (gg > 0)
            {
                vou = db.GiamGia.FirstOrDefault(x => x.magg == gg);
            }

            // GÁN MẶC ĐỊNH nếu không có voucher
            if (vou == null)
            {
                vou = new GiamGia
                {
                    magg = 0,
                    mucgiam = 0
                };
            }

            ViewBag.GiamGia = vou;
            ViewBag.TT = tt;
            ViewBag.Search = search;
            //ViewBag.Gia = sp.giaban * quantity;
            ViewBag.Anh = anh;
            List<GiamGia> ggia = db.GiamGia.ToList();
            if(search != 0)
            {
                ggia = db.GiamGia.Where(x => x.magg == search).ToList();
            }

            ViewBag.GG = ggia;
            return View(sp);
        }
    }
}