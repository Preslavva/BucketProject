using System.Security.Claims;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.BLL.Business_Logic.Entity;
using BucketProject.DAL.Models.Enums;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using BucketProject.BLL.Business_Logic.Strategies;
using AutoMapper;
using BucketProject.UI.ViewModels.ViewModels;


namespace BucketProject.BLL.Business_Logic.Services
{
    public class GoalService : IGoalService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGoalRepo _goalRepo;
        private readonly IMapper _mapper;

        public GoalService(IGoalRepo goalRepo, IHttpContextAccessor contextAccessor, IMapper mapper)
        {
            _goalRepo = goalRepo;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }

        public void CreateGoal(GoalViewModel viewModel)
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            int userId = _goalRepo.GetIdOfUser(username);

            Goal newGoal = _mapper.Map<Goal>(viewModel);

            _goalRepo.InsertGoalAndAssignToUser(userId, newGoal);
        }



        public List<GoalViewModel> LoadGoalsByCategory(string category)
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
                throw new Exception("User not logged in.");

            if (!Enum.TryParse<Category>(category, true, out var parsedCategory))
                throw new ArgumentException($"Invalid category: {category}");

            int userId = _goalRepo.GetIdOfUser(username);

            var allGoals = _goalRepo.LoadGoalsOfUserbyCategory(userId, parsedCategory);

            var today = DateTime.Today;

            var nonExpiredGoals = allGoals
                .Where(g => !g.Deadline.HasValue || g.Deadline.Value.Date >= today)
                .ToList();

            var viewModelList = _mapper.Map<List<GoalViewModel>>(nonExpiredGoals);

            return viewModelList;
        }




        public void UpdateGoal(int goalId, GoalViewModel viewModel)
        {
            Goal goal = _goalRepo.GetGoalById(goalId);
            if (goal == null)
                throw new Exception("Goal not found");

            goal.UpdateDescription(viewModel.Description);

            _goalRepo.UpdateGoalDescription(goal, viewModel.Description);
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
            var goal = _goalRepo.GetGoalById(goalId);
            if (goal == null)
                throw new Exception("Goal not found");

            if (isDone)
                goal.MarkAsDone();
            else
                goal.UndoCompletion();

            _goalRepo.ChangeGoalStatus(goal, isDone);
        }


        public void PostponeGoal(int goalId)
        {
            var goal = _goalRepo.GetGoalById(goalId);
            if (goal == null)
                throw new Exception("Goal not found");

            var strategy = DeadlineStrategyManager.GetStrategy(goal.Category);
            goal.Postpone(strategy);

            _goalRepo.PostponeGoal(goal);
        }



        public Dictionary<string, Dictionary<string, Dictionary<string, List<HistoryViewModel>>>> LoadGroupedExpiredGoals()
        {
            var grouped = new Dictionary<string, Dictionary<string, Dictionary<string, List<HistoryViewModel>>>>();

            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return grouped;

            int userId = _goalRepo.GetIdOfUser(username);
            var expiredGoals = _goalRepo.LoadExpiredGoalsOfUser(userId);

            foreach (var goal in expiredGoals)
            {
                DateTime date = goal.Deadline!.Value.Date;
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

                var mappedGoal = _mapper.Map<HistoryViewModel>(goal);
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


    }
}

