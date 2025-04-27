using System.ComponentModel.DataAnnotations;
using BucketProject.BLLBusiness_Logic.Domain;


namespace BucketProject.UI.ViewModels.ViewModels
{
    public class GoalViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Description { get; set; }
        public bool IsDone { get; set; }
        public List<GoalViewModel> Children { get; set; } = new();
        public int? ParentGoalId { get; set; }

        public List<UserSummaryDTO> Recipients { get; set; } = new();


    }
}
