using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MMA.Pages
{
    public class FAQModel : PageModel
    {
        public List<FAQItem> FAQList { get; set; } = new();

        public void OnGet()
        {
            FAQList = new List<FAQItem>
            {
                new FAQItem { Question = "Phòng tập MMA gồm những bộ môn nào?", Answer = "MMA Sài Gòn tích hợp toàn diện: Gym, Boxing, Muay Thái, Kickboxing, Yoga, Dance, Zumba & EMS." },
                new FAQItem { Question = "Người mới chưa từng tập võ có tham gia lớp Boxing/Muay Thái được không?", Answer = "Hoàn toàn được! Các HLV Quốc Gia sẽ hướng dẫn lộ trình cơ bản 1-1 từ tư thế đứng, đấm đến di chuyển cho người mới." },
                new FAQItem { Question = "Có được tập thử miễn phí trước khi đăng ký chính thức không?", Answer = "Có, bạn được đăng ký 01 buổi tập thử miễn phí và đo chỉ số cơ thể InBody hoàn toàn không mất phí." },
                new FAQItem { Question = "Giờ mở cửa của các chi nhánh MMA là khi nào?", Answer = "Tất cả 6 chi nhánh MMA mở cửa liên tục từ 5:30 sáng đến 22:00 tối tất cả các ngày trong tuần." }
            };
        }
    }

    public class FAQItem
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }
}
