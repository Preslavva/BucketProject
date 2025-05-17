using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLLBusiness_Logic.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BucketProject.Controllers
{
    public class ManagerController : Controller
    {

        private readonly IStatsService _statsService;

        public ManagerController(IStatsService statsService)
        {
            _statsService = statsService;
        }
        public IActionResult Manager()
        {
            var userStats = _statsService.GetUserRegistrationsPerMonth();
            ViewBag.UserLabels = userStats.Select(s => s.Period).ToList();
            ViewBag.UserData = userStats.Select(s => s.Count).ToList();

            var goalStats = _statsService.GetGoalsPerMonth();
            ViewBag.GoalLabels = goalStats.Select(s => s.Period).ToList();
            ViewBag.GoalData = goalStats.Select(s => s.Count).ToList();

            var summary = _statsService.GetGoalSummaryStatsManager();

            ViewBag.TotalGoals = summary.TotalGoals;
            ViewBag.PersonalGoals = summary.PersonalGoals;
            ViewBag.SharedGoals = summary.SharedGoals;
            ViewBag.PostponedGoals = summary.PostponedGoals;
            ViewBag.AIGoals = summary.AIGoals;
            ViewBag.ActiveUsersCount = summary.ActiveUsersCount;

            List<StatsDTO> typeStats = _statsService.GetGoalTypeStatisticsManager();
            List<StatsDTO> categoryStats = _statsService.GetGoalCategoryStatisticsManager();

            ViewBag.TypeLabels = typeStats.Select(s => s.Type).ToList();
            ViewBag.TypeData = typeStats.Select(s => s.Count).ToList();

            ViewBag.CategoryLabels = categoryStats.Select(s => s.Category).ToList();
            ViewBag.CategoryData = categoryStats.Select(s => s.Count).ToList();

            return View();
        }

    }
}
