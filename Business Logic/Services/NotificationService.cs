using System.Text;
using BucketProject.BLL.Business_Logic.Strategies;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.BLLBusiness_Logic.Domain;

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
            var notifications = new List<Goal>();
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
                if (!deadline.HasValue)
                    continue;

            
                if (notificationStrategy.ShouldNotify(today, deadline.Value))
                {
                   
                    List<UserEntity> userEntities = _goalRepo.LoadSharedUsersForGoal(
                       goal.Id,
                        userId
                    );
                    List<User> recipients = _mapper.Map<List<User>>(userEntities);

                    goal.Recipients = recipients;
                    notifications.Add(goal);
                }
            }

            return notifications;
        }
        public List<Goal> GetSharedCompletionGoals()
        {
            var username = _contextAccessor.HttpContext?.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return new List<Goal>();

            int currentUserId = _goalRepo.GetIdOfUser(username);

          
            List<GoalEntity> entities = _goalRepo.LoadSharedGoalsCompletedByOthers(currentUserId);

            List<Goal> flatGoals = _mapper.Map<List<Goal>>(entities);

            var grouped = flatGoals
                .GroupBy(g => g.Id)
                .Select(grp =>
                {
                    var master = grp.First();
                    master.Recipients = grp
                        .SelectMany(g => g.Recipients)
                        .ToList();

                    return master;
                })
                .ToList();

            return grouped;
        }

        public List<Goal> GetSharedDeletedGoals()
        {
            var username = _contextAccessor.HttpContext?.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return new List<Goal>();

            int currentUserId = _goalRepo.GetIdOfUser(username);


            List<GoalEntity> entities = _goalRepo.LoadSharedDeletedGoals(currentUserId);

            List<Goal> flatGoals = _mapper.Map<List<Goal>>(entities);

            var grouped = flatGoals
                .GroupBy(g => g.Id)
                .Select(grp =>
                {
                    var master = grp.First();
                    master.Recipients = grp
                        .SelectMany(g => g.Recipients)
                        .ToList();

                    return master;
                })
                .ToList();

            return grouped;
        }
        public List<Goal> GetSharedPostponedGoals()
        {
            var username = _contextAccessor.HttpContext?.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return new List<Goal>();

            int currentUserId = _goalRepo.GetIdOfUser(username);


            List<GoalEntity> entities = _goalRepo.LoadSharedPostponedGoals(currentUserId);

            List<Goal> flatGoals = _mapper.Map<List<Goal>>(entities);

            var grouped = flatGoals
                .GroupBy(g => g.Id)
                .Select(grp =>
                {
                    var master = grp.First();
                    master.Recipients = grp
                        .SelectMany(g => g.Recipients)
                        .ToList();

                    return master;
                })
                .ToList();

            return grouped;
        }
    }
}

