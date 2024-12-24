using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDA2.Models;

namespace WebDA2.Areas.Admin.Controllers
{
    public class HoaDonAdminController : AdminBaseController
    {
        CuaHangITEntities db = new CuaHangITEntities();
        // GET: Admin/HoaDonAdmin
        public ActionResult DanhSachDonHang(string search, int? chiNhanhId, string sortOrder)
        {
            // Lấy danh sách hóa đơn từ database
            var dsHD = db.DonHangs.AsQueryable();

            // Lọc theo chi nhánh nếu có
            if (chiNhanhId.HasValue && chiNhanhId != 0)
            {
                dsHD = dsHD.Where(hd => hd.id_chinhanh == chiNhanhId.Value);
            }

            // Tìm kiếm theo SDT nếu có
            if (!string.IsNullOrEmpty(search))
            {
                dsHD = dsHD.Where(hd => hd.SDT.Contains(search));
            }

            // Sắp xếp theo ngày đặt (mặc định là tăng dần)
            ViewBag.CurrentSort = sortOrder;
            switch (sortOrder)
            {
                case "ngay_desc":
                    dsHD = dsHD.OrderByDescending(hd => hd.NgayDat);
                    break;
                default:
                    dsHD = dsHD.OrderBy(hd => hd.NgayDat);
                    break;
            }

            // Truyền danh sách chi nhánh vào ViewBag để hiển thị dropdown
            ViewBag.ChiNhanhList = db.ChiNhanhs.ToList();

            return View(dsHD.ToList());
        }

