using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.Models.Entities
{
    [Table("ChiNhanh")]
    public class ChiNhanhEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaCN { get; set; }

        [Required]
        [StringLength(100)]
        public string TenCN { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string DiaChi { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string Hotline { get; set; } = string.Empty;

        [StringLength(500)]
        public string? HinhAnh { get; set; }

        public bool TrangThai { get; set; } = true;
    }

    [Table("VaiTro")]
    public class VaiTroEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaVaiTro { get; set; }

        [Required]
        [StringLength(50)]
        public string TenVaiTro { get; set; } = string.Empty;

        [StringLength(255)]
        public string? MoTa { get; set; }
    }

    [Table("NguoiDung")]
    public class NguoiDungEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaND { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string SoDT { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public int MaVaiTro { get; set; }
        public int? MaCN_TrucThuoc { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public bool TrangThai { get; set; } = true;

        [ForeignKey("MaVaiTro")]
        public virtual VaiTroEntity? VaiTro { get; set; }

        [ForeignKey("MaCN_TrucThuoc")]
        public virtual ChiNhanhEntity? ChiNhanh { get; set; }
    }

    [Table("BoMon")]
    public class BoMonEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaBoMon { get; set; }

        [Required]
        [StringLength(50)]
        public string TenBoMon { get; set; } = string.Empty;

        public string? MoTa { get; set; }
    }

    [Table("GoiTap")]
    public class GoiTapEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaGoi { get; set; }

        [Required]
        [StringLength(150)]
        public string TenGoi { get; set; } = string.Empty;

        public int MaBoMon { get; set; }
        public int ThoiHanThang { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaTien { get; set; }

        public string? MoTa { get; set; }
        public bool TrangThai { get; set; } = true;

        [ForeignKey("MaBoMon")]
        public virtual BoMonEntity? BoMon { get; set; }
    }

    [Table("HuanLuyenVien")]
    public class HuanLuyenVienEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaHLV { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [StringLength(15)]
        public string? SoDT { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? ChuyenMon { get; set; }

        [StringLength(255)]
        public string? KinhNghiem { get; set; }

        [StringLength(500)]
        public string? HinhAnh { get; set; }
    }

    [Table("MaGiamGia")]
    public class MaGiamGiaEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaKhuyenMai { get; set; }

        [Required]
        [StringLength(50)]
        public string MaCode { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaTriGiam { get; set; }

        [StringLength(50)]
        public string LoaiGiamGia { get; set; } = "SoTien"; // "SoTien", "PhanTram"

        public int SoLuong { get; set; } = 0;

        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
    }

    [Table("PhuongThucThanhToan")]
    public class PhuongThucThanhToanEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaPT { get; set; }

        [Required]
        [StringLength(50)]
        public string TenPhuongThuc { get; set; } = string.Empty;
    }

    [Table("DangKyGoi")]
    public class DangKyGoiEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaDK { get; set; }

        [Required]
        [StringLength(20)]
        public string MaHD { get; set; } = string.Empty;

        public int MaND { get; set; }
        public int MaCN { get; set; }
        public int MaGoi { get; set; }
        public int? MaKhuyenMai { get; set; }

        public DateTime NgayDK { get; set; } = DateTime.Now;
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; } = "Chưa thanh toán";

        [ForeignKey("MaND")]
        public virtual NguoiDungEntity? NguoiDung { get; set; }

        [ForeignKey("MaCN")]
        public virtual ChiNhanhEntity? ChiNhanh { get; set; }

        [ForeignKey("MaGoi")]
        public virtual GoiTapEntity? GoiTap { get; set; }

        [ForeignKey("MaKhuyenMai")]
        public virtual MaGiamGiaEntity? MaGiamGia { get; set; }
    }

    [Table("GiaoDichThanhToan")]
    public class GiaoDichThanhToanEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaGD { get; set; }

        public int MaDK { get; set; }
        public int MaPT_ThanhToan { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SoTien { get; set; }

        public DateTime ThoiGianThanhToan { get; set; } = DateTime.Now;

        [ForeignKey("MaDK")]
        public virtual DangKyGoiEntity? DangKyGoi { get; set; }

        [ForeignKey("MaPT_ThanhToan")]
        public virtual PhuongThucThanhToanEntity? PhuongThucThanhToan { get; set; }
    }
}
