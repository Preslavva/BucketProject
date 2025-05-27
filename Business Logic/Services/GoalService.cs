using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.DAL.Models.Enums;
using Microsoft.AspNetCore.Http;
using BucketProject.BLL.Business_Logic.Strategies;
using AutoMapper;
using BucketProject.DAL.Models.Entities;
using BucketProject.BLL.Business_Logic.Domain;
using System.Text;
using System.ComponentModel.DataAnnotations;
using BucketProject.BLL.Business_Logic.DTOs;
using Exceptions.Exceptions;
using BucketProject.BLL.Business_Logic.InterfacesRepo;




namespace BucketProject.BLL.Business_Logic.Services
{
    public class GoalService: IGoalService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGoalRepo _goalRepo;
        private readonly IMapper _mapper;
        private readonly IAIClient _aIClient;
        private readonly IGoalInviteRepo _inviteRepo;
        private readonly IUserService _userService;


        public GoalService(IGoalRepo goalRepo, IHttpContextAccessor contextAccessor, IMapper mapper, IAIClient aIClient, IGoalInviteRepo inviteRepo, IUserService userService)
        {
            _goalRepo = goalRepo;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _aIClient = aIClient;
            _inviteRepo = inviteRepo;
            _userService = userService;
        }
       
        private void EnsureUserIsOwner(int goalId, int currentUserId)
        {
            GoalEntity goalEntity = _goalRepo.GetGoalById(goalId,_userService.GetCurrentUserId());

            if (goalEntity == null)
                throw new Exception("Goal not found.");

            if (goalEntity.OwnerId != currentUserId)
                throw new UnauthorizedAccessException("You are not the owner of this goal.");
        }

        public void CreateGoal(Goal goalDomain, IEnumerable<int>? sharedWithUserIds)
        {
            if (string.IsNullOrWhiteSpace(goalDomain.Description))
                throw new ValidationException("Goal description cannot be empty.");

            if (goalDomain.Description.Length < 5)
                throw new ValidationException("Goal description must have at least 5 characters.");

            if (goalDomain.Description.Length > 50)
                throw new ValidationException("Goal description is too long. Maximum allowed is 50 characters.");

            if (goalDomain.Type == null)
                throw new ValidationException("Goal type cannot be empty.");


            int ownerId = _userService.GetCurrentUserId();
            GoalEntity entity = _mapper.Map<GoalEntity>(goalDomain);

            bool shareIntent = sharedWithUserIds != null && sharedWithUserIds.Any();

            _goalRepo.InsertGoal(ownerId, entity);

            int goalId = entity.Id;

            if (!shareIntent)
            { 
                _goalRepo.AssignUsersToGoal(goalId, new[] { ownerId });
            }
            else
            {
                foreach (int friendId in sharedWithUserIds!)
                {
                    if (friendId == ownerId) continue;
                    _inviteRepo.InsertInvitation(goalId, ownerId, friendId);
                }
            }
        }
        public List<Goal> LoadPersonalGoalsByCategory(string category)
        {
            Enum.TryParse<Category>(category, true, out var parsedCategory);

            int userId = _userService.GetCurrentUserId();

            List<GoalEntity> allEntities = _goalRepo.LoadPersonalGoalsOfUserbyCategory(userId, parsedCategory);

            List<Goal> allGoals = _mapper.Map<List<Goal>>(allEntities);

            var today = DateTime.Today;
            return allGoals
                .Where(g => !g.Deadline.HasValue || g.Deadline.Value.Date >today)
                .ToList();
        }

        public List<Goal> LoadSharedGoalsByCategory(string category)
        {

            int userId = _userService.GetCurrentUserId();
            Enum.TryParse<Category>(category, true, out var parsedCategory);

            List<GoalEntity> entities = _goalRepo.LoadSharedGoalsOfUserByCategory(userId, parsedCategory);

            List<Goal> goals = _mapper.Map<List<Goal>>(entities);
 
            foreach (Goal g in goals)
            {
                List<UserEntity> recipients = _goalRepo.LoadSharedUsersForGoal(g.Id, userId);
                g.Recipients = _mapper.Map<List<User>>(recipients);
            }

            DateTime today = DateTime.Today;
            return goals
                .Where(g => !g.Deadline.HasValue || g.Deadline.Value.Date > today)
                .ToList();
        }



        public void UpdateGoal(int goalId, Goal goalDomain)
        {
            if (string.IsNullOrWhiteSpace(goalDomain.Description))
            {
                throw new ValidationException("New description cannot be empty");
            }
            if (goalDomain.Description.Length < 5)
                throw new ValidationException("Goal description must have at least 5 characters.");

            int userId = _userService.GetCurrentUserId();
            EnsureUserIsOwner(goalId, userId);

            GoalEntity entityGoal = _goalRepo.GetGoalById(goalId, _userService.GetCurrentUserId());
           

            entityGoal.Description = goalDomain.Description.Trim();

            _goalRepo.UpdateGoalDescription(entityGoal);
        }


        public void DeleteGoal(int goalId)
        {
            int userId = _userService.GetCurrentUserId();
            EnsureUserIsOwner(goalId, userId);

            GoalEntity goal = _goalRepo.GetGoalById(goalId, userId);

            _goalRepo.DeleteGoal(goal);
        }


        public void ChangeGoalStatus(int goalId, bool isDone)
        {
            GoalEntity entityGoal = _goalRepo.GetGoalById(goalId, _userService.GetCurrentUserId());

            Goal goal = _mapper.Map<Goal>(entityGoal);

            if (isDone)
                goal.MarkAsDone();
            else
                goal.UndoCompletion();

            entityGoal = _mapper.Map<GoalEntity>(goal);

            _goalRepo.ChangeGoalStatus(entityGoal, _userService.GetCurrentUserId());
        }


