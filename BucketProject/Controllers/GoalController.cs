
using Microsoft.AspNetCore.Mvc;
using BucketProject.BLL.Business_Logic.InterfacesService;
using AutoMapper;
using Newtonsoft.Json;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.UI.ViewModels.ViewModels;
using BucketProjetc.BLL.Business_Logic.InterfacesService;
using BucketProject.DAL.Models.Entities;
using System.ComponentModel.DataAnnotations;
using BucketProject.BLL.Business_Logic.Services;
using BucketProject.BLL.Business_Logic.DTOs;
using Exceptions.Exceptions;

namespace BucketProject.UI.BucketProject.Controllers
{
    public class GoalController : Controller
    {
        private readonly IGoalService _goalService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ISocialService _socialService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<GoalController> _logger;


        public GoalController(IGoalService goalService, IMapper mapper, IConfiguration configuration, ISocialService socialService, IUserService userService, INotificationService notificationService, ILogger<GoalController> logger)
        {
            _goalService = goalService;
            _mapper = mapper;
            _configuration = configuration;
            _socialService = socialService;
            _userService = userService;
            _notificationService = notificationService;
            _logger = logger;

        }

        [HttpPost]
        public IActionResult CreateMonthGoal(GoalViewModel viewModel, int[] friendIds)
        {
            viewModel.Category = "Month";

         
                ViewBag.AvailableTypes = GetAvailableTypes();
                ViewBag.Friends = _socialService.GetFriends(_userService.GetCurrentUserId());
                TempData.Keep("SubGoals");
                TempData.Keep("SubGoalForId");
                
            try
            {
                Goal domainModel = _mapper.Map<Goal>(viewModel);
                _goalService.CreateGoal(domainModel, friendIds);
            }
            catch (ValidationException vex)
            {
                TempData["ErrorMessage"] = vex.Message;
            }

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

                ViewBag.AvailableTypes = GetAvailableTypes();
                ViewBag.Friends = _socialService.GetFriends(_userService.GetCurrentUserId());
                TempData.Keep("SubGoals");
                TempData.Keep("SubGoalForId");
              
            try
            {
                Goal domainModel = _mapper.Map<Goal>(viewModel);
                _goalService.CreateGoal(domainModel, friendIds);
            }
            catch (ValidationException vex)
            {
                TempData["ErrorMessage"] = vex.Message;
            }

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

           
                ViewBag.AvailableTypes = GetAvailableTypes();
                ViewBag.Friends = _socialService.GetFriends(_userService.GetCurrentUserId());
                TempData.Keep("SubGoals");
                TempData.Keep("SubGoalForId");
            
            try
            {
                Goal domainModel = _mapper.Map<Goal>(viewModel);
                _goalService.CreateGoal(domainModel, friendIds);
            }
            catch (ValidationException vex)
            {
                TempData["ErrorMessage"] = vex.Message;
            }

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
        public IActionResult CreateWeekGoal(GoalViewModel viewModel, int[] friendIds)
        {
            viewModel.Category = "Week";

            ViewBag.AvailableTypes = GetAvailableTypes();
            ViewBag.Friends = _socialService.GetFriends(_userService.GetCurrentUserId());
            TempData.Keep("SubGoals");
            TempData.Keep("SubGoalForId");

            try
            {
                Goal domainModel = _mapper.Map<Goal>(viewModel);
                _goalService.CreateGoal(domainModel, friendIds);
            }
            catch (ValidationException vex)
            {
                TempData["ErrorMessage"] = vex.Message;
            }

            if (TempData.ContainsKey("SubGoals"))
            {
                string raw = TempData["SubGoals"]?.ToString();
                if (!string.IsNullOrWhiteSpace(raw))
                {
                    var subGoals = JsonConvert.DeserializeObject<List<GoalViewModel>>(raw);

                    var updated = subGoals!
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
            
                try
                {
                    var updated = _mapper.Map<Goal>(viewModel);
                    _goalService.UpdateGoal(viewModel.Id, updated);
                }
                catch (ValidationException vex)
                {
                   
                    TempData["ErrorMessage"] = vex.Message;
                }
                return RedirectToAction("WeekGoals");
            
        }


        [HttpPost]
        public IActionResult EditMonthGoal(GoalViewModel viewModel)
        {
            try
            {
                var updated = _mapper.Map<Goal>(viewModel);
                _goalService.UpdateGoal(viewModel.Id, updated);
            }
            catch (ValidationException vex)
            {

                TempData["ErrorMessage"] = vex.Message;
            }
            return RedirectToAction("MonthGoals");
        }


        [HttpPost]
        public IActionResult EditYearGoal(GoalViewModel viewModel)
        {

            try
            {
                var updated = _mapper.Map<Goal>(viewModel);
                _goalService.UpdateGoal(viewModel.Id, updated);
            }
            catch (ValidationException vex)
            {

                TempData["ErrorMessage"] = vex.Message;
            }
            return RedirectToAction("YearGoals");
        }


        [HttpPost]
        public IActionResult EdiBucketListGoal(GoalViewModel viewModel)
        {
            try
            {
                var updated = _mapper.Map<Goal>(viewModel);
                _goalService.UpdateGoal(viewModel.Id, updated);
            }
            catch (ValidationException vex)
            {

                TempData["ErrorMessage"] = vex.Message;
            }
            return RedirectToAction("BucketList");
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


            List<GoalInviteDTO> pendingInvitations = _goalService.GetPendingInvitations(_userService.GetCurrentUserId(), "Week");


            List<GoalInviteViewModel> pendingInvitationVMs = pendingInvitations
                .Select(inv => new GoalInviteViewModel
                {
                    InvitationId = inv.Id,
                    GoalDescription = _goalService.GetGoalDescription(inv.GoalId),
                    InviterUsername = _socialService.GetUsername(inv.InviterId),
                    ParentGoalDescription = _goalService.GetParentGoalDescription(inv.GoalId)
                })
                .ToList();




            List<GoalInviteDTO> sentInvitationsOfUser = _goalService.GetInvitationsOf(_userService.GetCurrentUserId(), "Week");
            List<GoalInviteViewModel> sentInvitationsOFVMs = sentInvitationsOfUser
  .Select(inv => new GoalInviteViewModel
  {
      InvitationId = inv.Id,
      GoalDescription = _goalService.GetGoalDescription(inv.GoalId),
      CreatedAt = _goalService.GetCreatedAt(inv.GoalId),
      InvitedUsername = _socialService.GetUsername(inv.InvitedId),
      Status = _goalService.GetInvitationStatus(inv.GoalId, inv.InvitedId),
      ParentGoalDescription = _goalService.GetParentGoalDescription(inv.GoalId),
      GoalId = inv.GoalId,
      TriggeredByUserId = inv.InvitedId
  })
  .ToList();


            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = personalParents,
                SharedGoals = sharedParents,
                PendingInvitations = pendingInvitationVMs,
                SentInvitations = sentInvitationsOFVMs

            };

            ViewBag.AvailableTypes = GetAvailableTypes();
            ViewBag.Friends = _socialService.GetFriends(_userService.GetCurrentUserId());
            ViewBag.CurrentUserId = _userService.GetCurrentUserId();

            if (TempData.TryGetValue("ErrorMessage", out var obj)
       && obj is string msg && !string.IsNullOrEmpty(msg))
            {
                ViewBag.ErrorMessage = msg;
            }

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


            List<GoalInviteDTO> pendingInvitations = _goalService.GetPendingInvitations(_userService.GetCurrentUserId(), "Year");

            List<GoalInviteViewModel> pendingInvitationVMs = pendingInvitations
                .Select(inv => new GoalInviteViewModel
                {
                    InvitationId = inv.Id,
                    GoalDescription = _goalService.GetGoalDescription(inv.GoalId),
                    InviterUsername = _socialService.GetUsername(inv.InviterId),
                    ParentGoalDescription = _goalService.GetParentGoalDescription(inv.GoalId)
                })
                .ToList();




            List<GoalInviteDTO> sentInvitationsOfUser = _goalService.GetInvitationsOf(_userService.GetCurrentUserId(), "Year");
            List<GoalInviteViewModel> sentInvitationsOFVMs = sentInvitationsOfUser
  .Select(inv => new GoalInviteViewModel
  {
      InvitationId = inv.Id,
      GoalDescription = _goalService.GetGoalDescription(inv.GoalId),
      CreatedAt = _goalService.GetCreatedAt(inv.GoalId),
      InvitedUsername = _socialService.GetUsername(inv.InvitedId),
      Status = _goalService.GetInvitationStatus(inv.GoalId, inv.InvitedId),
      ParentGoalDescription = _goalService.GetParentGoalDescription(inv.GoalId),
      GoalId = inv.GoalId,
      TriggeredByUserId = inv.InvitedId
  })
  .ToList();


            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = personalParents,
                SharedGoals = sharedParents,
                PendingInvitations = pendingInvitationVMs,
                SentInvitations = sentInvitationsOFVMs

            };

            ViewBag.AvailableTypes = GetAvailableTypes();
            ViewBag.Friends = _socialService.GetFriends(_userService.GetCurrentUserId());
            ViewBag.CurrentUserId = _userService.GetCurrentUserId();

            if (TempData.TryGetValue("ErrorMessage", out var obj)
       && obj is string msg && !string.IsNullOrEmpty(msg))
            {
                ViewBag.ErrorMessage = msg;
            }

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


            List<GoalInviteDTO> pendingInvitations = _goalService.GetPendingInvitations(_userService.GetCurrentUserId(), "Bucket_list");

            List<GoalInviteViewModel> pendingInvitationVMs = pendingInvitations
                .Select(inv => new GoalInviteViewModel
                {
                    InvitationId = inv.Id,
                    GoalDescription = _goalService.GetGoalDescription(inv.GoalId),
                    InviterUsername = _socialService.GetUsername(inv.InviterId),
                    ParentGoalDescription = _goalService.GetParentGoalDescription(inv.GoalId)
                })
                .ToList();




            List<GoalInviteDTO> sentInvitationsOfUser = _goalService.GetInvitationsOf(_userService.GetCurrentUserId(), "Bucket_list");
            List<GoalInviteViewModel> sentInvitationsOFVMs = sentInvitationsOfUser
  .Select(inv => new GoalInviteViewModel
  {
      InvitationId = inv.Id,
      GoalDescription = _goalService.GetGoalDescription(inv.GoalId),
      CreatedAt = _goalService.GetCreatedAt(inv.GoalId),
      InvitedUsername = _socialService.GetUsername(inv.InvitedId),
      Status = _goalService.GetInvitationStatus(inv.GoalId, inv.InvitedId),
      ParentGoalDescription = _goalService.GetParentGoalDescription(inv.GoalId),
      GoalId = inv.GoalId,
      TriggeredByUserId = inv.InvitedId
  })
  .ToList();


            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = personalParents,
                SharedGoals = sharedParents,
                PendingInvitations = pendingInvitationVMs,
                SentInvitations = sentInvitationsOFVMs

            };

            ViewBag.AvailableTypes = GetAvailableTypes();
            ViewBag.Friends = _socialService.GetFriends(_userService.GetCurrentUserId());
            ViewBag.CurrentUserId = _userService.GetCurrentUserId();

            if (TempData.TryGetValue("ErrorMessage", out var obj)
       && obj is string msg && !string.IsNullOrEmpty(msg))
            {
                ViewBag.ErrorMessage = msg;
            }

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


            List<GoalInviteDTO> pendingInvitations = _goalService.GetPendingInvitations(_userService.GetCurrentUserId(), "Month");

            List<GoalInviteViewModel> pendingInvitationVMs = pendingInvitations
                .Select(inv => new GoalInviteViewModel
                {
                    InvitationId = inv.Id,
                    GoalDescription = _goalService.GetGoalDescription(inv.GoalId),
                    InviterUsername = _socialService.GetUsername(inv.InviterId),
                    ParentGoalDescription = _goalService.GetParentGoalDescription(inv.GoalId)
                })
                .ToList();


            List<GoalInviteDTO> sentInvitationsOfUser = _goalService.GetInvitationsOf(_userService.GetCurrentUserId(), "Month");
            List<GoalInviteViewModel> sentInvitationsOFVMs = sentInvitationsOfUser
  .Select(inv => new GoalInviteViewModel
  {
      InvitationId = inv.Id,
      GoalDescription = _goalService.GetGoalDescription(inv.GoalId),
      CreatedAt = _goalService.GetCreatedAt(inv.GoalId),
      InvitedUsername = _socialService.GetUsername(inv.InvitedId),
      Status = _goalService.GetInvitationStatus(inv.GoalId, inv.InvitedId),
      ParentGoalDescription = _goalService.GetParentGoalDescription(inv.GoalId),
      GoalId = inv.GoalId,
      TriggeredByUserId = inv.InvitedId
  })
  .ToList();


            GoalsPageViewModel pageModel = new GoalsPageViewModel
            {
                Goals = personalParents,
                SharedGoals = sharedParents,
                PendingInvitations = pendingInvitationVMs,
                SentInvitations = sentInvitationsOFVMs

            };

            ViewBag.AvailableTypes = GetAvailableTypes();
            ViewBag.Friends = _socialService.GetFriends(_userService.GetCurrentUserId());
            ViewBag.CurrentUserId = _userService.GetCurrentUserId();

            if (TempData.TryGetValue("ErrorMessage", out var obj)
       && obj is string msg && !string.IsNullOrEmpty(msg))
            {
                ViewBag.ErrorMessage = msg;
            }

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
            try
            {
                List<Goal> subGoals = await _goalService.BreakDownGoalAsync(id);

                List<GoalViewModel> subGoalViewModels = _mapper.Map<List<GoalViewModel>>(subGoals);

                TempData["SubGoals"] = JsonConvert.SerializeObject(subGoalViewModels);
                TempData["SubGoalForId"] = id;

                return RedirectToAction("WeekGoals");
            }
            catch (EmptyAIResponseException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("WeekGoals");
            }
            catch (VagueGoalDescriptionException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("WeekGoals");
            }
            catch (AIRequestFailedException ex)
            {
                TempData["ErrorMessage"] = ex.UserMessage;

                _logger.LogError(ex, "AI call failed during goal breakdown.");

                return RedirectToAction("WeekGoals");
            }
        }

