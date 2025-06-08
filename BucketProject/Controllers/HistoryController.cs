
using BucketProject.BLL.Business_Logic.InterfacesService;
using Microsoft.AspNetCore.Mvc;
using BucketProject.UI.ViewModels.ViewModels;
using AutoMapper;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.DAL.Models.Enums;
using BucketProject.BLL.Business_Logic.Services;

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
        public IActionResult FilterHistory(
     DateTime? startDate,
     DateTime? endDate,
     string? category,
     string? goalType,
     int page = 1,
     int pageSize = 4)
        {
            int totalGoals = _goalService.CountGoalsCreatedInRange(startDate, endDate, category, goalType);

            List<Goal> goals = _goalService.GetGoalsCreatedInRange(startDate, endDate, category, goalType, page, pageSize);
            List<HistoryViewModel> result = _mapper.Map<List<HistoryViewModel>>(goals);

            int totalPages = (int)Math.Ceiling((double)totalGoals / pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;

            foreach (var goal in result)
            {
                if (goal.ParentGoalId != null)
                {
                    goal.ParentGoalDescription = _goalService.GetParentGoalDescription(goal.Id); 
                }
            }

            return PartialView("_HistoryPartial", result);
        }

    }

}

