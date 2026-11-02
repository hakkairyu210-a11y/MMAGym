using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMA.Data;
using MMA.Models;
using MMA.Models.Entities;

namespace MMA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MmaDbContext _context;

        // --- Mock Data Store ---
        private static readonly List<ClassModel> ClassesData = new()
        {
            new() { Id = "mma", Title = "MMA - Mixed Martial Arts", ImageUrl = "https://images.unsplash.com/photo-1549719386-74dfcbf7dbed?w=800&auto=format&fit=crop&q=80", IconClass = "fas fa-shield-halved", Description = "Võ thuật tự do tổng hợp đỉnh cao: Kết hợp Boxing, Muay Thái, Nhu thuật BJJ và Wrestling. Đốt calo tối đa và tự vệ toàn diện.", ScheduleTime = "Thứ 2, 4, 6 | 18:00 - 19:30", TrainerName = "Văn Bá Huyên", CaloriesBurned = "800 - 1200 kcal/buổi", TargetAudience = "Mọi đối tượng đam mê võ thuật thực chiến & thể lực thép", DetailedBenefits = "Kỹ thuật đấm, đá, vật, siết cổ truyền & hiện đại; gia tăng sức mạnh bùng nổ." },
            new() { Id = "boxing", Title = "Boxing Chuyên Sâu", ImageUrl = "https://images.unsplash.com/photo-1517838277536-f5f99be501cd?w=800&auto=format&fit=crop&q=80", IconClass = "fas fa-hand-fist", Description = "Rèn luyện phản xạ nhanh nhạy, sức bền cơ bắp và kỹ thuật đấm chuẩn võ thuật.", ScheduleTime = "Thứ 2, 4, 6 | 09:00, 13:00", TrainerName = "Nguyễn Văn Khoa", CaloriesBurned = "700 - 1000 kcal/buổi", TargetAudience = "Phù hợp nam nữ muốn săn chắc cơ thể & xả stress", DetailedBenefits = "Tối ưu phản xạ tay mắt, tăng tốc độ di chuyển và săn chắc vòng tay, bụng." },
            new() { Id = "muay", Title = "Muay Thái Võ Thuật", ImageUrl = "https://images.unsplash.com/photo-1517838277536-f5f99be501cd?w=800&auto=format&fit=crop&q=80", IconClass = "fas fa-shield-cat", Description = "Võ thuật cổ truyền Thái Lan tập trung phản xạ tự vệ và gia tăng thể chất mạnh mẽ bằng chỏ và gối.", ScheduleTime = "Chủ nhật | 18:00", TrainerName = "Văn Bá Huyên", CaloriesBurned = "750 - 1100 kcal/buổi", TargetAudience = "Người thích sự uy lực và rèn luyện độ dẻo dai toàn thân", DetailedBenefits = "Luyện đòn chỏ gối sắc bén, tăng mật độ xương và khả năng chịu đựng." },
            new() { Id = "fitness", Title = "Fitness & Body Building", ImageUrl = "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?w=800&auto=format&fit=crop&q=80", IconClass = "fas fa-dumbbell", Description = "Rèn luyện thể lực tổng hợp, tăng cơ giảm mỡ dưới sự giám sát sát sao từ HLV chuyên nghiệp.", ScheduleTime = "Thứ 2, 4, 6 | 09:00, 13:00, 18:00", TrainerName = "HLV Thể Lực", CaloriesBurned = "500 - 800 kcal/buổi", TargetAudience = "Người muốn tăng cơ, phát triển thể hình cân đối", DetailedBenefits = "Tăng khối lượng cơ bắp, đốt mỡ thừa và cải thiện tư thế vóc dáng." },
            new() { Id = "yoga-an", Title = "Yoga Ấn Độ", ImageUrl = "https://images.unsplash.com/photo-1545205597-3d9d02c29597?w=800&auto=format&fit=crop&q=80", IconClass = "fas fa-spa", Description = "Phương pháp Yoga truyền thống từ Ấn Độ giúp tĩnh tâm, thư giãn tinh thần và nâng cao độ dẻo dai.", ScheduleTime = "Thứ 2 | 09:00", TrainerName = "Phạm Xuân Quốc Huy", CaloriesBurned = "350 - 500 kcal/buổi", TargetAudience = "Phù hợp dân văn phòng, người muốn cân bằng cuộc sống", DetailedBenefits = "Cột sống dẻo dai, giảm đau lưng hông, cải thiện giấc ngủ và nhịp thở." },
            new() { Id = "cycling", Title = "Cycling Đạp Xe", ImageUrl = "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?w=800&auto=format&fit=crop&q=80", IconClass = "fas fa-bicycle", Description = "Lớp đạp xe tập thể trên nền nhạc sôi động giúp rèn luyện sức bền tim mạch.", ScheduleTime = "Thứ 4 | 09:00", TrainerName = "Văn Bá Huyên", CaloriesBurned = "600 - 850 kcal/buổi", TargetAudience = "Người muốn đốt mỡ đùi, mông và săn chắc đôi chân", DetailedBenefits = "Tăng nhịp tim khỏe mạnh, nâng cao sức bền chân và đùi." },
            new() { Id = "zumba", Title = "Zumba Dance", ImageUrl = "https://images.unsplash.com/photo-1518611012118-696072aa579a?w=800&auto=format&fit=crop&q=80", IconClass = "fas fa-drum", Description = "Sự kết hợp hoàn hảo giữa vũ đạo Latin cuồng nhiệt và các bài tập cardio năng động.", ScheduleTime = "Thứ 7 | 09:00", TrainerName = "Dương Hữu Trường", CaloriesBurned = "500 - 750 kcal/buổi", TargetAudience = "Tất cả mọi người đam mê âm nhạc & nhảy giải tỏa áp lực", DetailedBenefits = "Giải phóng năng lượng tiêu cực, tăng độ linh hoạt cơ thể." }
        };

        private static readonly List<TrainerModel> TrainersData = new()
        {
            new() { Id = "1", Name = "Văn Bá Huyên", Role = "Senior MMA & Muay Thai Coach", ImageUrl = "https://images.unsplash.com/photo-1567013127542-490d757e51fc?w=500&auto=format&fit=crop&q=80", ExperienceYears = 12, Certificates = "Bằng HLV MMA Quốc Tế & Thượng Đài SEA Games HCB", Bio = "Vận động viên võ thuật quốc gia với hơn 12 năm kinh nghiệm đào tạo thảm đấu MMA và Muay Thái thực chiến.", Specialties = "MMA, Muay Thái, Kickboxing & Phản xạ tự vệ" },
            new() { Id = "2", Name = "Nguyễn Văn Khoa", Role = "Boxing Head Coach", ImageUrl = "https://images.unsplash.com/photo-1549719386-74dfcbf7dbed?w=500&auto=format&fit=crop&q=80", ExperienceYears = 10, Certificates = "HLV Boxing Quốc Gia Cấp 1", Bio = "Chuyên gia đào tạo kỹ thuật đấm boxing đỉnh cao, đã giúp hơn 600+ hội viên làm chủ bộ pháp và thể lực bùng nổ.", Specialties = "Boxing Chuyên Sâu, Footwork & Giảm Cân Cấp Tốc" },
            new() { Id = "3", Name = "Dương Hữu Trường", Role = "Master Body Builder", ImageUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=500&auto=format&fit=crop&q=80", ExperienceYears = 9, Certificates = "Chứng chỉ HLV Thể Hình ISSA Mỹ", Bio = "VĐV Thể hình chuyên nghiệp, chuyên thiết kế lộ trình tăng cơ và siết mỡ khoa học cho cả nam và nữ.", Specialties = "Tăng Cơ Nhanh, Siết Mỡ Chuyên Sâu & Nutrition" },
            new() { Id = "4", Name = "Phạm Xuân Quốc Huy", Role = "Yoga & Recovery Specialist", ImageUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=500&auto=format&fit=crop&q=80", ExperienceYears = 8, Certificates = "Chứng chỉ Yoga Alliance RYT-500 Ấn Độ", Bio = "Chuyên gia Yoga trị liệu phục hồi cơ thể, giúp giải tỏa căng thẳng cột sống và hồi phục tổn thương thể thao.", Specialties = "Yoga Trị Liệu, Hatha Yoga & Giảm Đau Cột Sống" }
        };

        private static readonly List<MembershipPlanModel> MembershipPlansData = new()
        {
            new() { 
                Id = "plan-3m", 
                Name = "Gói 3 Tháng", 
                DurationText = "3 THÁNG", 
                OriginalPrice = 3600000, 
                Price = 2990000, 
                PerMonthPrice = "~996.000đ / tháng", 
                IsPopular = false,
                Features = new() { 
                    "Tập không giới hạn Gym & Cardio tại 6 chi nhánh", 
                    "Tham gia các lớp MMA, Boxing, Muay Thái, Yoga, Cycling", 
                    "Tặng 01 buổi đo chỉ số cơ thể InBody & tư vấn HLV", 
                    "Sử dụng tủ đồ cá nhân & phòng tắm nóng lạnh" 
                } 
            },
            new() { 
                Id = "plan-6m", 
                Name = "Gói 6 Tháng", 
                DurationText = "6 THÁNG", 
                OriginalPrice = 6800000, 
                Price = 4990000, 
                PerMonthPrice = "~831.000đ / tháng", 
                IsPopular = false,
                Features = new() { 
                    "Tất cả quyền lợi Gói 3 Tháng", 
                    "Tặng 02 buổi tập riêng 1-1 cùng HLV Cá Nhân (PT)", 
                    "Tặng 01 Bộ Găng Tay Boxing MMA Chuyên Nghiệp", 
                    "Bảo lưu gói tập tối đa 30 ngày miễn phí" 
                } 
            },
            new() { 
                Id = "plan-12m", 
                Name = "Gói 1 Năm", 
                DurationText = "12 THÁNG (1 NĂM)", 
                OriginalPrice = 12500000, 
                Price = 7990000, 
                PerMonthPrice = "~665.000đ / tháng", 
                IsPopular = true,
                PopularBadgeText = "ƯU ĐÃI KHUYÊN DÙNG - TIẾT KIỆM 40%",
                Features = new() { 
                    "Tất cả quyền lợi Gói 6 Tháng", 
                    "Tặng 05 buổi tập riêng 1-1 cùng HLV Cá Nhân (PT)", 
                    "Tặng Thẻ VIP Dẫn Theo 01 Người Bạn Tập Cùng (2 lần/tháng)", 
                    "Tặng Full Bộ Đồ Tập Võ MMA + Khăn Tập Cao Cấp", 
                    "Bảo lưu gói tập tối đa 60 ngày miễn phí" 
                } 
            }
        };

        private static readonly List<ClubModel> ClubsData = new()
        {
            new() { Id = "1", Name = "MMA - Rạch Miễu", ImageUrl = "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?w=600&auto=format&fit=crop&q=80", Address = "Nhà thi đấu Rạch Miễu, 1 Đ. Hoa Phượng, Phú Nhuận, TP.HCM", Date = "18 Th6" },
            new() { Id = "2", Name = "MMA - Dương Quang Đông", ImageUrl = "https://images.unsplash.com/photo-1540497077202-7c8a3999166f?w=600&auto=format&fit=crop&q=80", Address = "23 Dương Quang Đông, Phường Chánh Hưng, Q8, TP.HCM", Date = "23 Th5" },
            new() { Id = "3", Name = "MMA - Topaz City", ImageUrl = "https://images.unsplash.com/photo-1571902943202-507ec2618e8f?w=600&auto=format&fit=crop&q=80", Address = "Topaz City Block B2, 39 Cao Lỗ, Chánh Hưng, Q8, TP.HCM", Date = "23 Th5" },
            new() { Id = "4", Name = "MMA - Lý Chiêu Hoàng", ImageUrl = "https://images.unsplash.com/photo-1574680096145-d05b474e2155?w=600&auto=format&fit=crop&q=80", Address = "105 Đ. Lý Chiêu Hoàng, Khu V, Bình Phú, Q6, TP. HCM", Date = "23 Th5" }
        };

        private static readonly List<TestimonialModel> TestimonialsData = new()
        {
            new() { Id = "1", Name = "Hội Viên Ngọc Yến", Designation = "Giảm 10% mỡ thành công", ImageUrl = "https://fitbox.vn/wp-content/uploads/2026/05/HOI-VIEN-NGOC-YEN-150x150.png", Content = "Biết đến với hành trình tập luyện đáng ngưỡng mộ đã giúp chị giảm hơn 10% mỡ, chị Yến khiến nhiều người thán phục với tần suất tập luyện chăm chỉ, chị luôn quyết tâm và khắt khe với bản thân." },
            new() { Id = "2", Name = "Hội Viên Sỹ Nguyên", Designation = "Gắn bó hơn 3 năm", ImageUrl = "https://fitbox.vn/wp-content/uploads/2026/05/HOI-VIEN-SY-NGUYEN-150x150.png", Content = "Không xa lạ với hội viên tại MMA - đến nay anh đã gắn bó với MMA hơn 3 năm. Có niềm đam mê thể thao từ lâu anh Nguyên làm quen với Gym và có một quá trình dài tập luyện." }
        };

        private static readonly List<NewsModel> NewsData = new()
        {
            new() { Id = "1", Title = "CÁC BÀI TẬP GYM GIẢM MỠ BỤNG CHO NỮ – MMA Gym", ImageUrl = "https://fitbox.vn/wp-content/uploads/2026/05/bai-tap-gym-giam-mo-bung-side-crunch_0f6a9d262e394145889b37cf8dbfd245_grande.jpg", Category = "Thư viện bài tập", Author = "mmasaigon@gmail.com", Date = "16/06/2026", Excerpt = "Hướng dẫn các động tác gập bụng nghiêng, leo núi, plank liên sườn hỗ trợ đốt cháy mỡ bụng sâu hiệu quả vượt trội dành cho chị em phái nữ khi luyện tập tại nhà hoặc phòng gym." },
            new() { Id = "2", Title = "CÁC LOẠI HÌNH YOGA PHỔ BIẾN TẠI VIỆT NAM – MMA Gym", ImageUrl = "https://fitbox.vn/wp-content/uploads/2026/05/yoga-tri-lieu-xuong-khop_8620cf121f694101bae7d375913dcb21_grande.jpg", Category = "Thư viện bài tập", Author = "mmasaigon@gmail.com", Date = "16/06/2026", Excerpt = "Tìm hiểu về Hatha Yoga, Vinyasa Yoga, Yin Yoga, Gentle Yoga đang được phổ biến rộng rãi khắp các phòng tập Yoga chất lượng tại Việt Nam." }
        };

        private static readonly List<ScheduleSlotModel> ScheduleData = new()
        {
            new() {
                TimeSlot = "09:00 AM",
                Items = new() {
                    new() { Day = "MON", ClassName = "Fitness", ClassType = "fitness", TrainerName = "HLV Thể Lực", ColorClass = "fitness-slot", Duration = "09:00 - 10:00" },
                    new() { Day = "MON", ClassName = "Yoga Ấn", ClassType = "yoga-an", TrainerName = "P.X.Quốc Huy", ColorClass = "yoga-an-slot", Duration = "09:00 - 10:00" },
                    new() { Day = "TUE", ClassName = "Boxing", ClassType = "boxing", TrainerName = "Nguyễn Văn Khoa", ColorClass = "boxing-slot", Duration = "09:00 - 10:00" },
                    new() { Day = "WED", ClassName = "MMA", ClassType = "mma", TrainerName = "Văn Bá Huyên", ColorClass = "mma-slot", Duration = "09:00 - 10:30" },
                    new() { Day = "WED", ClassName = "Cycling", ClassType = "cycling", TrainerName = "Văn Bá Huyên", ColorClass = "cycling-slot", Duration = "09:00 - 10:00" },
                    new() { Day = "SAT", ClassName = "Zumba", ClassType = "zumba", TrainerName = "D.Hữu Trường", ColorClass = "zumba-slot", Duration = "09:00 - 10:00" }
                }
            },
            new() {
                TimeSlot = "06:00 PM",
                Items = new() {
                    new() { Day = "MON", ClassName = "MMA Thực Chiến", ClassType = "mma", TrainerName = "Văn Bá Huyên", ColorClass = "mma-slot", Duration = "18:00 - 19:30" },
                    new() { Day = "MON", ClassName = "Fitness", ClassType = "fitness", TrainerName = "HLV Thể Lực", ColorClass = "fitness-slot", Duration = "18:00 - 19:00" },
                    new() { Day = "WED", ClassName = "MMA Thực Chiến", ClassType = "mma", TrainerName = "Văn Bá Huyên", ColorClass = "mma-slot", Duration = "18:00 - 19:30" },
                    new() { Day = "FRI", ClassName = "MMA Thực Chiến", ClassType = "mma", TrainerName = "Văn Bá Huyên", ColorClass = "mma-slot", Duration = "18:00 - 19:30" }
                }
            }
        };

        public HomeController(ILogger<HomeController> logger, MmaDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            List<ClubModel> clubsList = ClubsData;
            List<MembershipPlanModel> plansList = MembershipPlansData;
            try
            {
                if (_context.ChiNhanhs.Any())
                {
                    clubsList = _context.ChiNhanhs.Select(c => new ClubModel
                    {
                        Id = c.MaCN.ToString(),
                        Name = c.TenCN,
                        Address = c.DiaChi,
                        ImageUrl = string.IsNullOrEmpty(c.HinhAnh) ? "https://images.unsplash.com/photo-1540497077202-7c8a3999166f?w=600&auto=format&fit=crop&q=80" : c.HinhAnh,
                        Date = "Hoạt động"
                    }).ToList();
                }

                // Ensure database has exact 3 clean packages: Gói 3 Tháng, Gói 6 Tháng, Gói 1 Năm
                var dbGoiTaps = _context.GoiTaps.OrderBy(g => g.MaGoi).ToList();
                if (!dbGoiTaps.Any())
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
                    dbGoiTaps = _context.GoiTaps.OrderBy(g => g.MaGoi).ToList();
                }
                else
                {
                    if (dbGoiTaps.Count >= 1) { dbGoiTaps[0].TenGoi = "Gói 3 Tháng"; dbGoiTaps[0].ThoiHanThang = 3; dbGoiTaps[0].GiaTien = 2990000; }
                    if (dbGoiTaps.Count >= 2) { dbGoiTaps[1].TenGoi = "Gói 6 Tháng"; dbGoiTaps[1].ThoiHanThang = 6; dbGoiTaps[1].GiaTien = 4990000; }
                    if (dbGoiTaps.Count >= 3) { dbGoiTaps[2].TenGoi = "Gói 1 Năm"; dbGoiTaps[2].ThoiHanThang = 12; dbGoiTaps[2].GiaTien = 7990000; }
                    try { _context.SaveChanges(); } catch (Exception) { }
                    dbGoiTaps = dbGoiTaps.Take(3).ToList();
                }

                plansList = dbGoiTaps.OrderBy(g => g.GiaTien).Select(g => new MembershipPlanModel
                {
                    Id = "plan-" + g.MaGoi,
                    Name = g.TenGoi,
                    DurationText = g.ThoiHanThang >= 12 ? "12 THÁNG (1 NĂM)" : g.ThoiHanThang + " THÁNG",
                    Price = g.GiaTien,
                    PerMonthPrice = $"~{(g.GiaTien / (g.ThoiHanThang > 0 ? g.ThoiHanThang : 1)):N0}đ / tháng",
                    OriginalPrice = g.GiaTien >= 7000000 ? 12500000 : (g.GiaTien >= 4000000 ? 6800000 : 3900000),
                    IsPopular = g.ThoiHanThang >= 12,
                    PopularBadgeText = g.ThoiHanThang >= 12 ? "ƯU ĐÃI KHUYÊN DÙNG - TIẾT KIỆM 40%" : "GÓI PHỔ THÔNG",
                    Features = new List<string> {
                        string.IsNullOrEmpty(g.MoTa) ? "Tập luyện tự do tất cả các môn MMA, Gym, Boxing." : g.MoTa,
                        "Miễn phí tủ đồ cá nhân & phòng tắm nóng lạnh",
                        "Tư vấn dinh dưỡng & kiểm tra BMI định kỳ",
                        "Tập luyện trên toàn hệ thống chi nhánh MMA"
                    }
                }).ToList();
            }
            catch (Exception) { }

            var model = new HomeViewModel
            {
                RegisterForm = new RegisterViewModel(),
                FeaturedClasses = ClassesData,
                FeaturedTrainers = TrainersData,
                MembershipPlans = plansList,
                Testimonials = TestimonialsData,
                Clubs = clubsList.Take(3).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [Route("")]
        public IActionResult Index(HomeViewModel model)
        {
            // Bắt buộc Đăng nhập mới được Đăng ký khóa tập
            var currentSessionUser = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(currentSessionUser))
            {
                TempData["LoginError"] = "Vui lòng ĐĂNG NHẬP tài khoản của bạn trước khi đăng ký gói tập!";
                return RedirectToAction("Login", "Account", new { returnUrl = "/#register-section" });
            }

            if (!ModelState.IsValid)
            {
                TempData["FormSuccess"] = false;
                model.FeaturedClasses = ClassesData;
                model.FeaturedTrainers = TrainersData;
                model.MembershipPlans = MembershipPlansData;
                model.Testimonials = TestimonialsData;
                model.Clubs = ClubsData.Take(3).ToList();
                return View(model);
            }

            SaveRegistrationToDb(model.RegisterForm.Name, model.RegisterForm.Email, model.RegisterForm.Phone, model.RegisterForm.SelectedClass, model.RegisterForm.RegistrationType, model.RegisterForm.PaymentMethod);

            TempData["FormSuccess"] = true;
            TempData["SubmittedName"] = model.RegisterForm.Name;
            TempData["PaymentMethod"] = model.RegisterForm.PaymentMethod;
            TempData["SelectedClass"] = model.RegisterForm.SelectedClass;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("gioi-thieu")]
        public IActionResult GioiThieu()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [Route("gioi-thieu")]
        public IActionResult GioiThieu(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["FormSuccess"] = false;
                return View(model);
            }

            SaveRegistrationToDb(model.Name, model.Email, model.Phone, model.SelectedClass, model.RegistrationType, model.PaymentMethod);

            TempData["FormSuccess"] = true;
            TempData["SubmittedName"] = model.Name;
            TempData["PaymentMethod"] = model.PaymentMethod;
            return RedirectToAction("GioiThieu");
        }

        [HttpGet]
        [Route("lop-hoc")]
        public IActionResult LopHoc()
        {
            List<ClassModel> list = ClassesData;
            try
            {
                if (_context.BoMons.Any())
                {
                    list = _context.BoMons.Select(b => new ClassModel
                    {
                        Id = b.MaBoMon.ToString(),
                        Title = b.TenBoMon,
                        Description = string.IsNullOrEmpty(b.MoTa) ? "Khóa học võ thuật & fitness chuyên sâu" : b.MoTa,
                        ImageUrl = "https://images.unsplash.com/photo-1549719386-74dfcbf7dbed?w=800&auto=format&fit=crop&q=80",
                        IconClass = "fas fa-hand-fist",
                        ScheduleTime = "Các ngày trong tuần | Ca Sáng & Tối",
                        TrainerName = "Đội ngũ HLV Quốc Gia",
                        CaloriesBurned = "700 - 1100 kcal/buổi",
                        TargetAudience = "Mọi đối tượng đam mê thể thao & võ thuật",
                        DetailedBenefits = "Phản xạ nhanh nhạy, sức bền cơ bắp và kỹ thuật thực chiến đỉnh cao."
                    }).ToList();
                }
            }
            catch (Exception) { }

            return View(list);
        }

        [HttpGet]
        [Route("huan-luyen-vien")]
        public IActionResult HuanLuyenVien()
        {
            return View(TrainersData);
        }

        [HttpGet]
        [Route("huan-luyen-vien/{id}")]
        public IActionResult TrainerDetail(string id)
        {
            var trainer = TrainersData.FirstOrDefault(t => t.Id == id || t.Name.ToLower().Replace(" ", "-") == id.ToLower());
            if (trainer == null)
            {
                trainer = TrainersData.FirstOrDefault();
            }
            return View(trainer);
        }

        [HttpGet]
        [Route("lich-hoc")]
        public IActionResult LichHoc()
        {
            List<ScheduleSlotModel> scheduleList = ScheduleData;
            try
            {
                if (_context.LichTaps.Any())
                {
                    var itemsFromDb = _context.LichTaps.ToList();
                    var morningItems = itemsFromDb.Where(l => l.KhungGio.Contains("09:00") || l.KhungGio.Contains("AM")).Select(l => new ScheduleItemModel
                    {
                        Day = l.ThuTrongTuan,
                        ClassName = l.TenLop,
                        ClassType = string.IsNullOrEmpty(l.ClassType) ? "mma" : l.ClassType,
                        TrainerName = l.TenHLV,
                        ColorClass = string.IsNullOrEmpty(l.ColorClass) ? "mma-slot" : l.ColorClass,
                        Duration = string.IsNullOrEmpty(l.ThoiGianChiTiet) ? "09:00 - 10:30" : l.ThoiGianChiTiet
                    }).ToList();

                    var eveningItems = itemsFromDb.Where(l => l.KhungGio.Contains("18:00") || l.KhungGio.Contains("PM") || l.KhungGio.Contains("Tối")).Select(l => new ScheduleItemModel
                    {
                        Day = l.ThuTrongTuan,
                        ClassName = l.TenLop,
                        ClassType = string.IsNullOrEmpty(l.ClassType) ? "mma" : l.ClassType,
                        TrainerName = l.TenHLV,
                        ColorClass = string.IsNullOrEmpty(l.ColorClass) ? "mma-slot" : l.ColorClass,
                        Duration = string.IsNullOrEmpty(l.ThoiGianChiTiet) ? "18:00 - 19:30" : l.ThoiGianChiTiet
                    }).ToList();

                    scheduleList = new List<ScheduleSlotModel>
                    {
                        new() { TimeSlot = "09:00 AM", Items = morningItems },
                        new() { TimeSlot = "06:00 PM", Items = eveningItems }
                    };
                }
            }
            catch (Exception) { }

            return View(scheduleList);
        }

        [HttpGet]
        [Route("tin-tuc")]
        public IActionResult TinTuc()
        {
            return View(NewsData);
        }

        [HttpGet]
        [Route("cau-lac-bo")]
        public IActionResult CauLacBo()
        {
            List<ClubModel> list = ClubsData;
            try
            {
                if (_context.ChiNhanhs.Any())
                {
                    list = _context.ChiNhanhs.Select(c => new ClubModel
                    {
                        Id = c.MaCN.ToString(),
                        Name = c.TenCN,
                        Address = c.DiaChi,
                        ImageUrl = "https://images.unsplash.com/photo-1540497077202-7c8a3999166f?w=600&auto=format&fit=crop&q=80",
                        Date = "Hoạt động"
                    }).ToList();
                }
            }
            catch (Exception) { }

            return View(list);
        }

        [HttpGet]
        [Route("lien-he")]
        public IActionResult LienHe()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [Route("lien-he")]
        public IActionResult LienHe(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["FormSuccess"] = false;
                return View(model);
            }

            SaveRegistrationToDb(model.Name, model.Email, model.Phone, model.SelectedClass, model.RegistrationType, model.PaymentMethod);

            TempData["FormSuccess"] = true;
            TempData["SubmittedName"] = model.Name;
            TempData["PaymentMethod"] = model.PaymentMethod;
            return RedirectToAction("LienHe");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void SaveRegistrationToDb(string name, string email, string phone, string selectedClass, string regType = "Gói Tập", string payMethod = "TienMat")
        {
            try
            {
                var sessionEmail = HttpContext.Session.GetString("UserEmail");
                var sessionUserId = HttpContext.Session.GetInt32("UserId");

                string targetEmail = !string.IsNullOrEmpty(email) ? email.Trim().ToLower() : (sessionEmail ?? "").Trim().ToLower();
                string targetPhone = (phone ?? "").Trim();
                string targetName = !string.IsNullOrEmpty(name) ? name.Trim() : (HttpContext.Session.GetString("UserName") ?? "Hội viên MMA");

                // 1. Find or create NguoiDung in SQL DB
                NguoiDungEntity? existingUser = null;
                if (sessionUserId.HasValue)
                {
                    existingUser = _context.NguoiDungs.FirstOrDefault(u => u.MaND == sessionUserId.Value);
                }
                if (existingUser == null && !string.IsNullOrEmpty(targetEmail))
                {
                    existingUser = _context.NguoiDungs.FirstOrDefault(u => u.Email.ToLower() == targetEmail);
                }
                if (existingUser == null && !string.IsNullOrEmpty(targetPhone))
                {
                    existingUser = _context.NguoiDungs.FirstOrDefault(u => u.SoDT == targetPhone);
                }

                if (existingUser == null)
                {
                    var memberRole = _context.VaiTros.FirstOrDefault(r => r.TenVaiTro == "Học viên");
                    existingUser = new NguoiDungEntity
                    {
                        HoTen = targetName,
                        Email = string.IsNullOrEmpty(targetEmail) ? $"user{DateTime.Now.Ticks}@mmasaigon.vn" : targetEmail,
                        SoDT = string.IsNullOrEmpty(targetPhone) ? "0900000000" : targetPhone,
                        PasswordHash = "123456",
                        MaVaiTro = memberRole?.MaVaiTro ?? 3,
                        NgayTao = DateTime.Now,
                        TrangThai = true
                    };
                    _context.NguoiDungs.Add(existingUser);
                    _context.SaveChanges();
                }

                // 2. Determine Package / Price / Duration by matching existing DB packages
                GoiTapEntity? targetGoi = null;
                string clsLower = (selectedClass ?? "").ToLower();

                if (clsLower.Contains("12") || clsLower.Contains("1 năm") || clsLower.Contains("nam"))
                {
                    targetGoi = _context.GoiTaps.FirstOrDefault(g => g.ThoiHanThang == 12 || g.TenGoi.Contains("1 Năm") || g.TenGoi.Contains("12"));
                }
                else if (clsLower.Contains("6"))
                {
                    targetGoi = _context.GoiTaps.FirstOrDefault(g => g.ThoiHanThang == 6 || g.TenGoi.Contains("6"));
                }
                else if (clsLower.Contains("3"))
                {
                    targetGoi = _context.GoiTaps.FirstOrDefault(g => g.ThoiHanThang == 3 || g.TenGoi.Contains("3"));
                }

                if (targetGoi == null && !string.IsNullOrEmpty(selectedClass))
                {
                    targetGoi = _context.GoiTaps.FirstOrDefault(g => g.TenGoi.ToLower().Contains(clsLower) || clsLower.Contains(g.TenGoi.ToLower()));
                }
                if (targetGoi == null)
                {
                    targetGoi = _context.GoiTaps.FirstOrDefault();
                }

                decimal price = targetGoi != null && targetGoi.GiaTien > 0 ? targetGoi.GiaTien : (clsLower.Contains("12") ? 7990000 : (clsLower.Contains("6") ? 4990000 : 2990000));
                int durationMonths = targetGoi != null && targetGoi.ThoiHanThang > 0 ? targetGoi.ThoiHanThang : (clsLower.Contains("12") ? 12 : (clsLower.Contains("6") ? 6 : 3));
                int goiId = targetGoi?.MaGoi ?? 1;

                var defaultCN = _context.ChiNhanhs.FirstOrDefault() ?? new ChiNhanhEntity { MaCN = 1 };

                string statusText = payMethod == "MomoOnline" || payMethod.Contains("Momo")
                    ? "Chờ Admin duyệt (Momo Online)"
                    : (payMethod == "VisaCard" || payMethod.Contains("Visa")
                        ? "Chờ Admin duyệt (Visa Card)"
                        : "Chờ Admin duyệt (Tiền mặt)");

                // 3. Create DangKyGoi (Hóa đơn) in SQL DB
                var order = new DangKyGoiEntity
                {
                    MaHD = "HD-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    MaND = existingUser.MaND,
                    MaCN = defaultCN.MaCN,
                    MaGoi = goiId,
                    NgayDK = DateTime.Now,
                    NgayBatDau = DateTime.Now,
                    NgayKetThuc = DateTime.Now.AddMonths(durationMonths),
                    TongTien = price,
                    TrangThai = statusText
                };

                _context.DangKyGois.Add(order);
                _context.SaveChanges();

                // 4. Record Transaction
                int ptId = payMethod.Contains("Momo") ? 3 : (payMethod.Contains("Visa") ? 2 : 1);
                var transaction = new GiaoDichThanhToanEntity
                {
                    MaDK = order.MaDK,
                    MaPT_ThanhToan = ptId,
                    SoTien = price,
                    ThoiGianThanhToan = DateTime.Now
                };
                _context.GiaoDichThanhToans.Add(transaction);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Lỗi lưu đăng ký gói vào SQL Server DB.");
            }
        }
    }
}
