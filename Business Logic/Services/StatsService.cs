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

        public List<StatsDTO> GetGoalAmountStatistics()
        {
            int id = GetCurrentUserId();
             List<GoalEntity> entitiesPersonal =_goalRepo.LoadPersonalGoalsOfUser(id);
            List<Goal> goalsPersonal = _mapper.Map<List<Goal>>(entitiesPersonal);

            List<GoalEntity> entitiesShared = _goalRepo.LoadSharedGoalsOfUser(id);
            List<Goal> goalsShared = _mapper.Map<List<Goal>>(entitiesShared);

            return new List<StatsDTO>
    {
        new StatsDTO
        {
            Ownership = "Personal",
            Completed = goalsPersonal.Count(g => g.IsDone),
            Incomplete = goalsPersonal.Count(g => !g.IsDone)
        },
        new StatsDTO
        {
            Ownership = "Shared",
            Completed = goalsShared.Count(g => g.IsDone),
            Incomplete = goalsShared.Count(g => !g.IsDone)
        }
    };

        }



    }
}
