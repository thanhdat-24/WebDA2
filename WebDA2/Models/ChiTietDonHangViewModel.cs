using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDA2.Models
{
    public class ChiTietDonHangViewModel
    {
        public DonhangViewModel DonHang { get; set; } // Thông tin đơn hàng
        public KhachHang KhachHang { get; set; } // Thông tin khách hàng
        public List<SanPhamViewModel> SanPhamDaMua { get; set; } // Danh sách sản phẩm đã mua
        public int TongSoLuong { get; set; }
        public decimal TongGiaTri { get; set; }
    }
}