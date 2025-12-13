using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class ProductController : Controller
    {
        G6_QLBQAEntities db = new G6_QLBQAEntities();
        // GET: Product

        public ActionResult Index(string sortOrder, string searchString, int? categoryID, int? page)
        {
            var sanPhams = db.SanPham.AsQueryable();

            if (categoryID != null)
            {
                sanPhams = sanPhams.Where(s => s.madm == categoryID);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                sanPhams = sanPhams.Where(s => s.tensp.Contains(searchString));
            }

            // Lưu trạng thái sắp xếp
            ViewBag.CurrentSort = sortOrder;

            switch (sortOrder)
            {
                case "price_asc":
                    sanPhams = sanPhams.OrderBy(s => s.giaban);
                    break;
                case "price_desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.giaban);
                    break;
                case "newest": 
                    sanPhams = sanPhams.OrderByDescending(s => s.ngaynhap);
                    break;
                case "sales": //lượt bán
                              // tạm thời sắp xếp theo số lượng
                    sanPhams = sanPhams.OrderBy(s => s.soluong);
                    break;
                default:
                    sanPhams = sanPhams.OrderBy(s => s.tensp);
                    break;
            }

            // Phân trang
            int pageSize = 9;
            int pageNumber = (page ?? 1);

            int totalItems = sanPhams.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentCategory = categoryID;
            ViewBag.CurrentSort = sortOrder;

            // Lấy dữ liệu trang hiện tại
            var hienThiSP = sanPhams
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            // Lấy danh sách danh mục để đổ vào Sidebar bên trái
            ViewBag.ListDanhMuc = db.DanhMuc.ToList();

            return View(hienThiSP);
        }

        public ActionResult ChiTietSP(int masp)
        {
            var sp = db.SanPham.FirstOrDefault(s => s.masp == masp);
            //sản phẩm liên quan
            List<SanPham> lienquan = db.SanPham.Where(s => s.madm == sp.madm || s.maloai == sp.maloai || s.math == sp.math && s.masp != masp).ToList();
            ViewBag.LQ = lienquan;
            var thuongHieu = db.ThuongHieu.FirstOrDefault(t => t.math == sp.math);
            ViewBag.TH = thuongHieu.tenth;
            List<CTSanPham> color = db.CTSanPham.Where(r => r.masp == masp).GroupBy(r => r.mam).Select(g => g.FirstOrDefault()).ToList();
            List<CTSanPham> size = db.CTSanPham.Where(r => r.masp == masp).GroupBy(r => r.mas).Select(g => g.FirstOrDefault()).ToList();
            ViewBag.Image = color;
            ViewBag.Size = size;
            return View(sp);
        }
    }
}