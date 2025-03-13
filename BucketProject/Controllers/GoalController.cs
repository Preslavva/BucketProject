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
            HttpContext.Session.GetString("Username");
            return View();
        }


        [HttpPost]
        public IActionResult CreateWeekGoal(string description)
        {
            Category category = Category.Week;
            _goalService.CreateGoal(category, description);
            return RedirectToAction("WeekGoals");
        }

    //    [HttpPost]
    //    public IActionResult CreateMonthGoal(string description)
    //    {
    //        Category category = Category.Month;
    //        _goalService.CreateGoal(category, description);
    //        return RedirectToAction("Index");
    //    }

    //    [HttpPost]
    //    public IActionResult CreateYearGoal(string description)
    //    {
    //        Category category = Category.Year;
    //        _goalService.CreateGoal(category, description);
    //        return RedirectToAction("Index");
    //    }

    //    [HttpPost]
    //    public IActionResult CreateLifeGoal(string description)
    //    {
    //        Category category = Category.Bucket_list;
    //        _goalService.CreateGoal(category, description);
    //        return RedirectToAction("Index");
    //    }

    //    [HttpPost]
    //    public IActionResult AssignGoalToUser(int goalId, int userId)
    //    {
    //        User user = new User { Id = userId };
    //        Goal goal = new Goal { Id = goalId };

    //        _goalService.AssignGoalToUser(user, goal);

    //        return View();
    //    }

    //    [HttpGet]
    //    public IActionResult LoadGoalsByCategory(Category category)
    //    {
    //        int userId = GetLoggedInUserId();
    //        User user = new User { Id = userId };

    //        List<Goal> goals = _goalService.LoadGoalsByCategory(user, category);

    //        return View(goals);
    //    }

    //    private int GetLoggedInUserId()
    //    {
    //        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
    //        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    //    }
    }
}

