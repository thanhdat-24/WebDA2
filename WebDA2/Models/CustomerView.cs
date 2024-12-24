using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebDA2.Models
{
    public class CustomerView
    {

        [Required(ErrorMessage = "Họ tên khách hàng không được để trống")]
        public string CustomerName { set; get; }
        [Required(ErrorMessage = "Địa chỉ khách hàng không được để trống")]
        public string Address { set; get; }
        [Required(ErrorMessage = "Số điện thoại khách hàng không được để trống")]
        public string Phone { set; get; }
        [Required(ErrorMessage = "Email khách hàng không được để trống")]
        public string Email { set; get; }
        public int chinhanh {  set; get; }
        public int TypePayment {  set; get; }
        public int TypePaymentVN { set; get; }
    }
}