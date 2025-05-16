using BucketProject.DAL.Models.Enums;
using BucketProject.DAL.Models.Entities;
using BucketProject.BLL.Business_Logic.Strategies;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLLBusiness_Logic.Domain;

namespace BucketProject.BLL.Business_Logic.Domain;

public class Goal
{
    public int Id { get; private set; }
    public Category Category { get; private set; }

    public GoalType Type { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.Now;

    public DateTime? CompletedAt { get; private set; }
    public string Description { get; private set; }

    private DateTime? _deadline;
    public DateTime? Deadline
    {
        get
        {
            if (_deadline==null)
            {
                _deadline=DeadlineStrategyDeterminator.GetStrategy(Category)?.GetDeadline(CreatedAt, IsPostponed);
            }
            return _deadline;
        }
        private set
        {
            _deadline = value;
        }
    }



    private bool _isDone;
    public bool IsDone
    {
        get => _isDone;
        private set
        {
            
            if (_isDone != value)
            {
                _isDone = value;

                if (_isDone)
                {
                    
                    if (CompletedAt == null)
                    {
                        CompletedAt = DateTime.Now;
                    }
                }
                else
                {
                    CompletedAt = null;
                }
            }
        }
    }


    public bool IsDeleted { get; private set; }

    public bool IsPostponed { get; private set; }

    public int? ParentGoalId { get; private set; }

    public int OwnerId { get; private set; }

    public List<Goal> Children { get; private set; } = new();

    public List<UserEntity> Users { get; private set; }
    public List<User> Recipients { get; set; } = new();

    public Goal() { }
    
public Goal(int id, Category category, GoalType type, DateTime createdAt, DateTime? completedAt, string description, DateTime? deadline, bool isDone, bool isDeleted, bool isPostponed, int? parentGoalId, int ownerId)
    {
        Id = id;
        Category = category;
        Type = type;
        CreatedAt = createdAt;
        CompletedAt = completedAt;
        Description = description;
        Deadline = deadline;
        Deadline = deadline;
        IsDone = isDone;
        IsDone = isDone;
        IsDeleted = isDeleted;
        IsPostponed = isPostponed;
        ParentGoalId = parentGoalId;
        OwnerId = ownerId;
    }

    public void MarkAsDone()
    {
        IsDone = true;
    }

    public void UndoCompletion()
    {
        IsDone = false;
    }

    public void Postpone(IDeadlineStrategy? strategy)
    {
        if (strategy != null)
        {
            Deadline = strategy.GetDeadline(CreatedAt, true);
        }

        IsPostponed = true;
    }

    public void UpdateDescription(string newDescription)
    {
        Description = newDescription;
    }
}
    

