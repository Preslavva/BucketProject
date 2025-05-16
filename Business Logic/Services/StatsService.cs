using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.DAL.Models.Entities;
using BucketProject.BLLBusiness_Logic.Domain;
using BucketProject.BLL.Business_Logic.Domain;
using AutoMapper;
using BucketProject.BLL.Business_Logic.InterfacesService;

namespace BucketProject.BLL.Business_Logic.Services
{
    public class StatsService: IStatsService
    {
        private readonly IGoalRepo _goalRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;

        public StatsService(IGoalRepo goalRepo, IHttpContextAccessor contextAccessor, IMapper mapper)
        {
            _goalRepo = goalRepo;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }
        public int GetCurrentUserId()
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                throw new Exception("User not logged in.");


            int userId = _goalRepo.GetIdOfUser(username);
            return userId;
        }

        public List<StatsDTO> GetGoalTypeStatistics()
        {
            int id = GetCurrentUserId();
            List<GoalEntity> entities = _goalRepo.LoadGoalsOfUser(id);
            List<Goal> goals = _mapper.Map<List<Goal>>(entities);

            return goals
                .GroupBy(g => g.Type)
                .Select(g => new StatsDTO
                {
                    Type = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToList();
        }

        public List<StatsDTO> GetGoalCategoryStatistics()
        {
            int id = GetCurrentUserId();
            List<GoalEntity> entities = _goalRepo.LoadGoalsOfUser(id);
            List<Goal> goals = _mapper.Map<List<Goal>>(entities);

            return goals
                .GroupBy(g => g.Category)
                .Select(g => new StatsDTO
                {
                    Category = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToList();
        }

        public List<StatsDTO> GetGoalAmountStatisticsWeekly()
        {
            int id = GetCurrentUserId();
            
            List<Goal> goalsPersonal = _mapper.Map<List<Goal>>(_goalRepo.LoadPersonalGoalsOfUser(id));
            List<Goal> goalsShared = _mapper.Map<List<Goal>>(_goalRepo.LoadSharedGoalsOfUser(id));

            var groupedPersonal = goalsPersonal
                .GroupBy(g => GetWeekLabel(g.CreatedAt))
                .Select(g => new StatsDTO
                {
                    Period = g.Key,
                    Ownership = "Personal",
                    Completed = g.Count(x => x.IsDone),
                    Incomplete = g.Count(x => !x.IsDone)
                });

            var groupedShared = goalsShared
                .GroupBy(g => GetWeekLabel(g.CreatedAt))
                .Select(g => new StatsDTO
                {
                    Period = g.Key,
                    Ownership = "Shared",
                    Completed = g.Count(x => x.IsDone),
                    Incomplete = g.Count(x => !x.IsDone)
                });

            return groupedPersonal
                .Concat(groupedShared)
                .OrderBy(s => s.Period)
                .ToList();
        }



        private string GetWeekLabel(DateTime date)
        {
            var startOfWeek = date.AddDays(-(int)date.DayOfWeek).Date;
            var endOfWeek = startOfWeek.AddDays(6);

            return $"{startOfWeek:yyyy-MM-dd} - {endOfWeek:yyyy-MM-dd}";
        }

        public List<StatsDTO> GetGoalAmountStatisticsMonthly()
        {
            int id = GetCurrentUserId();

            List<Goal> goalsPersonal = _mapper.Map<List<Goal>>(_goalRepo.LoadPersonalGoalsOfUser(id));
            List<Goal> goalsShared = _mapper.Map<List<Goal>>(_goalRepo.LoadSharedGoalsOfUser(id));

            var groupedPersonal = goalsPersonal
                .GroupBy(g => GetMonthLabel(g.CreatedAt))
                .Select(g => new StatsDTO
                {
                    Period = g.Key, 
                    Ownership = "Personal",
                    Completed = g.Count(x => x.IsDone),
                    Incomplete = g.Count(x => !x.IsDone)
                });

            var groupedShared = goalsShared
                .GroupBy(g => GetMonthLabel(g.CreatedAt))
                .Select(g => new StatsDTO
                {
                    Period = g.Key, 
                    Ownership = "Shared",
                    Completed = g.Count(x => x.IsDone),
                    Incomplete = g.Count(x => !x.IsDone)
                });

            return groupedPersonal
                .Concat(groupedShared)
                .OrderBy(s => s.Period)
                .ToList();
        }
        private string GetMonthLabel(DateTime date)
        {
            return date.ToString("MMMM yyyy");
        }

        public List<StatsDTO> GetGoalAmountStatisticsYearly()
        {
            int id = GetCurrentUserId();

            List<Goal> goalsPersonal = _mapper.Map<List<Goal>>(_goalRepo.LoadPersonalGoalsOfUser(id));
            List<Goal> goalsShared = _mapper.Map<List<Goal>>(_goalRepo.LoadSharedGoalsOfUser(id));

            var groupedPersonal = goalsPersonal
                .GroupBy(g => g.CreatedAt.Year.ToString())
                .Select(g => new StatsDTO
                {
                    Period = g.Key,
                    Ownership = "Personal",
                    Completed = g.Count(x => x.IsDone),
                    Incomplete = g.Count(x => !x.IsDone)
                });

            var groupedShared = goalsShared
                .GroupBy(g => g.CreatedAt.Year.ToString())
                .Select(g => new StatsDTO
                {
                    Period = g.Key,
                    Ownership = "Shared",
                    Completed = g.Count(x => x.IsDone),
                    Incomplete = g.Count(x => !x.IsDone)
                });

            return groupedPersonal
                .Concat(groupedShared)
                .OrderBy(s => s.Period)
                .ToList();
        }

        public double GetAverageCompletionTimeInDays()
        {
            int id = GetCurrentUserId();

            List<Goal> allGoals = _mapper.Map<List<Goal>>(
                _goalRepo.LoadPersonalGoalsOfUser(id)
                    .Concat(_goalRepo.LoadSharedGoalsOfUser(id))
            );

            var completedDurations = allGoals
                .Where(g => g.IsDone && g.CompletedAt.HasValue)
                .Select(g => (g.CompletedAt.Value - g.CreatedAt).TotalDays);

            return completedDurations.Any() ? completedDurations.Average() : 0;
        }


        public StatsDTO GetGoalSummaryStats()
        {
            int userId = GetCurrentUserId();

            var personalGoals = _mapper.Map<List<Goal>>(_goalRepo.LoadPersonalGoalsOfUser(userId));
            var sharedGoals = _mapper.Map<List<Goal>>(_goalRepo.LoadSharedGoalsOfUser(userId));

            var allGoals = personalGoals.Concat(sharedGoals).ToList();

            return new StatsDTO
            {
                TotalGoals = allGoals.Count,
                CompletedGoals = allGoals.Count(g => g.IsDone),
                IncompleteGoals = allGoals.Count(g => !g.IsDone)
            };
        }

    }

}
