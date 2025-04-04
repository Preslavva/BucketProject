using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.UI.ViewModels.ViewModels;
using BucketProject.BLL.Business_Logic.Strategies;
using BucketProject.BLL.Business_Logic.Entity;


using Microsoft.AspNetCore.Http;
using BucketProject.DAL.Models.Enums;
using AutoMapper;

namespace BucketProject.BLL.Business_Logic.Services
{
    public class NotificationService
    {

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGoalRepo _goalRepo;
        private readonly IMapper _mapper;

        public NotificationService(IGoalRepo goalRepo, IHttpContextAccessor contextAccessor, IMapper mapper)
        {
            _goalRepo = goalRepo;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }


        public List<NotificationViewModel> CheckAndNotify(DateTime today)
        {
            var notifications = new List<NotificationViewModel>();

            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            if (username == null)
                return notifications;

            int userId = _goalRepo.GetIdOfUser(username);
            List<Goal> goals = _goalRepo.LoadGoalsOfUser(userId);

            foreach (Goal goal in goals)
            {
                if (goal.IsDone)
                    continue;

                var deadlineStrategy = DeadlineStrategyManager.GetStrategy(goal.Category);
                var notificationStrategy = NotificationStrategyManager.GetStrategy(goal.Category);

                if (deadlineStrategy == null || notificationStrategy == null)
                    continue;

                DateTime? deadline = deadlineStrategy.GetDeadline(goal.CreatedAt, goal.IsPostponed);

                if (deadline.HasValue && notificationStrategy.ShouldNotify(today, deadline.Value))
                {
                    string message = notificationStrategy.GetNotificationMessage(goal.Description, deadline.Value);

                    var viewModel = _mapper.Map<NotificationViewModel>(goal);
                    viewModel.Message = message;
                    viewModel.Deadline = deadline;

                    notifications.Add(viewModel);
                }
            }

            return notifications;
        }

    }

}
