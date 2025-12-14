using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models
{
    public class CartItem
    {
        public int MaSP { get; set; }

        public int MaMau { get; set; }
        public int MaSize { get; set; }

        public string TenSP { get; set; }
        public string Image { get; set; }

        public decimal Gia { get; set; }
        public int SoLuong { get; set; }

        public decimal ThanhTien => Gia * SoLuong;
    }
}