using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using BucketProject.BLL.Business_Logic.InterfacesRepo;
using BucketProject.DAL.Models.Entities;
using BucketProject.BLL.Business_Logic.Domain;
using AutoMapper;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.DAL.Data.Repositories;
using System.Globalization;
using BucketProject.BLL.Business_Logic.DTOs;
using Exceptions.Exceptions;

namespace BucketProject.BLL.Business_Logic.Services
{
    public class StatsService : IStatsService
    {
        private readonly IGoalRepo _goalRepo;
        private readonly IManagerRepo _managerRepo;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;

        public StatsService(IGoalRepo goalRepo, IHttpContextAccessor contextAccessor, IMapper mapper, IManagerRepo managerRepo, IUserService userService)
        {
            _goalRepo = goalRepo;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _managerRepo = managerRepo;
            _userService = userService;
        }


        public List<StatsDTO> GetGoalTypeStatistics()
        {
            int id = _userService.GetCurrentUserId();
            List<GoalEntity> entities = _goalRepo.LoadGoalsOfUser(id);
            List<Goal> goals = _mapper.Map<List<Goal>>(entities);

            return goals
                .GroupBy(g => g.Type)
                .Select(g => new StatsDTO
                {
                    Type = g.Key.ToString(),
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count) 
                .Take(5)                         
                .ToList();
        }

        public List<StatsDTO> GetGoalCategoryStatistics()
        {
            int id = _userService.GetCurrentUserId();
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

        public List<StatsDTO> GetCompletedGoalsPerWeek()
        {
            int userId = _userService.GetCurrentUserId();

            List<Goal> personalGoals = _mapper.Map<List<Goal>>(_goalRepo.LoadPersonalGoalsOfUser(userId));
            List<Goal> sharedGoals = _mapper.Map<List<Goal>>(_goalRepo.LoadSharedGoalsOfUser(userId));
            var allGoals = personalGoals.Concat(sharedGoals);

            var completedPerWeek = allGoals
                .Where(g => g.IsDone && g.CompletedAt.HasValue)
                .GroupBy(g => GetWeekLabel(g.CompletedAt.Value))
                .Select(grp => new StatsDTO
                {
                    Period = grp.Key,
                    Count = grp.Count()
                })
                .OrderBy(dto => dto.Period)
                .ToList();

            return completedPerWeek;
        }

        private string GetWeekLabel(DateTime date)
        {
            var startOfWeek = date.AddDays(-(int)date.DayOfWeek).Date;
            var endOfWeek = startOfWeek.AddDays(6);

            return $"{startOfWeek:yyyy-MM-dd} - {endOfWeek:yyyy-MM-dd}";
        }


        public List<StatsDTO> GetCompletedGoalsPerMonth()
        {
            int userId = _userService.GetCurrentUserId();
            List<Goal> personalGoals = _mapper.Map<List<Goal>>(_goalRepo.LoadPersonalGoalsOfUser(userId));
            List<Goal> sharedGoals = _mapper.Map<List<Goal>>(_goalRepo.LoadSharedGoalsOfUser(userId));
            var allGoals = personalGoals.Concat(sharedGoals);

            var completedPerMonth = allGoals
                .Where(g => g.IsDone && g.CompletedAt.HasValue)
                .GroupBy(g => new DateTime(
                    g.CompletedAt.Value.Year,
                    g.CompletedAt.Value.Month,
                    1
                ))
                .Select(grp => new
                {
                    MonthKey = grp.Key,    
                    Count = grp.Count()
                })
                .OrderBy(x => x.MonthKey)
                .Select(x => new StatsDTO
                {
                    Period = x.MonthKey.ToString("yyyy MMMM"),
                    Count = x.Count
                })
                .ToList();

            return completedPerMonth;
        }


        public List<StatsDTO> GetCompletedGoalsPerYear()
        {
            int userId = _userService.GetCurrentUserId();

            List<Goal> personalGoals = _mapper.Map<List<Goal>>(_goalRepo.LoadPersonalGoalsOfUser(userId));
            List<Goal> sharedGoals = _mapper.Map<List<Goal>>(_goalRepo.LoadSharedGoalsOfUser(userId));
            var allGoals = personalGoals.Concat(sharedGoals);

            var completedPerYear = allGoals
                .Where(g => g.IsDone && g.CompletedAt.HasValue)
                .GroupBy(g => g.CompletedAt.Value.Year.ToString())
                .Select(grp => new StatsDTO
                {
                    Period = grp.Key,   
                    Count = grp.Count()
                })
                .OrderBy(dto => dto.Period)
                .ToList();

            return completedPerYear;
        }


        public double GetAverageCompletionTimeInDays()
        {
            int id = _userService.GetCurrentUserId();

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
            int userId = _userService.GetCurrentUserId();

            List<Goal> personalGoals = _mapper.Map<List<Goal>>(_goalRepo.LoadPersonalGoalsOfUser(userId));
            List<Goal> sharedGoals = _mapper.Map<List<Goal>>(_goalRepo.LoadSharedGoalsOfUser(userId));

            var allGoals = personalGoals.Concat(sharedGoals).ToList();

            List<Goal> aiGoals = allGoals.Where(g => g.ParentGoalId != null).ToList();
            List<Goal> postponedGoals = allGoals.Where(g => g.IsPostponed == true).ToList();

            return new StatsDTO
            {
                TotalGoals = allGoals.Count,
                CompletedGoals = allGoals.Count(g => g.IsDone),
                IncompleteGoals = allGoals.Count(g => !g.IsDone),
                PersonalGoals = personalGoals.Count,
                SharedGoals = sharedGoals.Count,
                PostponedGoals = postponedGoals.Count,
                AIGoals = aiGoals.Count
            };
        }

        public List<StatsDTO> GetUserRegistrationsPerMonth()
        {
            List<UserEntity> entities = _managerRepo.GetAllUsers();
            List<User> users = _mapper.Map<List<User>>(entities);

            return users
                .GroupBy(u => u.CreatedAt.ToString("yyyy MMM", CultureInfo.InvariantCulture))
                .Select(g => new StatsDTO
                {
                    Period = g.Key,
                    Count = g.Count()
                })
                .OrderBy(s => DateTime.ParseExact(s.Period, "yyyy MMM", CultureInfo.InvariantCulture))
                .ToList();
        }

        public List<StatsDTO> GetGoalsPerMonth()
        {
            List<GoalEntity> entities = _managerRepo.GetAllGoals();
            List<Goal> goals = _mapper.Map<List<Goal>>(entities);


            return goals
                .GroupBy(u => u.CreatedAt.ToString("yyyy MMM", CultureInfo.InvariantCulture))
                .Select(g => new StatsDTO
                {
                    Period = g.Key,
                    Count = g.Count()
                })
                .OrderBy(s => DateTime.ParseExact(s.Period, "yyyy MMM", CultureInfo.InvariantCulture))
                .ToList();
        }

        public StatsDTO GetGoalSummaryStatsManager()
        {
            List<GoalEntity> entitiesGoals = _managerRepo.GetAllGoals();
            List<Goal> allGoals = _mapper.Map<List<Goal>>(entitiesGoals);

            List<GoalEntity> entitiesPersonal = _managerRepo.LoadAllPersonalGoals();
            List<Goal> allPersonal = _mapper.Map<List<Goal>>(entitiesPersonal);

            List<GoalEntity> entitiesShared = _managerRepo.LoadAllSharedGoals();
            List<Goal> allShared = _mapper.Map<List<Goal>>(entitiesShared);

            List<Goal> aiGoals = allGoals.Where(g => g.ParentGoalId != null).ToList();
            List<Goal> postponedGoals = allGoals.Where(g => g.IsPostponed == true).ToList();

            int countActive = _managerRepo.GetActiveUsersCount();

            return new StatsDTO
            {
                TotalGoals = allGoals.Count,
                PersonalGoals = allPersonal.Count,
                SharedGoals = allShared.Count,
                AIGoals = aiGoals.Count,
                PostponedGoals = postponedGoals.Count,
                ActiveUsersCount = countActive
            };

        }

        public List<StatsDTO> GetGoalTypeStatisticsManager()
        {
            List<GoalEntity> entities = _managerRepo.GetAllGoals();
            List<Goal> goals = _mapper.Map<List<Goal>>(entities);

            return goals
                .GroupBy(g => g.Type)
                .Select(g => new StatsDTO
                {
                    Type = g.Key.ToString(),
                    Count = g.Count()
                })
                .OrderByDescending(stat => stat.Count)
                .Take(5)
                .ToList();
        }


        public List<StatsDTO> GetGoalCategoryStatisticsManager()
        {
            List<GoalEntity> entities = _managerRepo.GetAllGoals();
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

        public List<StatsDTO> GetUsersNationalityStatistics()
        {
            List<UserEntity> entities = _managerRepo.GetAllUsers();
            List<User> users = _mapper.Map<List<User>>(entities);

            return users
                 .GroupBy(u => (u.Nationality ?? string.Empty).Trim(),
                 StringComparer.OrdinalIgnoreCase)
                .Select(g => new StatsDTO
                {
                    Nationality = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(stat => stat.Count)
                .Take(5)
                .ToList();
        }

        public List<StatsDTO> GetUsersGenderStatistics()
        {
            List<UserEntity> entities = _managerRepo.GetAllUsers();
            List<User> users = _mapper.Map<List<User>>(entities);

            return users
                .GroupBy(g => g.Gender)
                .Select(g => new StatsDTO
                {
                    Gender = g.Key,
                    Count = g.Count()
                })
                .ToList();
        }

        public List<StatsDTO> GetUserAgeGroupStatistics()
        {
            List<UserEntity> entities = _managerRepo.GetAllUsers();
            List<User> users = _mapper.Map<List<User>>(entities);

            DateTime now = DateTime.Today;

            return users
                .Select(u => new
                {
                    Age = now.Year - u.DateOfBirth.Year - (u.DateOfBirth.Date > now.AddYears(-(now.Year - u.DateOfBirth.Year)) ? 1 : 0)
                })
                .GroupBy(u => GetAgeGroup(u.Age))
                .Select(g => new StatsDTO
                {
                    Label = g.Key, 
                    Count = g.Count()
                })
                .ToList();
        }

        private string GetAgeGroup(int age)
        {
            if (age < 18) return "Under 18";
            if (age <= 25) return "18–25";
            if (age <= 35) return "26–35";
            if (age <= 45) return "36–45";
            if (age <= 60) return "46–60";
            return "60+";
        }
        public List<User> SearchUsers(string query, string gender, string nationality, int? minAge, int? maxAge, DateTime? createdAfter, int page, int pageSize)
        {
            List<UserEntity> userEntities = _managerRepo.SearchUsers(query, gender, nationality, minAge, maxAge, createdAfter, page, pageSize);
            return _mapper.Map<List<User>>(userEntities);
        }

        public List<string> GetAllGenders()
        {
            return _managerRepo.GetAllDistinctGenders();
        }

        public List<string> GetAllNationalities()
        {
            return _managerRepo
                   .GetAllDistinctNationalities()          
                   .AsEnumerable()                        
                   .Where(n => !string.IsNullOrWhiteSpace(n))
                   .Select(n => n.Trim())
                   .Distinct(StringComparer.OrdinalIgnoreCase)
                   .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                   .ToList();
        }

        public int GetFilteredUserCount(string query, string gender, string nationality, int? minAge, int? maxAge, DateTime? createdAfter)
        {
            return _managerRepo.CountFilteredUsers(query, gender, nationality, minAge, maxAge, createdAfter);
        }

        public List<StatsDTO> GetTopUserCombinations()
        {
            var entities = _managerRepo.GetAllUsers();
            var users = _mapper.Map<List<User>>(entities);

            DateTime today = DateTime.Today;

            var result = users
                .Select(u =>
                {
                    int age = today.Year - u.DateOfBirth.Year
                              - (u.DateOfBirth.Date > today.AddYears(-(today.Year - u.DateOfBirth.Year)) ? 1 : 0);

                    string nationality = (u.Nationality ?? string.Empty).Trim();
                    string gender = (u.Gender ?? string.Empty).Trim();

                    return new
                    {
                        Age = age,
                        NormNationality = nationality.ToUpperInvariant(),
                        NormGender = gender.ToUpperInvariant(),
                        DisplayNationality = nationality,
                        DisplayGender = gender
                    };
                })
                .GroupBy(x => new { x.Age, x.NormNationality, x.NormGender })

                .Select(g => new StatsDTO
                {
                    Age = g.Key.Age,
                    Nationality = g.First().DisplayNationality,  
                    Gender = g.First().DisplayGender,
                    Count = g.Count()
                })
                .OrderByDescending(s => s.Count)
                .Take(5)
                .ToList();

            return result;
        }




    }


}


