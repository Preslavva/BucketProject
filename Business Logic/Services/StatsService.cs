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
using BucketProject.DAL.Data.Repositories;
using System.Globalization;
using Org.BouncyCastle.Asn1.X509.SigI;

namespace BucketProject.BLL.Business_Logic.Services
{
    public class StatsService : IStatsService
    {
        private readonly IGoalRepo _goalRepo;
        private readonly IManagerRepo _managerRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;

        public StatsService(IGoalRepo goalRepo, IHttpContextAccessor contextAccessor, IMapper mapper, IManagerRepo managerRepo)
        {
            _goalRepo = goalRepo;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _managerRepo = managerRepo;
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
                .GroupBy(g => g.Nationality)
                .Select(g => new StatsDTO
                {
                    Nationality = g.Key,
                    Count = g.Count()
                })
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
            return _managerRepo.GetAllDistinctNationalities();
        }
        public int GetFilteredUserCount(string query, string gender, string nationality, int? minAge, int? maxAge, DateTime? createdAfter)
        {
            return _managerRepo.CountFilteredUsers(query, gender, nationality, minAge, maxAge, createdAfter);
        }

    }


}