        public void PostponeGoal(int goalId)
        {

            int userId = _userService.GetCurrentUserId();
            EnsureUserIsOwner(goalId, userId);

            GoalEntity entityGoal = _goalRepo.GetGoalById(goalId,userId);

            Goal goal = _mapper.Map<Goal>(entityGoal);

            var strategy = DeadlineStrategyDeterminator.GetStrategy(goal.Category);
            goal.Postpone(strategy);

            entityGoal = _mapper.Map<GoalEntity>(goal);

            _goalRepo.PostponeGoal(entityGoal);
        }

        //the first string represents the time category
        //the second string represents the grouping key
        //the third string is the type of goal
        public Dictionary<string, Dictionary<string, Dictionary<string, List<Goal>>>> LoadGroupedExpiredGoals()
        {
            var grouped = new Dictionary<string, Dictionary<string, Dictionary<string, List<Goal>>>>();

            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return grouped;

            int userId = _userService.GetCurrentUserId();

            List<GoalEntity> expiredEntities = _goalRepo.LoadExpiredGoalsOfUser(userId);
            List<Goal> expiredGoals = _mapper.Map<List<Goal>>(expiredEntities);

            foreach (Goal goal in expiredGoals)
            {
                var sharedEntities = _goalRepo.LoadSharedUsersForGoal(
                    goal.Id,
                    userId
                );
                goal.Recipients = _mapper.Map<List<User>>(sharedEntities);
            }

            var childGoals = expiredGoals
                .Where(g => g.ParentGoalId.HasValue)
                .GroupBy(g => g.ParentGoalId.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var root in expiredGoals.Where(g => g.ParentGoalId == null))
            {
                if (!root.Deadline.HasValue)
                    continue;

                DateTime date = root.Deadline.Value.Date;
                string category = root.Category.ToString();
                string type = root.Type.ToString();
                string key = GetGroupingKey(root.Category, date);

                AddToGroup(grouped, category, key, type, root);

                if (childGoals.TryGetValue(root.Id, out var children))
                {
                    foreach (var child in children)
                        AddToGroup(grouped, category, key, type, child);
                }
            }

            return grouped;
        }

        private string GetGroupingKey(Category category, DateTime date)
        {
            return category switch
            {
                Category.Week => $"Week {date.AddDays((int)date.DayOfWeek-8):dd.MM.yyyy} - {date.AddDays((int)date.DayOfWeek - 2):dd.MM.yyyy}",
                Category.Month => $"{date:MMMM yyyy}",
                Category.Year => $"{date.Year}",
                _ => "Other"
            };
        }


        private void AddToGroup(
     Dictionary<string, Dictionary<string, Dictionary<string, List<Goal>>>> group,
     string category,
     string key,
     string type,
     Goal goal)
        {
            if (!group.ContainsKey(category))
                group[category] = new Dictionary<string, Dictionary<string, List<Goal>>>();

            if (!group[category].ContainsKey(key))
                group[category][key] = new Dictionary<string, List<Goal>>();

            if (!group[category][key].ContainsKey(type))
                group[category][key][type] = new List<Goal>();

            group[category][key][type].Add(goal);
        }

        public async Task<List<Goal>> BreakDownGoalAsync(int goalId)
        {
            GoalEntity entity = _goalRepo.GetGoalById(goalId, _userService.GetCurrentUserId());

            Goal goal = _mapper.Map<Goal>(entity);

            if (string.IsNullOrWhiteSpace(goal.Description) || goal.Description.Trim().Length < 5)
            {
                throw new VagueGoalDescriptionException();
            }

            List<string> subGoalDescriptions = await _aIClient.BreakDownTextIntoGoalsAsync(goal.Description, goal.Category);

            List<Goal> subGoals = subGoalDescriptions.Select(desc =>
            {
                Goal subGoal = _mapper.Map<Goal>(entity);

                subGoal.UpdateDescription(desc);
                return subGoal;
            }).ToList();

            return subGoals;
        }

        public List<GoalInviteDTO> GetPendingInvitations(int userId, string category)
        {
            List<GoalInvitation> entities = _inviteRepo.GetPendingFor(userId, category);
            List<GoalInviteDTO> domains = _mapper.Map<List<GoalInviteDTO>>(entities);
            return domains;
        }
        public List<GoalInviteDTO> GetInvitationsOf(int userId, string category)
        {

            List<GoalInvitation> entities = _inviteRepo.GetInvitationsOf(userId, category);
            List<GoalInviteDTO> domains = _mapper.Map<List<GoalInviteDTO>>(entities);
            return domains;
        }

        public void RespondToInvitation(int invitationId, bool accept, int currentUserId)
        {
            GoalInvitation inv = _inviteRepo.GetById(invitationId);

            _inviteRepo.UpdateStatus(invitationId, accept ? "Accepted" : "Declined");

            if (!accept) return;                
            var participants = new[] { inv.InviterId, currentUserId }.Distinct();
            _goalRepo.AssignUsersToGoal(inv.GoalId, participants);
        }


        public string GetGoalDescription(int goalId)
        {
            GoalEntity goal = _goalRepo.GetGoalById(goalId, _userService.GetCurrentUserId());
            return goal.Description;
        }

        public DateTime GetCreatedAt(int goalId)
        {
            GoalEntity goal = _goalRepo.GetGoalById(goalId, _userService.GetCurrentUserId());
            return goal.CreatedAt;
        }
        public string GetInvitationStatus(int goalId, int invitedId)
        {
            string status = _inviteRepo.GetInvitationStatus(goalId, invitedId);
            return status;
        }
        public string? GetParentGoalDescription(int subGoalId)
        {
            string? description = _inviteRepo.GetParentGoalDescription(subGoalId);
            return description;
        }
    }

}

