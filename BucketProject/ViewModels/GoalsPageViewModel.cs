

namespace BucketProject.UI.ViewModels.ViewModels
{
    public class GoalsPageViewModel
    {
        public List<GoalViewModel> Goals { get; set; }
        public List<GoalViewModel> SharedGoals { get; set; }


        public int TotalPersonal =>
            Goals.Sum(g => 1 + (g.Children?.Count ?? 0));

        public int AccomplishedCountPersonal =>
            Goals.Sum(g =>
                (g.IsDone ? 1 : 0) +
                (g.Children?.Count(c => c.IsDone) ?? 0)
            );

        public int RemainingPersonal => TotalPersonal - AccomplishedCountPersonal;

        public int TotalShared =>
           SharedGoals.Sum(g => 1 + (g.Children?.Count ?? 0));

        public int AccomplishedCountShared =>
            SharedGoals.Sum(g =>
                (g.IsDone ? 1 : 0) +
                (g.Children?.Count(c => c.IsDone) ?? 0)
            );

        public int RemainingShared => TotalShared - AccomplishedCountShared;
        public List<GoalInviteViewModel> PendingInvitations { get; set; } = new();
    }
}
