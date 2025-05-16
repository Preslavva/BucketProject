using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BucketProject.UI.ViewModels.ViewModels;
using BucketProject.BLL.Business_Logic.InterfacesService;

namespace BucketProject.UI.BucketProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotificationService _notificationService;

        public HomeController(ILogger<HomeController> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            TempData["NotificationCount"] = _notificationService.GetTotalNotificationGoalCount(DateTime.Today);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
