using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.DAL.Models.Enums;

namespace BucketProject.DAL.Models.Entities
{
    public class GoalEntity
    {
        public int Id { get; set; }
        public Category Category { get; set; }

        public GoalType Type { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public bool IsDone { get; set; }
        public bool IsDeleted { get;  set; }

        public bool IsPostponed { get; set; }

        public List<User> Users { get; set; } = new List<User>();

        public GoalEntity(int id, Category category, GoalType type, string description, DateTime createdAt, DateTime? deadline, DateTime? completedAt, bool isDone, bool isDeleted, bool isPostponed)
        {
            this.Id = id;
            this.Category = category;
            this.Description = description;
            this.CreatedAt = createdAt;
            this.Deadline = deadline;
            this.IsDone = isDone;
            this.IsDeleted = isDeleted;
            this.Type = type;
            this.CompletedAt = completedAt;
            this.IsPostponed = isPostponed;

        }

    }
}
