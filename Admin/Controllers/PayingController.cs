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
            Session["MaGG"] = gg;
            Session["Ship"] = ship;

            return View();
        }

        private void TaoHoaDon(string hinhThucThanhToan)
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
                return;

            int matk = (int)Session["UserID"];

            // ✅ LẤY GIẢM GIÁ & SHIP TỪ SESSION (ổn định nhất)
            int magg = Session["MaGG"] != null ? (int)Session["MaGG"] : 0;
            decimal ship = Session["Ship"] != null ? (decimal)Session["Ship"] : 0;

            decimal tienHang = cart.Sum(x => x.ThanhTien);
            decimal tienGiam = 0;

            if (magg > 0)
            {
                var gg = db.GiamGia.FirstOrDefault(x => x.magg == magg);
                if (gg != null)
                    tienGiam = tienHang * (gg.mucgiam ?? 0) / 100;
            }

            decimal tongTien = tienHang + ship - tienGiam;

            HoaDon hd = new HoaDon
            {
                matk = matk,
                magg = magg > 0 ? (int?)magg : null,
                ngaylap = DateTime.Now,
                diachigiaohang = db.TaiKhoan.Find(matk).diachi,
                tinhtrang = "Chờ xác nhận",
                dathanhtoan = hinhThucThanhToan == "ONLINE",
                tongtien = tongTien // ✅ GÁN LUÔN – KHÔNG CHỜ TRIGGER
            };

            db.HoaDon.Add(hd);
            db.SaveChanges();

            foreach (var item in cart)
            {
                var ct = new CTHoaDon
                {
                    mahd = hd.mahd,
                    masp = item.MaSP,
                    mam = item.MaMau,
                    mas = item.MaSize,
                    soluong = item.SoLuong,
                    dongia = item.Gia
                };

                db.CTHoaDon.Add(ct);
            }

            db.SaveChanges();

            // XÓA SESSION
            Session.Remove("Cart");
            Session.Remove("MaGG");
            Session.Remove("Ship");
        }


        [HttpPost]
        public ActionResult ThanhToanCOD()
        {
            TaoHoaDon("COD");
            return RedirectToAction("MyOrders", "Order");
        }

        [HttpPost]
        public ActionResult XacNhanThanhToan()
        {
            TaoHoaDon("ONLINE");
            return RedirectToAction("MyOrders", "Order");
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