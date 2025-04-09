
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
            ViewBag.Username = HttpContext.Session.GetString("Username");

            DateTime today = DateTime.Today;
            List<GoalDomain> goalDomains = _notificationService.CheckAndNotify(today);

            List<NotificationViewModel> notifications = new List<NotificationViewModel>();

            foreach (var goal in goalDomains)
            {
                NotificationViewModel vm = _mapper.Map<NotificationViewModel>(goal);

                var deadlineStrategy = DeadlineStrategyDeterminator.GetStrategy(goal.Category);
                var notificationStrategy = NotificationStrategyManager.GetStrategy(goal.Category);

                if (deadlineStrategy == null || notificationStrategy == null)
                    continue;

                DateTime? deadline = deadlineStrategy.GetDeadline(goal.CreatedAt, goal.IsPostponed);

                if (deadline.HasValue)
                {
                    vm.Deadline = deadline.Value;
                    vm.Message = notificationStrategy.GetNotificationMessage(goal.Description, deadline.Value);
                    notifications.Add(vm);
                }
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
