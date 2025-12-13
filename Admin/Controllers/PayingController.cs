using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
            ViewBag.TK = tk;
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

        public ActionResult QRThanhToan(decimal soTien, string noiDung)
        {
            string bankBin = "970436"; // Vietcombank
            string accountNumber = "1040408564";
            string merchantInfo =
                "0010A000000727" +
                "01" + bankBin.Length.ToString("D2") + bankBin +
                "02" + accountNumber.Length.ToString("D2") + accountNumber;

            string qrContent =
                                "000201010212" +
                "38" + merchantInfo.Length.ToString("D2") + merchantInfo +
                "5303704" +
                $"54{soTien.ToString("000000")}" +
                "5802VN" +
                $"62{noiDung.Length.ToString("D2")}{noiDung}";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            using (Bitmap bitmap = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return File(ms.ToArray(), "image/png");
                }
            }
        }
    }
}