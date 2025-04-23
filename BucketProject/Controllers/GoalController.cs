
using Microsoft.AspNetCore.Mvc;
using BucketProject.BLL.Business_Logic.InterfacesService;
using AutoMapper;
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
            viewModel.Category = "Month";

            if (!ModelState.IsValid)
            {
                ViewBag.AvailableTypes = GetAvailableTypes();
                TempData.Keep("SubGoals");
                TempData.Keep("SubGoalForId");
                return RedirectToAction("MonthGoals");
            }

            Goal domainModel = _mapper.Map<Goal>(viewModel);
            _goalService.CreateGoal(domainModel);

            if (TempData.ContainsKey("SubGoals"))
            {
                string? raw = TempData["SubGoals"]?.ToString();
                if (!string.IsNullOrWhiteSpace(raw))
                {
                    List<GoalViewModel>? subGoals = JsonConvert.DeserializeObject<List<GoalViewModel>>(raw);

                    List<GoalViewModel> updated = subGoals
                        .Where(g => !string.Equals(
                            g.Description?.Trim(),
                            viewModel.Description?.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (updated.Any())
                    {
                        TempData["SubGoals"] = JsonConvert.SerializeObject(updated);
                        TempData.Keep("SubGoals");

                        TempData["SubGoalForId"] = viewModel.ParentGoalId?.ToString();
                        TempData.Keep("SubGoalForId");
                    }
                    else
                    {
                        TempData.Remove("SubGoals");
                        TempData.Remove("SubGoalForId");
                    }
                }
            }

            return RedirectToAction("MonthGoals");
        }


        [HttpPost]
        public IActionResult CreateYearGoal(GoalViewModel viewModel)
        {
            viewModel.Category = "Year";

            if (!ModelState.IsValid)
            {
                ViewBag.AvailableTypes = GetAvailableTypes();
                TempData.Keep("SubGoals");
                TempData.Keep("SubGoalForId");
                return RedirectToAction("YearGoals");
            }

            Goal domainModel = _mapper.Map<Goal>(viewModel);
            _goalService.CreateGoal(domainModel);

            if (TempData.ContainsKey("SubGoals"))
            {
                string? raw = TempData["SubGoals"]?.ToString();
                if (!string.IsNullOrWhiteSpace(raw))
                {
                    List<GoalViewModel>? subGoals = JsonConvert.DeserializeObject<List<GoalViewModel>>(raw);

                    List<GoalViewModel> updated = subGoals
                        .Where(g => !string.Equals(
                            g.Description?.Trim(),
                            viewModel.Description?.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (updated.Any())
                    {
                        TempData["SubGoals"] = JsonConvert.SerializeObject(updated);
                        TempData.Keep("SubGoals");

                        TempData["SubGoalForId"] = viewModel.ParentGoalId?.ToString();
                        TempData.Keep("SubGoalForId");
                    }
                    else
                    {
                        TempData.Remove("SubGoals");
                        TempData.Remove("SubGoalForId");
                    }
                }
            }

            return RedirectToAction("YearGoals");
        }

        [HttpPost]
        public IActionResult CreateBucketListGoal(GoalViewModel viewModel)
        {
            viewModel.Category = "Week";

            if (!ModelState.IsValid)
            {
                ViewBag.AvailableTypes = GetAvailableTypes();
                TempData.Keep("SubGoals");
                TempData.Keep("SubGoalForId");
                return RedirectToAction("BucketListGoals");
            }

            Goal domainModel = _mapper.Map<Goal>(viewModel);
            _goalService.CreateGoal(domainModel);

            if (TempData.ContainsKey("SubGoals"))
            {
                string? raw = TempData["SubGoals"]?.ToString();
                if (!string.IsNullOrWhiteSpace(raw))
                {
                    List<GoalViewModel>? subGoals = JsonConvert.DeserializeObject<List<GoalViewModel>>(raw);

                    List<GoalViewModel> updated = subGoals
                        .Where(g => !string.Equals(
                            g.Description?.Trim(),
                            viewModel.Description?.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (updated.Any())
                    {
                        TempData["SubGoals"] = JsonConvert.SerializeObject(updated);
                        TempData.Keep("SubGoals");

                        TempData["SubGoalForId"] = viewModel.ParentGoalId?.ToString();
                        TempData.Keep("SubGoalForId");
                    }
                    else
                    {
                        TempData.Remove("SubGoals");
                        TempData.Remove("SubGoalForId");
                    }
                }
            }

            return RedirectToAction("BucketList");
        }

        [HttpPost]
        public IActionResult CreateWeekGoal(GoalViewModel viewModel)
        {
            viewModel.Category = "Week";

            if (!ModelState.IsValid)
            {
                ViewBag.AvailableTypes = GetAvailableTypes();
                TempData.Keep("SubGoals");
                TempData.Keep("SubGoalForId");
                return RedirectToAction("WeekGoals");
            }

            Goal domainModel = _mapper.Map<Goal>(viewModel);
            _goalService.CreateGoal(domainModel);

            if (TempData.ContainsKey("SubGoals"))
            {
                string raw = TempData["SubGoals"]?.ToString();
                if (!string.IsNullOrWhiteSpace(raw))
                {
                    List<GoalViewModel> subGoals = JsonConvert.DeserializeObject<List<GoalViewModel>>(raw);

                    List<GoalViewModel> updated = subGoals
                        .Where(g => !string.Equals(
                            g.Description?.Trim(),
                            viewModel.Description?.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (updated.Any())
                    {
                        TempData["SubGoals"] = JsonConvert.SerializeObject(updated);
                        TempData.Keep("SubGoals");

                        TempData["SubGoalForId"] = viewModel.ParentGoalId?.ToString();
                        TempData.Keep("SubGoalForId");
                    }
                    else
                    {
                        TempData.Remove("SubGoals");
                        TempData.Remove("SubGoalForId");
                    }
                }
            }

            return RedirectToAction("WeekGoals");
        }



        [HttpPost]
        public IActionResult EditWeekGoal(GoalViewModel viewModel)
        {
            Goal newGoal = _mapper.Map<Goal>(viewModel);

            _goalService.UpdateGoal(viewModel.Id, newGoal);

            return RedirectToAction("WeekGoals");
        }


        [HttpPost]
        public IActionResult EditMonthGoal(GoalViewModel viewModel)
        {
            Goal newGoal = _mapper.Map<Goal>(viewModel);

            _goalService.UpdateGoal(viewModel.Id, newGoal);

            return RedirectToAction("MonthGoals");
        }


        [HttpPost]
        public IActionResult EditYearGoal(GoalViewModel viewModel)
        {

            Goal newGoal = _mapper.Map<Goal>(viewModel);

            _goalService.UpdateGoal(viewModel.Id, newGoal);

            return RedirectToAction("YearGoals");
        }


        [HttpPost]
        public IActionResult EdiBucketListGoal(GoalViewModel viewModel)
        {
            Goal newGoal = _mapper.Map<Goal>(viewModel);

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
            List<Goal> goalDomains = _goalService.LoadGoalsByCategory("Week");

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

            var pageModel = new GoalsPageViewModel
            {
                Goals = parentGoals
            };

            ViewBag.AvailableTypes = GetAvailableTypes();

            return View(pageModel);
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
            List<Goal> goalDomains = _goalService.LoadGoalsByCategory("Year");

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

            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = parentGoals
            };

            ViewBag.AvailableTypes = GetAvailableTypes();

            return View(pageModel);
        }

        [HttpGet]
        public IActionResult BucketList()
        {

            List<Goal> goalDomains = _goalService.LoadGoalsByCategory("Bucket_list");

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

            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = parentGoals
            };

            ViewBag.AvailableTypes = GetAvailableTypes();

            return View(pageModel);
        }
        [HttpGet]
        public IActionResult MonthGoals()
        {
            List<Goal> goalDomains = _goalService.LoadGoalsByCategory("Month");

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

            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = parentGoals
            };

            ViewBag.AvailableTypes = GetAvailableTypes();

            return View(pageModel);
        }

        [HttpGet]
        public IActionResult WeekGoalsPreview()
        {
            List<Goal> goalDomains = _goalService.LoadGoalsByCategory("Week");

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

            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = parentGoals
            };

            ViewBag.AvailableTypes = GetAvailableTypes();

            return View(pageModel);
        }

        [HttpGet]
        public IActionResult MonthGoalsPreview()
        {
            List<Goal> goalDomains = _goalService.LoadGoalsByCategory("Month");

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

            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = parentGoals
            };

            ViewBag.AvailableTypes = GetAvailableTypes();

            return View(pageModel);
        }

        [HttpGet]
        public IActionResult YearGoalsPreview()
        {
            List<Goal> goalDomains = _goalService.LoadGoalsByCategory("Year");

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

            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = parentGoals
            };

            ViewBag.AvailableTypes = GetAvailableTypes();

            return View(pageModel);
        }

        public async Task<IActionResult> BreakDownGoalWeek(int id)
        {
            List<Goal> subGoals = await _goalService.BreakDownGoalAsync(id); 

            List<GoalViewModel> subGoalViewModels = _mapper.Map<List<GoalViewModel>>(subGoals); 

            TempData["SubGoals"] = JsonConvert.SerializeObject(subGoalViewModels);
            TempData["SubGoalForId"] = id;

            return RedirectToAction("WeekGoals");
        }

        [HttpPost]
        public async Task<IActionResult> BreakDownGoalMonth(int id)
        {
            List<Goal> subGoals = await _goalService.BreakDownGoalAsync(id);

            List<GoalViewModel> subGoalViewModels = _mapper.Map<List<GoalViewModel>>(subGoals);

            TempData["SubGoals"] = JsonConvert.SerializeObject(subGoals);
            TempData["SubGoalForId"] = id;

            return RedirectToAction("MonthGoals");

        }

        [HttpPost]
        public async Task<IActionResult> BreakDownGoalYear(int id)
        {
            List<Goal> subGoals = await _goalService.BreakDownGoalAsync(id);

            List<GoalViewModel> subGoalViewModels = _mapper.Map<List<GoalViewModel>>(subGoals);

            TempData["SubGoals"] = JsonConvert.SerializeObject(subGoals);
            TempData["SubGoalForId"] = id;

            return RedirectToAction("YearGoals");

        }

        [HttpPost]
        public async Task<IActionResult> BreakDownGoalBucketList(int id)
        {
            List<Goal> subGoals = await _goalService.BreakDownGoalAsync(id);

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
        [HttpPost]
        public IActionResult RemoveSubGoalFromBreakdownWeek(string description)
        {
            if (TempData["SubGoals"] is string json)
            {
                var subGoals = JsonConvert.DeserializeObject<List<GoalViewModel>>(json);
                var updated = subGoals
                    .Where(g => !string.Equals(
                        g.Description?.Trim(),
                        description?.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (updated.Any())
                {
                    TempData["SubGoals"] = JsonConvert.SerializeObject(updated);
                    TempData.Keep("SubGoals");
                }
                else
                {
                    TempData.Remove("SubGoals");
                    TempData.Remove("SubGoalForId");
                }
            }

            return RedirectToAction("WeekGoals");
        }

   
     [HttpPost]
        public IActionResult RemoveSubGoalFromBreakdownMonth(string description)
        {
            if (TempData["SubGoals"] is string json)
            {
                var subGoals = JsonConvert.DeserializeObject<List<GoalViewModel>>(json);
                var updated = subGoals
                    .Where(g => !string.Equals(
                        g.Description?.Trim(),
                        description?.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (updated.Any())
                {
                    TempData["SubGoals"] = JsonConvert.SerializeObject(updated);
                    TempData.Keep("SubGoals");
                }
                else
                {
                    TempData.Remove("SubGoals");
                    TempData.Remove("SubGoalForId");
                }
            }

            return RedirectToAction("MonthGoals");
        }
        [HttpPost]
        public IActionResult RemoveSubGoalFromBreakdownYear(string description)
        {
            if (TempData["SubGoals"] is string json)
            {
                var subGoals = JsonConvert.DeserializeObject<List<GoalViewModel>>(json);
                var updated = subGoals
                    .Where(g => !string.Equals(
                        g.Description?.Trim(),
                        description?.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (updated.Any())
                {
                    TempData["SubGoals"] = JsonConvert.SerializeObject(updated);
                    TempData.Keep("SubGoals");
                }
                else
                {
                    TempData.Remove("SubGoals");
                    TempData.Remove("SubGoalForId");
                }
            }

            return RedirectToAction("YearGoals");
        }
        [HttpPost]
        public IActionResult RemoveSubGoalFromBreakdownBucketList(string description)
        {
            if (TempData["SubGoals"] is string json)
            {
                var subGoals = JsonConvert.DeserializeObject<List<GoalViewModel>>(json);
                var updated = subGoals
                    .Where(g => !string.Equals(
                        g.Description?.Trim(),
                        description?.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (updated.Any())
                {
                    TempData["SubGoals"] = JsonConvert.SerializeObject(updated);
                    TempData.Keep("SubGoals");
                }
                else
                {
                    TempData.Remove("SubGoals");
                    TempData.Remove("SubGoalForId");
                }
            }

            return RedirectToAction("BucketList");
        }

    }
}



