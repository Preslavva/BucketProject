namespace BucketProject.Models
{
    public class Goal
    {
        public int Id { get; set; }
        public Category Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Description { get; set; }

        private DateTime? _deadline;
        public DateTime? Deadline
        {
            get => _deadline;
            private set => _deadline = DetermineDeadLine(CreatedAt, Category);
        }

        public bool IsDone { get; set; }

        public bool IsDeleted { get; set; }

        public List<User> Users { get; set; }

        public Goal(int id, Category category, string description, DateTime createdAt, DateTime deadline, bool isDone, bool isDeleted)
        {
            this.Id = id;
            this.Category = category;
            this.Description = description;
            this.CreatedAt = createdAt;
            this.Deadline = deadline;
            this.IsDone = isDone;
            this.IsDeleted = isDeleted;
        }

        public Goal(Category category, DateTime createdAt, string description )
        {
            this.Category = category;
            this.Description = description;
            this.IsDone = false;
            this.IsDeleted = false;
            Users = new List<User>();
        }

        public Goal() { }

        public DateTime? DetermineDeadLine(DateTime createdAt, Category category)
        {
            if (category==Category.week)
            {
                return createdAt.AddDays(7);
            }
            else if (category == Category.month)
            {
                return createdAt.AddMonths(1);

            }
            else if (category == Category.year)
            {
                return createdAt.AddYears(1);

            }
            else if (category == Category.bucket_list)
            {
                return null;

            }
            return null;
        }
    }
}
