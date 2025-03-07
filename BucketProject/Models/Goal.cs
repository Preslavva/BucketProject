namespace BucketProject.Models
{
    public class Goal
    {
        public int Id { get; set; }
        public Category Category { get; set; }

        public string Description { get; set; }

        public DateTime Deadline{ get; set; }

        public bool IsDone { get; set; }

        public bool IsDeleted { get; set; }

        public List<User> Users { get; set; }

        public Goal(int id, Category category, string description, DateTime deadline, bool isDone, bool isDeleted)
        {
            this.Id = id;
            this.Category = category;
            this.Description = description;
            this.Deadline = deadline;
            this.IsDone = isDone;
            this.IsDeleted = isDeleted;
        }

        public Goal(Category category, string description, DateTime deadline)
        {
            this.Category = category;
            this.Description = description;
            this.Deadline = deadline;
            this.IsDone = false;
            this.IsDeleted = false;
        }

        public Goal() { }
    }
}
