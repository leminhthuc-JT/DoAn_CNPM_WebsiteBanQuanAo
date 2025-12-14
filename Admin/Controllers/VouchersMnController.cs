using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class VouchersMnController : Controller
    {
        G6_QLBQAEntities db = new G6_QLBQAEntities();

        //DANH SÁCH 
        public ActionResult Index()
        {
            var list = db.GiamGia.OrderByDescending(x => x.ngaybd).ToList();
            return View(list);
        }

        //THÊM 
        public ActionResult Create()
        {
            return View(new GiamGia());
        }

        [HttpPost]
        public ActionResult Create(GiamGia model)
        {
            if (model.mucgiam > 99)
            {
                ModelState.AddModelError("", "Mức giảm không được vượt quá 99%");
            }

            if (model.ngaybd <= DateTime.Today)
            {
                ModelState.AddModelError("", "Ngày bắt đầu không được nhỏ hơn ngày hiện tại");
            }

            if (model.ngaykt <= DateTime.Today)
            {
                ModelState.AddModelError("ngaykt", "Ngày kết thúc phải lớn hơn ngày hiện tại");
            }

            if (model.ngaykt <= model.ngaybd)
            {
                ModelState.AddModelError("ngaykt", "Ngày kết thúc phải sau ngày bắt đầu");
            }

            if (ModelState.IsValid)
            {
                db.GiamGia.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        //SỬA
        public ActionResult Edit(int id)
        {
            var gg = db.GiamGia.Find(id);
            if (gg == null) return HttpNotFound();
            return View(gg);
        }

        [HttpPost]
        public ActionResult Edit(GiamGia model)
        {
            if (model.mucgiam > 99)
            {
                ModelState.AddModelError("", "Mức giảm không được vượt quá 99%");
            }

            if (model.ngaybd < DateTime.Today)
            {
                ModelState.AddModelError("", "Ngày bắt đầu không được nhỏ hơn ngày hiện tại");
            }

            if (model.ngaykt <= DateTime.Today)
            {
                ModelState.AddModelError("ngaykt", "Ngày kết thúc phải lớn hơn ngày hiện tại");
            }

            if (model.ngaykt <= model.ngaybd)
            {
                ModelState.AddModelError("ngaykt", "Ngày kết thúc phải sau ngày bắt đầu");
            }

            if (ModelState.IsValid)
            {
                var gg = db.GiamGia.Find(model.magg);
                gg.mucgiam = model.mucgiam;
                gg.ngaybd = model.ngaybd;
                gg.ngaykt = model.ngaykt;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        //XÓA
        public ActionResult Delete(int id)
        {
            var gg = db.GiamGia.Find(id);
            if (gg != null)
            {
                db.GiamGia.Remove(gg);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}