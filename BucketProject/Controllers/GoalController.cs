
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using BucketProject.BLL.Business_Logic.InterfacesService;
using AutoMapper;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.UI.ViewModels.ViewModels;

namespace BucketProject.UI.BucketProject.Controllers
{
    public class GoalController : Controller
    {
        private readonly IGoalService _goalService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GoalController(IGoalService goalService, IMapper mapper, IConfiguration configuration)
        {
            _goalService = goalService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult CreateMonthGoal(GoalViewModel viewModel)
        {
            GoalDomain newGoal = _mapper.Map<GoalDomain>(viewModel);

            viewModel.Category = "Month";

            if (!ModelState.IsValid)
            {
            ViewBag.AvailableTypes = GetAvailableTypes();

            }

            _goalService.CreateGoal(newGoal); 

            return RedirectToAction("MonthGoals");
        }


        [HttpPost]
        public IActionResult CreateYearGoal(GoalViewModel viewModel)
        {
            GoalDomain newGoal = _mapper.Map<GoalDomain>(viewModel);

            viewModel.Category = "Year";

            if (!ModelState.IsValid)
            {
                ViewBag.AvailableTypes = GetAvailableTypes();

            }

            _goalService.CreateGoal(newGoal);

            return RedirectToAction("YearGoals");
        }

        [HttpPost]
        public IActionResult CreateBucketListGoal(GoalViewModel viewModel)
        {
            GoalDomain newGoal = _mapper.Map<GoalDomain>(viewModel);

            viewModel.Category = "Bucket_list";

            if (!ModelState.IsValid)
            {
            ViewBag.AvailableTypes = GetAvailableTypes();

            }

            _goalService.CreateGoal(newGoal);

            return RedirectToAction("BucketList");
        }

        [HttpPost]
        public IActionResult CreateWeekGoal(GoalViewModel viewModel)
        {
            GoalDomain newGoal = _mapper.Map<GoalDomain>(viewModel);

            viewModel.Category = "Week";

            if (!ModelState.IsValid)
            {
                ViewBag.AvailableTypes = GetAvailableTypes();

            }

            _goalService.CreateGoal(newGoal);
            return RedirectToAction("WeekGoals");
        }




        [HttpPost]
        public IActionResult EditWeekGoal(GoalViewModel viewModel)
        {
            GoalDomain newGoal = _mapper.Map<GoalDomain>(viewModel);

            _goalService.UpdateGoal(viewModel.Id, newGoal);

            return RedirectToAction("WeekGoals");
        }


        [HttpPost]
        public IActionResult EditMonthGoal(GoalViewModel viewModel)
        {
            GoalDomain newGoal = _mapper.Map<GoalDomain>(viewModel);

            _goalService.UpdateGoal(viewModel.Id, newGoal);

            return RedirectToAction("MonthGoals");
        }


        [HttpPost]
        public IActionResult EditYearGoal(GoalViewModel viewModel)
        {

            GoalDomain newGoal = _mapper.Map<GoalDomain>(viewModel);

            _goalService.UpdateGoal(viewModel.Id, newGoal);

            return RedirectToAction("YearGoals");
        }


        [HttpPost]
        public IActionResult EdiBucketListGoal(GoalViewModel viewModel)
        {
            GoalDomain newGoal = _mapper.Map<GoalDomain>(viewModel);

            _goalService.UpdateGoal(viewModel.Id, newGoal);

            return RedirectToAction("YearGoals");
        }

        [HttpPost]
        public IActionResult DeleteGoalWeek(int id)
        {
            _goalService.DeleteGoal(id);

            return RedirectToAction("WeekGoals");
        }


        [HttpPost]
        public IActionResult DeleteGoalYear(int id)
        {
            _goalService.DeleteGoal(id);
            return RedirectToAction("YearGoals");
        }
        [HttpPost]
        public IActionResult DeleteGoalMonth(int id)
        {
            _goalService.DeleteGoal(id);
            return RedirectToAction("MonthGoals");
        }

        [HttpPost]
        public IActionResult DeleteGoalBucketList(int id)
        {
            _goalService.DeleteGoal(id);
            return RedirectToAction("BucketList");
        }


       

        [HttpGet]
        public IActionResult WeekGoals()
        {
            List<GoalDomain> goalDomains = _goalService.LoadGoalsByCategory("Week");

            List<GoalViewModel> viewModels = _mapper.Map<List<GoalViewModel>>(goalDomains);

            List<GoalViewModel> parentGoals = viewModels
                .Where(g => g.ParentGoalId == null)
                .ToList();

            foreach (GoalViewModel goal in parentGoals)
            {
                goal.Children = viewModels
                    .Where(g => g.ParentGoalId == goal.Id)
                    .ToList();
            }

            ViewBag.AvailableTypes = GetAvailableTypes();

            return View(parentGoals);
        }


        [HttpPost]
        public IActionResult ChangeGoalStatusWeek(int id, bool isDone)
        {
            _goalService.ChangeGoalStatus(id, isDone);
            return RedirectToAction("WeekGoals");

        }

        [HttpPost]
        public IActionResult ChangeGoalStatusMonth(int id, bool isDone)
        {
            _goalService.ChangeGoalStatus(id, isDone);
            return RedirectToAction("MonthGoals");

        }
        [HttpPost]
        public IActionResult ChangeGoalStatusYear(int id, bool isDone)
        {
            _goalService.ChangeGoalStatus(id, isDone);
            return RedirectToAction("YearGoals");

        }

        [HttpPost]
        public IActionResult ChangeGoalStatusBucketList(int id, bool isDone)
        {
            _goalService.ChangeGoalStatus(id, isDone);
            return RedirectToAction("BucketList");

        }


        [HttpGet]
        public IActionResult YearGoals()
        {
            List<GoalDomain> goalDomains = _goalService.LoadGoalsByCategory("Year");

            List<GoalViewModel> viewModels = _mapper.Map<List<GoalViewModel>>(goalDomains);

            List<GoalViewModel> parentGoals = viewModels
                .Where(g => g.ParentGoalId == null)
                .ToList();

            foreach (GoalViewModel goal in parentGoals)
            {
                goal.Children = viewModels
                    .Where(g => g.ParentGoalId == goal.Id)
                    .ToList();
            }

            ViewBag.AvailableTypes = GetAvailableTypes();

            return View(parentGoals);
        }

        [HttpGet]
        public IActionResult BucketList()
        {

            List<GoalDomain> goalDomains = _goalService.LoadGoalsByCategory("Bucket_list");

            List<GoalViewModel> viewModels = _mapper.Map<List<GoalViewModel>>(goalDomains);

            List<GoalViewModel> parentGoals = viewModels
                .Where(g => g.ParentGoalId == null)
                .ToList();

            foreach (GoalViewModel goal in parentGoals)
            {
                goal.Children = viewModels
                    .Where(g => g.ParentGoalId == goal.Id)
                    .ToList();
            }

            ViewBag.AvailableTypes = GetAvailableTypes();

            return View(parentGoals);
        }
        [HttpGet]
        public IActionResult MonthGoals()
        {
            List<GoalDomain> goalDomains = _goalService.LoadGoalsByCategory("Month");

            List<GoalViewModel> viewModels = _mapper.Map<List<GoalViewModel>>(goalDomains);

            List<GoalViewModel> parentGoals = viewModels
                .Where(g => g.ParentGoalId == null)
                .ToList();

            foreach (GoalViewModel goal in parentGoals)
            {
                goal.Children = viewModels
                    .Where(g => g.ParentGoalId == goal.Id)
                    .ToList();
            }

            ViewBag.AvailableTypes = GetAvailableTypes();

            return View(parentGoals);
        }

        [HttpGet]
        public IActionResult WeekGoalsPreview()
        {
            List<GoalDomain> goalDomains = _goalService.LoadGoalsByCategory("Week");

            List<GoalViewModel> viewModels = _mapper.Map<List<GoalViewModel>>(goalDomains);

            List<GoalViewModel> parentGoals = viewModels
                .Where(g => g.ParentGoalId == null)
                .ToList();

            foreach (GoalViewModel goal in parentGoals)
            {
                goal.Children = viewModels
                    .Where(g => g.ParentGoalId == goal.Id)
                    .ToList();
            }

            ViewBag.AvailableTypes = GetAvailableTypes();

            return View(parentGoals);
        }

        [HttpGet]
        public IActionResult MonthGoalsPreview()
        {
            List<GoalDomain> goalDomains = _goalService.LoadGoalsByCategory("Month");

            List<GoalViewModel> viewModels = _mapper.Map<List<GoalViewModel>>(goalDomains);

            List<GoalViewModel> parentGoals = viewModels
                .Where(g => g.ParentGoalId == null)
                .ToList();

            foreach (GoalViewModel goal in parentGoals)
            {
                goal.Children = viewModels
                    .Where(g => g.ParentGoalId == goal.Id)
                    .ToList();
            }

            ViewBag.AvailableTypes = GetAvailableTypes();

            return View(parentGoals);
        }

        [HttpGet]
        public IActionResult YearGoalsPreview()
        {
            List<GoalDomain> goalDomains = _goalService.LoadGoalsByCategory("Year");

            List<GoalViewModel> viewModels = _mapper.Map<List<GoalViewModel>>(goalDomains);

            List<GoalViewModel> parentGoals = viewModels
                .Where(g => g.ParentGoalId == null)
                .ToList();

            foreach (GoalViewModel goal in parentGoals)
            {
                goal.Children = viewModels
                    .Where(g => g.ParentGoalId == goal.Id)
                    .ToList();
            }

            ViewBag.AvailableTypes = GetAvailableTypes();

            return View(parentGoals);
        }

        public async Task<IActionResult> BreakDownGoalWeek(int id)
        {
            List<GoalDomain> subGoals = await _goalService.BreakDownGoalAsync(id); 

            List<GoalViewModel> subGoalViewModels = _mapper.Map<List<GoalViewModel>>(subGoals); 

            TempData["SubGoals"] = JsonConvert.SerializeObject(subGoalViewModels);
            TempData["SubGoalForId"] = id;

            return RedirectToAction("WeekGoals");
        }

        [HttpPost]
        public async Task<IActionResult> BreakDownGoalMonth(int id)
        {
            List<GoalDomain> subGoals = await _goalService.BreakDownGoalAsync(id);

            List<GoalViewModel> subGoalViewModels = _mapper.Map<List<GoalViewModel>>(subGoals);

            TempData["SubGoals"] = JsonConvert.SerializeObject(subGoals);
            TempData["SubGoalForId"] = id;

            return RedirectToAction("MonthGoals");

        }

        [HttpPost]
        public async Task<IActionResult> BreakDownGoalYear(int id)
        {
            List<GoalDomain> subGoals = await _goalService.BreakDownGoalAsync(id);

            List<GoalViewModel> subGoalViewModels = _mapper.Map<List<GoalViewModel>>(subGoals);

            TempData["SubGoals"] = JsonConvert.SerializeObject(subGoals);
            TempData["SubGoalForId"] = id;

            return RedirectToAction("YearGoals");

        }

        [HttpPost]
        public async Task<IActionResult> BreakDownGoalBucketList(int id)
        {
            List<GoalDomain> subGoals = await _goalService.BreakDownGoalAsync(id);

            List<GoalViewModel> subGoalViewModels = _mapper.Map<List<GoalViewModel>>(subGoals);

            TempData["SubGoals"] = JsonConvert.SerializeObject(subGoals);
            TempData["SubGoalForId"] = id;

            return RedirectToAction("BcuketList");

        }
        private List<string> GetAvailableTypes()
        {
            return new List<string>
    {
          "Fitness",
            "Career",
            "Education",
            "Finance",
            "Organization",
            "Relationships",
            "Social",
            "Travel",
            "Hobbies",
            "Psychology",
            "Digital",
            "Order"
    };
        }

    }

   
}
