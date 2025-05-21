using System.ComponentModel.DataAnnotations;
using BucketProject.BLL.Business_Logic.DTOs;


namespace BucketProject.UI.ViewModels.ViewModels
{
    public class GoalViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Type { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 50 characters.")]
        public string Description { get; set; }
        public bool IsDone { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<GoalViewModel> Children { get; set; } = new();
        public int? ParentGoalId { get; set; }

        public int OwnerId { get; set; }

        public List<UserSummaryDTO> Recipients { get; set; } = new();


    }
}
