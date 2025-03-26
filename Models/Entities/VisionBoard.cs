namespace BucketProject.DAL.Models.Entities;

public class VisionBoard
{
    public int Id { get; set; }
    public string Name { get; set; }

    public User Owner { get; set; }
    public List<User> Users { get; set; }

    public List<byte[]> Pictures { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public bool IsDeleted { get; set; }

    public VisionBoard(string name, User owner, DateTime createdAt, bool IsDeleted)
    {
        this.Name = name;
        this.Owner = owner;
        this.CreatedAt = createdAt;
        this.IsDeleted = false;
        Users = new List<User>();
        Pictures = new List<byte[]>();
    }

    public VisionBoard(int id,string name, User owner, DateTime createdAt, bool IsDeleted)
    {
        this.Id = id;
        this.Name = name;
        this.Owner = owner;
        this.CreatedAt = createdAt;

    }
}
