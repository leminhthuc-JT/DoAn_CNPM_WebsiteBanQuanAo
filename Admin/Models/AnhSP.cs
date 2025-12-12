using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Admin.Models
{
	public class AnhSP
	{
        [Key, Column(Order = 0)]
        public int MASP { get; set; }
        [Key, Column(Order = 1)]
        public string ANH { get; set; }
        public string TENANH { get; set; }


        public virtual SanPham SANPHAM { get; set; }
    }
}