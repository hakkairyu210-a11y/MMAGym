using System.Collections.Generic;

namespace MMA.Models
{
    public class ScheduleSlotModel
    {
        public string TimeSlot { get; set; } = string.Empty;
        public List<ScheduleItemModel> Items { get; set; } = new();
    }

    public class ScheduleItemModel
    {
        public string Day { get; set; } = string.Empty; // "MON", "TUE", etc.
        public string ClassName { get; set; } = string.Empty;
        public string ClassType { get; set; } = string.Empty; // "fitness", "boxing", etc.
        public string TrainerName { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
    }
}
