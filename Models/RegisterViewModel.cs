using System.ComponentModel.DataAnnotations;

namespace MMA.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên của bạn.")]
        [Display(Name = "Họ tên")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ Email không hợp lệ.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Độ tuổi")]
        public string Age { get; set; } = string.Empty;

        [Display(Name = "Loại đăng ký")]
        public string RegistrationType { get; set; } = "Gói Tập Membership";

        [Display(Name = "Bộ môn / Gói tập đăng ký")]
        public string SelectedClass { get; set; } = string.Empty;

        [Display(Name = "Hình thức thanh toán")]
        public string PaymentMethod { get; set; } = "TienMat"; // TienMat, MomoOnline, VisaCard

        [Display(Name = "Huấn luyện viên phụ trách")]
        public string SelectedTrainer { get; set; } = "Văn Bá Huyên (Senior Coach)";

        [Display(Name = "Ca học / Lịch tập đăng ký")]
        public string SelectedScheduleSlot { get; set; } = "Thứ 2, 4, 6 - Ca 18:00 PM";

        [Display(Name = "Giới tính")]
        public string Gender { get; set; } = "Nam";

        [Display(Name = "Mã Đơn")]
        public int OrderId { get; set; }

        [Display(Name = "Mã Hóa Đơn")]
        public string InvoiceCode { get; set; } = string.Empty;

        [Display(Name = "Chi nhánh")]
        public string BranchName { get; set; } = "MMA - Dương Quang Đông (Q8)";

        [Display(Name = "Huấn luyện viên")]
        public string TrainerName { get; set; } = "Văn Bá Huyên (Senior Coach)";

        [Display(Name = "Tổng tiền")]
        public decimal Amount { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Chưa thanh toán";

        public bool IsApproved { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = "123456";
    }
}
