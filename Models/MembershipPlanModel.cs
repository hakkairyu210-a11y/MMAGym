using System.Collections.Generic;

namespace MMA.Models
{
    public class MembershipPlanModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty; // "Gói 3 Tháng", "Gói 6 Tháng", "Gói 12 Tháng (1 Năm)"
        public string DurationText { get; set; } = string.Empty;
        public decimal OriginalPrice { get; set; }
        public decimal Price { get; set; }
        public string PerMonthPrice { get; set; } = string.Empty;
        public bool IsPopular { get; set; }
        public string PopularBadgeText { get; set; } = "BÁN CHẠY NHẤT";
        public List<string> Features { get; set; } = new();
    }
}
