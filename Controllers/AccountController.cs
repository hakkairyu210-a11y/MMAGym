using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMA.Data;
using MMA.Models;
using MMA.Models.Entities;

namespace MMA.Controllers
{
    public class AccountController : Controller
    {
        private readonly MmaDbContext _context;

        public AccountController(MmaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("dang-nhap")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            string prefillEmail = TempData["RegisterSuccessEmail"]?.ToString() ?? "";
            var model = new LoginViewModel
            {
                Email = prefillEmail,
                RememberMe = false
            };
            return View(model);
        }

        [HttpPost]
        [Route("dang-nhap")]
        public IActionResult Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var inputStr = (model.Email ?? "").Trim().ToLower();
            var inputPass = (model.Password ?? "").Trim();

            // 1. Explicit Check for Default Admin: username 'admin' and password '123456'
            if ((inputStr == "admin" || inputStr == "admin@mma.vn" || inputStr == "admin@mmasaigon.vn") && inputPass == "123456")
            {
                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetString("UserName", "Quản Trị Viên MMA");
                HttpContext.Session.SetString("UserEmail", "admin@mma.vn");
                HttpContext.Session.SetInt32("UserId", 1);

                TempData["UserRole"] = "Admin";
                TempData["UserName"] = "Quản Trị Viên MMA";
                TempData["LoginSuccess"] = "Đăng nhập hệ thống Admin thành công!";

                HandleRememberMeCookie(model.Email, model.RememberMe);
                return RedirectToAction("Index", "Admin");
            }

            // 2. Check Database strictly for registered users
            try
            {
                var userInDb = _context.NguoiDungs
                    .Include(u => u.VaiTro)
                    .FirstOrDefault(u => (u.Email.ToLower() == inputStr || u.HoTen.ToLower() == inputStr || u.SoDT == inputStr) && u.PasswordHash == inputPass);

                if (userInDb != null)
                {
                    bool isAdmin = userInDb.VaiTro?.TenVaiTro == "Admin" || userInDb.MaVaiTro == 1;
                    string roleStr = isAdmin ? "Admin" : "Member";

                    HttpContext.Session.SetString("UserRole", roleStr);
                    HttpContext.Session.SetString("UserName", userInDb.HoTen);
                    HttpContext.Session.SetString("UserEmail", userInDb.Email);
                    HttpContext.Session.SetInt32("UserId", userInDb.MaND);

                    TempData["UserRole"] = roleStr;
                    TempData["UserName"] = userInDb.HoTen;
                    TempData["LoginSuccess"] = isAdmin ? "Đăng nhập hệ thống Admin thành công!" : "Đăng nhập tài khoản hội viên thành công!";

                    HandleRememberMeCookie(model.Email, model.RememberMe);

                    if (isAdmin)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception) { }

            // Reject invalid or non-existent accounts
            ModelState.AddModelError("", "Tài khoản không tồn tại trong CSDL hoặc Mật khẩu không chính xác. Vui lòng ĐĂNG KÝ tài khoản mới!");
            return View(model);
        }

        private void HandleRememberMeCookie(string? email, bool rememberMe)
        {
            try
            {
                if (rememberMe && !string.IsNullOrEmpty(email))
                {
                    Response.Cookies.Append("MmaRememberEmail", email, new Microsoft.AspNetCore.Http.CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(30),
                        HttpOnly = true,
                        IsEssential = true
                    });
                }
                else
                {
                    Response.Cookies.Delete("MmaRememberEmail");
                }
            }
            catch (Exception) { }
        }

        [HttpGet]
        [Route("dang-ky")]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [Route("dang-ky")]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var emailTrim = (model.Email ?? "").Trim().ToLower();
            var phoneTrim = (model.Phone ?? "").Trim();

            // 1. Check if email exists in DB
            try
            {
                if (_context.NguoiDungs.Any(u => u.Email.ToLower() == emailTrim))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng. Vui lòng ĐĂNG NHẬP hoặc chọn Email khác!");
                    return View(model);
                }

                // 2. Check if Phone exists in DB
                if (_context.NguoiDungs.Any(u => u.SoDT == phoneTrim))
                {
                    ModelState.AddModelError("Phone", "Số điện thoại này đã được đăng ký trong CSDL. Vui lòng ĐĂNG NHẬP hoặc nhập SĐT khác!");
                    return View(model);
                }

                var memberRole = _context.VaiTros.FirstOrDefault(r => r.TenVaiTro == "Học viên");
                var newUser = new NguoiDungEntity
                {
                    HoTen = model.Name.Trim(),
                    Email = emailTrim,
                    SoDT = phoneTrim,
                    PasswordHash = model.Password,
                    MaVaiTro = memberRole?.MaVaiTro ?? 3,
                    NgayTao = DateTime.Now,
                    TrangThai = true
                };

                _context.NguoiDungs.Add(newUser);
                _context.SaveChanges();

