using BucketProject.BLLBusiness_Logic.Domain;

namespace BucketProject.UI.ViewModels.ViewModels
{
    public class SocialViewModel
    {
        public List<UserSummaryDTO> IncomingRequests { get; set; } = new();
        public List<UserSummaryDTO> Friends { get; set; } = new();

        public List<UserSummaryDTO> PotentialFriends { get; set; } = new();
        public List<UserSummaryDTO> OutgoingRequests { get; set; } = new();

        public List<int> OutgoingRequestIds { get; set; } = new();

    }
}
