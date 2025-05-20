
using BucketProject.DAL.Models.Enums;
using BucketProject.DAL.Models.Entities;


namespace BucketProject.DAL.Data.InterfacesRepo;

    public interface IGoalRepo
    {
    //int GetIdOfUser(string username);
 
     List<GoalEntity> LoadPersonalGoalsOfUserbyCategory(int userId, Category category);
    List<GoalEntity> LoadSharedGoalsOfUserByCategory(int userId, Category category);

    void UpdateGoalDescription(GoalEntity goal);
    void DeleteGoal(GoalEntity goal);
    void ChangeGoalStatus(GoalEntity goal, int userId);

    void PostponeGoal(GoalEntity goal);
    List<GoalEntity> LoadExpiredGoalsOfUser(int userId);
    List<GoalEntity> LoadGoalsOfUser(int userId);

    GoalEntity GetGoalById(int goalId, int userId);
    List<UserEntity> LoadSharedUsersForGoal(int goalId, int ownerId);
   
    int InsertGoal(int ownerUserId, GoalEntity goal);
    void AssignUsersToGoal(int goalId, IEnumerable<int> userIds);
    List<GoalEntity> LoadSharedGoalsCompletedByOthers(int currentUserId);
    List<GoalEntity> LoadSharedDeletedGoals(int currentUserId);
    List<GoalEntity> LoadSharedPostponedGoals(int currentUserId);
    void DismissNotification(int userId, int goalId, string type, int triggeredByUserId);
    List<GoalEntity> LoadSharedGoalsOfUser(int userId);
    List<GoalEntity> LoadPersonalGoalsOfUser(int userId);

}
