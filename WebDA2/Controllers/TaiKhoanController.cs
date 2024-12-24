using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using WebDA2.Models;
using System.Net.Mail;
using System.Net;

namespace WebDA2.Controllers
{
    public class TaiKhoanController : Controller
    {
        CuaHangITEntities db = new CuaHangITEntities();

        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(TaiKhoan _user)
        {
            // Kiểm tra và lưu vào db
            if (ModelState.IsValid)
            {
                bool check = db.TaiKhoans.Any(s => s.TenDangNhap == _user.TenDangNhap);
                bool checkEmail = db.TaiKhoans.Any(s => s.Email == _user.Email);
                bool checkSdt = db.TaiKhoans.Any(s => s.SDT == _user.SDT);

                if (!check && !checkEmail && !checkSdt)
                {
                    _user.LoaiTaiKhoan = "KhachHang";
                    _user.TrangThai = true;
                    _user.MatKhau = GetMD5(_user.MatKhau);
                    _user.rowguid = Guid.NewGuid();
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.TaiKhoans.Add(_user);
                    db.SaveChanges();
                    ViewBag.ShowSuccessMessage = true;
                    return RedirectToAction("DangNhap_ThanhCong", "ThongBao");
                }
                else
                {
                    if (check)
                    {
                        ViewBag.error = "* Tên đã tồn tại!";
                    }
                    if (checkEmail)
                    {
                        ViewBag.erroremail = "* Email đã tồn tại!";
                    }
                    if (checkSdt)
                    {
                        ViewBag.errorsdt = "* Số điện thoại đã được đăng ký!";
                    }
                    return View();
                }
            }
            return View();
        }

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
        public ActionResult DangNhap()
        {

            if (Session["idUser"] != null)
            {
                ViewBag.ShowSuccessMessage = true;
                return RedirectToAction("DangNhap_ThanhCong", "ThongBao");
            }
            return View("DangNhap");
        }
        [HttpPost]
        public ActionResult DangNhap(string tendangnhap, string matkhau)
        {
            if (ModelState.IsValid)
            {
                var f_matkhau = GetMD5(matkhau);
                var data = db.TaiKhoans.Where(s => s.TenDangNhap.Equals(tendangnhap) && s.MatKhau.Equals(f_matkhau)).ToList();
                if (data.Count() > 0)
                {
                    Session["NameUser"] = data.FirstOrDefault().TenDangNhap;
                    Session["idUser"] = data.FirstOrDefault().ID_TaiKhoan;
                    ViewBag.ShowSuccessMessage = true;
                    return RedirectToAction("DangNhap_ThanhCong", "ThongBao");
                }
                else
                {
                    ViewBag.Error = "* Tài khoản hoặc mật khẩu không đúng !";
                    return View();
                }
            }
            return View();
        }

        public ActionResult DangXuat()
        {
            Session.Clear();
            return View("DangNhap");
        }
        public ActionResult ThongTinTaiKhoan()
        {
            if (Session["idUser"] == null)
            {
                return View("DangNhap");
            }

            int idUser = (int)Session["idUser"];
            var user = db.TaiKhoans.Find(idUser);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }
        public ActionResult CapNhat(int id)
        {
            TaiKhoan timkiemUser = db.TaiKhoans.Find(id);
            return View(timkiemUser);
        }

        [HttpPost]
        public ActionResult CapNhat(TaiKhoan model, HttpPostedFileBase FileAnh)
        {
            TaiKhoan EditUser = db.TaiKhoans.Find(model.ID_TaiKhoan);

            // Kiểm tra tên đăng nhập trùng lặp
            var checkTenDangNhap = db.TaiKhoans.Any(u => u.TenDangNhap == model.TenDangNhap && u.ID_TaiKhoan != model.ID_TaiKhoan);
            if (checkTenDangNhap)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
                return View(EditUser);
            }

            // Kiểm tra số điện thoại trùng lặp
            var checkSDT = db.TaiKhoans.Any(u => u.SDT == model.SDT && u.ID_TaiKhoan != model.ID_TaiKhoan);
            if (checkSDT)
            {
                ModelState.AddModelError("SDT", "Số điện thoại đã được sử dụng.");
                return View(EditUser);
            }

            EditUser.TenDangNhap = model.TenDangNhap;

            // Kiểm tra nếu mật khẩu đã được thay đổi, sau đó mã hóa mật khẩu
            if (!string.IsNullOrEmpty(model.MatKhau) && EditUser.MatKhau != model.MatKhau)
            {
                EditUser.MatKhau = GetMD5(model.MatKhau); // Mã hóa mật khẩu trước khi lưu
            }

            EditUser.Email = model.Email;
            EditUser.SDT = model.SDT;


            db.SaveChanges();
            return RedirectToAction("ThongTinTaiKhoan", "TaiKhoan");
        }
        public ActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuenMatKhau(string tenDangNhap, string email)
        {
            // Tìm tài khoản dựa trên tên đăng nhập và email
            var user = db.TaiKhoans.FirstOrDefault(u => u.TenDangNhap == tenDangNhap && u.Email == email);

            if (user != null)
            {
                // Tạo mật khẩu mới ngẫu nhiên
                string newPassword = GenerateRandomPassword(8);
                user.MatKhau = GetMD5(newPassword); // Mã hóa mật khẩu mới
                db.SaveChanges();

                // Tạo nội dung email với mật khẩu được định dạng
                string emailBody = $@"
                <p>Chào bạn,</p>
                <p>Mật khẩu mới của bạn là:</p>
                <p><strong style='font-size:20px; color:#000;'>{newPassword}</strong></p>
                <p>Hãy đăng nhập lại và đổi mật khẩu để bảo mật tài khoản.</p>
                <p>Trân trọng,<br/>Hệ thống VicTorTech.VN</p>
                ";

                // Gửi email mật khẩu mới
                SendEmail(email, "Lấy lại mật khẩu", emailBody);

                ViewBag.Message = "Mật khẩu mới đã được gửi vào email của bạn.";
            }
            else
            {
                ViewBag.Error = "Tên đăng nhập hoặc email không khớp với tài khoản nào!";
            }

            return View();
        }

        // Hàm tạo mật khẩu ngẫu nhiên
        private string GenerateRandomPassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];
                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }
            return res.ToString();
        }
        // Hàm gửi email
        private void SendEmail(string toEmail, string subject, string body)
        {
            var fromEmail = new MailAddress("tdvcstore28235@gmail.com", "VicTorTech.VN Xin Trân Trọng Gửi Đến Bạn");
            var toEmailAddress = new MailAddress(toEmail);
            var fromEmailPassword = "gxhkyicmamyulpfe"; // Mật khẩu email gửi

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmailAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true // Cho phép sử dụng HTML trong nội dung email
            })
            {
                smtp.Send(message);
            }

        }
    }
}