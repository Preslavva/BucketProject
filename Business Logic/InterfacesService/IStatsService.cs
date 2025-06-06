using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.BLL.Business_Logic.DTOs;

namespace BucketProject.BLL.Business_Logic.InterfacesService;

public interface IStatsService
{
    List<StatsDTO> GetGoalTypeStatistics();
    List<StatsDTO> GetGoalCategoryStatistics();
    List<StatsDTO> GetCompletedGoalsPerWeek();
    List<StatsDTO> GetCompletedGoalsPerMonth();
    List<StatsDTO> GetCompletedGoalsPerYear();
    double GetAverageCompletionTimeInDays();
    StatsDTO GetGoalSummaryStats();
    List<StatsDTO> GetUserRegistrationsPerMonth();
    List<StatsDTO> GetGoalsPerMonth();
    StatsDTO GetGoalSummaryStatsManager();
    List<StatsDTO> GetGoalTypeStatisticsManager();
     List<StatsDTO> GetGoalCategoryStatisticsManager();
  
    List<StatsDTO> GetUserAgeGroupStatistics();
    List<StatsDTO> GetUsersGenderStatistics();
    List<StatsDTO> GetUsersNationalityStatistics();
    List<string> GetAllGenders();
    List<string> GetAllNationalities();
    List<User> SearchUsers(string query, string gender, string nationality, int? minAge, int? maxAge, DateTime? createdAfter, int page, int pageSize);
    int GetFilteredUserCount(string query, string gender, string nationality, int? minAge, int? maxAge, DateTime? createdAfter);
    List<StatsDTO> GetTopUserCombinations();
    string GetWeeklyCompletionRateMessageWeek();
    string GetCompletionRateMessageMonth();
    string GetYearlyCompletionRateMessage();
}
