using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDA2.Models;

namespace WebDA2.Areas.Admin.Controllers
{
    public class QuanTriChiNhanhController : AdminBaseController
    {
        CuaHangITEntities db = new CuaHangITEntities();
        public ActionResult DanhSachChiNhanh(string searchString)
        {
            var danhSach = db.ChiNhanhs.ToList();

            // Tìm kiếm chi nhánh theo tên
            if (!String.IsNullOrEmpty(searchString))
            {
                danhSach = danhSach
                            .Where(cn => cn.Ten_CN.ToLower().Contains(searchString.ToLower()))
                            .ToList();
            }

            ViewBag.SearchString = searchString; // Để giữ giá trị ô tìm kiếm khi submit
            return View(danhSach);
        }


        public ActionResult ThemChiNhanh()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemChiNhanh(ChiNhanh model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.rowguid = Guid.NewGuid();
                    db.ChiNhanhs.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("DanhSachChiNhanh");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi thêm chi nhánh: " + ex.Message);
                }
            }
            return View(model);
        }

        public ActionResult CapNhatChiNhanh(int id)
        {
            var chiNhanh = db.ChiNhanhs.Find(id);
            if (chiNhanh == null)
            {
                return HttpNotFound();
            }
            return View(chiNhanh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatChiNhanh(ChiNhanh model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var chiNhanh = db.ChiNhanhs.Find(model.ID_CN);
                    if (chiNhanh != null)
                    {
                        chiNhanh.Ten_CN = model.Ten_CN;
                        chiNhanh.DiaChi = model.DiaChi;
                        chiNhanh.SDT = model.SDT;
                        db.SaveChanges();
                        return RedirectToAction("DanhSachChiNhanh");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi cập nhật chi nhánh: " + ex.Message);
                }
            }
            return View(model);
        }

        public ActionResult ChiTietChiNhanh(int id)
        {
            // Lấy thông tin chi nhánh theo ID
            var chiNhanh = db.ChiNhanhs.Find(id);
            if (chiNhanh == null)
            {
                return HttpNotFound();
            }
            return View(chiNhanh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaChiNhanh(int id)
        {
            try
            {
                var chiNhanh = db.ChiNhanhs.Find(id);
                if (chiNhanh != null)
                {
                    db.ChiNhanhs.Remove(chiNhanh);
                    db.SaveChanges();
                }
                return RedirectToAction("DanhSachChiNhanh");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi xóa chi nhánh: " + ex.Message);
                return RedirectToAction("DanhSachChiNhanh");
            }
        }
    }
}
