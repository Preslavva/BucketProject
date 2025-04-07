using BucketProject.DAL.Models.Enums;
using BucketProject.DAL.Models.Entities;

using BucketProject.BLL.Business_Logic.Strategies;
using BucketProject.BLL.Business_Logic.InterfacesService;

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
                _deadline=DeadlineStrategyManager.GetStrategy(Category)?.GetDeadline(CreatedAt, IsPostponed);
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

    public bool IsDeleted { get; private set; }

    public bool IsPostponed { get; private set; }

    public int? ParentGoalId { get; private set; }

    public List<Goal> Children { get; private set; } = new();

    public List<User> Users { get; private set; }

    ////for reading from database
    //public Goal(int id, Category category, GoalType type, string description, DateTime createdAt, DateTime? deadline, DateTime? completedAt, bool isDone, bool isDeleted, bool isPostponed)
    //{
    //    this.Id = id;
    //    this.Category = category;
    //    this.Description = description;
    //    this.CreatedAt = createdAt;
    //    this.Deadline = deadline;
    //    this.IsDone = isDone;
    //    this.IsDeleted = isDeleted;
    //    this.Type = type;
    //    this.CompletedAt = completedAt;
    //    this.IsPostponed = isPostponed;
       
    //}

    ////saving to database
    //public Goal(Category category, GoalType type, string description)
    //{
    //    this.Category = category;
    //    this.Type = type;
    //    this.Description = description;
    //    this.CreatedAt = DateTime.Now;
    //    this.IsDone = false;
    //    this.IsDeleted = false;
    //    this.IsPostponed = false;
    //    Users = new List<User>();
    //}


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

    public void Delete()
    {
        IsDeleted = true;
    }

 
    public void SetDeadline(DateTime deadline)
    {
        Deadline = deadline;
    }

    public void AddUser(User user)
    {
        if (!Users.Contains(user))
        {
            Users.Add(user);
        }
    }

    public void UpdateDescription(string newDescription)
    {
        Description = newDescription;
    }
}
    

