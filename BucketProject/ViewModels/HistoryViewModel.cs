
using BucketProject.BLL.Business_Logic.DTOs;

namespace BucketProject.UI.ViewModels.ViewModels
{
    public class HistoryViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsDone { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public List<UserSummaryDTO> Recipients { get; set; } = new();
    }
}
