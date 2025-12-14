using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class ClientMnController : Controller
    {
        // GET: ClientMn
        private G6_QLBQAEntities db = new G6_QLBQAEntities();

        public ActionResult Index()
        {
            var accounts = db.TaiKhoan.OrderBy(t => t.maquyen).ThenBy(t => t.tenkh).ToList();
            return View(accounts);
        }


        public ActionResult Edit(int id)
        {
            var tk = db.TaiKhoan.Find(id);
            if (tk == null) return HttpNotFound();
            return View(tk);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int matk, int maquyen)
        {
            var tk = db.TaiKhoan.Find(matk);
            if (tk != null)
            {
                tk.maquyen = maquyen;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tk);
        }

        public ActionResult Delete(int id)
        {
            var tk = db.TaiKhoan.Find(id);
            if (tk == null) return HttpNotFound();
            return View(tk);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var tk = db.TaiKhoan.Find(id);
            if (tk != null)
            {
                db.TaiKhoan.Remove(tk);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}