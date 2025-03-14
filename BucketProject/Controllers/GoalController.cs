using BucketProject.Models;
using BucketProject.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace BucketProject.Controllers
{
    public class GoalController : Controller
    {
        private readonly GoalService _goalService;
        public GoalController(GoalService goalService)
        {
            _goalService = goalService;
        }

        [HttpGet]
        public IActionResult WeekGoals()
        {
            Category weekCategory = Category.Week;
            List<Goal> goals = _goalService.LoadGoalsByCategory(weekCategory);

            return View(goals);
        }

        [HttpGet]
        public IActionResult MonthGoals()
        {
            HttpContext.Session.GetString("Username");
            return View();
        }

        [HttpGet]
        public IActionResult YearGoals()
        {
            HttpContext.Session.GetString("Username");
            return View();
        }

        [HttpPost]
        public IActionResult CreateWeekGoal(string description)
        {
            Category category = Category.Week;
            _goalService.CreateGoal(category, description);
            return RedirectToAction("WeekGoals",new { category = Category.Week});
        }

        [HttpPost]
        public IActionResult CreateMonthGoal(string description)
        {
            Category category = Category.Month;
            _goalService.CreateGoal(category, description);
            return RedirectToAction("MonthGoals");
        }

        [HttpPost]
        public IActionResult CreateYearGoal(string description)
        {
            Category category = Category.Year;
            _goalService.CreateGoal(category, description);
            return RedirectToAction("YearGoals");
        }

        //[HttpPost]
        //public IActionResult CreateLifeGoal(string description)
        //{
        //    Category category = Category.Bucket_list;
        //    _goalService.CreateGoal(category, description);
        //    return RedirectToAction("Index");
        //}


      


    }
}

