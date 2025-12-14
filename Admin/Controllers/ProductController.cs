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

        public ActionResult Index(string sortOrder, string searchString, int? categoryID, int? brandID, int? typeID, int? sizeID, string priceRange, int? page)
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

            if (brandID != null)
            {
                sanPhams = sanPhams.Where(s => s.math == brandID);
            }

            if (typeID != null)
            {
                sanPhams = sanPhams.Where(s => s.maloai == typeID);
            }

            // 5. Lọc theo Size (Cần join bảng CTSanPham)
            if (sizeID != null)
            {
                // Lấy ra danh sách ID sản phẩm có size này
                var spCoSize = db.CTSanPham.Where(ct => ct.mas == sizeID).Select(ct => ct.masp).Distinct();
                sanPhams = sanPhams.Where(s => spCoSize.Contains(s.masp));
            }

            // 6. Lọc theo Mức giá (Chuỗi dạng "min-max")
            if (!String.IsNullOrEmpty(priceRange))
            {
                if (priceRange == "under100")
                {
                    sanPhams = sanPhams.Where(s => s.giaban < 100000);
                }
                else if (priceRange == "100-300")
                {
                    sanPhams = sanPhams.Where(s => s.giaban >= 100000 && s.giaban <= 300000);
                }
                else if (priceRange == "300-500")
                {
                    sanPhams = sanPhams.Where(s => s.giaban >= 300000 && s.giaban <= 500000);
                }
                else if (priceRange == "above500")
                {
                    sanPhams = sanPhams.Where(s => s.giaban > 500000);
                }
            }

            // --- Sắp xếp ---
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
                case "sales":
                    sanPhams = sanPhams.OrderBy(s => s.soluong);
                    break;
                default:
                    sanPhams = sanPhams.OrderBy(s => s.tensp);
                    break;
            }

            // --- Phân trang ---
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            int totalItems = sanPhams.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // --- Lưu trạng thái lọc để giữ lại khi chuyển trang ---
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;

            ViewBag.CurrentCategory = categoryID;
            ViewBag.CurrentBrand = brandID;
            ViewBag.CurrentType = typeID;
            ViewBag.CurrentSize = sizeID;
            ViewBag.CurrentPrice = priceRange;
            ViewBag.SearchString = searchString;

            var hienThiSP = sanPhams.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // --- Lấy dữ liệu cho Sidebar ---
            ViewBag.ListDanhMuc = db.DanhMuc.ToList();
            ViewBag.ListThuongHieu = db.ThuongHieu.ToList();
            ViewBag.ListLoai = db.LoaiSp.ToList(); 
            ViewBag.ListSize = db.size.ToList(); 

            return View(hienThiSP);
        }

        public ActionResult ChiTietSP(int masp)
        {
            var sp = db.SanPham.FirstOrDefault(s => s.masp == masp);
            if (sp == null) return HttpNotFound();

            // Sản phẩm liên quan (SỬA LOGIC)
            ViewBag.LQ = db.SanPham
                .Where(s =>
                    (s.madm == sp.madm || s.maloai == sp.maloai || s.math == sp.math)
                    && s.masp != masp
                )
                .ToList();

            // Thương hiệu
            ViewBag.TH = db.ThuongHieu
                .Where(t => t.math == sp.math)
                .Select(t => t.tenth)
                .FirstOrDefault();

            // 🔥 CHỈ DÙNG 1 NGUỒN DUY NHẤT
            ViewBag.CTSP = db.CTSanPham
                .Where(x => x.masp == masp)
                .ToList();

            // Đánh giá
            ViewBag.Reviews = db.BinhLuan
                .Where(bl => bl.masp == masp)
                .OrderByDescending(bl => bl.ngay)
                .ToList();

            return View(sp);
        }
    }
}