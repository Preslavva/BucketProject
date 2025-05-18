using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLL.Business_Logic.Services;
using BucketProject.BLLBusiness_Logic.Domain;
using Microsoft.AspNetCore.Mvc;
using BucketProject.DAL.Data.Repositories;
using BucketProject.DAL.Data.InterfacesRepo;
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
        public IActionResult Manager(string query, string gender, string nationality, int? minAge, int? maxAge, DateTime? createdAfter)
        {
            ViewBag.Genders = _statsService.GetAllGenders();
            ViewBag.Nationalities = _statsService.GetAllNationalities();

            List<User> users = HasActiveFilters(query, gender, nationality, minAge, maxAge, createdAfter)
                ? _statsService.SearchUsers(query, gender, nationality, minAge, maxAge, createdAfter)
                : new List<User>();

           
            List<StatsDTO> userStats = _statsService.GetUserRegistrationsPerMonth();
            ViewBag.UserLabels = userStats.Select(s => s.Period).ToList();
            ViewBag.UserData = userStats.Select(s => s.Count).ToList();

            List<StatsDTO> goalStats = _statsService.GetGoalsPerMonth();
            ViewBag.GoalLabels = goalStats.Select(s => s.Period).ToList();
            ViewBag.GoalData = goalStats.Select(s => s.Count).ToList();

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

            List<StatsDTO> genderStats = _statsService.GetUsersGenderStatistics();
            ViewBag.GenderLabels = genderStats.Select(s => s.Gender).ToList();
            ViewBag.GenderData = genderStats.Select(s => s.Count).ToList();

            List<StatsDTO> ageStats = _statsService.GetUserAgeGroupStatistics();
            ViewBag.AgeLabels = ageStats.Select(s => s.Label).ToList();
            ViewBag.AgeData = ageStats.Select(s => s.Count).ToList();

            return View(users);
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
        public IActionResult FilterUsersAjax(string query, string gender, string nationality, int? minAge, int? maxAge, DateTime? createdAfter)
        {
            List<User> users = HasActiveFilters(query, gender, nationality, minAge, maxAge, createdAfter)
                ? _statsService.SearchUsers(query, gender, nationality, minAge, maxAge, createdAfter)
                : new List<User>();

            return PartialView("_UserTablePartial", users);
        }


    }
}
