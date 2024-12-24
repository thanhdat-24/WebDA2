using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;
using WebDA2.Models.Payments;
using WebDA2.Models;

namespace WebDA2.Controllers
{
    public class CartController : Controller
    {
        CuaHangITEntities db = new CuaHangITEntities();
        public ActionResult GioHang()
        {
            ShoppingCart cart = (ShoppingCart)Session["cart"];
            if (Session["Cart"] == null)
            {
                ViewBag.Message = "Chưa có sản phẩm trong giỏ hàng.";
                return View();
            }
            // Kiểm tra nếu giỏ hàng trống
            if (cart.Items.Count() == 0)
            {
                ViewBag.Message = "Chưa có sản phẩm trong giỏ hàng.";
            }
            return View(cart.Items);
        }
        public ActionResult Partial_Item_ThanhToan()
        {
            ShoppingCart cart = (ShoppingCart)Session["cart"];
            if (cart != null)
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }
        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                int orderCode = Convert.ToInt32(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var itemOrder = db.DonHangs.FirstOrDefault(x => x.ID_DonHang == orderCode);
                        if (itemOrder != null)
                        {
                            itemOrder.TrangThai = 2;//đã thanh toán
                            db.SaveChanges();
                        }
                        //Thanh toan thanh cong
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";

                    }
                    else
                    {
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                    }
                    ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                }
            }
            return RedirectToAction("ThanhToan_ThanhCong","ThongBao");
        }
        public ActionResult CheckOut()
        {
            ShoppingCart cart = (ShoppingCart)Session["cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(CustomerView order)
        {
            if (ModelState.IsValid)
            {
                var kTraKhachHang = db.KhachHangs.FirstOrDefault(x => x.SDT == order.Phone);
                if (kTraKhachHang == null)
                {
                    return RedirectToAction("DangKy");
                }
                ShoppingCart cart = (ShoppingCart)Session["cart"];
                if (cart != null && cart.Items.Any())
                {
                    var idKhachHang = kTraKhachHang.ID_KhachHang;
                    DonHang DH = new DonHang
                    {
                        TenDonHang = "Đơn hàng #" + DateTime.Now.Ticks,
                        TenKH = order.CustomerName,
                        SDT = order.Phone,
                        Email = order.Email,
                        DiaChi = order.Address,
                        id_chinhanh = order.chinhanh,
                        id_khachhang = idKhachHang,
                        TongGia = cart.Items.Sum(x => x.Price * x.Quantity),
                        TypePayment = order.TypePayment,
                        NgayDat = DateTime.Now,
                        TrangThai = 1, // Trạng thái "Chờ xử lý"
                        rowguid = Guid.NewGuid()
                    };
                    List<string> sanPhamHetHang = new List<string>();
                    foreach (var item in cart.Items)
                    {
                        var chitietkho = db.ChiTietKhoHangs
                                            .Where(ctk => ctk.id_sanpham == item.ProductId
                                                       && ctk.KhoHang.id_chinhanh == order.chinhanh)
                                            .OrderBy(ctk => ctk.SoLuong)
                                            .FirstOrDefault();

                        if (chitietkho == null || chitietkho.SoLuong < item.Quantity)
                        {
                            sanPhamHetHang.Add(item.ProductName);
                        }
                    }
                    if (sanPhamHetHang.Any())
                    {
                        TempData["ErrorMessage"] = "Các sản phẩm sau không đủ hàng: " + string.Join(", ", sanPhamHetHang);
                        return RedirectToAction("CheckOut");
                    }
                    foreach (var item in cart.Items)
                    {
                        var chitietkho = db.ChiTietKhoHangs
                                            .Where(ctk => ctk.id_sanpham == item.ProductId
                                                       && ctk.KhoHang.id_chinhanh == order.chinhanh
                                                       && ctk.SoLuong >= item.Quantity)
                                            .OrderBy(ctk => ctk.SoLuong)
                                            .FirstOrDefault();
                        if (chitietkho != null)
                        {
                            chitietkho.SoLuong -= item.Quantity;

                            DH.ChiTietDonHangs.Add(new ChiTietDonHang
                            {
                                SoLuong = item.Quantity,
                                Gia = item.Price,
                                id_chitietkho = chitietkho.ID_ChiTietKhoHang,
                                rowguid = Guid.NewGuid()
                            });
                        }
                    }
                    // Lưu đơn hàng vào cơ sở dữ liệu
                    db.DonHangs.Add(DH);
                    db.SaveChanges();
                    // Nếu phương thức thanh toán là VNPAY
                    if (order.TypePayment == 2) // 2: Thanh toán VNPAY
                    {
                        var url = UrlPayment(order.TypePaymentVN, DH.ID_DonHang);
                        return Redirect(url);
                    }
                    // Xử lý gửi email cho khách hàng và admin
                    string strSanPham = "";
                    decimal thanhTien = 0;
                    decimal tongTien = 0;
                    foreach (var sp in cart.Items)
                    {
                        var sanpham = db.SanPhams.FirstOrDefault(s => s.IDSanPham == sp.ProductId);
                        if (sanpham != null)
                        {
                            strSanPham += "<tr>";
                            strSanPham += $"<td>{sp.ProductName}</td>";
                            strSanPham += $"<td><img src='{sanpham.AnhOnl}' alt='Product Image' style='width:100px;height:auto;' /></td>";
                            strSanPham += $"<td>{sp.Quantity}</td>";
                            strSanPham += $"<td>{String.Format("{0:#,##0} VND", sp.Price)}</td>";
                            strSanPham += $"<td>{String.Format("{0:#,##0} VND", sp.TotalPrice)}</td>";
                            strSanPham += "</tr>";
                        }
                    }
                    thanhTien = (decimal)DH.TongGia;
                    tongTien = thanhTien;
                    // Gửi email cho khách hàng
                    string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
                    contentCustomer = contentCustomer.Replace("{{MaDon}}", DH.ID_DonHang.ToString());
                    contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                    contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                    contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", DH.TenKH);
                    contentCustomer = contentCustomer.Replace("{{Phone}}", DH.SDT);
                    contentCustomer = contentCustomer.Replace("{{Email}}", DH.Email);
                    contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", DH.DiaChi);
                    contentCustomer = contentCustomer.Replace("{{ThanhTien}}", String.Format("{0:#,##0} VND", thanhTien));
                    contentCustomer = contentCustomer.Replace("{{TongTien}}", String.Format("{0:#,##0} VND", tongTien));
                    WebDA2.Models.SendMail.sendMail("ShopOnline", "Đơn hàng #" + DH.ID_DonHang.ToString(), contentCustomer, DH.Email);
                    // Gửi email cho admin
                    string contentAdmin = contentCustomer; // Tái sử dụng mẫu email
                    WebDA2.Models.SendMail.sendMail("ShopOnline", "Đơn hàng mới #" + DH.ID_DonHang.ToString(), contentAdmin, ConfigurationManager.AppSettings["EmailAdmin"]);
                    // Xóa giỏ hàng
                    cart.clearCart();
                    Session["CartCount"] = 0;
                    TempData["SuccessMessage"] = "Đặt hàng thành công!";
                    return RedirectToAction("GioHang");
                }
            }
            TempData["ErrorMessage"] = "Đặt hàng không thành công!";
            return RedirectToAction("GioHang", "Cart");
        }
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(KhachHang user)
        {
            if (ModelState.IsValid)
            {
                var checksdt = db.KhachHangs.FirstOrDefault(s => s.SDT == user.SDT);
                if (checksdt == null)
                {
                    try
                    {
                        user.rowguid = Guid.NewGuid();
                        db.KhachHangs.Add(user);
                        db.SaveChanges();
                        return RedirectToAction("CheckOut");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "Lỗi khi thêm khách hàng: " + ex.Message;
                    }
                }
                else
                {
                    ViewBag.Message = "Số điện thoại đã tồn tại.";
                }
            }
            else
            {
                ViewBag.Message = "Thông tin không hợp lệ. Vui lòng kiểm tra lại.";
            }
            return View(user);
        }

        public ActionResult Partial_CheckOut()
        {
            return Partial_CheckOut();
        }
        public ActionResult Update(int id, int quantity)
        {
            ShoppingCart cart = (ShoppingCart)Session["cart"];
            if (cart != null)
            {
                cart.updateQuantity(id, quantity);

            }
            return RedirectToAction("GioHang", "Cart");
        }
        public ActionResult AddToCart(int id)
        {
            var checkProduct = db.SanPhams.FirstOrDefault(x => x.IDSanPham == id);
            int quantity = 1;
            ShoppingCart cart = (ShoppingCart)Session["cart"];
            if (checkProduct != null)
            {
                if (cart == null)
                {
                    cart = new ShoppingCart();
                }
                ShoppingCartItem item = new ShoppingCartItem()
                {
                    IDCustomer = id,
                    ProductId = checkProduct.IDSanPham,
                    ProductName = checkProduct.TenSanPham,
                    ProductImg = checkProduct.AnhOnl,

                    Quantity = quantity
                };
                item.Price = (Decimal)checkProduct.GiaVon;
                if (checkProduct.GiaKhuyenMai > 0)
                {
                    item.Price = (decimal)(checkProduct.GiaKhuyenMai);
                }
                item.TotalPrice = (Decimal)(item.Quantity * item.Price);
                cart.AddToCart(item, quantity);
                Session["cart"] = cart;
                Session["CartCount"] = cart.Items.Count; // Số lượng sản phẩm trong giỏ hàng
            }
            Session["CartMessage"] = "Sản phẩm đã được thêm vào giỏ hàng!";
            return RedirectToAction("GioHang", "Cart");
        }
        public ActionResult Delete(int id)
        {
            ShoppingCart cart = (ShoppingCart)Session["cart"];

            if (cart != null)
            {
                var checkProduct = cart.Items.FirstOrDefault(x => x.ProductId == id);
                if (checkProduct != null)
                {
                    cart.Remove(id);
                    Session["CartCount"] = cart.Items.Count;
                }
            }
            return RedirectToAction("GioHang", "Cart");
        }
        public string UrlPayment(int TypePaymentVN, int orderCode)
        {
            var urlPayment = "";
            var order = db.DonHangs.FirstOrDefault(x => x.ID_DonHang == orderCode);
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"];
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
            VnPayLibrary vnpay = new VnPayLibrary();
            long Price = (long)Math.Round(order.TongGia * 100 ?? 0);
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString());
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }
            vnpay.AddRequestData("vnp_CreateDate", order.NgayDat.HasValue ? order.NgayDat.Value.ToString("yyyyMMddHHmmss") : DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.ID_DonHang);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.ID_DonHang.ToString()); 
            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return urlPayment;
        }
    }
}
