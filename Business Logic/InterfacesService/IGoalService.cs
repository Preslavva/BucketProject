using BucketProject.DAL.Models.Entities;
using BucketProject.DAL.Models.Enums;


namespace BucketProject.BLL.Business_Logic.InterfacesService
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
