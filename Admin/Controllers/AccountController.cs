using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;

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
            var user = db.TaiKhoan
                         .FirstOrDefault(t => t.tenkh == username
                                           && t.matkhau == password);

            if (user == null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
                return View();
            }

            Session["UserID"] = user.matk;
            Session["UserName"] = user.tenkh;
            Session["Role"] = user.maquyen;

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
            //Kiểm tra username trùng
            if (db.TaiKhoan.Any(t => t.tenkh == username))
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
                tenkh = username,
                matkhau = password,
                gioitinh = null,
                ngaysinh = null,
                email = email,
                sdt = phone,
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

        public ActionResult Logout()
        {
            Session.Clear();         
            Session.Abandon();       

            return RedirectToAction("Index", "Home");
        }
    }
}