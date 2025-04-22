
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.UI.ViewModels.ViewModels;


namespace BucketProject.BLL.Business_Logic.InterfacesService
{
    public interface IGoalService
    {
        void CreateGoal(Goal goal);
        List<Goal> LoadGoalsByCategory(string category);
        void UpdateGoal(int goalId, Goal goalDomain);
        void DeleteGoal(int goalId);
        void PostponeGoal(int goalId);

        void ChangeGoalStatus(int goalId, bool isDone);

        Dictionary<string, Dictionary<string, Dictionary<string, List<Goal>>>> LoadGroupedExpiredGoals();
        Task<List<Goal>> BreakDownGoalAsync(int goalId);

    }
}
