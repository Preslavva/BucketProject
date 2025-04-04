
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

            var notifications = _notificationService.CheckAndNotify(DateTime.Today);
            return View(notifications);
        }

        [HttpPost]
        public IActionResult PostponeGoal(int id)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            _goalService.PostponeGoal(id);
            return RedirectToAction("Notification");
        }

    }
}
