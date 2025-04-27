
using Microsoft.AspNetCore.Mvc;
using BucketProject.BLL.Business_Logic.InterfacesService;
using AutoMapper;
using Newtonsoft.Json;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.UI.ViewModels.ViewModels;
using BucketProjetc.BLL.Business_Logic.InterfacesService;
using BucketProject.BLLBusiness_Logic.Domain;

namespace BucketProject.UI.BucketProject.Controllers
{
    public class GoalController : Controller
    {
        private readonly IGoalService _goalService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ISocialService _socialService;
        private readonly IUserService _userService;



        public GoalController(IGoalService goalService, IMapper mapper, IConfiguration configuration, ISocialService socialService, IUserService userService)
        {
            _goalService = goalService;
            _mapper = mapper;
            _configuration = configuration;
            _socialService = socialService;
            _userService = userService;

        }
        private int CurrentUserId
        {
            get
            {

                User currentUser = _userService.GetUserByUsername();
                return currentUser.Id;
            }
        }

        [HttpPost]
        public IActionResult CreateMonthGoal(GoalViewModel viewModel, int[] friendIds)
        {
            viewModel.Category = "Month";

            if (!ModelState.IsValid)
            {
                ViewBag.AvailableTypes = GetAvailableTypes();
                ViewBag.Friends = _socialService.GetFriends(CurrentUserId);
                TempData.Keep("SubGoals");
                TempData.Keep("SubGoalForId");
                return RedirectToAction("MonthGoals");
            }

            Goal domainModel = _mapper.Map<Goal>(viewModel);
            _goalService.CreateGoal(domainModel, friendIds);

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

            return RedirectToAction("MonthGoals");
        }


        [HttpPost]
        public IActionResult CreateYearGoal(GoalViewModel viewModel, int[] friendIds)
        {
            viewModel.Category = "Year";


            if (!ModelState.IsValid)
            {
                ViewBag.AvailableTypes = GetAvailableTypes();
                ViewBag.Friends = _socialService.GetFriends(CurrentUserId);
                TempData.Keep("SubGoals");
                TempData.Keep("SubGoalForId");
                return RedirectToAction("YearGoals");
            }

            Goal domainModel = _mapper.Map<Goal>(viewModel);
            _goalService.CreateGoal(domainModel, friendIds);

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

            return RedirectToAction("YearGoals");
        }

        [HttpPost]
        public IActionResult CreateBucketListGoal(GoalViewModel viewModel, int[] friendIds)
        {
            viewModel.Category = "Bucket_list";

            if (!ModelState.IsValid)
            {
                ViewBag.AvailableTypes = GetAvailableTypes();
                ViewBag.Friends = _socialService.GetFriends(CurrentUserId);
                TempData.Keep("SubGoals");
                TempData.Keep("SubGoalForId");
                return RedirectToAction("BucketListGoals");
            }

            Goal domainModel = _mapper.Map<Goal>(viewModel);
            _goalService.CreateGoal(domainModel, friendIds);

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
            return RedirectToAction("BucketList");
        }

        [HttpPost]
        public IActionResult CreateWeekGoal(GoalViewModel viewModel, int[]friendIds)
        {
            viewModel.Category = "Week";

            if (!ModelState.IsValid)
            {
                ViewBag.AvailableTypes = GetAvailableTypes();
                ViewBag.Friends = _socialService.GetFriends(CurrentUserId);
                TempData.Keep("SubGoals");
                TempData.Keep("SubGoalForId");
                return RedirectToAction("WeekGoals");
            }

            Goal domainModel = _mapper.Map<Goal>(viewModel);
            _goalService.CreateGoal(domainModel,friendIds);

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
            List<Goal> personalDomains = _goalService.LoadPersonalGoalsByCategory("Week");
            List<Goal> sharedDomains = _goalService.LoadSharedGoalsByCategory("Week");

           
            List<GoalViewModel> personalVMs = _mapper.Map<List<GoalViewModel>>(personalDomains);
            List<GoalViewModel> sharedVMs = _mapper.Map<List<GoalViewModel>>(sharedDomains);

        
            List<GoalViewModel> personalParents = personalVMs
                .Where(g => g.ParentGoalId == null)
                .ToList();
            foreach (var p in personalParents)
            {
                p.Children = personalVMs
                    .Where(c => c.ParentGoalId == p.Id)
                    .ToList();
            }

           
            List<GoalViewModel> sharedParents = sharedVMs
                .Where(g => g.ParentGoalId == null)
                .ToList();
            foreach (var p in sharedParents)
            {
                p.Children = sharedVMs
                    .Where(c => c.ParentGoalId == p.Id)
                    .ToList();
            }

        
            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = personalParents,
                SharedGoals = sharedParents
            };

