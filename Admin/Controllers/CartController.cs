using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.Models;

namespace Admin.Controllers
{
    public class CartController : Controller
    {
        private G6_QLBQAEntities db = new G6_QLBQAEntities();

        // GET: Cart
        public ActionResult Index()
        {

            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = GetCart();
            ViewBag.TongTien = cart.Sum(x => x.ThanhTien);
            return View(cart);
        }



        public ActionResult AddToCart(int masp, int quantity = 1)
        {
            var cart = GetCart();

            var sp = db.SanPham.First(x => x.masp == masp);

            string image = db.CTSanPham
                .Where(x => x.masp == masp)
                .Select(x => x.link)
                .FirstOrDefault();

            var item = cart.FirstOrDefault(x => x.MaSP == masp);

            if (item != null)
            {
                item.SoLuong += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    MaSP = masp,
                    TenSP = sp.tensp,
                    Gia = sp.giaban,
                    SoLuong = quantity,
                    Image = image
                });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Update(int MaSP, string action)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MaSP == MaSP);

            if (item != null)
            {
                if (action == "plus") item.SoLuong++;
                else if (action == "minus" && item.SoLuong > 1) item.SoLuong--;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Remove(int MaSP)
        {
            var cart = GetCart();
            cart.RemoveAll(x => x.MaSP == MaSP);
            return RedirectToAction("Index");
        }

        private List<CartItem> GetCart()
        {
            if (Session["Cart"] == null)
                Session["Cart"] = new List<CartItem>();

            return (List<CartItem>)Session["Cart"];
        }
    }
}