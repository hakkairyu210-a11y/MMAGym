using System;

namespace MMA.Models
{
    public class UserModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = "Member"; // "Admin", "Trainer", "Member"
        public string AvatarUrl { get; set; } = "https://fitbox.vn/wp-content/uploads/2026/05/HOI-VIEN-SY-NGUYEN-150x150.png";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Hoạt động";
    }
}
