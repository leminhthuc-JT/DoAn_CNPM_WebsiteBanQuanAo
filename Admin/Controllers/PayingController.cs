using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using QRCoder;

namespace Admin.Controllers
{
    public class PayingController : Controller
    {
        // GET: Paying
        private G6_QLBQAEntities db = new G6_QLBQAEntities();
        public ActionResult Index(decimal ship = 0, int gg = 0, string tt = "")
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "Cart");

            // Tổng tiền hàng
            decimal tienHang = cart.Sum(x => x.ThanhTien);

            // Voucher
            GiamGia voucher = null;
            if (gg > 0)
                voucher = db.GiamGia.FirstOrDefault(x => x.magg == gg);

            int mucGiam = voucher?.mucgiam ?? 0;
            decimal tienGiam = tienHang * mucGiam / 100;
            decimal tongTien = tienHang + ship - tienGiam;

            ViewBag.Cart = cart;
            ViewBag.Ship = ship;
            ViewBag.GiamGia = voucher;
            ViewBag.TT = tt;
            ViewBag.TienHang = tienHang;
            ViewBag.TienGiam = tienGiam;
            ViewBag.TongTien = tongTien;

            // thông tin user
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int matk = (int)Session["UserID"];
            ViewBag.TK = db.TaiKhoan.Find(matk);

            ViewBag.GG = db.GiamGia.ToList();

            return View();
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