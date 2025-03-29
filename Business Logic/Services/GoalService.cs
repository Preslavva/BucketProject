using System.Security.Claims;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.BLL.Business_Logic.Entity;
using BucketProject.DAL.Models.Enums;
using Microsoft.AspNetCore.Http;
using System.Globalization;


namespace BucketProject.BLL.Business_Logic.Services
{
    public class GoalService: IGoalService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGoalRepo _goalRepo;

        public GoalService(IGoalRepo goalRepo, IHttpContextAccessor contextAccessor)
        {
            _goalRepo = goalRepo;
            _contextAccessor = contextAccessor;
        }

        public void CreateGoal(Category category, GoalType type, string description)
        {
          string? username = _contextAccessor.HttpContext.Session.GetString("Username");

            int id = _goalRepo.GetIdOfUser(username);
            Goal newGoal = new Goal(category, type, description);
            _goalRepo.InsertGoalAndAssignToUser(id, newGoal);
        }

        public List<Goal> LoadGoalsByCategory(Category category)
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            int id = _goalRepo.GetIdOfUser(username);

            var allGoals = _goalRepo.LoadGoalsOfUserbyCategory(id, category);

            var today = DateTime.Today;

            var nonExpiredGoals = allGoals
                .Where(g => g.Deadline.HasValue && g.Deadline.Value.Date >= today)
                .ToList();

            return nonExpiredGoals;
        }


        public void UpdateGoal(Goal goal, string description)
        {
            _goalRepo.UpdateGoalDescription(goal, description);
        }
        public void DeleteGoal(Goal goal)
        {
            _goalRepo.DeleteGoal(goal);
        }
       
        public void ChangeGoalStatus(Goal goal, bool isDone)
        {
            _goalRepo.ChangeGoalStatus(goal, isDone);
        }

        //public Dictionary<Category, List<Goal>> LoadExpiredGoalsGroupedByCategory()
        //{
        //    string? username = _contextAccessor.HttpContext.Session.GetString("Username");
        //    if (string.IsNullOrEmpty(username))
        //        return new Dictionary<Category, List<Goal>>();

        //    int userId = _goalRepo.GetIdOfUser(username);

        //    var expiredGoals = _goalRepo.LoadExpiredGoalsOfUser(userId);

        //    var grouped = expiredGoals
        //        .Where(g => g.Deadline.HasValue && g.Deadline.Value.Date < DateTime.Today)
        //        .GroupBy(g => g.Category)
        //        .ToDictionary(g => g.Key, g => g.ToList());

        //    return grouped;
        //}

        public (
           Dictionary<string, Dictionary<GoalType, List<Goal>>> weekly,
           Dictionary<string, Dictionary<GoalType, List<Goal>>> monthly,
           Dictionary<string, Dictionary<GoalType, List<Goal>>> yearly
       ) LoadGroupedExpiredGoals()
        {
            var weekly = new Dictionary<string, Dictionary<GoalType, List<Goal>>>();
            var monthly = new Dictionary<string, Dictionary<GoalType, List<Goal>>>();
            var yearly = new Dictionary<string, Dictionary<GoalType, List<Goal>>>();

            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return (weekly, monthly, yearly);

            int userId = _goalRepo.GetIdOfUser(username);
            var expiredGoals = _goalRepo.LoadExpiredGoalsOfUser(userId)
                .Where(g => g.Deadline.HasValue && g.Deadline.Value.Date < DateTime.Today);

            foreach (var goal in expiredGoals)
            {
                var date = goal.Deadline!.Value.Date;

                switch (goal.Category)
                {
                    case Category.Week:
                        var startOfWeek = date.AddDays(-(int)date.DayOfWeek);
                        var endOfWeek = startOfWeek.AddDays(6);
                        var weekKey = $"Week {startOfWeek:dd.MM.yyyy} - {endOfWeek:dd.MM.yyyy}";
                        AddToGroup(weekly, weekKey, goal);
                        break;

                    case Category.Month:
                        var monthKey = $"{date:MMMM yyyy}";
                        AddToGroup(monthly, monthKey, goal);
                        break;

                    case Category.Year:
                        var yearKey = $"{date.Year}";
                        AddToGroup(yearly, yearKey, goal);
                        break;
                }
            }

            return (weekly, monthly, yearly);
        }


        private void AddToGroup(Dictionary<string, Dictionary<GoalType, List<Goal>>> group, string key, Goal goal)
        {
            var type = goal.Type;

            if (!group.ContainsKey(key))
                group[key] = new Dictionary<GoalType, List<Goal>>();

            if (!group[key].ContainsKey(type))
                group[key][type] = new List<Goal>();

            group[key][type].Add(goal);
        }

    }

}

