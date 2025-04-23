using System.Text;
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


        public List<Goal> CheckAndNotify(DateTime today)
        {
            List<Goal> notifications = new List<Goal>();

            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            if (username == null)
                return notifications;

            int userId = _goalRepo.GetIdOfUser(username);
            List<GoalEntity> goalEntities = _goalRepo.LoadGoalsOfUser(userId);
            List<Goal> goals = _mapper.Map<List<Goal>>(goalEntities);

            foreach (Goal goal in goals)
            {
                if (goal.IsDone)
                    continue;

                var deadlineStrategy = DeadlineStrategyDeterminator.GetStrategy(goal.Category);
                var notificationStrategy = NotificationStrategyManager.GetStrategy(goal.Category);

                if (deadlineStrategy == null || notificationStrategy == null)
                    continue;

                DateTime? deadline = deadlineStrategy.GetDeadline(goal.CreatedAt, goal.IsPostponed);

                if (deadline.HasValue && notificationStrategy.ShouldNotify(today, deadline.Value))
                {
                    notifications.Add(goal); 
                }
            }

            return notifications;
        }
    }
    }
