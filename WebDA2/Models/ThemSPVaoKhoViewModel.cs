using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDA2.Models
{
    public class ThemSPVaoKhoViewModel
    {
        public int ID_Kho { get; set; } // ID của kho
        public List<SanPham> SanPhamList { get; set; } // Danh sách sản phẩm
        public int ID_SanPham { get; set; } // Sản phẩm được chọn
        public int SoLuong { get; set; } // Số lượng cần thêm
    }
}