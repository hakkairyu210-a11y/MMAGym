using System.Collections.Generic;

namespace MMA.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalMembers { get; set; } = 1250;
        public int TotalClasses { get; set; } = 24;
        public int TotalTrainers { get; set; } = 18;
        public decimal MonthlyRevenue { get; set; } = 385000000;
        public int NewRegistrationsCount { get; set; } = 48;

        public List<UserModel> RecentMembers { get; set; } = new();
        public List<RegisterViewModel> PendingRegistrations { get; set; } = new();
        public List<ClassModel> TopClasses { get; set; } = new();
    }
}
