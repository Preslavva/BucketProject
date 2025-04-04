using System.Runtime.CompilerServices;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.DAL.Models.Enums;
using BucketProject.DAL.Models.Entities;



namespace BucketProject.DAL.Data.InterfacesRepo;

    public interface IGoalRepo
    {
    int GetIdOfUser(string username);
    void InsertGoalAndAssignToUser(int userId, GoalEntity goal);
    List<GoalEntity> LoadGoalsOfUserbyCategory(int userId, Category category);
    void UpdateGoalDescription(GoalEntity goal);
    void DeleteGoal(GoalEntity goal);
    void ChangeGoalStatus(GoalEntity goal);

    void PostponeGoal(GoalEntity goal);
    List<GoalEntity> LoadExpiredGoalsOfUser(int userId);
    List<GoalEntity> LoadGoalsOfUser(int userId);

    GoalEntity GetGoalById(int id);
} 
