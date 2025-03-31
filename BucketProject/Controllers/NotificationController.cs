using BucketProject.BLL.Business_Logic.Entity;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLL.Business_Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace BucketProject.BLL.Business_Logic.Controllers
{
    public class NotificationController : Controller
    {
        private readonly NotificationService _notificationService;
        private readonly GoalService _goalService;

        public NotificationController(NotificationService notificationService, GoalService goalService)
        {
            _notificationService = notificationService;
            _goalService = goalService;
        }

        [HttpGet]
        public IActionResult Notification()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");

            List<Goal> goals = _notificationService.CheckAndNotify(DateTime.Today);
            return View(goals);
        }
        [HttpPost]
        public IActionResult PostponeGoalWeek(Goal goal)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            _goalService.PostponeGoal(goal);
            return RedirectToAction("WeekGoals");

        }

        [HttpPost]
        public IActionResult PostponeGoalMonth(Goal goal)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            _goalService.PostponeGoal(goal);
            return RedirectToAction("MonthGoals");

        }
        [HttpPost]
        public IActionResult PostponeGoalYear(Goal goal)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            _goalService.PostponeGoal(goal);
            return RedirectToAction("YearGoals");

        }

    }
}
