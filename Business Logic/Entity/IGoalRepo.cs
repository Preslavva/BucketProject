using System.Runtime.CompilerServices;
using BucketProject.DAL.Models.Enums;


namespace BucketProject.BLL.Business_Logic.Entity;

    public interface IGoalRepo
    {
    int GetIdOfUser(string username);
    void InsertGoalAndAssignToUser(int userId, Goal goal);
    List<Goal> LoadGoalsOfUserbyCategory(int userId, Category category);

    void UpdateGoalDescription(Goal goal, string description);
    void DeleteGoal(Goal goal);
    void ChangeGoalStatus(Goal goal, bool isDone);

    int GetNumberOfAccomplishedGoalsByCategoryAndDeadline(Category category, DateTime deadline);

    void PostponeGoal(Goal goal, DateTime deadline);
} 
