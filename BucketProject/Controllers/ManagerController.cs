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
        private readonly IManagerRepo _managerRepo;
        private readonly IMapper _mapper;



        public ManagerController(IStatsService statsService, IManagerRepo managerRepo, IMapper mapper)
        {
            _statsService = statsService;
            _managerRepo = managerRepo;
            _mapper = mapper;
        }
        public IActionResult Manager(string query, string gender, string nationality, int? minAge, int? maxAge, DateTime? createdAfter)
        {
            // Load and map users (filtered or all)
            var users = _statsService.SearchUsers(query, gender, nationality, minAge, maxAge, createdAfter);
            users ??= new List<User>();

            // Charts
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

            var typeStats = _statsService.GetGoalTypeStatisticsManager();
            var categoryStats = _statsService.GetGoalCategoryStatisticsManager();
            ViewBag.TypeLabels = typeStats.Select(s => s.Type).ToList();
            ViewBag.TypeData = typeStats.Select(s => s.Count).ToList();
            ViewBag.CategoryLabels = categoryStats.Select(s => s.Category).ToList();
            ViewBag.CategoryData = categoryStats.Select(s => s.Count).ToList();

            var nationalityStats = _statsService.GetUsersNationalityStatistics();
            var genderStats = _statsService.GetUsersGenderStatistics();
            var ageStats = _statsService.GetUserAgeGroupStatistics();
            ViewBag.NationalityLabels = nationalityStats.Select(s => s.Nationality).ToList();
            ViewBag.NationalityData = nationalityStats.Select(s => s.Count).ToList();
            ViewBag.GenderLabels = genderStats.Select(s => s.Gender).ToList();
            ViewBag.GenderData = genderStats.Select(s => s.Count).ToList();
            ViewBag.AgeLabels = ageStats.Select(s => s.Label).ToList();
            ViewBag.AgeData = ageStats.Select(s => s.Count).ToList();

            ViewBag.Nationalities = _managerRepo.GetAllUsers()
                .Select(u => u.Nationality)
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            ViewBag.Genders = _managerRepo.GetAllUsers()
               .Select(u => u.Gender)
               .Distinct()
               .OrderBy(n => n)
               .ToList();

            return View(users); 
        }



    }
}
