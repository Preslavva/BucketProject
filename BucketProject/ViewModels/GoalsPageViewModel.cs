

namespace BucketProject.UI.ViewModels.ViewModels
{
    public class GoalsPageViewModel
    {
        public List<GoalViewModel> Goals { get; set; }

        public int Total =>
            Goals.Sum(g => 1 + (g.Children?.Count ?? 0));

        public int AccomplishedCount =>
            Goals.Sum(g =>
                (g.IsDone ? 1 : 0) +
                (g.Children?.Count(c => c.IsDone) ?? 0)
            );

        public int Remaining => Total - AccomplishedCount;
    }
}
