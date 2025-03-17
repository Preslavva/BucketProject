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

        [HttpPost]
        public IActionResult CreateLifeGoal(string description)
        {
            Category category = Category.Bucket_list;
            _goalService.CreateGoal(category, description);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CreateWeekGoal(string goalDescription)
        {

            Category category = Category.Week;
            _goalService.CreateGoal(category, goalDescription);

            return RedirectToAction("WeekGoals");
        }

        [HttpPost]
        public IActionResult EditGoalWeek(Goal goal, string description)
        {

            _goalService.UpdateGoal(goal, description);
            return RedirectToAction("WeekGoals");
        }

        [HttpPost]
        public IActionResult EditGoalMonth(Goal goal, string description)
        {

            _goalService.UpdateGoal(goal, description);
            return RedirectToAction("MonthGoals");
        }

        [HttpPost]
        public IActionResult EditGoalYear(Goal goal, string description)
        {

            _goalService.UpdateGoal(goal, description);
            return RedirectToAction("YearGoals");
        }

        [HttpPost]
        public IActionResult DeleteGoalWeek(Goal goal)
        {

            _goalService.DeleteGoal(goal);
            return RedirectToAction("WeekGoals");
        }

        [HttpPost]
        public IActionResult DeleteGoalYear(Goal goal)
        {

            _goalService.DeleteGoal(goal);
            return RedirectToAction("YearGoals");
        }
        [HttpPost]
        public IActionResult DeleteGoalMonth(Goal goal)
        {

            _goalService.DeleteGoal(goal);
            return RedirectToAction("MonthGoals");
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
            List<Goal> goals = _goalService.LoadGoalsByCategory(Category.Month);

            return View(goals);
        }

        [HttpPost]
        public IActionResult ChangeGoalStatusWeek(Goal goal, bool isDone)
        {

            _goalService.ChangeGoalStatus(goal, isDone);
            return RedirectToAction("WeekGoals");

        }

        [HttpPost]
        public IActionResult ChangeGoalStatusMonth(Goal goal, bool isDone)
        {

            _goalService.ChangeGoalStatus(goal, isDone);
            return RedirectToAction("MonthGoals");

        }
        [HttpPost]
        public IActionResult ChangeGoalStatusYear(Goal goal, bool isDone)
        {
            _goalService.ChangeGoalStatus(goal, isDone);
            return RedirectToAction("YearGoals");

        }

        [HttpGet]
        public IActionResult YearGoals()
        {
            List<Goal> goals = _goalService.LoadGoalsByCategory(Category.Year);

            return View(goals);
        }

    }

}