            [HttpPost]
        public async Task<IActionResult> BreakDownGoalMonth(int id)
        {
            try
            {
                List<Goal> subGoals = await _goalService.BreakDownGoalAsync(id);

                List<GoalViewModel> subGoalViewModels = _mapper.Map<List<GoalViewModel>>(subGoals);

                TempData["SubGoals"] = JsonConvert.SerializeObject(subGoalViewModels);
                TempData["SubGoalForId"] = id;

                return RedirectToAction("MonthGoals");
            }
            catch (EmptyAIResponseException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("MonthGoals");
            }
            catch (VagueGoalDescriptionException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("MonthGoals");
            }
            catch (AIRequestFailedException ex)
            {
                TempData["ErrorMessage"] = ex.UserMessage;

                _logger.LogError(ex, "AI call failed during goal breakdown.");

                return RedirectToAction("MonthGoals");
            }

        }

        [HttpPost]
        public async Task<IActionResult> BreakDownGoalYear(int id)
        {
            try
            {
                List<Goal> subGoals = await _goalService.BreakDownGoalAsync(id);

                List<GoalViewModel> subGoalViewModels = _mapper.Map<List<GoalViewModel>>(subGoals);

                TempData["SubGoals"] = JsonConvert.SerializeObject(subGoalViewModels);
                TempData["SubGoalForId"] = id;

                return RedirectToAction("YearGoals");
            }
            catch (EmptyAIResponseException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("YearGoals");
            }
            catch (VagueGoalDescriptionException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("YearGoals");
            }
            catch (AIRequestFailedException ex)
            {
                TempData["ErrorMessage"] = ex.UserMessage;

                _logger.LogError(ex, "AI call failed during goal breakdown.");

                return RedirectToAction("YearGoals");
            }
        }

