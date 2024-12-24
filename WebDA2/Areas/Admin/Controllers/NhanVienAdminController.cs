using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDA2.Models;
using System.Data.Entity;



namespace WebDA2.Areas.Admin.Controllers
{
    public class NhanVienAdminController : AdminBaseController
    {
        CuaHangITEntities db = new CuaHangITEntities();
        // GET: Admin/NhanVienAdmin
        
        public ActionResult DanhSachNhanVien(int? chiNhanhId)
        {
            var nhanViens = db.NhanViens.AsQueryable();

            if (chiNhanhId.HasValue)
            {
                nhanViens = nhanViens.Where(nv => nv.id_chinhanh == chiNhanhId);
            }

            ViewBag.ChiNhanhList = db.ChiNhanhs.ToList();
            return View(nhanViens.ToList());
        }
        public ActionResult ThemNhanVien()
        {
            // Tải dữ liệu cho các danh sách thả xuống (ChucVu, ChiNhanh, TaiKhoan)
            ViewBag.ChucVus = db.ChucVus.ToList();
            ViewBag.ChiNhanhs = db.ChiNhanhs.ToList();
            ViewBag.TaiKhoans = db.TaiKhoans.ToList();

            return View();
        }
        // POST: Admin/NhanVien/ThemNhanVien
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemNhanVien(NhanVien model, string id_chucvu, string id_chinhanh, string id_taikhoan)
        {
            // Kiểm tra nếu tên nhân viên trống
            if (string.IsNullOrWhiteSpace(model.TenNhanVien))
            {
                ModelState.AddModelError("TenNhanVien", "Tên nhân viên không được để trống.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Gán các trường khóa ngoại dựa trên giá trị đã chọn
                    model.id_chucvu = string.IsNullOrEmpty(id_chucvu) ? (int?)null : int.Parse(id_chucvu);
                    model.id_chinhanh = string.IsNullOrEmpty(id_chinhanh) ? (int?)null : int.Parse(id_chinhanh);
                    model.id_taikhoan = string.IsNullOrEmpty(id_taikhoan) ? (int?)null : int.Parse(id_taikhoan);
                    // Thêm nhân viên mới vào cơ sở dữ liệu
                    // Tự động tạo giá trị GUID cho rowguid 
                    model.rowguid = Guid.NewGuid();
                    db.NhanViens.Add(model);
                    db.SaveChanges();
                    // Chuyển hướng đến trang xác nhận hoặc danh sách sau khi thêm thành công
                    return RedirectToAction("DanhSachNhanVien");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            // Tải lại dữ liệu nếu xác thực không thành công
            ViewBag.ChucVus = db.ChucVus.ToList();
            ViewBag.ChiNhanhs = db.ChiNhanhs.ToList();
            ViewBag.TaiKhoans = db.TaiKhoans.ToList();

            return View(model);
        }
        // GET: Admin/NhanVien/CapNhatNhanVien
        public ActionResult CapNhatNhanVien(int id)
        {
            NhanVien timkiemUser = db.NhanViens.Find(id);
            return View(timkiemUser);  // Trả về model chứa dữ liệu nhân viên
        }
        [HttpPost]
        public ActionResult CapNhatNhanVien(NhanVien model, string id_chucvu, string id_chinhanh, string id_taikhoan)
        {
            NhanVien EditUser = db.NhanViens.Find(model.ID_NhanVien);
            EditUser.TenNhanVien = model.TenNhanVien;
            EditUser.Email = model.Email;
            EditUser.SDT = model.SDT;
            EditUser.DiaChi = model.DiaChi;
            EditUser.id_chinhanh = model.id_chinhanh;
            EditUser.id_chucvu = model.id_chucvu;
            EditUser.id_taikhoan = model.id_taikhoan;
            
            db.SaveChanges();
            return RedirectToAction("DanhSachNhanVien");
        }
        public ActionResult XoaNhanVien(int id)
        {
            // Tìm đối tượng xóa
            var deleteUser = db.NhanViens.Find(id);
            // Xóa
            if (deleteUser != null)
            {
                db.NhanViens.Remove(deleteUser);
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachNhanVien");
        }
    }
}