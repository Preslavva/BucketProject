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
        public IActionResult CreateGoal(Category category, string description, DateTime deadline)
        {
            int userId = GetLoggedInUserId(); 

            if (!string.IsNullOrWhiteSpace(description))
            {
                DateTime createdAt = DateTime.Now;
                bool isDone = false;
                bool isDeleted = false;

                _goalService.CreateGoal(category, description, deadline, isDone, isDeleted, createdAt);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AssignGoalToUser(int goalId, int userId)
        {
            User user = new User { Id = userId };
            Goal goal = new Goal { Id = goalId };

            _goalService.AssignGoalToUser(user, goal);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult LoadGoalsByCategory(Category category)
        {
            int userId = GetLoggedInUserId();
            User user = new User { Id = userId };

            List<Goal> goals = _goalService.LoadGoalsByCategory(user, category);

            return View(@category+ " Goals", goals);
        }

        private int GetLoggedInUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}

