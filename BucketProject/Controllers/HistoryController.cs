using BucketProject.DAL.Models.Entities;
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
            var groupedGoals = _goalService.LoadGroupedExpiredGoals();

            return View(groupedGoals); 
        }



    }
}
