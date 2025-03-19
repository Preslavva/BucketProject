using BucketProject.Models;
using BucketProject.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using BucketProject.Interfaces;

namespace BucketProject.Controllers
{
    public class GoalController : Controller
    {
        private readonly IGoalService _goalService;
        public GoalController(IGoalService goalService)
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
        public IActionResult CreateBucketListGoal(string description)
        {
            Category category = Category.Bucket_list;
            _goalService.CreateGoal(category, description);
            return RedirectToAction("BucketList");
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
        public IActionResult EditGoalBucketList(Goal goal, string description)
        {

            _goalService.UpdateGoal(goal, description);
            return RedirectToAction("BucketList");
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

        [HttpPost]
        public IActionResult DeleteGoalBucketList(Goal goal)
        {

            _goalService.DeleteGoal(goal);
            return RedirectToAction("BucketList");
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

        [HttpPost]
        public IActionResult ChangeGoalStatusBucketList(Goal goal, bool isDone)
        {
            _goalService.ChangeGoalStatus(goal, isDone);
            return RedirectToAction("BucketList");

        }


        [HttpGet]
        public IActionResult YearGoals()
        {
            List<Goal> goals = _goalService.LoadGoalsByCategory(Category.Year);

            return View(goals);
        }

        [HttpGet]
        public IActionResult BucketList()
        {
            List<Goal> goals = _goalService.LoadGoalsByCategory(Category.Bucket_list);

            return View(goals);
        }
    }

}
