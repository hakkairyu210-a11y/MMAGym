namespace MMA.Models
{
    public class TrainerModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string FacebookUrl { get; set; } = string.Empty;
        public string InstagramUrl { get; set; } = string.Empty;
        public string TwitterUrl { get; set; } = string.Empty;
        public string LinkedinUrl { get; set; } = string.Empty;
        public int ExperienceYears { get; set; } = 8;
        public string Certificates { get; set; } = "Bằng HLV Quốc Gia & Chứng chỉ Quốc Tế";
        public string Bio { get; set; } = "Huấn luyện viên giàu kinh nghiệm, đào tạo thành công hơn 500+ hội viên đạt vóc dáng và kỹ năng võ thuật đỉnh cao.";
        public string Specialties { get; set; } = "MMA, Boxing, Muay Thái & Kickboxing";
    }
}
