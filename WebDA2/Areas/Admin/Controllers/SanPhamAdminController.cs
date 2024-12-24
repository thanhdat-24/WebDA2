using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDA2.Models;
using System.Data.Entity;


namespace WebDA2.Areas.Admin.Controllers
{
    public class SanPhamAdminController : AdminBaseController
    {
        CuaHangITEntities db = new CuaHangITEntities();
        // GET: Admin/SanPhamAdmin
        public ActionResult TatCaSanPham(int? page, int? khoId, string searchQuery, string trangThai)
        {
            IQueryable<SanPhamViewModel> query;
            if (khoId.HasValue && khoId > 0)
            {
                // Lọc sản phẩm theo kho hàng và hiển thị bằng tên ChiNhanh
                query = from sp in db.SanPhams
                        join ct in db.ChiTietKhoHangs on sp.IDSanPham equals ct.id_sanpham
                        where ct.id_kho == khoId.Value
                        select new SanPhamViewModel
                        {
                            SanPham = sp,
                            SoLuong = ct.SoLuong
                        };
            }
            else
            {
                // Lấy tất cả sản phẩm nếu không có lọc
                query = from sp in db.SanPhams
                        where string.IsNullOrEmpty(searchQuery) || sp.TenSanPham.Contains(searchQuery)
                        select new SanPhamViewModel
                        {
                            SanPham = sp,
                            // Cộng dồn số lượng sản phẩm từ tất cả các chi nhánh trong ChiTietKhoHang
                            SoLuong = (from ct in db.ChiTietKhoHangs
                                       where ct.id_sanpham == sp.IDSanPham
                                       select (int?)ct.SoLuong).Sum() ?? 0 // Tổng số lượng, nếu null thì mặc định là 0
                        };
            }
            if (!string.IsNullOrEmpty(trangThai)) {
                //San phẩm tồn kho
                if (trangThai == "InStock")
                {
                    query = query.Where(sp => sp.SoLuong > 0);
                }
                //San phẩm tạm hết
                else if (trangThai == "OutOfStock") {
                    query = query.Where(sp => sp.SoLuong == 0);
                } 
            }
            // Lấy danh sách kho hàng để hiển thị trong dropdown
            ViewBag.KhoHangs = db.KhoHangs.ToList();
            return View(query.ToList()); // Trả về danh sách sản phẩm
        }


        public ActionResult ThemSanPham()
        {
            // Lấy danh sách các loại sản phẩm từ cơ sở dữ liệu
            ViewBag.LoaiSanPhamList = db.LoaiSanPhams.ToList();

            // Lấy danh sách các hãng sản xuất từ cơ sở dữ liệu
            ViewBag.HangSXList = db.HangSXes.ToList();

            // Trả về view để hiển thị form nhập liệu
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemSanPham(SanPham model, HttpPostedFileBase FileAnh, string Cpu, string Main, string Ram, string OCung, string CardDoHoa, string ManHinh, string HeDieuHanh, string Pin)
        {
            // Truyền lại dữ liệu cho dropdown trong trường hợp có lỗi validation
            ViewBag.LoaiSanPhamList = db.LoaiSanPhams.ToList();
            ViewBag.HangSXList = db.HangSXes.ToList();

            // Kiểm tra tên sản phẩm đã tồn tại hay chưa
            var KiemTraTen = db.SanPhams.FirstOrDefault(u => u.TenSanPham == model.TenSanPham);
            if (KiemTraTen != null)
            {
                ModelState.AddModelError("", "Tên Sản Phẩm đã tồn tại. Vui lòng chọn tên khác.");
                return View(model);
            }
            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrEmpty(model.TenSanPham))
            {
                ModelState.AddModelError("", "Tên Sản Phẩm không được trống.");
                return View(model);
            }
            if (model.Gia <= 0)
            {
                ModelState.AddModelError("", "Giá phải lớn hơn 0.");
                return View(model);
            }
            // Lưu file ảnh nếu có upload
            if (FileAnh != null && FileAnh.ContentLength > 0)
            {
                string fileName = Path.GetFileName(FileAnh.FileName);
                string path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                FileAnh.SaveAs(path);
                model.Anh = "/Content/Images/" + fileName;  // Lưu đường dẫn của ảnh vào cơ sở dữ liệu
            }
            // Tự động tạo giá trị GUID cho rowguid 
            model.rowguid = Guid.NewGuid();
            // Lưu sản phẩm vào cơ sở dữ liệu
            db.SanPhams.Add(model);
            db.SaveChanges();

            // Lưu thông tin chi tiết sản phẩm
            ThongTinChiTietSP chiTietSP = new ThongTinChiTietSP
            {
                id_sanpham = model.IDSanPham,
                Main = Main,
                CPU = Cpu,
                RAM = Ram,
                OCUNG = OCung,
                CARD = CardDoHoa,
                MANHINH = ManHinh,
                HEDIEUHANH = HeDieuHanh,
                PIN = Pin,
                rowguid = Guid.NewGuid()
            };

            db.ThongTinChiTietSPs.Add(chiTietSP);
            db.SaveChanges();

            return RedirectToAction("TatCaSanPham");  // Điều hướng đến trang hiển thị tất cả sản phẩm
        }

