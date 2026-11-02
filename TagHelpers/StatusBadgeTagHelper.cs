using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MMA.TagHelpers
{
    [HtmlTargetElement("status-badge")]
    public class StatusBadgeTagHelper : TagHelper
    {
        public string Status { get; set; } = "Active";
        public string Text { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            
            string badgeClass = Status.ToLower() switch
            {
                "active" or "hoạt động" or "đang mở" => "bg-success bg-opacity-25 text-success border border-success border-opacity-50",
                "pending" or "chờ xử lý" or "chờ gọi điện" => "bg-warning bg-opacity-25 text-warning border border-warning border-opacity-50",
                "inactive" or "đã khóa" or "hết chỗ" => "bg-danger bg-opacity-25 text-danger border border-danger border-opacity-50",
                _ => "bg-secondary text-white"
            };

            string displayText = string.IsNullOrEmpty(Text) ? Status : Text;

            output.Attributes.SetAttribute("class", $"badge px-2.5 py-1 text-xs fw-bold rounded-2 {badgeClass}");
            output.Content.SetContent(displayText);
        }
    }
}
