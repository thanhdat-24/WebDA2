using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDA2.Models;

namespace WebDA2.Controllers
{
    public class ThongBaoController : Controller
    {
        // GET: ThongBao
        CuaHangITEntities db = new CuaHangITEntities();
        public ActionResult DangNhap_ThanhCong()
        {
            return View();
        }
        public ActionResult DangKy_ThanhCong()
        {
            return View();
        }
        public ActionResult ThanhToan_ThanhCong()
        {
            return View();
        }
    }
}