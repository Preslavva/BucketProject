
using AutoMapper;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLL.Business_Logic.Services;
using BucketProject.BLL.Business_Logic.Strategies;
using BucketProject.BLL.Business_Logic.DTOs;
using BucketProject.UI.ViewModels.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace BucketProject.BLL.Business_Logic.Controllers
{
    public class NotificationController : Controller
    {
        
        private readonly IGoalService _goalService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService, IGoalService goalService, IMapper mapper, IUserService userService)
        {
            _notificationService = notificationService;
            _goalService = goalService;
            _mapper = mapper;
            _userService = userService;
        }
        [HttpGet]
        public IActionResult Notification()
        {
            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;

            int currentUserId = _userService.GetCurrentUserId();
            ViewBag.CurrentUserId = currentUserId;

            DateTime today = DateTime.Today;
            List<NotificationViewModel> notifications = new List<NotificationViewModel>();

            // deadline notifications
            List<Goal> deadlineGoals = _notificationService.CheckAndNotify(today);
            foreach (Goal goal in deadlineGoals)
            {
                IDeadlineStrategy? ds = DeadlineStrategyDeterminator.GetStrategy(goal.Category);
                INotificationStrategy? ns = NotificationStrategyManager.GetStrategy(goal.Category);
                if (ds == null || ns == null)
                    continue;

                DateTime? dl = ds.GetDeadline(goal.CreatedAt, goal.IsPostponed);
                if (!dl.HasValue)
                    continue;

                NotificationViewModel vm = _mapper.Map<NotificationViewModel>(goal);
                vm.TypeOfNotification = "Deadline";
                vm.Message = ns.GetNotificationMessage(goal.Description, dl.Value);
                if (goal.Recipients != null && goal.Recipients.Any())
                {
                    vm.TriggeredByUserId = goal.Recipients.First().Id;
                }
                else
                {
                    vm.TriggeredByUserId = goal.OwnerId;
                }


                notifications.Add(vm);
            }

            //completion notifications

            List<Goal> completionGoals = _notificationService.GetSharedCompletionGoals();
            foreach (var goal in completionGoals)
            { 
                foreach (var completer in goal.Recipients)
                {
                    NotificationViewModel vm = _mapper.Map<NotificationViewModel>(goal);
                    vm.TypeOfNotification = "Completion";
                    vm.TriggeredByUserId = completer.Id;

                    var when = goal.CompletedAt?.ToString("MMMM dd, yyyy") ?? "an unknown date";
                    vm.Message = $"{completer.Username} completed the goal “{goal.Description}” on {when}.";

                    notifications.Add(vm);
                }
            }

            // deleted notifications
            List<Goal> deletedGoals = _notificationService.GetSharedDeletedGoals();
            foreach (Goal goal in deletedGoals)
            {
                NotificationViewModel vm = _mapper.Map<NotificationViewModel>(goal);
                vm.TypeOfNotification = "Deleted";
                vm.Id = goal.Id;
                //vm.TriggeredByUserId = goal.Recipients.First().Id; 
                vm.TriggeredByUserId = goal.OwnerId;

                vm.Message = $"The goal “{goal.Description}” was deleted.";
                notifications.Add(vm);
            }

            // postponed notifications
            List<Goal> postponedGoals = _notificationService.GetSharedPostponedGoals();
            foreach (var goal in postponedGoals)
            {
                NotificationViewModel vm = _mapper.Map<NotificationViewModel>(goal);
                vm.TypeOfNotification = "Postponed";
                vm.Id = goal.Id;
                //vm.TriggeredByUserId = goal.Recipients.First().Id; 
                vm.TriggeredByUserId = goal.OwnerId;

                vm.Message = $"The goal “{goal.Description}” was postponed.";
                notifications.Add(vm);
            }

            TempData["NotificationCount"] = notifications.Count;

            return View(notifications);
        }


        [HttpPost]
        public IActionResult PostponeGoal(int id)
        {
            _goalService.PostponeGoal(id);
            return RedirectToAction("Notification");
        }

        [HttpPost]
        public IActionResult DismissNotification(int goalId, string notificationType, int triggeredByUserId)
        {
            int userId = _userService.GetCurrentUserId();
            _notificationService.DismissNotification(userId, goalId, notificationType, triggeredByUserId);
            return RedirectToAction("Notification");
        }


    }
}
