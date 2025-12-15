using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models
{
	public class DashboardViewModel
	{
        public int SoLuongKhachHang { get; set; }
        public int SoLuongDonHang { get; set; }
        public decimal TongDoanhThu { get; set; }
        public int SoMaGiamGia { get; set; }

        // Cảnh báo kho hàng
        public int SanPhamSapHetHang { get; set; }
        public List<SanPham> ListSapHetHang { get; set; }

        // Đơn hàng mới nhất
        public List<HoaDon> DonHangMoi { get; set; }
    }
}