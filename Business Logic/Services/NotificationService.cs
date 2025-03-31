using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.BLL.Business_Logic.Entity;
using BucketProject.BLL.Business_Logic.Strategies;
using Microsoft.AspNetCore.Http;
using BucketProject.DAL.Models.Enums;

namespace BucketProject.BLL.Business_Logic.Services
{
    public class NotificationService
    {

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGoalRepo _goalRepo;

        public NotificationService(IGoalRepo goalRepo, IHttpContextAccessor contextAccessor)
        {
            _goalRepo = goalRepo;
            _contextAccessor = contextAccessor;
        }


        public List<Goal> CheckAndNotify(DateTime today)
        {
            var notifiableGoals = new List<Goal>();

            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            if (username == null)
                return notifiableGoals;

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
                    notifiableGoals.Add(goal);
                }
            }

            return notifiableGoals;
        }

    }

}
