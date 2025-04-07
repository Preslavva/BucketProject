
using BucketProject.DAL.Models.Entities;
using BucketProject.UI.ViewModels.ViewModels;


namespace BucketProject.BLL.Business_Logic.InterfacesService
{
    public interface IGoalService
    {
        void CreateGoal(GoalViewModel viewModel);
        List<GoalViewModel> LoadGoalsByCategory(string category);
        void UpdateGoal(int goalId, GoalViewModel viewModel);
        void DeleteGoal(int goalId);
        void PostponeGoal(int goalId);

        void ChangeGoalStatus(int goalId, bool isDone);

        Dictionary<string, Dictionary<string, Dictionary<string, List<HistoryViewModel>>>> LoadGroupedExpiredGoals();
        Task<List<GoalViewModel>> BreakDownGoalAsync(int goalId);
        List<GoalViewModel> LoadChildGoalsOfGoal(int goalId);

    }
}
