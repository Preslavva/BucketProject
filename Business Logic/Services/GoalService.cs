using System.Security.Claims;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.DAL.Models.Enums;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using BucketProject.BLL.Business_Logic.Strategies;
using AutoMapper;
using BucketProject.UI.ViewModels.ViewModels;
using BucketProject.DAL.Models.Entities;
using BucketProject.BLL.Business_Logic.Domain;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.Extensions.Configuration;
using BucketProjetc.BLL.Business_Logic.InterfacesService;



namespace BucketProject.BLL.Business_Logic.Services
{
    public class GoalService : IGoalService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGoalRepo _goalRepo;
        private readonly IMapper _mapper;
        private readonly IAIClient _aIClient;

        public GoalService(IGoalRepo goalRepo, IHttpContextAccessor contextAccessor, IMapper mapper, IAIClient aIClient)
        {
            _goalRepo = goalRepo;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _aIClient = aIClient;
        }


        public void CreateGoal(Goal goalDomain)
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            int userId = _goalRepo.GetIdOfUser(username);

            GoalEntity goal = _mapper.Map<GoalEntity>(goalDomain);

            _goalRepo.InsertGoalAndAssignToUser(userId, goal);
        }



        public List<Goal> LoadGoalsByCategory(string category)
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                throw new Exception("User not logged in.");

            if (!Enum.TryParse<Category>(category, true, out var parsedCategory))
                throw new ArgumentException($"Invalid category: {category}");

            int userId = _goalRepo.GetIdOfUser(username);

            List<GoalEntity> allEntities = _goalRepo.LoadGoalsOfUserbyCategory(userId, parsedCategory);

            List<Goal> allGoals = _mapper.Map<List<Goal>>(allEntities);

            var today = DateTime.Today;
            return allGoals
                .Where(g => !g.Deadline.HasValue || g.Deadline.Value.Date >= today)
                .ToList();
        }


      


        public void UpdateGoal(int goalId, Goal goalDomain)
        {
            var entityGoal = _goalRepo.GetGoalById(goalId);
            if (entityGoal == null)
                throw new Exception("Goal not found");

            entityGoal.Description = goalDomain.Description;

            _goalRepo.UpdateGoalDescription(entityGoal);
        }



        public void DeleteGoal(int goalId)
        {
            GoalEntity goal = _goalRepo.GetGoalById(goalId);
            if (goal == null)
                throw new Exception("Goal not found");

            _goalRepo.DeleteGoal(goal);
        }


        public void ChangeGoalStatus(int goalId, bool isDone)
        {
            GoalEntity entityGoal = _goalRepo.GetGoalById(goalId);
            if (entityGoal == null)
                throw new Exception("Goal not found");

            Goal goal = _mapper.Map<Goal>(entityGoal);

            if (isDone)
                goal.MarkAsDone();
            else
                goal.UndoCompletion();

            entityGoal = _mapper.Map<GoalEntity>(goal);

            _goalRepo.ChangeGoalStatus(entityGoal);
        }


        public void PostponeGoal(int goalId)
        {
            GoalEntity entityGoal = _goalRepo.GetGoalById(goalId);
            if (entityGoal == null)
                throw new Exception("Goal not found");

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

            int userId = _goalRepo.GetIdOfUser(username);
            List<GoalEntity> expiredEntities = _goalRepo.LoadExpiredGoalsOfUser(userId);
            List<Goal> expiredGoals = _mapper.Map<List<Goal>>(expiredEntities);

            // thie dictionary holds int value for the ParentGoalId and List<GoalDomain> with the goals that share the same parent goal
            var childGoals = expiredGoals
                .Where(g => g.ParentGoalId.HasValue)
                .GroupBy(g => g.ParentGoalId.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (Goal goal in expiredGoals.Where(g => g.ParentGoalId == null))
            {
                if (!goal.Deadline.HasValue) continue;

                DateTime date = goal.Deadline.Value.Date;
                string category = goal.Category.ToString();
                string type = goal.Type.ToString();
                string key = GetGroupingKey(goal.Category, date);

 
                AddToGroup(grouped, category, key, type, goal);

                if (childGoals.ContainsKey(goal.Id))
                {
                    foreach (var child in childGoals[goal.Id])
                    {
                        AddToGroup(grouped, category, key, type, child);
                    }
                }
            }

            return grouped;
        }
        private string GetGroupingKey(Category category, DateTime date)
        {
            return category switch
            {
                Category.Week => $"Week {date.AddDays((int)date.DayOfWeek-6):dd.MM.yyyy} - {date:dd.MM.yyyy}",
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
            GoalEntity entity = _goalRepo.GetGoalById(goalId);
            if (entity == null || string.IsNullOrWhiteSpace(entity.Description))
                throw new ArgumentException("Invalid goal.");

            Goal goal = _mapper.Map<Goal>(entity);

            List<string> subGoalDescriptions = await _aIClient.BreakDownTextIntoGoalsAsync(goal.Description, goal.Category);

            List<Goal> subGoals = subGoalDescriptions.Select(desc =>
            {
                Goal subGoal = _mapper.Map<Goal>(entity);

                subGoal.UpdateDescription(desc);
                return subGoal;
            }).ToList();

            return subGoals;
        }
    }
}

