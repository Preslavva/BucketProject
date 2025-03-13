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

        public void CreateGoal(Category category, string description)
        {
            string[] descriptions = description.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var desc in descriptions)
            {
                if (!string.IsNullOrWhiteSpace(desc))
                {
                    Goal newGoal = new Goal(category, desc.Trim()); 
                    _goalRepo.CreateGoal(newGoal);
                }
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
