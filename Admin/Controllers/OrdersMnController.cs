using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class OrdersMnController : Controller
    {
        G6_QLBQAEntities db = new G6_QLBQAEntities();

        //DANH SÁCH ĐƠN HÀNG
        public ActionResult Index()
        {
            var orders = db.HoaDon
                .OrderByDescending(x => x.ngaylap)
                .ToList();

            return View(orders);
        }

        //CHUYỂN TRẠNG THÁI
        [HttpPost]
        public ActionResult UpdateStatus(int id, string nextStatus)
        {
            var order = db.HoaDon.Find(id);
            if (order == null)
                return RedirectToAction("Index");

            // KHÓA LOGIC – KHÔNG CHO QUAY NGƯỢC
            if (order.tinhtrang == "Chờ xác nhận")
            {
                if (nextStatus == "Đã xác nhận" || nextStatus == "Đã hủy")
                    order.tinhtrang = nextStatus;
            }
            else if (order.tinhtrang == "Đã xác nhận")
            {
                if (nextStatus == "Chờ giao hàng")
                    order.tinhtrang = nextStatus;
            }
            else if (order.tinhtrang == "Chờ giao hàng")
            {
                if (nextStatus == "Đã hoàn thành")
                    order.tinhtrang = nextStatus;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //HỦY ĐƠN (CHỈ KHI CHỜ XÁC NHẬN)
        [HttpPost]
        public ActionResult Cancel(int id)
        {
            var hd = db.HoaDon.Find(id);
            if (hd == null) return HttpNotFound();

            if (hd.tinhtrang == "Chờ xác nhận")
            {
                hd.tinhtrang = "Đã hủy";
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        //XÁC NHẬN ĐÃ THANH TOÁN
        [HttpPost]
        public ActionResult XacNhanThanhToan(int id)
        {
            var hd = db.HoaDon.Find(id);
            if (hd == null) return HttpNotFound();

            hd.dathanhtoan = true;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}