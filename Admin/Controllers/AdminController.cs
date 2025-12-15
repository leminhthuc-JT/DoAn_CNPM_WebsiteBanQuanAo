using System;
using System.Linq;
using System.Web.Mvc;
using Admin.Models;
using System.Data.Entity;

namespace Admin.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private G6_QLBQAEntities db = new G6_QLBQAEntities();
        // GET: Admin/DashBoard
        public ActionResult DashBoard()
        {
            // 1. Khởi tạo ViewModel
            DashboardViewModel model = new DashboardViewModel();

            // 2. Thống kê số liệu
            // - Đếm khách hàng (Quyền = 2 là khách)
            model.SoLuongKhachHang = db.TaiKhoan.Count(x => x.maquyen == 2);

            // - Đếm tổng đơn hàng (Giả sử bạn có bảng HoaDon)
            model.SoLuongDonHang = db.HoaDon.Count();

            // - Tổng doanh thu (Tính tổng cột TongTien, xử lý null nếu chưa có đơn nào)
            model.TongDoanhThu = db.HoaDon.Sum(x => (decimal?)x.tongtien) ?? 0;

            // - Đếm mã giảm giá đang kích hoạt (Ngày kết thúc >= hôm nay)
            model.SoMaGiamGia = db.GiamGia.Count(x => x.ngaykt >= DateTime.Now);

            // 3. Logic Kho hàng (Cảnh báo sản phẩm sắp hết < 5 cái)
            model.SanPhamSapHetHang = db.SanPham.Count(x => x.soluong < 5 && x.soluong > 0);
            model.ListSapHetHang = db.SanPham
                                     .Where(x => x.soluong < 5 && x.soluong > 0)
                                     .OrderBy(x => x.soluong)
                                     .Take(5) // Lấy 5 cái tiêu biểu
                                     .ToList();

            // 4. Lấy 5 đơn hàng mới nhất
            model.DonHangMoi = db.HoaDon
                                 .OrderByDescending(x => x.ngaylap) // Giả sử bảng HoaDon có cột NgayDat
                                 .Take(5)
                                 .ToList();

            return View(model);
        }
    }
}