
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
        
        private readonly GoalService _goalService;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService, GoalService goalService, IMapper mapper)
        {
            _notificationService = notificationService;
            _goalService = goalService;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Notification()
        {
            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;

            int currentUserId = _goalService.GetCurrentUserId();
            ViewBag.CurrentUserId = currentUserId;

            DateTime today = DateTime.Today;
            List<NotificationViewModel> notifications = new List<NotificationViewModel>();

            // deadline notifications
            List<Goal> deadlineGoals = _notificationService.CheckAndNotify(today);
            foreach (Goal goal in deadlineGoals)
            {
                var ds = DeadlineStrategyDeterminator.GetStrategy(goal.Category);
                var ns = NotificationStrategyManager.GetStrategy(goal.Category);
                if (ds == null || ns == null)
                    continue;

                var dl = ds.GetDeadline(goal.CreatedAt, goal.IsPostponed);
                if (!dl.HasValue)
                    continue;

                NotificationViewModel vm = _mapper.Map<NotificationViewModel>(goal);
                vm.TypeOfNotification = "Deadline";
                vm.Message = ns.GetNotificationMessage(goal.Description, dl.Value);
              //  vm.TriggeredByUserId = goal.Recipients.First().Id; 


                notifications.Add(vm);
            }

            //completion notifications

            List<Goal> completionGoals = _notificationService.GetSharedCompletionGoals();
            foreach (Goal goal in completionGoals)
            {
                NotificationViewModel vm = _mapper.Map<NotificationViewModel>(goal);
                vm.TypeOfNotification = "Completion";
                vm.TriggeredByUserId = goal.Recipients.First().Id; 

                var completer = goal.Recipients.First();
                var when = goal.CompletedAt?.ToString("MMMM dd, yyyy");
      
                vm.Message = $"{completer.Username} completed the goal “{goal.Description}” on {when}.";

                notifications.Add(vm);
            }

            // deleted notifications
            List<Goal> deletedGoals = _notificationService.GetSharedDeletedGoals();
            foreach (Goal goal in deletedGoals)
            {
                NotificationViewModel vm = _mapper.Map<NotificationViewModel>(goal);
                vm.TypeOfNotification = "Deleted";
                vm.Id = goal.Id;
                vm.TriggeredByUserId = goal.Recipients.First().Id; 


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
                vm.TriggeredByUserId = goal.Recipients.First().Id; 

                vm.Message = $"The goal “{goal.Description}” was postponed.";
                notifications.Add(vm);
            }

            TempData["NotificationCount"] = notifications.Count;
            HttpContext.Session.SetInt32("NotCounter", notifications.Count);

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
            int userId = _goalService.GetCurrentUserId();
            _notificationService.DismissNotification(userId, goalId, notificationType, triggeredByUserId);
            return RedirectToAction("Notification");
        }


    }
}
