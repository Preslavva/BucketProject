using System.Runtime.CompilerServices;
using BucketProject.Models;
using Microsoft.Extensions.Configuration;

namespace BucketProject.Interfaces
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
