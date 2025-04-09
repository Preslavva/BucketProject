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


        public void CreateGoal(GoalDomain goalDomain)
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            int userId = _goalRepo.GetIdOfUser(username);

            Goal goal = _mapper.Map<Goal>(goalDomain);

            _goalRepo.InsertGoalAndAssignToUser(userId, goal);
        }



        public List<GoalDomain> LoadGoalsByCategory(string category)
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                throw new Exception("User not logged in.");

            if (!Enum.TryParse<Category>(category, true, out var parsedCategory))
                throw new ArgumentException($"Invalid category: {category}");

            int userId = _goalRepo.GetIdOfUser(username);

            List<Goal> allEntities = _goalRepo.LoadGoalsOfUserbyCategory(userId, parsedCategory);

            List<GoalDomain> allGoals = _mapper.Map<List<GoalDomain>>(allEntities);

            var today = DateTime.Today;
            return allGoals
                .Where(g => !g.Deadline.HasValue || g.Deadline.Value.Date >= today)
                .ToList();
        }


      


        public void UpdateGoal(int goalId, GoalDomain goalDomain)
        {
            var entityGoal = _goalRepo.GetGoalById(goalId);
            if (entityGoal == null)
                throw new Exception("Goal not found");

            entityGoal.Description = goalDomain.Description;

            _goalRepo.UpdateGoalDescription(entityGoal);
        }



        public void DeleteGoal(int goalId)
        {
            Goal goal = _goalRepo.GetGoalById(goalId);
            if (goal == null)
                throw new Exception("Goal not found");

            _goalRepo.DeleteGoal(goal);
        }


        public void ChangeGoalStatus(int goalId, bool isDone)
        {
            Goal entityGoal = _goalRepo.GetGoalById(goalId);
            if (entityGoal == null)
                throw new Exception("Goal not found");

            GoalDomain goal = _mapper.Map<GoalDomain>(entityGoal);

            if (isDone)
                goal.MarkAsDone();
            else
                goal.UndoCompletion();

            entityGoal = _mapper.Map<Goal>(goal);

            _goalRepo.ChangeGoalStatus(entityGoal);
        }


        public void PostponeGoal(int goalId)
        {
            Goal entityGoal = _goalRepo.GetGoalById(goalId);
            if (entityGoal == null)
                throw new Exception("Goal not found");

            GoalDomain goal = _mapper.Map<GoalDomain>(entityGoal);

            var strategy = DeadlineStrategyDeterminator.GetStrategy(goal.Category);
            goal.Postpone(strategy);

            entityGoal = _mapper.Map<Goal>(goal);

            _goalRepo.PostponeGoal(entityGoal);
        }




        public Dictionary<string, Dictionary<string, Dictionary<string, List<GoalDomain>>>> LoadGroupedExpiredGoals()
        {
            var grouped = new Dictionary<string, Dictionary<string, Dictionary<string, List<GoalDomain>>>>();

            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return grouped;

            int userId = _goalRepo.GetIdOfUser(username);
            List<Goal> expiredEntities = _goalRepo.LoadExpiredGoalsOfUser(userId);
            List<GoalDomain> expiredGoals = _mapper.Map<List<GoalDomain>>(expiredEntities);

            var childGoalsLookup = expiredGoals
                .Where(g => g.ParentGoalId.HasValue)
                .GroupBy(g => g.ParentGoalId.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (GoalDomain goal in expiredGoals.Where(g => g.ParentGoalId == null))
            {
                if (!goal.Deadline.HasValue) continue;

                DateTime date = goal.Deadline.Value.Date;
                string category = goal.Category.ToString();
                string type = goal.Type.ToString();
                string key = GetGroupingKey(goal.Category, date);

 
                AddToGroup(grouped, category, key, type, goal);

                if (childGoalsLookup.ContainsKey(goal.Id))
                {
                    foreach (var child in childGoalsLookup[goal.Id])
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
                Category.Week => $"Week {date.AddDays((int)date.DayOfWeek-5):dd.MM.yyyy} - {date:dd.MM.yyyy}",
                Category.Month => $"{date:MMMM yyyy}",
                Category.Year => $"{date.Year}",
                _ => "Other"
            };
        }



        private void AddToGroup(
     Dictionary<string, Dictionary<string, Dictionary<string, List<GoalDomain>>>> group,
     string category,
     string key,
     string type,
     GoalDomain goal)
        {
            if (!group.ContainsKey(category))
                group[category] = new Dictionary<string, Dictionary<string, List<GoalDomain>>>();

            if (!group[category].ContainsKey(key))
                group[category][key] = new Dictionary<string, List<GoalDomain>>();

            if (!group[category][key].ContainsKey(type))
                group[category][key][type] = new List<GoalDomain>();

            group[category][key][type].Add(goal);
        }

        public async Task<List<GoalDomain>> BreakDownGoalAsync(int goalId)
        {
            Goal entity = _goalRepo.GetGoalById(goalId);
            if (entity == null || string.IsNullOrWhiteSpace(entity.Description))
                throw new ArgumentException("Invalid goal.");

            GoalDomain goal = _mapper.Map<GoalDomain>(entity);

            List<string> subGoalDescriptions = await _aIClient.BreakDownTextIntoGoalsAsync(goal.Description, goal.Category);

            List<GoalDomain> subGoals = subGoalDescriptions.Select(desc =>
            {
                GoalDomain subGoal = _mapper.Map<GoalDomain>(entity);

                subGoal.UpdateDescription(desc);
                return subGoal;
            }).ToList();

            return subGoals;
        }
    }
}