            ViewBag.AvailableTypes = GetAvailableTypes();
            ViewBag.Friends = _socialService.GetFriends(CurrentUserId);

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
            List<Goal> personalDomains = _goalService.LoadPersonalGoalsByCategory("Year");
            List<Goal> sharedDomains = _goalService.LoadSharedGoalsByCategory("Year");


            List<GoalViewModel> personalVMs = _mapper.Map<List<GoalViewModel>>(personalDomains);
            List<GoalViewModel> sharedVMs = _mapper.Map<List<GoalViewModel>>(sharedDomains);


            List<GoalViewModel> personalParents = personalVMs
                .Where(g => g.ParentGoalId == null)
                .ToList();
            foreach (var p in personalParents)
            {
                p.Children = personalVMs
                    .Where(c => c.ParentGoalId == p.Id)
                    .ToList();
            }


            List<GoalViewModel> sharedParents = sharedVMs
                .Where(g => g.ParentGoalId == null)
                .ToList();
            foreach (var p in sharedParents)
            {
                p.Children = sharedVMs
                    .Where(c => c.ParentGoalId == p.Id)
                    .ToList();
            }


            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = personalParents,
                SharedGoals = sharedParents
            };

            ViewBag.AvailableTypes = GetAvailableTypes();
            ViewBag.Friends = _socialService.GetFriends(CurrentUserId);

