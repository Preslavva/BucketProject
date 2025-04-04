using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.UI.ViewModels.ViewModels;
using BucketProject.BLL.Business_Logic.Strategies;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.DAL.Models.Entities;




using Microsoft.AspNetCore.Http;
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

            // Step 1: Load raw entities from repo
            List<GoalEntity> goalEntities = _goalRepo.LoadGoalsOfUser(userId);

            // Step 2: Map to domain models
            List<Goal> goals = _mapper.Map<List<Goal>>(goalEntities);

            // Step 3: Business logic
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
