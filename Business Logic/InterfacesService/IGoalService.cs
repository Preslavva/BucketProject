using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.DAL.Models.Entities;
using BucketProject.BLL.Business_Logic.DTOs;


namespace BucketProject.BLL.Business_Logic.InterfacesService
{
    public interface IGoalService
    {
        void CreateGoal(Goal goalDomain, IEnumerable<int> sharedWithUserIds);
        List<Goal> LoadPersonalGoalsByCategory(string category);
        List<Goal> LoadSharedGoalsByCategory(string category);

        void UpdateGoal(int goalId, Goal goalDomain);
        void DeleteGoal(int goalId);
        void PostponeGoal(int goalId);

        void ChangeGoalStatus(int goalId, bool isDone);

        Dictionary<string, Dictionary<string, Dictionary<string, List<Goal>>>> LoadGroupedExpiredGoals();
        Task<List<Goal>> BreakDownGoalAsync(int goalId);
        List<GoalInviteDTO> GetPendingInvitations(int userId, string category);
        void RespondToInvitation(int invitationId, bool accept, int currentUserId);
        string GetGoalDescription(int goalId);
        List<GoalInviteDTO> GetInvitationsOf(int userId, string category);

        DateTime GetCreatedAt(int goalId);
        string GetInvitationStatus(int goalId, int invitedId);
        string? GetParentGoalDescription(int subGoalId);
        List<Goal> GetGoalsCreatedInRange(
                DateTime? startDate,
                DateTime? endDate,
                string? category,
                string? type,
                int page,
                int pageSize);
        List<string> GetGoalTypesForCurrentUser();
        int CountGoalsCreatedInRange(DateTime? startDate, DateTime? endDate, string? category, string? type);
        int GetProgressPercent(Goal goal);

    }
}
