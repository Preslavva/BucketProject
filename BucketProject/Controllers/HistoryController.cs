using BucketProject.BLL.Business_Logic.Entity;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.DAL.Models.Enums;
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

            var (weekly, monthly, yearly) = _goalService.LoadGroupedExpiredGoals();

            ViewBag.Weekly = weekly ?? new Dictionary<string, Dictionary<GoalType, List<Goal>>>();
            ViewBag.Monthly = monthly ?? new Dictionary<string, Dictionary<GoalType, List<Goal>>>();
            ViewBag.Yearly = yearly ?? new Dictionary<string, Dictionary<GoalType, List<Goal>>>();


            return View();
        }

    }
}
