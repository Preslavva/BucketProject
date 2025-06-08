
using BucketProject.BLL.Business_Logic.InterfacesService;
using Microsoft.AspNetCore.Mvc;
using BucketProject.UI.ViewModels.ViewModels;
using AutoMapper;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.DAL.Models.Enums;

namespace BucketProject.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IGoalService _goalService;
        private readonly IMapper _mapper;

        public HistoryController(IGoalService goalService, IMapper mapper)
        {
            _goalService = goalService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult History()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("LogIn", "User");

            ViewBag.GoalTypes = _goalService.GetGoalTypesForCurrentUser();
            ViewBag.Categories = Enum.GetNames(typeof(Category)).ToList(); 

            return View("History");
        }

        [HttpGet]
        public IActionResult FilterHistory(DateTime? startDate, DateTime? endDate, string? category, string? goalType, int page = 1, int pageSize = 4)
        {
            List<Goal> goals = _goalService.GetGoalsCreatedInRange(startDate, endDate, category, goalType, page, pageSize);
            List<HistoryViewModel> result = _mapper.Map<List<HistoryViewModel>>(goals);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.HasMore = result.Count == pageSize;

            return PartialView("_HistoryPartial", result);
        }
    }

}

