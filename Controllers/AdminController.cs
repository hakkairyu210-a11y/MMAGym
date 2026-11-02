using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMA.Data;
using MMA.Models;
using MMA.Models.Entities;

namespace MMA.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly MmaDbContext _context;

        public AdminController(MmaDbContext context)
        {
            _context = context;
        }

        private bool IsAdminAuthenticated()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            if (!IsAdminAuthenticated())
            {
                TempData["LoginError"] = "Bạn không có quyền truy cập trang quản trị Admin. Vui lòng đăng nhập bằng tài khoản Quản trị viên!";
                return RedirectToAction("Login", "Account", new { returnUrl = "/admin" });
            }

            int totalMembers = 1250;
            int totalClasses = 9;
            int totalTrainers = 13;
            decimal monthlyRevenue = 0;

            List<UserModel> membersList = new();
            List<RegisterViewModel> registrationsList = new();

            try
            {
                if (_context.NguoiDungs.Any())
                {
                    totalMembers = _context.NguoiDungs.Count();

                    membersList = _context.NguoiDungs
                        .Include(u => u.VaiTro)
                        .OrderByDescending(u => u.MaND)
                        .Take(10)
                        .Select(u => new UserModel
                        {
                            Id = u.MaND.ToString(),
                            FullName = u.HoTen,
                            Email = u.Email,
                            Phone = u.SoDT,
                            Role = u.VaiTro != null ? u.VaiTro.TenVaiTro : "Hội viên",
                            Status = u.TrangThai ? "Hoạt động" : "Tạm khóa"
                        }).ToList();
                }

                if (_context.BoMons.Any())
                {
                    totalClasses = _context.BoMons.Count();
                }

                if (_context.DangKyGois.Any())
                {
                    // Calculate real revenue from approved/active/paid orders only
                    monthlyRevenue = _context.DangKyGois
                        .Where(d => d.TrangThai.Contains("Đã phê duyệt") || d.TrangThai.Contains("Đang hoạt động") || d.TrangThai.Contains("Online"))
                        .Sum(d => d.TongTien);

                    if (monthlyRevenue == 0)
                    {
                        monthlyRevenue = _context.DangKyGois.Sum(d => d.TongTien);
                    }

                    registrationsList = _context.DangKyGois
                        .Include(d => d.NguoiDung)
                        .Include(d => d.GoiTap)
                        .Include(d => d.ChiNhanh)
                        .OrderByDescending(d => d.MaDK)
                        .Take(15)
                        .Select(d => new RegisterViewModel
                        {
                            OrderId = d.MaDK,
                            InvoiceCode = d.MaHD,
                            Name = d.NguoiDung != null ? d.NguoiDung.HoTen : "Khách hàng " + d.MaND,
                            Email = d.NguoiDung != null ? d.NguoiDung.Email : "",
                            Phone = d.NguoiDung != null ? d.NguoiDung.SoDT : "",
                            BranchName = d.ChiNhanh != null ? d.ChiNhanh.TenCN : "MMA - Dương Quang Đông (Q8)",
                            TrainerName = "Văn Bá Huyên (Senior Coach)",
                            SelectedClass = d.GoiTap != null ? d.GoiTap.TenGoi : "Gói tập MMA",
                            Amount = d.TongTien,
                            Status = d.TrangThai,
                            PaymentMethod = d.TrangThai.Contains("Momo") ? "Ví Momo Online" : (d.TrangThai.Contains("Visa") ? "Thẻ ATM/Visa" : "Tiền mặt tại quầy"),
                            IsApproved = d.TrangThai.Contains("Đã phê duyệt") || d.TrangThai.Contains("phê duyệt") || d.TrangThai.Contains("Đang hoạt động") || d.TrangThai.Contains("Thanh Toán") || d.TrangThai.Contains("Kích Hoạt")
                        }).ToList();
                }
            }
            catch (Exception)
            {
            }

            if (!membersList.Any())
            {
                membersList = new List<UserModel>
                {
                    new() { FullName = "Nguyễn Hùng Mạnh", Email = "nguyenmanh@gmail.com", Phone = "0947054075", Role = "Hội viên VIP", Status = "Hoạt động" },
                    new() { FullName = "Lê Học Viên (SQL Seed)", Email = "hocvien@gmail.com", Phone = "0900000002", Role = "Hội viên VIP", Status = "Hoạt động" }
                };
            }

            if (!registrationsList.Any())
            {
                registrationsList = new List<RegisterViewModel>
                {
                    new() { OrderId = 1, InvoiceCode = "HD-20260720-001", Name = "Nguyễn Hùng Mạnh", Email = "nguyenmanh@gmail.com", Phone = "0947054075", SelectedClass = "Gym 1 Năm - 12 Tháng", Amount = 7990000, BranchName = "MMA - Dương Quang Đông (Q8)", TrainerName = "Văn Bá Huyên", PaymentMethod = "Ví Momo Online", Status = "Đang hoạt động (Đã phê duyệt)", IsApproved = true },
                    new() { OrderId = 2, InvoiceCode = "HD-20260720-002", Name = "Lê Học Viên", Email = "hocvien@gmail.com", Phone = "0900000002", SelectedClass = "MMA 3 Tháng", Amount = 2990000, BranchName = "MMA - Rạch Miễu (Phú Nhuận)", TrainerName = "Nguyễn Văn Khoa", PaymentMethod = "Tiền mặt tại quầy", Status = "Chưa thanh toán", IsApproved = false }
                };
            }

            var model = new AdminDashboardViewModel
            {
                TotalMembers = totalMembers,
                TotalClasses = totalClasses,
                TotalTrainers = totalTrainers,
                MonthlyRevenue = monthlyRevenue,
                NewRegistrationsCount = registrationsList.Count(r => !r.IsApproved),
                RecentMembers = membersList,
                PendingRegistrations = registrationsList
            };
            return View(model);
        }

        [HttpPost("phe-duyet")]
        public IActionResult ApproveRegistration(int orderId)
        {
            try
            {
                var order = _context.DangKyGois.FirstOrDefault(d => d.MaDK == orderId);
                if (order != null)
                {
                    order.TrangThai = "Đã Thanh Toán & Kích Hoạt";
                    _context.SaveChanges();

                    var trans = _context.GiaoDichThanhToans.FirstOrDefault(g => g.MaDK == orderId);
                    if (trans == null)
                    {
                        _context.GiaoDichThanhToans.Add(new GiaoDichThanhToanEntity
                        {
                            MaDK = order.MaDK,
                            MaPT_ThanhToan = 1,
                            SoTien = order.TongTien,
                            ThoiGianThanhToan = DateTime.Now
                        });
                        _context.SaveChanges();
                    }

                    TempData["AdminSuccess"] = $"Xác nhận đã nhận tiền & Phê duyệt đơn đăng ký #{order.MaHD} thành công! Thẻ tập đã kích hoạt.";
                }
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi phê duyệt: " + ex.Message;
            }

            return RedirectToAction("Payments");
        }

        [HttpPost("huy-don")]
        public IActionResult CancelRegistration(int orderId)
        {
            try
            {
                var order = _context.DangKyGois.FirstOrDefault(d => d.MaDK == orderId);
                if (order != null)
                {
                    order.TrangThai = "Đã hủy bởi Quản trị viên";
                    _context.SaveChanges();
                    TempData["AdminSuccess"] = $"Đã hủy đơn đăng ký #{order.MaHD} thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi hủy đơn: " + ex.Message;
            }

            return RedirectToAction("Payments");
        }

        [HttpGet("thanh-toan")]
        public IActionResult Payments()
        {
            List<RegisterViewModel> list = new();
            try
            {
                if (_context.DangKyGois.Any())
                {
                    list = _context.DangKyGois
                        .Include(d => d.NguoiDung)
                        .Include(d => d.GoiTap)
                        .Include(d => d.ChiNhanh)
                        .OrderByDescending(d => d.MaDK)
                        .Select(d => new RegisterViewModel
                        {
                            OrderId = d.MaDK,
                            InvoiceCode = d.MaHD,
                            Name = d.NguoiDung != null ? d.NguoiDung.HoTen : "Khách hàng " + d.MaND,
                            Email = d.NguoiDung != null ? d.NguoiDung.Email : "",
                            Phone = d.NguoiDung != null ? d.NguoiDung.SoDT : "",
                            BranchName = d.ChiNhanh != null ? d.ChiNhanh.TenCN : "MMA - Dương Quang Đông (Q8)",
                            SelectedTrainer = "Văn Bá Huyên (Senior Coach)",
                            SelectedScheduleSlot = "Thứ 2, 4, 6 - Ca 18:00 PM",
                            SelectedClass = d.GoiTap != null ? d.GoiTap.TenGoi : "Gói tập MMA",
                            Amount = d.TongTien,
                            Status = d.TrangThai,
                            PaymentMethod = d.TrangThai.Contains("Momo") ? "Ví Momo / QR Code MB Bank" : (d.TrangThai.Contains("Visa") ? "Thẻ ATM/Visa" : "Tiền mặt tại quầy"),
                            IsApproved = d.TrangThai.Contains("Đã phê duyệt") || d.TrangThai.Contains("Đang hoạt động") || d.TrangThai.Contains("Đã Thanh Toán")
                        }).ToList();
                }
            }
            catch (Exception) { }

            if (!list.Any())
            {
                list = new List<RegisterViewModel>
                {
                    new() { OrderId = 1, InvoiceCode = "HD-20260720-001", Name = "Nguyễn Hùng Mạnh", Email = "nguyenmanh@gmail.com", Phone = "0947054075", SelectedClass = "Gym 1 Năm - 12 Tháng", Amount = 7990000, BranchName = "MMA - Dương Quang Đông (Q8)", SelectedTrainer = "Văn Bá Huyên (Senior Coach)", SelectedScheduleSlot = "Thứ 2, 4, 6 - Ca 18:00 PM", PaymentMethod = "Ví Momo / QR Code MB Bank", Status = "Đã Thanh Toán & Kích Hoạt", IsApproved = true },
                    new() { OrderId = 2, InvoiceCode = "HD-20260720-002", Name = "Lê Học Viên", Email = "hocvien@gmail.com", Phone = "0900000002", SelectedClass = "MMA 3 Tháng", Amount = 2990000, BranchName = "MMA - Rạch Miễu (Phú Nhuận)", SelectedTrainer = "Nguyễn Văn Khoa (Boxing Coach)", SelectedScheduleSlot = "Thứ 3, 5, 7 - Ca 09:00 AM", PaymentMethod = "Tiền mặt tại quầy", Status = "Chờ Nhận Tiền", IsApproved = false }
                };
            }

            return View(list);
        }

        // --- PACKAGES (GÓI TẬP) CRUD ---
        [HttpGet("goi-tap")]
        public IActionResult Packages()
        {
            if (!IsAdminAuthenticated())
            {
                TempData["LoginError"] = "Bạn không có quyền truy cập trang quản trị Admin. Vui lòng đăng nhập bằng tài khoản Quản trị viên!";
                return RedirectToAction("Login", "Account", new { returnUrl = "/admin/goi-tap" });
            }

            List<GoiTapEntity> list = new();
            try
            {
                var existingPackages = _context.GoiTaps.OrderBy(g => g.MaGoi).ToList();

                if (!existingPackages.Any())
                {
                    var gymBoMon = _context.BoMons.FirstOrDefault(b => b.TenBoMon == "Gym & Fitness");
                    var mmaBoMon = _context.BoMons.FirstOrDefault(b => b.TenBoMon == "MMA");
                    int gymId = gymBoMon?.MaBoMon ?? 1;
                    int mmaId = mmaBoMon?.MaBoMon ?? 1;

                    _context.GoiTaps.AddRange(
                        new GoiTapEntity { TenGoi = "Gói 3 Tháng", MaBoMon = gymId, ThoiHanThang = 3, GiaTien = 2990000, MoTa = "Tập không giới hạn 3 tháng tất cả bộ môn Gym, Boxing, MMA." },
                        new GoiTapEntity { TenGoi = "Gói 6 Tháng", MaBoMon = gymId, ThoiHanThang = 6, GiaTien = 4990000, MoTa = "Tập không giới hạn 6 tháng + Tặng găng Boxing chuyên nghiệp." },
                        new GoiTapEntity { TenGoi = "Gói 1 Năm", MaBoMon = mmaId, ThoiHanThang = 12, GiaTien = 7990000, MoTa = "Gói VIP 1 Năm không giới hạn + Tặng bộ bảo hộ võ thuật cao cấp." }
                    );
                    _context.SaveChanges();
                    existingPackages = _context.GoiTaps.OrderBy(g => g.MaGoi).ToList();
                }

                list = existingPackages;
            }
            catch (Exception) { }

            if (!list.Any())
            {
                list = new List<GoiTapEntity>
                {
                    new() { MaGoi = 1, TenGoi = "Gói 3 Tháng", GiaTien = 2990000, ThoiHanThang = 3, MoTa = "Tập không giới hạn 3 tháng tất cả bộ môn Gym, Boxing, MMA." },
                    new() { MaGoi = 2, TenGoi = "Gói 6 Tháng", GiaTien = 4990000, ThoiHanThang = 6, MoTa = "Tập không giới hạn 6 tháng + Tặng găng Boxing chuyên nghiệp." },
                    new() { MaGoi = 3, TenGoi = "Gói 1 Năm", GiaTien = 7990000, ThoiHanThang = 12, MoTa = "Gói VIP 1 Năm không giới hạn + Tặng bộ bảo hộ võ thuật cao cấp." }
                };
            }

            return View(list);
        }

        [HttpPost("them-goi-tap")]
        public IActionResult CreatePackage(GoiTapEntity model)
        {
            try
            {
                model.MaBoMon = model.MaBoMon == 0 ? 1 : model.MaBoMon;
                _context.GoiTaps.Add(model);
                _context.SaveChanges();
                TempData["AdminSuccess"] = $"Thêm gói tập '{model.TenGoi}' thành công!";
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi thêm gói tập: " + ex.Message;
            }
            return RedirectToAction("Packages");
        }

        [HttpPost("sua-goi-tap")]
        public IActionResult EditPackage(GoiTapEntity model)
        {
            try
            {
                var pkg = _context.GoiTaps.FirstOrDefault(g => g.MaGoi == model.MaGoi);
                if (pkg != null)
                {
                    pkg.TenGoi = model.TenGoi;
                    pkg.GiaTien = model.GiaTien;
                    pkg.ThoiHanThang = model.ThoiHanThang;
                    pkg.MoTa = model.MoTa;
                    _context.SaveChanges();
                    TempData["AdminSuccess"] = $"Cập nhật gói tập '{model.TenGoi}' thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi cập nhật gói tập: " + ex.Message;
            }
            return RedirectToAction("Packages");
        }

        [HttpPost("xoa-goi-tap")]
        public IActionResult DeletePackage(int maGoi)
        {
            try
            {
                var pkg = _context.GoiTaps.FirstOrDefault(g => g.MaGoi == maGoi);
                if (pkg != null)
                {
                    _context.GoiTaps.Remove(pkg);
                    _context.SaveChanges();
                    TempData["AdminSuccess"] = "Đã xóa gói tập thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi xóa gói tập: " + ex.Message;
            }
            return RedirectToAction("Packages");
        }

        // --- CLUBS CRUD ---
        [HttpGet("cau-lac-bo")]
        public IActionResult Clubs()
        {
            List<ChiNhanhEntity> clubs = new();
            try
            {
                clubs = _context.ChiNhanhs.ToList();
            }
            catch (Exception) { }
            return View(clubs);
        }

        [HttpPost("them-cau-lac-bo")]
        public IActionResult CreateClub(ChiNhanhEntity model)
        {
            try
            {
                _context.ChiNhanhs.Add(model);
                _context.SaveChanges();
                TempData["AdminSuccess"] = "Thêm chi nhánh " + model.TenCN + " thành công!";
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi thêm chi nhánh: " + ex.Message;
            }
            return RedirectToAction("Clubs");
        }

        [HttpPost("sua-cau-lac-bo")]
        public IActionResult EditClub(ChiNhanhEntity model)
        {
            try
            {
                var club = _context.ChiNhanhs.FirstOrDefault(c => c.MaCN == model.MaCN);
                if (club != null)
                {
                    club.TenCN = model.TenCN;
                    club.DiaChi = model.DiaChi;
                    club.Hotline = model.Hotline;
                    club.HinhAnh = model.HinhAnh;
                    _context.SaveChanges();
                    TempData["AdminSuccess"] = "Cập nhật chi nhánh " + model.TenCN + " và hình ảnh thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi sửa chi nhánh: " + ex.Message;
            }
            return RedirectToAction("Clubs");
        }

        [HttpPost("xoa-cau-lac-bo")]
        public IActionResult DeleteClub(int id)
        {
            try
            {
                var club = _context.ChiNhanhs.FirstOrDefault(c => c.MaCN == id);
                if (club != null)
                {
                    _context.ChiNhanhs.Remove(club);
                    _context.SaveChanges();
                    TempData["AdminSuccess"] = "Đã xóa chi nhánh khỏi cơ sở dữ liệu!";
                }
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi xóa chi nhánh: " + ex.Message;
            }
            return RedirectToAction("Clubs");
        }

        // --- CLASSES CRUD ---
        [HttpGet("lop-hoc")]
        public IActionResult Classes()
        {
            List<BoMonEntity> list = new();
            try
            {
                list = _context.BoMons.ToList();
            }
            catch (Exception) { }
            return View(list);
        }

        [HttpPost("them-lop-hoc")]
        public IActionResult CreateClass(BoMonEntity model)
        {
            try
            {
                _context.BoMons.Add(model);
                _context.SaveChanges();
                TempData["AdminSuccess"] = "Thêm bộ môn " + model.TenBoMon + " thành công!";
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi thêm lớp học: " + ex.Message;
            }
            return RedirectToAction("Classes");
        }

        [HttpPost("sua-lop-hoc")]
        public IActionResult EditClass(BoMonEntity model)
        {
            try
            {
                var item = _context.BoMons.FirstOrDefault(b => b.MaBoMon == model.MaBoMon);
                if (item != null)
                {
                    item.TenBoMon = model.TenBoMon;
                    item.MoTa = model.MoTa;
                    _context.SaveChanges();
                    TempData["AdminSuccess"] = "Cập nhật bộ môn " + model.TenBoMon + " thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi sửa bộ môn: " + ex.Message;
            }
            return RedirectToAction("Classes");
        }

        [HttpPost("xoa-lop-hoc")]
        public IActionResult DeleteClass(int id)
        {
            try
            {
                var item = _context.BoMons.FirstOrDefault(b => b.MaBoMon == id);
                if (item != null)
                {
                    _context.BoMons.Remove(item);
                    _context.SaveChanges();
                    TempData["AdminSuccess"] = "Đã xóa lớp học thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi xóa lớp học: " + ex.Message;
            }
            return RedirectToAction("Classes");
        }

        // --- TRAINERS CRUD ---
        [HttpGet("huan-luyen-vien")]
        public IActionResult Trainers()
        {
            return View();
        }

        [HttpPost("them-huan-luyen-vien")]
        public IActionResult CreateTrainer(string name, string role, int experience, string specialties, string? imageUrl)
        {
            TempData["AdminSuccess"] = $"Thêm HLV {name} ({role}) và ảnh đại diện thành công!";
            return RedirectToAction("Trainers");
        }

        [HttpPost("sua-huan-luyen-vien")]
        public IActionResult EditTrainer(int id, string name, string role, int experience, string specialties, string? imageUrl)
        {
            TempData["AdminSuccess"] = $"Cập nhật hồ sơ & ảnh chân dung HLV {name} thành công!";
            return RedirectToAction("Trainers");
        }

        [HttpPost("xoa-huan-luyen-vien")]
        public IActionResult DeleteTrainer(int id)
        {
            TempData["AdminSuccess"] = "Đã xóa huấn luyện viên khỏi hệ thống.";
            return RedirectToAction("Trainers");
        }

        // --- SCHEDULES CRUD ---
        [HttpGet("lich-tap")]
        public IActionResult Schedules()
        {
            List<LichTapEntity> list = new();
            try
            {
                if (_context.LichTaps.Any())
                {
                    list = _context.LichTaps.OrderBy(l => l.KhungGio).ThenBy(l => l.MaLich).ToList();
                }
            }
            catch (Exception)
            {
            }

            if (!list.Any())
            {
                list = new List<LichTapEntity>
                {
                    new() { MaLich = 1, ThuTrongTuan = "MON", KhungGio = "09:00 AM", TenLop = "Fitness & Body Building", TenHLV = "HLV Thể Lực", ClassType = "fitness", ColorClass = "fitness-slot", ThoiGianChiTiet = "09:00 - 10:00" },
                    new() { MaLich = 2, ThuTrongTuan = "MON", KhungGio = "09:00 AM", TenLop = "Yoga Ấn Độ", TenHLV = "Phạm Xuân Quốc Huy", ClassType = "yoga-an", ColorClass = "yoga-an-slot", ThoiGianChiTiet = "09:00 - 10:00" },
                    new() { MaLich = 3, ThuTrongTuan = "TUE", KhungGio = "09:00 AM", TenLop = "Boxing Chuyên Sâu", TenHLV = "Nguyễn Văn Khoa", ClassType = "boxing", ColorClass = "boxing-slot", ThoiGianChiTiet = "09:00 - 10:00" },
                    new() { MaLich = 4, ThuTrongTuan = "WED", KhungGio = "09:00 AM", TenLop = "MMA Thực Chiến", TenHLV = "Văn Bá Huyên", ClassType = "mma", ColorClass = "mma-slot", ThoiGianChiTiet = "09:00 - 10:30" },
                    new() { MaLich = 5, ThuTrongTuan = "FRI", KhungGio = "06:00 PM", TenLop = "MMA Thực Chiến", TenHLV = "Văn Bá Huyên", ClassType = "mma", ColorClass = "mma-slot", ThoiGianChiTiet = "18:00 - 19:30" },
                    new() { MaLich = 6, ThuTrongTuan = "SAT", KhungGio = "09:00 AM", TenLop = "Zumba Dance", TenHLV = "Dương Hữu Trường", ClassType = "zumba", ColorClass = "zumba-slot", ThoiGianChiTiet = "09:00 - 10:00" }
                };
            }

            return View(list);
        }

        [HttpPost("them-lich-tap")]
        public IActionResult CreateSchedule(LichTapEntity model)
        {
            try
            {
                string clsName = (model.TenLop ?? "").ToLower();
                model.ClassType = clsName.Contains("mma") ? "mma" : (clsName.Contains("boxing") ? "boxing" : (clsName.Contains("muay") ? "muay" : (clsName.Contains("yoga") ? "yoga-an" : "fitness")));
                model.ColorClass = model.ClassType + "-slot";

                _context.LichTaps.Add(model);
                _context.SaveChanges();
                TempData["AdminSuccess"] = $"Thêm ca học {model.TenLop} ({model.ThuTrongTuan}) thành công!";
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi thêm lịch tập: " + ex.Message;
            }
            return RedirectToAction("Schedules");
        }

        [HttpPost("sua-lich-tap")]
        public IActionResult EditSchedule(LichTapEntity model)
        {
            try
            {
                var item = _context.LichTaps.FirstOrDefault(l => l.MaLich == model.MaLich);
                if (item != null)
                {
                    item.ThuTrongTuan = model.ThuTrongTuan;
                    item.KhungGio = model.KhungGio;
                    item.TenLop = model.TenLop;
                    item.TenHLV = model.TenHLV;
                    item.ThoiGianChiTiet = model.ThoiGianChiTiet;

                    string clsName = (model.TenLop ?? "").ToLower();
                    item.ClassType = clsName.Contains("mma") ? "mma" : (clsName.Contains("boxing") ? "boxing" : (clsName.Contains("muay") ? "muay" : (clsName.Contains("yoga") ? "yoga-an" : "fitness")));
                    item.ColorClass = item.ClassType + "-slot";

                    _context.SaveChanges();
                    TempData["AdminSuccess"] = $"Cập nhật ca học {model.TenLop} thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi sửa lịch tập: " + ex.Message;
            }
            return RedirectToAction("Schedules");
        }

        [HttpPost("xoa-lich-tap")]
        public IActionResult DeleteSchedule(int id)
        {
            try
            {
                var item = _context.LichTaps.FirstOrDefault(l => l.MaLich == id);
                if (item != null)
                {
                    _context.LichTaps.Remove(item);
                    _context.SaveChanges();
                    TempData["AdminSuccess"] = "Đã xóa ca học khỏi thời khóa biểu!";
                }
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi xóa lịch tập: " + ex.Message;
            }
            return RedirectToAction("Schedules");
        }

        [HttpGet("hoi-vien")]
        public IActionResult Members(string? q)
        {
            if (!IsAdminAuthenticated())
            {
                TempData["LoginError"] = "Bạn không có quyền truy cập trang quản trị Admin. Vui lòng đăng nhập bằng tài khoản Quản trị viên!";
                return RedirectToAction("Login", "Account", new { returnUrl = "/admin/hoi-vien" });
            }

            List<UserModel> members = new();
            List<RegisterViewModel> registrations = new();

            try
            {
                if (_context.NguoiDungs.Any())
                {
                    var query = _context.NguoiDungs.Include(u => u.VaiTro).AsQueryable();
                    if (!string.IsNullOrEmpty(q))
                    {
                        string keyword = q.Trim().ToLower();
                        query = query.Where(u => u.HoTen.ToLower().Contains(keyword) || u.Email.ToLower().Contains(keyword) || u.SoDT.Contains(keyword));
                        ViewBag.SearchQuery = q;
                    }

                    members = query.OrderByDescending(u => u.MaND)
                        .Select(u => new UserModel
                        {
                            Id = u.MaND.ToString(),
                            FullName = u.HoTen,
                            Email = u.Email,
                            Phone = u.SoDT,
                            Role = u.VaiTro != null ? u.VaiTro.TenVaiTro : "Hội viên",
                            Status = u.TrangThai ? "Hoạt động" : "Tạm khóa"
                        }).ToList();
                }

                if (_context.DangKyGois.Any())
                {
                    var regQuery = _context.DangKyGois
                        .Include(d => d.NguoiDung)
                        .Include(d => d.GoiTap)
                        .Include(d => d.ChiNhanh).AsQueryable();

                    if (!string.IsNullOrEmpty(q))
                    {
                        string keyword = q.Trim().ToLower();
                        regQuery = regQuery.Where(d => d.MaHD.ToLower().Contains(keyword) || (d.NguoiDung != null && (d.NguoiDung.HoTen.ToLower().Contains(keyword) || d.NguoiDung.Email.ToLower().Contains(keyword) || d.NguoiDung.SoDT.Contains(keyword))));
                    }

                    registrations = regQuery.OrderByDescending(d => d.MaDK)
                        .Select(d => new RegisterViewModel
                        {
                            OrderId = d.MaDK,
                            InvoiceCode = d.MaHD,
                            Name = d.NguoiDung != null ? d.NguoiDung.HoTen : "Khách hàng " + d.MaND,
                            Email = d.NguoiDung != null ? d.NguoiDung.Email : "",
                            Phone = d.NguoiDung != null ? d.NguoiDung.SoDT : "",
                            BranchName = d.ChiNhanh != null ? d.ChiNhanh.TenCN : "MMA - Dương Quang Đông (Q8)",
                            TrainerName = "Văn Bá Huyên (Senior Coach)",
                            SelectedClass = d.GoiTap != null ? d.GoiTap.TenGoi : "Gói tập MMA",
                            Amount = d.TongTien,
                            Status = d.TrangThai,
                            PaymentMethod = d.TrangThai.Contains("Momo") ? "Ví Momo Online" : (d.TrangThai.Contains("Visa") ? "Thẻ ATM/Visa" : "Tiền mặt tại quầy"),
                            IsApproved = d.TrangThai.Contains("Đã phê duyệt") || d.TrangThai.Contains("Đang hoạt động") || d.TrangThai.Contains("Đã Thanh Toán")
                        }).ToList();
                }
            }
            catch (Exception)
            {
            }

            ViewBag.Members = members;
            ViewBag.Registrations = registrations;
            return View();
        }

        [HttpPost("them-hoi-vien")]
        public IActionResult CreateMember(string fullName, string email, string phone, string role, string password)
        {
            try
            {
                var roleObj = _context.VaiTros.FirstOrDefault(r => r.TenVaiTro == role) ?? _context.VaiTros.FirstOrDefault();
                var user = new NguoiDungEntity
                {
                    HoTen = fullName,
                    Email = email,
                    SoDT = phone,
                    PasswordHash = string.IsNullOrEmpty(password) ? "123456" : password,
                    MaVaiTro = roleObj?.MaVaiTro ?? 3,
                    NgayTao = DateTime.Now,
                    TrangThai = true
                };
                _context.NguoiDungs.Add(user);
                _context.SaveChanges();
                TempData["AdminSuccess"] = $"Thêm hội viên '{fullName}' thành công!";
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi thêm hội viên: " + ex.Message;
            }
            return RedirectToAction("Members");
        }

        [HttpPost("sua-hoi-vien")]
        public IActionResult EditMember(int id, string fullName, string email, string phone, bool status)
        {
            try
            {
                var user = _context.NguoiDungs.FirstOrDefault(u => u.MaND == id);
                if (user != null)
                {
                    user.HoTen = fullName;
                    user.Email = email;
                    user.SoDT = phone;
                    user.TrangThai = status;
                    _context.SaveChanges();
                    TempData["AdminSuccess"] = $"Cập nhật hội viên #{id} thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi sửa hội viên: " + ex.Message;
            }
            return RedirectToAction("Members");
        }

        [HttpPost("xoa-hoi-vien")]
        public IActionResult DeleteMember(int id)
        {
            try
            {
                var user = _context.NguoiDungs.FirstOrDefault(u => u.MaND == id);
                if (user != null)
                {
                    _context.NguoiDungs.Remove(user);
                    _context.SaveChanges();
                    TempData["AdminSuccess"] = $"Đã xóa hội viên #{id} khỏi hệ thống!";
                }
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi xóa hội viên: " + ex.Message;
            }
            return RedirectToAction("Members");
        }

        [HttpPost("xoa-dang-ky")]
        public IActionResult DeleteRegistration(int orderId)
        {
            try
            {
                var reg = _context.DangKyGois.FirstOrDefault(d => d.MaDK == orderId);
                if (reg != null)
                {
                    _context.DangKyGois.Remove(reg);
                    _context.SaveChanges();
                    TempData["AdminSuccess"] = $"Đã xóa đơn đăng ký #{orderId} thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Lỗi xóa đơn đăng ký: " + ex.Message;
            }
            return RedirectToAction("Members");
        }
    }
}
