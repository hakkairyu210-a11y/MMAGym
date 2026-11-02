using System.ComponentModel.DataAnnotations;

namespace MMA.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Email hoặc Tên đăng nhập.")]
        [Display(Name = "Tên đăng nhập / Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }
    }
}
