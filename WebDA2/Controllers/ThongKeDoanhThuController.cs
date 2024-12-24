using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDA2.Models;

namespace WebDA2.Controllers
{
    public class ThongKeDoanhThuController : Controller
    {
        // GET: ThongKeDoanhThu
        CuaHangITEntities db = new CuaHangITEntities();
        public ActionResult ThongKe()
        {

            return View();
        }
        [HttpGet]
        public ActionResult ThongKeDT(string fromDate, string toDate, int? month, int? year)
        {
            var query = from dh in db.DonHangs
                        join ctdh in db.ChiTietDonHangs
                        on dh.ID_DonHang equals ctdh.id_donhang
                        join ctkh in db.ChiTietKhoHangs
                        on ctdh.id_chitietkho equals ctkh.ID_ChiTietKhoHang
                        join p in db.SanPhams
                        on ctkh.id_sanpham equals p.IDSanPham
                        select new
                        {
                            CreatedDate = dh.NgayDat,
                            Quantity = ctdh.SoLuong,
                            Price = ctdh.Gia,
                            originalPrice = p.GiaVon
                        };

            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate < endDate);
            }
            if (month.HasValue)
            {
                query = query.Where(x => DbFunctions.TruncateTime(x.CreatedDate).Value.Month == month.Value);
            }
            if (year.HasValue)
            {
                query = query.Where(x => DbFunctions.TruncateTime(x.CreatedDate).Value.Year == year.Value);
            }

            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y => y.Quantity * y.originalPrice),
                TotalSell = x.Sum(y => y.Quantity * y.Price)
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy
            });

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

    }
}

