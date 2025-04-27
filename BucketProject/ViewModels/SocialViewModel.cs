using BucketProject.BLLBusiness_Logic.Domain;

namespace BucketProject.UI.ViewModels.ViewModels
{
    public class SocialViewModel
    {
        public List<UserSummaryDTO> Friends { get; set; }
        public List<UserSummaryDTO> NonFriends { get; set; }


    }
}
