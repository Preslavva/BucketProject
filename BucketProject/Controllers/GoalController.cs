using BucketProject.BLL.Business_Logic.Entity;
using BucketProject.DAL.Models.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using BucketProject.BLL.Business_Logic.InterfacesService;

namespace BucketProjetc.UI.BucketProject.Controllers
{
    public class GoalController : Controller
    {
        private readonly IGoalService _goalService;
        public GoalController(IGoalService goalService)
        {
            _goalService = goalService;
        }

        [HttpPost]
        public IActionResult CreateMonthGoal(GoalType type,string description)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            Category category = Category.Month;
            _goalService.CreateGoal(category,type,description);
            return RedirectToAction("MonthGoals");
        }

        [HttpPost]
        public IActionResult CreateYearGoal(GoalType type,string description)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            Category category = Category.Year;
            _goalService.CreateGoal(category, type, description);
            return RedirectToAction("YearGoals");
        }

        [HttpPost]
        public IActionResult CreateBucketListGoal(GoalType type,string description)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            Category category = Category.Bucket_list;
            _goalService.CreateGoal(category,type,description);
            return RedirectToAction("BucketList");
        }

        [HttpPost]
        public IActionResult CreateWeekGoal(GoalType type,string goalDescription)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");

            Category category = Category.Week;
            _goalService.CreateGoal(category, type, goalDescription);

            return RedirectToAction("WeekGoals");
        }



        [HttpPost]
        public IActionResult EditGoalWeek(Goal goal, string description)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");

            _goalService.UpdateGoal(goal, description);
            return RedirectToAction("WeekGoals");
        }

        [HttpPost]
        public IActionResult EditGoalMonth(Goal goal, string description)
        {

            ViewBag.Username = HttpContext.Session.GetString("Username");
            _goalService.UpdateGoal(goal, description);
            return RedirectToAction("MonthGoals");
        }

        [HttpPost]
        public IActionResult EditGoalYear(Goal goal, string description)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");

            _goalService.UpdateGoal(goal, description);
            return RedirectToAction("YearGoals");
        }

        [HttpPost]
        public IActionResult EditGoalBucketList(Goal goal, string description)
        {

            ViewBag.Username = HttpContext.Session.GetString("Username");
            _goalService.UpdateGoal(goal, description);
            return RedirectToAction("BucketList");
        }

        [HttpPost]
        public IActionResult DeleteGoalWeek(Goal goal)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            _goalService.DeleteGoal(goal);
            return RedirectToAction("WeekGoals");
        }

        [HttpPost]
        public IActionResult DeleteGoalYear(Goal goal)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            _goalService.DeleteGoal(goal);
            return RedirectToAction("YearGoals");
        }
        [HttpPost]
        public IActionResult DeleteGoalMonth(Goal goal)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            _goalService.DeleteGoal(goal);
            return RedirectToAction("MonthGoals");
        }

        [HttpPost]
        public IActionResult DeleteGoalBucketList(Goal goal)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            _goalService.DeleteGoal(goal);
            return RedirectToAction("BucketList");
        }


        [HttpGet]
        public IActionResult WeekGoals()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            Category weekCategory = Category.Week;
            List<Goal> goals = _goalService.LoadGoalsByCategory(weekCategory);

            return View(goals);
        }

        [HttpGet]
        public IActionResult MonthGoals()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            List<Goal> goals = _goalService.LoadGoalsByCategory(Category.Month);

            return View(goals);
        }

        [HttpPost]
        public IActionResult ChangeGoalStatusWeek(Goal goal, bool isDone)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            _goalService.ChangeGoalStatus(goal, isDone);
            return RedirectToAction("WeekGoals");

        }

        [HttpPost]
        public IActionResult ChangeGoalStatusMonth(Goal goal, bool isDone)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            _goalService.ChangeGoalStatus(goal, isDone);
            return RedirectToAction("MonthGoals");

        }
        [HttpPost]
        public IActionResult ChangeGoalStatusYear(Goal goal, bool isDone)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            _goalService.ChangeGoalStatus(goal, isDone);
            return RedirectToAction("YearGoals");

        }

        [HttpPost]
        public IActionResult ChangeGoalStatusBucketList(Goal goal, bool isDone)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            _goalService.ChangeGoalStatus(goal, isDone);
            return RedirectToAction("BucketList");

        }


        [HttpGet]
        public IActionResult YearGoals()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            List<Goal> goals = _goalService.LoadGoalsByCategory(Category.Year);

            return View(goals);
        }

        [HttpGet]
        public IActionResult BucketList()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            List<Goal> goals = _goalService.LoadGoalsByCategory(Category.Bucket_list);

            return View(goals);
        }

        [HttpGet]
        public IActionResult WeekGoalsPreview()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            Category weekCategory = Category.Week;
            List<Goal> goals = _goalService.LoadGoalsByCategory(weekCategory);

            return View(goals);
        }

        [HttpGet]
        public IActionResult MonthGoalsPreview()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            Category weekCategory = Category.Month;
            List<Goal> goals = _goalService.LoadGoalsByCategory(weekCategory);

            return View(goals);
        }

        [HttpGet]
        public IActionResult YearGoalsPreview()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            Category weekCategory = Category.Year;
            List<Goal> goals = _goalService.LoadGoalsByCategory(weekCategory);

            return View(goals);
        }

    }
}
