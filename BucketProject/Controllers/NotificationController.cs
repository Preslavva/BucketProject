
using AutoMapper;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLL.Business_Logic.Services;
using BucketProject.BLL.Business_Logic.Strategies;
using BucketProject.UI.ViewModels.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace BucketProject.BLL.Business_Logic.Controllers
{
    public class NotificationController : Controller
    {
        private readonly NotificationService _notificationService;
        private readonly GoalService _goalService;
        private readonly IMapper _mapper;

        public NotificationController(NotificationService notificationService, GoalService goalService, IMapper mapper)
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
            ViewBag.CurrentUserId = _goalService.GetCurrentUserId();

            DateTime today = DateTime.Today;

            var notifications = new List<NotificationViewModel>();

         
            var deadlineGoals = _notificationService.CheckAndNotify(today);
            foreach (var goal in deadlineGoals)
            {
                
                var vm = _mapper.Map<NotificationViewModel>(goal);

            
                var ds = DeadlineStrategyDeterminator.GetStrategy(goal.Category);
                var ns = NotificationStrategyManager.GetStrategy(goal.Category);
                if (ds == null || ns == null)
                    continue;

                var dl = ds.GetDeadline(goal.CreatedAt, goal.IsPostponed);
                if (!dl.HasValue)
                    continue;

                vm.Deadline = dl.Value;
                vm.Message = ns.GetNotificationMessage(goal.Description, dl.Value);

                notifications.Add(vm);
            }

          
            var completionGoals = _notificationService.GetSharedCompletionGoals();
            foreach (var goal in completionGoals)
            {
                var vm = _mapper.Map<NotificationViewModel>(goal);

           
                var completer = goal.Recipients.First();
                var when = goal.CompletedAt?.ToString("MMMM dd, yyyy");

                vm.Deadline = goal.CompletedAt ?? today;
                vm.Message = $"{completer.Username} completed the goal “{goal.Description}” on {when}.";

                notifications.Add(vm);
            }

            var deletedGoals = _notificationService.GetSharedDeletedGoals();
            foreach (var goal in deletedGoals)
            {
                var vm = _mapper.Map<NotificationViewModel>(goal);


                var completer = goal.Recipients.First();
             
                vm.Message = $" The goal “{goal.Description}” was deleted";

                notifications.Add(vm);
            }

            var postponedGoals = _notificationService.GetSharedPostponedGoals();
            foreach (var goal in postponedGoals)
            {
                var vm = _mapper.Map<NotificationViewModel>(goal);


                var completer = goal.Recipients.First();

                vm.Message = $" The goal “{goal.Description}” was postponed";

                notifications.Add(vm);
            }
            return View(notifications);
        }



        [HttpPost]
        public IActionResult PostponeGoal(int id)
        {
            _goalService.PostponeGoal(id);
            return RedirectToAction("Notification");
        }

    }
}
