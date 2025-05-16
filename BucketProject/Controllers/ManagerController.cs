using BucketProject.BLL.Business_Logic.InterfacesService;
using Microsoft.AspNetCore.Mvc;

namespace BucketProject.Controllers
{
    public class ManagerController : Controller
    {

        private readonly IStatsService _statsService;

        public ManagerController(IStatsService statsService)
        {
            _statsService = statsService;
        }
        public IActionResult Manager()
        {
            var userStats = _statsService.GetUserRegistrationsPerMonth();
            ViewBag.UserLabels = userStats.Select(s => s.Period).ToList();
            ViewBag.UserData = userStats.Select(s => s.Count).ToList();

            var goalStats = _statsService.GetGoalsPerMonth();
            ViewBag.GoalLabels = goalStats.Select(s => s.Period).ToList();
            ViewBag.GoalData = goalStats.Select(s => s.Count).ToList();

            return View();
        }

    }
}
