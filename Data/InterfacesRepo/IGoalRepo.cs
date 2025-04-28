
using BucketProject.DAL.Models.Enums;
using BucketProject.DAL.Models.Entities;



namespace BucketProject.DAL.Data.InterfacesRepo;

    public interface IGoalRepo
    {
    int GetIdOfUser(string username);
    void InsertGoalAndAssignToUsers(
     int ownerUserId,
     GoalEntity goal,
     IEnumerable<int> sharedWithUserIds);
 
     List<GoalEntity> LoadPersonalGoalsOfUserbyCategory(int userId, Category category);
    List<GoalEntity> LoadSharedGoalsOfUserByCategory(int userId, Category category);

    void UpdateGoalDescription(GoalEntity goal);
    void DeleteGoal(GoalEntity goal);
    void ChangeGoalStatus(GoalEntity goal);

    void PostponeGoal(GoalEntity goal);
    List<GoalEntity> LoadExpiredGoalsOfUser(int userId);
    List<GoalEntity> LoadGoalsOfUser(int userId);

    GoalEntity GetGoalById(int id);
    List<GoalEntity> LoadChildGoalsOfGoals(int goalId);
    List<UserEntity> LoadSharedUsersForGoal(int goalId, int ownerId);
    void AssignUserToGoal(int goalId, int userId);
} 
