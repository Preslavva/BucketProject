using BucketProject.BLLBusiness_Logic.Domain;

namespace BucketProject.UI.ViewModels.ViewModels
{
    public class GoalInviteViewModel
    {
        public int InvitationId { get; set; }
        public string GoalDescription { get; set; }
        public string InviterUsername { get; set; }
        public string InvitedUsername { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string? ParentGoalDescription { get; set; }
        


    }
}
