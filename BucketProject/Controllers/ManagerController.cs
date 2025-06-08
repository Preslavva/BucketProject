using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLL.Business_Logic.DTOs;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace BucketProject.Controllers
{
    public class ManagerController : Controller
    {

        private readonly IStatsService _statsService;
        private readonly IMapper _mapper;

        public ManagerController(IStatsService statsService, IMapper mapper)
        {
            _statsService = statsService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult ManagerSearch()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Manager")
                return RedirectToAction("LogIn", "User");

            ViewBag.Genders = _statsService.GetAllGenders();
            ViewBag.Nationalities = _statsService.GetAllNationalities();

            return View("ManagerSearch"); 
        }

        [HttpGet]
        public IActionResult ManagerStats()
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role != "Manager")
                return RedirectToAction("LogIn", "User");

            List<StatsDTO> userStats = _statsService.GetUserRegistrationsPerMonth();
            ViewBag.UserLabels = userStats.Select(s => s.Period).ToList();
            ViewBag.UserData = userStats.Select(s => s.Count).ToList();
            ViewBag.UserMessage = _statsService.GetMonthlyUserRegistrationMessage();

            List<StatsDTO> goalStats = _statsService.GetGoalsPerMonth();
            ViewBag.GoalLabels = goalStats.Select(s => s.Period).ToList();
            ViewBag.GoalData = goalStats.Select(s => s.Count).ToList();
            ViewBag.GoalMessage = _statsService.GetMonthlyGoalCreationMessage();

            StatsDTO summary = _statsService.GetGoalSummaryStatsManager();
            ViewBag.TotalGoals = summary.TotalGoals;
            ViewBag.PersonalGoals = summary.PersonalGoals;
            ViewBag.SharedGoals = summary.SharedGoals;
            ViewBag.PostponedGoals = summary.PostponedGoals;
            ViewBag.AIGoals = summary.AIGoals;
            ViewBag.ActiveUsersCount = summary.ActiveUsersCount;

            List<StatsDTO> typeStats = _statsService.GetGoalTypeStatisticsManager();
            ViewBag.TypeLabels = typeStats.Select(s => s.Type).ToList();
            ViewBag.TypeData = typeStats.Select(s => s.Count).ToList();

            List<StatsDTO> categoryStats = _statsService.GetGoalCategoryStatisticsManager();
            ViewBag.CategoryLabels = categoryStats.Select(s => s.Category).ToList();
            ViewBag.CategoryData = categoryStats.Select(s => s.Count).ToList();

            List<StatsDTO> nationalityStats = _statsService.GetUsersNationalityStatistics();
            ViewBag.NationalityLabels = nationalityStats.Select(s => s.Nationality).ToList();
            ViewBag.NationalityData = nationalityStats.Select(s => s.Count).ToList();

            var genderStats = _statsService.GetUsersGenderStatistics();
            ViewBag.GenderLabels = genderStats.Select(s => s.Gender).ToList();
            ViewBag.GenderData = genderStats.Select(s => s.Count).ToList();

            var ageStats = _statsService.GetUserAgeGroupStatistics();
            ViewBag.AgeLabels = ageStats.Select(s => s.Label).ToList();
            ViewBag.AgeData = ageStats.Select(s => s.Count).ToList();

            var userCombinations = _statsService.GetTopUserCombinations();
            ViewBag.UserComboLabels = userCombinations
                .Select(c => $"{c.Age}, {c.Nationality}, {c.Gender}")
                .ToList();
            ViewBag.UserComboData = userCombinations
                .Select(c => c.Count)
                .ToList();


            return View("ManagerStats");
        }

        private bool HasActiveFilters(string query, string gender, string nationality, int? minAge, int? maxAge, DateTime? createdAfter)
        {
            return !string.IsNullOrWhiteSpace(query)
                || !string.IsNullOrWhiteSpace(gender)
                || !string.IsNullOrWhiteSpace(nationality)
                || minAge.HasValue
                || maxAge.HasValue
                || createdAfter.HasValue;
        }

        [HttpGet]
        public IActionResult FilterUsersAjax(string query, string gender, string nationality, int? minAge, int? maxAge, DateTime? createdAfter, int page = 1)
        {
            const int pageSize = 4;

            if (!HasActiveFilters(query, gender, nationality, minAge, maxAge, createdAfter))
            {
                ViewBag.CurrentPage = 1;
                ViewBag.TotalPages = 0;
                return PartialView("_UserTablePartial", new List<User>());
            }

            List<User> pagedUsers = _statsService.SearchUsers(query, gender, nationality, minAge, maxAge, createdAfter, page, pageSize);
            int totalUserCount = _statsService.GetFilteredUserCount(query, gender, nationality, minAge, maxAge, createdAfter);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalUserCount / (double)pageSize);

            return PartialView("_UserTablePartial", pagedUsers);
        }



    }
}

