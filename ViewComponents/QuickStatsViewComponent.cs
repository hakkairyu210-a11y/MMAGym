using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MMA.ViewComponents
{
    public class QuickStatsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var stats = new QuickStatsModel
            {
                ActiveMembers = 1250,
                TotalBranches = 6,
                TotalTrainers = 13,
                SatisfiedRate = 98.5
            };
            return await Task.FromResult(View(stats));
        }
    }

    public class QuickStatsModel
    {
        public int ActiveMembers { get; set; }
        public int TotalBranches { get; set; }
        public int TotalTrainers { get; set; }
        public double SatisfiedRate { get; set; }
    }
}
