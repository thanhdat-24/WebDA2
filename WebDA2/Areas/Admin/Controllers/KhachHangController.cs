using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDA2.Models;

namespace WebDA2.Areas.Admin.Controllers
{
    public class KhachHangController : AdminBaseController
    {
         CuaHangITEntities db = new CuaHangITEntities(); // Đảm bảo bạn đã khai báo ApplicationDbContext

        // GET: Admin/KhachHang
        public ActionResult DanhSachKhachHang(int? chiNhanhId, string searchTerm)
        {
            // Lấy danh sách chi nhánh để hiển thị trên dropdown
            ViewBag.ChiNhanhList = db.ChiNhanhs.Select(cn => new SelectListItem
            {
                Value = cn.ID_CN.ToString(),
                Text = cn.Ten_CN
            }).ToList();

            // Lấy danh sách khách hàng
            var khachHangs = db.KhachHangs.AsQueryable();

            // Lọc theo chi nhánh
            if (chiNhanhId.HasValue)
            {
                khachHangs = khachHangs.Where(kh => kh.id_chinhanh == chiNhanhId.Value);
            }

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrEmpty(searchTerm))
            {
                khachHangs = khachHangs.Where(kh => kh.TenKH.Contains(searchTerm) || kh.SDT.Contains(searchTerm));
            }

            return View(khachHangs.ToList());
        }

        public ActionResult ThemKhachHang()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ThemKhachHang(KhachHang model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng tên khách hàng
                var isNameDuplicate = db.KhachHangs.Any(kh => kh.TenKH == model.TenKH);
                if (isNameDuplicate)
                {
                    ModelState.AddModelError("TenKH", "Tên khách hàng đã tồn tại.");
                    return View(model);
                }

                // Kiểm tra trùng số điện thoại
                var isPhoneDuplicate = db.KhachHangs.Any(kh => kh.SDT == model.SDT);
                if (isPhoneDuplicate)
                {
                    ModelState.AddModelError("SDT", "Số điện thoại đã tồn tại.");
                    return View(model);
                }

                // Nếu không trùng, thêm khách hàng mới
                model.rowguid = Guid.NewGuid();
                db.KhachHangs.Add(model);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Thêm khách hàng thành công!";
                return RedirectToAction("DanhSachKhachHang");
            }

            return View(model);
        }

        public ActionResult CapNhatKhachHang(int id)
        {
            KhachHang timkiemUser = db.KhachHangs.Find(id);
            return View(timkiemUser);
        }

        [HttpPost]
        public ActionResult CapNhatKhachHang(KhachHang model)
        {
            // Lấy khách hàng cần chỉnh sửa từ cơ sở dữ liệu
            KhachHang EditUser = db.KhachHangs.Find(model.ID_KhachHang);

            if (EditUser == null)
            {
                return HttpNotFound("Không tìm thấy khách hàng.");
            }

            // Kiểm tra số điện thoại trùng lặp
            var checkSDT = db.KhachHangs.Any(u => u.SDT == model.SDT && u.ID_KhachHang != model.ID_KhachHang);
            if (checkSDT)
            {
                ModelState.AddModelError("SDT", "Số điện thoại đã được sử dụng.");
                return View(EditUser);
            }

            // Cập nhật thông tin khách hàng
            EditUser.TenKH = model.TenKH;
            EditUser.SDT = model.SDT;

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();

            TempData["SuccessMessage"] = "Cập nhật thông tin khách hàng thành công!";
            return RedirectToAction("DanhSachKhachHang");
        }

        public ActionResult XoaKhachHang(int id)
        {
            var deleteKH = db.KhachHangs.Find(id);
            // Xóa
            if (deleteKH != null)
            {
                db.KhachHangs.Remove(deleteKH);
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachKhachHang");
        }
    }
}