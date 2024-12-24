using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDA2.Models
{
    public class DonhangViewModel
    {
        public int ID_DonHang { get; set; }
        public string TenDonHang { get; set; }
        public DateTime? NgayDat { get; set; }
        public decimal? TongSL { get; set; }
        public string Branch { get; set; }
        public string EmailDH { get; set; }
        public string DiaChiNhanHang { get; set; }
    }
}