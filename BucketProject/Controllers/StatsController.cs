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


            List<StatsDTO> weeklyStats = _statsService.GetCompletedGoalsPerWeek();

            ViewBag.WeekLabels = weeklyStats.Select(s => s.Period).ToList();
            ViewBag.WeekData = weeklyStats.Select(s => s.Count).ToList();
            ViewBag.WeekMessage = _statsService.GetWeeklyCompletionRateMessageWeek();


            List<StatsDTO> monthlyStats = _statsService.GetCompletedGoalsPerMonth();

            ViewBag.MonthLabels = monthlyStats.Select(s => s.Period).ToList();
            ViewBag.MonthData = monthlyStats.Select(s => s.Count).ToList();
            ViewBag.MonthMessage = _statsService.GetCompletionRateMessageMonth();


            List<StatsDTO> yearlyStats = _statsService.GetCompletedGoalsPerYear();

            ViewBag.YearLabels = yearlyStats.Select(s => s.Period).ToList();
            ViewBag.YearData = yearlyStats.Select(s => s.Count).ToList();
            ViewBag.YearMessage = _statsService.GetYearlyCompletionRateMessage();

            double avgDays = _statsService.GetAverageCompletionTimeInDays();
            ViewBag.AverageCompletionDays = Math.Round(avgDays, 1);

            StatsDTO summary = _statsService.GetGoalSummaryStats();

            ViewBag.TotalGoals = summary.TotalGoals;
            ViewBag.CompletedGoals = summary.CompletedGoals;
            ViewBag.IncompleteGoals = summary.IncompleteGoals;
            ViewBag.PersonalGoals = summary.PersonalGoals;
            ViewBag.SharedGoals = summary.SharedGoals;
            ViewBag.PostponedGoals = summary.PostponedGoals;
            ViewBag.AIGoals = summary.AIGoals;


            List<StatsDTO> stats = _statsService.GetUserRegistrationsPerMonth();
            ViewBag.Labels = stats.Select(s => s.Period).ToList();
            ViewBag.Data = stats.Select(s => s.Count).ToList();

            return View();
        }


    }
}
