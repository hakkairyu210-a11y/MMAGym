using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.Models.Entities
{
    [Table("LichTap")]
    public class LichTapEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaLich { get; set; }

        [Required]
        public int MaCN { get; set; }

        [Required]
        [StringLength(20)]
        public string ThuTrongTuan { get; set; } = "MON"; // MON, TUE, WED, THUR, FRI, SAT, SUN

        [Required]
        [StringLength(50)]
        public string KhungGio { get; set; } = "09:00 AM"; // 09:00 AM, 06:00 PM

        [Required]
        [StringLength(100)]
        public string TenLop { get; set; } = "MMA Thực Chiến";

        [Required]
        [StringLength(100)]
        public string TenHLV { get; set; } = "Văn Bá Huyên";

        [StringLength(50)]
        public string ClassType { get; set; } = "mma";

        [StringLength(50)]
        public string ColorClass { get; set; } = "mma-slot";

        [StringLength(50)]
        public string ThoiGianChiTiet { get; set; } = "18:00 - 19:30";

        public int? MaHLV { get; set; }

        [ForeignKey("MaCN")]
        public virtual ChiNhanhEntity? ChiNhanh { get; set; }

        [ForeignKey("MaHLV")]
        public virtual HuanLuyenVienEntity? HuanLuyenVien { get; set; }
    }
}