        public ActionResult CapNhatSanPham(int id)
        {
            var sanPham = db.SanPhams.FirstOrDefault(s => s.IDSanPham == id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }

            // Lấy thông tin chi tiết sản phẩm
            var thongTinChiTiet = db.ThongTinChiTietSPs.FirstOrDefault(c => c.id_sanpham == id);
            ViewBag.ThongTinChiTiet = thongTinChiTiet;  // Truyền vào ViewBag để sử dụng trong partial view

            ViewBag.LoaiSanPhamList = db.LoaiSanPhams.ToList();
            ViewBag.HangSXList = db.HangSXes.ToList();

            return View(sanPham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatSanPham(SanPham model, HttpPostedFileBase FileAnh)
        {
            ViewBag.LoaiSanPhamList = db.LoaiSanPhams.ToList();     
            ViewBag.HangSXList = db.HangSXes.ToList();
            string cpuValue = Request.Form["thongTinChiTiet.CPU"];
            string mainValue = Request.Form["thongTinChiTiet.Main"];
            string ramValue = Request.Form["thongTinChiTiet.RAM"];
            string hdhValue = Request.Form["thongTinChiTiet.HEDIEUHANH"];
            string romValue = Request.Form["thongTinChiTiet.OCUNG"];
            string manhinhValue = Request.Form["thongTinChiTiet.MANHINH"];
            string cardValue = Request.Form["thongTinChiTiet.CARD"];
            string pinValue = Request.Form["thongTinChiTiet.PIN"];
 
            var oldData = db.SanPhams.Find(model.IDSanPham);
            // Kiểm tra tên sản phẩm trùng
            var KiemTraTen = db.SanPhams.FirstOrDefault(u => u.TenSanPham == model.TenSanPham && u.IDSanPham != model.IDSanPham);
            if (KiemTraTen != null)
            {
                ModelState.AddModelError("", "Tên Sản Phẩm đã tồn tại. Vui lòng chọn tên khác.");
            }
            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrEmpty(model.TenSanPham) || model.Gia <= 0)
            {
                ModelState.AddModelError("", "Tên Sản Phẩm và Giá không được để trống và Giá phải lớn hơn 0.");
            }
            if (!ModelState.IsValid)
            {
                // Log tất cả lỗi trong ModelState để kiểm tra
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine("ModelState Error: " + error.ErrorMessage);
                }
                return View(model); // Trả về lại view với dữ liệu không hợp lệ
            }
            // Xử lý file ảnh
            // Xử lý file ảnh nếu có ảnh mới được tải lên
            if (FileAnh != null && FileAnh.ContentLength > 0)
            {
                string fileName = Path.GetFileName(FileAnh.FileName);
                string path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                FileAnh.SaveAs(path);
                model.Anh = "/Content/Images/" + fileName;  // Đường dẫn ảnh mới
            }
            else
            {
                // Giữ lại đường dẫn ảnh cũ nếu không có ảnh mới
                model.Anh = oldData.Anh;
            }

            oldData.TenSanPham = model.TenSanPham;
            oldData.id_loaisp = model.id_loaisp;
            oldData.Size = model.Size; 
            oldData.ThongTinSanPham = model.ThongTinSanPham;
            oldData.MauSac = model.MauSac;
            oldData.id_hangsx = model.id_hangsx;
            oldData.BaoHanh = model.BaoHanh;
            oldData.AnhOnl = model.AnhOnl;
            oldData.Anh = model.Anh;
            oldData.Gia = model.Gia;
            oldData.GiaKhuyenMai = model.GiaKhuyenMai;
            oldData.GiaVon = model.GiaVon;
            oldData.Hot = model.Hot;
            // Cập nhật sản phẩm
            db.SaveChanges();
            // Kiểm tra và cập nhật chi tiết sản phẩm
            var chiTietSP = db.ThongTinChiTietSPs.FirstOrDefault(c => c.id_sanpham == model.IDSanPham);
            if (chiTietSP != null)
            {
                chiTietSP.CPU = cpuValue;
                chiTietSP.Main = mainValue;
                chiTietSP.RAM = ramValue;
                chiTietSP.OCUNG = romValue;
                chiTietSP.CARD = cardValue;
                chiTietSP.MANHINH = manhinhValue;
                chiTietSP.HEDIEUHANH = hdhValue;
                chiTietSP.PIN = pinValue;
                chiTietSP.id_sanpham = model.IDSanPham;
               // db.Entry(chiTietSP).State = EntityState.Modified;
            }
            else
            {
                // Thêm mới nếu chưa có
                ThongTinChiTietSP newChiTietSP = new ThongTinChiTietSP
                {
                    id_sanpham = model.IDSanPham,
                    CPU = cpuValue,
                    Main =mainValue,
                    RAM = ramValue,
                    OCUNG = romValue,
                    CARD = cardValue,
                    MANHINH = manhinhValue,
                    HEDIEUHANH = hdhValue,
                    PIN = pinValue
                };
                db.ThongTinChiTietSPs.Add(newChiTietSP);
            }
            db.SaveChanges();
            // Chuyển hướng về trang danh sách sản phẩm
            return RedirectToAction("TatCaSanPham");
        }

