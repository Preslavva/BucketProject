using BucketProject.BLL.Business_Logic.Entity;
using BucketProject.DAL.Models.Enums;


namespace BucketProject.BLL.Business_Logic.InterfacesService
{
    public interface IGoalService
    {
        void CreateGoal(Category category, GoalType type, string description);
        List<Goal> LoadGoalsByCategory(Category category);
        void UpdateGoal(Goal goal, string description);
        void DeleteGoal(Goal goal);
        void ChangeGoalStatus(Goal goal, bool isDone);

        // Dictionary<Category, List<Goal>> LoadExpiredGoalsGroupedByCategory();

        public (
      Dictionary<string, Dictionary<GoalType, List<Goal>>> weekly,
      Dictionary<string, Dictionary<GoalType, List<Goal>>> monthly,
      Dictionary<string, Dictionary<GoalType, List<Goal>>> yearly
  ) LoadGroupedExpiredGoals();

    }
}
