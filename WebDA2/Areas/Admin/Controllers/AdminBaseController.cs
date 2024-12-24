using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDA2.Models;
using WebDA2.Areas.Admin.Filters;

namespace WebDA2.Areas.Admin.Controllers
{
    // Kế thừa từ Controller để có thể sử dụng các tính năng của controller
    public class AdminBaseController : Controller
    {
        public AdminBaseController()
        {
        }

        // Áp dụng filter đăng nhập cho tất cả action trong controller kế thừa AdminBaseController
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Kiểm tra session nếu không có đăng nhập thì chuyển hướng tới trang đăng nhập
            if (Session["AdminUser"] == null)
            {
                filterContext.Result = new RedirectResult("/Admin/DangNhapAdmin/DangNhapAdmin");
            }
        }
    }
}