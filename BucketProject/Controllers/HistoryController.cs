using BucketProject.BLL.Business_Logic.InterfacesService;
using Microsoft.AspNetCore.Mvc;

namespace BucketProject.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IGoalService _goalService;

        public HistoryController(IGoalService goalService)
        {
            _goalService = goalService;
        }

        public IActionResult History()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");

            var groupedGoals = _goalService.LoadExpiredGoalsGroupedByCategory();
            return View(groupedGoals);
        }

    }
}
