using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDA2.Models;
using System.Data.Entity;

namespace WebDA2.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: SanPham
        CuaHangITEntities db = new CuaHangITEntities();
        public ActionResult SanPham(string keyword, string sortOrder, int? id_loaisp, int? id_hangsx)
        {
            var sanPham = db.SanPhams.AsQueryable();
            // Tìm kiếm theo từ khóa (nếu có)
            if (!string.IsNullOrEmpty(keyword))
            {
                sanPham = sanPham.Where(sp => sp.TenSanPham.Contains(keyword));
            }
            // Lọc sản phẩm theo loại sản phẩm
            if (id_loaisp.HasValue)
            {
                sanPham = sanPham.Where(sp => sp.id_loaisp == id_loaisp.Value);
            }
            // Lọc theo hãng sản xuất 
            if (id_hangsx.HasValue)
            {
                sanPham = sanPham.Where(sp => sp.id_hangsx == id_hangsx.Value);
            }
            // Sắp xếp theo giá
            switch (sortOrder)
            {
                case "price_asc":
                    sanPham = sanPham.OrderBy(sp => sp.GiaKhuyenMai);
                    break;
                case "price_desc":
                    sanPham = sanPham.OrderByDescending(sp => sp.GiaKhuyenMai);
                    break;
                default:
                    sanPham = sanPham.OrderBy(sp => sp.TenSanPham);
                    break;
            }
            // Truyền danh sách loại sản phẩm vào ViewBag để hiển thị trong Layout
            return View(sanPham.ToList());
        }


        public ActionResult SanPham_ChiTiet(int id)
        {
            // Tìm sản phẩm theo ID
            var sanPham = db.SanPhams
                .Include(s => s.HangSX)         // Lấy đầy đủ thông tin từ HangSX
                .Include(s => s.LoaiSanPham)     // Lấy đầy đủ thông tin từ LoaiSanPham
                .FirstOrDefault(s => s.IDSanPham == id);
            if (sanPham == null)
            {
                return RedirectToAction("Errors", "ThongBao");
            }
            // Lấy thông tin chi tiết sản phẩm từ bảng ThongTinChiTietSP
            var thongTinChiTiet = db.ThongTinChiTietSPs.FirstOrDefault(c => c.id_sanpham == id);

            // Lấy danh sách sản phẩm tương tự (cùng id_loaisp nhưng khác sản phẩm hiện tại)
            var sanPhamTuongTu = db.SanPhams
                .Where(s => s.id_loaisp == sanPham.id_loaisp && s.IDSanPham != id)
                .Take(8) // Lấy tối đa 5 sản phẩm để tránh quá tải giao diện
                .ToList();
            // Tạo ViewModel để chuyển thông tin vào View
            var viewModel = new SanPhamViewModel
            {
                SanPham = sanPham,
                ThongTinChiTiet = thongTinChiTiet,
                HangSX = sanPham.HangSX,
                LoaiSanPham = sanPham.LoaiSanPham,
                SanPhamTuongTu = sanPhamTuongTu // Thêm danh sách sản phẩm tương tự vào ViewModel
            };
            return View(viewModel); // Trả về View hiển thị chi tiết sản phẩm
        }
    }
}