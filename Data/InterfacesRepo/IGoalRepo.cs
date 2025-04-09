using System.Runtime.CompilerServices;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.DAL.Models.Enums;
using BucketProject.DAL.Models.Entities;



namespace BucketProject.DAL.Data.InterfacesRepo;

    public interface IGoalRepo
    {
    int GetIdOfUser(string username);
    void InsertGoalAndAssignToUser(int userId, Goal goal);
    List<Goal> LoadGoalsOfUserbyCategory(int userId, Category category);
    void UpdateGoalDescription(Goal goal);
    void DeleteGoal(Goal goal);
    void ChangeGoalStatus(Goal goal);

    void PostponeGoal(Goal goal);
    List<Goal> LoadExpiredGoalsOfUser(int userId);
    List<Goal> LoadGoalsOfUser(int userId);

    Goal GetGoalById(int id);
    List<Goal> LoadChildGoalsOfGoals(int goalId);
} 
