//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebDA2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MSmerge_conflict_HTQLCuaHangIT_CN2_DonHang
    {
        public int ID_DonHang { get; set; }
        public string TenDonHang { get; set; }
        public Nullable<int> id_khachhang { get; set; }
        public Nullable<int> id_chinhanh { get; set; }
        public string TenKH { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public string SDT { get; set; }
        public Nullable<int> TrangThai { get; set; }
        public Nullable<decimal> TongGia { get; set; }
        public Nullable<int> TypePayment { get; set; }
        public Nullable<System.DateTime> NgayDat { get; set; }
        public System.Guid rowguid { get; set; }
        public Nullable<System.Guid> origin_datasource_id { get; set; }
    }
}
