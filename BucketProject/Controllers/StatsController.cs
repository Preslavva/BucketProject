using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLL.Business_Logic.DTOs;
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

            ViewBag.TypeLabels = typeStats.Select(s => s.Type).ToList();
            ViewBag.TypeData = typeStats.Select(s => s.Count).ToList();

            ViewBag.CategoryLabels = categoryStats.Select(s => s.Category).ToList();
            ViewBag.CategoryData = categoryStats.Select(s => s.Count).ToList();


            var weeklyStats = _statsService.GetGoalAmountStatisticsWeekly();

            ViewBag.WeeklyPeriods = weeklyStats.Select(s => s.Period).Distinct().ToList();

            ViewBag.WeeklyPersonalCompleted = weeklyStats
                .Where(s => s.Ownership == "Personal")
                .Select(s => s.Completed).ToList();

            ViewBag.WeeklyPersonalIncomplete = weeklyStats
                .Where(s => s.Ownership == "Personal")
                .Select(s => s.Incomplete).ToList();

            ViewBag.WeeklySharedCompleted = weeklyStats
                .Where(s => s.Ownership == "Shared")
                .Select(s => s.Completed).ToList();

            ViewBag.WeeklySharedIncomplete = weeklyStats
                .Where(s => s.Ownership == "Shared")
                .Select(s => s.Incomplete).ToList();


            var monthlyStats = _statsService.GetGoalAmountStatisticsMonthly();

            ViewBag.MonthlyPeriods = monthlyStats.Select(s => s.Period).Distinct().ToList();

            ViewBag.MonthlyPersonalCompleted = monthlyStats
                .Where(s => s.Ownership == "Personal")
                .Select(s => s.Completed).ToList();

            ViewBag.MonthlyPersonalIncomplete = monthlyStats
                .Where(s => s.Ownership == "Personal")
                .Select(s => s.Incomplete).ToList();

            ViewBag.MonthlySharedCompleted = monthlyStats
                .Where(s => s.Ownership == "Shared")
                .Select(s => s.Completed).ToList();

            ViewBag.MonthlySharedIncomplete = monthlyStats
                .Where(s => s.Ownership == "Shared")
                .Select(s => s.Incomplete).ToList();

            var yearlyStats = _statsService.GetGoalAmountStatisticsYearly();

            ViewBag.YearlyPeriods = yearlyStats.Select(s => s.Period).Distinct().ToList();

            ViewBag.YearlyPersonalCompleted = yearlyStats
                .Where(s => s.Ownership == "Personal")
                .Select(s => s.Completed).ToList();

            ViewBag.YearlyPersonalIncomplete = yearlyStats
                .Where(s => s.Ownership == "Personal")
                .Select(s => s.Incomplete).ToList();

            ViewBag.YearlySharedCompleted = yearlyStats
                .Where(s => s.Ownership == "Shared")
                .Select(s => s.Completed).ToList();

            ViewBag.YearlySharedIncomplete = yearlyStats
                .Where(s => s.Ownership == "Shared")
                .Select(s => s.Incomplete).ToList();


            double avgDays = _statsService.GetAverageCompletionTimeInDays();
            ViewBag.AverageCompletionDays = Math.Round(avgDays, 1);

            var summary = _statsService.GetGoalSummaryStats();

            ViewBag.TotalGoals = summary.TotalGoals;
            ViewBag.CompletedGoals = summary.CompletedGoals;
            ViewBag.IncompleteGoals = summary.IncompleteGoals;
            ViewBag.PersonalGoals = summary.PersonalGoals;
            ViewBag.SharedGoals = summary.SharedGoals;
            ViewBag.PostponedGoals = summary.PostponedGoals;
            ViewBag.AIGoals = summary.AIGoals;


            var stats = _statsService.GetUserRegistrationsPerMonth();
            ViewBag.Labels = stats.Select(s => s.Period).ToList();
            ViewBag.Data = stats.Select(s => s.Count).ToList();

            return View();
        }


    }
}
