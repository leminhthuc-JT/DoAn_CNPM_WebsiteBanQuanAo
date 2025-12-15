using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Admin.Controllers
{
    public class ProductMnController : Controller
    {
        // GET: ProductMn

        private G6_QLBQAEntities db = new G6_QLBQAEntities();

        public ActionResult Index()
        {
            var sanPhams = db.SanPham
                .Include(s => s.DanhMuc)
                .Include(s => s.LoaiSp)
                .Include(s => s.NhaCungCap)
                .Include(s => s.ThuongHieu)
                .OrderByDescending(s => s.masp);
            return View(sanPhams.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.madm = new SelectList(db.DanhMuc, "madm", "tendm");
            ViewBag.maloai = new SelectList(db.LoaiSp, "maloai", "tenloai");
            ViewBag.mancc = new SelectList(db.NhaCungCap, "mancc", "tenncc");
            ViewBag.math = new SelectList(db.ThuongHieu, "math", "tenth");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] // Cho phép nhập HTML ở phần mô tả nếu dùng CKEditor
        public ActionResult Create(SanPham sanpham, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/ImagesSP/"), fileName);

                    if (!Directory.Exists(Server.MapPath("~/Content/ImagesSP/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/ImagesSP/"));
                    }

                    ImageFile.SaveAs(path);
                    sanpham.anhchinh = fileName;
                }
                else
                {
                    sanpham.anhchinh = "no-image.png";
                }

                if (sanpham.ngaynhap == null) sanpham.ngaynhap = DateTime.Now;

                db.SanPham.Add(sanpham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.madm = new SelectList(db.DanhMuc, "madm", "tendm", sanpham.madm);
            ViewBag.maloai = new SelectList(db.LoaiSp, "maloai", "tenloai", sanpham.maloai);
            ViewBag.mancc = new SelectList(db.NhaCungCap, "mancc", "tenncc", sanpham.mancc);
            ViewBag.math = new SelectList(db.ThuongHieu, "math", "tenth", sanpham.math);
            return View(sanpham);
        }

        public ActionResult Edit(int id)
        {
            SanPham sanpham = db.SanPham.Find(id);
            if (sanpham == null) return HttpNotFound();

            ViewBag.madm = new SelectList(db.DanhMuc, "madm", "tendm", sanpham.madm);
            ViewBag.maloai = new SelectList(db.LoaiSp, "maloai", "tenloai", sanpham.maloai);
            ViewBag.mancc = new SelectList(db.NhaCungCap, "mancc", "tenncc", sanpham.mancc);
            ViewBag.math = new SelectList(db.ThuongHieu, "math", "tenth", sanpham.math);
            return View(sanpham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(SanPham sanpham, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Giữ nguyên các thông tin cũ nếu không muốn mất
                // Cách an toàn là load lại từ DB, nhưng ở đây ta dùng Entry Modified
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/ImagesSP/"), fileName);
                    ImageFile.SaveAs(path);
                    sanpham.anhchinh = fileName;
                }
                else
                {
                    // Nếu không chọn ảnh mới, ta cần giữ lại tên ảnh cũ.
                    // Lưu ý: View phải có HiddenFor cho anhchinh, nếu không nó sẽ bị null
                }

                db.Entry(sanpham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.madm = new SelectList(db.DanhMuc, "madm", "tendm", sanpham.madm);
            ViewBag.maloai = new SelectList(db.LoaiSp, "maloai", "tenloai", sanpham.maloai);
            ViewBag.mancc = new SelectList(db.NhaCungCap, "mancc", "tenncc", sanpham.mancc);
            ViewBag.math = new SelectList(db.ThuongHieu, "math", "tenth", sanpham.math);
            return View(sanpham);
        }

        public ActionResult Delete(int id)
        {
            SanPham sanpham = db.SanPham.Find(id);
            if (sanpham == null) return HttpNotFound();
            return View(sanpham);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SanPham sanpham = db.SanPham.Find(id);
            try
            {
                // 1. Xóa ảnh khỏi server (dọn rác)
                if (!string.IsNullOrEmpty(sanpham.anhchinh) && sanpham.anhchinh != "no-image.png")
                {
                    string path = Path.Combine(Server.MapPath("~/Content/ImagesSP/"), sanpham.anhchinh);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                // 2. Xóa dữ liệu trong DB
                db.SanPham.Remove(sanpham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                // Lỗi này xảy ra khi sản phẩm dính khóa ngoại (đang có trong đơn hàng hoặc chi tiết SP)
                ModelState.AddModelError("", "Không thể xóa sản phẩm này vì đã có dữ liệu liên quan (Chi tiết SP hoặc Đơn hàng). Hãy xóa dữ liệu liên quan trước hoặc chọn 'Ẩn' sản phẩm thay vì xóa.");
                return View(sanpham); // Trả về trang Delete kèm thông báo lỗi
            }
            catch (Exception ex)
            {
                // Các lỗi khác
                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                return View(sanpham);
            }
        }
    }
}