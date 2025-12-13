using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class HomeController : Controller
    {
        G6_QLBQAEntities db = new G6_QLBQAEntities();
        // GET: Home
        public ActionResult Index()
        {
            List<SanPham> lstSP = db.SanPham.ToList();
            return View(lstSP);
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(string FullName, string Email, string Phone, string Message)
        {
            //Kiểm tra email
            if (!CheckEmail(Email))
            {
                ViewBag.Error = "Email không đúng định dạng.";
                return View();
            }

            //Kiểm tra số điện thoại
            if (!CheckPhone(Phone))
            {
                ViewBag.Error = "Số điện thoại không đúng định dạng Việt Nam.";
                return View();
            }

            //Lưu thông tin liên hệ
            string path = Server.MapPath("~/Content/Message/contact_messages.txt");

            //Thông tin liên hệ
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("------ LIÊN HỆ MỚI ------");
            sb.AppendLine("Thời gian: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            sb.AppendLine("Họ tên: " + FullName);
            sb.AppendLine("Email: " + Email);
            sb.AppendLine("SĐT: " + Phone);
            sb.AppendLine("Nội dung: " + Message);
            sb.AppendLine();

            //Ghi file (append)
            System.IO.File.AppendAllText(path, sb.ToString(), Encoding.UTF8);

            TempData["Success"] = "Gửi liên hệ thành công!";
            return RedirectToAction("Contact");
        }

        private bool CheckEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            if (email.Contains(" "))
                return false;

            int atIndex = email.IndexOf('@');
            int dotIndex = email.LastIndexOf('.');

            // phải có @, có . sau @
            if (atIndex <= 0 || dotIndex <= atIndex + 1)
                return false;

            // không được kết thúc bằng .
            if (dotIndex == email.Length - 1)
                return false;

            return true;
        }

        private bool CheckPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            if (phone.Length != 10)
                return false;

            if (!phone.All(char.IsDigit))
                return false;

            if (!phone.StartsWith("0"))
                return false;

            string prefix = phone.Substring(0, 2);
            string[] validPrefixes = { "03", "05", "07", "08", "09" };

            return validPrefixes.Contains(prefix);
        }
    }
}