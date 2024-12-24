using System.Linq;
using System.Web.Mvc;
using WebDA2.Models;

namespace WebDA2.Areas.Admin.Controllers
{
    public class DangNhapAdminController : Controller
    {
        CuaHangITEntities db = new CuaHangITEntities();

        // GET: Admin/DangNhapAdmin
        public ActionResult DangNhapAdmin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhapAdmin(string username, string password)
        {
            var hashedPassword = GetMD5(password);  // Mã hóa mật khẩu
            var admin = db.TaiKhoans.FirstOrDefault(u =>
                u.TenDangNhap == username &&
                u.MatKhau == hashedPassword &&
                (u.LoaiTaiKhoan == "Admin" || u.LoaiTaiKhoan == "QuanLy")
            );

            if (admin != null)
            {
                // Kiểm tra trạng thái tài khoản
                if (admin.TrangThai == true)
                {
                    // Lưu thông tin admin vào session
                    Session["AdminUser"] = admin.TenDangNhap;
                    Session["AdminId"] = admin.ID_TaiKhoan;
                    // Chuyển hướng đến trang quản trị chính
                    return RedirectToAction("TrangChuAdmin", "HomeAdmin");
                }
                else
                {
                    ViewBag.Error = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.";
                    return View();
                }
            }
            else
            {
                // Debugging lỗi
                var testUser = db.TaiKhoans.FirstOrDefault(u => u.TenDangNhap == username);
                if (testUser == null)
                {
                    ViewBag.Error = "Tên đăng nhập không tồn tại!";
                }
                else if (testUser.MatKhau != hashedPassword)
                {
                    ViewBag.Error = "Mật khẩu không đúng!";
                }
                else if (testUser.LoaiTaiKhoan != "Admin" && testUser.LoaiTaiKhoan != "QuanLy")
                {
                    ViewBag.Error = "Loại tài khoản không hợp lệ!";
                }
                return View();
            }
        }


        public static string GetMD5(string str)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] data = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));
                return string.Concat(data.Select(b => b.ToString("x2")));
            }
        }
        public ActionResult DangXuatAdmin()
        {
            // Xóa session
            Session.Clear();
            // Chuyển hướng về trang đăng nhập admin
            return RedirectToAction("DangNhapAdmin", "DangNhapAdmin", new { area = "Admin" });
        }
    }
}