        public ActionResult ChiTietDonHang(int id_donhang)
        {
            // Tìm đơn hàng
            var donHang = db.DonHangs
                .Where(dh => dh.ID_DonHang == id_donhang)
                .Select(dh => new DonhangViewModel
                {
                    ID_DonHang = dh.ID_DonHang,
                    TenDonHang = dh.TenDonHang,
                    NgayDat = dh.NgayDat,
                    Branch = dh.ChiNhanh.Ten_CN,
                    EmailDH = dh.Email,
                    DiaChiNhanHang = dh.DiaChi

                })
                .FirstOrDefault();
            if (donHang == null) return HttpNotFound();
            // Lấy thông tin khách hàng
            var khachHang = db.KhachHangs.FirstOrDefault(kh => kh.ID_KhachHang == db.DonHangs.FirstOrDefault(dh => dh.ID_DonHang == id_donhang).id_khachhang);
            // Lấy danh sách sản phẩm đã mua trong đơn hàng
            var sanPhamDaMua = db.ChiTietDonHangs
                .Where(ct => ct.id_donhang == id_donhang)
                .Select(ct => new SanPhamViewModel
                {
                    SanPham = db.SanPhams.FirstOrDefault(sp => sp.IDSanPham == (ct.ChiTietKhoHang.id_sanpham ?? 0)), // Xử lý null
                    SoLuong = ct.SoLuong ?? 0, // Nếu SoLuong cũng nullable, xử lý tương tự
                    ChiTietKhoHang = ct.ChiTietKhoHang,
                    KhoHang = ct.ChiTietKhoHang.KhoHang,
                    ChiNhanh = ct.ChiTietKhoHang.KhoHang.ChiNhanh
                }).ToList();

            // Tính tổng số lượng và tổng giá
            int tongSoLuong = sanPhamDaMua.Sum(sp => sp.SoLuong);
            decimal tongGiaTri = sanPhamDaMua.Sum(sp => sp.SoLuong * ((sp.SanPham.GiaKhuyenMai ?? 0) > 0 ? (sp.SanPham.GiaKhuyenMai ?? 0) : (sp.SanPham.Gia ?? 0)));
            // Tạo ViewModel
            var chiTietDonHangViewModel = new ChiTietDonHangViewModel
            {
                DonHang = donHang,
                KhachHang = khachHang,
                SanPhamDaMua = sanPhamDaMua,
                TongSoLuong = tongSoLuong,
                TongGiaTri = tongGiaTri
            };
            return View(chiTietDonHangViewModel);
        }
        [HttpGet]
        public ActionResult CapNhatChiTietHoaDon(int id)
        {
            // Lấy thông tin đơn hàng
            var donHang = db.DonHangs
                .Where(dh => dh.ID_DonHang == id)
                .Select(dh => new DonhangViewModel
                {
                    ID_DonHang = dh.ID_DonHang,
                    TenDonHang = dh.TenDonHang,
                    NgayDat = dh.NgayDat,
                    Branch = dh.ChiNhanh.Ten_CN
                })
                .FirstOrDefault();

            if (donHang == null) return HttpNotFound();

            // Lấy danh sách sản phẩm hiện tại trong đơn hàng
            var sanPhamDaMua = db.ChiTietDonHangs
    .Where(ct => ct.id_donhang == id)
    .Select(ct => new
    {
        SanPham = ct.ChiTietKhoHang != null ? db.SanPhams.FirstOrDefault(sp => sp.IDSanPham == ct.ChiTietKhoHang.id_sanpham) : null,
        SoLuong = ct.SoLuong,
        ChiTietKhoHang = ct.ChiTietKhoHang,
        KhoHang = ct.ChiTietKhoHang != null ? ct.ChiTietKhoHang.KhoHang : null,
        ChiNhanh = ct.ChiTietKhoHang != null && ct.ChiTietKhoHang.KhoHang != null
            ? ct.ChiTietKhoHang.KhoHang.ChiNhanh
            : null
    })
    .AsEnumerable() // Chuyển sang xử lý tại bộ nhớ sau khi lấy dữ liệu từ DB
    .Select(ct => new SanPhamViewModel
    {
        SanPham = ct.SanPham,
        SoLuong = ct.SoLuong ?? 0,
        ChiTietKhoHang = ct.ChiTietKhoHang,
        KhoHang = ct.KhoHang,
        ChiNhanh = ct.ChiNhanh
    })
    .ToList();
            // Tạo ViewModel để hiển thị
            var chiTietDonHangViewModel = new ChiTietDonHangViewModel
            {
                DonHang = donHang,
                SanPhamDaMua = sanPhamDaMua
            };

            return View(chiTietDonHangViewModel);
        }
        [HttpPost]
        public ActionResult CapNhatChiTietHoaDon(int id, List<SanPhamViewModel> sanPhamMoi)
        {
            var donHang = db.DonHangs.FirstOrDefault(dh => dh.ID_DonHang == id);
            if (donHang == null) return HttpNotFound();

            // Duyệt qua danh sách sản phẩm mới để cập nhật
            foreach (var sp in sanPhamMoi)
            {
                if (sp?.SanPham == null || sp.SanPham.IDSanPham <= 0) continue;

                var chiTiet = db.ChiTietDonHangs
                    .FirstOrDefault(ct => ct.id_donhang == id &&
                                          ct.ChiTietKhoHang != null &&
                                          ct.ChiTietKhoHang.id_sanpham == sp.SanPham.IDSanPham);

                if (chiTiet != null)
                {
                    // Nếu sản phẩm đã tồn tại, cập nhật số lượng
                    chiTiet.SoLuong = sp.SoLuong;
                }
                else
                {
                    // Nếu sản phẩm chưa tồn tại, thêm mới
                    var chiTietKho = db.ChiTietKhoHangs
                        .FirstOrDefault(kh => kh.id_sanpham == sp.SanPham.IDSanPham);

                    if (chiTietKho != null)
                    {
                        db.ChiTietDonHangs.Add(new ChiTietDonHang
                        {
                            id_donhang = id,
                            id_chitietkho = chiTietKho.ID_ChiTietKhoHang,
                            SoLuong = sp.SoLuong,
                            Gia = sp.SanPham.Gia
                        });
                    }
                }
            }

            db.SaveChanges();
            return RedirectToAction("ChiTietDonHang", new { id_donhang = id });
        }

        public ActionResult XoaHoaDon()
        {
            return View();
        }

    }
}