using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDA2.Models;

namespace WebDA2.Areas.Admin.Controllers
{
    public class ThongBaoAdminController : Controller
    {
        // GET: ThongBao
        CuaHangITEntities db = new CuaHangITEntities();
        public ActionResult ThemSP_ThanhCong()
        {
            return View();
        }
        public ActionResult DangKy_ThanhCong()
        {
            return View();
        }
        public ActionResult Errors()
        {
            return View();
        }
    }
}