            return View(pageModel);
        }

        [HttpGet]
        public IActionResult BucketList()
        {

            List<Goal> personalDomains = _goalService.LoadPersonalGoalsByCategory("Bucket_list");
            List<Goal> sharedDomains = _goalService.LoadSharedGoalsByCategory("Bucket_list");


            List<GoalViewModel> personalVMs = _mapper.Map<List<GoalViewModel>>(personalDomains);
            List<GoalViewModel> sharedVMs = _mapper.Map<List<GoalViewModel>>(sharedDomains);


            List<GoalViewModel> personalParents = personalVMs
                .Where(g => g.ParentGoalId == null)
                .ToList();
            foreach (var p in personalParents)
            {
                p.Children = personalVMs
                    .Where(c => c.ParentGoalId == p.Id)
                    .ToList();
            }


            List<GoalViewModel> sharedParents = sharedVMs
                .Where(g => g.ParentGoalId == null)
                .ToList();
            foreach (var p in sharedParents)
            {
                p.Children = sharedVMs
                    .Where(c => c.ParentGoalId == p.Id)
                    .ToList();
            }


            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = personalParents,
                SharedGoals = sharedParents
            };

            ViewBag.AvailableTypes = GetAvailableTypes();
            ViewBag.Friends = _socialService.GetFriends(CurrentUserId);

            return View(pageModel);
        }
        [HttpGet]
        public IActionResult MonthGoals()
        {
            List<Goal> personalDomains = _goalService.LoadPersonalGoalsByCategory("Month");
            List<Goal> sharedDomains = _goalService.LoadSharedGoalsByCategory("Month");


            List<GoalViewModel> personalVMs = _mapper.Map<List<GoalViewModel>>(personalDomains);
            List<GoalViewModel> sharedVMs = _mapper.Map<List<GoalViewModel>>(sharedDomains);


            List<GoalViewModel> personalParents = personalVMs
                .Where(g => g.ParentGoalId == null)
                .ToList();
            foreach (var p in personalParents)
            {
                p.Children = personalVMs
                    .Where(c => c.ParentGoalId == p.Id)
                    .ToList();
            }


            List<GoalViewModel> sharedParents = sharedVMs
                .Where(g => g.ParentGoalId == null)
                .ToList();
            foreach (var p in sharedParents)
            {
                p.Children = sharedVMs
                    .Where(c => c.ParentGoalId == p.Id)
                    .ToList();
            }


            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = personalParents,
                SharedGoals = sharedParents
            };

            ViewBag.AvailableTypes = GetAvailableTypes();
            ViewBag.Friends = _socialService.GetFriends(CurrentUserId);

            return View(pageModel);
        }

        [HttpGet]
        public IActionResult WeekGoalsPreview()
        {
            List<Goal> personalDomains = _goalService.LoadPersonalGoalsByCategory("Week");
            List<Goal> sharedDomains = _goalService.LoadSharedGoalsByCategory("Week");


            List<GoalViewModel> personalVMs = _mapper.Map<List<GoalViewModel>>(personalDomains);
            List<GoalViewModel> sharedVMs = _mapper.Map<List<GoalViewModel>>(sharedDomains);


            List<GoalViewModel> personalParents = personalVMs
                .Where(g => g.ParentGoalId == null)
                .ToList();
            foreach (var p in personalParents)
            {
                p.Children = personalVMs
                    .Where(c => c.ParentGoalId == p.Id)
                    .ToList();
            }


            List<GoalViewModel> sharedParents = sharedVMs
                .Where(g => g.ParentGoalId == null)
                .ToList();
            foreach (var p in sharedParents)
            {
                p.Children = sharedVMs
                    .Where(c => c.ParentGoalId == p.Id)
                    .ToList();
            }


            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = personalParents,
                SharedGoals = sharedParents
            };

           

            return View(pageModel);
        }

        [HttpGet]
        public IActionResult MonthGoalsPreview()
        {
            List<Goal> personalDomains = _goalService.LoadPersonalGoalsByCategory("Month");
            List<Goal> sharedDomains = _goalService.LoadSharedGoalsByCategory("Month");


            List<GoalViewModel> personalVMs = _mapper.Map<List<GoalViewModel>>(personalDomains);
            List<GoalViewModel> sharedVMs = _mapper.Map<List<GoalViewModel>>(sharedDomains);


            List<GoalViewModel> personalParents = personalVMs
                .Where(g => g.ParentGoalId == null)
                .ToList();
            foreach (var p in personalParents)
            {
                p.Children = personalVMs
                    .Where(c => c.ParentGoalId == p.Id)
                    .ToList();
            }


            List<GoalViewModel> sharedParents = sharedVMs
                .Where(g => g.ParentGoalId == null)
                .ToList();
            foreach (var p in sharedParents)
            {
                p.Children = sharedVMs
                    .Where(c => c.ParentGoalId == p.Id)
                    .ToList();
            }


            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = personalParents,
                SharedGoals = sharedParents
            };

            return View(pageModel);
        }

        [HttpGet]
        public IActionResult YearGoalsPreview()
        {
            List<Goal> personalDomains = _goalService.LoadPersonalGoalsByCategory("Year");
            List<Goal> sharedDomains = _goalService.LoadSharedGoalsByCategory("Year");


            List<GoalViewModel> personalVMs = _mapper.Map<List<GoalViewModel>>(personalDomains);
            List<GoalViewModel> sharedVMs = _mapper.Map<List<GoalViewModel>>(sharedDomains);


            List<GoalViewModel> personalParents = personalVMs
                .Where(g => g.ParentGoalId == null)
                .ToList();
            foreach (var p in personalParents)
            {
                p.Children = personalVMs
                    .Where(c => c.ParentGoalId == p.Id)
                    .ToList();
            }


            List<GoalViewModel> sharedParents = sharedVMs
                .Where(g => g.ParentGoalId == null)
                .ToList();
            foreach (var p in sharedParents)
            {
                p.Children = sharedVMs
                    .Where(c => c.ParentGoalId == p.Id)
                    .ToList();
            }


            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = personalParents,
                SharedGoals = sharedParents
            };

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





