using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLLBusiness_Logic.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Org.BouncyCastle.Security;

namespace BucketProject.UI.BucketProject.Controllers
{
    public class StatsController : Controller
    {
        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet]
        public IActionResult Stats()
        {
            List<StatsDTO> typeStats = _statsService.GetGoalTypeStatistics();
            List<StatsDTO> categoryStats = _statsService.GetGoalCategoryStatistics();
            List<StatsDTO> stackedStats = _statsService.GetGoalAmountStatistics();

            ViewBag.TypeLabels = typeStats.Select(s => s.Type).ToList();
            ViewBag.TypeData = typeStats.Select(s => s.Count).ToList();

            ViewBag.CategoryLabels = categoryStats.Select(s => s.Category).ToList();
            ViewBag.CategoryData = categoryStats.Select(s => s.Count).ToList();

            ViewBag.StackedLabels = stackedStats.Select(s => s.Ownership).ToList();  
            ViewBag.CompletedData = stackedStats.Select(s => s.Completed).ToList();     
            ViewBag.IncompleteData = stackedStats.Select(s => s.Incomplete).ToList();

            return View();
        }

    }
}
