using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class CouponMnController : Controller
    {
        // GET: CouponMn

        private G6_QLBQAEntities db = new G6_QLBQAEntities();

        public ActionResult Index()
        {
            var coupons = db.GiamGia.OrderByDescending(x => x.ngaykt).ToList();
            return View(coupons);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GiamGia model)
        {
            if (ModelState.IsValid)
            {
                if (model.ngaybd < DateTime.Today)
                {
                    ModelState.AddModelError("ngaybd", "Ngày bắt đầu không được ở quá khứ.");
                    return View(model);
                }

                if (model.ngaykt < DateTime.Today)
                {
                    ModelState.AddModelError("ngaykt", "Ngày kết thúc không được ở quá khứ.");
                    return View(model);
                }

                if (model.ngaykt <= model.ngaybd)
                {
                    ModelState.AddModelError("ngaykt", "Ngày kết thúc phải lớn hơn ngày bắt đầu.");
                    return View(model);
                }

                db.GiamGia.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var item = db.GiamGia.Find(id);
            if (item == null) return HttpNotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GiamGia model)
        {
            if (ModelState.IsValid)
            {
                if (model.ngaybd < DateTime.Today)
                {
                    ModelState.AddModelError("ngaybd", "Ngày bắt đầu không được nhỏ hơn ngày hiện tại.");
                    return View(model);
                }

                if (model.ngaykt < DateTime.Today)
                {
                    ModelState.AddModelError("ngaykt", "Ngày hết hạn không được nhỏ hơn ngày hiện tại.");
                    return View(model);
                }

                if (model.ngaykt <= model.ngaybd)
                {
                    ModelState.AddModelError("ngaykt", "Ngày kết thúc phải lớn hơn ngày bắt đầu.");
                    return View(model);
                }

                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var item = db.GiamGia.Find(id);
            if (item == null) return HttpNotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var item = db.GiamGia.Find(id);
            if (item != null)
            {
                db.GiamGia.Remove(item);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}