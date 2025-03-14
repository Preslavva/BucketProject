using System.Security.Claims;
using BucketProject.Models;
using BucketProject.Repositories;

namespace BucketProject.Services
{
    public class GoalService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly GoalRepo _goalRepo;

        public GoalService(GoalRepo goalRepo, IHttpContextAccessor contextAccessor)
        {
            _goalRepo = goalRepo;
            _contextAccessor = contextAccessor;

        }


        public void CreateGoal(Category category, string description)
        {
          string? username = _contextAccessor.HttpContext.Session.GetString("Username");

            int id = _goalRepo.GetIdOfUser(username);

            string[] descriptions = description.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var desc in descriptions)
            {
                if (!string.IsNullOrWhiteSpace(desc))
                {
                    Goal newGoal = new Goal(category, desc.Trim());
                    _goalRepo.InsertGoalAndAssignToUser(id, newGoal);
                }
            }
        }

        //public void AssignGoalToUser(User user, Goal goal)
        //{
        //    _goalRepo.AssignGoalToUser(user, goal);
        //}

        public List<Goal> LoadGoalsByCategory(Category category)
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            int id = _goalRepo.GetIdOfUser(username);
            return  _goalRepo.LoadGoalsOfUserbyCategory(id, category);
        }

       

    }
}
