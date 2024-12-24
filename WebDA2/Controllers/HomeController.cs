using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDA2.Models;

namespace WebDA2.Controllers
{
    public class HomeController : Controller
    {
        CuaHangITEntities db = new CuaHangITEntities();
        // GET: Home
        public ActionResult TrangChu()
        {
            var loaiSanPhamIds = new[] { 3, 4, 5, 6, 8 }; // Mảng IDLoaiSP

            // Lọc các sản phẩm theo IDLoaiSP và Hot = true (1)
            var SP = db.SanPhams
                       .Where(x => loaiSanPhamIds.Contains(x.id_loaisp.Value) && x.Hot == true) // Dùng Contains trên mảng
                       .ToList();  // Lấy tất cả sản phẩm phù hợp với điều kiện trên
            var loaiSanPhams = db.LoaiSanPhams
                                 .Where(x => loaiSanPhamIds.Contains(x.IDLoaiSP)) // Dùng Contains với mảng
                                 .ToList();  // Lọc và chuyển thành danh sách
            ViewBag.LoaiSanPhams = loaiSanPhams;  // Truyền loại sản phẩm vào View
            return View(SP);  // Trả về danh sách sản phẩm đã lọc
        }
    }
}