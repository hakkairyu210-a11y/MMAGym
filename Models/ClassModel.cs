namespace MMA.Models
{
    public class ClassModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ScheduleTime { get; set; } = string.Empty;
        public string TrainerName { get; set; } = string.Empty;
        public string CaloriesBurned { get; set; } = "600-900 kcal/buổi";
        public string TargetAudience { get; set; } = "Phù hợp mọi lứa tuổi & trình độ";
        public string DetailedBenefits { get; set; } = "Tăng cường sức bền, giảm mỡ nhanh chóng và rèn luyện phản xạ.";
    }
}
