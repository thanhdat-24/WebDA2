using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDA2.Models;

namespace WebDA2.Areas.Admin.Controllers
{
    public class DanhMucHangHoaController : Controller
    {
        CuaHangITEntities db = new CuaHangITEntities();
        // GET: Admin/DanhMucHangHoa
        public ActionResult DanhSachHangSX()
        {
            var danhSachHangSX = db.HangSXes.ToList(); // Lấy danh sách các hãng sản xuất
            return View(danhSachHangSX);
        }
        public ActionResult DanhSachLoaiSanPham()
        {
            var danhSachLoaiSP = db.LoaiSanPhams.ToList(); // Lấy danh sách các loại sản phẩm
            return View(danhSachLoaiSP);
        }
        [HttpGet]
        public ActionResult ThemHangSX()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemHangSX(HangSX model)
        {
            if (ModelState.IsValid) // Kiểm tra tính hợp lệ của dữ liệu
            {
                model.rowguid = Guid.NewGuid();
                db.HangSXes.Add(model); // Thêm hãng sản xuất vào DB
                db.SaveChanges(); // Lưu thay đổi vào DB
                return RedirectToAction("DanhSachHangSX"); // Quay lại danh sách
            }
            return View(model); // Trả lại view với model nếu có lỗi
        }

        [HttpGet]
        public ActionResult ThemLoaiSanPham()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemLoaiSanPham(LoaiSanPham model)
        {
            if (ModelState.IsValid) // Kiểm tra tính hợp lệ của dữ liệu
            {
                model.rowguid = Guid.NewGuid();
                db.LoaiSanPhams.Add(model); // Thêm loại sản phẩm vào DB
                db.SaveChanges(); // Lưu thay đổi vào DB
                return RedirectToAction("DanhSachLoaiSanPham"); // Quay lại danh sách
            }
            return View(model); // Trả lại view với model nếu có lỗi
        }
        [HttpGet]
        public ActionResult CapNhatHangSX(int id)
        {
            var hangSX = db.HangSXes.Find(id); // Tìm hãng sản xuất theo ID
            if (hangSX == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy, trả về lỗi 404
            }
            return View(hangSX); // Trả về view để chỉnh sửa
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatHangSX(HangSX model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm đối tượng Hãng Sản Xuất trong cơ sở dữ liệu theo ID
                    var hangSX = db.HangSXes.Find(model.IDHangSX); // Sử dụng model.IDHangSX
                    if (hangSX != null)
                    {
                        // Cập nhật các trường cần thiết
                        hangSX.TenHangSX = model.TenHangSX;
                        hangSX.Icon = model.Icon;

                        // Lưu thay đổi vào cơ sở dữ liệu
                        db.SaveChanges();
                        return RedirectToAction("DanhSachHangSX"); // Chuyển đến danh sách hãng sản xuất
                    }
                    else
                    {
                        ModelState.AddModelError("", "Hãng sản xuất không tồn tại.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi cập nhật hãng sản xuất: " + ex.Message);
                }
            }
            return View(model); // Nếu có lỗi, trả lại view với model
        }
        [HttpGet]
        public ActionResult CapNhatLoaiSanPham(int id)
        {
            var loaiSP = db.LoaiSanPhams.Find(id); // Tìm loại sản phẩm theo ID
            if (loaiSP == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy, trả về lỗi 404
            }
            return View(loaiSP); // Trả về view để chỉnh sửa
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatLoaiSanPham(LoaiSanPham model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm đối tượng LoaiSanPham trong cơ sở dữ liệu theo ID
                    var loaiSanPham = db.LoaiSanPhams.Find(model.IDLoaiSP);
                    if (loaiSanPham != null)
                    {
                        // Cập nhật các trường cần thiết (Không cập nhật cột GUID)
                        loaiSanPham.TenLoaiSP = model.TenLoaiSP;
                        loaiSanPham.Icon = model.Icon;

                        // Lưu thay đổi vào cơ sở dữ liệu
                        db.SaveChanges();

                        // Sau khi cập nhật, chuyển hướng đến danh sách LoaiSanPham
                        return RedirectToAction("DanhSachLoaiSanPham");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Loại sản phẩm không tồn tại.");
                    }
                }
                catch (Exception ex)
                {
                    // Thêm lỗi vào ModelState nếu có ngoại lệ trong quá trình cập nhật
                    ModelState.AddModelError("", "Lỗi cập nhật loại sản phẩm: " + ex.Message);
                }
            }

            // Nếu không hợp lệ hoặc có lỗi, trả lại view với model
            return View(model);
        }
        // Action Xóa Hãng Sản Xuất
        public ActionResult XoaHangSX(int id)
        {
            // Tìm Hãng Sản Xuất theo ID
            var hangSX = db.HangSXes.Find(id);
            if (hangSX == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy, trả về lỗi 404
            }

            try
            {
                db.HangSXes.Remove(hangSX); // Xóa hãng sản xuất khỏi DB
                db.SaveChanges(); // Lưu thay đổi vào DB
                return RedirectToAction("DanhSachHangSX"); // Quay lại trang danh sách
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, hiển thị thông báo lỗi
                ModelState.AddModelError("", "Lỗi khi xóa hãng sản xuất: " + ex.Message);
                return RedirectToAction("DanhSachHangSX"); // Quay lại trang danh sách
            }
        }
        // Action Xóa Loại Sản Phẩm
        public ActionResult XoaLoaiSanPham(int id)
        {
            // Tìm Loại Sản Phẩm theo ID
            var loaiSanPham = db.LoaiSanPhams.Find(id);
            if (loaiSanPham == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy, trả về lỗi 404
            }

            try
            {
                db.LoaiSanPhams.Remove(loaiSanPham); // Xóa loại sản phẩm khỏi DB
                db.SaveChanges(); // Lưu thay đổi vào DB
                return RedirectToAction("DanhSachLoaiSanPham"); // Quay lại trang danh sách
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, hiển thị thông báo lỗi
                ModelState.AddModelError("", "Lỗi khi xóa loại sản phẩm: " + ex.Message);
                return RedirectToAction("DanhSachLoaiSanPham"); // Quay lại trang danh sách
            }
        }
    }
}