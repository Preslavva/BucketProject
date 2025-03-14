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
            set => _deadline = DetermineDeadLine(CreatedAt, Category);
        }

        public bool IsDone { get; set; }

        public bool IsDeleted { get; set; }

        public List<User> Users { get; set; }

        //for reading from database
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

        //saving to database
        public Goal(Category category, string description)
        {
            this.Category = category;
            this.Description = description;
            this.CreatedAt = DateTime.Now;
            this.Deadline = DetermineDeadLine(DateTime.Now, category);
            this.IsDone = false;
            this.IsDeleted = false;
            Users = new List<User>();
        }

        public Goal() { }

        public DateTime? DetermineDeadLine(DateTime createdAt, Category category)
        {
            if (category==Category.Week)
            {
                return createdAt.AddDays(7);
            }
            else if (category == Category.Month)
            {
                return createdAt.AddMonths(1);

            }
            else if (category == Category.Year)
            {
                return createdAt.AddYears(1);

            }
            else if (category == Category.Bucket_list)
            {
                return null;

            }
            return null;
        }
    }
}