                TempData["RegisterSuccessEmail"] = emailTrim;
                TempData["LoginSuccess"] = "Đăng ký tài khoản hội viên thành công! Vui lòng nhập mật khẩu để Đăng Nhập.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Không thể tạo tài khoản vào CSDL: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        [Route("dang-xuat")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData.Remove("UserRole");
            TempData.Remove("UserName");
            TempData["LogoutSuccess"] = "Đã đăng xuất tài khoản.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("the-tap-cua-toi")]
        [Route("tai-khoan")]
        public IActionResult MyPackages()
        {
            var loggedInEmail = HttpContext.Session.GetString("UserEmail");
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");

            if (string.IsNullOrEmpty(loggedInEmail) && !loggedInUserId.HasValue)
            {
                TempData["LoginError"] = "Vui lòng ĐĂNG NHẬP để xem thông tin thẻ tập của bạn!";
                return RedirectToAction("Login", new { returnUrl = "/the-tap-cua-toi" });
            }

            // Fetch current user from DB strictly
            NguoiDungEntity? currentUser = null;
            try
            {
                currentUser = _context.NguoiDungs
                    .Include(u => u.VaiTro)
                    .FirstOrDefault(u => (loggedInUserId.HasValue && u.MaND == loggedInUserId.Value) || (!string.IsNullOrEmpty(loggedInEmail) && u.Email.ToLower() == loggedInEmail.ToLower()));
            }
            catch (Exception) { }

            ViewBag.UserName = currentUser?.HoTen ?? HttpContext.Session.GetString("UserName") ?? "Hội Viên MMA Sài Gòn";
            ViewBag.UserEmail = currentUser?.Email ?? loggedInEmail ?? "Chưa cập nhật";
            ViewBag.UserPhone = !string.IsNullOrEmpty(currentUser?.SoDT) ? currentUser.SoDT : "Chưa cập nhật";
            ViewBag.UserRole = currentUser?.VaiTro?.TenVaiTro ?? "Hội Viên Thẻ VIP";

            List<RegisterViewModel> list = new();
            try
            {
                if (_context.DangKyGois.Any())
                {
                    // Strictly isolate data for logged in user!
                    list = _context.DangKyGois
                        .Include(d => d.NguoiDung)
                        .Include(d => d.GoiTap)
                        .Include(d => d.ChiNhanh)
                        .Where(d => (loggedInUserId.HasValue && d.MaND == loggedInUserId.Value) || (d.NguoiDung != null && !string.IsNullOrEmpty(loggedInEmail) && d.NguoiDung.Email.ToLower() == loggedInEmail.ToLower()))
                        .OrderByDescending(d => d.MaDK)
                        .Select(d => new RegisterViewModel
                        {
                            OrderId = d.MaDK,
                            InvoiceCode = d.MaHD,
                            Name = d.NguoiDung != null ? d.NguoiDung.HoTen : "Hội viên MMA",
                            Email = d.NguoiDung != null ? d.NguoiDung.Email : "",
                            Phone = d.NguoiDung != null ? d.NguoiDung.SoDT : "",
                            BranchName = d.ChiNhanh != null ? d.ChiNhanh.TenCN : "MMA - Dương Quang Đông (Q8)",
                            SelectedTrainer = "Văn Bá Huyên (Senior Coach)",
                            SelectedScheduleSlot = "Thứ 2, 4, 6 - Ca 18:00 PM",
                            SelectedClass = d.GoiTap != null ? d.GoiTap.TenGoi : "Gói tập MMA VIP",
                            Amount = d.TongTien,
                            Status = d.TrangThai,
                            PaymentMethod = d.TrangThai.Contains("Momo") ? "Ví Momo / VietQR Code" : (d.TrangThai.Contains("Visa") ? "Thẻ ATM/Visa" : "Tiền mặt tại quầy"),
                            IsApproved = d.TrangThai.Contains("Đã phê duyệt") || d.TrangThai.Contains("phê duyệt") || d.TrangThai.Contains("Đang hoạt động") || d.TrangThai.Contains("Thanh Toán") || d.TrangThai.Contains("Kích Hoạt")
                        }).ToList();
                }
            }
            catch (Exception) { }

            return View(list);
        }

        [HttpPost]
        [Route("huy-goi-tap")]
        public IActionResult CancelPackage(int orderId)
        {
            var loggedInEmail = HttpContext.Session.GetString("UserEmail");
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");

            if (string.IsNullOrEmpty(loggedInEmail) && !loggedInUserId.HasValue)
            {
                TempData["LoginError"] = "Vui lòng ĐĂNG NHẬP để thực hiện thao tác này!";
                return RedirectToAction("Login", new { returnUrl = "/the-tap-cua-toi" });
            }

            try
            {
                var packageReg = _context.DangKyGois
                    .Include(d => d.NguoiDung)
                    .FirstOrDefault(d => d.MaDK == orderId && 
                        ((loggedInUserId.HasValue && d.MaND == loggedInUserId.Value) || 
                         (d.NguoiDung != null && !string.IsNullOrEmpty(loggedInEmail) && d.NguoiDung.Email.ToLower() == loggedInEmail.ToLower())));

                if (packageReg != null)
                {
                    packageReg.TrangThai = "Đã hủy bởi người dùng";
                    _context.SaveChanges();
                    TempData["UserSuccess"] = $"Hủy đơn đăng ký #{packageReg.MaHD} thành công!";
                }
                else
                {
                    TempData["UserError"] = "Không tìm thấy đơn đăng ký gói tập phù hợp hoặc bạn không có quyền hủy.";
                }
            }
            catch (Exception ex)
            {
                TempData["UserError"] = "Lỗi khi hủy gói tập: " + ex.Message;
            }

            return RedirectToAction("MyPackages");
        }
    }
}
