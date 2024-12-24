using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDA2.Models
{
    public class ChiTietKhoViewModel
    {
        public KhoHang KhoHang { get; set; } // Thông tin kho
        public List<SanPhamViewModel> ChiTietSanPham { get; set; } // Danh sách sản phẩm trong kho
    }
}