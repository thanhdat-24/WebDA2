using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDA2.Models
{
    public class SanPhamViewModel
    {
        public SanPham SanPham { get; set; }
        public int SoLuong { get; set; }

        public ThongTinChiTietSP ThongTinChiTiet { get; set; }
        public HangSX HangSX { get; set; }

        public LoaiSanPham LoaiSanPham { get; set; } 
        public ChiTietKhoHang ChiTietKhoHang { get; set; }
        public KhoHang KhoHang { get; set; }

        public ChiNhanh ChiNhanh { get; set; }
        public List<SanPham> SanPhamTuongTu { get; set; }
    }
}