        public ActionResult ChiTietSanPham(int id)
        {
            // Tìm sản phẩm theo ID
            var sanPham = db.SanPhams
            .Include(s => s.HangSX)         // Lấy đầy đủ thông tin từ HangSX
            .Include(s => s.LoaiSanPham)     // Lấy đầy đủ thông tin từ LoaiSanPham
            .FirstOrDefault(s => s.IDSanPham == id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            // Lấy thông tin chi tiết sản phẩm từ bảng ThongTinChiTietSP
            var thongTinChiTiet = db.ThongTinChiTietSPs.FirstOrDefault(c => c.id_sanpham == id);

            // Tạo ViewModel để chuyển thông tin sản phẩm và chi tiết sản phẩm vào View
            // ViewModel này đã được thêm tại Folder (Models)
            var viewModel = new SanPhamViewModel
            {
                SanPham = sanPham,
                ThongTinChiTiet = thongTinChiTiet,
                HangSX = sanPham.HangSX,
                LoaiSanPham = sanPham.LoaiSanPham
            };

            return View(viewModel); // Trả về View hiển thị chi tiết sản phẩm
        }
        public ActionResult XoaSP(int id_sp)
        {
            // Tìm đối tượng cần xóa
            var deleteSP = db.SanPhams.Find(id_sp);
            // Xóa
            if (deleteSP != null)
            {
                db.SanPhams.Remove(deleteSP);
                db.SaveChanges();
            }
            return RedirectToAction("TatCaSanPham");
        }
    }
}