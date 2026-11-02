using System.Collections.Generic;

namespace MMA.Models
{
    public class HomeViewModel
    {
        public RegisterViewModel RegisterForm { get; set; } = new();
        public List<ClassModel> FeaturedClasses { get; set; } = new();
        public List<TestimonialModel> Testimonials { get; set; } = new();
        public List<TrainerModel> FeaturedTrainers { get; set; } = new();
        public List<MembershipPlanModel> MembershipPlans { get; set; } = new();
        public List<ClubModel> Clubs { get; set; } = new();
    }
}
