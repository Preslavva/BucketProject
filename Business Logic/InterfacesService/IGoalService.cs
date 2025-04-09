
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.UI.ViewModels.ViewModels;


namespace BucketProject.BLL.Business_Logic.InterfacesService
{
    public interface IGoalService
    {
        void CreateGoal(GoalDomain goal);
        List<GoalDomain> LoadGoalsByCategory(string category);
        void UpdateGoal(int goalId, GoalDomain goalDomain);
        void DeleteGoal(int goalId);
        void PostponeGoal(int goalId);

        void ChangeGoalStatus(int goalId, bool isDone);

        Dictionary<string, Dictionary<string, Dictionary<string, List<GoalDomain>>>> LoadGroupedExpiredGoals();
        Task<List<GoalDomain>> BreakDownGoalAsync(int goalId);
        //List<GoalViewModel> LoadChildGoalsOfGoal(int goalId);

    }
}
