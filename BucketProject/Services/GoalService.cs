using System.Security.Claims;
using BucketProject.Interfaces;
using BucketProject.Models;
using BucketProject.Repositories;


namespace BucketProject.Services
{
    public class GoalService:IGoalService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGoalRepo _goalRepo;

        public GoalService(IGoalRepo goalRepo, IHttpContextAccessor contextAccessor)
        {
            _goalRepo = goalRepo;
            _contextAccessor = contextAccessor;
        }

        public void CreateGoal(Category category, string description)
        {
          string? username = _contextAccessor.HttpContext.Session.GetString("Username");

            int id = _goalRepo.GetIdOfUser(username);
            Goal newGoal = new Goal(category, description);
            _goalRepo.InsertGoalAndAssignToUser(id, newGoal);
        }

        public List<Goal> LoadGoalsByCategory(Category category)
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            int id = _goalRepo.GetIdOfUser(username);
            return  _goalRepo.LoadGoalsOfUserbyCategory(id, category);
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
    }
}