        [HttpPost]
        public async Task<IActionResult> BreakDownGoalBucketList(int id)
        {
            try
            {
                List<Goal> subGoals = await _goalService.BreakDownGoalAsync(id);

                List<GoalViewModel> subGoalViewModels = _mapper.Map<List<GoalViewModel>>(subGoals);

                TempData["SubGoals"] = JsonConvert.SerializeObject(subGoalViewModels);
                TempData["SubGoalForId"] = id;

                return RedirectToAction("BucketList");
            }
            catch (EmptyAIResponseException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("BucketList");
            }
            catch (VagueGoalDescriptionException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("BucketList");
            }
            catch (AIRequestFailedException ex)
            {
                TempData["ErrorMessage"] = ex.UserMessage;

                _logger.LogError(ex, "AI call failed during goal breakdown.");

                return RedirectToAction("BucketList");
            }

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
            "Other"
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
        [HttpPost]
        public IActionResult RespondInvitationWeek(int invitationId, bool accept)
        {
            int currentUserId = _userService.GetCurrentUserId();

            _goalService.RespondToInvitation(invitationId, accept, currentUserId);

            return RedirectToAction("WeekGoals");
        }
        [HttpPost]
        public IActionResult RespondInvitationMonth(int invitationId, bool accept)
        {
            int currentUserId = _userService.GetCurrentUserId();

            _goalService.RespondToInvitation(invitationId, accept, currentUserId);

            return RedirectToAction("MonthGoals");
        }
        [HttpPost]
        public IActionResult RespondInvitationYear(int invitationId, bool accept)
        {
            int currentUserId = _userService.GetCurrentUserId();

            _goalService.RespondToInvitation(invitationId, accept, currentUserId);

            return RedirectToAction("YearGoals");
        }
        [HttpPost]
        public IActionResult RespondInvitationBucketList(int invitationId, bool accept)
        {
            int currentUserId = _userService.GetCurrentUserId();

            _goalService.RespondToInvitation(invitationId, accept, currentUserId);

            return RedirectToAction("BucketList");
        }

        [HttpPost]
        public IActionResult DismissNotificationWeek(int goalId, string notificationType, int triggeredByUserId)
        {
            int userId = _userService.GetCurrentUserId();
            _notificationService.DismissNotification(userId, goalId, notificationType, triggeredByUserId);
            return RedirectToAction("WeekGoals");
        }

        [HttpPost]
        public IActionResult DismissNotificationMonth(int goalId, string notificationType, int triggeredByUserId)
        {
            int userId = _userService.GetCurrentUserId();
            _notificationService.DismissNotification(userId, goalId, notificationType, triggeredByUserId);
            return RedirectToAction("MonthGoals");
        }

        [HttpPost]
        public IActionResult DismissNotificationYear(int goalId, string notificationType, int triggeredByUserId)
        {
            int userId = _userService.GetCurrentUserId();
            _notificationService.DismissNotification(userId, goalId, notificationType, triggeredByUserId);
            return RedirectToAction("YearGoals");
        }

        [HttpPost]
        public IActionResult DismissNotificationBucketList(int goalId, string notificationType, int triggeredByUserId)
        {
            int userId = _userService.GetCurrentUserId();
            _notificationService.DismissNotification(userId, goalId, notificationType, triggeredByUserId);
            return RedirectToAction("BucketList");
        }
    }
}
