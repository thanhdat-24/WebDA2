using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDA2.Models;

namespace WebDA2.Utilities
{
    public static class InventoryManager
    {
        /// <summary>
        /// Giảm số lượng tồn kho.
        /// </summary>
        /// <param name="db">Đối tượng CSDL</param>
        /// <param name="khoId">ID của kho</param>
        /// <param name="sanPhamId">ID của sản phẩm</param>
        /// <param name="soLuong">Số lượng cần giảm</param>
        /// <returns>True nếu thành công, False nếu không đủ số lượng</returns>
        public static bool DeductStock(CuaHangITEntities db, int khoId, int sanPhamId, int soLuong)
        {
            var khoHang = db.ChiTietKhoHangs
                .SingleOrDefault(x => x.id_kho == khoId && x.id_sanpham == sanPhamId);

            if (khoHang == null || khoHang.SoLuong < soLuong)
            {
                return false; // Không đủ tồn kho hoặc sản phẩm không tồn tại
            }

            khoHang.SoLuong -= soLuong; // Giảm số lượng tồn kho
            return true;
        }
    }
}