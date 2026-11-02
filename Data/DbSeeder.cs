using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MMA.Models.Entities;

namespace MMA.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(MmaDbContext context)
        {
            try
            {
                // Ensure database created
                await context.Database.EnsureCreatedAsync();

                // 1. Seed VaiTro
                if (!await context.VaiTros.AnyAsync())
                {
                    context.VaiTros.AddRange(
                        new VaiTroEntity { TenVaiTro = "Admin", MoTa = "Quản trị hệ thống" },
                        new VaiTroEntity { TenVaiTro = "PT", MoTa = "Huấn luyện viên cá nhân" },
                        new VaiTroEntity { TenVaiTro = "Học viên", MoTa = "Khách hàng / Hội viên" }
                    );
                    await context.SaveChangesAsync();
                }

                // 2. Seed PhuongThucThanhToan
                if (!await context.PhuongThucThanhToans.AnyAsync())
                {
                    context.PhuongThucThanhToans.AddRange(
                        new PhuongThucThanhToanEntity { TenPhuongThuc = "Tiền mặt" },
                        new PhuongThucThanhToanEntity { TenPhuongThuc = "Thẻ ATM/Visa" },
                        new PhuongThucThanhToanEntity { TenPhuongThuc = "Ví Momo" }
                    );
                    await context.SaveChangesAsync();
                }

                // 3. Seed ChiNhanh
                if (!await context.ChiNhanhs.AnyAsync())
                {
                    context.ChiNhanhs.AddRange(
                        new ChiNhanhEntity { TenCN = "MMA Rạch Miễu", DiaChi = "Nhà thi đấu Rạch Miễu, 1 Đ. Hoa Phượng, Phú Nhuận, TP.HCM", Hotline = "0947054075" },
                        new ChiNhanhEntity { TenCN = "MMA Dương Quang Đông", DiaChi = "23 Dương Quang Đông, Phường Chánh Hưng, Q8, TP.HCM", Hotline = "0947054075" },
                        new ChiNhanhEntity { TenCN = "MMA Topaz City", DiaChi = "Topaz City Block B2, 39 Cao Lỗ, Chánh Hưng, Q8, TP.HCM", Hotline = "0947054075" },
                        new ChiNhanhEntity { TenCN = "MMA Lý Chiêu Hoàng", DiaChi = "105 Đ. Lý Chiêu Hoàng, Khu V, Bình Phú, Q6, TP.HCM", Hotline = "0947054075" }
                    );
                    await context.SaveChangesAsync();
                }

                // 4. Seed BoMon
                if (!await context.BoMons.AnyAsync())
                {
                    context.BoMons.AddRange(
                        new BoMonEntity { TenBoMon = "MMA", MoTa = "Võ thuật tự do tổng hợp" },
                        new BoMonEntity { TenBoMon = "Boxing", MoTa = "Boxing quyền anh chuyên sâu" },
                        new BoMonEntity { TenBoMon = "Muay Thái", MoTa = "Võ thuật cổ truyền Thái Lan" },
                        new BoMonEntity { TenBoMon = "Gym & Fitness", MoTa = "Thể hình và rèn luyện thể lực" },
                        new BoMonEntity { TenBoMon = "Yoga", MoTa = "Yoga Ấn Độ & Yoga trị liệu" }
                    );
                    await context.SaveChangesAsync();
                }

                // 5. Seed GoiTap (Ensure exact 3 clean packages: Gói 3 Tháng, Gói 6 Tháng, Gói 1 Năm)
                var currentPackages = await context.GoiTaps.OrderBy(g => g.MaGoi).ToListAsync();
                if (!currentPackages.Any())
                {
                    var mmaBoMon = await context.BoMons.FirstOrDefaultAsync(b => b.TenBoMon == "MMA");
                    var gymBoMon = await context.BoMons.FirstOrDefaultAsync(b => b.TenBoMon == "Gym & Fitness");
                    int mmaId = mmaBoMon?.MaBoMon ?? 1;
                    int gymId = gymBoMon?.MaBoMon ?? 4;

                    context.GoiTaps.AddRange(
                        new GoiTapEntity { TenGoi = "Gói 3 Tháng", MaBoMon = gymId, ThoiHanThang = 3, GiaTien = 2990000, MoTa = "Tập không giới hạn 3 tháng tất cả bộ môn Gym, Boxing, MMA." },
                        new GoiTapEntity { TenGoi = "Gói 6 Tháng", MaBoMon = gymId, ThoiHanThang = 6, GiaTien = 4990000, MoTa = "Tập không giới hạn 6 tháng + Tặng găng Boxing chuyên nghiệp." },
                        new GoiTapEntity { TenGoi = "Gói 1 Năm", MaBoMon = mmaId, ThoiHanThang = 12, GiaTien = 7990000, MoTa = "Gói VIP 1 Năm không giới hạn + Tặng bộ bảo hộ võ thuật cao cấp." }
                    );
                    await context.SaveChangesAsync();
                }
                else
                {
                    if (currentPackages.Count >= 1) { currentPackages[0].TenGoi = "Gói 3 Tháng"; currentPackages[0].ThoiHanThang = 3; currentPackages[0].GiaTien = 2990000; }
                    if (currentPackages.Count >= 2) { currentPackages[1].TenGoi = "Gói 6 Tháng"; currentPackages[1].ThoiHanThang = 6; currentPackages[1].GiaTien = 4990000; }
                    if (currentPackages.Count >= 3) { currentPackages[2].TenGoi = "Gói 1 Năm"; currentPackages[2].ThoiHanThang = 12; currentPackages[2].GiaTien = 7990000; }
                    try { await context.SaveChangesAsync(); } catch (Exception) { }
                }

                // 6. Seed NguoiDung (Default Admin & Member)
                if (!await context.NguoiDungs.AnyAsync(u => u.Email == "admin" || u.Email == "admin@mma.vn"))
                {
                    var adminRole = await context.VaiTros.FirstOrDefaultAsync(r => r.TenVaiTro == "Admin");
                    var memberRole = await context.VaiTros.FirstOrDefaultAsync(r => r.TenVaiTro == "Học viên");

                    context.NguoiDungs.AddRange(
                        new NguoiDungEntity { HoTen = "admin", Email = "admin", SoDT = "0947054075", PasswordHash = "123456", MaVaiTro = adminRole?.MaVaiTro ?? 1 },
                        new NguoiDungEntity { HoTen = "Quản Trị Viên MMA", Email = "admin@mma.vn", SoDT = "0900000001", PasswordHash = "123456", MaVaiTro = adminRole?.MaVaiTro ?? 1 },
                        new NguoiDungEntity { HoTen = "Nguyễn Hùng Mạnh", Email = "nguyenmanhhdz06@gmail.com", SoDT = "0947054075", PasswordHash = "123456", MaVaiTro = memberRole?.MaVaiTro ?? 3 }
                    );
                    await context.SaveChangesAsync();
                }

                // 7. Seed HuanLuyenVien
                if (!await context.HuanLuyenViens.AnyAsync())
                {
                    context.HuanLuyenViens.AddRange(
                        new HuanLuyenVienEntity { HoTen = "Văn Bá Huyên", SoDT = "0901234567", Email = "bahuyen@mma.vn", ChuyenMon = "MMA Thực Chiến", KinhNghiem = "8 năm thi đấu & huấn luyện MMA", HinhAnh = "/images/trainers/trainer1.jpg" },
                        new HuanLuyenVienEntity { HoTen = "Nguyễn Văn Khoa", SoDT = "0902345678", Email = "vankhoa@mma.vn", ChuyenMon = "Boxing Chuyên Sâu", KinhNghiem = "6 năm HLV Boxing chuyên nghiệp", HinhAnh = "/images/trainers/trainer2.jpg" },
                        new HuanLuyenVienEntity { HoTen = "Phạm Xuân Quốc Huy", SoDT = "0903456789", Email = "quochuy@mma.vn", ChuyenMon = "Yoga & Thể Lực", KinhNghiem = "5 năm HLV Yoga Ấn Độ", HinhAnh = "/images/trainers/trainer3.jpg" }
                    );
                    await context.SaveChangesAsync();
                }

                // 8. Seed MaGiamGia
                if (!await context.MaGiamGias.AnyAsync())
                {
                    context.MaGiamGias.AddRange(
                        new MaGiamGiaEntity { MaCode = "MMAGYM10", GiaTriGiam = 10, LoaiGiamGia = "PhanTram", SoLuong = 100, NgayBatDau = DateTime.Now.AddDays(-10), NgayKetThuc = DateTime.Now.AddDays(60) },
                        new MaGiamGiaEntity { MaCode = "WELCOME500K", GiaTriGiam = 500000, LoaiGiamGia = "SoTien", SoLuong = 50, NgayBatDau = DateTime.Now.AddDays(-5), NgayKetThuc = DateTime.Now.AddDays(30) }
                    );
                    await context.SaveChangesAsync();
                }

                // 9. Seed LichTap
                try
                {
                    if (!await context.LichTaps.AnyAsync())
                    {
                        var hlvHuyen = await context.HuanLuyenViens.FirstOrDefaultAsync(h => h.HoTen.Contains("Huyên"));
                        var hlvKhoa = await context.HuanLuyenViens.FirstOrDefaultAsync(h => h.HoTen.Contains("Khoa"));
                        var hlvHuy = await context.HuanLuyenViens.FirstOrDefaultAsync(h => h.HoTen.Contains("Huy"));

                        context.LichTaps.AddRange(
                            new LichTapEntity { MaCN = 1, ThuTrongTuan = "MON", KhungGio = "09:00 AM", TenLop = "Fitness & Body Building", TenHLV = "HLV Thể Lực", ClassType = "fitness", ColorClass = "fitness-slot", ThoiGianChiTiet = "09:00 - 10:00" },
                            new LichTapEntity { MaCN = 1, ThuTrongTuan = "MON", KhungGio = "09:00 AM", TenLop = "Yoga Ấn Độ", TenHLV = "Phạm Xuân Quốc Huy", MaHLV = hlvHuy?.MaHLV, ClassType = "yoga-an", ColorClass = "yoga-an-slot", ThoiGianChiTiet = "09:00 - 10:00" },
                            new LichTapEntity { MaCN = 1, ThuTrongTuan = "TUE", KhungGio = "09:00 AM", TenLop = "Boxing Chuyên Sâu", TenHLV = "Nguyễn Văn Khoa", MaHLV = hlvKhoa?.MaHLV, ClassType = "boxing", ColorClass = "boxing-slot", ThoiGianChiTiet = "09:00 - 10:00" },
                            new LichTapEntity { MaCN = 1, ThuTrongTuan = "WED", KhungGio = "09:00 AM", TenLop = "MMA Thực Chiến", TenHLV = "Văn Bá Huyên", MaHLV = hlvHuyen?.MaHLV, ClassType = "mma", ColorClass = "mma-slot", ThoiGianChiTiet = "09:00 - 10:30" },
                            new LichTapEntity { MaCN = 1, ThuTrongTuan = "FRI", KhungGio = "06:00 PM", TenLop = "MMA Thực Chiến", TenHLV = "Văn Bá Huyên", MaHLV = hlvHuyen?.MaHLV, ClassType = "mma", ColorClass = "mma-slot", ThoiGianChiTiet = "18:00 - 19:30" },
                            new LichTapEntity { MaCN = 1, ThuTrongTuan = "SAT", KhungGio = "09:00 AM", TenLop = "Zumba Dance", TenHLV = "Dương Hữu Trường", ClassType = "zumba", ColorClass = "zumba-slot", ThoiGianChiTiet = "09:00 - 10:00" }
                        );
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception) { }
            }
            catch (Exception ex)
            {
                // Log or handle initial migration warning gracefully
                Console.WriteLine("DbSeeder notice: " + ex.Message);
            }
        }
    }
}
