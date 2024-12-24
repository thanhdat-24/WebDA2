using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebDA2.Models;


namespace WebDA2.Areas.Admin.Controllers
{

    public class HomeAdminController :  AdminBaseController
    {
        CuaHangITEntities db = new CuaHangITEntities();
       public ActionResult TrangChuAdmin()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TongChiNhanh = db.ChiNhanhs.Count(),
                SoThanhVien = db.TaiKhoans.Count(),
                SoKhachHang = db.KhachHangs.Count(),
                SoSanPham = db.SanPhams.Count()
            };

            // Truyền ViewModel sang View
            return View(viewModel);
        }
    }
}