using System.Web.Mvc;

namespace WebDA2.Areas.Admin.Filters
{
    public class AdminLoginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Kiểm tra session hoặc các điều kiện đăng nhập khác
            if (filterContext.HttpContext.Session["AdminUser"] == null)
            {
                filterContext.Result = new RedirectResult("/Admin/DangNhapAdmin/DangNhapAdmin");
            }
        }
    }
}
