using System.Security.Claims;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.BLL.Business_Logic.Entity;
using BucketProject.DAL.Models.Enums;
using Microsoft.AspNetCore.Http;


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
