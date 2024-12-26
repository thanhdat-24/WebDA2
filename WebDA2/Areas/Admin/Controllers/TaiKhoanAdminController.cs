using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDA2.Models;
using System.Security.Cryptography;
using System.Text;

namespace WebDA2.Areas.Admin.Controllers
{
    public class TaiKhoanAdminController : AdminBaseController
    {
        CuaHangITEntities db = new CuaHangITEntities();
        // GET: Admin/TaiKhoanAdmin
        public ActionResult DanhSachTaiKhoan(string search = "")
        {
            var tk = db.TaiKhoans.AsQueryable();

            // Nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                tk = tk.Where(t => t.TenDangNhap.Contains(search));
            }
            return View(tk.ToList());
        }
        public ActionResult ThemTaiKhoan()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemTaiKhoan(TaiKhoan _user, string MatKhauLai)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lặp
                bool checkTenDangNhap = db.TaiKhoans.Any(s => s.TenDangNhap == _user.TenDangNhap);
                bool checkEmail = db.TaiKhoans.Any(s => s.Email == _user.Email);
                bool checkSdt = db.TaiKhoans.Any(s => s.SDT == _user.SDT);
                if (checkTenDangNhap || checkEmail || checkSdt)
                {
                    if (checkTenDangNhap)
                    {
                        ViewBag.errorTenDangNhap = "* Tên đăng nhập đã tồn tại!";
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
                // Kiểm tra mật khẩu khớp
                if (_user.MatKhau != MatKhauLai)
                {
                    ViewBag.errorMatKhauLai = "* Mật khẩu nhập lại không khớp!";
                    return View();
                }
                // Mã hóa mật khẩu
                _user.MatKhau = GetMD5(_user.MatKhau);
                _user.rowguid = Guid.NewGuid();
                db.Configuration.ValidateOnSaveEnabled = false;
                db.TaiKhoans.Add(_user);
                db.SaveChanges();
                return RedirectToAction("DanhSachTaiKhoan", "TaiKhoanAdmin");
            }
            return View();
        }
        public static string GetMD5(string str)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(str);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // Chuyển byte sang chuỗi hex
                }
                return sb.ToString();
            }
        }
        [HttpPost]
        public JsonResult CapNhatTrangThai(int id, bool TrangThai)
        {
            try
            {
                var taiKhoan = db.TaiKhoans.Find(id);
                if (taiKhoan == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy tài khoản." });
                }

                taiKhoan.TrangThai = TrangThai;
                db.SaveChanges();

                return Json(new { success = true, message = "Cập nhật thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        public ActionResult CapNhatTaiKhoan(int id)
        {
            // Kiểm tra nếu tài khoản có ID = 1 thì không cho phép cập nhật
            if (id == 1)
            {
                ViewBag.ErrorMessage = "Đây là tài khoản quản trị hệ thống, không thể cập nhật.";
                return View(new TaiKhoan()); // Trả về một đối tượng TaiKhoan rỗng thay vì null
            }
            TaiKhoan timkiemUser = db.TaiKhoans.Find(id);
            if (timkiemUser == null)
            {
                return HttpNotFound();
            }
            return View(timkiemUser);
        }

        [HttpPost]
        public ActionResult CapNhatTaiKhoan(TaiKhoan model)
        {
            // Kiểm tra nếu tài khoản có ID = 1 thì không cho phép cập nhật
            if (model.ID_TaiKhoan == 1)
            {
                ViewBag.ErrorMessage = "Đây là tài khoản quản trị hệ thống, không thể cập nhật.";
                return View(model); // Trả về view với thông báo lỗi
            }

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
            EditUser.LoaiTaiKhoan = model.LoaiTaiKhoan;
            // Kiểm tra nếu TrangThai là null, gán giá trị mặc định là false
            EditUser.TrangThai = model.TrangThai;

            db.SaveChanges();
            return RedirectToAction("DanhSachTaiKhoan", "TaiKhoanAdmin");
        }
        public ActionResult XoaTaiKhoan(int id)
        {
            // Kiểm tra nếu tài khoản có ID = 1 thì không cho phép xóa
            if (id == 1)
            {
                ViewBag.ErrorMessage = "Đây là tài khoản quản trị hệ thống, không thể xóa.";
                return RedirectToAction("DanhSachTaiKhoan"); // Trả về danh sách tài khoản
            }

            // Tìm đối tượng xóa
            var deleteUser = db.TaiKhoans.Find(id);

            // Xóa
            if (deleteUser != null)
            {
                db.TaiKhoans.Remove(deleteUser);
                db.SaveChanges();

                // Thêm thông báo thành công
                TempData["SuccessMessage"] = "Tài khoản đã được xóa thành công.";
            }

            return RedirectToAction("DanhSachTaiKhoan");
        }


    }
}