namespace BucketProject.Models
{
    public class VisionBoard
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<User> Users { get; set; }

        public List<string> Pictures { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
