using Microsoft.EntityFrameworkCore;
using MMA.Models.Entities;

namespace MMA.Data
{
    public class MmaDbContext : DbContext
    {
        public MmaDbContext(DbContextOptions<MmaDbContext> options) : base(options)
        {
        }

        public DbSet<ChiNhanhEntity> ChiNhanhs { get; set; } = null!;
        public DbSet<VaiTroEntity> VaiTros { get; set; } = null!;
        public DbSet<NguoiDungEntity> NguoiDungs { get; set; } = null!;
        public DbSet<BoMonEntity> BoMons { get; set; } = null!;
        public DbSet<GoiTapEntity> GoiTaps { get; set; } = null!;
        public DbSet<PhuongThucThanhToanEntity> PhuongThucThanhToans { get; set; } = null!;
        public DbSet<DangKyGoiEntity> DangKyGois { get; set; } = null!;
        public DbSet<GiaoDichThanhToanEntity> GiaoDichThanhToans { get; set; } = null!;
        public DbSet<LichTapEntity> LichTaps { get; set; } = null!;
        public DbSet<HuanLuyenVienEntity> HuanLuyenViens { get; set; } = null!;
        public DbSet<MaGiamGiaEntity> MaGiamGias { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Unique Indexes matching SQL Schema
            modelBuilder.Entity<VaiTroEntity>()
                .HasIndex(v => v.TenVaiTro)
                .IsUnique();

            modelBuilder.Entity<NguoiDungEntity>()
                .HasIndex(n => n.Email)
                .IsUnique();

            modelBuilder.Entity<PhuongThucThanhToanEntity>()
                .HasIndex(p => p.TenPhuongThuc)
                .IsUnique();

            modelBuilder.Entity<DangKyGoiEntity>()
                .HasIndex(d => d.MaHD)
                .IsUnique();

            modelBuilder.Entity<MaGiamGiaEntity>()
                .HasIndex(m => m.MaCode)
                .IsUnique();
        }
    }
}
