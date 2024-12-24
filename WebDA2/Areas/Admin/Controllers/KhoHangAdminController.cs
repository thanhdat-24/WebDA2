using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDA2.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


namespace WebDA2.Areas.Admin.Controllers
{
    public class KhoHangAdminController : AdminBaseController
    {
        CuaHangITEntities db = new CuaHangITEntities();
        // GET: Admin/KhoHangAdmin
        public ActionResult TatCaKhoHang(int? chiNhanhId)
        {
            var khoHangs = db.KhoHangs.AsQueryable();
            if (chiNhanhId.HasValue)
            {
                khoHangs = khoHangs.Where(kh => kh.id_chinhanh == chiNhanhId);
            }
            ViewBag.ChiNhanhList = db.ChiNhanhs.ToList();
            return View(khoHangs.ToList());
        }
        public ActionResult XoaKhoHang(int id_kho)
        {
            var deleteKH = db.KhoHangs.Find(id_kho);
            //Thực hiện Xóa
            if (deleteKH != null)
            {
                db.KhoHangs.Remove(deleteKH);
                db.SaveChanges();
            }
            return RedirectToAction("TatCaKhoHang");
        }
        public ActionResult ChiTietKho(int id_kho)
        {
            // Lấy thông tin kho hàng theo id_kho
            var khoHang = db.KhoHangs.Include(kh => kh.ChiNhanh).FirstOrDefault(kh => kh.ID_Kho == id_kho);

            if (khoHang == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy kho hàng
            }

            // Lấy danh sách sản phẩm trong kho
            var chiTietKho = from ct in db.ChiTietKhoHangs
                             join sp in db.SanPhams on ct.id_sanpham equals sp.IDSanPham
                             where ct.id_kho == id_kho
                             select new SanPhamViewModel
                             {
                                 SanPham = sp,
                                 SoLuong = ct.SoLuong,
                                 HangSX = sp.HangSX,
                                 LoaiSanPham = sp.LoaiSanPham
                             };

            // Tạo ViewModel để truyền dữ liệu
            var model = new ChiTietKhoViewModel
            {
                KhoHang = khoHang,
                ChiTietSanPham = chiTietKho.ToList()
            };

            return View(model);
        }
        public ActionResult TimKiemSanPham(string tenSanPham)
        {
            var sanPhamList = db.SanPhams
                                .Where(sp => sp.TenSanPham.Contains(tenSanPham))
                                .Select(sp => new
                                {
                                    sp.IDSanPham,
                                    sp.TenSanPham
                                })
                                .ToList();

            return Json(sanPhamList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ThemSPVaoKho(int id_kho)
        {
            // Lấy danh sách sản phẩm
            var sanPhamList = db.SanPhams.ToList();

            // Tạo ViewModel để truyền dữ liệu đến View
            var viewModel = new ThemSPVaoKhoViewModel
            {
                ID_Kho = id_kho,
                SanPhamList = sanPhamList,
                SoLuong = 0 // Giá trị mặc định
            };

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ThemSPVaoKho(int id_kho, int id_sanpham, int soLuong)
        {
            if (soLuong <= 0)
            {
                ModelState.AddModelError("SoLuong", "Số lượng phải lớn hơn 0.");
                return RedirectToAction("ThemSPVaoKho", new { id_kho });
            }

            var chiTietKho = db.ChiTietKhoHangs
                .FirstOrDefault(ct => ct.id_kho == id_kho && ct.id_sanpham == id_sanpham);

            if (chiTietKho != null)
            {
                chiTietKho.SoLuong += soLuong;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Đã cập nhật số lượng sản phẩm vào kho thành công!";
            }
            else
            {
                chiTietKho = new ChiTietKhoHang
                {
                    id_kho = id_kho,
                    id_sanpham = id_sanpham,
                    SoLuong = soLuong,
                    rowguid = Guid.NewGuid()
                };
                db.ChiTietKhoHangs.Add(chiTietKho);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Thêm sản phẩm mới vào kho thành công!";
            }

            return RedirectToAction("ThemSPVaoKho", new { id_kho });
        }





    }
}