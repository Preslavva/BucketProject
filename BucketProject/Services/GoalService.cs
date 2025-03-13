using BucketProject.Models;
using BucketProject.Repositories;

namespace BucketProject.Services
{
    public class GoalService
    {
        private readonly GoalRepo _goalRepo;

        public GoalService(GoalRepo goalRepo)
        {
            _goalRepo = goalRepo;
        }

        public void CreateGoal(Category category, string description, DateTime deadline, bool isDone, bool isDeleted, DateTime createdAt)
        {
            if (!string.IsNullOrWhiteSpace(description))
            {
                _goalRepo.CreateGoal(category, description, deadline, createdAt, isDone, isDeleted);
            }
        }

        public void AssignGoalToUser(User user, Goal goal)
        {
            _goalRepo.AssignGoalToUser(user, goal);
        }

        public List<Goal> LoadGoalsByCategory(User user, Category category)
        {
           return  _goalRepo.LoadGoalsOfUserbyCategory(user, category);
        }
    }
}
