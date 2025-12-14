using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class OrderController : Controller
    {

        G6_QLBQAEntities db = new G6_QLBQAEntities();

        // DANH SÁCH ĐƠN HÀNG (HOADON)
        public ActionResult MyOrders()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserID"];

            var orders = db.HoaDon
                .Where(h => h.matk == userId)
                .OrderByDescending(h => h.ngaylap)
                .ToList();

            return View(orders);
        }

        [HttpGet]
        public ActionResult ReviewOrder(int mahd)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserID"];

            var order = db.HoaDon
                          .FirstOrDefault(h => h.mahd == mahd && h.matk == userId);

            if (order == null || order.tinhtrang != "Đã hoàn thành")
                return RedirectToAction("MyOrders");

            ViewBag.MaHD = mahd;
            return View();
        }

        [HttpPost]
        public ActionResult ReviewOrder(int mahd, int sosao, string mota)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserID"];

            // Lấy tất cả sản phẩm trong đơn
            var listSP = db.CTHoaDon
                           .Where(ct => ct.mahd == mahd)
                           .Select(ct => ct.masp)
                           .Distinct()
                           .ToList();

            foreach (var masp in listSP)
            {
                // Chặn đánh giá trùng
                bool exists = db.BinhLuan
                    .Any(b => b.matk == userId && b.masp == masp);

                if (!exists)
                {
                    db.BinhLuan.Add(new BinhLuan
                    {
                        matk = userId,
                        masp = masp,
                        sosao = sosao,
                        mota = mota,
                        ngay = DateTime.Now
                    });
                }
            }

            db.SaveChanges();

            TempData["Success"] = "Đánh giá thành công!";
            return RedirectToAction("MyOrders");
        }


        // HỦY ĐƠN
        [HttpPost]
        public ActionResult CancelOrder(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account");

            var hd = db.HoaDon.Find(id);
            if (hd == null)
                return HttpNotFound();

            if (hd.tinhtrang == "Chờ xác nhận")
            {
                hd.tinhtrang = "Đã hủy";
                db.SaveChanges();
            }

            return RedirectToAction("MyOrders");
        }

        [HttpPost]
        public ActionResult RequestReturn(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account");

            var order = db.HoaDon.Find(id);

            if (order == null)
                return RedirectToAction("MyOrders");

            // Chỉ cho đổi trả khi đã giao hàng
            if (order.tinhtrang == "Đã hoàn thành")
            {
                order.tinhtrang = "Chờ đổi trả";
                db.SaveChanges();

                TempData["Success"] = "Yêu cầu đổi trả đã được gửi!";
            }

            return RedirectToAction("MyOrders");
        }
    }
}