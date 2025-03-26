using System.Runtime.CompilerServices;
using BucketProject.DAL.Models.Entities;
using BucketProject.DAL.Models.Enums;


namespace BucketProject.DAL.Data.InterfacesRepo
{
        public interface IGoalRepo
        {
        int GetIdOfUser(string username);
        void InsertGoalAndAssignToUser(int userId, Goal goal);
        List<Goal> LoadGoalsOfUserbyCategory(int userId, Category category);

        void UpdateGoalDescription(Goal goal, string description);
        void DeleteGoal(Goal goal);
        void ChangeGoalStatus(Goal goal, bool isDone);
    } 
}
