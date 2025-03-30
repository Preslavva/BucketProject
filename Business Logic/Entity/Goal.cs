using BucketProject.DAL.Models.Enums;
using BucketProject.DAL.Models.Entities;

using BucketProject.BLL.Business_Logic.Strategies;

namespace BucketProject.BLL.Business_Logic.Entity;

public class Goal
{
    public int Id { get; set; }
    public Category Category { get; set; }

    public GoalType Type { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? CompletedAt { get; set; }
    public string Description { get; set; }

    private DateTime? _deadline;
    public DateTime? Deadline
    {
        get
        {
            return _deadline ??= DeadlineStrategyManager.GetStrategy(Category)?.GetDeadline(CreatedAt,IsPostponed);
        }
        set
        {
            _deadline = value;
        }
    }



    private bool _isDone;
    public bool IsDone
    {
        get => _isDone;
        set
        {
            _isDone = value;

            if (_isDone)
            {
                CompletedAt = DateTime.Now;
            }
            else
            {
                CompletedAt = null;
            }
        }
    }

    public bool IsDeleted { get; set; }

    public bool IsPostponed { get; set; }
    
    public List<User> Users { get; set; }

    //for reading from database
    public Goal(int id, Category category, GoalType type, string description, DateTime createdAt, DateTime completedAt, bool isDone, bool isDeleted, bool isPostponed)
    {
        this.Id = id;
        this.Category = category;
        this.Description = description;
        this.CreatedAt = createdAt;
        this.Deadline = Deadline;
        this.IsDone = isDone;
        this.IsDeleted = isDeleted;
        this.Type = type;
        this.CompletedAt = completedAt;
        this.IsPostponed = isPostponed;
       
    }

    //saving to database
    public Goal(Category category, GoalType type, string description)
    {
        this.Category = category;
        this.Type = type;
        this.Description = description;
        this.CreatedAt = DateTime.Now;
        this.IsDone = false;
        this.IsDeleted = false;
        this.IsPostponed = false;
        Users = new List<User>();
    }

    public Goal() { }

    
}
