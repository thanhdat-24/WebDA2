using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDA2.Models;

namespace WebDA2.Controllers
{
    public class DichVuController : Controller
    {
        CuaHangITEntities db = new CuaHangITEntities();
        // GET: DichVU
        public ActionResult DVSuaChua()
        {
            var chinhanh = db.ChiNhanhs.ToList();
            return View(chinhanh);
        }
    }
}