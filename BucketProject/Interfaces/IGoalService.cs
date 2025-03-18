using BucketProject.Models;

namespace BucketProject.Interfaces
{
    public interface IGoalService
    {
        void CreateGoal(Category category, string description);
        List<Goal> LoadGoalsByCategory(Category category);
        void UpdateGoal(Goal goal, string description);
        void DeleteGoal(Goal goal);
        void ChangeGoalStatus(Goal goal, bool isDone);
    }
}
