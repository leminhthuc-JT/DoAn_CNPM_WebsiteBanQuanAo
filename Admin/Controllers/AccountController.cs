using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Admin.Controllers
{
    public class AccountController : Controller
    {
        G6_QLBQAEntities db = new G6_QLBQAEntities();

        public void SendOTPEmail(string toEmail, string code)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("apitest114@gmail.com");
            mail.To.Add(toEmail);
            mail.Subject = "Mã xác thực đăng ký (OTP)";
            mail.Body = $"Mã OTP của bạn là: {code}";
            mail.IsBodyHtml = false;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("apitest114@gmail.com", "gdzl ixle ieco kqwa");

            smtp.Send(mail);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            username = username?.Trim();
            password = password?.Trim();

            var user = db.TaiKhoan
                 .FirstOrDefault(t =>
                     t.tenkh.Trim() == username &&
                     t.matkhau.Trim() == password
                 );

            if (user == null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
                return View();
            }

            Session["UserID"] = user.matk;
            Session["UserName"] = user.tenkh;
            Session["Role"] = user.maquyen;

            Session["Avatar"] = string.IsNullOrEmpty(user.anhdaidien)? "macdinh.png": user.anhdaidien;

            //Nếu là admin
            if (user.maquyen == 1)
            {
                return RedirectToAction("Index", "Home");
            }

            //Nếu là customer
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string username, string password, string fullname, string email, string phone)
        {
            //Kiểm tra email
            if (!CheckEmail(email))
            {
                ViewBag.Error = "Email không hợp lệ!";
                return View();
            }    

            //Kiểm tra số điện thoại
            if (!CheckPhone(phone))
            {
                ViewBag.Error = "Số điện thoại không hợp lệ!";
                return View();
            }

            //Kiểm tra username trùng
            if (db.TaiKhoan.Any(t => t.tenkh.Trim() == username.Trim()))
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View();
            }

            //Kiểm tra email trùng
            if (db.TaiKhoan.Any(t => t.email == email))
            {
                ViewBag.Error = "Email đã được sử dụng!";
                return View();
            }

            //Kiểm tra số điện thoại trùng
            if (db.TaiKhoan.Any(t => t.sdt == phone))
            {
                ViewBag.Error = "Số điện thoại đã được sử dụng!";
                return View();
            }

            //Tạo mã OTP
            Random rd = new Random();
            string otp = rd.Next(100000, 999999).ToString();

            //Lưu dữ liệu tạm vào session
            Session["PendingUsername"] = username;
            Session["PendingPassword"] = password;
            Session["PendingFullName"] = fullname;
            Session["PendingEmail"] = email;
            Session["PendingPhone"] = phone;
            Session["OTP"] = otp;

            //Gửi OTP qua email
            SendOTPEmail(email, otp);

            //Chuyển đến trang xác minh OTP
            return RedirectToAction("VerifyOTP","Account");
        }

        [HttpGet]
        public ActionResult VerifyOTP()
        {
            return View();
        }

        [HttpPost]
        public ActionResult VerifyOTP(string otp)
        {
            string realOTP = Session["OTP"]?.ToString();

            if (otp != realOTP)
            {
                ViewBag.Error = "Mã OTP không chính xác!";
                return View();
            }

            //Lấy dữ liệu đăng ký từ session
            string username = Session["PendingUsername"].ToString();
            string password = Session["PendingPassword"].ToString();
            string fullname = Session["PendingFullName"].ToString();
            string email = Session["PendingEmail"].ToString();
            string phone = Session["PendingPhone"].ToString();

            //Tạo tài khoản
            TaiKhoan tk = new TaiKhoan()
            {
                tenkh = username.Trim(),
                matkhau = password.Trim(),
                gioitinh = null,
                ngaysinh = null,
                email = email.Trim(),
                sdt = phone.Trim(),
                diachi = "",
                anhdaidien = "macdinh.png",
                maquyen = 2
            };

            db.TaiKhoan.Add(tk);
            db.SaveChanges();

            //Xoá session tạm
            Session.Remove("PendingUsername");
            Session.Remove("PendingPassword");
            Session.Remove("PendingFullName");
            Session.Remove("PendingEmail");
            Session.Remove("PendingPhone");
            Session.Remove("OTP");

            TempData["Success"] = "Đăng ký thành công! Mời đăng nhập.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult ResendOTP()
        {
            string email = Session["PendingEmail"]?.ToString();

            if (email == null)
            {
                TempData["Error"] = "Không tìm thấy email để gửi lại OTP.";
                return RedirectToAction("VerifyOTP");
            }

            //Tạo OTP mới
            Random rd = new Random();
            string otp = rd.Next(100000, 999999).ToString();

            Session["OTP"] = otp;

            //Gửi email
            SendOTPEmail(email, otp);

            TempData["Success"] = "Mã OTP mới đã được gửi!";
            return RedirectToAction("VerifyOTP");
        }

        [HttpGet]
        public ActionResult Profile()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login");

            int id = (int)Session["UserID"];
            var user = db.TaiKhoan.Find(id);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(TaiKhoan model,HttpPostedFileBase AvatarFile)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login");

            var user = db.TaiKhoan.Find(model.matk);

            if (db.TaiKhoan.Any(t => t.sdt == model.sdt && t.matk != model.matk))
            {
                ViewBag.Error = "Số điện thoại đã được sử dụng!";
                return View("Profile", user);
            }

            if (AvatarFile != null && AvatarFile.ContentLength > 2 * 1024 * 1024)
            {
                ViewBag.Error = "Ảnh không được vượt quá 2MB!";
                return View("Profile", user);
            }

            if (AvatarFile != null && AvatarFile.ContentLength > 0)
            {
                string ext = Path.GetExtension(AvatarFile.FileName).ToLower();

                string[] allowExt = { ".jpg", ".jpeg", ".png" };

                if (!allowExt.Contains(ext))
                {
                    ViewBag.Error = "Chỉ cho phép ảnh JPG, PNG!";
                    return View("Profile", user);
                }

                string fileName = "avatar_" + user.matk + ext;
                string path = Server.MapPath("~/Content/Avatar/" + fileName);

                AvatarFile.SaveAs(path);

                user.anhdaidien = fileName;

                Session["Avatar"] = fileName;
            }

            user.gioitinh = model.gioitinh;
            user.ngaysinh = model.ngaysinh;
            user.sdt = model.sdt;
            user.diachi = model.diachi;

            db.SaveChanges();

            ViewBag.Message = "Cập nhật thông tin thành công!";
            return View("Profile", user);
        }

        [HttpPost]
        public ActionResult RequestChangeEmail(string newEmail)
        {
            if (!CheckEmail(newEmail))
            {
                TempData["ErrorEmail"] = "Email không hợp lệ!";
                return RedirectToAction("Profile");
            }

            int userId = (int)Session["UserID"];

            var user = db.TaiKhoan.Find(userId);

            if (user.email.Trim().Equals(newEmail.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorEmail"] = "Email mới phải khác email hiện tại!";
                return RedirectToAction("Profile");
            }

            bool emailExists = db.TaiKhoan
                .Any(t => t.email == newEmail && t.matk != userId);

            if (emailExists)
            {
                TempData["ErrorEmail"] = "Email đã được sử dụng!";
                return RedirectToAction("Profile");
            }

            string otp = new Random().Next(100000, 999999).ToString();

            Session["NewEmail"] = newEmail;
            Session["EmailOTP"] = otp;

            SendOTPEmail(newEmail, otp);

            TempData["ShowOTP"] = true;
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public ActionResult ConfirmChangeEmail(string otp)
        {
            if (Session["EmailOTP"] == null || otp != Session["EmailOTP"].ToString())
            {
                TempData["ErrorEmail"] = "OTP không đúng!";
                TempData["ShowOTP"] = true;
                return RedirectToAction("Profile");
            }

            int id = (int)Session["UserID"];
            var user = db.TaiKhoan.Find(id);

            user.email = Session["NewEmail"].ToString();
            db.SaveChanges();

            Session.Remove("NewEmail");
            Session.Remove("EmailOTP");

            TempData["Message"] = "Đổi email thành công!";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public ActionResult ChangePassword(string currentPass, string newPass, string confirmPass)
        {
            int id = (int)Session["UserID"];
            var user = db.TaiKhoan.Find(id);

            if (user.matkhau.Trim() != currentPass.Trim())
            {
                TempData["ErrorPass"] = "Mật khẩu hiện tại không đúng!";
                return RedirectToAction("Profile");
            }

            if (newPass != confirmPass)
            {
                TempData["ErrorPass"] = "Mật khẩu xác nhận không khớp!";
                return RedirectToAction("Profile");
            }

            user.matkhau = newPass.Trim();
            db.SaveChanges();

            TempData["MessagePass"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Profile");
        }

        public ActionResult Logout()
        {
            Session.Clear();         
            Session.Abandon();       

            return RedirectToAction("Index", "Home");
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