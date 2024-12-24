using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDA2.Models;

namespace WebDA2.Controllers
{
    public class TraCuuDonController : Controller
    {
        CuaHangITEntities db = new CuaHangITEntities();
        // GET: TraCuuDon
        public ActionResult TraCuuDon()
        {
            return View();
        }
        // Xử lý tra cứu SĐT
        [HttpPost]
        public ActionResult TraCuuDon(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                ViewBag.Message = "Vui lòng nhập số điện thoại!";
                return View();
            }
            // Tìm khách hàng dựa trên SĐT
            var customer = db.KhachHangs.FirstOrDefault(kh => kh.SDT == phoneNumber);

            if (customer == null)
            {
                ViewBag.Message = "Không tìm thấy khách hàng với số điện thoại này!";
                return View();
            }
            // Tìm danh sách đơn hàng của khách hàng
            var orders = db.DonHangs
                .Where(dh => dh.id_khachhang == customer.ID_KhachHang)
                .ToList();
            if (!orders.Any())
            {
                ViewBag.Message = "Khách hàng chưa có đơn hàng nào!";
                return View();
            }
            // Chuyển đến trang danh sách đơn hàng
            return RedirectToAction("DonHang", new { id_kh = customer.ID_KhachHang });
        }
        // Trang danh sách đơn hàng
        public ActionResult DonHang(int id_kh, string search = "")
        {
            var customer = db.KhachHangs.Find(id_kh);
            if (customer == null) return HttpNotFound();

            var orders = db.DonHangs
                .Where(dh => dh.id_khachhang == id_kh);

            // Áp dụng tìm kiếm nếu có từ khóa
            if (!string.IsNullOrEmpty(search))
            {
                orders = orders.Where(dh => dh.TenDonHang.Contains(search));
            }

            var orderList = orders.Select(dh => new DonhangViewModel
            {
                ID_DonHang = dh.ID_DonHang,
                TenDonHang = dh.TenDonHang,
                NgayDat = dh.NgayDat,
                Branch = dh.ChiNhanh.Ten_CN
            }).ToList();

            ViewBag.Customer = customer;
            return View(orderList);
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
            // Tạo ViewModel
            var chiTietDonHangViewModel = new ChiTietDonHangViewModel
            {
                DonHang = donHang,
                KhachHang = khachHang,
                SanPhamDaMua = sanPhamDaMua
            };

            return View(chiTietDonHangViewModel);
        }

    }
}
