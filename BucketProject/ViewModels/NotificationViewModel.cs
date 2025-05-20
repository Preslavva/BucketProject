

using BucketProject.BLL.Business_Logic.DTOs;

namespace BucketProject.UI.ViewModels.ViewModels
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? Deadline { get; set; }
        public bool IsDone { get; set; }
        public bool IsDeleted { get; set; }
        public string Message { get; set; }
        public int OwnerId { get; set; }
        public List<UserSummaryDTO>? Recipients { get; set; }
        public string TypeOfNotification { get; set; } = string.Empty;
        public int TriggeredByUserId { get; set; }

    }
}
