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
        private readonly IConfiguration _configuration;
        private readonly IAIClient _aIClient;

        public GoalService(IGoalRepo goalRepo, IHttpContextAccessor contextAccessor, IMapper mapper, IConfiguration configuration, IAIClient aIClient)
        {
            _goalRepo = goalRepo;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _configuration = configuration;
            _aIClient = aIClient;
        }

        public void CreateGoal(GoalViewModel viewModel)
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            int userId = _goalRepo.GetIdOfUser(username);

            Goal newGoal = _mapper.Map<Goal>(viewModel);
            GoalEntity goalEntity = _mapper.Map<GoalEntity>(newGoal);

            _goalRepo.InsertGoalAndAssignToUser(userId, goalEntity);
        }



        public List<GoalViewModel> LoadGoalsByCategory(string category)
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
            List<Goal> nonExpiredGoals = allGoals
                .Where(g => !g.Deadline.HasValue || g.Deadline.Value.Date >= today)
                .ToList();

            return _mapper.Map<List<GoalViewModel>>(nonExpiredGoals);
        }


        public List<GoalViewModel> LoadChildGoalsOfGoal(int goalId)
        {
            List<GoalEntity> allEntities = _goalRepo.LoadChildGoalsOfGoals(goalId);

            List<Goal> allGoals = _mapper.Map<List<Goal>>(allEntities);

            return _mapper.Map<List<GoalViewModel>>(allGoals);
        }


        public void UpdateGoal(int goalId, GoalViewModel viewModel)
        {
            GoalEntity entityGoal = _goalRepo.GetGoalById(goalId);
            if (entityGoal == null)
                throw new Exception("Goal not found");

            Goal goal = _mapper.Map<Goal>(entityGoal);

            goal.UpdateDescription(viewModel.Description);
            entityGoal = _mapper.Map<GoalEntity>(goal);

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

            var strategy = DeadlineStrategyManager.GetStrategy(goal.Category);
            goal.Postpone(strategy);

            entityGoal = _mapper.Map<GoalEntity>(goal);

            _goalRepo.PostponeGoal(entityGoal);
        }




        public Dictionary<string, Dictionary<string, Dictionary<string, List<HistoryViewModel>>>> LoadGroupedExpiredGoals()
        {
            var grouped = new Dictionary<string, Dictionary<string, Dictionary<string, List<HistoryViewModel>>>>();

            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return grouped;

            int userId = _goalRepo.GetIdOfUser(username);

            List<GoalEntity> expiredEntities = _goalRepo.LoadExpiredGoalsOfUser(userId);

            List<Goal> expiredGoals = _mapper.Map<List<Goal>>(expiredEntities);

            foreach (Goal goal in expiredGoals)
            {
                if (!goal.Deadline.HasValue) continue;

                DateTime date = goal.Deadline.Value.Date;
                string category = goal.Category.ToString();
                string type = goal.Type.ToString();
                string key;

                switch (goal.Category)
                {
                    case Category.Week:
                        DateTime startOfWeek = date.AddDays(-(int)date.DayOfWeek);
                        DateTime endOfWeek = startOfWeek.AddDays(6);
                        key = $"Week {startOfWeek:dd.MM.yyyy} - {endOfWeek:dd.MM.yyyy}";
                        break;

                    case Category.Month:
                        key = $"{date:MMMM yyyy}";
                        break;

                    case Category.Year:
                        key = $"{date.Year}";
                        break;

                    default:
                        continue;
                }

                HistoryViewModel mappedGoal = _mapper.Map<HistoryViewModel>(goal);
                AddToGroup(grouped, category, key, type, mappedGoal);
            }

            return grouped;
        }


        private void AddToGroup(
     Dictionary<string, Dictionary<string, Dictionary<string, List<HistoryViewModel>>>> group,
     string category,
     string key,
     string type,
     HistoryViewModel goal)
        {
            if (!group.ContainsKey(category))
                group[category] = new Dictionary<string, Dictionary<string, List<HistoryViewModel>>>();

            if (!group[category].ContainsKey(key))
                group[category][key] = new Dictionary<string, List<HistoryViewModel>>();

            if (!group[category][key].ContainsKey(type))
                group[category][key][type] = new List<HistoryViewModel>();

            group[category][key][type].Add(goal);
        }

        public async Task<List<GoalViewModel>> BreakDownGoalAsync(int goalId)
        {
            GoalEntity entity = _goalRepo.GetGoalById(goalId);
            if (entity == null || string.IsNullOrWhiteSpace(entity.Description))
                throw new ArgumentException("Invalid goal.");

            Goal goal = _mapper.Map<Goal>(entity);

            List<string> subGoalDescriptions = await _aIClient.BreakDownTextIntoGoalsAsync(goal.Description, goal.Category);

            List<GoalViewModel> subGoals = subGoalDescriptions.Select(desc =>
            {
                var vm = _mapper.Map<GoalViewModel>(goal);
                vm.Description = desc;
                return vm;
            }).ToList();

            return subGoals;
        }
    }
}

