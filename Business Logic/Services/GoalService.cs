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


        public Dictionary<Category, Dictionary<string, Dictionary<GoalType, List<Goal>>>> LoadGroupedExpiredGoals()
        {
            var grouped = new Dictionary<Category, Dictionary<string, Dictionary<GoalType, List<Goal>>>>();

            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return grouped;

            int userId = _goalRepo.GetIdOfUser(username);
            var expiredGoals = _goalRepo.LoadExpiredGoalsOfUser(userId);

            foreach (var goal in expiredGoals)
            {
                DateTime date = goal.Deadline!.Value.Date;
                Category category = goal.Category;
                string key;

                switch (category)
                {
                    case Category.Week:
                        DateTime startOfWeek = date.AddDays(-(int)date.DayOfWeek);
                        DateTime endOfWeek = startOfWeek.AddDays(6);
                        key = $"Week {startOfWeek:dd.MM.yyyy} - {endOfWeek:dd.MM.yyyy}";
                        break;

                    case Category.Month:
                        key = $"{date:MMMM yyyy}";
                        break;

                    case Category.Year:
                        key = $"{date.Year}";
                        break;

                    default:
                        continue;
                }

                AddToGroup(grouped, category, key, goal);
            }

            return grouped;
        }



        private void AddToGroup(
      Dictionary<Category, Dictionary<string, Dictionary<GoalType, List<Goal>>>> group,
      Category category,
      string key,
      Goal goal)
        {
            if (!group.ContainsKey(category))
                group[category] = new Dictionary<string, Dictionary<GoalType, List<Goal>>>();

            if (!group[category].ContainsKey(key))
                group[category][key] = new Dictionary<GoalType, List<Goal>>();

            var type = goal.Type;

            if (!group[category][key].ContainsKey(type))
                group[category][key][type] = new List<Goal>();

            group[category][key][type].Add(goal);
        }

    }

}

